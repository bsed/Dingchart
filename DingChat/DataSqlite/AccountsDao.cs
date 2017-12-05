using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;

namespace cn.lds.chatcore.pcw.DataSqlite {

class AccountsDao : BaseDao {

    private static AccountsDao instance = null;
    public static AccountsDao getInstance() {
        if (instance == null) {
            instance = new AccountsDao();
        }
        return instance;
    }

    public int save(AccountsTable table) {
        int count = -1;
        try {
            //this._mgr.Delete("accounts", null, null);
            VcardsTable vcardTable = VcardsDao.getInstance().findByNo(table.no);
            if (vcardTable != null) {

                vcardTable.email = table.email;
                vcardTable.gender = table.gender;
                vcardTable.desc = table.desc;
                vcardTable.avatarStorageRecordId = table.avatarStorageRecordId;
                VcardsDao.getInstance().save(vcardTable);
            }
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("no", table.no);
            entity.Add("Name", table.name);
            entity.Add("nickname", table.nickname);
            entity.Add("mobile", table.mobile);
            entity.Add("email", table.email);
            entity.Add("loginid", table.loginId);
            entity.Add("subscriptionOpenId", table.subscriptionOpenId);
            entity.Add("clientuserId", table.clientuserId);
            entity.Add("AvatarStorageRecordId", table.avatarStorageRecordId);
            entity.Add("birthday", table.birthday);
            entity.Add("gender", table.gender);
            entity.Add("country", table.country);
            entity.Add("province", table.province);
            entity.Add("city", table.city);
            entity.Add("desc", table.desc);
            entity.Add("qrcodeid", table.qrcodeId);
            entity.Add("enableNoDisturb", table.enableNoDisturb);
            entity.Add("startTimeOfNoDisturb", table.startTimeOfNoDisturb);
            entity.Add("endTimeOfNoDisturb", table.endTimeOfNoDisturb);
            entity.Add("needFriendConfirmation", table.needFriendConfirmation);
            entity.Add("allowFindMobileContacts", table.allowFindMobileContacts);
            entity.Add("allowFindMeByLoginId", table.allowFindMeByLoginId);

            if (this.isExist("accounts", "clientuserId", table.clientuserId)) {
                count = this._mgr.Update("accounts", entity, "clientuserId=@clientuserId", new System.Data.SQLite.SQLiteParameter[] {
                    new SQLiteParameter("clientuserId",table.clientuserId)
                });
            } else {
                count = this._mgr.Save("accounts", entity);
            }
            if(count>0) {
                App.AccountsModel = table;
                // 通知通讯录画面更新
                BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                businessEvent.eventDataType = BusinessEventDataType.MyDetailsChangeEvent;
                EventBusHelper.getInstance().fireEvent(businessEvent);
            }

        } catch (Exception e) {
            Log.Error(typeof(AccountsDao), e);
        }

        return count;

    }

    /// <summary>
    /// 通过NO查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public AccountsTable findByNo(String no) {
        AccountsTable table = null;
        try {
            table = new AccountsTable();
            DataRow entity = this._mgr.QueryOne("accounts", "no", no);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<AccountsTable>(entity.Table);
        } catch (Exception e) {
            Log.Error(typeof(AccountsDao), e);
        }
        return table;
    }

}
}
