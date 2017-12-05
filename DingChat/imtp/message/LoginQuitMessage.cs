using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.im.sdk.bean;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.imtp.message {
class LoginQuitMessage : Message {

    public long sendTimestamp;  // 时间戳

    public long getSendTimestamp() {
        return sendTimestamp;
    }

    public void setSendTimestamp(long sendTimestamp) {
        this.sendTimestamp = sendTimestamp;
    }

    public override MsgType getType() {
        return MsgType.LoginQuit;
    }
    public override String createContentJsonStr() {
        return null;
    }
    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setSendTimestamp(long.Parse(json.GetValue("sendTimestamp").ToStr()));

        } catch (Exception e) {
            Log.Error(typeof(LoginQuitMessage), e);
            this.setParseError(true);
        }
    }
}
}
