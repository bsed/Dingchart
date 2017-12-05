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
public class GroupLogoChangedMessage : Message {
    public long groupId;
    public String groupNo;
    public string avatarId;

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

    public string getAvatarId() {
        return avatarId;
    }

    public void setAvatarId(string avatarId) {
        this.avatarId = avatarId;
    }

    public override MsgType getType() {
        return MsgType.GroupLogoChanged;
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
            if (json.Property("avatarId") != null) {
                this.setAvatarId(json.GetValue("avatarId").ToStr());
            }

        } catch (Exception e) {
            Log.Error(typeof(GroupLogoChangedMessage), e);
            this.setParseError(true);
        }
    }
}
}
