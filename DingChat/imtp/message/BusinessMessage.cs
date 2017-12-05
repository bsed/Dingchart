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
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.imtp.message {
public class BusinessMessage : Message {
    public String title;
    public String date;
    public String detaiUrl;
    public String prompt;
    public List<BusinessInfoMessage> info = new List<BusinessInfoMessage>();
    public String remark;
    public String appid;

    //app相关
    public Boolean havaAppInfo = false;
    public String thirdAppName;
    public String thirdAppOpenUrl;
    public String downloadUrl;
    public List<BusinessInfoMessage> parameters = new List<BusinessInfoMessage>();

    public String getTitle() {
        return this.title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getDate() {
        return this.date;
    }

    public void setDate(String date) {
        this.date = date;
    }

    public String getUrl() {
        return this.detaiUrl;
    }

    public void setUrl(String detaiUrl) {
        this.detaiUrl = detaiUrl;
    }

    public String getPrompt() {
        return this.prompt;
    }

    public void setPrompt(String prompt) {
        this.prompt = prompt;
    }

    public List<BusinessInfoMessage> getInfo() {
        return this.info;
    }

    public void setInfo(List<BusinessInfoMessage> info) {
        this.info = info;
    }

    public String getRemark() {
        return this.remark;
    }

    public void setRemark(String remark) {
        this.remark = remark;
    }

    public String getAppid() {
        return appid;
    }

    public void setAppid(String appid) {
        this.appid = appid;
    }

    public String getDetaiUrl() {
        return detaiUrl;
    }

    public void setDetaiUrl(String detaiUrl) {
        this.detaiUrl = detaiUrl;
    }

    public Boolean isHavaAppInfo() {
        return havaAppInfo;
    }

    public void setHavaAppInfo(Boolean havaAppInfo) {
        this.havaAppInfo = havaAppInfo;
    }

    public String getThirdAppName() {
        return thirdAppName;
    }

    public void setThirdAppName(String thirdAppName) {
        this.thirdAppName = thirdAppName;
    }

    public String getThirdAppOpenUrl() {
        return thirdAppOpenUrl;
    }

    public void setThirdAppOpenUrl(String thirdAppOpenUrl) {
        this.thirdAppOpenUrl = thirdAppOpenUrl;
    }

    public String getDownloadUrl() {
        return downloadUrl;
    }

    public void setDownloadUrl(String downloadUrl) {
        this.downloadUrl = downloadUrl;
    }

    public List<BusinessInfoMessage> getParameters() {
        return parameters;
    }

    public void setParameters(List<BusinessInfoMessage> parameters) {
        this.parameters = parameters;
    }


    public override MsgType getType() {
        return MsgType.Business;
    }


    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setTitle(json.GetValue("title").ToStr());
            this.setDate(json.GetValue("date").ToStr());
            this.setUrl(json.GetValue("detailUrl").ToStr());
            this.setPrompt(json.GetValue("prompt").ToStr());
            this.setRemark(json.GetValue("remark").ToStr());
            this.setAppid(json.GetValue("appid").ToStr());

            List<BusinessInfoMessage> jsonArray = JsonConvert.DeserializeObject<List<BusinessInfoMessage>>(json["properties"].ToStr(), new convertor<BusinessInfoMessage>());
            this.info = jsonArray;

            //AccountsBean accountsBean = JsonConvert.DeserializeObject<AccountsBean>(contacts.ToStr());

            JObject appInfo = json.GetValue("appInfo").ToObject<JObject>();
            //JSONObject appInfo = json.optJSONObject("appInfo");
            if (null != appInfo) {
                this.setThirdAppName(appInfo.GetValue("thirdAppName").ToStr());
                this.setThirdAppOpenUrl(appInfo.GetValue("thirdAppOpenUrl").ToStr());
                this.setDownloadUrl(appInfo.GetValue("downloadUrl").ToStr());

                List<BusinessInfoMessage> BusinessInfoMessageArray = JsonConvert.DeserializeObject<List<BusinessInfoMessage>>(appInfo["parameters"].ToStr(), new convertor<BusinessInfoMessage>());
                this.parameters = BusinessInfoMessageArray;
                //final JSONArray p = appInfo.optJSONArray("parameters");
                //if (null != p && p.length() > 0) {
                //    for (int i = 0; i < p.length(); i++) {
                //        final JSONObject o = p.getJSONObject(i);
                //        final BusinessInfoMessage BusinessInfoMessage = new BusinessInfoMessage(ToolsHelper.isNullString(JsonHelper.getString(o, "key")), ToolsHelper.isNullString(JsonHelper.getString(o, "value")));
                //        this.parameters.add(BusinessInfoMessage);
                //    }
                //}
                if (this.thirdAppOpenUrl != null) {
                    this.havaAppInfo = true;
                } else {
                    this.havaAppInfo = false;
                }

            } else {
                this.havaAppInfo = false;
            }


        } catch (Exception e) {
            this.setParseError(true);
        }
    }


    public override String createContentJsonStr() {

        JObject json = new JObject();
        try {
            json.Add("title", this.getTitle());
            json.Add("date", this.getDate());
            json.Add("detaiUrl", this.getUrl());
            json.Add("prompt", this.getPrompt());
            json.Add("remark", this.getRemark());
            json.Add("appid", this.getAppid());
            JArray properties = new JArray();
            foreach (BusinessInfoMessage businessInfoMessage in this.getInfo()) {
                JObject obj = new JObject();
                obj.Add("key", businessInfoMessage.key);
                obj.Add("value", businessInfoMessage.value);
                properties.Add(obj);
            }
            json.Add("properties", properties);

            if (this.isHavaAppInfo()) {
                JObject appInfo = new JObject();
                //JSONObject appInfo = new JSONObject();
                appInfo.Add("thirdAppName", this.thirdAppName);
                appInfo.Add("thirdAppOpenUrl", this.thirdAppOpenUrl);
                appInfo.Add("downloadUrl", this.downloadUrl);
                if (this.parameters.Count > 0) {
                    JArray parameters = new JArray();
                    foreach (BusinessInfoMessage businessInfoMessage in this.getParameters()) {
                        JObject obj = new JObject();
                        obj.Add("key", businessInfoMessage.key);
                        obj.Add("value", businessInfoMessage.value);
                        parameters.Add(obj);
                    }
                    appInfo.Add("parameters", parameters);
                }
                json.Add("appInfo", appInfo);
            }


        } catch (Exception e) {
            Log.Error(typeof(BusinessMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));

    }

    /**
     * 将Message表content字段解析成BusinessMessage对象
     * 纯业务字段，不包含from、to等信息
     *
     * @param jsonStr 名片消息业务字段json字符串
     * @return 业务消息类，不包含共通字段
     */
    public BusinessMessage toModel(String jsonStr) {
        try {
            //JSONObject json = new JSONObject(jsonStr);
            JObject json = JObject.Parse(jsonStr);
            this.title = json.GetValue("title").ToStr();
            this.date = json.GetValue("date").ToStr();
            this.detaiUrl = json.GetValue("detaiUrl").ToStr();
            this.prompt = json.GetValue("prompt").ToStr();
            this.remark = json.GetValue("remark").ToStr();
            this.appid = json.GetValue("appid").ToStr();
            this.info.Clear();

            List<BusinessInfoMessage> jsonArray = JsonConvert.DeserializeObject<List<BusinessInfoMessage>>(json["properties"].ToStr(), new convertor<BusinessInfoMessage>());
            this.info = jsonArray;

            //JSONObject appInfo = json.optJSONObject("appInfo");
            JObject appInfo = json.GetValue("appInfo").ToObject<JObject>();
            if (null != appInfo) {
                this.thirdAppName = appInfo.GetValue("thirdAppName").ToStr();
                this.thirdAppOpenUrl = appInfo.GetValue("thirdAppOpenUrl").ToStr();
                this.downloadUrl = appInfo.GetValue("downloadUrl").ToStr();

                List<BusinessInfoMessage> BusinessInfoMessageArray = JsonConvert.DeserializeObject<List<BusinessInfoMessage>>(appInfo["parameters"].ToStr(), new convertor<BusinessInfoMessage>());
                this.parameters = BusinessInfoMessageArray;

                //final JSONArray p = appInfo.optJSONArray("parameters");
                //if (null != p && p.length() > 0) {
                //    for (int i = 0; i < p.length(); i++) {
                //        final JSONObject o = p.getJSONObject(i);
                //        final BusinessInfoMessage BusinessInfoMessage = new BusinessInfoMessage(JsonHelper.getString(o, "key"), JsonHelper.getString(o, "value"));
                //        this.parameters.add(BusinessInfoMessage);
                //    }
                //}
                if (this.thirdAppOpenUrl != null) {
                    this.havaAppInfo = true;
                } else {
                    this.havaAppInfo = false;
                }

            } else {
                this.havaAppInfo = false;
            }


            return this;
        } catch (Exception e) {
            Log.Error(typeof(BusinessMessage), e);
        }
        return null;
    }

    public String toContent() {
        JObject json = new JObject();
        //final JSONObject json = new JSONObject();
        try {
            json.Add("title", this.getTitle());
            json.Add("date", this.getDate());
            json.Add("detaiUrl", this.getUrl());
            json.Add("prompt", this.getPrompt());
            json.Add("remark", this.getRemark());
            json.Add("appid", this.getAppid());

            JArray properties = new JArray();
            foreach (BusinessInfoMessage businessInfoMessage in this.getInfo()) {
                JObject obj = new JObject();
                obj.Add("key", businessInfoMessage.key);
                obj.Add("value", businessInfoMessage.value);
                properties.Add(obj);
            }
            json.Add("properties", properties);

            if (this.isHavaAppInfo()) {
                JObject appInfo = new JObject();
                appInfo.Add("thirdAppName", this.thirdAppName);
                appInfo.Add("thirdAppOpenUrl", this.thirdAppOpenUrl);
                appInfo.Add("downloadUrl", this.downloadUrl);
                if (this.parameters.Count > 0) {
                    JArray parameters = new JArray();
                    foreach (BusinessInfoMessage businessInfoMessage in this.getParameters()) {
                        JObject obj = new JObject();
                        obj.Add("key", businessInfoMessage.key);
                        obj.Add("value", businessInfoMessage.value);
                        parameters.Add(obj);
                    }
                    appInfo.Add("parameters", parameters);
                }
                json.Add("appInfo", appInfo);
            }

            return JsonConvert.SerializeObject(json);
        } catch (Exception e) {
            Log.Error(typeof(BusinessMessage), e);
        }
        return "";

    }


}

}
