using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event.EventData;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.im.sdk.enums;
using Newtonsoft.Json;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;

namespace cn.lds.chatcore.pcw.Services {
public class userlogin {
    public String loginId {
        get;
        set;
    }
    public String password {
        get;
        set;
    }
}

public  class LoginServices {

    private static LoginServices instance = null;

    public static LoginServices getInstance() {
        if (instance == null) {
            instance = new LoginServices();
        }
        return instance;
    }

    bool flag = true;
    Thread pingThread = null;

    public static  void setClientUserInfoToCookie() {
        try {
            ///为OA专门增加用户的cookie
            String hostUrl = ProgramSettingHelper.Host + ":" + ProgramSettingHelper.Port;
            Uri uri = new Uri(hostUrl);
            App.CookieContainer.Add(uri, new Cookie("no", App.AccountsModel.no));
            App.CookieContainer.Add(uri, new Cookie("tenantNo", App.CurrentTenantNo));

            CookieCollection cookieCollection = App.CookieContainer.GetCookies(uri);
            cookieCollection.Add(new Cookie("nickname", App.AccountsModel.nickname));

            ContactsApi.setTenantNo(App.CurrentTenantNo);
            ///为OA专门增加用户的cookie
        } catch (Exception ex) {
            Log.Error(typeof(LoginServices),ex);
        }

    }
    public void DeleteByTenantNo(string tenantNoList) {
        try {
            MasterDao.getInstance().DeleteNotInTenantNo(tenantNoList);
            OrganizationDao.getInstance().DeleteNotInTenantNo(tenantNoList);
            OrganizationMemberDao.getInstance().DeleteNotInTenantNo(tenantNoList);
            PublicAccountsDao.getInstance().DeleteNotInTenantNo(tenantNoList);
            PublicWebDao.getInstance().DeleteNotInTenantNo(tenantNoList);
            ThirdAppClassDao.getInstance().DeleteNotInTenantNo(tenantNoList);
            ThirdAppGroupDao.getInstance().DeleteNotInTenantNo(tenantNoList);
            TodoTaskDao.getInstance().DeleteNotInTenantNo(tenantNoList);
        } catch (Exception e) {
            Log.Error(typeof(LoginServices), e);
        }
    }
    public EventData<Object> LoginPost(string loginId, string password, String mobile, String captcha) {
        try {
            string url = ProgramSettingHelper.Host + ":" + ProgramSettingHelper.Port + ServiceCode.ServerLoginUrl;

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            if (mobile == null || "".Equals(mobile)) {
                parameters.Add("loginId", loginId);
                parameters.Add("password", password);
            } else {
                //parameters.Add("nonceToken", nonceToken);
                parameters.Add("mobile", mobile);
                parameters.Add("captcha", captcha);
            }
            parameters.Add("deviceId", Computer.DeviceId);
            parameters.Add("deviceType", DeviceType.PC.ToStr());
            parameters.Add("osType", OsType.WINDOWS.ToStr());
            parameters.Add("osVersion", Computer.SystemType);
            //parameters.Add("softwareType", this.osType.getValue());
            //parameters.Add("softwareVersion", this.osType.getValue());

            //异步处理，返回
            JObject obj = RestRequestHelper.login(url, parameters);
            if (obj == null) {
                //请求失败处理
                return RestRequestHelper.failureHandler(url, EventDataType.login, obj, null,null);
            }
            JToken jtoken = obj.GetValue("status");
            String status = jtoken.ToString();
            if (!"success".Equals(status)) {
                //请求失败处理
                return RestRequestHelper.failureHandler(url, EventDataType.login, obj, null,null);
            } else {
                string responseContent = obj["data"].ToStr();

                //var jObject = JObject.Parse(responseContent);
                //DataTable table = JsonHelper.JsonToDataTable(jObject.ToStr());
                //App.AccountsModel.loginId = table.Rows[0]["loginId"].ToStr();
                //App.AccountsModel.nickname = table.Rows[0]["nickname"].ToStr();
                //App.AccountsModel.no = table.Rows[0]["no"].ToStr();


                LoginBean loginBean = JsonConvert.DeserializeObject<LoginBean>(responseContent);

                Dictionary<string, LoginBeanTenants> SortedByKey = loginBean.tenants.OrderBy(o => o.Value.sortNum).ToDictionary(p => p.Key, o => o.Value);
                App.TenantNoDic = SortedByKey;

                App.AccountsModel.loginId = loginBean.loginId;
                App.AccountsModel.nickname = loginBean.nickname;
                App.AccountsModel.no = loginBean.no;
                App.AccountsModel.tenants = SortedByKey;
                App.nonceToken = loginBean.nonceToken;



                foreach (LoginBeanTenants tenants in loginBean.tenants.Values) {
                    //下公司头像
                    if (!string.IsNullOrEmpty( tenants.logoID)) {
                        DownloadServices.getInstance().DownloadMethod(tenants.logoID, DownloadType.SYSTEM_IMAGE, null);

                    }
                }

                if (App.AccountsModel.tenants!=null) {

                    KeyValuePair<string, LoginBeanTenants> pair = App.AccountsModel.tenants.First();
                    App.CurrentTenantNo = pair.Key;

                    //App.CurrentTenantNo = "T300000000000";
                    App.TenantNoList = new List<string>(App.AccountsModel.tenants.Keys);
                    //一个租户的话就直接算完成选择租户，否组的话进入租户选择界面
                    if (App.AccountsModel.tenants.Count == 1) {
                        App.CurrentTenantNoLoadOk = true;
                    }
                }



                // 登录成功时，设置登录人的no
                LoginByBarcodeService.getInstance().toClientId = loginBean.no;
                // 一次性登录token
                LoginServices.setClientUserInfoToCookie();
                //登陆成功就ping地址保持状态
                pingThread = new Thread(new ThreadStart(Ping));
                pingThread.IsBackground = true;
                pingThread.Start();

                //请求成功处理
                return RestRequestHelper.successHandler(url, EventDataType.login, obj, null);

            }
        } catch (Exception ex) {

            Log.Error(typeof(LoginServices), ex);
            return null;
        }
    }



    /// <summary>
    /// 连接远端主机
    /// </summary>
    /// <param Name="hostname">主机名称</param>
    /// <param Name="port">端口</param>
    /// <returns>远端主机响应文本</returns>
    public string Connect(string hostname, int port) {
        String url = hostname + ":" + port+"/ping";
        return RestRequestHelper.getSync(url,null,null);

    }


    private void Ping() {
        while (flag) {
            Connect(ProgramSettingHelper.Host,Convert.ToInt32(ProgramSettingHelper.Port));
            // TODO:如果上面的方法没有解决SESSION的问题，可以试试这个
            //ContactsApi.ping();
            Thread.Sleep(20000);
        }
    }

    /// <summary>
    /// 判断是否登录
    /// </summary>
    /// <returns></returns>
    public Boolean IsLogin() {
        //try {
        //    String loginStatus = ">>>>>>>> ";
        //    loginStatus += "App.GroupsLoadOk=" + App.GroupsLoadOk;
        //    loginStatus += ",App.ContactsLoadOk=" + App.ContactsLoadOk;
        //    loginStatus += ",App.OrganizationsLoadOk=" + App.OrganizationsLoadOk;
        //    loginStatus += ",App.PublicAccountsLoadOk=" + App.PublicAccountsLoadOk;

        //    Console.WriteLine(loginStatus);
        //} catch (Exception ex) {
        //    Log.Error(typeof(LoginServices), ex);
        //}
        if (App.GroupsLoadOk && App.ContactsLoadOk
                && App.OrganizationsLoadOk
                && App.PublicAccountsLoadOk
                && App.CacheLoadOk
                && App.ChatSessionLoadOk
                && App.OrganizationsLoadCount==App.TenantNoList.Count
                && App.PublicAccountsLoadCount == App.TenantNoList.Count
                && App.CurrentTenantNo!=string.Empty
                && App.CurrentTenantNoLoadOk
           ) {
            return true;
        } else {
            return false;
        }
    }
}
}
