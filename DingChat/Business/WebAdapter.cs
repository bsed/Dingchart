using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Business {
[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
[System.Runtime.InteropServices.ComVisibleAttribute(true)]
public class WebAdapter {

    // 登陆用户信息
    private AccountsTable accountsTable = null;
    private AccountsTable GetLoginUserInfo() {
        if (accountsTable==null) {
            accountsTable = AccountsServices.getInstance().findByNo(App.AccountsModel.no);
        }
        return accountsTable;
    }

    /// <summary>
    /// 获取登录用户No
    /// </summary>
    /// <returns></returns>
    public String getLoginUserNo() {
        return this.GetLoginUserInfo().no;
    }

    /// <summary>
    /// 获取登录用户Name
    /// </summary>
    /// <returns></returns>
    public String getLoginUserName() {
        return this.GetLoginUserInfo().name;
    }

    /// <summary>
    /// 关闭弹出的网页
    /// </summary>
    /// <returns></returns>
    public void closePopWebPageWindow() {
        //发送关闭通知
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.data = null;
        Businessdata.eventDataType = BusinessEventDataType.ClosePopWebPageWindow;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }


}
}
