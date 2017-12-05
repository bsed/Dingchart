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
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.imtp.message {
public class OrganizationMemberChangedMessage : Message {
    /** 组织用户Id */
    public int id;
    /** 社群用户Id */
    public int userId;
    /** 社群用户编号 */
    public String no;
    /** 组织用户名称 */
    public String nickname;
    /** 组织用户头像Id */
    public string avatarId;
    /** 职位描述（枚举：mainJob/主职，auxiliaryJob/副职） */
    public String jobDescription;
    /** 备注 */
    public String remark;
    /** 所属组织id */
    public String organizationId;
    /** 逻辑删除标记 */
    public Boolean deleted;
    /** 职称 */
    public String post;
    /** 所属部门id */
    public String office;
    /** 办公电话 */
    public String officeTel;
    /** 排序字段 */
    public int sortNum;

    /** email */
    public String email;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getUserId() {
        return userId;
    }

    public void setUserId(int userId) {
        this.userId = userId;
    }

    public String getNo() {
        return no;
    }

    public void setNo(String no) {
        this.no = no;
    }

    public String getNickname() {
        return nickname;
    }

    public void setNickname(String nickname) {
        this.nickname = nickname;
    }

    public string getAvatarId() {
        return avatarId;
    }

    public void setAvatarId(string avatarId) {
        this.avatarId = avatarId;
    }

    public String getJobDescription() {
        return jobDescription;
    }

    public void setJobDescription(String jobDescription) {
        this.jobDescription = jobDescription;
    }

    public String getRemark() {
        return remark;
    }

    public void setRemark(String remark) {
        this.remark = remark;
    }

    public String getOrganizationId() {
        return organizationId;
    }

    public void setOrganizationId(String organizationId) {
        this.organizationId = organizationId;
    }

    public Boolean isDeleted() {
        return deleted;
    }

    public void setDeleted(Boolean deleted) {
        this.deleted = deleted;
    }

    public String getPost() {
        return post;
    }

    public void setPost(String post) {
        this.post = post;
    }

    public String getOffice() {
        return office;
    }

    public void setOffice(String office) {
        this.office = office;
    }

    public String getOfficeTel() {
        return officeTel;
    }

    public void setOfficeTel(String officeTel) {
        this.officeTel = officeTel;
    }

    public int getSortNum() {
        return sortNum;
    }

    public void setSortNum(int sortNum) {
        this.sortNum = sortNum;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    /**
     * 返回消息类型
     * @return
     */
    public override MsgType getType() {
        return MsgType.OrganizationMemberChanged;
    }

    /**
     * 填充消息属性
     * @param type
     * @param sendMessage
     */

    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String) sendMessage.getMessage());
            base.parse(type, sendMessage);
            /** 组织用户Id */
            if (json.Property("id") != null) {
                this.setId(json.GetValue("id").ToObject<int>());
            }
            /** 社群用户Id */
            if (json.Property("userId") != null) {
                this.setUserId(json.GetValue("userId").ToObject<int>());
            }
            /** 社群用户编号 */
            this.setNo(json.GetValue("no").ToStr());
            /** 组织用户名称 */
            this.setNickname(json.GetValue("nickname").ToStr());
            /** 组织用户头像Id */
            if (json.Property("avatarId") != null) {
                this.setAvatarId(json.GetValue("avatarId").ToStr());
            }

            /** 职位描述（枚举：mainJob/主职，auxiliaryJob/副职） */
            this.setJobDescription(json.GetValue("jobDescription").ToStr());
            /** 备注 */
            this.setRemark(json.GetValue("remark").ToStr());
            /** 所属组织id */
            this.setOrganizationId(json.GetValue("organizationId").ToStr());
            /** 逻辑删除标记 */
            if (json.Property("deleted") != null) {
                this.setDeleted(json.GetValue("deleted").ToObject<Boolean>());
            }
            this.setEmail(json.GetValue("email").ToStr());
            /** 职称 */
            this.setPost(json.GetValue("post").ToStr());
            /** 所属部门 */
            this.setOffice(json.GetValue("office").ToStr());
            /** 办公电话 */
            this.setOfficeTel(json.GetValue("officeTel").ToStr());
            /** 排序字段 */
            if (json.Property("sortNum") != null) {
                this.setSortNum(json.GetValue("sortNum").ToObject<int>());
            }
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberChangedMessage), e);
            this.setParseError(true);
        }
    }
    /**
     * 构建JSON串（通知类消息、不用保存到数据库，所以未实现方法体）
     * @return
     */

    public override String createContentJsonStr() {
        return null;
    }
}
}
