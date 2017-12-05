using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
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
class OrganizationServices : BaseService {
    private static OrganizationServices instance = null;

    public static OrganizationServices getInstance() {
        if (instance == null) {
            instance = new OrganizationServices();
        }
        return instance;
    }


    /// <summary>
    /// 好友相关通知消息
    /// 组织变更消息
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {
        try {
            // 获取消息类型
            Message message = messageArrivedEvent.message;
            MsgType msgType = message.getType();
            switch (msgType) {
            case MsgType.OrganizationChanged:
                processOrganizationChangedMessage(message);
                break;
            default:
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof (OrganizationServices), e);
        }
    }

    /// <summary>
    /// P001: getMOrganizations 获取所有组织机构（团队）列表(MOBILE)
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        try {
            switch (eventData.eventDataType) {
            //  P001: getMOrganizations 获取所有组织机构（团队）列表(MOBILE)
            case EventDataType.getMOrganizations:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    P001(eventData);
                }
                // API请求失败
                else {
                    // 如果是初次登录，则继续请求API
                    if (App.IsFirstLogin) {
                        String tenantNo = eventData.extras["tenantNo"].ToStr();
                        this.GetOrganization(tenantNo);
                    } else {
                        // 否则标识加载完成，交由DataPullService来处理关键数据同步问题
                        OrganizationMemberService.getInstance().FireOrganizationsLoadOk();
                    }
                }
                this.MarkDataLoadComplete(eventData);
                break;

            }
        } catch (Exception e) {
            Log.Error(typeof (OrganizationServices), e);
        }
    }

    /// <summary>
    /// P001: getMOrganizations 获取所有组织机构（团队）列表(MOBILE)
    /// </summary>
    /// <param Name="data"></param>
    private void P001(EventData<Object> data) {
        try {
            Object contacts = data.data;
            List<OrganizationBean> list = JsonConvert.DeserializeObject<List<OrganizationBean>>(
                                              contacts.ToStr(), new convertor<OrganizationBean>());
            //AccountsBean accountsBean = JsonConvert.DeserializeObject<AccountsBean>(contacts.ToStr());
            String tenantNo = data.extras["tenantNo"].ToStr();
            // 有拉取回来数据，则更新表
            if (list.Count > 0) {
                List<OrganizationTable> modelList = new List<OrganizationTable>();
                modelList = Convertors(modelList, list, tenantNo);

                OrganizationDao.getInstance().InsertOrganization(modelList);

                TimeServices.getInstance().SaveTime(TimestampType.Organization, data.timestamp, tenantNo);

            }

            OrganizationMemberService.getInstance().RequestOrganizationMember(tenantNo);
        } catch (Exception e) {
            Log.Error(typeof (OrganizationServices), e);
        }

    }

    /// <summary>
    /// 组织变更消息
    /// </summary>
    /// <param Name="message"></param>
    private void processOrganizationChangedMessage(Message message) {
        try {
            OrganizationChangedMessage organizationChangedMessage = (OrganizationChangedMessage) message;


            // 构建表对象
            OrganizationTable organizationTable = new OrganizationTable();
            // 组织ID
            organizationTable.organizationId = organizationChangedMessage.getId().ToStr();
            // 业务编号
            organizationTable.no = organizationChangedMessage.getNo();
            // 组织名称
            organizationTable.name = organizationChangedMessage.getName();
            // 所属组织ID
            organizationTable.parentId = organizationChangedMessage.getParentId().ToStr();
            // 组织LogoId
            organizationTable.logoStorageRecordId = organizationChangedMessage.getLogoStorageRecordId().ToStr();
            // 简介
            organizationTable.introduction = organizationChangedMessage.getIntroduction();
            // 负责人
            organizationTable.leader = organizationChangedMessage.getLeader();
            // 电话
            organizationTable.telephone = organizationChangedMessage.getTelephone();
            // 传真
            organizationTable.fax = organizationChangedMessage.getFax();
            // 地址
            organizationTable.address = organizationChangedMessage.getAddress();
            // 邮编
            organizationTable.postcode = organizationChangedMessage.getPostcode();
            // 虚拟组织标记
            organizationTable.@virtual = organizationChangedMessage.isVirtual();
            // 排序字段
            organizationTable.sortNum = organizationChangedMessage.getSortNum().ToStr();

            //查询tenantNo
            OrganizationTable orgOld= FindOrganizationByOrgId(organizationTable.organizationId.ToInt());
            organizationTable.tenantNo = orgOld.tenantNo;

            // 保存表
            this.save(organizationTable, organizationChangedMessage.isDeleted());


        } catch (Exception e) {
            Log.Error(typeof (OrganizationServices), e);
        }
    }

    /// <summary>
    /// 保存组织
    /// </summary>
    /// <param Name="organizationTable"></param>
    /// <param Name="deleted"></param>
    public void save(OrganizationTable organizationTable, Boolean deleted) {
        try {
            // 删除数据
            OrganizationDao.getInstance().deleteById(organizationTable.organizationId);
            // 如果不是删除、则保存数据
            if (!deleted) {
                OrganizationDao.getInstance().save(organizationTable);
            }
            // 通知通讯录画面更新
            BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
            Businessdata.eventDataType = BusinessEventDataType.OrgChangedEvent;
            EventBusHelper.getInstance().fireEvent(Businessdata);
        } catch (Exception e) {
            Log.Error(typeof (OrganizationServices), e);
        }

    }

    /// <summary>
    /// 调用列表api
    /// </summary>
    public void GetOrganization(string tenantNo) {
        long orgTime = TimeServices.getInstance().GetTime(TimestampType.Organization, tenantNo);
        ContactsApi.getMOrganizations(orgTime, tenantNo);
        // TODO:同步组织成员数据的API在组织数据处理完成后进行。
        //long orgMeTime = TimeServices.getInstance().GetTime(TimestampType.OrganizationMember);
        //ContactsApi.getMOrganizationUsers(orgMeTime);
    }


    private List<OrganizationTable> Convertors(List<OrganizationTable> modelList, List<OrganizationBean> beanList, String tenantNo) {
        for (int i = 0; i < beanList.Count; i++) {
            OrganizationTable model = new OrganizationTable();
            //BeanUtils.copyProperties(model, beanList[i]);
            //model.id = beanList[i].id;
            model.no = beanList[i].no;
            model.name = beanList[i].name;
            model.parentId = beanList[i].parentId;
            model.logoStorageRecordId = beanList[i].logoStorageRecordId;
            model.introduction = beanList[i].introduction;
            model.leader = beanList[i].leader;
            model.telephone = beanList[i].telephone;
            model.fax = beanList[i].fax;
            model.address = beanList[i].address;
            model.postcode = beanList[i].postcode;
            model.deleted = beanList[i].deleted;
            model.@virtual = beanList[i].@virtual;
            model.sortNum = beanList[i].sortNum;

            model.organizationId = beanList[i].id.ToStr();
            model.tenantNo = tenantNo;
            modelList.Add(model);
        }
        return modelList;
    }

    /// <summary>
    /// 通过ID查找
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public OrganizationTable FindOrganizationByOrgId(int id) {
        try {
            return OrganizationDao.getInstance().FindOrganizationByOrgId(id);
        } catch (Exception e) {
            Log.Error(typeof (OrganizationServices), e);
            return null;
        }
    }

    /// <summary>
    /// 查找组织
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public List<OrganizationTable> FindAllOrganization(string conditionCol, object conditionVal) {
        try {
            return OrganizationDao.getInstance().FindAllOrganization(conditionCol, conditionVal);
        } catch (Exception e) {
            Log.Error(typeof (OrganizationServices), e);
            return null;
        }
    }
    public List<OrganizationTable> FindAllOrganizationFromDB(string conditionCol, object conditionVal) {
        try {
            return OrganizationDao.getInstance().FindAllOrganizationFromDB(conditionCol, conditionVal);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationServices), e);
            return null;
        }
    }

    public OrganizationTable FindAllOrganizationFromDBByTentant(String tenant, string parent) {
        try {
            return OrganizationDao.getInstance().FindAllOrganizationFromDBByTentant(tenant, parent);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationServices), e);
            return null;
        }
    }

    public List<OrganizationTable> FindChildOrganization(int organization_id) {
        List<OrganizationTable> list = null;
        try {
            list = OrganizationDao.getInstance().FindChildOrganization(organization_id);
        } catch (Exception e) {
            Log.Error(typeof (OrganizationServices), e);
        }
        return list;
    }
}
}
