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
using cn.lds.chatcore.pcw.Common.Utils;
namespace cn.lds.chatcore.pcw.imtp.message {
public class AtMessage : Message {
    public String text;
    public List<String> atNos;
    public String senderGroupNickname;

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }

    public MsgType getType() {
        return MsgType.At;
    }

    public List<String> getAtNos() {
        return atNos;
    }

    public void setAtNos(List<String> atNos) {
        this.atNos = atNos;
    }

    public String getSenderGroupNickname() {
        return senderGroupNickname;
    }

    public void setSenderGroupNickname(String senderGroupNickname) {
        this.senderGroupNickname = senderGroupNickname;
    }


    public  override void parse(MsgType type,  SendMessage sendMessage) {

        try {
            JObject obj = JObject.Parse((String) sendMessage.getMessage());
            base.parse(type, sendMessage);

            this.setText(obj.GetValue("text").ToStr());

            List<JToken> atNosList = obj["atNos"].Children().ToList();
            List<String> atNos = new List<String>();
            foreach(JToken token in atNosList) {
                atNos.Add(token.ToStr());
            }

            this.setAtNos(atNos);

            this.setSenderGroupNickname(obj.GetValue("senderGroupNickname").ToStr());
        } catch (Exception e) {
            Log.Error(typeof(AtMessage), e);
            this.setParseError(true);
        }
    }


    public override String createContentJsonStr() {

        JObject json = new JObject();
        try {
            json.Add("text", this.getText());
            JArray jarray = new JArray();
            jarray.Add(this.getAtNos());
            json.Add("atNos", jarray);
            json.Add("senderGroupNickname", this.getSenderGroupNickname());
        } catch (Exception e) {
            Log.Error(typeof(AtMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }

    /**
    * 将Message表content字段解析成AtMessage对象
    * 纯业务字段，不包含from、to等信息
    *
    * @param jsonStr 语音消息业务字段json字符串
    * @return 语片消息类，不包含共通字段
    */
    public AtMessage toModel(String jsonStr) {

        try {
            JObject json = JObject.Parse(jsonStr);
            if (json.Property("text") != null) {
                this.setText(json.GetValue("text").ToStr());
            }
            if (json.Property("senderGroupNickname") != null) {
                this.setSenderGroupNickname(json.GetValue("senderGroupNickname").ToStr());
            }
            if (json.Property("atNos") != null) {
                List<String > list = json.GetValue("atNos").ToObject<List<String>>();
                this.setAtNos(list);
            }
            return this;
        } catch (Exception e) {
            Log.Error(typeof(AtMessage), e);
        }
        return null;
    }

    /**
    * 将at消息转换成Message表中的content字段，只包含业务字段
    *
    * @return 语片消息业务字段的JSON串
    */
    public String toContent() {
        try {
            JObject json = new JObject();
            json.Add("text", this.getText());
            JArray jarray = new JArray();
            jarray.Add(this.getAtNos());
            json.Add("atNos", jarray);
            json.Add("senderGroupNickname", this.getSenderGroupNickname());
            return JsonConvert.SerializeObject(json);
        } catch (Exception e) {
            Log.Error(typeof(AtMessage), e);
        }
        return "";
    }
}
}
