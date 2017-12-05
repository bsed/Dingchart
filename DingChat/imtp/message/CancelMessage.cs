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
public class CancelMessage : Message {
    /**
    * 撤销消息的服务端ID
    */
    public String serverMessageId;


    public MsgType getType() {
        return MsgType.Cancel;
    }


    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject obj = JObject.Parse((String) sendMessage.getMessage());
            String serverMessageId = obj.GetValue("serverMessageId").ToStr();
            base.parse(type, sendMessage);
            this.setType(type);
            this.setServerMessageId(serverMessageId);
        } catch (Exception e) {
            Log.Error(typeof(CancelMessage), e);
            this.setParseError(true);
        }
    }

    public override String createContentJsonStr() {
        JObject json = new JObject();
        try {
            json.Add("serverMessageId", this.getServerMessageId());
        } catch (Exception e) {
            Log.Error(typeof(CancelMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }

    /**
     * 将消息转换成Message表中的content字段，只包含业务字段
     *
     * @return 代金卷消息业务字段的JSON串
     */
    public String toContent() {
        try {
            JObject json = new JObject();
            json.Add("serverMessageId", this.getServerMessageId());
            return JsonConvert.SerializeObject(json);
        } catch (Exception e) {
            Log.Error(typeof(CancelMessage), e);
        }
        return "";
    }

    /**
     * 将Message表content字段解析成Message对象
     * 纯业务字段，不包含from、to等信息
     *
     * @param jsonStr 消息业务字段的JSON串
     * @return 消息类，不包含共通字段
     */
    public CancelMessage toModel(String jsonStr) {
        try {
            JObject obj = JObject.Parse(jsonStr);
            this.serverMessageId = obj.GetValue("serverMessageId").ToStr();
            return this;
        } catch (Exception e) {
            Log.Error(typeof(CancelMessage), e);
        }
        return null;
    }

    public String getServerMessageId() {
        return serverMessageId;
    }

    public void setServerMessageId(String serverMessageId) {
        this.serverMessageId = serverMessageId;
    }
}
}
