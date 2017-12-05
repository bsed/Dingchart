using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Common.Utils;
using System.Text;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Services.core;
using System.Threading;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Views.Windows;
using RestSharp.Authenticators;

namespace cn.lds.chatcore.pcw.Common.Services {
class RestRequestHelper {
    // REST请求超时时间
    private static int REST_REQUEST_TIMEOUT = 20000;
    // 是否已经发起重新登录的请求
    private static Boolean IS_RELOGIN = false;

    public static void printHttpRequestLog(String methodType,String url, EventDataType eventDataType, IDictionary<String, Object> param, Dictionary<String, Object> extras) {
        try {
            return;
            Thread t = new Thread(new ThreadStart(() => {
                StringBuilder sb = new StringBuilder();
                sb.Append("HTTP 发起请求【method】" + methodType);
                sb.Append("，【url】" + url);
                if (param != null) {
                    sb.Append("，【param】");
                    foreach (KeyValuePair<string, object> kvp in param) {
                        string conditionCol = kvp.Key;
                        object conditionVal = kvp.Value;
                        sb.Append(conditionCol + "=" + conditionVal + "，");
                    }
                }
                if (extras != null) {
                    sb.Append("，【extras】");
                    foreach (KeyValuePair<string, object> kvp in extras) {
                        string conditionCol = kvp.Key;
                        object conditionVal = kvp.Value;
                        sb.Append(conditionCol + "=" + conditionVal);
                    }
                }
                Console.WriteLine(sb.ToStr());
            }));
            t.IsBackground = true;
            t.Start();

        } catch (Exception e) {
            Log.Error(typeof(RestRequestHelper), e);
        }
    }

    public static void printHttpRequestResultLog(String url, EventDataType eventDataType, JObject obj, Dictionary<String, Object> extras) {
        try {
            return;
            Thread t = new Thread(new ThreadStart(() => {
                StringBuilder sb = new StringBuilder();
                sb.Append("HTTP 返回结果 【url】" + url);
                if (obj != null) {
                    sb.Append("，【obj】" + obj);
                }
                if (extras != null) {
                    sb.Append("，【extras】");
                    foreach (KeyValuePair<string, object> kvp in extras) {
                        string conditionCol = kvp.Key;
                        object conditionVal = kvp.Value;
                        sb.Append(conditionCol + "=" + conditionVal);
                    }
                }
                Console.WriteLine(sb.ToStr());
            }));
            t.IsBackground = true;
            t.Start();

        } catch (Exception e) {
            Log.Error(typeof(RestRequestHelper), e);
        }
    }

    /// <summary>
    /// 请求超时后的重新登录处理
    /// </summary>
    /// <param Name="httpStatusCode"></param>
    private static void ReLoginWhenRequestTimeOut(HttpStatusCode httpStatusCode, String url) {
        try {
            // 如果请求是401或者403错误
            if (httpStatusCode == HttpStatusCode.Unauthorized || httpStatusCode == HttpStatusCode.Forbidden) {
                // 判断是否是登录的请求超时，如果登录请求超时，则直接将程序T出到登录画面
                if (ServiceCode.login.Equals(url)) {
                    Log.Error(typeof(RestRequestHelper), "重登陆超时，退出到登录画面！");
                    // 打开重登陆画面
                    FrameEvent<Object> eventData = new FrameEvent<Object>();
                    eventData.frameEventDataType = FrameEventDataType.LOGOUT_TOKEN_INVALID;
                    EventBusHelper.getInstance().fireEvent(eventData);
                } else {
                    if (IS_RELOGIN) {
                        return;
                    }
                    Log.Error(typeof(RestRequestHelper), "session过期，退出到登录画面！");

                    LoginRequestModel loginRequestModel = new LoginRequestModel();
                    loginRequestModel.loginType = LoginType.nonceToken;
                    loginRequestModel.nonceToken = App.nonceToken;
                    IS_RELOGIN = true;
                    ContactsApi.login(loginRequestModel);
                }
            }

        } catch (Exception e) {
            Log.Error(typeof(RestRequestHelper), e);
        }
    }

    /// <summary>
    /// 重新登录请求成功处理
    /// </summary>
    /// <param Name="httpStatusCode"></param>
    private static void ReLoginComplete(JObject obj) {
        try {
            IS_RELOGIN = false;
            // 处理API返回数据
            if (obj["data"] != null) {
                // 刷token
                LoginBean loginBean = JsonConvert.DeserializeObject<LoginBean>(obj["data"].ToStr());
                App.nonceToken = loginBean.nonceToken;
            }
        } catch (Exception e) {
            Log.Error(typeof(RestRequestHelper), e);
        }
    }

    ///// <summary>
    ///// 取得需要证书认证的httpsClient对象
    ///// </summary>
    ///// <param name="url"></param>
    ///// <returns></returns>
    //public static RestClient GetHttpsClientWithCer(string url) {
    //    // https://stackoverflow.com/questions/40989379/the-request-was-aborted-could-not-create-ssl-tls-secure-channel-restsharp-ss?rq=1
    //    string basicAuthorUserName = "username";
    //    string basicAuthorPassword = "password";
    //    string clientCertificateFilePath = "Path-To-Certificate-File";
    //    string clientCertificatePassword = "certificate-password";
    //    // Importing Certificates
    //    var certificates = new X509Certificate2();
    //    certificates.Import(clientCertificateFilePath, clientCertificatePassword, X509KeyStorageFlags.PersistKeySet);

    //    // Creating RestSharp Client Object
    //    var client = new RestClient {
    //        BaseUrl = new Uri(url),
    //        ClientCertificates = new X509CertificateCollection { certificates },
    //        Authenticator = new HttpBasicAuthenticator(basicAuthorUserName, basicAuthorPassword)
    //    };
    //    client.Timeout = REST_REQUEST_TIMEOUT;
    //    if (App.CookieContainer != null) {
    //        client.CookieContainer = App.CookieContainer;
    //    }

    //    return client;
    //}

    ///// <summary>
    ///// 取得http请求 和 不需要证书认证的https请求
    ///// </summary>
    ///// <param name="url"></param>
    ///// <returns></returns>
    //public static RestClient GetHttpClient(string url) {
    //    // 参考资料：https://jingyan.baidu.com/article/e6c8503c6f5da5e54f1a18c0.html
    //    if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
    //        ServicePointManager.ServerCertificateValidationCallback +=
    //            (sender, certificate, chain, sslPolicyErrors) => true;
    //    }
    //    RestClient client = new RestClient(url);
    //    client.Timeout = REST_REQUEST_TIMEOUT;
    //    if (App.CookieContainer != null) {
    //        client.CookieContainer = App.CookieContainer;
    //    }

    //    return client;
    //}

    /// <summary>
    /// REST get共通
    /// </summary>
    /// <param Name="url"></param>
    /// <param Name="eventDataType"></param>
    /// <param Name="param"></param>
    /// <param Name="extras"></param>
    public static void get(String url, EventDataType eventDataType, IDictionary<String, Object> param, Dictionary<String, Object> extras,bool noHeader) {
        printHttpRequestLog("get", url, eventDataType, param, extras);
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }
        RestClient client = new RestClient(url);
        client.Timeout = REST_REQUEST_TIMEOUT;
        if (App.CookieContainer != null) {
            client.CookieContainer = App.CookieContainer;
        }

        RestRequest request = new RestRequest("", Method.GET);

        if(noHeader==false) {
            // Saas对应，传入租户No
            if (extras != null && extras.ContainsKey("tenantNo")) {

                String tenantNo = extras["tenantNo"].ToStr();
                if (!string.IsNullOrEmpty(tenantNo)) {
                    request.AddHeader("Tenant", tenantNo);
                } else {
                    request.AddHeader("Tenant", App.CurrentTenantNo);
                }
            } else {
                request.AddHeader("Tenant", App.CurrentTenantNo);
            }
        }




        if (param != null) {
            ICollection<String> list = param.Keys;
            foreach (String key in list) {
                request.AddParameter(key, param[key]);
            }
        }

        //IRestResponse _result = client.Execute(request);

        client.ExecuteAsync(request, restResponse => {
            if (restResponse.ErrorException != null) {
                //throw new ApplicationException(message, restResponse.ErrorException);
                Log.Error(typeof(RestRequestHelper),url, restResponse.ErrorException);
                //请求失败处理
                failureHandler(url, eventDataType, null, extras, restResponse);
            }

            if (restResponse.StatusCode.Equals(HttpStatusCode.OK)) {
                if ("pang".Equals(restResponse.Content)) {
                    return;
                }
                //异步处理，返回
                JObject obj = JObject.Parse(restResponse.Content);
                JToken jtoken = obj.GetValue("status");
                String status = jtoken.ToString();
                if (!"success".Equals(status)) {
                    //请求失败处理
                    failureHandler(url, eventDataType, obj, extras, restResponse);
                } else {
                    //EventData<Object> data = new EventData<Object>();

                    //data.eventType = EventType.HttpRequest;
                    //data.eventDataType = eventDataType;
                    //if (obj["data"] != null)
                    //{
                    //    string responseContent = obj["data"].ToString();
                    //    data.data = responseContent;
                    //}
                    //EventBusHelper.getInstance().fireEvent(data);
                    // 请求成功处理
                    successHandler(url, eventDataType, obj, extras);
                }

            } else {
                //请求失败处理
                failureHandler(url, eventDataType, null, extras, restResponse);
            }
        });
    }



    public static String postSync(String url, IDictionary<String, Object> param, Object body, Dictionary<String, Object> extras) {
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }
        RestClient client = new RestClient(url);
        client.Timeout = REST_REQUEST_TIMEOUT;
        if (App.CookieContainer != null) {
            client.CookieContainer = App.CookieContainer;
        }

        RestRequest request = new RestRequest("", Method.POST);
        //Saas对应，传入租户No
        if (extras != null && extras.ContainsKey("tenantNo")) {

            String tenantNo = extras["tenantNo"].ToStr();
            if (!string.IsNullOrEmpty(tenantNo)) {
                request.AddHeader("Tenant", tenantNo);
            } else {
                request.AddHeader("Tenant", App.CurrentTenantNo);
            }
        } else {
            request.AddHeader("Tenant", App.CurrentTenantNo);
        }
        var json = request.JsonSerializer.Serialize(body);

        request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
        request.RequestFormat = DataFormat.Json;

        if (param != null) {
            ICollection<String> list = param.Keys;
            foreach (String key in list) {
                request.AddParameter(key, param[key]);
            }
        }

        IRestResponse _result = client.Execute(request);
        if (_result.ErrorException != null) {
            //throw new ApplicationException(message, restResponse.ErrorException);
            Log.Error(typeof(RestRequestHelper), url,_result.ErrorException);
            return null;
        }
        if (_result.StatusCode.Equals(HttpStatusCode.OK)) {
            //异步处理，返回
            JObject obj = JObject.Parse(_result.Content);
            JToken jtoken = obj.GetValue("status");
            String status = jtoken.ToString();
            if (!"success".Equals(status)) {
                return null;
            } else {
                return obj["data"].ToStr();
            }
            //
        } else {
            //请求失败处理
            return null;
        }
    }

    public static String getSync(String url, IDictionary<String, Object> param,  Dictionary<String, Object> extras) {
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }
        RestClient client = new RestClient(url);
        client.Timeout = REST_REQUEST_TIMEOUT;
        if (App.CookieContainer != null) {
            client.CookieContainer = App.CookieContainer;
        }

        RestRequest request = new RestRequest("", Method.GET);
        //Saas对应，传入租户No
        if (extras != null && extras.ContainsKey("tenantNo")) {

            String tenantNo = extras["tenantNo"].ToStr();
            if (!string.IsNullOrEmpty(tenantNo)) {
                request.AddHeader("Tenant", tenantNo);
            } else {
                request.AddHeader("Tenant", App.CurrentTenantNo);
            }
        } else {
            request.AddHeader("Tenant", App.CurrentTenantNo);
        }
        //var json = request.JsonSerializer.Serialize(body);

        //request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
        //request.RequestFormat = DataFormat.Json;

        if (param != null) {
            ICollection<String> list = param.Keys;
            foreach (String key in list) {
                request.AddParameter(key, param[key]);
            }
        }

        IRestResponse _result = client.Execute(request);
        if (_result.ErrorException != null) {
            //throw new ApplicationException(message, restResponse.ErrorException);
            if (_result != null) {
                ReLoginWhenRequestTimeOut(_result.StatusCode, url);
            }
            Log.Error(typeof(RestRequestHelper),url, _result.ErrorException);
            return null;
        }

        if (_result.StatusCode.Equals(HttpStatusCode.OK)) {
            //异步处理，返回
            try {
                if ("pang".Equals(_result.Content)) {
                    return null;
                }
                JObject obj = JObject.Parse(_result.Content);
                JToken jtoken = obj.GetValue("status");
                String status = jtoken.ToString();
                if (!"success".Equals(status)) {
                    return null;
                } else {
                    return obj["data"].ToStr();
                }
            } catch (Exception e) {
                Log.Error(typeof(RestRequestHelper), e);
                //return _result.Content;
                return null;
            }
            //
        } else {
            //请求失败处理
            if (_result != null) {
                ReLoginWhenRequestTimeOut(_result.StatusCode, url);
            }
            return null;
        }
    }



    /// <summary>
    /// REST post共通
    /// </summary>
    /// <param Name="url"></param>
    /// <param Name="eventDataType"></param>
    /// <param Name="param"></param>
    /// <param Name="body"></param>
    /// <param Name="extras"></param>
    public static void post(String url, EventDataType eventDataType, IDictionary<String, Object> param, Object body, Dictionary<String, Object> extras) {
        printHttpRequestLog("post", url, eventDataType, param, extras);

        if (extras != null && extras.ContainsKey("showLoading")) {
            BusinessEvent<object> Businessdata = new BusinessEvent<object>();
            Businessdata.data = eventDataType.ToStr();
            Businessdata.eventDataType = BusinessEventDataType.LoadingWaitShow;
            EventBusHelper.getInstance().fireEvent(Businessdata);
        }

        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }
        RestClient client = new RestClient(url);
        client.Timeout = REST_REQUEST_TIMEOUT;
        if (App.CookieContainer != null) {
            client.CookieContainer = App.CookieContainer;
        }


        RestRequest request = new RestRequest("", Method.POST);
        //Saas对应，传入租户No
        if (extras != null && extras.ContainsKey("tenantNo")) {

            String tenantNo = extras["tenantNo"].ToStr();
            if (!string.IsNullOrEmpty(tenantNo)) {
                request.AddHeader("Tenant", tenantNo);
            } else {
                request.AddHeader("Tenant", App.CurrentTenantNo);
            }
        } else {
            request.AddHeader("Tenant", App.CurrentTenantNo);
        }
        var json = request.JsonSerializer.Serialize(body);

        request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
        request.RequestFormat = DataFormat.Json;

        if (param != null) {
            ICollection<String> list = param.Keys;
            foreach (String key in list) {
                request.AddParameter(key, param[key]);
            }
        }



        client.ExecuteAsync(request, restResponse => {
            if (restResponse.ErrorException != null) {
                //throw new ApplicationException(message, restResponse.ErrorException);
                Log.Error(typeof(RestRequestHelper),url, restResponse.ErrorException);
                //请求失败处理
                failureHandler(url, eventDataType, null, extras, restResponse);
            }
            if (restResponse.StatusCode.Equals(HttpStatusCode.OK)) {
                //异步处理，返回
                JObject obj = JObject.Parse(restResponse.Content);
                JToken jtoken = obj.GetValue("status");
                String status = jtoken.ToString();
                if (!"success".Equals(status)) {
                    //请求失败处理
                    failureHandler(url, eventDataType, obj, extras, restResponse);
                } else {
                    if (eventDataType == EventDataType.login) {
                        //设置HttpWebRequest的CookieContainer为刚才建立的那个myCookieContainer

                        App.CookieContainer = client.CookieContainer;
                    }

                    // 判断是否是重新登录请求成功
                    if (ServiceCode.login.Equals(url)) {
                        ReLoginComplete(obj);
                    } else {
                        // 请求成功处理
                        successHandler(url, eventDataType, obj, extras);
                    }

                }
                //
            } else {
                //请求失败处理
                failureHandler(url, eventDataType, null, extras, restResponse);
            }
        });



    }

    /// <summary>
    /// REST put共通
    /// </summary>
    /// <param Name="url"></param>
    /// <param Name="eventDataType"></param>
    /// <param Name="param"></param>
    /// <param Name="body"></param>
    /// <param Name="extras"></param>
    public static void put(String url, EventDataType eventDataType, IDictionary<String, Object> param, Object body, Dictionary<String, Object> extras) {
        printHttpRequestLog("put", url, eventDataType, param, extras);
        if (extras != null && extras.ContainsKey("showLoading")) {
            BusinessEvent<object> businessdata = new BusinessEvent<object>();
            businessdata.data = eventDataType.ToStr();
            businessdata.eventDataType = BusinessEventDataType.LoadingWaitShow;
            EventBusHelper.getInstance().fireEvent(businessdata);
        }
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }
        RestClient client = new RestClient(url);
        client.Timeout = REST_REQUEST_TIMEOUT;
        if (App.CookieContainer != null) {
            client.CookieContainer = App.CookieContainer;
        }

        RestRequest request = new RestRequest("", Method.PUT);
        //Saas对应，传入租户No
        if (extras != null && extras.ContainsKey("tenantNo")) {

            String tenantNo = extras["tenantNo"].ToStr();
            if (!string.IsNullOrEmpty(tenantNo)) {
                request.AddHeader("Tenant", tenantNo);
            } else {
                request.AddHeader("Tenant", App.CurrentTenantNo);
            }
        } else {
            request.AddHeader("Tenant", App.CurrentTenantNo);
        }
        var json = request.JsonSerializer.Serialize(body);

        request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
        request.RequestFormat = DataFormat.Json;

        if (param != null) {
            ICollection<String> list = param.Keys;
            foreach (String key in list) {
                request.AddParameter(key, param[key]);
            }
        }

        //IRestResponse _result = client.Execute(request);

        client.ExecuteAsync(request, restResponse => {
            if (restResponse.ErrorException != null) {
                const string message = "Error retrieving response.";
                //throw new ApplicationException(message, restResponse.ErrorException);
                Log.Error(typeof(RestRequestHelper),url, restResponse.ErrorException);
                //请求失败处理
                failureHandler(url, eventDataType, null, extras, restResponse);
            }

            if (restResponse.StatusCode.Equals(HttpStatusCode.OK)) {
                //异步处理，返回
                JObject obj = JObject.Parse(restResponse.Content);
                JToken jtoken = obj.GetValue("status");
                String status = jtoken.ToString();
                if (!"success".Equals(status)) {
                    //请求失败处理
                    failureHandler(url, eventDataType, obj, extras, restResponse);
                } else {
                    if (eventDataType == EventDataType.login) {
                        //设置HttpWebRequest的CookieContainer为刚才建立的那个myCookieContainer

                        App.CookieContainer = client.CookieContainer;
                    }

                    //EventData<Object> data = new EventData<Object>();

                    //data.eventType = EventType.HttpRequest;
                    //data.eventDataType = eventDataType;
                    //if (obj["data"] != null)
                    //{
                    //    string responseContent = obj["data"].ToString();
                    //    data.data = responseContent;
                    //}
                    //EventBusHelper.getInstance().fireEvent(data);
                    // 请求成功处理
                    successHandler(url, eventDataType, obj, extras);
                }
                //
            } else {
                //请求失败处理
                failureHandler(url, eventDataType, null, extras, restResponse);
            }

        });


    }

    /// <summary>
    /// REST delete共通
    /// </summary>
    /// <param Name="url"></param>
    /// <param Name="eventDataType"></param>
    /// <param Name="param"></param>
    /// <param Name="extras"></param>
    public static void delete(String url, EventDataType eventDataType, IDictionary<String, Object> param, Object body, Dictionary<String, Object> extras) {
        printHttpRequestLog("delete", url, eventDataType, param, extras);
        if (extras != null && extras.ContainsKey("showLoading")) {
            BusinessEvent<object> Businessdata = new BusinessEvent<object>();
            Businessdata.data = eventDataType.ToStr();
            Businessdata.eventDataType = BusinessEventDataType.LoadingWaitShow;
            EventBusHelper.getInstance().fireEvent(Businessdata);
        }
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }
        RestClient client = new RestClient(url);
        client.Timeout = REST_REQUEST_TIMEOUT;
        if (App.CookieContainer != null) {
            client.CookieContainer = App.CookieContainer;
        }

        RestRequest request = new RestRequest("", Method.DELETE);
        //Saas对应，传入租户No
        if (extras != null && extras.ContainsKey("tenantNo")) {

            String tenantNo = extras["tenantNo"].ToStr();
            if (!string.IsNullOrEmpty(tenantNo)) {
                request.AddHeader("Tenant", tenantNo);
            } else {
                request.AddHeader("Tenant", App.CurrentTenantNo);
            }
        } else {
            request.AddHeader("Tenant", App.CurrentTenantNo);
        }
        var json = request.JsonSerializer.Serialize(body);

        request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
        request.RequestFormat = DataFormat.Json;

        if (param != null) {
            ICollection<String> list = param.Keys;
            foreach (String key in list) {
                request.AddParameter(key, param[key]);
            }
        }

        // IRestResponse _result = client.Execute(request);


        client.ExecuteAsync(request, restResponse => {
            if (restResponse.ErrorException != null) {
                //throw new ApplicationException(message, restResponse.ErrorException);
                Log.Error(typeof(RestRequestHelper),url, restResponse.ErrorException);
                //请求失败处理
                failureHandler(url, eventDataType, null, extras, restResponse);
            }

            if (restResponse.StatusCode.Equals(HttpStatusCode.OK)) {
                //异步处理，返回
                JObject obj = JObject.Parse(restResponse.Content);
                JToken jtoken = obj.GetValue("status");
                String status = jtoken.ToString();
                if (!"success".Equals(status)) {
                    //请求失败处理
                    failureHandler(url, eventDataType, obj, extras, restResponse);
                } else {
                    //EventData<Object> data = new EventData<Object>();

                    //data.eventType = EventType.HttpRequest;
                    //data.eventDataType = eventDataType;
                    //if (obj["data"] != null)
                    //{
                    //    string responseContent = obj["data"].ToString();
                    //    data.data = responseContent;
                    //}
                    //EventBusHelper.getInstance().fireEvent(data);
                    //请求成功处理
                    successHandler(url, eventDataType, obj, extras);

                }

            } else {
                //请求失败处理
                failureHandler(url, eventDataType, null, extras, restResponse);
            }
        });
    }

    /// <summary>
    /// API请求成功处理
    /// </summary>
    /// <param Name="eventDataType"></param>
    /// <param Name="obj"></param>
    public static EventData<Object> successHandler(String url, EventDataType eventDataType, JObject obj, Dictionary<String, Object> extras) {
        printHttpRequestResultLog(url, eventDataType, obj, extras);

        if (extras != null && extras.ContainsKey("showLoading")) {
            BusinessEvent<object> Businessdata = new BusinessEvent<object>();
            Businessdata.data = eventDataType.ToStr();
            Businessdata.eventDataType = BusinessEventDataType.LoadingWaitClose;
            EventBusHelper.getInstance().fireEvent(Businessdata);
            //Console.WriteLine("successHandler"+Businessdata.data.ToStr());
        }
        EventData<Object> data = new EventData<Object>();

        data.eventType = EventType.HttpRequest;
        data.eventDataType = eventDataType;
        data.extras = extras;
        // 处理API返回数据
        if (obj["data"] != null) {
            string responseContentData = obj["data"].ToStr();
            data.data = responseContentData;
        }
        // 处理时间戳
        if (obj["Timestamp"] != null) {
            long responseContentTimestamp = long.Parse(obj["Timestamp"].ToStr());
            data.timestamp = responseContentTimestamp;
        }

        //var task1 = new Task(() => {
        //    if (App.LoadingWait != null && App.LoadingWait.eventDataType == eventDataType.ToStr()) {
        //        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        //        Businessdata.data = eventDataType.ToStr();
        //        Businessdata.eventDataType = BusinessEventDataType.LoadingWaitClose;
        //        EventBusHelper.getInstance().fireEvent(Businessdata);
        //    }
        //});

        //task1.Start();

        EventBusHelper.getInstance().fireEvent(data);
        return data;
    }

    /// <summary>
    /// API请求成功处理
    /// </summary>
    /// <param Name="eventDataType"></param>
    /// <param Name="obj"></param>
    public static EventData<Object> failureHandler(String url, EventDataType eventDataType, JObject obj, Dictionary<String, Object> extras,IRestResponse iResponse) {
        // API请求失败时，判断是否需要重新登录
        if (iResponse!=null) {
            ReLoginWhenRequestTimeOut(iResponse.StatusCode,url);
        }

        if (extras != null && extras.ContainsKey("showLoading")) {
            BusinessEvent<object> Businessdata = new BusinessEvent<object>();
            Businessdata.data = eventDataType.ToStr();
            Businessdata.eventDataType = BusinessEventDataType.LoadingWaitClose;
            EventBusHelper.getInstance().fireEvent(Businessdata);
            //Console.WriteLine("failureHandler" + Businessdata.data.ToStr());
        }
        printHttpRequestResultLog(url, eventDataType, obj, extras);
        EventData<Object> data = new EventData<Object>();

        data.eventType = EventType.HttpRequestError;
        data.eventDataType = eventDataType;
        data.extras = extras;
        // 如果obj为null，代表前方法的_result.StatusCode.Equals(HttpStatusCode.OK)判断失败
        if (obj == null) {
            RestRequestError restRequestError = new RestRequestError();
            restRequestError.errcode = TostMessage.MSG_E9999_CODE;
            restRequestError.errmsg = TostMessage.MSG_E9999;
            List<RestRequestError> restRequestErrorList = new List<RestRequestError>();
            restRequestErrorList.Add(restRequestError);
            data.errors = restRequestErrorList;
        } else {
            // 处理API返回数据
            if (obj["data"] != null) {
                string responseContentData = obj["data"].ToStr();
                data.data = responseContentData;
            }
            // 处理API请求返回的错误信息
            if (obj["errors"] != null) {
                string responseContentErrors = obj["errors"].ToStr();
                List<RestRequestError> restRequestErrorList = JsonConvert.DeserializeObject<List<RestRequestError>>(responseContentErrors, new convertor<RestRequestError>());
                data.errors = restRequestErrorList;
            }
        }


        //if (App.LoadingWait != null && App.LoadingWait.eventDataType==eventDataType.ToStr()) {

        //    BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        //    Businessdata.data = eventDataType.ToStr();
        //    Businessdata.eventDataType = BusinessEventDataType.LoadingWaitClose;
        //    EventBusHelper.getInstance().fireEvent(Businessdata);


        //}
        EventBusHelper.getInstance().fireEvent(data);
        return data;
    }

    /// <summary>
    /// 登录处理
    /// </summary>
    /// <param Name="url"></param>
    /// <param Name="body"></param>
    /// <returns></returns>
    public static JObject login(String url, Object body) {
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }
        RestClient client = new RestClient(url);
        client.Timeout = REST_REQUEST_TIMEOUT;
        if (App.CookieContainer != null) {
            client.CookieContainer = App.CookieContainer;
        }

        RestRequest request = new RestRequest("", Method.POST);

        var json = request.JsonSerializer.Serialize(body);

        request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
        request.RequestFormat = DataFormat.Json;

        IRestResponse _result = client.Execute(request);

        if (_result.ErrorException != null) {
            //todo 登录超时异常，需要处理界面提示
            //throw new ApplicationException(message, restResponse.ErrorException);
            Log.Error(typeof(RestRequestHelper),url, _result.ErrorException);
            return null;
        }

        if (_result.StatusCode.Equals(HttpStatusCode.OK)) {
            //异步处理，返回
            JObject obj = JObject.Parse(_result.Content);
            App.CookieContainer = client.CookieContainer;
            return obj;
        }
        return null;
    }
}
}
