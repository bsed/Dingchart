using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using cn.lds.im.sdk;
using cn.lds.im.sdk.notification;
using cn.lds.im.sdk.api;
using cn.lds.im.sdk.enums;
using cn.lds.im.sdk.bean;
using cn.lds.im.sdk.message.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.imtp.message {
public class PublicCardMessage : Message {
    /**
     * 公众号ID
     */
    public String appId {
        get;
        set;
    }
    /**
     * 公众号名称
     */
    public String name {
        get;
        set;
    }
    /**
     * 公众号图片
     */
    public string logoId {
        get;
        set;
    }

    public BitmapImage logoPath {
        get;
        set;
    }

    /**
     *功能介绍
     */
    public String introduction {
        get;
        set;
    }
    /**
     *主体名称
     */
    public String ownerName {
        get;
        set;
    }

    public String tenantNo {
        get;
        set;
    }
    //    private boolean includeSubscription;
    //    private boolean includeWebsite;

    public String getTenantNo() {
        return tenantNo;
    }

    public void setTenantNo(String tenantNo) {
        this.tenantNo = tenantNo;
    }


    public String getAppId() {
        return appId;
    }

    public void setAppId(String appId) {
        this.appId = appId;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public string getLogoId() {
        return logoId;
    }

    public void setLogoId(string logoId) {
        this.logoId = logoId;
    }

    public String getIntroduction() {
        return introduction;
    }

    public void setIntroduction(String introduction) {
        this.introduction = introduction;
    }

    public String getOwnerName() {
        return ownerName;
    }

    public void setOwnerName(String ownerName) {
        this.ownerName = ownerName;
    }


    public override void parse( MsgType type,  SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);

            this.setType(type);
            this.setAppId(json.GetValue("appId").ToStr());
            this.setName(json.GetValue("name").ToStr());
            if (json.Property("logoId") != null) {
                this.setLogoId(json.GetValue("logoId").ToStr());
            }

            this.setIntroduction(json.GetValue("introduction").ToStr());
            this.setOwnerName(json.GetValue("ownerName").ToStr());
            this.setTenantNo(json.GetValue("tenantNo").ToStr());
        } catch ( Exception e) {
            Log.Error(typeof(PublicCardMessage), e);
            this.setParseError(true);
        }
    }


    public override String createContentJsonStr() {
        JObject json = new JObject();
        try {
            json.Add("appId", this.getAppId());
            json.Add("name", this.getName());
            json.Add("logoId", this.getLogoId());
            json.Add("introduction", this.getIntroduction());
            json.Add("ownerName", this.getOwnerName());
            json.Add("tenantNo", this.getTenantNo());
        } catch (Exception e) {
            Log.Error(typeof(PublicCardMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }

    /**
     * 将消息转换成Message表中的content字段，只包含业务字段
     *
     * @return 代金卷消息业务字段的JSON串
     */
    public String toContent() {
        try {
            JObject json = new JObject();
            json.Add("appId", this.getAppId());
            json.Add("name", this.getName());
            json.Add("logoId", this.getLogoId());
            json.Add("introduction", this.getIntroduction());
            json.Add("ownerName", this.getOwnerName());
            json.Add("tenantNo", this.getTenantNo());
            return JsonConvert.SerializeObject(json);
        } catch (Exception e) {
            Log.Error(typeof(PublicCardMessage), e);
        }
        return "";
    }

    /**
     * 将Message表content字段解析成消息对象
     * 纯业务字段，不包含from、to等信息
     *
     * @param jsonStr 消息业务字段的JSON串
     * @return 消息类，不包含共通字段
     */
    public PublicCardMessage toModel(String jsonStr) {
        try {
            JObject json = JObject.Parse(jsonStr);
            this.setAppId(json.GetValue("appId").ToStr());
            this.setName(json.GetValue("name").ToStr());
            if (json.Property("logoId") != null) {
                this.setLogoId(json.GetValue("logoId").ToStr());
            }

            this.setIntroduction(json.GetValue("introduction").ToStr());
            this.setOwnerName(json.GetValue("ownerName").ToStr());
            this.setTenantNo(json.GetValue("tenantNo").ToStr());
            return this;
        } catch (Exception e) {
            Log.Error(typeof(PublicCardMessage), e);
        }
        return null;
    }
}
}
