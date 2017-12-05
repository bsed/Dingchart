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
public class AVMeetingRefuseMessage : Message {

    private String avType;
    private int uId;
    private String roomId;
    private long timestamp;

    public String getAvType() {
        return avType;
    }

    public void setAvType(String avType) {
        this.avType = avType;
    }

    public int getUId() {
        return uId;
    }

    public void setUId(int uId) {
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

    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setAvType(json.GetValue("avType").ToStr());
            this.setUId(int.Parse(json.GetValue("uId").ToStr()));
            this.setRoomId(json.GetValue("roomId").ToStr());
            this.setTimestamp(long.Parse(json.GetValue("timestamp").ToStr()));
        } catch (Exception e) {
            this.setParseError(true);
        }

    }

    public override String createContentJsonStr() {
        JObject json = new JObject();
        try {
            json.Add("avType", this.getAvType());
            json.Add("uId", this.getUId());
            json.Add("roomId", this.getRoomId());
            json.Add("timestamp", this.getTimestamp());
        } catch (Exception ex) {
            Log.Error(typeof(AVMeetingRefuseMessage), ex);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }

}
}
