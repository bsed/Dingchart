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
using cn.lds.chatcore.pcw.imtp;

namespace cn.lds.chatcore.pcw.Services.core {
class AVMeetingTimeChargingService {
    private static AVMeetingTimeChargingService _instance = null;
    public static AVMeetingTimeChargingService getInstance() {
        if (_instance == null) {
            _instance = new AVMeetingTimeChargingService();
        }
        return _instance;
    }


    /// <summary>
    /// 接通时，增+
    /// </summary>
    /// <param name="agoraKey"></param>
    /// <param name="roomId"></param>
    /// <param name="avType"></param>
    /// <param name="callType"></param>
    /// <param name="fromImId"></param>
    /// <param name="toImId"></param>
    /// <returns></returns>
    public AVMeetingTimeChargingTable AddAVMeetingTimeCharging(String agoraKey
            ,String roomId, AVMeetingType avType, AVMeetingCallType callType,String fromImId,String toImId) {
        try {
            AVMeetingTimeChargingTable table = new AVMeetingTimeChargingTable();
            String nowTime = DateTimeHelper.getTimeStamp().ToStr();
            String endTime = DateTimeHelper.getTimeStamp(DateTimeHelper.getDate(nowTime).AddMinutes(1)).ToStr();
            table.chargingId = nowTime;
            table.agoraKey = agoraKey;
            table.roomId = roomId;
            table.avType = avType.ToStr();
            table.callType = callType.ToStr();
            table.fromImId = fromImId;
            table.toImId = toImId;
            table.startTime = nowTime;
            table.endTime = endTime;
            table.useTime = 1;// 只要通话开始，就计时1分钟
            AVMeetingTimeChargingDao.getInstance().save(table);
            return table;
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingTimeChargingService), e);
        }
        return null;
    }

    /// <summary>
    /// 更新计时
    /// </summary>
    /// <param name="chargingId"></param>
    /// <param name="useMinutes"></param>
    /// <returns></returns>
    public void UpdateAVMeetingTimeCharging(String chargingId, int useMinutes) {
        try {
            AVMeetingTimeChargingTable table = AVMeetingTimeChargingDao.getInstance().findByChargingId(chargingId);
            if (table == null) {
                return;
            }
            String endTime = DateTimeHelper.getTimeStamp(DateTimeHelper.getDate(table.endTime).AddMinutes(useMinutes)).ToStr();
            table.endTime = endTime;
            table.useTime = table.useTime+ useMinutes;
            AVMeetingTimeChargingDao.getInstance().save(table);
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingTimeChargingService), e);
        }
    }

    /// <summary>
    /// TODO:上传计时数据
    /// </summary>
    public void UploadAVMeetingTimeCharging() {
        try {
            List<AVMeetingTimeChargingTable> list = AVMeetingTimeChargingDao.getInstance().findAll();
            if (list == null || list.Count==0) {
                return;
            }
            // TODO:调用rest接口上传数据，原则上，语音聊天或视频聊天的时候，不进行上传业务操作
            // ContactsApi.UploadAVMeetingTimeCharging(list);
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingTimeChargingService), e);
        }
    }

    /// <summary>
    /// TODO:API请求处理
    /// XXXX:上传计费
    /// </summary>
    /// <param Name="eventData"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        switch (eventData.eventDataType) {
        /*
        //XXXX:上传计费
        case EventDataType.UploadAVMeetingTimeCharging:
        // API请求成功
        if (eventData.eventType == EventType.HttpRequest) {
        AVMeetingTimeChargingDao.getInstance().deleteAll();
        }
        // API请求失败
        else {

        }
        break;
        */
        default:
            break;
        }

    }

}
}
