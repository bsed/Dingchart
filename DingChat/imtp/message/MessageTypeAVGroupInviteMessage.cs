using cn.lds.im.sdk.bean;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace cn.lds.chatcore.pcw.imtp.message {
public class MessageTypeAVGroupInviteMessage : Message {
    private List<SelectPeopleSubViewModel> userCard;
    private String roomId;
    private String userName;//发起人姓名  为了消息列表里的通知提示
    private String userImId;//发起人IMID 同上
    private long timestamp;

    public List<SelectPeopleSubViewModel> getUserCard() {
        //List<SelectPeopleSubViewModel> list = new List<SelectPeopleSubViewModel>();
        //for(int i=0; i< userCard.Count; i++) {
        //    if (userCard[i].MemberImId != App.AccountsModel.no) {
        //        list.Add(list[i]);
        //    }

        //}
        return userCard;
    }

    public void setUserCard(List<SelectPeopleSubViewModel> userCard) {
        this.userCard = userCard;
    }

    public string getUserName() {
        return userName;
    }

    public void setUserName(string userName) {
        this.userName = userName;
    }

    public String getRoomId() {
        return roomId;
    }

    public void setRoomId(String roomId) {
        this.roomId = roomId;
    }
    public String getUserImId() {
        return userImId;
    }

    public void setUserImId(String userImId) {
        this.userImId = userImId;
    }
    public long getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(long timestamp) {
        this.timestamp = timestamp;
    }

    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);


            List<SelectPeopleSubViewModel> list = JsonConvert.DeserializeObject<List<SelectPeopleSubViewModel>>(
                    json.GetValue("userCard").ToStr(), new convertor<SelectPeopleSubViewModel>());
            this.setUserCard(list);
            this.setUserName(json.GetValue("userName").ToStr());
            this.setUserImId(json.GetValue("userImId").ToStr());
            this.setRoomId(json.GetValue("roomId").ToStr());
            this.setTimestamp(long.Parse(json.GetValue("timestamp").ToStr()));
        } catch (Exception e) {
            this.setParseError(true);
        }

    }

    public override String createContentJsonStr() {
        JObject json = new JObject();
        try {
            //json.Add("userCard", JsonConvert.SerializeObject(this.getUserCard()));
            //转换字符串 变成小写开头
            json.Add("userCard",JArray.FromObject( this.getUserCard(), new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            //json.Add("userCard", JsonConvert.SerializeObject(this.getUserCard(), Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            json.Add("userName", this.getUserName());
            json.Add("userImId", this.getUserImId());
            json.Add("roomId", this.getRoomId());
            json.Add("timestamp", this.getTimestamp());
        } catch (Exception ex) {
            Log.Error(typeof(MessageTypeAVGroupInviteMessage), ex);
        }
        return this.addContentHeader(JsonConvert.SerializeObject(json, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver()}));
    }


    /**
    * 将Message表content字段解析成PictureMessage对象
    * 纯业务字段，不包含from、to等信息
    *
    * @param jsonStr 图片消息业务字段json字符串
    * @return 图片消息类，不包含共通字段
    */
    public MessageTypeAVGroupInviteMessage toModel(String jsonStr) {

        try {
            JObject json = JObject.Parse(jsonStr);
            //转换字符串 变成小写开头
            List<SelectPeopleSubViewModel> list = JsonConvert.DeserializeObject<List<SelectPeopleSubViewModel>>(
                    json.GetValue("userCard").ToStr(), new convertor<SelectPeopleSubViewModel>());
            this.userCard = list;
            this.userName=json.GetValue("userName").ToStr();
            this.userImId=json.GetValue("userImId").ToStr();
            this.roomId=json.GetValue("roomId").ToStr();
            this.timestamp=long.Parse(json.GetValue("timestamp").ToStr());
            return this;
        } catch (Exception e) {
            Log.Error(typeof(MessageTypeAVGroupInviteMessage), e);
        }
        return null;
    }

    public String toContent() {
        JObject json = new JObject();
        try {
            //        options.SerializerSettings.ContractResolver =
            //new CamelCasePropertyNamesContractResolver();
            json.Add("userCard", JArray.FromObject(this.getUserCard(), new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            //json.Add("userCard", JsonConvert.SerializeObject(this.getUserCard(), Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            json.Add("userName", this.getUserName());
            json.Add("userImId", this.getUserImId());
            json.Add("roomId", this.getRoomId());
            json.Add("timestamp", this.getTimestamp());
        } catch (Exception ex) {
            Log.Error(typeof(MessageTypeAVGroupInviteMessage), ex);
        }
        //return JsonConvert.SerializeObject(json);
        return JsonConvert.SerializeObject(json, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}
}
