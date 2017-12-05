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
class LocationMessage : Message {
    public String address {
        get;
        set;
    }
    //Base64编码缩略图内容
    public String picture {
        get;
        set;
    }

    public BitmapImage BitmapPicture {
        get;
        set;
    }

    public double longitude {
        get;
        set;
    }

    public double latitude {
        get;
        set;
    }

    public int scale {
        get;
        set;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getPicture() {
        return this.picture;
    }

    public void setPicture( String picture) {
        this.picture = picture;
    }

    public double getLongitude() {
        return longitude;
    }

    public void setLongitude(double longitude) {
        this.longitude = longitude;
    }

    public double getLatitude() {
        return latitude;
    }

    public void setLatitude(double latitude) {
        this.latitude = latitude;
    }

    public int getScale() {
        return scale;
    }

    public void setScale(int scale) {
        this.scale = scale;
    }


    public override MsgType getType() {
        return MsgType.Location;
    }


    public override void parse( MsgType type,  SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setAddress(json.GetValue("address").ToStr());
            this.setPicture(json.GetValue("picture").ToStr());
            if (json.Property("longitude") != null) {

                this.setLongitude(json.GetValue("longitude").ToObject<double>());
            }
            if (json.Property("latitude") != null) {

                this.setLatitude(json.GetValue("latitude").ToObject<double>());
            }
            if (json.Property("scale") != null) {

                this.setScale(json.GetValue("scale").ToObject<int>());
            }

        } catch ( Exception e) {
            Log.Error(typeof(LocationMessage), e);
            this.setParseError(true);
        }

    }


    public override String createContentJsonStr() {

        JObject json = new JObject();
        try {
            json.Add("address", this.getAddress());
            json.Add("longitude", this.getLongitude());
            json.Add("latitude", this.getLatitude());
            json.Add("picture", this.getPicture());
            json.Add("scale", this.getScale());
        } catch (Exception e) {
            Log.Error(typeof(LocationMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }

    /**
     * 将Message表content字段解析成LocationMessage对象
     * 纯业务字段，不包含from、to等信息
     *
     * @param jsonStr 位置消息业务字段json字符串
     * @return 位置消息类，不包含共通字段
     */
    public LocationMessage toModel(String jsonStr) {

        try {
            JObject json = JObject.Parse(jsonStr);

            this.setAddress(json.GetValue("address").ToStr());
            this.setPicture(json.GetValue("picture").ToStr());
            if (json.Property("longitude") != null) {

                this.setLongitude(json.GetValue("longitude").ToObject<double>());
            }
            if (json.Property("latitude") != null) {

                this.setLatitude(json.GetValue("latitude").ToObject<double>());
            }
            if (json.Property("scale") != null) {

                this.setScale(json.GetValue("scale").ToObject<int>());
            }
            return this;
        } catch(Exception e) {
            Log.Error(typeof(LocationMessage), e);
        }
        return null;
    }

    /**
     * 将位置消息转换成Message表中的content字段，只包含业务字段
     *
     * @return 名位置消息业务字段的JSON串
     */
    public String toContent() {
        try {
            JObject json = new JObject();
            json.Add("address", address);
            json.Add("picture", picture);
            json.Add("longitude", longitude);
            json.Add("latitude", latitude);
            json.Add("scale", scale);
            return JsonConvert.SerializeObject(json);
        } catch(Exception e) {
            Log.Error(typeof(LocationMessage), e);
        }
        return "";
    }
}
}
