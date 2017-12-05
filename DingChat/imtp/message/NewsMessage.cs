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
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.imtp.message {

public class MultimediaEntry {
    //编号
    public String no {
        set;
        get;
    }
    //标题
    public String title {
        set;
        get;
    }
    //概要
    public String summary {
        set;
        get;
    }
    //封面图片
    public String timestamp {
        set;
        get;
    }
    //缩略图
    public String thumbnail {
        set;
        get;
    }


    public String getTitle() {
        return this.title;
    }

    public void setTitle( String title) {
        this.title = title;
    }

    public String getSummary() {
        return this.summary;
    }

    public void setSummary( String summary) {
        this.summary = summary;
    }

    public String getTimestamp() {
        return this.timestamp;
    }

    public void setTimestamp( String timestamp) {
        this.timestamp = timestamp;
    }

    public String getThumbnail() {
        return this.thumbnail;
    }

    public void setThumbnail( String thumbnail) {
        this.thumbnail = thumbnail;
    }

    public String getNo() {
        return this.no;
    }

    public void setNo( String no) {
        this.no = no;
    }
}


class NewsMessage : Message {
    public int count;
    public List<MultimediaEntry> articles = new List<MultimediaEntry>();


    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);

            if (json.Property("count") != null) {

                this.setCount(json.GetValue("count").ToObject<int>());
            }


            String entriesString = json["articles"].ToStr();

            List<MultimediaEntry> jsonArray = JsonConvert.DeserializeObject<List<MultimediaEntry>>(entriesString, new convertor<MultimediaEntry>());

            this.articles.AddRange(jsonArray);

        } catch (Exception e) {
            Log.Error(typeof(NewsMessage), e);
            this.setParseError(true);
        }
    }


    public override String createContentJsonStr() {
        return null;
    }

    /**
     * 将Message表content字段解析成NewsMessage对象
     * 纯业务字段，不包含from、to等信息
     *
     * @param jsonStr 图文消息业务字段json字符串
     * @return 图文消息类，不包含共通字段
     */
    public NewsMessage toModel(String jsonStr) {
        try {
            JObject json = JObject.Parse(jsonStr);

            if (json.Property("count") != null) {

                this.count = json.GetValue("count").ToObject<int>();
            }


            String entriesString = json["articles"].ToStr();

            List<MultimediaEntry> jsonArray = JsonConvert.DeserializeObject<List<MultimediaEntry>>(entriesString, new convertor<MultimediaEntry>());

            this.articles.AddRange(jsonArray);
            return this;
        } catch (Exception e) {
            Log.Error(typeof(NewsMessage), e);
        }
        return null;
    }

    /**
     * 将图文消息转换成Message表中的content字段，只包含业务字段
     *
     * @return 图文消息业务字段的JSON串
     */
    public String toContent() {
        try {
            //JObject json = new JObject();
            //json.Add("count", count);
            //json.Add("articles", this.articles);

            IDictionary<String, Object> obj = new Dictionary<String, Object>();
            obj.Add("count", count);
            obj.Add("articles", this.articles);

            var json = JsonConvert.SerializeObject(obj);

            return json;
        } catch (Exception e) {
            Log.Error(typeof(NewsMessage), e);
        }
        return "";
    }

    public int getCount() {
        return this.count;
    }

    public void setCount( int count) {
        this.count = count;
    }

    public List<MultimediaEntry> getEntries() {
        return this.articles;
    }

    public MsgType getType() {
        return MsgType.News;
    }


}
}
