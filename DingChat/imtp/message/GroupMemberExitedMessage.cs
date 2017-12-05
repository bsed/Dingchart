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
using cn.lds.im.sdk.message.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.imtp.message {
public class GroupMemberExitedMessage : Message {
    public String groupId;
    public String groupNo;

    public String operatorUserId;
    public String operatorUserNo;

    public String managerUserId;
    public String managerUserNo;

    public String getGroupId() {
        return groupId;
    }

    public void setGroupId(String groupId) {
        this.groupId = groupId;
    }

    public String getGroupNo() {
        return groupNo;
    }

    public void setGroupNo(String groupNo) {
        this.groupNo = groupNo;
    }

    public String getOperatorUserId() {
        return operatorUserId;
    }

    public void setOperatorUserId(String operatorUserId) {
        this.operatorUserId = operatorUserId;
    }

    public String getOperatorUserNo() {
        return operatorUserNo;
    }

    public void setOperatorUserNo(String operatorUserNo) {
        this.operatorUserNo = operatorUserNo;
    }

    public String getManagerUserId() {
        return managerUserId;
    }

    public void setManagerUserId(String managerUserId) {
        this.managerUserId = managerUserId;
    }

    public String getManagerUserNo() {
        return managerUserNo;
    }

    public void setManagerUserNo(String managerUserNo) {
        this.managerUserNo = managerUserNo;
    }

    public override MsgType getType() {
        return MsgType.GroupMemberExited;
    }

    public override String createContentJsonStr() {
        return null;
    }

    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setGroupId(json.GetValue("groupId").ToStr());
            this.setGroupNo(json.GetValue("groupNo").ToStr());
            this.setOperatorUserId(json.GetValue("operatorUserId").ToStr());
            this.setOperatorUserNo(json.GetValue("operatorUserNo").ToStr());
            this.setManagerUserId(json.GetValue("managerUserId").ToStr());
            this.setManagerUserNo(json.GetValue("managerUserNo").ToStr());
        } catch (Exception e) {
            Log.Error(typeof(GroupMemberExitedMessage), e);
            this.setParseError(true);
        }
    }
}
}
