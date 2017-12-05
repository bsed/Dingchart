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
public class GroupNameChangedMessage : Message {
    public long groupId;

    public String groupNo;

    public String groupName;

    public long operatorUserId;

    public String operatorUserNo;

    public long getGroupId() {
        return groupId;
    }

    public void setGroupId(long groupId) {
        this.groupId = groupId;
    }

    public String getGroupNo() {
        return groupNo;
    }

    public void setGroupNo(String groupNo) {
        this.groupNo = groupNo;
    }

    public String getGroupName() {
        return groupName;
    }

    public void setGroupName(String groupName) {
        this.groupName = groupName;
    }

    public long getOperatorUserId() {
        return operatorUserId;
    }

    public void setOperatorUserId(long operatorUserId) {
        this.operatorUserId = operatorUserId;
    }

    public String getOperatorUserNo() {
        return operatorUserNo;
    }

    public void setOperatorUserNo(String operatorUserNo) {
        this.operatorUserNo = operatorUserNo;
    }

    public override MsgType getType() {
        return MsgType.GroupNameChanged;
    }

    public override String createContentJsonStr() {
        return null;
    }

    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            if (json.Property("groupId") != null) {
                this.setGroupId(json.GetValue("groupId").ToObject<long>());
            }

            this.setGroupNo(json.GetValue("groupNo").ToStr());
            this.setGroupName(json.GetValue("groupName").ToStr());
            if (json.Property("operatorUserId") != null) {

                this.setOperatorUserId(json.GetValue("operatorUserId").ToObject<long>());
            }
            this.setOperatorUserNo(json.GetValue("operatorUserNo").ToStr());
        } catch (Exception e) {
            Log.Error(typeof(GroupNameChangedMessage), e);
            this.setParseError(true);
        }
    }
}
}
