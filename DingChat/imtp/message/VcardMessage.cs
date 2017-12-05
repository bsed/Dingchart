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
public class VcardMessage : Message {

    public long userId {
        get;
        set;
    }
    public String userNo {
        get;
        set;
    }
    public String nickname {
        get;
        set;
    }
    public String gender {
        get;
        set;
    }
    public string avatarStorageId {
        get;
        set;
    }

    public BitmapImage AvatarPath {
        get;
        set;
    }

    public String country {
        get;
        set;
    }
    public String province {
        get;
        set;
    }
    public String city {
        get;
        set;
    }
    public String moodMessage {
        get;
        set;
    }

    public long getUserId() {
        return userId;
    }

    public void setUserId(long userId) {
        this.userId = userId;
    }

    public String getUserNo() {
        return userNo;
    }

    public void setUserNo(String userNo) {
        this.userNo = userNo;
    }

    public String getNickname() {
        return nickname;
    }

    public void setNickname(String nickname) {
        this.nickname = nickname;
    }

    public String getGender() {
        return gender;
    }

    public void setGender(String gender) {
        this.gender = gender;
    }

    public string getAvatarStorageId() {
        return avatarStorageId;
    }

    public void setAvatarStorageId(string avatarStorageId) {
        this.avatarStorageId = avatarStorageId;
    }

    public String getCountry() {
        return country;
    }

    public void setCountry(String country) {
        this.country = country;
    }

    public String getProvince() {
        return province;
    }

    public void setProvince(String province) {
        this.province = province;
    }

    public String getCity() {
        return city;
    }

    public void setCity(String city) {
        this.city = city;
    }

    public String getMoodMessage() {
        return moodMessage;
    }

    public void setMoodMessage(String moodMessage) {
        this.moodMessage = moodMessage;
    }


    public override void parse( MsgType type,  SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);

            this.setType(type);
            if (json.Property("userId") != null) {
                this.setUserId(json.GetValue("userId").ToObject<long>());
            }

            this.setUserNo(json.GetValue("userNo").ToStr());
            this.setNickname(json.GetValue("nickname").ToStr());
            this.setGender(json.GetValue("gender").ToStr());
            if (json.Property("avatarStorageId") != null) {
                this.setAvatarStorageId(json.GetValue("avatarStorageId").ToStr());
            }

            this.setCountry(json.GetValue("country").ToStr());
            this.setProvince(json.GetValue("province").ToStr());
            this.setCity(json.GetValue("city").ToStr());
            this.setMoodMessage(json.GetValue("moodMessage").ToStr());

        } catch ( Exception e) {
            Log.Error(typeof(VcardMessage), e);
            this.setParseError(true);
        }
    }


    public override String createContentJsonStr() {
        JObject json = new JObject();
        try {

            json.Add("userId", this.getUserId());
            json.Add("userNo", this.getUserNo());
            json.Add("nickname", this.getNickname());
            json.Add("gender", this.getGender());
            json.Add("avatarStorageId", this.getAvatarStorageId());
            json.Add("country", this.getCountry());
            json.Add("province", this.getProvince());
            json.Add("city", this.getCity());
            json.Add("moodMessage", this.getMoodMessage());

        } catch (Exception e) {
            Log.Error(typeof(VcardMessage), e);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json));
    }

    /**
     * 将Message表content字段解析成VcardMessage对象
     * 纯业务字段，不包含from、to等信息
     *
     * @param jsonStr 名片消息业务字段json字符串
     * @return 名片消息类，不包含共通字段
     */
    public VcardMessage toModel(String jsonStr) {

        try {
            JObject json = JObject.Parse(jsonStr);

            if (json.Property("userId") != null) {
                this.setUserId(json.GetValue("userId").ToObject<long>());
            }

            this.setUserNo(json.GetValue("userNo").ToStr());
            this.setNickname(json.GetValue("nickname").ToStr());
            this.setGender(json.GetValue("gender").ToStr());
            if (json.Property("avatarStorageId") != null) {
                this.setAvatarStorageId(json.GetValue("avatarStorageId").ToStr());
            }

            this.setCountry(json.GetValue("country").ToStr());
            this.setProvince(json.GetValue("province").ToStr());
            this.setCity(json.GetValue("city").ToStr());
            this.setMoodMessage(json.GetValue("moodMessage").ToStr());
            return this;
        } catch(Exception e) {
            Log.Error(typeof(VcardMessage), e);
        }
        return null;
    }

    /**
     * 将名片消息转换成Message表中的content字段，只包含业务字段
     *
     * @return 名片消息业务字段的JSON串
     */
    public String toContent() {
        try {
            JObject json = new JObject();
            json.Add("userId", userId);
            json.Add("userNo", userNo);
            json.Add("nickname", nickname);
            json.Add("gender", gender);
            json.Add("avatarStorageId", avatarStorageId);
            json.Add("country", country);
            json.Add("province", province);
            json.Add("city", city);
            json.Add("moodMessage", moodMessage);
            return JsonConvert.SerializeObject(json);
        } catch(Exception e) {
            Log.Error(typeof(VcardMessage), e);
        }
        return "";
    }
}
}
