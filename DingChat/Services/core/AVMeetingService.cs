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
class AVMeetingService {
    private static AVMeetingService _instance = null;
    public static AVMeetingService getInstance() {
        if (_instance == null) {
            _instance = new AVMeetingService();
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
            // 语音聊天&视频聊天消息
            Message message = messageArrivedEvent.message;
            MsgType msgType = message.getType();
            switch (msgType) {
            // 占线通知
            case MsgType.AVMeetingBusy:
                ProcessAVMeetingBusyMessage(message);
                break;
            // 取消通知
            case MsgType.AVMeetingCancel:
                ProcessAVMeetingCancelMessage(message);
                break;
            // 已接通通知
            case MsgType.AVMeetingConnect:
                ProcessAVMeetingConnectMessage(message);
                break;
            // 邀请通知
            case MsgType.AVMeetingInvite:
                ProcessAVMeetingInviteMessage(message);
                break;
            // 拒绝通知
            case MsgType.AVMeetingRefuse:
                ProcessAVMeetingRefuseMessage(message);
                break;
            // 切换音频通知
            case MsgType.AVMeetingSwitch:
                ProcessAVMeetingSwitchMessage(message);
                break;
            // 超时通知
            case MsgType.AVMeetingTimeOut:
                ProcessAVMeetingTimeOutMessage(message);
                break;

            // 群成员拒绝通知
            //case MsgType.MessageTypeAVGroupRefuse:
            //    ProcessMessageTypeAVGroupRefuse(message);
            //    break;
            // 群成员取消通知
            //case MsgType.MessageTypeAVGroupCancel:
            //    ProcessMessageTypeAVGroupCancelMessage(message);
            //    break;
            default:
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }

    /// <summary>
    /// 群成员取消通知
    /// </summary>
    /// <param Name="message"></param>
    private void ProcessMessageTypeAVGroupCancelMessage(Message message) {
        try {
            // 消息体转换
            MessageTypeAVGroupCancelMessage refuseMessage = (MessageTypeAVGroupCancelMessage)message;

            // 更新消息表
            // this.SetChatMessageTextByType(AVMeetingBussinessType.refuse_by_other_side, refuseMessage.getRoomId());
            //发出业务事件通知
            //AVMeetingRefuseEventData data = new AVMeetingRefuseEventData();
            //data.message = refuseMessage;

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = refuseMessage.getUserCard();
            businessEvent.eventDataType = BusinessEventDataType.AVGroupMeetingCancel;

            EventBusHelper.getInstance().fireEvent(businessEvent);

        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }
    /// <summary>
    /// 群成员拒绝通知
    /// </summary>
    /// <param Name="message"></param>
    private void ProcessMessageTypeAVGroupRefuse(Message message) {
        try {
            // 消息体转换
            MessageTypeAVGroupRefuseMessage refuseMessage = (MessageTypeAVGroupRefuseMessage)message;

            // 更新消息表
            // this.SetChatMessageTextByType(AVMeetingBussinessType.refuse_by_other_side, refuseMessage.getRoomId());
            //发出业务事件通知
            //AVMeetingRefuseEventData data = new AVMeetingRefuseEventData();
            //data.message = refuseMessage;

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = refuseMessage.getUserCard();
            businessEvent.eventDataType = BusinessEventDataType.AVGroupMeetingRefuse;

            EventBusHelper.getInstance().fireEvent(businessEvent);

        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }

    /// <summary>
    /// 占线通知消息
    /// </summary>
    /// <param Name="message"></param>
    private void ProcessAVMeetingBusyMessage(Message message) {
        try {
            // 消息体转换
            AVMeetingBusyMessage busyMessage = (AVMeetingBusyMessage)message;
            // 更新消息表
            this.SetChatMessageTextByType(AVMeetingBussinessType.busy_by_other_side, busyMessage.getRoomId());
            //发出业务事件通知
            //AVMeetingBusyEventData data = new AVMeetingBusyEventData();
            //data.message = busyMessage;

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = busyMessage.getAvType();
            businessEvent.eventDataType = BusinessEventDataType.AVMeetingBusy;

            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }

    /// <summary>
    /// 已接通通知
    /// </summary>
    /// <param Name="message"></param>
    private void ProcessAVMeetingConnectMessage(Message message) {
        try {
            // 消息体转换
            AVMeetingConnectMessage connectMessage = (AVMeetingConnectMessage)message;

            if(connectMessage.getDevice()=="PC") {
                return;
            }

            //发出业务事件通知
            AVMeetingConnectEventData data = new AVMeetingConnectEventData();
            data.message = connectMessage;

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = connectMessage.getAvType();
            businessEvent.eventDataType = BusinessEventDataType.AVMeetingConnect;

            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }

    /// <summary>
    /// 取消通知
    /// </summary>
    /// <param Name="message"></param>
    private void ProcessAVMeetingCancelMessage(Message message) {
        try {
            // 消息体转换
            AVMeetingCancelMessage cancelMessage = (AVMeetingCancelMessage)message;

            // 更新消息表
            this.SetChatMessageTextByType(AVMeetingBussinessType.cancel_by_other_side, cancelMessage.getRoomId());
            //发出业务事件通知
            AVMeetingCancelEventData data = new AVMeetingCancelEventData();
            data.message = cancelMessage;

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = cancelMessage.getAvType();
            businessEvent.eventDataType = BusinessEventDataType.AVMeetingCancel;

            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }



    /// <summary>
    /// 邀请通知
    /// </summary>
    /// <param Name="message"></param>
    private void ProcessAVMeetingInviteMessage(Message message) {
        try {
            // 消息体转换
            AVMeetingInviteMessage inviteMessage = (AVMeetingInviteMessage)message;
            // 如果大于1分钟，则无视处理
            if (!DateTimeHelper.CheckTimestampDiffByMinutes(inviteMessage.timestamp,1)) {
                return;
            }

            //发出业务事件通知
            AVMeetingInviteEventData data = new AVMeetingInviteEventData();
            data.message = inviteMessage;
            data.callType = AVMeetingCallType.called;

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = data;
            businessEvent.eventDataType = BusinessEventDataType.AVMeetingInvite;

            EventBusHelper.getInstance().fireEvent(businessEvent);

        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }

    /// <summary>
    /// 拒绝通知
    /// </summary>
    /// <param Name="message"></param>
    private void ProcessAVMeetingRefuseMessage(Message message) {
        try {
            // 消息体转换
            AVMeetingRefuseMessage refuseMessage = (AVMeetingRefuseMessage)message;

            // 更新消息表
            this.SetChatMessageTextByType(AVMeetingBussinessType.refuse_by_other_side, refuseMessage.getRoomId());
            //发出业务事件通知
            //AVMeetingRefuseEventData data = new AVMeetingRefuseEventData();
            //data.message = refuseMessage;

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = refuseMessage.getAvType();
            businessEvent.eventDataType = BusinessEventDataType.AVMeetingRefuse;

            EventBusHelper.getInstance().fireEvent(businessEvent);

        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }

    /// <summary>
    /// 切换音频通知
    /// </summary>
    /// <param Name="message"></param>
    private void ProcessAVMeetingSwitchMessage(Message message) {
        try {
            // 消息体转换
            AVMeetingSwitchMessage switchMessage = (AVMeetingSwitchMessage)message;


            //发出业务事件通知
            AVMeetingSwitchEventData data = new AVMeetingSwitchEventData();
            data.message = switchMessage;

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = data;
            businessEvent.eventDataType = BusinessEventDataType.AVMeetingSwitch;

            EventBusHelper.getInstance().fireEvent(businessEvent);

        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }

    /// <summary>
    /// 超时通知
    /// </summary>
    /// <param Name="message"></param>
    private void ProcessAVMeetingTimeOutMessage(Message message) {
        try {
            // 消息体转换
            AVMeetingTimeOutMessage timeOutMessage = (AVMeetingTimeOutMessage)message;

            //让对方显示“对方已取消”
            AVMeetingService.getInstance()
            .SendAVMeetingCancelMessage(timeOutMessage.getFrom(), timeOutMessage.getAvType(), App.AccountsModel.clientuserId.ToInt(), timeOutMessage.getRoomId());


            //发出业务事件通知
            //AVMeetingTimeOutEventData data = new AVMeetingTimeOutEventData();
            //data.message = timeOutMessage;

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = timeOutMessage.getAvType(); ;
            businessEvent.eventDataType = BusinessEventDataType.AVMeetingTimeOut;

            EventBusHelper.getInstance().fireEvent(businessEvent);

            // 更新消息表
            this.SetChatMessageTextByType(AVMeetingBussinessType.timeout_by_other_side, timeOutMessage.getRoomId());
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }


    /// <summary>
    /// 发送：占线通知消息
    /// </summary>
    /// <param Name="message"></param>
    public void SendAVMeetingBusyMessage(String to,String avType, uint uId,String roomId) {
        try {
            Message message = new AVMeetingBusyMessage();
            String messageId = ImClientService.getInstance().generateMessageId();
            // 构建消息基础数据
            message.setMessageId(messageId);
            message.setType(MsgType.AVMeetingBusy);
            message.setTo(to);
            // 构建消息业务数据
            ((AVMeetingBusyMessage)message).setAvType(avType);
            ((AVMeetingBusyMessage)message).setUId(uId);
            ((AVMeetingBusyMessage)message).setRoomId(roomId);
            ((AVMeetingBusyMessage)message).setTimestamp(DateTimeHelper.getTimeStamp());
            // 发送消息
            MessageService.getInstance().doSend(message);

            // 更新消息表
            //this.SetChatMessageTextByType(AVMeetingBussinessType.busy_by_other_side, roomId);

        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }


    /// <summary>
    /// 发送：接通通知
    /// </summary>
    /// <param Name="message"></param>
    public void SendAVMeetingConnectMessage(String to, String avType, int uId, String roomId,string device) {
        try {
            Message message = new AVMeetingConnectMessage();
            String messageId = ImClientService.getInstance().generateMessageId();
            // 构建消息基础数据
            message.setMessageId(messageId);
            message.setType(MsgType.AVMeetingConnect);
            message.setTo(to);
            // 构建消息业务数据
            ((AVMeetingConnectMessage)message).setAvType(avType);
            ((AVMeetingConnectMessage)message).setUId(uId);
            ((AVMeetingConnectMessage)message).setRoomId(roomId);
            ((AVMeetingConnectMessage)message).setTimestamp(DateTimeHelper.getTimeStamp());
            ((AVMeetingConnectMessage)message).setDevice(device);
            // 发送消息
            MessageService.getInstance().doSend(message);

        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }
    /// <summary>
    /// 发送：取消通知
    /// </summary>
    /// <param Name="message"></param>
    public void SendAVMeetingCancelMessage(String to, String avType, int uId, String roomId) {
        try {
            Message message = new AVMeetingCancelMessage();
            String messageId = ImClientService.getInstance().generateMessageId();
            // 构建消息基础数据
            message.setMessageId(messageId);
            message.setType(MsgType.AVMeetingCancel);
            message.setTo(to);
            // 构建消息业务数据
            ((AVMeetingCancelMessage)message).setAvType(avType);
            ((AVMeetingCancelMessage)message).setUId(uId);
            ((AVMeetingCancelMessage)message).setRoomId(roomId);
            ((AVMeetingCancelMessage)message).setTimestamp(DateTimeHelper.getTimeStamp());
            // 发送消息
            MessageService.getInstance().doSend(message);

            // 更新消息表
            this.SetChatMessageTextByType(AVMeetingBussinessType.cancel_by_myself, roomId);

        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }



    ///// <summary>
    ///// todo 在messageservice中处理了。
    ///// </summary>
    ///// <param Name="message"></param>
    //private void SendAVMeetingInviteMessage(String to, AVMeetingType avType, int uId, String roomId) {
    //    try {


    //    } catch (Exception e) {
    //        Log.Error(typeof(AVMeetingService), e);
    //    }
    //}

    /// <summary>
    /// 发送：拒绝通知
    /// </summary>
    /// <param Name="message"></param>
    public void SendAVMeetingRefuseMessage(String to, String avType, int uId, String roomId) {
        try {

            Message message = new AVMeetingRefuseMessage();
            String messageId = ImClientService.getInstance().generateMessageId();
            // 构建消息基础数据
            message.setMessageId(messageId);
            message.setType(MsgType.AVMeetingRefuse);
            message.setTo(to);
            // 构建消息业务数据
            ((AVMeetingRefuseMessage)message).setAvType(avType);
            ((AVMeetingRefuseMessage)message).setUId(uId);
            ((AVMeetingRefuseMessage)message).setRoomId(roomId);
            ((AVMeetingRefuseMessage)message).setTimestamp(DateTimeHelper.getTimeStamp());
            // 发送消息
            MessageService.getInstance().doSend(message);
            // 更新消息表
            this.SetChatMessageTextByType(AVMeetingBussinessType.refuse_by_myself, roomId);

        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }

    /// <summary>
    /// 发送：切换音频通知
    /// </summary>
    /// <param Name="message"></param>
    public void SendAVMeetingSwitchMessage(String to, String avType, int uId, String roomId, string amStatus) {
        try {

            Message message = new AVMeetingSwitchMessage();
            String messageId = ImClientService.getInstance().generateMessageId();
            // 构建消息基础数据
            message.setMessageId(messageId);
            message.setType(MsgType.AVMeetingSwitch);
            message.setTo(to);
            // 构建消息业务数据
            ((AVMeetingSwitchMessage)message).setAvType(avType);
            ((AVMeetingSwitchMessage)message).setUId(uId);
            ((AVMeetingSwitchMessage)message).setRoomId(roomId);
            ((AVMeetingSwitchMessage)message).setTimestamp(DateTimeHelper.getTimeStamp());
            ((AVMeetingSwitchMessage)message).setAmStatus(amStatus);
            // 发送消息
            MessageService.getInstance().doSend(message);

        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }

    /// <summary>
    /// 发送：超时通知
    /// </summary>
    /// <param Name="message"></param>
    public void SendAVMeetingTimeOutMessage(String to, String avType, int uId, String roomId,bool sendMessage) {
        try {

            Message message = new AVMeetingTimeOutMessage();
            String messageId = ImClientService.getInstance().generateMessageId();
            // 构建消息基础数据
            message.setMessageId(messageId);
            message.setType(MsgType.AVMeetingTimeOut);
            message.setTo(to);
            // 构建消息业务数据
            ((AVMeetingTimeOutMessage)message).setAvType(avType);
            ((AVMeetingTimeOutMessage)message).setUId(uId);
            ((AVMeetingTimeOutMessage)message).setRoomId(roomId);
            ((AVMeetingTimeOutMessage)message).setTimestamp(DateTimeHelper.getTimeStamp());

            if(sendMessage) {
                // 发送消息
                MessageService.getInstance().doSend(message);
                // 更新消息表
                this.SetChatMessageTextByType(AVMeetingBussinessType.cancel_by_other_side, roomId);
            } else {
                // 更新消息表
                this.SetChatMessageTextByType(AVMeetingBussinessType.timeout_by_other_side, roomId);

            }


        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }


    /// <summary>
    /// 更新消息显示
    /// </summary>
    /// <param name="bussinessType"></param>
    /// <param name="roomId"></param>
    public void SetChatMessageTextByType(AVMeetingBussinessType bussinessType,String roomId) {
        try {
            MessagesTable messagesTable = MessageDao.getInstance().findLastAVMeetingInviteMessagesByRoomId(roomId);
            if (messagesTable==null) {
                return;
            }
            String strText = "";
            switch (bussinessType) {
            // 通话完成，显示：通话时长s%
            case AVMeetingBussinessType.complete:
                DateTime tm_start = DateTimeHelper.getDate(messagesTable.timestamp.ToStr());
                DateTime tm_end = DateTime.Now;
                TimeSpan ts = tm_end - tm_start;
                strText = "通话时长";
                if (ts.Hours>0) {
                    strText += ts.Hours+"小时";
                }
                if (ts.Minutes > 0) {
                    strText += ts.Minutes + "分";
                }
                if (ts.Seconds > 0) {
                    strText += ts.Seconds + "秒";
                }
                break;
            // 通话取消，显示：已取消
            case AVMeetingBussinessType.cancel_by_myself:
                strText = "已取消";
                break;
            // 通话取消，显示：对方已取消
            case AVMeetingBussinessType.cancel_by_other_side:
                strText = "对方已取消";
                break;
            // 通话拒绝，显示：已拒绝
            case AVMeetingBussinessType.refuse_by_myself:
                strText = "已拒绝";
                break;
            // 通话拒绝，显示：对方已拒绝
            case AVMeetingBussinessType.refuse_by_other_side:
                strText = "对方已拒绝";
                break;
            // 通话拒绝，显示：对方忙
            case AVMeetingBussinessType.busy_by_other_side:
                strText = "对方忙";
                break;
            // 通话超时，显示：对方未应答
            case AVMeetingBussinessType.timeout_by_other_side:
                strText = "对方未应答";
                break;
            // 视频切换语音
            case AVMeetingBussinessType.AVMeetingSwitch:
                strText = Constants.MESSAGE_FORMAT_TEXT_AUDIO;
                break;
            default:
                break;
            }
            messagesTable.text = strText;
            MessageDao.getInstance().save(messagesTable);

            //发出业务事件通知
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = messagesTable;
            businessEvent.eventDataType = BusinessEventDataType.AVMeetingRefresh;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingService), e);
        }
    }
}
}
