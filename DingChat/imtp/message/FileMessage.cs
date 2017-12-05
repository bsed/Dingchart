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
class FileMessage : Message {
    public string fileStorageId {
        get;
        set;
    }
    //自己发送文件的时候记录文件路径
    public string filePath;
    public String fileName {
        get;
        set;
    }

    public long fileSize {
        get;
        set;
    }

    public string getFileStorageId() {
        return fileStorageId;
    }

    public void setFileStorageId(string fileStorageId) {
        this.fileStorageId = fileStorageId;
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

    public MsgType getType() {
        return MsgType.File;
    }


    public override void parse(MsgType type, SendMessage sendMessage) {
        //		if (!(jsonStr instanceof String))
        //			return;
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);

            this.setFileName(json.GetValue( "fileName").ToStr());
            if (json.Property("fileSize") != null) {
                this.setFileSize(json.GetValue("fileSize").ToObject<long>());
            }
            if (json.Property("fileStorageId") != null) {
                this.setFileStorageId(json.GetValue("fileStorageId").ToStr());
            }



        } catch (Exception e) {
            Log.Error(typeof(FileMessage), e);
            this.setParseError(true);
        }
    }

    public override String createContentJsonStr() {

        JObject json = new JObject();
        try {
            json.Add("fileName", this.getFileName());
            json.Add("fileSize", this.getFileSize());
            json.Add("fileStorageId", this.getFileStorageId());
        } catch (Exception e) {
            Log.Error(typeof(VoiceMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }

    /**
     * 将消息转换成Message表中的content字段，只包含业务字段
     *
     * @return 业务字段的JSON串
     */
    public String toContent() {
        try {
            JObject json = new JObject();
            json.Add("fileName", this.getFileName());
            json.Add("fileSize", this.getFileSize());
            json.Add("fileStorageId", this.getFileStorageId());
            return JsonConvert.SerializeObject(json);
        } catch (Exception e) {
            Log.Error(typeof(VoiceMessage), e);
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
    public FileMessage toModel(String jsonStr) {
        try {
            JObject json = JObject.Parse(jsonStr);
            this.setFileName(json.GetValue("fileName").ToStr());
            if (json.Property("fileSize") != null) {
                this.setFileSize(json.GetValue("fileSize").ToObject<long>());
            }
            if (json.Property("fileStorageId") != null) {
                this.setFileStorageId(json.GetValue("fileStorageId").ToStr());
            }


            return this;
        } catch (Exception e) {
            Log.Error(typeof(VoiceMessage), e);
        }
        return null;
    }
}
}
