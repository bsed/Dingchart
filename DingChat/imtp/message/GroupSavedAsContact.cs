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
public class GroupSavedAsContact : Message {

    private String groupNo;
    private bool savedAsContact;
    private string device;

    public String getDevice() {
        return device;
    }

    public void setDevice(String device) {
        this.device = device;
    }

    public String getGroupNo() {
        return groupNo;
    }

    public void setGroupNo(String groupNo) {
        this.groupNo = groupNo;
    }

    public bool getSavedAsContact() {
        return savedAsContact;
    }

    public void setSavedAsContact(bool savedAsContact) {
        this.savedAsContact = savedAsContact;
    }

    public override MsgType getType() {
        return MsgType.GroupSavedAsContact;
    }



    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);

            this.setDevice(json.GetValue("device").ToStr());
            this.setGroupNo(json.GetValue("groupNo").ToStr());
            if (json.Property("savedAsContact") != null) {
                this.setSavedAsContact(json.GetValue("savedAsContact").ToStr().ToBool());
            }

        } catch (Exception e) {
            Log.Error(typeof(GroupSavedAsContact), e);
            this.setParseError(true);
        }
    }

    public override String createContentJsonStr() {
        JObject json = new JObject();
        try {
            json.Add("groupNo", this.getGroupNo());
            json.Add("savedAsContact", this.getSavedAsContact());
            json.Add("device", this.getDevice());
        } catch (Exception ex) {
            Log.Error(typeof(GroupSavedAsContact), ex);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }
}
}
