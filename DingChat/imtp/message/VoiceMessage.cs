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
class VoiceMessage : Message {
    /** 语音文件存储ID */
    public string voiceStorageId;
    /** 音频时长 */
    public long duration;

    public string getVoiceStorageId() {
        return voiceStorageId;
    }

    public void setVoiceStorageId(string voiceStorageId) {
        this.voiceStorageId = voiceStorageId;
    }

    public long getDuration() {
        return duration;
    }

    public void setDuration(long duration) {
        this.duration = duration;
    }

    public MsgType getType() {
        return MsgType.Voice;
    }


    public override void parse( MsgType type,  SendMessage sendMessage) {
        try {

            JObject json = JObject.Parse((String) sendMessage.getMessage());
            base.parse(type, sendMessage);
            if (json.Property("voiceStorageId") != null) {
                this.setVoiceStorageId(json.GetValue("voiceStorageId").ToStr());
            }
            if (json.Property("duration") != null) {
                this.setDuration(json.GetValue("duration").ToObject<int>());
            }



        } catch ( Exception e) {
            Log.Error(typeof(VoiceMessage), e);
            this.setParseError(true);
        }
    }


    public override String createContentJsonStr() {
        JObject json = new JObject();
        try {
            json.Add("voiceStorageId", this.getVoiceStorageId());
            json.Add("duration", this.getDuration());
        } catch (Exception e) {
            Log.Error(typeof(VoiceMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }

    /**
     * 将Message表content字段解析成VoiceMessage对象
     * 纯业务字段，不包含from、to等信息
     *
     * @param jsonStr 语音消息业务字段json字符串
     * @return 语片消息类，不包含共通字段
     */
    public VoiceMessage toModel(String jsonStr) {

        try {

            JObject json = JObject.Parse(jsonStr);
            if (json.Property("voiceStorageId") != null) {
                this.setVoiceStorageId(json.GetValue("voiceStorageId").ToStr());
            }
            if (json.Property("duration") != null) {
                this.setDuration(json.GetValue("duration").ToObject<int>());
            }


            return this;
        } catch(Exception e) {
            Log.Error(typeof(VoiceMessage), e);
        }
        return null;
    }

    /**
     * 将语片消息转换成Message表中的content字段，只包含业务字段
     *
     * @return 语片消息业务字段的JSON串
     */
    public String toContent() {
        try {
            JObject json = new JObject();
            json.Add("voiceStorageId", voiceStorageId);
            json.Add("duration", duration);
            return JsonConvert.SerializeObject(json);
        } catch(Exception e) {
            Log.Error(typeof(VoiceMessage), e);
        }
        return "";
    }
}
}
