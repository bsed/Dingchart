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
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.imtp.message {
public class GroupMemberDeletedMessage : Message {
    public String groupId;

    public String groupNo;

    public String operatorUserId;

    public String operatorUserNo;

    public List<GroupMemberDetails> members;

    public String getGroupId() {
        return groupId;
    }

    public void setGroupId(String groupId) {
        this.groupId = groupId;
    }

    public String getOperatorUserNo() {
        return operatorUserNo;
    }

    public void setOperatorUserNo(String operatorUserNo) {
        this.operatorUserNo = operatorUserNo;
    }

    public String getOperatorUserId() {
        return operatorUserId;
    }

    public void setOperatorUserId(String operatorUserId) {
        this.operatorUserId = operatorUserId;
    }

    public String getGroupNo() {
        return groupNo;
    }

    public void setGroupNo(String groupNo) {
        this.groupNo = groupNo;
    }

    public List<GroupMemberDetails> getMembers() {
        return members;
    }

    public void setMembers(List<GroupMemberDetails> members) {
        this.members = members;
    }

    public override MsgType getType() {
        return MsgType.GroupMemberDeleted;
    }

    public override String createContentJsonStr() {
        return null;
    }

    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            this.setGroupId(json.GetValue("groupId").ToStr());
            this.setGroupNo(json.GetValue("groupNo").ToStr());
            this.setOperatorUserId(json.GetValue("operatorUserId").ToStr());
            this.setOperatorUserNo(json.GetValue("operatorUserNo").ToStr());

            //final JSONArray jsonArray = json.getJSONArray("members");
            //List<GroupMemberDetails> lst = new ArrayList<>();

            //final int length = jsonArray.length();
            //for (int i = 0; i < length; i++) {
            //    final JSONObject o = jsonArray.getJSONObject(i);
            //    final GroupMemberDetails memberDetail = new GroupMemberDetails();
            //    memberDetail.setUserId(o.optString("userId"));
            //    memberDetail.setUserNo(o.optString("userNo"));
            //    memberDetail.setNickname(o.optString("nickname"));
            //    memberDetail.setAvatarStorageId(o.optString("avatarStorageId"));

            //    lst.add(memberDetail);
            //}
            //this.setMembers(lst);

            List<GroupMemberDetails> jsonArray = JsonConvert.DeserializeObject<List<GroupMemberDetails>>(json["members"].ToStr(), new convertor<GroupMemberDetails>());
            this.members = jsonArray;
        } catch (Exception e) {
            Log.Error(typeof(GroupMemberDeletedMessage), e);
            this.setParseError(true);
        }
    }


}

//public class GroupMemberDetails
//{
//    String userId;
//    String userNo;
//    String nickname;
//    String avatarStorageId;

//    public String getUserId()
//    {
//        return userId;
//    }

//    public void setUserId(String userId)
//    {
//        this.userId = userId;
//    }

//    public String getUserNo()
//    {
//        return userNo;
//    }

//    public void setUserNo(String userNo)
//    {
//        this.userNo = userNo;
//    }

//    public String getNickname()
//    {
//        return nickname;
//    }

//    public void setNickname(String nickname)
//    {
//        this.nickname = nickname;
//    }

//    public String getAvatarStorageId()
//    {
//        return avatarStorageId;
//    }

//    public void setAvatarStorageId(String avatarStorageId)
//    {
//        this.avatarStorageId = avatarStorageId;
//    }
//}
}
