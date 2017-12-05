using cn.lds.chatcore.pcw.Beans;
using cn.lds.im.sdk.bean;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Utils;
using ikvm.extensions;
using java.lang;
using Exception = System.Exception;
using String = System.String;
using Newtonsoft.Json;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.imtp.message {
public class AppMessage : Message {

    public String title;
    public String date;
    public String pcDetailUrl;
    public List<BusinessInfoMessage> info = new List<BusinessInfoMessage>();
    public String appid;

    //app相关
    public String thirdAppName;
    public String thirdAppLogoId;
    public String thirdAppOpenUrl;
    public String downloadUrl;
    public List<BusinessInfoMessage> parameters = new List<BusinessInfoMessage>();

    public String getTitle() {
        return this.title;
    }

    public void setTitle( String title) {
        this.title = title;
    }

    public String getDate() {
        return this.date;
    }

    public void setDate( String date) {
        this.date = date;
    }

    public List<BusinessInfoMessage> getInfo() {
        return this.info;
    }

    public void setInfo( List<BusinessInfoMessage> info) {
        this.info = info;
    }

    public String getAppid() {
        return appid;
    }

    public void setAppid(String appid) {
        this.appid = appid;
    }

    public String getPcDetailUrl() {
        return pcDetailUrl;
    }

    public void setPcDetailUrl(String pcDetailUrl) {
        this.pcDetailUrl = pcDetailUrl;
    }

    public String getThirdAppLogoId() {
        return thirdAppLogoId;
    }

    public void setThirdAppLogoId(String thirdAppLogoId) {
        this.thirdAppLogoId = thirdAppLogoId;
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
        return MsgType.App;
    }


    public override void parse( MsgType type,  SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String) sendMessage.getMessage());
            base.parse(type, sendMessage);

            this.setTitle(json.GetValue("title").ToStr());
            this.setDate(json.GetValue("date").ToStr());
            this.setPcDetailUrl(json.GetValue("pcDetailUrl").ToStr());
            this.setAppid(json.GetValue("appid").ToStr());

            List<BusinessInfoMessage> jsonArray = JsonConvert.DeserializeObject<List<BusinessInfoMessage>>(json["properties"].ToStr(), new convertor<BusinessInfoMessage>());

            this.info = jsonArray;

            JObject appInfo = JObject.Parse((String) json["appInfo"].ToStr());
            if (null != appInfo) {
                this.setThirdAppName(appInfo.GetValue("thirdAppName").ToStr());
                this.setThirdAppLogoId(appInfo.GetValue("thirdAppLogoId").ToStr());
                this.setThirdAppOpenUrl(appInfo.GetValue("thirdAppOpenUrl").ToStr());
                this.setDownloadUrl(appInfo.GetValue("downloadUrl").ToStr());

                List<BusinessInfoMessage> BusinessInfoMessageArray = JsonConvert.DeserializeObject<List<BusinessInfoMessage>>(appInfo["parameters"].ToStr(), new convertor<BusinessInfoMessage>());
                this.parameters = BusinessInfoMessageArray;
            }

        } catch ( Exception e) {
            this.setParseError(true);
        }
    }

    /// <summary>
    /// TODO 安卓端这个代码干啥用的，暂时没删除哈
    /// </summary>
    /// <returns></returns>
    public String getAppInfo() {
        JObject appInfo = new JObject();
        try {
            appInfo.Add("thirdAppName", this.thirdAppName);
            appInfo.Add("thirdAppLogoId", this.thirdAppLogoId);
            appInfo.Add("thirdAppOpenUrl", this.thirdAppOpenUrl);
            appInfo.Add("downloadUrl", this.downloadUrl);
            if (this.parameters.Count > 0) {
                appInfo.Add("parameters", new JArray(this.getParameters()));
            }
        } catch (Exception ex) {
            Log.Error(typeof(AppMessage), ex);
        }
        return JsonConvert.SerializeObject(appInfo);
    }

    public override String createContentJsonStr() {

        JObject json = new JObject();
        try {
            json.Add("title", this.getTitle());
            json.Add("date", this.getDate());
            json.Add("pcDetailUrl", this.getPcDetailUrl());
            json.Add("appid", this.getAppid());
            JArray properties = new JArray();
            foreach (BusinessInfoMessage businessInfoMessage in this.getInfo()) {
                JObject obj = new JObject();
                obj.Add("key", businessInfoMessage.key);
                obj.Add("value", businessInfoMessage.value);
                properties.Add(obj);
            }
            json.Add("properties", properties);

            JObject appInfo = new JObject();
            appInfo.Add("thirdAppName", this.thirdAppName);
            appInfo.Add("thirdAppLogoId", this.thirdAppLogoId);
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


        } catch (Exception ex) {
            Log.Error(typeof(AppMessage), ex);
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
    public AppMessage toModel(String jsonStr) {
        try {
            JObject json = new JObject(jsonStr);
            this.title = json.GetValue("title").ToStr();
            this.date = json.GetValue("date").ToStr();
            this.pcDetailUrl = json.GetValue("pcDetailUrl").ToStr();
            this.appid = json.GetValue("appid").ToStr();
            this.info.Clear();

            List<BusinessInfoMessage> jsonArray = JsonConvert.DeserializeObject<List<BusinessInfoMessage>>(json["properties"].ToStr(), new convertor<BusinessInfoMessage>());
            this.info = jsonArray;
            JObject appInfo = json.GetValue("appInfo").ToObject<JObject>();
            if (null != appInfo) {
                this.thirdAppName = appInfo.GetValue("thirdAppName").ToStr();
                this.thirdAppOpenUrl = appInfo.GetValue("thirdAppOpenUrl").ToStr();
                this.downloadUrl = appInfo.GetValue("downloadUrl").ToStr();
                this.thirdAppLogoId = appInfo.GetValue("thirdAppLogoId").ToStr();

                List<BusinessInfoMessage> BusinessInfoMessageArray = JsonConvert.DeserializeObject<List<BusinessInfoMessage>>(appInfo["parameters"].ToStr(), new convertor<BusinessInfoMessage>());
                this.parameters = BusinessInfoMessageArray;
            }


            return this;
        } catch (Exception e) {
            Log.Error(typeof(AppMessage), e);
        }
        return null;
    }

    public String toContent() {

        JObject json = new JObject();
        try {
            json.Add("title", this.getTitle());
            json.Add("date", this.getDate());
            json.Add("pcDetailUrl", this.getPcDetailUrl());
            json.Add("appid", this.getAppid());

            JArray properties = new JArray();
            foreach (BusinessInfoMessage businessInfoMessage in this.getInfo()) {
                JObject obj = new JObject();
                obj.Add("key", businessInfoMessage.key);
                obj.Add("value", businessInfoMessage.value);
                properties.Add(obj);
            }
            json.Add("properties", properties);

            JObject appInfo = new JObject();
            appInfo.Add("thirdAppName", this.thirdAppName);
            appInfo.Add("thirdAppOpenUrl", this.thirdAppOpenUrl);
            appInfo.Add("downloadUrl", this.downloadUrl);
            appInfo.Add("thirdAppLogoId", this.thirdAppLogoId);
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

            return JsonConvert.SerializeObject(json);
        } catch (Exception ex) {
            Log.Error(typeof(AppMessage), ex);
        }
        return "";

    }


}
}
