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
public class UserDisabledMessage : Message {
    /**
    * 返回消息类型
    * @return
    */
    public override MsgType getType() {
        return MsgType.UserDisabled;
    }

    /**
     * 填充消息属性
     * @param type
     * @param sendMessage
     */

    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            //JObject obj = JObject.Parse((String) sendMessage.getMessage());
            //String text = obj.GetValue("text").ToString();
            base.parse(type, sendMessage);

        } catch (Exception e) {
            Log.Error(typeof(UserDisabledMessage), e);
            this.setParseError(true);
        }
    }

    /**
     * 构建JSON串（通知类消息、不用保存到数据库，所以未实现方法体）
     * @return
     */

    public override String createContentJsonStr() {
        return null;
    }
}
}
