using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.Services {
class DataPullService {
    private static DataPullService dataPullService;
    private Timer stateTimer = null;
    private int interval = 5*60000;
    //private int dueTime = 30000;    // 30秒后进行数据检查
    private int dueTime = 2 * 60000;  // 2分钟后进行数据检查
    public static DataPullService getInstance() {
        if (dataPullService == null) {
            dataPullService = new DataPullService();
        }
        return dataPullService;
    }

    public void startDataPull() {

        if (stateTimer == null) {
            var autoEvent = new AutoResetEvent(false);
            DataPullService dataPull = getInstance();
            stateTimer = new Timer(dataPull.pullData,autoEvent, dueTime, interval);
        }
    }

    private void stopTimer() {
        if (this.stateTimer != null) {

            this.stateTimer.Dispose();
        }
        this.stateTimer = null;

    }

    /// <summary>
    /// 调试方法
    /// </summary>
    /// <param Name="BussinessName"></param>
    /// <param Name="dataFlag"></param>
    private void DebugLog(String BussinessName,Boolean dataFlag) {
        try {
            Console.WriteLine("关键数据拉取检查："+ BussinessName+ "="+ dataFlag);
        } catch (Exception ex) {
            Log.Error(typeof (DataPullService), ex);
        }
    }

    private void pullData(Object stateInfo) {
        try {
            //TODO 需要判断，如果全部完成了，应该停止timer
            Boolean CanStopTimer = true;

            // 个人详情
            this.DebugLog("个人详情", AccountsServices.getInstance().DataLoadComplete);
            if (AccountsServices.getInstance().DataLoadComplete == false) {
                CanStopTimer = false;
                //拉取个人详情
                AccountsServices.getInstance().GetMyDetail();
            }

            // 联系人
            this.DebugLog("联系人", ContactsServices.getInstance().DataLoadComplete);
            if (ContactsServices.getInstance().DataLoadComplete == false) {
                CanStopTimer = false;
                //下载好友列表
                ContactsServices.getInstance().Contacts();
            }
            // 群
            this.DebugLog("群", MucServices.getInstance().DataLoadComplete);
            if (MucServices.getInstance().DataLoadComplete == false) {
                CanStopTimer = false;
                //下载群列表
                MucServices.getInstance().RequestGroups();
            }
            // 组织
            this.DebugLog("组织", OrganizationServices.getInstance().DataLoadComplete);
            if (OrganizationServices.getInstance().DataLoadComplete == false) {
                CanStopTimer = false;
                // 同步组织数据

                if (App.TenantNoList.Count > 0) {
                    foreach (string tenantNo in App.TenantNoList) {
                        // 同步组织数据
                        OrganizationServices.getInstance().GetOrganization(tenantNo);
                    }
                } else {
                    // 同步组织数据
                    OrganizationServices.getInstance().GetOrganization(string.Empty);
                }

            }
            // 组织成员
            this.DebugLog("组织成员", OrganizationMemberService.getInstance().DataLoadComplete);
            if (OrganizationMemberService.getInstance().DataLoadComplete == false) {
                CanStopTimer = false;
                // 同步组织数据
                if (App.TenantNoList.Count > 0) {
                    foreach (string tenantNo in App.TenantNoList) {
                        // 同步组织数据
                        OrganizationMemberService.getInstance().RequestOrganizationMember(tenantNo);
                    }
                } else {
                    // 同步组织数据
                    OrganizationMemberService.getInstance().RequestOrganizationMember(string.Empty);
                }

            }
            // 公众号
            this.DebugLog("公众号", PublicAccountsService.getInstance().DataLoadComplete);
            if (PublicAccountsService.getInstance().DataLoadComplete == false) {
                CanStopTimer = false;
                PublicAccountsService.getInstance().request(null);
            }
            // 应用分类和分组
            this.DebugLog("应用分类和分组", ThirdAppGroupAndClassService.getInstance().DataLoadComplete);
            if (ThirdAppGroupAndClassService.getInstance().DataLoadComplete == false) {
                CanStopTimer = false;
                // 拉取应用类别的分组
                if (App.TenantNoList.Count > 0) {
                    foreach (string tenantNo in App.TenantNoList) {
                        ThirdAppGroupAndClassService.getInstance().RequestForThirdAppGroups(tenantNo);
                    }
                } else {
                    ThirdAppGroupAndClassService.getInstance().RequestForThirdAppGroups(string.Empty);
                }
            }
            // 应用
            this.DebugLog("应用", PublicWebService.getInstance().DataLoadComplete);
            if (PublicWebService.getInstance().DataLoadComplete == false) {
                CanStopTimer = false;
                // 拉取应用列表
                if (App.TenantNoList.Count > 0) {
                    foreach (string tenantNo in App.TenantNoList) {
                        PublicWebService.getInstance().RequestForApps(tenantNo);
                    }
                } else {
                    PublicWebService.getInstance().RequestForApps(string.Empty);
                }
            }
            // 码表
            this.DebugLog("码表", MasterServices.getInstance().DataLoadComplete);
            if (MasterServices.getInstance().DataLoadComplete == false) {
                CanStopTimer = false;

                if (App.TenantNoList.Count > 0) {
                    foreach (string tenantNo in App.TenantNoList) {
                        MasterServices.getInstance().RequestMaster(tenantNo);
                    }
                } else {
                    MasterServices.getInstance().RequestMaster(string.Empty);
                }
            }
            // 判断是否可以停止timer
            if (CanStopTimer) {
                this.DebugLog("关键数据拉取完成，停止timer检查：", CanStopTimer);
                this.stopTimer();
            }
        } catch (Exception ex) {
            Log.Error(typeof(DataPullService), ex);
        }
    }

}
}
