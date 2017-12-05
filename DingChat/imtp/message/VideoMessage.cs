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
class VideoMessage : Message {

    /** 文件下载路径 */
    public string videoStorageId {
        get;
        set;
    }

    /** 文件名称 */
    public String fileName {
        get;
        set;
    }

    /** 文件大小 */
    public long fileSize {
        get;
        set;
    }

    /** 视频时长 */
    public long duration {
        get;
        set;
    }
    /** 转换后的视频时长 */
    public string videoLength {
        get;
        set;
    }
    /** 视频文件第一帧base 64 */
    //Base64编码缩略图内容
    public String firstFrame {
        get;
        set;
    }

    public BitmapImage firstFrameImage {
        get;
        set;
    }

    public int firstFrameWidth;

    public int firstFrameHeight;

    public string getVideoStorageId() {
        return videoStorageId;
    }

    public void setVideoStorageId(string videoStorageId) {
        this.videoStorageId = videoStorageId;
    }

    public String getFileName() {
        return fileName;
    }

    public void setFileName(String fileName) {
        this.fileName = fileName;
    }

    public long getFileSize() {
        return fileSize;
    }

    public void setFileSize(long fileSize) {
        this.fileSize = fileSize;
    }

    public long getDuration() {
        return duration;
    }

    public void setDuration(long duration) {
        this.duration = duration;
    }

    public String getFirstFrame() {
        return firstFrame;
    }

    public void setFirstFrame(String firstFrame) {
        this.firstFrame = firstFrame;
    }

    public int getFirstFrameWidth() {
        return firstFrameWidth;
    }

    public void setFirstFrameWidth(int firstFrameWidth) {
        this.firstFrameWidth = firstFrameWidth;
    }

    public int getFirstFrameHeight() {
        return firstFrameHeight;
    }

    public void setFirstFrameHeight(int firstFrameHeight) {
        this.firstFrameHeight = firstFrameHeight;
    }


    public override MsgType getType() {
        return MsgType.Video;
    }


    public override void parse( MsgType type,  SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            if (json.Property("videoStorageId") != null) {
                this.setVideoStorageId(json.GetValue("videoStorageId").ToStr());
            }

            this.setFileName(json.GetValue("fileName").ToStr());
            if (json.Property("fileSize") != null) {
                this.setFileSize(json.GetValue("fileSize").ToObject<long>());
            }
            if (json.Property("duration") != null) {
                this.setDuration(json.GetValue("duration").ToObject<long>());
            }

            this.setFirstFrame(json.GetValue("firstFrame").ToStr());
            if (json.Property("firstFrameWidth") != null) {
                this.setFirstFrameWidth(json.GetValue("firstFrameWidth").ToObject<int>());
            }
            if (json.Property("firstFrameHeight") != null) {
                this.setFirstFrameHeight(json.GetValue("firstFrameHeight").ToObject<int>());
            }

        } catch (Exception e) {
            Log.Error(typeof(VideoMessage), e);
            this.setParseError(true);
        }
    }


    public override String createContentJsonStr() {
        JObject json = new JObject();
        try {
            json.Add("videoStorageId", this.getVideoStorageId());
            json.Add("fileName", this.getFileName());
            json.Add("fileSize", this.getFileSize());
            json.Add("duration", this.getDuration());
            json.Add("firstFrame", this.getFirstFrame());
            json.Add("firstFrameWidth", this.getFirstFrameWidth());
            json.Add("firstFrameHeight", this.getFirstFrameHeight());
        } catch (Exception e) {
            Log.Error(typeof(VideoMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }

    /**
     * 将Message表content字段解析成VideoMessage对象
     * 纯业务字段，不包含from、to等信息
     *
     * @param jsonStr 视频消息业务字段json字符串
     * @return 视频消息类，不包含共通字段
     */
    public VideoMessage toModel(String jsonStr) {
        try {
            JObject json = JObject.Parse(jsonStr);
            if (json.Property("videoStorageId") != null) {
                this.setVideoStorageId(json.GetValue("videoStorageId").ToStr());
            }

            this.setFileName(json.GetValue("fileName").ToStr());
            if (json.Property("fileSize") != null) {
                this.setFileSize(json.GetValue("fileSize").ToObject<long>());
            }
            if (json.Property("duration") != null) {
                this.setDuration(json.GetValue("duration").ToObject<long>());
            }

            this.setFirstFrame(json.GetValue("firstFrame").ToStr());
            if (json.Property("firstFrameWidth") != null) {
                this.setFirstFrameWidth(json.GetValue("firstFrameWidth").ToObject<int>());
            }
            if (json.Property("firstFrameHeight") != null) {
                this.setFirstFrameHeight(json.GetValue("firstFrameHeight").ToObject<int>());
            }

            return this;
        } catch(Exception e) {
            Log.Error(typeof(VideoMessage), e);
        }
        return null;
    }

    /**
     * 将视频消息转换成Message表中的content字段，只包含业务字段
     *
     * @return 视频消息业务字段的JSON串
     */
    public String toContent() {
        try {
            JObject json = new JObject();
            json.Add("videoStorageId", this.getVideoStorageId());
            json.Add("fileName", this.getFileName());
            json.Add("fileSize", this.getFileSize());
            json.Add("duration", this.getDuration());
            json.Add("firstFrame", this.getFirstFrame());
            json.Add("firstFrameWidth", this.getFirstFrameWidth());
            json.Add("firstFrameHeight", this.getFirstFrameHeight());
            return JsonConvert.SerializeObject(json);
        } catch(Exception e) {
            Log.Error(typeof(VideoMessage), e);
        }
        return "";
    }
}
}
