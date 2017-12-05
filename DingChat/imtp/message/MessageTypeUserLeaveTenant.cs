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
public class MessageTypeUserLeaveTenant : Message {
    private String tenantNo;

    private String tenantName;


    public String getTenantNo() {
        return tenantNo;
    }

    public void setTenantNo(String tenantNo) {
        this.tenantNo = tenantNo;
    }



    public String getTenantName() {
        return tenantName;
    }

    public void setTenantName(String tenantName) {
        this.tenantName = tenantName;
    }



    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setTenantNo(json.GetValue("tenantNo").ToStr());
            this.setTenantName(json.GetValue("tenantName").ToStr());

        } catch (Exception e) {
            this.setParseError(true);
        }

    }

    public override String createContentJsonStr() {
        JObject json = new JObject();
        try {
            json.Add("tenantNo", this.getTenantNo());

            json.Add("tenantName", this.getTenantName());

        } catch (Exception ex) {
            Log.Error(typeof(MessageTypeUserLeaveTenant), ex);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }
}
}
