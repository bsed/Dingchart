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
public class UnknownMessage : Message {
    public String unkonown;

    public String getUnkonown() {
        return unkonown;
    }

    public void setUnkonown(String unkonown) {
        this.unkonown = unkonown;
    }

    public override  MsgType getType() {
        return MsgType.UNKNOWN;
    }


    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String) sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setUnkonown((String) sendMessage.getMessage());

        } catch (Exception e) {
            Log.Error(typeof(UnknownMessage), e);
            this.setParseError(true);
        }
    }


    public override string createContentJsonStr() {
        JObject json = new JObject();
        try {
            //TODO:新IM接口不需要这三个属性
//			json.put("from", this.getFrom());
//			json.put("to", this.getTo());
//			json.put("Timestamp", String.valueOf(this.getTimestamp()));
            json.Add("unknow", this.getUnkonown());
        } catch( Exception e) {
            Log.Error(typeof(UnknownMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }
}
}
