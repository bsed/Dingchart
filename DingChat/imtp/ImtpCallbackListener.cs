using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.im.sdk;
using cn.lds.im.sdk.notification;
using cn.lds.im.sdk.api;
using cn.lds.im.sdk.enums;
using cn.lds.im.sdk.bean;
using java.util;
using System.Threading;
using Newtonsoft.Json;
using cn.lds.im.sdk.message.util;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Event;
namespace cn.lds.chatcore.pcw.imtp {
class ImtpCallbackListener : CallbackListener {
    public void connectError(ConnAckReturnCode carc) {
        System.Console.WriteLine("CallbackListener：========connectError: " + carc);
        // 连接失败，需要发送事件，上层做业务逻辑处理
        try {
            FrameEvent<Object> eventData = new FrameEvent<Object>();
            eventData.frameEventDataType = FrameEventDataType.IM_CONNECTION_ERROR;
            EventBusHelper.getInstance().fireEvent(eventData);

        } catch (Exception e) {
            Log.Error(typeof(ImtpCallbackListener), e);
        }
    }

    public void connected() {
        System.Console.WriteLine("CallbackListener：========connected");
        // 连接成功，需要发送事件，上层做业务逻辑处理
        try {
            FrameEvent<Object> eventData = new FrameEvent<Object>();
            eventData.frameEventDataType = FrameEventDataType.IM_CONNECTED;
            EventBusHelper.getInstance().fireEvent(eventData);
        } catch (Exception e) {
            Log.Error(typeof(ImtpCallbackListener), e);
        }

    }

    public void connectionKicked(OsType osType) {
        System.Console.WriteLine("CallbackListener：========connectionKicked");
        //被踢，需要发送事件，上层做业务逻辑处理
        try {
            FrameEvent<Object> eventData = new FrameEvent<Object>();
            eventData.frameEventDataType = FrameEventDataType.LOGOUT_USER_KICKED;
            EventBusHelper.getInstance().fireEvent(eventData);
        } catch (Exception e) {
            Log.Error(typeof(ImtpCallbackListener), e);
        }

    }

    public void connectionLost() {
        System.Console.WriteLine("CallbackListener：========connectionLost");
        // 连接丢失，需要发送事件，上层做业务逻辑处理
        try {
            // 重置连接状态
            //ImClientService.getInstance().connecting = false;
            FrameEvent<Object> eventData = new FrameEvent<Object>();
            eventData.frameEventDataType = FrameEventDataType.IM_CONNECTION_LOST;
            EventBusHelper.getInstance().fireEvent(eventData);
        } catch (Exception e) {
            Log.Error(typeof(ImtpCallbackListener), e);
        }

    }

    public void exceptionCause(Exception t) {
        System.Console.WriteLine("CallbackListener：========exceptionCause");
        //todo 出现异常，需要发送事件，上层做业务逻辑处理
        try {
        } catch (Exception e) {
            Log.Error(typeof(ImtpCallbackListener), e);
        }
    }

    public void messageArrived(List messages) {
        System.Console.WriteLine("CallbackListener：========messageArrived");
        try {
            this.processMessage(messages);
        } catch (Exception e) {
            Log.Error(typeof(ImtpCallbackListener), e);
        }
    }

    public void sendComplete(List sendAckMessages) {
        System.Console.WriteLine("CallbackListener：========sendComplete");
        try {
            for (int i = 0; i < sendAckMessages.size(); i++) {
                SendAckMessage sendAckMessage = (SendAckMessage)sendAckMessages.get(i);
                String messageId = sendAckMessage.getMessageId();
                String serverMessageId = sendAckMessage.getServerMessageId();
                // 消息发送完成，需要发送事件，上层做业务逻辑处理
                MessageItem messageItem = MessageService.getInstance().updateSendStatus(messageId, 1, serverMessageId);

                BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                businessEvent.data = messageItem;
                businessEvent.eventDataType = BusinessEventDataType.MessageChangedEvent_TYPE_UPDATE;
                EventBusHelper.getInstance().fireEvent(businessEvent);
            }
        } catch (Exception e) {
            Log.Error(typeof(ImtpCallbackListener), e);
        }
    }

    public void sendError(SendMessage sendMessage) {
        System.Console.WriteLine("CallbackListener：========sendError");
        // 消息发送失败，需要发送事件，上层做业务逻辑处理
        try {
            MessageItem messageItem = MessageService.getInstance().updateSendStatus(sendMessage.getMessageId(), -1);

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = messageItem;
            businessEvent.eventDataType = BusinessEventDataType.MessageChangedEvent_TYPE_UPDATE;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(ImtpCallbackListener), e);
        }

    }

    /// <summary>
    /// 消息转换处理
    /// </summary>
    /// <param Name="messages"></param>
    private  void processMessage(List messages) {

        lock(this) {
            if (messages != null) {
                // 处理单条消息
                for (int i = 0; i < messages.size(); i++ ) {
                    SendMessage msg = (SendMessage)messages.get(i);
                    try {
                        MsgType msgType = (MsgType)msg.getMessageType();

                        Message myMessage = parseMessage(msgType, msg);

                        var json = JsonConvert.SerializeObject(myMessage);
                        //System.Console.WriteLine("=====messge type: " + myMessage.getType() + "  json:" + json);


                        if (MsgType.UNKNOWN == myMessage.getType()) {
                            Log.Error(typeof(ImtpCallbackListener), "收到未知消息！json：" + JsonConvert.SerializeObject(myMessage));
                        } else {
                            EventBusHelper.getInstance().fireEvent(new MessageArrivedEvent(myMessage));
                        }

                    } catch (Exception e) {
                        Log.Error(typeof(ImtpCallbackListener), e);
                    }
                }
            }

        }
    }

    /// <summary>
    /// 转换消息
    /// </summary>
    /// <param Name="msgType"></param>
    /// <param Name="sendMessage"></param>
    /// <returns></returns>
    private Message parseMessage(MsgType msgType, SendMessage sendMessage) {

        Message message = null;

        switch (msgType) {
        case MsgType.Text:
            message = ReflectClass.createInstance<TextMessage>();
            break;
        case MsgType.At:
            message = ReflectClass.createInstance<AtMessage>();
            break;
        case MsgType.Voice:
            message = ReflectClass.createInstance<VoiceMessage>();
            break;
        case MsgType.Image:
            message = ReflectClass.createInstance<PictureMessage>();
            break;
        case MsgType.Location:
            message = ReflectClass.createInstance<LocationMessage>();
            break;
        case MsgType.Video:
            message = ReflectClass.createInstance<VideoMessage>();
            break;
        case MsgType.VCard:
            message = ReflectClass.createInstance<VcardMessage>();
            break;
        case MsgType.PublicCard:
            message = ReflectClass.createInstance<PublicCardMessage>();
            break;
        //case MsgType.Activity:
        //    message = ReflectClass.createInstance<ActivityMessage>();
        //    break;
        case MsgType.News:
            message = ReflectClass.createInstance<NewsMessage>();
            break;
        case MsgType.File:
            message = ReflectClass.createInstance<FileMessage>();
            break;
        case MsgType.Link:
        case MsgType.Music:
            //TODO: 未处理
            break;

        case MsgType.OtherLogined:
            //TODO: 未处理
            break;

        case MsgType.FriendRequest:
            message = ReflectClass.createInstance<FriendRequestMessage>();
            break;
        case MsgType.FriendResponse:
            message = ReflectClass.createInstance<FriendResponseMessage>();
            break;
        case MsgType.ContactAdded:
            message = ReflectClass.createInstance<ContactAddedMessage>();
            break;
        case MsgType.ContactDeleted:
            //TODO: 接收，但未处理
            message = ReflectClass.createInstance<ContactDeletedMessage>();
            break;

        case MsgType.UserAvatarChanged:
            message = ReflectClass.createInstance<UserAvatarChangedMessage>();
            break;
        case MsgType.UserNicknameChanged:
            message = ReflectClass.createInstance<UserNicknameChangedMessage>();
            break;
        case MsgType.GroupMemberAdded:
            message = ReflectClass.createInstance<GroupMemberAddedMessage>();
            break;
        case MsgType.GroupMemberDeleted:
            message = ReflectClass.createInstance<GroupMemberDeletedMessage>();
            break;
        case MsgType.GroupMemberExited:
            message = ReflectClass.createInstance<GroupMemberExitedMessage>();
            break;
        case MsgType.GroupMemberNicknameChanged:
            message = ReflectClass.createInstance<GroupMemberNicknameChangedMessage>();
            break;
        case MsgType.GroupNameChanged:
            message = ReflectClass.createInstance<GroupNameChangedMessage>();
            break;
        case MsgType.GroupLogoChanged:
            message = ReflectClass.createInstance<GroupLogoChangedMessage>();
            break;

        //case MsgType.Ticket:
        //    message = ReflectClass.createInstance<TicketMessage>();
        //    break;
        //case MsgType.Product:
        //    message = ReflectClass.createInstance<ProductMessage>();
        //    break;
        case MsgType.Business:
            message = ReflectClass.createInstance<BusinessMessage>();
            break;
        case MsgType.App:
            message = ReflectClass.createInstance<AppMessage>();
            break;
        case MsgType.OrganizationChanged:
            message = ReflectClass.createInstance<OrganizationChangedMessage>();
            break;
        case MsgType.OrganizationMemberChanged:
            message = ReflectClass.createInstance<OrganizationMemberChangedMessage>();
            break;
        case MsgType.TodoTask:
            message = ReflectClass.createInstance<TodoTaskChangedMessage>();
            break;
        case MsgType.PublicWebsiteListChanged:
            message = ReflectClass.createInstance<PublicWebsiteListChangedMessage>();
            break;
        case MsgType.PublicListChanged:
            message = ReflectClass.createInstance<PublicListChangedMessage>();
            break;
        case MsgType.ThirdAppClassChanged:
            message = ReflectClass.createInstance<ThirdAppClassChangedMessage>();
            break;
        case MsgType.UserDisabled:
            message = ReflectClass.createInstance<UserDisabledMessage>();
            break;
        case MsgType.Cancel:
            message = ReflectClass.createInstance<CancelMessage>();
            break;
        case MsgType.LoginWaitting:
            message = ReflectClass.createInstance<LoginWaittingMessage>();
            break;
        case MsgType.LoginWaittingCancel:
            message = ReflectClass.createInstance<LoginWaittingCancelMessage>();
            break;
        case MsgType.LoginAuthorization:
            message = ReflectClass.createInstance<LoginAuthorizationMessage>();
            break;
        case MsgType.LoginQuit:
            message = ReflectClass.createInstance<LoginQuitMessage>();
            break;

        case MsgType.AVMeetingInvite:
            message = ReflectClass.createInstance<AVMeetingInviteMessage>();

            break;

        case MsgType.AVMeetingRefuse:
            message = ReflectClass.createInstance<AVMeetingRefuseMessage>();
            break;

        case MsgType.AVMeetingCancel:
            message = ReflectClass.createInstance<AVMeetingCancelMessage>();
            break;
        case MsgType.AVMeetingConnect:
            message = ReflectClass.createInstance<AVMeetingConnectMessage>();
            break;
        case MsgType.ReadMessage:
            message = ReflectClass.createInstance<ReadMessage>();
            break;
        case MsgType.AVMeetingTimeOut:
            message = ReflectClass.createInstance<AVMeetingTimeOutMessage>();
            break;

        case MsgType.AVMeetingBusy:
            message = ReflectClass.createInstance<AVMeetingBusyMessage>();
            break;

        case MsgType.AVMeetingSwitch:
            message = ReflectClass.createInstance<AVMeetingSwitchMessage>();
            break;
        //群视频邀请
        //    case MsgType.MessageTypeAVGroupInvite:
        //message = ReflectClass.createInstance<MessageTypeAVGroupInviteMessage>();
        //break;

        case MsgType.MessageTypeUserJoinTenant:
            message = ReflectClass.createInstance<MessageTypeUserJoinTenant>();
            break;

        case MsgType.MessageTypeUserLeaveTenant:
            message = ReflectClass.createInstance<MessageTypeUserLeaveTenant>();
            break;
        case MsgType.GroupSavedAsContact:
            message = ReflectClass.createInstance<GroupSavedAsContact>();
            break;

        default:
            message = ReflectClass.createInstance<UnknownMessage>();
            break;
        }

        try {
            message.parse(msgType, sendMessage);
        } catch (Exception e) {
            Log.Error(typeof(ImtpCallbackListener), e);
        }

        return message;
    }



}
}
