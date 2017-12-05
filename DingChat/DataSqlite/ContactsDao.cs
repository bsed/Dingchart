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
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Business.Cache;
using cn.lds.chatcore.pcw.Business;

namespace cn.lds.chatcore.pcw.DataSqlite {
class ContactsDao : BaseDao {


    private static ContactsDao instance = null;
    public static ContactsDao getInstance() {
        if (instance == null) {
            instance = new ContactsDao();
        }
        return instance;
    }

    private VcardsTable getFromCache(String idOrNo) {
        VcardCacheEntity cvcard = CacheHelper.getInstance().getVcard(idOrNo);
        if (cvcard == null)
            return null;
        return cvcard.table;
    }

    private void updateCache(VcardsTable table) {
        VcardCacheEntity vcard = CacheHelper.getInstance().getVcard(table.no);
        if (vcard == null) {
            vcard = new VcardCacheEntity();
            IdNoCacheEntity idno = new IdNoCacheEntity();
            idno.id = table.clientuserId;
            idno.no = table.no;
            vcard.id = idno;

        }
        vcard.table = table;
        CacheHelper.getInstance().addVcard(vcard);
    }

    private ContactsCacheEntity getContactsCacheEntity(String idOrNo) {
        ContactsCacheEntity cacheEntity = CacheHelper.getInstance().getContacts(idOrNo);
        if (cacheEntity == null) {
            return null;
        }
        if (cacheEntity.table == null) {
            return null;
        }
        return cacheEntity;
    }

    private void updateContactsCache(ContactsTable table) {
        try {
            if (table == null) {
                return;
            }
            ContactsCacheEntity cacheEntity = CacheHelper.getInstance().getContacts(table.no);
            if (cacheEntity == null) {
                cacheEntity = new ContactsCacheEntity();
                IdNoCacheEntity idno = new IdNoCacheEntity();
                idno.id = table.clientuserId;
                idno.no = table.no;
                cacheEntity.id = idno;
            }
            cacheEntity.table = table;
            CacheHelper.getInstance().addContacts(cacheEntity);
        } catch (Exception e) {
            Log.Error(typeof(SettingDao), e);
        }
    }


    public int deleteByUserId(String ClientuserId) {
        int count = -1;
        try {
            SQLiteParameter[] param = new SQLiteParameter[] {
                new SQLiteParameter("ClientuserId",ClientuserId)
            };
            count = this._mgr.Delete("contacts", "ClientuserId=@ClientuserId", param);

            CacheHelper.getInstance().removeContacts(ClientuserId);
        } catch (Exception e) {
            Log.Error(typeof(ContactsDao), e);
        }
        return count;

    }

    /// <summary>
    /// 保存好友信息
    /// </summary>
    /// <param Name="bean"></param>
    /// <returns></returns>
    public int SaveFriend(ContactsTable bean) {
        int count = -1;
        try {
            //_mgr.Delete("contacts", "clientuserId=@clientuserId", new SQLiteParameter[]
            //        {
            //            new SQLiteParameter("clientuserId", bean.clientuserId)
            //        });
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("clientuserId", bean.clientuserId.ToStr());
            entity.Add("no", bean.no.ToStr());
            entity.Add("AvatarStorageRecordId", bean.avatarStorageRecordId);
            entity.Add("Name", bean.name);
            entity.Add("nickname", bean.nickname);
            entity.Add("alias", bean.alias);
            entity.Add("favorite", bean.favorite);
            entity.Add("flag", bean.flag);
            entity.Add("deletedBy", bean.deletedBy);
            String totalPinyin = null;
            String fristPinyin = null;
            if (!string.IsNullOrEmpty(bean.alias)) {
                totalPinyin = PinyinHelper.getTotalPinyin(bean.alias);
                fristPinyin = PinyinHelper.getFristPinyin(bean.alias);
            } else {
                totalPinyin = PinyinHelper.getTotalPinyin(bean.nickname);
                fristPinyin = PinyinHelper.getFristPinyin(bean.nickname);
            }
            entity.Add("totalPinyin", totalPinyin);
            entity.Add("fristPinyin", fristPinyin);

            if (this.isExist("contacts", "clientuserId", bean.clientuserId)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("clientuserId",bean.clientuserId)
                };
                count = this._mgr.Update("contacts", entity, "clientuserId=@clientuserId", param);
            } else {
                count = this._mgr.Save("contacts", entity);
            }

            this.updateContactsCache(bean);
        } catch (Exception e) {
            Log.Error(typeof(ContactsDao), e);
        }



        return count;

    }

    /// <summary>
    /// 插入好友列表  contacts
    /// </summary>
    /// <param Name="dt"></param>
    public int InsertFriend(List<ContactsTable> contacts) {
        int count = -1;
        try {
            List<Dictionary<string, object>> entitys = new List<Dictionary<string, object>>();
            for (int i = 0; i < contacts.Count; i++) {
                ContactsTable bean = contacts[i];

                _mgr.Delete("contacts", "clientuserId=@clientuserId", new SQLiteParameter[] {
                    new SQLiteParameter("clientuserId", bean.clientuserId)
                });

                if (contacts[i].flag != "normal") continue;
                Dictionary<string, object> entity = new Dictionary<string, object>();

                entity.Add("clientuserId", bean.clientuserId.ToStr());
                entity.Add("no", bean.no.ToStr());
                entity.Add("AvatarStorageRecordId", bean.avatarStorageRecordId);
                entity.Add("Name", bean.name);
                entity.Add("nickname", bean.nickname);
                entity.Add("alias", bean.alias);
                entity.Add("favorite", bean.favorite);
                entity.Add("flag", bean.flag);
                entity.Add("deletedBy", bean.deletedBy);
                String totalPinyin = null;
                String fristPinyin = null;
                if (!string.IsNullOrEmpty(bean.alias)) {
                    totalPinyin = PinyinHelper.getTotalPinyin(bean.alias);
                    fristPinyin = PinyinHelper.getFristPinyin(bean.alias);
                } else {
                    totalPinyin = PinyinHelper.getTotalPinyin(bean.nickname);
                    fristPinyin = PinyinHelper.getFristPinyin(bean.nickname);
                }
                entity.Add("totalPinyin", totalPinyin);
                entity.Add("fristPinyin", fristPinyin);
                entitys.Add(entity);

            }
            count = this._mgr.Save("contacts", entitys);

            // 更新缓存
            foreach (ContactsTable table in contacts) {
                if (table.flag != "normal") continue;
                this.updateContactsCache(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(ContactsDao), e);
        }

        return count;
    }

    /// <summary>
    /// 查找所有通讯录
    /// </summary>
    /// <returns></returns>
    public List<ContactsTable> FindAllFriend(string conditionCol, object conditionVal) {
        List<ContactsTable> list = null;
        try {
            list = new List<ContactsTable>();
            Dictionary<string, object> entity = new Dictionary<string, object>();
            if (conditionCol!=null) {
                entity.Add(conditionCol, conditionVal);
            }

            Dictionary< string, object> orders = new Dictionary<string, object>();
            orders.Add("totalPinyin", "asc");

            DataTable dataTable = this._mgr.QueryDt("contacts", entity, orders);
            list = DataUtils.DataTableToModelList<ContactsTable>(dataTable);
        } catch (Exception e) {
            Log.Error(typeof(ContactsDao), e);
        }
        return list;

    }


    /// <summary>
    /// 根据ID查找联系人
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public ContactsTable FindContactsById(long id) {
        try {
            ContactsCacheEntity cacheEntity = this.getContactsCacheEntity(id.ToStr());
            if (cacheEntity != null && cacheEntity.table != null) {
                return cacheEntity.table;
            }
        } catch (Exception e) {
            Log.Error(typeof(ContactsDao), e);
        }


        ContactsTable table = null;
        try {
            DataRow entity = this._mgr.QueryOne("contacts", "Clientuserid", id);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<ContactsTable>(entity.Table);
            return table;
        } catch (Exception e) {
            Log.Error(typeof(ContactsDao), e);
            return null;
        }
    }

    /// <summary>
    /// 根据NO查找联系人
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public ContactsTable FindContactsByNo(String no) {

        try {
            ContactsCacheEntity cacheEntity = this.getContactsCacheEntity(no);
            if (cacheEntity != null && cacheEntity.table != null) {
                return cacheEntity.table;
            }
        } catch (Exception e) {
            Log.Error(typeof(ContactsDao), e);
        }


        ContactsTable table = null;
        try {

            DataRow entity = this._mgr.QueryOne("contacts", "no", no);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<ContactsTable>(entity.Table);
            return table;
        } catch (Exception e) {
            Log.Error(typeof(ContactsDao), e);
            return null;
        }
    }

    /// <summary>
    /// 重构没有拼音的数据
    /// </summary>
    public void ReBuildDataWithoutPinyin() {

        try {
            List<ContactsTable> list = null;
            String sql = "select * from contacts where totalPinyin is null or totalPinyin = ''";
            DataTable dt = this._mgr.ExecuteRow(sql, null);
            list = DataUtils.DataTableToModelList<ContactsTable>(dt);
            if (list!=null) {
                foreach (ContactsTable table in list) {
                    this.SaveFriend(table);
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(ContactsDao), e);
        }
    }

}
}
