using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Business.Cache;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using Newtonsoft.Json;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Services.core;

namespace cn.lds.chatcore.pcw.Services {
class OrganizationMemberService : BaseService {
    private static OrganizationMemberService instance = null;

    public static OrganizationMemberService getInstance() {
        if (instance == null) {
            instance = new OrganizationMemberService();
        }
        return instance;
    }

    /// <summary>
    ///  请求API拉取组织成员
    /// </summary>
    public void RequestOrganizationMember(string tenantNo) {
        try {
            // 同步组织成员数据
            long orgMeTime = TimeServices.getInstance().GetTime(TimestampType.OrganizationMember, tenantNo);
            ContactsApi.getMOrganizationUsers(orgMeTime, tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
        }
    }
    /// <summary>
    /// 组织成员变更消息
    /// 好友头像变更通知消息\
    /// 好友昵称变更通知消息
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {
        try {
            // 获取消息类型
            Message message = messageArrivedEvent.message;
            MsgType msgType = message.getType();
            switch (msgType) {
            //组织成员变更消息
            case MsgType.OrganizationMemberChanged:
                processOrganizationMemberChangedMessage(message);
                break;
            //好友头像变更通知消息
            case MsgType.UserAvatarChanged:
                processUserAvatarChangedMessage(message);
                break;
            //好友昵称变更通知消息
            case MsgType.UserNicknameChanged:
                processUserNicknameChangedMessage(message);
                break;
            default:
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
        }
    }

    /// <summary>
    /// P002: getMOrganizationUsers 获取所有组织用户列表
    /// E002: 修改我的头像 changeAvatar
    /// E004: 设置昵称 changeNickname
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        try {
            switch (eventData.eventDataType) {
            //  P002: getMOrganizationUsers 获取所有组织用户列表
            case EventDataType.getMOrganizationUsers:

                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    P002(eventData);
                }
                // API请求失败
                else {
                    // 如果是初次登录，则继续请求API
                    if (App.IsFirstLogin) {
                        String tenantNo = eventData.extras["tenantNo"].ToStr();
                        this.RequestOrganizationMember(tenantNo);
                    } else {
                        // 否则标识加载完成，交由DataPullService来处理关键数据同步问题
                        this.FireOrganizationsLoadOk();
                    }

                }
                this.MarkDataLoadComplete(eventData);
                break;
            //  E002: 修改我的头像 changeAvatar
            case EventDataType.changeAvatar:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E002(eventData);
                }
                // API请求失败
                else {

                }
                break;
            //  E004: 设置昵称 changeNickname
            case EventDataType.changeNickname:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E004(eventData);
                }
                // API请求失败
                else {

                }
                break;

            }
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
        }
    }

    /// <summary>
    /// P002: getMOrganizationUsers 获取所有组织用户列表
    /// </summary>
    /// <param Name="data"></param>
    private void P002(EventData<Object> data) {
        try {
            Object contacts = data.data;
            List<OrganizationMemberBean> list = JsonConvert.DeserializeObject<List<OrganizationMemberBean>>(contacts.ToStr(), new convertor<OrganizationMemberBean>());
            //AccountsBean accountsBean = JsonConvert.DeserializeObject<AccountsBean>(contacts.ToStr());
            String tenantNo = data.extras["tenantNo"].ToStr();


            //　有拉取回来数据，则更新表
            if (list.Count > 0) {
                List<OrganizationMemberTable> modelList = new List<OrganizationMemberTable>();
                modelList = Convertors(modelList, list, tenantNo);
                OrganizationMemberDao.getInstance().InsertOrganizationMember(modelList);
                TimeServices.getInstance().SaveTime(TimestampType.OrganizationMember, data.timestamp, tenantNo);
            }

            this.FireOrganizationsLoadOk();
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
        }
    }

    /// <summary>
    /// 触发数据拉取完成事件
    /// </summary>
    public void FireOrganizationsLoadOk() {
        try {
            App.OrganizationsLoadOk = true;
            if (App.OrganizationsLoadCount < App.TenantNoList.Count) {
                App.OrganizationsLoadCount++;
            }

            BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
            Businessdata.eventDataType = BusinessEventDataType.LoadingOk;
            EventBusHelper.getInstance().fireEvent(Businessdata);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
        }
    }

    /// <summary>
    /// E002: 修改我的头像 changeAvatar
    /// </summary>
    /// <param Name="data"></param>
    private void E002(EventData<Object> data) {
        try {
            // 获取更新的组织中的成员
            List<OrganizationMemberTable> list = OrganizationMemberDao.getInstance().FindOrganizationMemberByOrgId(int.Parse(App.AccountsModel.clientuserId));

            if (null == list) {
                return;
            }
            String avatarStorageId = data.extras["avatarStorageId"].ToStr();
            for (int i = 0; i < list.Count; i++ ) {

                OrganizationMemberTable organizationMemberTable = list[i];
                organizationMemberTable.avatarId = avatarStorageId;
                OrganizationMemberDao.getInstance().save(organizationMemberTable);
            }
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
        }
    }

    /// <summary>
    /// E004: 设置昵称 changeNickname
    /// </summary>
    /// <param Name="data"></param>
    private void E004(EventData<Object> data) {
        try {
            // 获取更新的组织中的成员
            List<OrganizationMemberTable> list = OrganizationMemberDao.getInstance().FindOrganizationMemberByOrgId(int.Parse(App.AccountsModel.clientuserId));

            if (null == list) {
                return;
            }
            String nickname = data.extras["nickname"].ToStr();
            for (int i = 0; i < list.Count; i++) {

                OrganizationMemberTable organizationMemberTable = list[i];
                organizationMemberTable.nickname = nickname;
                OrganizationMemberDao.getInstance().save(organizationMemberTable);
            }
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
        }
    }


    /// <summary>
    /// 组织成员变更消息
    /// </summary>
    /// <param Name="message"></param>
    private void processOrganizationMemberChangedMessage(Message message) {
        try {
            OrganizationMemberChangedMessage organizationMemberChangedMessage = (OrganizationMemberChangedMessage)message;

            // 构建表对象
            OrganizationMemberTable organizationMemberTable = new OrganizationMemberTable();
            // 组织用户Id
            organizationMemberTable.memberId = organizationMemberChangedMessage.getId().ToStr();
            // 社群用户Id
            organizationMemberTable.userId = organizationMemberChangedMessage.getUserId().ToStr();
            // 社群用户编号
            organizationMemberTable.no = organizationMemberChangedMessage.getNo();
            // 组织用户名称
            organizationMemberTable.nickname = organizationMemberChangedMessage.getNickname();
            // 组织用户头像Id
            organizationMemberTable.avatarId = organizationMemberChangedMessage.getAvatarId().ToStr();
            // 职位描述（枚举：mainJob/主职，auxiliaryJob/副职）
            organizationMemberTable.jobDescription = organizationMemberChangedMessage.getJobDescription();
            // 备注
            organizationMemberTable.remark = organizationMemberChangedMessage.getRemark();
            // 所属组织id
            organizationMemberTable.organizationId = organizationMemberChangedMessage.getOrganizationId();
            // 职称
            organizationMemberTable.post = organizationMemberChangedMessage.getPost();
            // 所属部门
            organizationMemberTable.office = organizationMemberChangedMessage.getOffice();
            // 办公电话
            organizationMemberTable.officeTel = organizationMemberChangedMessage.getOfficeTel();
            // 排序字段
            organizationMemberTable.sortNum = organizationMemberChangedMessage.getSortNum().ToStr();

            //查询tenantNo
            OrganizationTable orgOld = OrganizationServices.getInstance(). FindOrganizationByOrgId(organizationMemberTable.organizationId.ToInt());
            organizationMemberTable.tenantNo = orgOld.tenantNo;

            // 保存表
            this.save(organizationMemberTable, organizationMemberChangedMessage.isDeleted());
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
        }
    }

    /// <summary>
    /// 好友头像变更通知消息
    /// </summary>
    /// <param Name="message"></param>
    private void processUserAvatarChangedMessage(Message message) {
        try {
            UserAvatarChangedMessage userAvatarChangedMessage = (UserAvatarChangedMessage)message;
            // 获取更新的组织中的成员
            List<OrganizationMemberTable> list = OrganizationMemberDao.getInstance().FindOrganizationMemberByUserId(int.Parse(userAvatarChangedMessage.getUserId().ToStr()));
            if (null == list) {
                return;
            }
            // 获取变更的头像
            String avatar = userAvatarChangedMessage.getAvatarStorageId().ToStr();
            for (int i = 0; i < list.Count; i++ ) {
                OrganizationMemberTable organizationMemberTable = list[i];
                organizationMemberTable.avatarId = avatar;
                this.save(organizationMemberTable, false);
            }
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
        }
    }

    /// <summary>
    /// 好友昵称变更通知消息
    /// </summary>
    /// <param Name="message"></param>
    private void processUserNicknameChangedMessage(Message message) {
        try {
            UserNicknameChangedMessage userNicknameChangedMessage = (UserNicknameChangedMessage)message;
            // 获取更新的组织中的成员
            List<OrganizationMemberTable> list = OrganizationMemberDao.getInstance().FindOrganizationMemberByUserId(int.Parse(userNicknameChangedMessage.getUserId().ToStr()));
            if (null == list) {
                return;
            }
            // 获取变更的头像
            String strNewNickName = userNicknameChangedMessage.getNickname();
            for (int i = 0; i < list.Count; i++) {
                OrganizationMemberTable organizationMemberTable = list[i];
                organizationMemberTable.nickname = strNewNickName;
                this.save(organizationMemberTable, false);
            }
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
        }
    }

    /// <summary>
    /// 保存组织
    /// </summary>
    /// <param Name="organizationTable"></param>
    /// <param Name="deleted"></param>
    public void save(OrganizationMemberTable organizationMemberTable, Boolean deleted) {
        try {
            // 删除数据
            OrganizationMemberDao.getInstance().deleteByOrgIdAndUserNo(organizationMemberTable.organizationId,organizationMemberTable.no);
            // 如果不是删除、则保存数据
            if (!deleted) {
                OrganizationMemberDao.getInstance().save(organizationMemberTable);
            }
            // 通知通讯录画面更新
            BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
            Businessdata.eventDataType = BusinessEventDataType.OrgChangedEvent;
            EventBusHelper.getInstance().fireEvent(Businessdata);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
        }

    }
    private List<OrganizationMemberTable> Convertors(List<OrganizationMemberTable> modelList, List<OrganizationMemberBean> beanList,string tenantNo) {
        //BeanUtils.copyProperties(modelList, beanList);
        for (int i = 0; i < beanList.Count; i++) {
            OrganizationMemberTable model = new OrganizationMemberTable();
            //BeanUtils.copyProperties(model, beanList[i]);
            //model.id = beanList[i].id;
            model.memberId = beanList[i].id.ToStr();
            model.email = beanList[i].email.ToStr();
            model.userId = beanList[i].userId;
            model.deleted = beanList[i].deleted;
            model.no = beanList[i].no;
            model.nickname = beanList[i].nickname;
            model.avatarId = beanList[i].avatarId;
            model.jobDescription = beanList[i].jobDescription;
            model.organizationId = beanList[i].organizationId;
            model.post = beanList[i].post;
            model.office = beanList[i].office;
            model.officeTel = beanList[i].officeTel;
            model.sortNum = beanList[i].sortNum;
            model.tenantNo = tenantNo;
            model.location = beanList[i].location;
            model.empno = beanList[i].empno;
            modelList.Add(model);
        }
        return modelList;
    }

    /// <summary>
    /// 通过ID查找
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public List<OrganizationMemberTable> FindOrganizationMemberByUserId(int id) {
        try {
            return OrganizationMemberDao.getInstance().FindOrganizationMemberByUserId(id);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
            return null;
        }
    }

    public List<OrganizationMemberTable> FindOrganizationMemberByUserIdAndTent(int id,string tent) {
        try {
            return OrganizationMemberDao.getInstance().FindOrganizationMemberByUserIdAndTent(id, tent);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
            return null;
        }
    }



    /// <summary>
    /// 根据memberId查找组织成员信息
    /// </summary>
    /// <param Name="memberId"></param>
    /// <returns></returns>
    public OrganizationMemberTable FindOrganizationMemberByMemberId(String memberId) {
        try {
            return OrganizationMemberDao.getInstance().FindOrganizationMemberByMemberId(memberId);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
            return null;
        }
    }

    /// <summary>
    /// 通过no查找
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public List<OrganizationMemberTable> FindOrganizationMemberByUserNo(String no) {
        try {
            return OrganizationMemberDao.getInstance().FindOrganizationMemberByUserNo(no);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
            return null;
        }
    }

    /// <summary>
    /// 通过orgId查找
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public List<OrganizationMemberTable> FindOrganizationMemberByOrgId(int orgId) {
        try {
            return OrganizationMemberDao.getInstance().FindOrganizationMemberByOrgId(orgId);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberService), e);
            return null;
        }
    }


}
}
