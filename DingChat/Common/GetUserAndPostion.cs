using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models;

namespace cn.lds.chatcore.pcw.Common {
public class GetUserAndPostion {

    public static UserAndPostion Get(MessageItem itemBean) {
        UserAndPostion ret = new UserAndPostion();
        string msgType = itemBean.type;
        if (msgType == MsgType.Notify.ToStr() || msgType == MsgType.Cancel.ToStr()) {
            ret.UserNo = string.Empty;
            ret.Postion = MessagePostionType.Center;

        } else if (itemBean.incoming && itemBean.resource != itemBean.user) {
            //对方说话
            ret.UserNo = itemBean.user;
            ret.Postion = MessagePostionType.Left;

        } else {
            //自己说话
            ret.UserNo = itemBean.resource;
            ret.Postion = MessagePostionType.Right;
        }
        return ret;
    }
}
}
