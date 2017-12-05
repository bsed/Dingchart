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
using cn.lds.im.sdk.message.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.imtp.message {
public class PictureMessage : Message {
    /*  */
    public String imageStorageId;
    //Base64编码缩略图内容
    public String thumbnail;

    public int thumbnailWidth;

    public int thumbnailHeight;

    public String getImageStorageId() {
        return imageStorageId;
    }

    public void setImageStorageId(String imageStorageId) {
        this.imageStorageId = imageStorageId;
    }

    public String getThumbnail() {
        return thumbnail;
    }

    public void setThumbnail(String thumbnail) {
        this.thumbnail = thumbnail;
    }

    public int getThumbnailWidth() {
        return thumbnailWidth;
    }

    public void setThumbnailWidth(int thumbnailWidth) {
        this.thumbnailWidth = thumbnailWidth;
    }

    public int getThumbnailHeight() {
        return thumbnailHeight;
    }

    public void setThumbnailHeight(int thumbnailHeight) {
        this.thumbnailHeight = thumbnailHeight;
    }


    public override MsgType getType() {
        return MsgType.Image;
    }


    public override void parse( MsgType type,  SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);

            this.setImageStorageId(json.GetValue("imageStorageId").ToStr());
            this.setThumbnail(json.GetValue("thumbnail").ToStr());
            if (json.Property("thumbnailWidth") != null) {
                this.setThumbnailWidth(json.GetValue("thumbnailWidth").ToObject<int>());
            }
            if (json.Property("thumbnailHeight") != null) {
                this.setThumbnailHeight(json.GetValue("thumbnailHeight").ToObject<int>());
            }

        } catch (Exception e) {
            Log.Error(typeof(PictureMessage), e);
            this.setParseError(true);
        }
    }


    public override String createContentJsonStr() {
        JObject json = new JObject();
        try {
            json.Add("imageStorageId", this.getImageStorageId());
            json.Add("thumbnail", this.getThumbnail());
            json.Add("thumbnailWidth", this.getThumbnailWidth());
            json.Add("thumbnailHeight", this.getThumbnailHeight());
        } catch (Exception e) {
            Log.Error(typeof(PictureMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }

    /**
     * 将Message表content字段解析成PictureMessage对象
     * 纯业务字段，不包含from、to等信息
     *
     * @param jsonStr 图片消息业务字段json字符串
     * @return 图片消息类，不包含共通字段
     */
    public PictureMessage toModel(String jsonStr) {

        try {
            JObject json =  JObject.Parse(jsonStr);
            this.imageStorageId = json.GetValue("imageStorageId").ToStr();
            this.thumbnail = json.GetValue("thumbnail").ToStr();
            if (json.Property("thumbnailWidth") != null) {
                this.thumbnailWidth = json.GetValue("thumbnailWidth").ToObject<int>();
            }
            if (json.Property("thumbnailHeight") != null) {
                this.thumbnailHeight = json.GetValue("thumbnailHeight").ToObject<int>();
            }

            return this;
        } catch(Exception e) {
            Log.Error(typeof(PictureMessage), e);
        }
        return null;
    }

    /**
     * 将图片消息转换成Message表中的content字段，只包含业务字段
     *
     * @return图片消息业务字段的JSON串
     */
    public String toContent() {
        try {
            JObject json = new JObject();
            json.Add("imageStorageId", imageStorageId);
            json.Add("thumbnail", thumbnail);
            json.Add("thumbnailWidth", thumbnailWidth);
            json.Add("thumbnailHeight", thumbnailHeight);
            return JsonConvert.SerializeObject(json);
        } catch(Exception e) {
            Log.Error(typeof(PictureMessage), e);
        }
        return "";
    }


}
}
