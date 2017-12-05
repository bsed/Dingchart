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
class ThirdAppClassChangedMessage : Message {

    public override MsgType getType() {
        return MsgType.ThirdAppClassChanged;
    }


    public override String createContentJsonStr() {
        return null;
    }

    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            base.parse(type, sendMessage);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppClassChangedMessage), e);
            this.setParseError(true);
        }
    }
}
}
