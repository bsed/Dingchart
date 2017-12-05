using cn.lds.im.sdk.bean;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Common;
using Newtonsoft.Json;

namespace cn.lds.chatcore.pcw.imtp.message {
public class AVMeetingInviteMessage : Message {
    private String avType;
    private uint uId;
    private String roomId;
    private long timestamp;

    public String getAvType() {
        return avType;
    }

    public void setAvType(String avType) {
        this.avType = avType;
    }

    public uint getUId() {
        return uId;
    }

    public void setUId(uint uId) {
        this.uId = uId;
    }

    public String getRoomId() {
        return roomId;
    }

    public void setRoomId(String roomId) {
        this.roomId = roomId;
    }

    public long getTimestamp() {
        return timestamp;
    }


    public void setTimestamp(long timestamp) {
        this.timestamp = timestamp;
    }

    public override MsgType getType() {
        return MsgType.AVMeetingInvite;
    }

    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setAvType(json.GetValue("avType").ToStr());
            this.setUId(uint.Parse(json.GetValue("uId").ToStr()));
            this.setRoomId(json.GetValue("roomId").ToStr());
            this.setTimestamp(long.Parse(json.GetValue("timestamp").ToStr()));
        } catch (Exception e) {
            this.setParseError(true);
        }

    }
    /// <summary>
    /// 当前对象转json
    /// </summary>
    /// <returns></returns>
    public override String createContentJsonStr() {
        JObject json = new JObject();
        try {
            json.Add("avType", this.getAvType());
            json.Add("uId", this.getUId());
            json.Add("roomId", this.getRoomId());
            json.Add("timestamp", this.getTimestamp());
        } catch (Exception ex) {
            Log.Error(typeof(AVMeetingInviteMessage), ex);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }

    /**
    * 将Message表content字段解析成PictureMessage对象
    * 纯业务字段，不包含from、to等信息
    *
    * @param jsonStr 图片消息业务字段json字符串
    * @return 图片消息类，不包含共通字段
    */
    public AVMeetingInviteMessage toModel(String jsonStr) {

        try {
            JObject json = JObject.Parse(jsonStr);
            this.avType = json.GetValue("avType").ToStr();
            this.uId = uint.Parse(json.GetValue("uId").ToStr());
            this.roomId = json.GetValue("roomId").ToStr();
            this.timestamp = long.Parse(json.GetValue("timestamp").ToStr());
            return this;
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingInviteMessage), e);
        }
        return null;
    }

    public String toContent() {
        JObject json = new JObject();
        try {
            json.Add("avType", this.getAvType());
            json.Add("uId", this.getUId());
            json.Add("roomId", this.getRoomId());
            json.Add("timestamp", this.getTimestamp());
        } catch (Exception ex) {
            Log.Error(typeof(AVMeetingInviteMessage), ex);
        }
        return JsonConvert.SerializeObject(json);
    }
}
}
