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
public class MessageTypeAVGroupCancelMessage : Message {
    private List<SelectPeopleSubViewModel> userCard;
    private String roomId;
    private long timestamp;

    public List<SelectPeopleSubViewModel> getUserCard() {

        return userCard;
    }

    public void setUserCard(List<SelectPeopleSubViewModel> userCard) {
        this.userCard = userCard;
    }



    public String getRoomId() {
        return roomId;
    }

    public void setRoomId(String roomId) {
        this.roomId = roomId;
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
            json.Add("userCard",JArray.FromObject( this.getUserCard(), new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            //json.Add("userCard", JsonConvert.SerializeObject(this.getUserCard(), Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));

            json.Add("roomId", this.getRoomId());
            json.Add("timestamp", this.getTimestamp());
        } catch (Exception ex) {
            Log.Error(typeof(MessageTypeAVGroupCancelMessage), ex);
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
    public MessageTypeAVGroupCancelMessage toModel(String jsonStr) {

        try {
            JObject json = JObject.Parse(jsonStr);
            List<SelectPeopleSubViewModel> list = JsonConvert.DeserializeObject<List<SelectPeopleSubViewModel>>(
                    json.GetValue("userCard").ToStr(), new convertor<SelectPeopleSubViewModel>());
            this.userCard = list;

            this.roomId=json.GetValue("roomId").ToStr();
            this.timestamp=long.Parse(json.GetValue("timestamp").ToStr());
            return this;
        } catch (Exception e) {
            Log.Error(typeof(MessageTypeAVGroupCancelMessage), e);
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

            json.Add("roomId", this.getRoomId());
            json.Add("timestamp", this.getTimestamp());
        } catch (Exception ex) {
            Log.Error(typeof(MessageTypeAVGroupCancelMessage), ex);
        }
        //return JsonConvert.SerializeObject(json);
        return JsonConvert.SerializeObject(json, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}
}
