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
public class ContactAddedMessage : Message {

    public long userId;

    public String userNo;

    public String nickname;

    public string avatarStorageId;

    private long addUserId;

    private String addUserNo;

    private String addNickname;

    private String addAvatarId;

    public String getAddAvatarId() {
        return addAvatarId;
    }

    public void setAddAvatarId(String addAvatarId) {
        this.addAvatarId = addAvatarId;
    }

    public String getAddNickname() {
        return addNickname;
    }

    public void setAddNickname(String addNickname) {
        this.addNickname = addNickname;
    }


    public String getAddUserNo() {
        return addUserNo;
    }

    public void setAddUserNo(String addUserNo) {
        this.addUserNo = addUserNo;
    }


    public long getAddUserId() {
        return addUserId;
    }

    public void setAddUserId(long addUserId) {
        this.addUserId = addUserId;
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

    public String getNickname() {
        return nickname;
    }

    public void setNickname(String nickname) {
        this.nickname = nickname;
    }

    public void setUserNo(String userNo) {
        this.userNo = userNo;
    }

    public string getAvatarStorageId() {
        return avatarStorageId;
    }

    public void setAvatarStorageId(string avatarStorageId) {
        this.avatarStorageId = avatarStorageId;
    }


    public override MsgType getType() {
        return MsgType.ContactAdded;
    }


    public override void parse( MsgType type,  SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            if (json.Property("userId") != null) {
                this.setUserId(json.GetValue("userId").ToObject<long>());
            }

            this.setUserNo(json.GetValue("userNo").ToStr());
            this.setNickname(json.GetValue("nickname").ToStr());
            if (json.Property("avatarStorageId") != null) {
                this.setAvatarStorageId(json.GetValue("avatarStorageId").ToStr());
            }
            if (json.Property("addUserId") != null) {
                this.setAddUserId(json.GetValue("addUserId").ToObject<long>());
            }
            if (json.Property("addUserNo") != null) {
                this.setAddUserNo(json.GetValue("addUserNo").ToStr());
            }
            if (json.Property("addNickname") != null) {
                this.setAddNickname(json.GetValue("addNickname").ToStr());
            }
            if (json.Property("addAvatarId") != null) {
                this.setAddAvatarId(json.GetValue("addAvatarId").ToStr());
            }
        } catch (Exception e) {
            Log.Error(typeof(ContactAddedMessage), e);
            this.setParseError(true);
        }
    }


    public override String createContentJsonStr() {
        return null;
    }
}
}
