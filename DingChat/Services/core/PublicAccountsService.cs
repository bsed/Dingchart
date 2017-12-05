using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Common.Utils;
using EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Services;
using ikvm.extensions;


namespace cn.lds.chatcore.pcw.Services.core {
class PublicAccountsService : BaseService {
    private static PublicAccountsService instance = null;
    public static PublicAccountsService getInstance() {
        if (instance == null) {
            instance = new PublicAccountsService();
        }
        return instance;
    }

    /// <summary>
    /// 订阅：公众号变更消息
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {
        try {
            // 获取消息类型
            Message message = messageArrivedEvent.message;
            MsgType msgType = message.getType();
            switch (msgType) {
            //公众号变更消息
            case MsgType.PublicListChanged:
                processPublicListChangedMessage(message);
                break;
            default:
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }

    /// <summary>
    /// API请求处理
    /// P001: 查找开放平台服务帐号 (根据名称模糊匹配) searchServiceAccounts --不在此处理
    /// P002: 查看开放平台服务帐号详情 getServiceAccountDetails --不在此处理
    /// P003: 获取开放平台服务帐号包含的公众帐号 getServiceAccountSubscription
    /// P004: 获取开放平台服务帐号包含的网站应用 getServiceAccountWebsite
    /// P005: 获取个人使用的公众帐号服务列表 listMySubscriptions
    /// P006: 获取个人使用的网站应用服务列表 listMyWebsites
    /// P007: 关注开放平台服务帐号包含的公众帐号 followSubscription
    /// P008: 取消关注开放平台服务帐号包含的公众帐号 unfollowSubscription
    /// P009: 授权开放平台服务帐号包含的网站应用 authorizeWebsite
    /// P010: 取消授权开放平台服务帐号包含的网站应用 unauthorizeWebsite
    /// P011: 点击开放平台服务包含的公众帐号的自定义菜单项 clickSubscriptionMenu  --不在此处理
    /// P012:设置公众号置顶 enableSubscriptionTopmost
    /// P013:设置公众号免打扰 allowReceiveSubscriptionMessages
    /// </summary>
    /// <param Name="eventData"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        switch (eventData.eventDataType) {
        // P003: 获取开放平台服务帐号包含的公众帐号 getServiceAccountSubscription
        case EventDataType.getServiceAccountSubscription:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P003(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // P004: 获取开放平台服务帐号包含的网站应用 getServiceAccountWebsite
        case EventDataType.getServiceAccountWebsite:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P004(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // P005: 获取个人使用的公众帐号服务列表 listMySubscriptions
        case EventDataType.listMySubscriptions:

            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P005(eventData);
            }
            // API请求失败
            else {

                P005_ERROR(eventData);
            }
            this.MarkDataLoadComplete(eventData);
            break;
        // P006: 获取个人使用的网站应用服务列表 listMyWebsites
        case EventDataType.listMyWebsites:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P006(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // P007: 关注开放平台服务帐号包含的公众帐号 followSubscription
        case EventDataType.followSubscription:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P007(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // P008: 取消关注开放平台服务帐号包含的公众帐号 unfollowSubscription
        case EventDataType.unfollowSubscription:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P008(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // P009: 授权开放平台服务帐号包含的网站应用 authorizeWebsite
        case EventDataType.authorizeWebsite:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P009(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // P010: 取消授权开放平台服务帐号包含的网站应用 unauthorizeWebsite
        case EventDataType.unauthorizeWebsite:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P010(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // P012:设置公众号置顶 enableSubscriptionTopmost
        case EventDataType.enableSubscriptionTopmost:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P012(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // P013:设置公众号免打扰 allowReceiveSubscriptionMessages
        case EventDataType.allowReceiveSubscriptionMessages:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P013(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // P014:查找公众号
        case EventDataType.searchServiceAccounts:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P014(eventData);
            }
            // API请求失败
            else {

            }
            break;

        case EventDataType.getServiceAccountDetails:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                P015(eventData);
            }
            // API请求失败
            else {

            }
            break;

        default:
            break;
        }

    }

    /// <summary>
    /// P003: 获取开放平台服务帐号包含的公众帐号 getServiceAccountSubscription
    /// </summary>
    /// <param Name="eventData"></param>
    private void P003(EventData<Object> eventData) {
        try {
            Object data = eventData.data;
            //List<PublicAccountsBean> list = JsonConvert.DeserializeObject<List<PublicAccountsBean>>(data.ToString(), new convertor<PublicAccountsBean>());
            //foreach (PublicAccountsBean publicAccountsBean in list) {

            //}
            String appId = eventData.extras["appId"].ToString();
            PublicAccountsTable table = PublicAccountsDao.getInstance().findByAppid(appId);
            if (table!=null) {
                PublicAccountsBean publicAccountsBean = JsonConvert.DeserializeObject<PublicAccountsBean>(data.ToString());
                table.menu = JsonConvert.SerializeObject(publicAccountsBean.menus);
                table.location = publicAccountsBean.enableLocationReporting;
                PublicAccountsDao.getInstance().save(table);



                // 发出事件通知
                BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                businessEvent.data = publicAccountsBean;
                businessEvent.eventDataType = BusinessEventDataType.PublicAccountAvaliableEvent;
                EventBusHelper.getInstance().fireEvent(businessEvent);
            }

        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }
    /// <summary>
    /// P004: 获取开放平台服务帐号包含的网站应用 getServiceAccountWebsite
    /// </summary>
    /// <param Name="eventData"></param>
    private void P004(EventData<Object> eventData) {
        try {

        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }
    /// <summary>
    /// P005: 获取个人使用的公众帐号服务列表 listMySubscriptions
    /// </summary>
    /// <param Name="eventData"></param>
    private void P005(EventData<Object> eventData) {
        try {

            List<String> list_app = new List<String>();//记录当前公众号 用于禁用功能
            long timestamp = eventData.timestamp;
            List<PublicAccountsBean> list = JsonConvert.DeserializeObject<List<PublicAccountsBean>>(eventData.data.ToString(), new convertor<PublicAccountsBean>());
            String tenantNo = eventData.extras["tenantNo"].ToStr();
            foreach (PublicAccountsBean publicAccountsBean in list) {
                try {

                    PublicAccountsTable table = PublicAccountsDao.getInstance().findByAppid(publicAccountsBean.appId);
                    if (null == table) {
                        table = new PublicAccountsTable();
                    }

                    table.appid = publicAccountsBean.appId;
                    list_app.Add(publicAccountsBean.appId);
                    table.userno = publicAccountsBean.userNo;
                    table.name = publicAccountsBean.name;
                    table.logoId = publicAccountsBean.logoId;
                    table.introduction = publicAccountsBean.introduction;
                    table.status = publicAccountsBean.status;
                    table.ownerName = publicAccountsBean.ownerName;
                    table.menu = "";
                    table.tenantNo = tenantNo;
                    PublicAccountsDao.getInstance().save(table);

                    // 下载头像
                    DownloadServices.getInstance().DownloadMethod(table.logoId, DownloadType.SYSTEM_IMAGE, null);

                    // 发出事件通知  暂时屏蔽 因为没有参数界面当做重新加载好友
                    //BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                    //businessEvent.eventDataType = BusinessEventDataType.ContactsChangedEvent;
                    //EventBusHelper.getInstance().fireEvent(businessEvent);

                    //异步拉取菜单
                    this.requestMenu(table.appid, tenantNo);
                } catch (Exception e) {
                    Log.Error(typeof(PublicAccountsService), e);
                }
            }

            this.setSatus(list_app);

            // 更新时间戳
            TimeServices.getInstance().SaveTime(TimestampType.PUBLIC, timestamp, tenantNo);

            String messageId = null;
            if (eventData.extras != null && eventData.extras.ContainsKey("messageId")) {
                messageId = eventData.extras["messageId"].ToString();
                MessageService.getInstance().processMessageAfterPublicAvaliable(messageId);
            }

            FirePublicAccountsLoadOk();

            // 通知通讯录画面更新
            BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
            Businessdata.eventDataType = BusinessEventDataType.PublicAccountChangedEvent;
            EventBusHelper.getInstance().fireEvent(Businessdata);


        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }

    }

    /// <summary>
    /// P005_ERROR: 获取个人使用的公众帐号服务列表 listMySubscriptions请求失败
    /// </summary>
    /// <param Name="eventData"></param>
    private void P005_ERROR(EventData<Object> eventData) {
        try {
            // 如果是初次登录，则继续请求API
            if (App.IsFirstLogin) {
                String tenantNo = eventData.extras["tenantNo"].ToStr();
                Dictionary<String, Object> extras = new Dictionary<String, Object>();
                extras.Add("tenantNo", tenantNo);
                this.request(extras);
            } else {
                // 否则标识加载完成，交由DataPullService来处理关键数据同步问题
                this.FirePublicAccountsLoadOk();
            }
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }

    }

    /// <summary>
    ///  触发公众号拉取玩出事件
    /// </summary>
    private void FirePublicAccountsLoadOk() {
        try {
            App.PublicAccountsLoadOk = true;

            if(App.PublicAccountsLoadCount<App.TenantNoList.Count) {
                App.PublicAccountsLoadCount++;
            }

            BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
            Businessdata.eventDataType = BusinessEventDataType.LoadingOk;
            EventBusHelper.getInstance().fireEvent(Businessdata);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }

    /// <summary>
    /// 修改状态
    /// </summary>
    /// <param Name="list_app"></param>
    public void setSatus(List<String> list_app) {

        if (null == list_app || list_app.Count < 1) {
            // TODO:后续好好整理下逻辑，
            // 靠，谁能告诉我这到底啥逻辑
            // 如果没有返回公众号，则全部本地公众号都被禁用了
            List<PublicAccountsTable> tables = null;
            try {
                // 获取全部的
                tables = PublicAccountsDao.getInstance().FindAll("");
            } catch (Exception e) {
                Log.Error(typeof(PublicAccountsService), e);
            }
            if (null == tables) {
                return;
            }
            foreach (PublicAccountsTable t in tables) {
                t.status = "1";
                try {
                    PublicAccountsDao.getInstance().save(t);
                } catch (Exception e) {
                    Log.Error(typeof(PublicAccountsService), e);
                }
            }
            return;
        }
        String s = "";
        for (int i = 0; i < list_app.Count; i++) {
            if (0 == i) {
                s = s + "'" + list_app[i] + "'";
            } else {
                s = s + ",'" + list_app[i] + "'";
            }
        }

        List<PublicAccountsTable> lstTables = null;
        try {
            lstTables = PublicAccountsDao.getInstance().FindAll(" and appid not in (" + s + ")");
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
        if (null == lstTables) {
            return;
        }
        foreach (PublicAccountsTable t in lstTables) {
            t.status = "1";
            try {
                PublicAccountsDao.getInstance().save(t);
            } catch (Exception e) {
                Log.Error(typeof(PublicAccountsService), e);
            }
        }

    }
    /// <summary>
    /// P006: 获取个人使用的网站应用服务列表 listMyWebsites
    /// </summary>
    /// <param Name="eventData"></param>
    private void P006(EventData<Object> eventData) {
        try {

        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }
    /// <summary>
    /// P007: 关注开放平台服务帐号包含的公众帐号 followSubscription
    /// </summary>
    /// <param Name="eventData"></param>
    private void P007(EventData<Object> eventData) {
        try {
            String tenantNo = eventData.extras["tenantNo"].ToStr();
            String appId = eventData.extras["appId"].ToString();


            if (App.TenantNoList.Count > 0) {
                foreach (string no in App.TenantNoList) {
                    Dictionary<String, Object> extras = new Dictionary<String, Object>();
                    extras.Add("tenantNo", no);
                    request(extras);
                }
            } else {
                request(null);
            }
            // 拉取菜单
            //ContactsApi.getServiceAccountSubscription(appId);
            this.requestMenu(appId, tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }
    /// <summary>
    /// P008: 取消关注开放平台服务帐号包含的公众帐号 unfollowSubscription
    /// </summary>
    /// <param Name="eventData"></param>
    private void P008(EventData<Object> eventData) {
        try {
            String appId = eventData.extras["appId"].ToString();
            //清空消息表
            MessageService.getInstance().clearMessages(appId);
            //删除会话表
            ChatSessionDao.getInstance().deleteByNo(appId);
            //删除公众号实体
            PublicAccountsDao.getInstance().deleteByAppId(appId);

            //发送删除完成事件
            //EventBus.getDefault().post(new PublicAccountRemovedEvent(appId));
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = appId;
            businessEvent.eventDataType = BusinessEventDataType.PublicAccountRemovedEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }
    /// <summary>
    /// P009: 授权开放平台服务帐号包含的网站应用 authorizeWebsite
    /// </summary>
    /// <param Name="eventData"></param>
    private void P009(EventData<Object> eventData) {
        try {

        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }
    /// <summary>
    /// P010: 取消授权开放平台服务帐号包含的网站应用 unauthorizeWebsite
    /// </summary>
    /// <param Name="eventData"></param>
    private void P010(EventData<Object> eventData) {
        try {

        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }
    /// <summary>
    /// P012:设置公众号置顶 enableSubscriptionTopmost
    /// </summary>
    /// <param Name="eventData"></param>
    private void P012(EventData<Object> eventData) {
        try {
            String appId = eventData.extras["appId"].ToStr();
            Boolean enableFlag = eventData.extras["enableFlag"].ToStr().ToBool();
            SettingService.getInstance().setTop(appId, enableFlag);

            //EventBus.getDefault().post(new PublicAccountFreshenViewEvent(appId));
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = appId;
            businessEvent.eventDataType = BusinessEventDataType.PublicAccountFreshenViewEvent;
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }
    /// <summary>
    /// P013:设置公众号免打扰 allowReceiveSubscriptionMessages
    /// </summary>
    /// <param Name="eventData"></param>
    private void P013(EventData<Object> eventData) {
        try {
            String appId = eventData.extras["appId"].ToStr();
            Boolean allowFlag = eventData.extras["allowFlag"].ToStr().ToBool();
            SettingService.getInstance().setQuiet(appId, allowFlag);

            //EventBus.getDefault().post(new PublicAccountFreshenViewEvent(appId));
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = appId;
            businessEvent.eventDataType = BusinessEventDataType.PublicAccountFreshenViewEvent;
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }

    private void P014(EventData<Object> eventData) {
        try {

            List<PublicAccountsBean> list =
                JsonConvert.DeserializeObject<List<PublicAccountsBean>>(eventData.data.ToString(),
                        new convertor<PublicAccountsBean>());

            foreach(PublicAccountsBean bean in list) {
                // 下载头像
                DownloadServices.getInstance().DownloadMethod(bean.logoId, DownloadType.SYSTEM_IMAGE, null);
            }


            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = list;
            businessEvent.eventDataType = BusinessEventDataType.PublicAccountFindEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }
    private void P015(EventData<Object> eventData) {
        try {

            PublicAccountsBean publicAccountsBean = JsonConvert.DeserializeObject<PublicAccountsBean>(eventData.data.ToString());
            // 下载头像
            DownloadServices.getInstance().DownloadMethod(publicAccountsBean.logoId, DownloadType.SYSTEM_IMAGE, null);

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = publicAccountsBean;
            businessEvent.eventDataType = BusinessEventDataType.PublicAccountDetailedEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }
    /// <summary>
    /// 公众号变更消息
    /// </summary>
    /// <param Name="message"></param>
    private
    void processPublicListChangedMessage(Message message) {
        try {
            // 重新拉取数据

            if (App.TenantNoList.Count > 0) {
                foreach (string tenantNo in App.TenantNoList) {
                    Dictionary<String, Object> extras = new Dictionary<String, Object>();
                    extras.Add("tenantNo", tenantNo);
                    this.request(extras);
                }
            } else {
                this.request(null);
            }

        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
    }


    /**
     * 请求开放平台服务帐号详情
     *
     * @param appid
     */
    public void getServiceAccountDetails(String appid,string tenantNo) {
        ContactsApi.getServiceAccountDetails(appid, tenantNo);
    }


    /// <summary>
    /// 系统初始化时，向后台同步已订阅公众号。
    /// ※ 不能再其它业务模块显示调用
    /// </summary>
    public void request(Dictionary<String, Object> extras) {
        if(extras==null) {
            extras = new Dictionary<string, object>();
        }

        String tenantNo = extras["tenantNo"].ToStr();
        ContactsApi.listMySubscriptions(TimeServices.getInstance().GetTime(TimestampType.PUBLIC, tenantNo), extras);
    }
    /// <summary>
    /// 从服务器拉取自定义菜单的数据
    /// </summary>
    /// <param Name="appid"></param>
    private void requestMenu(String appid,string tenantNo) {
        ContactsApi.getServiceAccountSubscription(appid, tenantNo);
    }

    /// <summary>
    /// 订阅公众号，并通过eventbus广播出去
    /// </summary>
    /// <param Name="appid"></param>
    public void requestTake(String appid,string tenantNo) {
        ContactsApi.followSubscription(appid, tenantNo);
    }

    /// <summary>
    /// 取消公众号订阅，并通过eventbus广播出去
    /// </summary>
    /// <param Name="appid"></param>
    public void requestCancel(String appid,string tenantNo) {
        ContactsApi.unfollowSubscription(appid, tenantNo);
    }

    /// <summary>
    /// 记录公共号使用动作
    /// </summary>
    /// <param Name="appId"></param>
    public void requestPublicNoticeEnterServie(String appId) {
        ContactsApi.publicNoticeEnterServie(appId);
    }

    /// <summary>
    /// 设置置顶
    /// </summary>
    /// <param Name="appId"></param>
    /// <param Name="isChecked"></param>
    public void setTopmost(String appId, Boolean isChecked,string tenantNo) {
        ContactsApi.enableSubscriptionTopmost(isChecked, appId, tenantNo);
    }

    /// <summary>
    /// 设置免打扰
    /// </summary>
    /// <param Name="appId"></param>
    /// <param Name="isChecked"></param>
    public void setQuiet(String appId, Boolean isChecked,string tenantNo) {
        ContactsApi.allowReceiveSubscriptionMessages(isChecked, appId, tenantNo);
    }

    /// <summary>
    /// 获取公众号详细信息，如果本地存在则返回，本地不存在从网络获取，返回table为null;接收PublicDetailsEvent
    /// </summary>
    /// <param Name="appId"></param>
    /// <returns></returns>
    public PublicAccountsTable findByAppId(String appId) {
        PublicAccountsTable table = null;
        try {
            table = PublicAccountsDao.getInstance().findByAppid(appId);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
        return table;
    }

    /// <summary>
    /// 公众号列表
    /// </summary>
    /// <returns></returns>
    public List<PublicAccountsTable> findAllPublicAccounts() {
        List<PublicAccountsTable> tables = null;
        try {
            tables = PublicAccountsDao.getInstance().FindAll("");
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsService), e);
        }
        if (null == tables)
            tables = new List<PublicAccountsTable>();
        return tables;
    }





    ///// <summary>
    ///// 第二个自定义菜单
    ///// </summary>
    ///// <param Name="context"></param>
    ///// <param Name="appid">公众号</param>
    ///// <param Name="listmenu">整体的自定义菜单数据</param>
    ///// <param Name="view">设置二级菜单位置时使用</param>
    //public void clickMenuMid(Context context, String appid, ArrayList<PublicMenuInfoModel> listmenu, View view) {
    //    if ("click".equals(listmenu.get(1).getType())) {
    //        LoadingDialog.showDialog(context, "数据加载,请稍后…");

    //        CoreHttpApi.clickSubscriptionMenu(appid, listmenu.get(1).getCode());

    //    } else if ("view".equals(listmenu.get(1).getType())) {
    //        String url = Constants.getCoreUrls().clickSubscriptionMenu();
    //        url = url.replace("{appId}", ToolsHelper.toString(appid));
    //        url = url.replace("{menuCode}", ToolsHelper.toString(listmenu.get(1).getCode()));
    //        WebViewActivityHelper.startWebViewActivity(context, url, listmenu.get(1).getName());

    //    } else if (listmenu.size() > 0 && listmenu.get(1).getChildren().size() != 0) {
    //        PublicMenuPopMid pmpl = new PublicMenuPopMid(context, appid, listmenu.get(1).getChildren(), listmenu.size
    //                () > 3 ? 3 : listmenu.size());
    //        pmpl.showPopupWindow(view);

    //    }

    //}

    ///// <summary>
    ///// 第三个自定义菜单
    ///// </summary>
    ///// <param Name="context"></param>
    ///// <param Name="appid">公众号</param>
    ///// <param Name="listmenu">整体的自定义菜单数据</param>
    ///// <param Name="view">设置二级菜单位置时使用</param>
    //public void clickMenuRight(Context context, String appid, ArrayList<PublicMenuInfoModel> listmenu, View view) {
    //    if ("click".equals(listmenu.get(2).getType())) {
    //        LoadingDialog.showDialog(context, "数据加载,请稍后…");

    //        CoreHttpApi.clickSubscriptionMenu(appid, listmenu.get(2).getCode());

    //    } else if ("view".equals(listmenu.get(2).getType())) {
    //        String url = Constants.getCoreUrls().clickSubscriptionMenu();
    //        url = url.replace("{appId}", ToolsHelper.toString(appid));
    //        url = url.replace("{menuCode}", ToolsHelper.toString(listmenu.get(2).getCode()));
    //        WebViewActivityHelper.startWebViewActivity(context, url, listmenu.get(2).getName());

    //    } else if (listmenu.size() > 0 && listmenu.get(2).getChildren().size() != 0) {
    //        PublicMenuPopRight pmpl = new PublicMenuPopRight(context, appid, listmenu.get(2).getChildren(),
    //                listmenu.size() > 3 ? 3 : listmenu.size());
    //        pmpl.showPopupWindow(view);

    //    }

    //}
}
}
