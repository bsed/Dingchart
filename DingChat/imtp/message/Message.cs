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

namespace cn.lds.chatcore.pcw.imtp.message {
public abstract class Message {

    public String messageId;
    public String from;
    public String to;

    public MsgType type = MsgType.UNKNOWN;
    public long timestamp;

    public bool parseError = false;


    public abstract String createContentJsonStr();

    public string tenantNo;

    public virtual void parse(MsgType type, SendMessage sendMessage) {
        this.setType(type);
        this.setMessageId(sendMessage.getMessageId());
        this.setFrom(sendMessage.getFromClientId());
        this.setTo(sendMessage.getToClientId());
        this.setTimestamp(sendMessage.getTime());
        this.setTenantNo(sendMessage.getTenantNo());
    }


    protected String addContentHeader(String jsonStr) {
        /*
        StringBuffer buffer = new StringBuffer();
        buffer.append(this.getType().value());
        buffer.append(jsonStr);
        return buffer.toString();
        */
        return jsonStr;
    }


    public String getMessageId() {
        return messageId;
    }

    public void setMessageId(String messageId) {
        this.messageId = messageId;
    }

    public String getFrom() {
        return from;
    }

    public void setFrom(String from) {
        this.from = from;
    }

    public String getTo() {
        return to;
    }

    public String getTenantNo() {
        return tenantNo;
    }


    public void setTo(String to) {
        this.to = to;
    }

    public virtual MsgType getType() {
        return this.type;
    }

    public void setType(MsgType type) {
        this.type = type;
    }

    public bool isParseError() {
        return parseError;
    }

    public void setParseError(bool parseError) {
        this.parseError = parseError;
    }

    public long getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(long timestamp) {
        this.timestamp = timestamp;
    }

    public void setTenantNo(string tenantNo) {
        this.tenantNo = tenantNo;
    }
}
}
