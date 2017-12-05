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
public class NotifyMessage : Message {
    private String comment;


    public String getComment() {
        return this.comment;
    }

    public void setComment(String comment) {
        this.comment = comment;
    }


    public override MsgType getType() {
        return MsgType.Notify;
    }


    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject obj = JObject.Parse((String)sendMessage.getMessage());

            base.parse(type, sendMessage);

            this.setComment(obj.GetValue("comment").ToStr());
            this.setType(type);
        } catch (Exception e) {
            Log.Error(typeof(NotifyMessage), e);
            this.setParseError(true);
        }
    }

    public override String createContentJsonStr() {

        JObject json = new JObject();
        try {
            json.Add("comment", this.getComment());
        } catch (Exception e) {
            Log.Error(typeof(NotifyMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }
}
}
