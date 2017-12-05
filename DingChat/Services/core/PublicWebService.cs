using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Common.Utils;
using EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business.Cache;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Models.Tables;
using ikvm.extensions;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.Publisher;

namespace cn.lds.chatcore.pcw.Services.core {
class PublicWebService : BaseService {
    private static PublicWebService _instance = null;
    public static PublicWebService getInstance() {
        if (_instance == null) {
            _instance = new PublicWebService();
        }
        return _instance;
    }

    /// <summary>
    /// 应用变更消息
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void OnMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {
        try {
            // 变更消息
            Message message = messageArrivedEvent.message;
            MsgType msgType = message.getType();
            switch (msgType) {
            case MsgType.PublicWebsiteListChanged:
                ProcessPublicWebsiteListChanged(message);
                break;
            default:
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(PublicWebService), e);
        }
    }

    /// <summary>
    /// 分类变更消息
    /// </summary>
    /// <param Name="message"></param>
    private void ProcessPublicWebsiteListChanged(Message message) {
        try {

            // 拉取应用列表
            if (App.TenantNoList.Count > 0) {
                foreach (string tenantNo in App.TenantNoList) {
                    this.RequestForApps(tenantNo);
                }
            } else {
                this.RequestForApps(string.Empty);
            }
        } catch (Exception e) {
            Log.Error(typeof(PublicWebService), e);
        }
    }

    /**
    * 异步拉取信息
    */
    public void RequestForApps(String tenantNo) {
        ContactsApi.apps(tenantNo);
    }

    /**
     * 添加常用应用
     */
    public void RequestForSetCommonWebApp(String appId) {
        ContactsApi.setCommonWebApp(appId);
    }

    /**
     * 取消常用应用
     */
    public void RequestForUnsetCommonWebApp(String appId) {
        ContactsApi.unsetCommonWebApp(appId);
    }

    /// <summary>
    /// API相关处理
    /// W001: 获取APP
    /// W005: 设置为常用
    /// W006: 取消常用
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void OnHttpRequestEvent(EventData<Object> eventData) {
        try {
            switch (eventData.eventDataType) {
            // W001: 获取APP
            case EventDataType.apps:

                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    W001(eventData);
                }
                // API请求失败
                else {
                    // 如果是初次登录，则继续请求API
                    if (App.IsFirstLogin) {
                        String tenantNo = eventData.extras["tenantNo"].ToStr();
                        this.RequestForApps(tenantNo);
                    }
                }
                this.MarkDataLoadComplete(eventData);
                break;
            // W005: 设置为常用
            case EventDataType.setCommonWebApp:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    W005(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // W006: 取消常用
            case EventDataType.unsetCommonWebApp:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    W006(eventData);
                }
                // API请求失败
                else {

                }
                break;

            }
        } catch (Exception e) {
            Log.Error(typeof(PublicWebService), e);
        }
    }

    /// <summary>
    /// W001: 获取APP
    /// </summary>
    /// <param Name="data"></param>
    private void W001(EventData<Object> data) {
        BusinessEvent<object> businessEvent = null;
        try {

            // 获取时间戳
            long timestamp = data.timestamp;
            Object apps = data.data;

            String tenantNo = data.extras["tenantNo"].ToStr();

            List<PublicWebBean> beanList = JsonConvert.DeserializeObject<List<PublicWebBean>>(
                                               apps.ToStr(), new convertor<PublicWebBean>());
            if (beanList.Count==0) {
                businessEvent = new BusinessEvent<object>();
                App.AppLoadOk = true;
                businessEvent.eventDataType = BusinessEventDataType.LoadingOk;
                EventBusHelper.getInstance().fireEvent(businessEvent);
                return;
            }
            //int max = 0;
            //try {
            //    Dictionary<string, object> entity = new Dictionary<string, object>();
            //    entity.Add("commonWebsite", 1);
            //    Dictionary<string, object> orders = new Dictionary<string, object>();
            //    orders.Add("sort", "asc");
            //    PublicWebTable t = PublicWebDao.getInstance().FindFirst(entity, orders);
            //    if (null != t)
            //        max = int.Parse(t.sort.ToStr()) + 1;
            //} catch (Exception ex) {
            //    Log.Error(typeof(PublicWebService), ex);
            //}


            List<PublicWebTable> tables = new List<PublicWebTable>();
            foreach (PublicWebBean publicWebBean in beanList) {
                try {
                    // 如果是手机应用，则跳过不保存
                    if (publicWebBean.includeMobileApp) {
                        continue;
                    }

                    // 如果是内置应用、并且未设置PC端URL，则跳过不保存
                    if (publicWebBean.includeComponent && string.IsNullOrEmpty(publicWebBean.componentPcUrl)) {
                        continue;
                    }

                    PublicWebTable table = PublicWebDao.getInstance().FindByAppId(publicWebBean.appId);
                    if (null == table) {
                        table = new PublicWebTable();
                        table.sort = 100000;
                        table.commonWebsiteTime = DateTimeHelper.getTimeStamp().ToStr();
                    } else {
                        // 如果是建设中的,或者是常用，即使曾经排序过。还是给扔后面去。
                        if ("inConstruction".Equals(table.websiteStatus)) {
                            table.sort = 100000;
                            table.commonWebsiteTime = DateTimeHelper.getTimeStamp().ToStr();
                        }
                    }
                    table.appId = publicWebBean.appId;
                    table.userNo = publicWebBean.userNo;
                    table.name = publicWebBean.name;
                    table.logoId = publicWebBean.logoId;
                    table.introduction = publicWebBean.introduction;
                    table.status = publicWebBean.status;
                    table.appSortIndex = publicWebBean.appSortIndex;
                    table.ownerName = publicWebBean.ownerName;
                    table.includeSubscription = publicWebBean.includeSubscription;
                    table.includeWebsite = publicWebBean.includeWebsite;
                    table.followed = publicWebBean.followed;
                    table.appClassificationId = publicWebBean.appClassificationId;
                    table.appclaasificationKey = publicWebBean.appclaasificationKey;
                    table.appClassificationName = publicWebBean.appClassificationName;
                    table.websiteStatus = publicWebBean.websiteStatus;
                    table.enableTopmost = publicWebBean.enableTopmost;
                    table.allowReceiveMessages = publicWebBean.allowReceiveMessages;
                    table.allowShareMyLocation = publicWebBean.allowShareMyLocation;
                    table.commonWebsite = publicWebBean.commonWebsite;
                    if (publicWebBean.url != null && !publicWebBean.url.Equals("")) {
                        table.url = publicWebBean.url;
                    }
                    if (publicWebBean.componentPcUrl != null && !publicWebBean.componentPcUrl.Equals("")) {
                        table.url = publicWebBean.componentPcUrl;
                    }
                    table.includeMobileApp = publicWebBean.includeMobileApp;
                    table.clientType = publicWebBean.clientType;
                    table.androidAppOpenUrl = publicWebBean.androidAppOpenUrl;
                    table.androidDownloadUrl = publicWebBean.androidDownloadUrl;
                    table.mobileAppParameters = JsonConvert.SerializeObject(publicWebBean.mobileAppParameters);
                    table.includeComponent = publicWebBean.includeComponent;
                    table.componentPhoneUrl = publicWebBean.componentPhoneUrl;
                    table.tenantNo = tenantNo;
                    tables.Add(table);

                } catch (Exception ex1) {
                    Log.Error(typeof(PublicWebService), ex1);
                }
            }
            try {

                // 删除全部
                int aa=    PublicWebDao.getInstance().DeleteByTenantNo(tenantNo);
                foreach (PublicWebTable table in tables) {
                    // 更新表
                    PublicWebDao.getInstance().save(table);
                    // 下载头像
                    DownloadBean downloadBean = new DownloadBean(table.logoId, DownloadType.SYSTEM_APP_IMAGE, null);
                    DownloadServices.getInstance().DownloadCatchAdd(downloadBean);
                }
                CacheHelper.getInstance().clearApps();
                CacheHelper.getInstance().loadAllApp();
            } catch (DbException ex2) {
                Log.Error(typeof(PublicWebService), ex2);
            }

            //更新时间戳
            if (timestamp > 0) {
                TimeServices.getInstance().SaveTime(TimestampType.PUBLICWEB, timestamp, tenantNo);
            }
        } catch (Exception e) {
            Log.Error(typeof(PublicWebService), e);
        }
        //初始化完成发送通知
        businessEvent = new BusinessEvent<object>();
        App.AppLoadOk = true;

        businessEvent.eventDataType = BusinessEventDataType.PublicWebRequestEvent;
        EventBusHelper.getInstance().fireEvent(businessEvent);
    }

    /// <summary>
    /// W005: 设置为常用
    /// </summary>
    /// <param Name="data"></param>
    private void W005(EventData<Object> data) {
        try {
            String appId = data.extras["appId"].ToStr();
            PublicWebTable table = PublicWebDao.getInstance().FindByAppId(appId);

            BusinessEvent<object> businessEvent = new BusinessEvent<object>();
            PublicWebSubscribeEventData publicWebSubscribeEventData = new PublicWebSubscribeEventData();
            publicWebSubscribeEventData.appId = appId;

            publicWebSubscribeEventData.followed = true;
            if (null != table) {
                table.commonWebsite = true;
                table.commonWebsiteTime = DateTimeHelper.getTimeStamp().ToStr();
                PublicWebDao.getInstance().save(table);
                publicWebSubscribeEventData.status = true;
            } else {
                publicWebSubscribeEventData.status = false;
            }

            businessEvent.eventDataType = BusinessEventDataType.PublicWebRequestEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(PublicWebService), e);
        }
    }
    /// <summary>
    /// W006: 取消常用
    /// </summary>
    /// <param Name="data"></param>
    private void W006(EventData<Object> data) {
        try {
            String appId = data.extras["appId"].ToStr();
            PublicWebTable table = PublicWebDao.getInstance().FindByAppId(appId);

            BusinessEvent<object> businessEvent = new BusinessEvent<object>();
            PublicWebSubscribeEventData publicWebSubscribeEventData = new PublicWebSubscribeEventData();
            publicWebSubscribeEventData.appId = appId;

            publicWebSubscribeEventData.followed = false;
            if (null != table) {
                table.commonWebsite = false;
                table.commonWebsiteTime = "";
                table.sort = 100000;

                PublicWebDao.getInstance().save(table);
                publicWebSubscribeEventData.status = true;
            } else {
                publicWebSubscribeEventData.status = false;
            }

            businessEvent.eventDataType = BusinessEventDataType.PublicWebRequestEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(PublicWebService), e);
        }
    }

    ///// <summary>
    ///// 根据分类获取未关注的应用列表,
    ///// TODO:这个啥时候用到，不知道啊，先写着了
    ///// </summary>
    ///// <param Name="thirdAppClassId">如果传入-1，代表查询全部</param>
    ///// <param Name="followed">是否已经关注</param>
    ///// <returns></returns>
    //public List<PublicWebTable> FindUnFollowedAppList(int thirdAppClassId, Boolean followed) {
    //    List<PublicWebTable> tables = null;
    //    try {
    //        Dictionary<string, object> entity = new Dictionary<string, object>();
    //        Dictionary<string, object> orders = new Dictionary<string, object>();

    //        if (followed) {
    //            entity.Add("followed", 0);
    //        } else {
    //            entity.Add("followed", 1);
    //        }
    //        // 查询某个分类下的应用
    //        if (thirdAppClassId != -1) {
    //            entity.Add("appClassificationId", thirdAppClassId);
    //        }
    //        tables = PublicWebDao.getInstance().FindAll(entity,orders);

    //    } catch (Exception e) {
    //        Log.Error(typeof(PublicWebService), e);
    //    }
    //    if (null == tables) {
    //        tables = new List<PublicWebTable>();
    //    }
    //    return tables;
    //}

    /// <summary>
    /// 查询所有添加到常用应用的列表
    /// </summary>
    /// <param Name="thirdAppClassId">如果传入-1，代表查询全部</param>
    /// <returns></returns>
    public List<PublicWebTable> FindSetCommonAppList(int thirdAppClassId) {
        List<PublicWebTable> tables = null;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            Dictionary<string, object> orders = new Dictionary<string, object>();

            // 查询某个分类下的应用
            if (thirdAppClassId != -1) {
                // 构建查询条件
                entity.Add("appClassificationId", thirdAppClassId);
                entity.Add("commonWebsite", 0);
                entity.Add("websiteStatus", "inUse");
                // 构建排序
                orders.Add("sort", "asc");
                orders.Add("commonWebsiteTime", "asc");
                orders.Add("appSortIndex", "asc");
                orders.Add("Name", "desc");
            } else {
                // 构建查询条件
                entity.Add("commonWebsite", 0);
                entity.Add("websiteStatus", "inUse");
                // 构建排序
                orders.Add("websiteStatus", "desc");
                orders.Add("appSortIndex", "asc");
                orders.Add("Name", "desc");
            }
            tables = PublicWebDao.getInstance().FindAll(entity, orders);

        } catch (Exception e) {
            Log.Error(typeof(PublicWebService), e);
        }
        if (null == tables) {
            tables = new List<PublicWebTable>();
        }
        return tables;
    }

    /// <summary>
    /// 根据应用ID查找
    /// </summary>
    /// <param Name="appId"></param>
    /// <returns></returns>
    public PublicWebTable FindByAppId(String appId) {
        try {
            return PublicWebDao.getInstance().FindByAppId(appId);

        } catch (Exception e) {
            Log.Error(typeof(PublicWebService), e);
            return null;
        }
    }

    /// <summary>
    /// 查找全部应用
    /// </summary>
    /// <param Name="thirdAppClassId">如果传入-1，代表查询全部</param>
    /// <returns></returns>
    public List<PublicWebTable> FindAllAppList(int thirdAppClassId,string tenantNo) {
        List<PublicWebTable> tables = null;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            Dictionary<string, object> orders = new Dictionary<string, object>();

            // 查询某个分类下的应用
            if (thirdAppClassId != -1) {
                // 构建查询条件
                entity.Add("appClassificationId", thirdAppClassId);
                entity.Add("tenantNo", tenantNo);
            }
            entity.Add("Status", "1");
            // 构建排序
            orders.Add("websiteStatus", "desc");
            orders.Add("appSortIndex", "asc");
            //orders.Add("Name", "desc"); // TODO:名字的排序不对
            orders.Add("totalPinyin", "asc");
            tables = PublicWebDao.getInstance().FindAll(entity, orders);

        } catch (Exception e) {
            Log.Error(typeof(PublicWebService), e);
        }
        if (null == tables) {
            tables = new List<PublicWebTable>();
        }
        return tables;
    }


    /// <summary>
    /// 查找全部应用
    /// </summary>
    /// <param Name="thirdAppClassId">如果传入-1，代表查询全部</param>
    /// <returns></returns>
    public List<PublicWebTable> FindAllAppListByKey(string appclaasificationKey, string tenantNo) {
        List<PublicWebTable> tables = null;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            Dictionary<string, object> orders = new Dictionary<string, object>();

            // 查询某个分类下的应用
            if (appclaasificationKey !=string.Empty) {
                // 构建查询条件
                entity.Add("appclaasificationKey", appclaasificationKey);
                entity.Add("tenantNo", tenantNo);
            }

            // 构建排序
            orders.Add("websiteStatus", "desc");
            orders.Add("appSortIndex", "asc");
            //orders.Add("Name", "desc"); // TODO:名字的排序不对
            orders.Add("totalPinyin", "asc");
            tables = PublicWebDao.getInstance().FindAll(entity, orders);

        } catch (Exception e) {
            Log.Error(typeof(PublicWebService), e);
        }
        if (null == tables) {
            tables = new List<PublicWebTable>();
        }
        return tables;
    }


    public List<PublicWebTable> FindAllAppListByKey(string tenantNo) {
        List<PublicWebTable> tables = null;
        try {
            tables = PublicWebDao.getInstance().findPublicWeb(tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(PublicWebService), e);
        }
        if (null == tables) {
            tables = new List<PublicWebTable>();
        }
        return tables;
    }
}
}
