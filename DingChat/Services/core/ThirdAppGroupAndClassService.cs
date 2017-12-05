using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Services.core {
class ThirdAppGroupAndClassService : BaseService {
    private static ThirdAppGroupAndClassService _instance = null;
    public static ThirdAppGroupAndClassService getInstance() {
        if (_instance == null) {
            _instance = new ThirdAppGroupAndClassService();
        }
        return _instance;
    }

    /// <summary>
    /// 分类变更消息
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void OnMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {
        try {
            // 获取消息类型
            Message message = messageArrivedEvent.message;
            MsgType msgType = message.getType();
            switch (msgType) {
            case MsgType.ThirdAppClassChanged:
                ProcessThirdAppClassChangedMessage(message);
                break;
            default:
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupAndClassService), e);
        }
    }

    /// <summary>
    /// 分类变更消息
    /// </summary>
    /// <param Name="message"></param>
    private void ProcessThirdAppClassChangedMessage(Message message) {
        try {
            //下载基础应用分组
            RequestForBaseAppGroups(App.CurrentTenantNo);
            if (App.TenantNoList.Count > 0) {
                foreach (string tenantNo in App.TenantNoList) {
                    this.RequestForThirdAppGroups(tenantNo);
                }
            } else {

                this.RequestForThirdAppGroups(App.CurrentTenantNo);
            }

        } catch (Exception e) {
            Log.Error(typeof(OrganizationServices), e);
        }
    }
    /**
     * 异步拉取信息
     */
    public void RequestForThirdAppGroups(string tenantNo) {
        ContactsApi.appClassificationGroups(tenantNo);
    }

    public void RequestForBaseAppGroups(string tenantNo) {
        ContactsApi.appBaseGroups(tenantNo);
    }
    /// <summary>
    /// Open007_1: appClassificationGroups 查找全部分类（mobile）2016.5.7 14:08 接口替换
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void OnHttpRequestEvent(EventData<Object> eventData) {
        try {
            switch (eventData.eventDataType) {
            //  Open007_1: appClassificationGroups 查找全部分类（mobile）2016.5.7 14:08 接口替换
            case EventDataType.appClassificationGroups:

                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    Open007_1(eventData);
                }
                // API请求失败
                else {
                    // 如果是初次登录，则继续请求API
                    if (App.IsFirstLogin) {
                        String tenantNo = eventData.extras["tenantNo"].ToStr();
                        this.RequestForThirdAppGroups(tenantNo);
                    }
                }
                this.MarkDataLoadComplete(eventData);
                break;
            //  Open007_1: appClassificationGroups 查找全部分类（mobile）2016.5.7 14:08 接口替换
            case EventDataType.appBaseGroups:

                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    Open007_2(eventData);
                }
                // API请求失败
                else {
                    // 如果是初次登录，则继续请求API
                    if (App.IsFirstLogin) {
                        String tenantNo = eventData.extras["tenantNo"].ToStr();
                        this.RequestForBaseAppGroups(tenantNo);
                    }
                }
                this.MarkDataLoadComplete(eventData);
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupAndClassService), e);
        }
    }

    /// <summary>
    /// Open007_2: 基础分类
    /// </summary>
    /// <param Name="data"></param>
    private void Open007_2(EventData<Object> data) {
        try {
            Object contacts = data.data;
            List<ThirdAppGroupBean> beanList = JsonConvert.DeserializeObject<List<ThirdAppGroupBean>>(
                                                   contacts.ToStr(), new convertor<ThirdAppGroupBean>());
            if (beanList.Count == 0) return;
            String tenantNo = "-1";
            int cont=     ThirdAppGroupDao.getInstance().DeleteByBase();
            foreach (ThirdAppGroupBean thirdAppGroupBean in beanList) {
                // 保存应用分类的分组
                ThirdAppGroupTable thirdAppGroupTable = new ThirdAppGroupTable();
                thirdAppGroupTable.thirdAppGroupId = thirdAppGroupBean.id;
                thirdAppGroupTable.name = thirdAppGroupBean.name;
                thirdAppGroupTable.iconId = thirdAppGroupBean.iconId;
                thirdAppGroupTable.sortNum = thirdAppGroupBean.sortNum;
                thirdAppGroupTable.tenantNo = tenantNo;
                thirdAppGroupTable.key = thirdAppGroupBean.key;
                ThirdAppGroupDao.getInstance().Save(thirdAppGroupTable);

                //延迟下载下载分组头像
                //DownloadServices.getInstance().DownloadMethod(thirdAppGroupTable.iconId, DownloadType.SYSTEM, null);
                DownloadBean downloadBean = new DownloadBean(thirdAppGroupTable.iconId, DownloadType.SYSTEM_APP_IMAGE, null);
                DownloadServices.getInstance().DownloadCatchAdd(downloadBean);
            }

            TimeServices.getInstance().SaveTime(TimestampType.BaseAppGroup, data.timestamp, tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupAndClassService), e);
        }
    }

    /// <summary>
    /// Open007_1: appClassificationGroups 查找全部分类（mobile）2016.5.7 14:08 接口替换
    /// </summary>
    /// <param Name="data"></param>
    private void Open007_1(EventData<Object> data) {
        try {
            Object contacts = data.data;
            List<ThirdAppGroupBean> beanList = JsonConvert.DeserializeObject<List<ThirdAppGroupBean>>(
                                                   contacts.ToStr(), new convertor<ThirdAppGroupBean>());
            if (beanList.Count == 0) return;
            String tenantNo = data.extras["tenantNo"].ToStr();

            //删分类
            int a=       ThirdAppGroupDao.getInstance().DeleteByTenantNo(tenantNo);
            //删分组
            ThirdAppClassDao.getInstance().DeleteByTenantNo(tenantNo);
            foreach (ThirdAppGroupBean thirdAppGroupBean in beanList) {
                // 保存应用分类的分组
                ThirdAppGroupTable thirdAppGroupTable = new ThirdAppGroupTable();
                thirdAppGroupTable.thirdAppGroupId = thirdAppGroupBean.id;
                thirdAppGroupTable.name = thirdAppGroupBean.name;
                thirdAppGroupTable.iconId = thirdAppGroupBean.iconId;
                thirdAppGroupTable.sortNum = thirdAppGroupBean.sortNum;
                thirdAppGroupTable.tenantNo = tenantNo;
                ThirdAppGroupDao.getInstance().Save(thirdAppGroupTable);

                //延迟下载下载分组头像
                //DownloadServices.getInstance().DownloadMethod(thirdAppGroupTable.iconId, DownloadType.SYSTEM, null);
                DownloadBean downloadBean = new DownloadBean(thirdAppGroupTable.iconId, DownloadType.SYSTEM_APP_IMAGE, null);
                DownloadServices.getInstance().DownloadCatchAdd(downloadBean);


                // 保存应用分类

                if (thirdAppGroupBean.appClassifications != null) {

                    foreach (ThirdAppClassBean thirdAppClassBean in thirdAppGroupBean.appClassifications) {
                        ThirdAppClassTable thirdAppClassTable = new ThirdAppClassTable();
                        thirdAppClassTable.thirdAppClassId = thirdAppClassBean.id;
                        thirdAppClassTable.name = thirdAppClassBean.name;
                        thirdAppClassTable.iconId = thirdAppClassBean.iconId;
                        thirdAppClassTable.sortNum = thirdAppClassBean.sortNum;
                        thirdAppClassTable.thirdAppGroupId = thirdAppGroupBean.id;
                        thirdAppClassTable.tenantNo = tenantNo;
                        ThirdAppClassDao.getInstance().Save(thirdAppClassTable);

                        //延迟下载分类头像
                        //DownloadServices.getInstance().DownloadMethod(thirdAppClassTable.iconId, DownloadType.SYSTEM, null);
                        downloadBean = new DownloadBean(thirdAppClassTable.iconId, DownloadType.SYSTEM_APP_IMAGE, null);
                        DownloadServices.getInstance().DownloadCatchAdd(downloadBean);
                    }
                }

            }

            TimeServices.getInstance().SaveTime(TimestampType.ThirdAppGroup, data.timestamp, tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupAndClassService), e);
        }
    }

    /// <summary>
    /// 查询全部分组
    /// </summary>
    /// <returns></returns>
    public List<ThirdAppGroupTable> FindAllThirdAppGroup(string tenantNo) {
        try {
            return ThirdAppGroupDao.getInstance().FindAll(tenantNo);

        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupAndClassService), e);
            return null;
        }
    }

    /// <summary>
    /// 通过ID查找某个分组
    /// </summary>
    /// <param Name="thirdAppGroupId"></param>
    /// <returns></returns>
    public ThirdAppGroupTable FindThirdAppGroupById(String thirdAppGroupId) {
        try {
            return ThirdAppGroupDao.getInstance().FindByThirdAppGroupId(thirdAppGroupId);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupAndClassService), e);
            return null;
        }
    }

    /// <summary>
    /// 查询全部分类（通过分组）
    /// </summary>
    /// <returns></returns>
    public List<ThirdAppClassTable> FindAllThirdAppClassByGroupId(String thirdAppGroupId,string tenantNo) {
        try {
            return ThirdAppClassDao.getInstance().FindAllThirdAppClassByGroupId(thirdAppGroupId, tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupAndClassService), e);
            return null;
        }
    }

    /// <summary>
    /// 查询全部分类
    /// </summary>
    /// <returns></returns>
    public List<ThirdAppClassTable> FindAllThirdAppClass(string tenantNo) {
        try {
            return ThirdAppClassDao.getInstance().FindAllThirdAppClass(tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupAndClassService), e);
            return null;
        }
    }

    /// <summary>
    /// 通过ID查找单个分类
    /// </summary>
    /// <param Name="thirdAppGroupId"></param>
    /// <returns></returns>
    public ThirdAppClassTable FindThirdAppClassById(String thirdAppClassId) {
        try {
            return ThirdAppClassDao.getInstance().FindThirdAppClassById(thirdAppClassId);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupAndClassService), e);
            return null;
        }
    }


    public ThirdAppGroupTable FindByThirdAppKey(String key) {
        try {
            return ThirdAppGroupDao.getInstance().FindByThirdAppKey(key);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupAndClassService), e);
            return null;
        }
    }


}
}
