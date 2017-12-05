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
public class GroupMemberNicknameChangedMessage : Message {
    public long groupId;
    public String groupNo;

    public long userId;
    public String userNo;
    public String nickname;//在组中昵称

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

    public long getUserId() {
        return userId;
    }

    public void setUserId(long userId) {
        this.userId = userId;
    }

    public String getUserNo() {
        return userNo;
    }

    public void setUserNo(String userNo) {
        this.userNo = userNo;
    }

    public String getNickname() {
        return nickname;
    }

    public void setNickname(String nickname) {
        this.nickname = nickname;
    }

    public override MsgType getType() {
        return MsgType.GroupMemberNicknameChanged;
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
            if (json.Property("userId") != null) {
                this.setUserId(json.GetValue("userId").ToObject<long>());
            }

            this.setUserNo(json.GetValue("userNo").ToStr());
            this.setNickname(json.GetValue("nickname").ToStr());
        } catch (Exception e) {
            Log.Error(typeof(GroupMemberNicknameChangedMessage), e);
            this.setParseError(true);
        }
    }
}
}
