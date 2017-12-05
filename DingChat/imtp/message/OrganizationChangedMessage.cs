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
public class OrganizationChangedMessage : Message {
    /** 组织ID */
    public int id;
    /**  业务编号 */
    public String no;
    /** 组织名称 */
    public String name;
    /** 所属组织ID */
    public int parentId;
    /** 头像 */
    public string logoStorageRecordId;
    /** 简介 */
    public String introduction;
    /** 负责人 */
    public String leader;
    /** 电话 */
    public String telephone;
    /** 传真 */
    public String fax;
    /** 地址 */
    public String address;
    /** 邮编 */
    public String postcode;
    /** 虚拟组织标记 */
    public Boolean @virtual;
    /** 逻辑删除标记 */
    public Boolean deleted;
    /** 排序字段 */
    public int sortNum;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getNo() {
        return no;
    }

    public void setNo(String no) {
        this.no = no;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public int getParentId() {
        return parentId;
    }

    public void setParentId(int parentId) {
        this.parentId = parentId;
    }

    public string getLogoStorageRecordId() {
        return logoStorageRecordId;
    }

    public void setLogoStorageRecordId(string logoStorageRecordId) {
        this.logoStorageRecordId = logoStorageRecordId;
    }

    public String getIntroduction() {
        return introduction;
    }

    public void setIntroduction(String introduction) {
        this.introduction = introduction;
    }

    public String getLeader() {
        return leader;
    }

    public void setLeader(String leader) {
        this.leader = leader;
    }

    public String getTelephone() {
        return telephone;
    }

    public void setTelephone(String telephone) {
        this.telephone = telephone;
    }

    public String getFax() {
        return fax;
    }

    public void setFax(String fax) {
        this.fax = fax;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getPostcode() {
        return postcode;
    }

    public void setPostcode(String postcode) {
        this.postcode = postcode;
    }

    public Boolean isVirtual() {
        return @virtual;
    }

    public void setVirtual(Boolean @virtual) {
        this.@virtual = @virtual;
    }

    public Boolean isDeleted() {
        return deleted;
    }

    public void setDeleted(Boolean deleted) {
        this.deleted = deleted;
    }

    public int getSortNum() {
        return sortNum;
    }

    public void setSortNum(int sortNum) {
        this.sortNum = sortNum;
    }

    /**
     * 返回消息类型
     * @return
     */

    public override MsgType getType() {
        return MsgType.OrganizationChanged;
    }

    /**
     * 填充消息属性
     * @param type
     * @param sendMessage
     */
    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            /** 组织ID */
            if (json.Property("id") != null) {
                this.setId(json.GetValue("id").ToObject<int>());
            }

            /**  业务编号 */
            this.setNo(json.GetValue("no").ToStr());
            /** 组织名称 */
            this.setName(json.GetValue("Name").ToStr());
            /** 所属组织ID */
            if (json.Property("parentId") != null) {
                this.setParentId(json.GetValue("parentId").ToObject<int>());
            }
            /** 头像 */
            if (json.Property("logoStorageRecordId") != null) {
                this.setLogoStorageRecordId(json.GetValue("logoStorageRecordId").ToStr());
            }

            /** 简介 */
            this.setIntroduction(json.GetValue("introduction").ToStr());
            /** 负责人 */
            this.setLeader(json.GetValue("leader").ToStr());
            /** 电话 */
            this.setTelephone(json.GetValue("telephone").ToStr());
            /** 传真 */
            this.setFax(json.GetValue("fax").ToStr());
            /** 地址 */
            this.setAddress(json.GetValue("address").ToStr());
            /** 邮编 */
            this.setPostcode(json.GetValue("postcode").ToStr());
            /** 虚拟组织标记 */
            if (json.Property("virtual") != null) {
                this.setVirtual(json.GetValue("virtual").ToObject<Boolean>());
            }

            /** 逻辑删除标记 */
            if (json.Property("deleted") != null) {
                this.setDeleted(json.GetValue("deleted").ToObject<Boolean>());
            }

            /** 排序字段 */
            if (json.Property("sortNum") != null) {
                this.setSortNum(json.GetValue("sortNum").ToObject<int>());
            }

        } catch (Exception e) {
            Log.Error(typeof(OrganizationChangedMessage), e);
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
