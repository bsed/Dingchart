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
using ikvm.extensions;

namespace cn.lds.chatcore.pcw.imtp.message {

public class TextMessage : Message {

    public String text;

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }

    public MsgType getType() {
        return MsgType.Text;
    }


    public  override void parse(MsgType type,  SendMessage sendMessage) {
//		if (!(jsonStr instanceof String))
//			return;
        try {

            JObject obj = JObject.Parse((String) sendMessage.getMessage());
            JToken jtoken = obj.GetValue("text");
            String text = jtoken.ToStr();
            base.parse(type, sendMessage);

            this.setText(text);
        } catch (Exception e) {
            Log.Error(typeof(TextMessage), e);
            this.setParseError(true);
        }
    }


    public  override String createContentJsonStr() {

        JObject json = new JObject();
        try {
            //TODO:新IM接口不需要这三个属性
            //json.put("from", this.getFrom());
            //json.put("to", this.getTo());
            //json.put("Timestamp", String.valueOf(this.getTimestamp()));
            json.Add("text", this.getText());

        } catch (Exception e) {
            Log.Error(typeof(TextMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));

    }

}
}
