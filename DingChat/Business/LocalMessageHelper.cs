using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.imtp;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Business {
/// <summary>
/// 本地消息通用类
/// </summary>
public class LocalMessageHelper {
    private static Dictionary<String, String> mapCreateMucMsgSended = new Dictionary<String, String>();

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param Name="message"></param>
    /// <param Name="isNotify"></param>
    private static void sendNotifyMessage(NotifyMessage message, Boolean isNotify) {
        message.setMessageId(ImClientService.getInstance().generateMessageId());
        if (message.getTimestamp() == 0) {
            message.setTimestamp(DateTimeHelper.getTimeStamp());
        }
        message.setType(MsgType.Notify);
        MessageService.getInstance().onChatMsg(message);
    }

    /// <summary>
    /// 创建群聊通知消息
    /// </summary>
    /// <param Name="httpResult"></param>
    /// <returns></returns>
    public static String getCreateMucMsg(Object httpResult) {
        // 消息
        String strMessage = "";
        try {
            // 获取返回数据
            MucTableBean mucTableBean = JsonConvert.DeserializeObject<MucTableBean>(httpResult.ToStr());
            //JSONObject json = httpResult.getJsonResult().getJSONObject("data");
            if (mucTableBean.members != null) {
                List<MucMembersTableBean> members = (List<MucMembersTableBean>)mucTableBean.members;
                for (int i = 0; i < members.Count; i++) {
                    MucMembersTableBean mucMembersTableBean = members[i];
                    // 如果不是本人
                    if (mucMembersTableBean.no != App.AccountsModel.no) {
                        String name = ContactsServices.getInstance().getContractNameByNo(mucMembersTableBean.no);
                        // 如果未取得名字
                        if (name == null) {
                            strMessage += "、" + mucMembersTableBean.nickname;
                        } else {
                            strMessage += "、" + name;
                        }
                    }
                }
                if (strMessage.StartsWith("、")) {
                    strMessage = "你邀请" + strMessage.Substring(1) + "加入了群聊";
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(LocalMessageHelper), e);
        }
        return strMessage;
    }

    /// <summary>
    /// 加入群聊通知消息（扫码二维码的人）
    /// </summary>
    /// <param Name="names"></param>
    /// <returns></returns>
    public static String getJoinMucMsgByBarcodeForScaner(List<String> names) {
        // 消息
        String strMessage = "";
        try {
            for (int i = 0; i < names.Count; i++) {
                strMessage += "、" + names[i];
            }
            if (strMessage.StartsWith("、")) {
                strMessage = "你通过扫描二维码加入群聊，群聊参与人还有：" + strMessage.Substring(1);
            }
        } catch (Exception e) {
            Log.Error(typeof(LocalMessageHelper), e);
        }
        return strMessage;
    }

    /// <summary>
    /// 加入群聊通知消息（被扫码二维码的人，其他人）
    /// </summary>
    /// <param Name="name1"></param>
    /// <param Name="name2"></param>
    /// <returns></returns>
    public static String getJoinMucMsgByBarcodeForScanSource(String name1, String name2) {
        // 消息
        String strMessage = name1 + "通过扫描" + name2 + "分享的二维码加入群聊";
        return strMessage;
    }

    /// <summary>
    /// 创建群聊通知消息
    /// </summary>
    /// <param Name="httpResult"></param>
    public static void sendCreateMucMsg(Object httpResult) {
        try {

            // 获取返回数据
            MucTableBean mucTableBean = JsonConvert.DeserializeObject<MucTableBean>(httpResult.ToStr());
            // 构建消息
            NotifyMessage message = new NotifyMessage();
            message.setFrom(mucTableBean.no);
            message.setTo(App.AccountsModel.no);
            message.setComment(LocalMessageHelper.getCreateMucMsg(httpResult));
            sendNotifyMessage(message, false);
        } catch (Exception e) {
            Log.Error(typeof(LocalMessageHelper), e);
        }
    }

    /// <summary>
    /// 加入群聊通知消息（扫码二维码的人）
    /// </summary>
    /// <param Name="groupNo"></param>
    /// <param Name="names"></param>
    public static void sendJoinMucMsgByBarcode(String groupNo, List<String> names) {
        try {
            // 构建消息
            String tagMsg = LocalMessageHelper.getJoinMucMsgByBarcodeForScaner(names);

            //Message 提示消息
            NotifyMessage message = new NotifyMessage();
            message.setFrom(groupNo);
            message.setTo(App.AccountsModel.no);
            message.setComment(tagMsg);

            long time = DateTimeHelper.getTimeStamp();
            //chat_session中的消息保存
            ChatSessionTable table = new ChatSessionTable();
            table.account = App.AccountsModel.no;
            table.chatType = ChatSessionType.MUC.ToStr();
            table.user = groupNo;
            table.resource = groupNo;
            table.timestamp = time.ToStr();
            table.lastMessage = tagMsg;
            sendNotifyMessage(message, false);
            ChatSessionService.getInstance().addChatSession(table);
        } catch (Exception e) {
            Log.Error(typeof(LocalMessageHelper), e);
        }
    }

    /// <summary>
    /// 加入群聊通知消息（被扫码二维码的人、其他人）
    /// </summary>
    /// <param Name="groupNo"></param>
    /// <param Name="name1"></param>
    /// <param Name="name2"></param>
    public static void sendJoinMucMsgByBarcode(String groupNo, String name1, String name2) {
        try {

            // 构建消息
            String tagMsg = LocalMessageHelper.getJoinMucMsgByBarcodeForScanSource(name1, name2);

            //构建会话提示消息
            NotifyMessage message = new NotifyMessage();
            message.setFrom(groupNo);
            message.setTo(App.AccountsModel.no);
            message.setComment(tagMsg);

            //构建chat_session的提示消息
            long time = DateTimeHelper.getTimeStamp();
            ChatSessionTable table = new ChatSessionTable();
            table.account = App.AccountsModel.no;
            table.chatType = ChatSessionType.MUC.ToStr();
            table.user = groupNo;
            table.resource = groupNo;
            table.timestamp = time.ToStr();
            table.lastMessage = tagMsg;

            sendNotifyMessage(message, false);
            //chatsession 信息写入 DB
            ChatSessionService.getInstance().addChatSession(table);
        } catch (Exception e) {
            Log.Error(typeof(LocalMessageHelper), e);
        }
    }



    /// <summary>
    /// 回到群聊通知消息
    /// </summary>
    /// <param Name="strNo"></param>
    public static void sendBackMucMsg(String strNo) {
        NotifyMessage message = new NotifyMessage();
        message.setFrom(strNo);
        message.setTo(App.AccountsModel.no);
        message.setComment("欢迎回到群聊");
        sendNotifyMessage(message, false);
    }

    /// <summary>
    /// 群成员收到群的第一条消息后，提示“ownerName创建了群，邀请你、A、B加入了群聊”
    /// </summary>
    /// <param Name="mucNo"></param>
    /// <param Name="ownerName"></param>
    /// <param Name="members"></param>
    /// <param Name="Timestamp"></param>
    public static void sendCreateGroupMsgForMember(String mucNo, String ownerName, List<String> members, long timestamp) {
        // 记录下已经发送过创建消息的群。避免重复发送创建消息（清除数据登录后、收到大量群消息时出现）

        if (mapCreateMucMsgSended.ContainsKey(mucNo)) {
            return;
        } else {
            mapCreateMucMsgSended.Add(mucNo, mucNo);
        }
        NotifyMessage message = new NotifyMessage();
        message.setFrom(mucNo);
        message.setTo(mucNo);
        message.setTimestamp(timestamp);

        StringBuilder sb = new StringBuilder();
        sb.Append(ownerName);
        sb.Append("创建了群，邀请你");
        for (int i = 0; i < members.Count; i++) {
            sb.Append("、").Append(members[i]);
        }
        sb.Append("加入了群聊");

        message.setComment(sb.ToString());

        sendNotifyMessage(message, false);
    }



    /// <summary>
    /// 群成员邀请人加入群，邀请人显示消息 “你邀请了A、B加入了群聊”
    /// </summary>
    /// <param Name="mucNo">群编号</param>
    /// <param Name="members">被邀请人名</param>
    public static void sendAddMemberMsg(String mucNo, List<String> members) {
        NotifyMessage message = new NotifyMessage();
        message.setFrom(mucNo);
        message.setTo(mucNo);

        StringBuilder sb = new StringBuilder();
        sb.Append("你邀请");
        for (int i = 0; i < members.Count; i++) {
            if (i == 0) {
                sb.Append(members[i]);
            } else {
                sb.Append("、").Append(members[i]);
            }
        }
        sb.Append("加入了群聊");

        message.setComment(sb.ToString());

        sendNotifyMessage(message, false);
    }



    /// <summary>
    /// 有新的群成员加入群，群中的已有成员显示消息“xx邀请了A、B加入了群聊”
    /// </summary>
    /// <param Name="mucNo">群编号</param>
    /// <param Name="operatorName">发起邀请人</param>
    /// <param Name="members">被邀请人名</param>
    public static void sendAddMemberMsg(String mucNo, String operatorName, List<String> members) {
        NotifyMessage message = new NotifyMessage();
        message.setFrom(mucNo);
        message.setTo(mucNo);

        StringBuilder sb = new StringBuilder();
        sb.Append(operatorName);
        sb.Append("邀请");
        for (int i = 0; i < members.Count; i++) {
            if (i == 0) {
                sb.Append(members[i]);
            } else {
                sb.Append("、").Append(members[i]);
            }
        }
        sb.Append("加入了群聊");

        message.setComment(sb.ToString());

        sendNotifyMessage(message, false);
    }


    /// <summary>
    /// 被邀请人加入群，新成员显示消息“xx邀请你加入了群聊，群聊参与人还有A、B”
    /// </summary>
    /// <param Name="mucNo">群编号</param>
    /// <param Name="operatorName">发起邀请人</param>
    /// <param Name="members">被邀请的其它人名+群里已有成员名</param>
    public static void sendAddMemberMsgForNewMember(String mucNo, String operatorName, List<String> members) {
        NotifyMessage message = new NotifyMessage();
        message.setFrom(mucNo);
        message.setTo(mucNo);

        StringBuilder sb = new StringBuilder();
        sb.Append(operatorName);
        sb.Append("邀请你加入了群聊，群聊参与人还有：");
        for (int i = 0; i < members.Count; i++) {
            if (i == 0) {
                sb.Append(members[i]);
            } else {
                if (members[i] == App.AccountsModel.nickname) {
                    continue;
                }
                sb.Append("、").Append(members[i]);
            }
        }

        message.setComment(sb.ToString());

        sendNotifyMessage(message, false);
    }


    /// <summary>
    /// 群主踢人，群主显示消息
    /// </summary>
    /// <param Name="mucNo">群编号</param>
    /// <param Name="memberName">被踢成员名称</param>
    public static void sendMucOwnerDeleteMemberMsg(String mucNo, String memberName) {
        NotifyMessage message = new NotifyMessage();
        message.setFrom(mucNo);
        message.setTo(mucNo);

        StringBuilder sb = new StringBuilder();
        sb.Append("你将");
        sb.Append(memberName);
        sb.Append("移出了群聊");

        message.setComment(sb.ToString());

        sendNotifyMessage(message, false);
    }


    /// <summary>
    /// 群主踢人，被踢成员显示消息
    /// </summary>
    /// <param Name="mucNo">群编号</param>
    /// <param Name="ownerName">群主名称</param>
    public static void sendGroupMemberDeletedMsg(String mucNo, String ownerName) {
        NotifyMessage message = new NotifyMessage();
        message.setFrom(mucNo);
        message.setTo(mucNo);

        StringBuilder sb = new StringBuilder();
        sb.Append(ownerName);
        sb.Append("将你移出群聊");

        message.setComment(sb.ToString());

        sendNotifyMessage(message, false);
    }


    /// <summary>
    /// 群主踢人，其它成员显示消息
    /// </summary>
    /// <param Name="mucNo">群编号</param>
    /// <param Name="ownerName">群主名称</param>
    /// <param Name="memberName">被踢成员名称</param>
    public static void sendGroupMemberDeletedMsg(String mucNo, String ownerName, String memberName) {
        NotifyMessage message = new NotifyMessage();
        message.setFrom(mucNo);
        message.setTo(mucNo);
        StringBuilder sb = new StringBuilder();
        sb.Append(ownerName);
        sb.Append("将");
        sb.Append(memberName);
        sb.Append("移出了群聊");

        message.setComment(sb.ToString());
        sendNotifyMessage(message, false);
    }


    /// <summary>
    /// 群名称变更消息通知
    /// </summary>
    /// <param Name="mucNo">群编号</param>
    /// <param Name="chanagerName">修改人名称</param>
    /// <param Name="mucName">修改后群名称</param>
    public static void sendGroupNameChangedMsg(String mucNo, String chanagerName, String mucName) {
        NotifyMessage message = new NotifyMessage();
        message.setFrom(mucNo);
        message.setTo(mucNo);

        StringBuilder sb = new StringBuilder();
        sb.Append(chanagerName);
        sb.Append("修改群名为\"");
        sb.Append(mucName);
        sb.Append("\"");
        message.setComment(sb.ToString());
        sendNotifyMessage(message, false);
    }
}
}
