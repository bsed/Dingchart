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
using cn.lds.im.sdk.message.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.imtp.message {
public  class FriendRequestMessage : Message {
    public long clientUserId;
    public String no;
    public String nickname;
    public String avatarUrl;
    public long id;
    public String requestMessage;
    public long requestDate;
    public String status;

    public long getClientUserId() {
        return this.clientUserId;
    }

    public void setClientUserId( long clientUserId) {
        this.clientUserId = clientUserId;
    }

    public String getNo() {
        return this.no;
    }

    public void setNo( String no) {
        this.no = no;
    }

    public String getNickname() {
        return this.nickname;
    }

    public void setNickname( String nickname) {
        this.nickname = nickname;
    }

    public String getAvatarUrl() {
        return this.avatarUrl;
    }

    public void setAvatarUrl( String avatarUrl) {
        this.avatarUrl = avatarUrl;
    }

    public long getId() {
        return this.id;
    }

    public void setId( long id) {
        this.id = id;
    }

    public String getRequestMessage() {
        return this.requestMessage;
    }

    public void setRequestMessage( String requestMessage) {
        this.requestMessage = requestMessage;
    }

    public long getRequestDate() {
        return this.requestDate;
    }

    public void setRequestDate( long requestDate) {
        this.requestDate = requestDate;
    }

    public String getStatus() {
        return this.status;
    }

    public void setStatus( String status) {
        this.status = status;
    }


    public override MsgType getType() {
        return MsgType.FriendRequest;
    }


    public override String createContentJsonStr() {
        return null;
    }


    public override void parse(MsgType type, SendMessage sendMessage) {
        //		if (!(jsonStr instanceof String))
        //			return;
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            if (json.Property("userId") != null) {
                this.setClientUserId(json.GetValue("userId").ToObject<long>());
            }

            this.setNo(json.GetValue("userNo").ToStr());
            this.setNickname(json.GetValue("nickname").ToStr());
            this.setAvatarUrl(json.GetValue("avatarStorageId").ToStr());
            if (json.Property("id") != null) {
                this.setId(json.GetValue("id").ToObject<long>());
            }

            this.setRequestMessage(json.GetValue("requestMessage").ToStr());
            if (json.Property("requestDate") != null) {
                this.setRequestDate(json.GetValue("requestDate").ToObject<long>());
            }

            this.setStatus(json.GetValue("status").ToStr());
        } catch (Exception e) {
            Log.Error(typeof(FriendRequestMessage), e);
            this.setParseError(true);
        }

    }
}
}
