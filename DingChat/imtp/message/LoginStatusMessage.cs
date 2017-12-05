﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.im.sdk.bean;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using Newtonsoft.Json;

namespace cn.lds.chatcore.pcw.imtp.message {
class LoginStatusMessage : Message {
    public String clientId;         // PC的no
    public long sendTimestamp;      // 时间戳
    public String checkCode;        // PC发给手机的校验码（每次展示二维码时生成）
    public String status;           // 0：已退出，1：已登录

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


    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public override MsgType getType() {
        return MsgType.LoginStatus;
    }

    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setClientId(json.GetValue("clientId").ToStr());
            this.setSendTimestamp(long.Parse(json.GetValue("sendTimestamp").ToStr()));
            this.setCheckCode(json.GetValue("checkCode").ToStr());
            this.setStatus(json.GetValue("status").ToStr());
        } catch (Exception e) {
            Log.Error(typeof(LoginStatusMessage), e);
            this.setParseError(true);
        }
    }

    public override String createContentJsonStr() {

        JObject json = new JObject();
        try {
            json.Add("clientId", this.getClientId());
            json.Add("sendTimestamp", this.getSendTimestamp());
            json.Add("checkCode", this.getCheckCode());
            json.Add("status", this.getStatus());
        } catch (Exception e) {
            Log.Error(typeof(AtMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }
}
}