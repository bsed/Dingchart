using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.im.sdk.bean;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using Newtonsoft.Json;
/// <summary>
/// 发送已读消息
/// </summary>
namespace cn.lds.chatcore.pcw.imtp.message {
class ReadMessage : Message {
    public String userNo;         // PC的no
    public long sendTimestamp;      // 时间戳
    private string device;

    public String getDevice() {
        return device;
    }

    public void setDevice(String device) {
        this.device = device;
    }

    public String getUserNo() {
        return userNo;
    }

    public void setUserNo(String userNo) {
        this.userNo = userNo;
    }

    public long getSendTimestamp() {
        return sendTimestamp;
    }

    public void setSendTimestamp(long sendTimestamp) {
        this.sendTimestamp = sendTimestamp;
    }




    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setUserNo(json.GetValue("userNo").ToStr());
            this.setSendTimestamp(long.Parse(json.GetValue("sendTimestamp").ToStr()));
            this.setDevice(json.GetValue("device").ToStr());
        } catch (Exception e) {
            Log.Error(typeof(ReadMessage), e);
            this.setParseError(true);
        }
    }

    public override String createContentJsonStr() {

        JObject json = new JObject();
        try {
            json.Add("userNo", this.getUserNo());
            json.Add("sendTimestamp", this.getSendTimestamp());
            json.Add("device", this.getDevice());
        } catch (Exception e) {
            Log.Error(typeof(AtMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }
}
}
