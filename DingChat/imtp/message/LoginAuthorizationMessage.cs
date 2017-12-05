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

class LoginAuthorizationMessage : Message {


    public String clientId;     // 手机的no
    public long sendTimestamp;  // 时间戳
    public String checkCode;    // PC发给手机的校验码（每次展示二维码时生成）
    public String mobile;   // 手机号
    public String captcha;   // 验证码



    public String getClientId() {
        return clientId;
    }

    public void setClientId(String clientId) {
        this.clientId = clientId;
    }

    public long getSendTimestamp() {
        return sendTimestamp;
    }

    public void setSendTimestamp(long sendTimestamp) {
        this.sendTimestamp = sendTimestamp;
    }

    public String getCheckCode() {
        return checkCode;
    }

    public void setCheckCode(String checkCode) {
        this.checkCode = checkCode;
    }

    public String getMobile() {
        return mobile;
    }

    public void setMobile(String mobile) {
        this.mobile = mobile;
    }

    public String getCaptcha() {
        return captcha;
    }

    public void setCaptcha(String captcha) {
        this.captcha = captcha;
    }

    public override MsgType getType() {
        return MsgType.LoginAuthorization;
    }
    public override String createContentJsonStr() {
        return null;
    }
    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setClientId(json.GetValue("clientId").ToStr());
            this.setSendTimestamp(long.Parse(json.GetValue("sendTimestamp").ToStr()));
            this.setCheckCode(json.GetValue("checkCode").ToStr());
            this.setMobile(json.GetValue("mobile").ToStr());
            this.setCaptcha(json.GetValue("captcha").ToStr());

        } catch (Exception e) {
            Log.Error(typeof(LoginAuthorizationMessage), e);
            this.setParseError(true);
        }
    }
}
}
