using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Services.core;

namespace cn.lds.chatcore.pcw.Services {
class MasterServices : BaseService {
    private static MasterServices instance = null;

    public static MasterServices getInstance() {
        if (instance == null) {
            instance = new MasterServices();
        }
        return instance;
    }


    private List< MasterTable> Convertors(List<MasterTable> modelList, List<MasterBean>beanList,string tenantNo) {
        for (int i = 0; i < beanList.Count; i++) {
            MasterTable model = new MasterTable();

            //BeanUtils.copyProperties(model, beanList[i]);
            model.key = beanList[i].key;
            model.value = beanList[i].value;
            model.text = beanList[i].text;
            model.parentKey = beanList[i].parentKey;
            model.description = beanList[i].description;
            model.tenantNo = tenantNo;
            model.sortOrder = beanList[i].order.ToStr();
            modelList.Add(model);
        }
        return modelList;
    }

    /// <summary>
    /// 同步码表数据
    /// </summary>
    public void RequestMaster(string tenantNo) {
        try {
            // 同步码表
            ContactsApi.syncMasterCode(TimestampType.location, tenantNo);
            ContactsApi.syncMasterCode(TimestampType.post, tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(MasterServices), e);
        }
    }
    /// <summary>
    ///API请求处理
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        try {
            switch (eventData.eventDataType) {
            //  同步码表数据
            case EventDataType.syncMasterCode:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    this.DoSyncMasterCode(eventData);
                }
                // API请求失败
                else {
                    // 如果是初次登录，则继续请求API
                    if (App.IsFirstLogin) {
                        String tenantNo = eventData.extras["tenantNo"].ToStr();
                        this.RequestMaster(tenantNo);
                    }
                }
                this.MarkDataLoadComplete(eventData);
                break;

            }
        } catch (Exception e) {
            Log.Error(typeof(MasterServices), e);
        }

    }
    /// <summary>
    /// 同步码表数据
    /// </summary>
    /// <param Name="eventData"></param>
    private void DoSyncMasterCode(EventData<Object> eventData) {
        try {

            string key = string.Empty;
            long time = 0;
            List<MasterBean> listDictionarys = new List<MasterBean>();
            Object contacts = eventData.data;

            String tenantNo = eventData.extras["tenantNo"].ToStr();
            var jArray = JArray.Parse(contacts.ToStr());


            IList<JToken> list = jArray.ToList();
            foreach (JToken token in list) {
                String dictionarys = token["dictionarys"].ToString();
                key = token["categoryKey"].ToString();
                time = long.Parse(token["updatedTime"].ToStr());
                listDictionarys =
                    JsonConvert.DeserializeObject<List<MasterBean>>(dictionarys,
                            new convertor<MasterBean>());
            }
            if (listDictionarys.Count == 0) return;
            List<MasterTable> modelList = new List<MasterTable>();
            modelList = Convertors(modelList, listDictionarys,tenantNo);
            MasterDao.getInstance().Insert(modelList, key);
            TimeServices.getInstance().SaveTime(key, time, tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(MasterServices), e);
        }
    }

    /// <summary>
    /// 根据码表类型和键查找显示的文本
    /// </summary>
    /// <param Name="masterType"></param>
    /// <param Name="strKey"></param>
    /// <returns></returns>
    public String getTextByTypeAndKey(MasterType masterType, String strKey,string tenantNo) {
        try {
            return MasterDao.getInstance().getTextByTypeAndKey(masterType, strKey, tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(MasterServices), e);
            return "";
        }
    }
}
}
