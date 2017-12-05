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
public class ContactDeletedMessage : Message {
    public long userId;

    public String userNo;

    public string delUserId;

    public string getDelUserId() {
        return delUserId;
    }

    public void setDelUserId(string delUserId) {
        this.delUserId = delUserId;
    }

    public string delUserNo;

    public string getDelUserNo() {
        return delUserNo;
    }

    public void setDelUserNo(string delUserNo) {
        this.delUserNo = delUserNo;
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


    public override MsgType getType() {
        return MsgType.ContactDeleted;
    }


    public  override String createContentJsonStr() {
        return null;
    }


    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            if (json.Property("userId") != null) {
                this.setUserId(json.GetValue("userId").ToObject<long>());
            }
            this.setUserNo(json.GetValue("userNo").ToStr());
            this.setDelUserId(json.GetValue("delUserId").ToStr());
            this.setDelUserNo(json.GetValue("delUserNo").ToStr());
        } catch (Exception e) {
            Log.Error(typeof(ContactAddedMessage), e);
            this.setParseError(true);
        }
    }
}
}
