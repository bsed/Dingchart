using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheManager.Core;
using System.Collections.Concurrent;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Models.Tables;
using System.Collections;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.Business.Cache {
class CacheHelper {
    //--------------------Cache-------------------------//
    private ICacheManager<GroupCacheEntity> groupsCache = null;
    private ICacheManager<VcardCacheEntity> vcardsCache = null;
    private ICacheManager<OrganizationCacheEntity> organizationCache = null;
    private ICacheManager<AppCacheEntity> appCache = null;
    private ICacheManager<SubscribtionCacheEntity> subscribtionCache = null;
    private ICacheManager<ContactSettingCacheEntity> contactSettingCache = null;
    private ICacheManager<ContactsCacheEntity> contactsCache = null;

    //--------------------Keys-------------------------//
    private ConcurrentDictionary<String, String> groupKeys = null;
    private ConcurrentDictionary<String, String> vcardKeys = null;
    private ConcurrentDictionary<String, String> organizationKeys = null;
    private ConcurrentDictionary<String, String> appKeys = null;
    private ConcurrentDictionary<String, String> subscribtionKeys = null;
    private ConcurrentDictionary<String, String> contactSettingKeys = null;
    private ConcurrentDictionary<String, String> contactsKeys = null;

    private static CacheHelper cacheHelper = null;

    private static Object locker = new Object();

    private CacheHelper() {
        //--------------------Keys 初始化-------------------------//
        groupKeys = new ConcurrentDictionary<String, String>();
        vcardKeys = new ConcurrentDictionary<String, String>();
        organizationKeys = new ConcurrentDictionary<String, String>();
        appKeys = new ConcurrentDictionary<String, String>();
        subscribtionKeys = new ConcurrentDictionary<String, String>();
        contactSettingKeys = new ConcurrentDictionary<String, String>();
        contactsKeys = new ConcurrentDictionary<String, String>();

        //--------------------Cache 初始化-------------------------//
        groupsCache = CacheFactory.Build<GroupCacheEntity>("groupsCache", settings => {
            settings.WithMaxRetries(2000)
            .WithDictionaryHandle();

        });

        vcardsCache = CacheFactory.Build<VcardCacheEntity>("vcardsCache", settings => {
            settings.WithMaxRetries(5000);
            settings.WithDictionaryHandle();
        });

        organizationCache = CacheFactory.Build<OrganizationCacheEntity>("organizationCache", settings => {
            settings.WithMaxRetries(2000);
            settings.WithDictionaryHandle();
        });

        appCache = CacheFactory.Build<AppCacheEntity>("appCache", settings => {
            settings.WithMaxRetries(500);
            settings.WithDictionaryHandle();
        });

        subscribtionCache = CacheFactory.Build<SubscribtionCacheEntity>("subscribtionCache", settings => {
            settings.WithMaxRetries(500);
            settings.WithDictionaryHandle();
        });

        contactSettingCache = CacheFactory.Build<ContactSettingCacheEntity>("contactSettingCache", settings => {
            settings.WithMaxRetries(1000);
            settings.WithDictionaryHandle();
        });

        contactsCache = CacheFactory.Build<ContactsCacheEntity>("contactsCache", settings => {
            settings.WithMaxRetries(1000);
            settings.WithDictionaryHandle();
        });
    }

    public class KeyPair {
        public String key {
            get;
            set;
        }
        public String value {
            get;
            set;
        }
    }


    public static CacheHelper getInstance() {
        if (cacheHelper == null) {
            cacheHelper = new CacheHelper();
        }
        return cacheHelper;
    }


    //--------------------Vcard start-------------------------//
    private KeyPair getVcardKey(String idOrNo) {
        ICollection<String> keys = this.vcardKeys.Keys;
        for (int i = 0; i < keys.Count; i++) {
            String key = keys.ElementAt<String>(i);
            if (key.Contains("_" + idOrNo + "_")) {
                String retrievedValue = null;
                this.vcardKeys.TryGetValue(key, out retrievedValue);
                KeyPair keypair = new KeyPair();
                keypair.key = key;
                keypair.value = retrievedValue;
                return keypair;
            }
        }
        return null;
    }
    public bool addVcard(VcardCacheEntity vcard) {
        var cvcard = this.vcardsCache.Get<VcardCacheEntity>(vcard.getKey());
        if (cvcard == null) {
            cvcard = vcard;
        }
        if (cvcard.id==null) {
            return false;
        }
        this.vcardKeys.TryAdd(cvcard.id.getIdString(), cvcard.getKey());
        this.vcardsCache.Put(cvcard.getKey(), cvcard);
        return true;
    }

    public VcardCacheEntity getVcard(String idOrNo) {
        KeyPair keypair = this.getVcardKey(idOrNo);
        if (keypair == null)
            return null;
        return this.vcardsCache.Get<VcardCacheEntity>(keypair.value);
    }

    //--------------------Vcard end-------------------------//


    //--------------------Organization start-------------------------//
    private KeyPair getOrganizationKey(String idOrNo) {
        ICollection<String> keys = this.organizationKeys.Keys;
        for (int i = 0; i < keys.Count; i++) {
            String key = keys.ElementAt<String>(i);
            if (key.Contains("_" + idOrNo + "_")) {
                String retrievedValue = null;
                this.organizationKeys.TryGetValue(key, out retrievedValue);
                KeyPair keypair = new KeyPair();
                keypair.key = key;
                keypair.value = retrievedValue;
                return keypair;
            }
        }
        return null;
    }

    public List<OrganizationCacheEntity> getAllOrgs() {
        List<OrganizationCacheEntity> orgs = new List<OrganizationCacheEntity>();
        ICollection<String> keys =  this.organizationKeys.Values;
        foreach (String key in keys) {
            OrganizationCacheEntity cache = this.getOrganization(key);
            if (cache != null)
                orgs.Add(cache);
        }
        return orgs;
    }
    public bool addOrganization(OrganizationCacheEntity org) {
        if (org == null || org.id == null) {
            return false;
        }

        this.organizationCache.Put(org.getKey(), org);
        this.organizationKeys.TryAdd(org.id.getIdString(), org.getKey());
        return true;
    }

    public OrganizationCacheEntity getOrganization(String idOrNo) {
        KeyPair keypair = this.getOrganizationKey(idOrNo);
        if (keypair == null)
            return null;
        return this.organizationCache.Get<OrganizationCacheEntity>(keypair.value);
    }


    public bool removeOrganization(String idOrNo) {
        KeyPair keypair = this.getOrganizationKey(idOrNo);
        if(keypair==null) {
            return true;
        }
        this.organizationCache.Remove(keypair.value);
        String result = null;
        this.organizationKeys.TryRemove(keypair.key, out result);
        return true;
    }
    public void clearOrganization() {
        this.organizationCache.Clear();
        this.organizationKeys.Clear();
    }
    //--------------------Organization end-------------------------//


    //--------------------OrganizationMember start-------------------------//
    public bool addOrgMember(String idOrNo, OrgMemberCacheEntity member) {
        OrganizationCacheEntity org = this.getOrganization(idOrNo);
        if (org == null || member == null || member.vcard == null || member.vcard.id == null
                || member.table == null) {
            return false;
        }
        org.removeMember(member);
        org.addMember(member);
        return true;
    }

    public bool removeOrgMember(String orgIdOrNo, String userIdOrNo) {
        OrganizationCacheEntity org = this.getOrganization(orgIdOrNo);
        if (org == null || userIdOrNo == null) {
            return false;
        }
        org.removeMember(userIdOrNo);
        return true;
    }

    public OrgMemberCacheEntity getOrgMember(String orgIdOrNo, String userIdOrNo) {
        if (orgIdOrNo == null) {
            return null;
        }
        OrganizationCacheEntity org = this.getOrganization(orgIdOrNo);
        if (org == null || userIdOrNo == null) {
            return null;
        }
        return org.getMember(userIdOrNo);

    }

    public IList<OrgMemberCacheEntity> getAllOrgMembers(String orgIdOrNo) {
        if (orgIdOrNo == null) {
            return null;
        }
        OrganizationCacheEntity org = this.getOrganization(orgIdOrNo);
        if (org == null) {
            return null;
        }
        return org.getMembers();

    }
    //--------------------OrganizationMember end-------------------------//



    //--------------------Apps start-------------------------//

    private KeyPair getAppKey(String idOrNo) {
        ICollection<String> keys = this.appKeys.Keys;
        for (int i = 0; i < keys.Count; i++) {
            String key = keys.ElementAt<String>(i);
            if (key.Contains("_" + idOrNo + "_")) {
                String retrievedValue = null;
                this.appKeys.TryGetValue(key, out retrievedValue);
                KeyPair keypair = new KeyPair();
                keypair.key = key;
                keypair.value = retrievedValue;
                return keypair;
            }
        }
        return null;
    }

    public void clearApps() {
        this.appCache.Clear();
        this.appKeys.Clear();
    }

    public List<AppCacheEntity> getAllApps() {
        List<AppCacheEntity> apps = new List<AppCacheEntity>();
        ICollection<String> keys = this.appKeys.Values;
        foreach (String key in keys) {
            AppCacheEntity cache =  this.getApp(key);
            if(cache != null)
                apps.Add(cache);
        }
        return apps;
    }
    public bool addApp(AppCacheEntity app) {
        if (app == null || app.id == null) {
            return false;
        }
        this.appCache.Put(app.getKey(), app);

        this.appKeys.TryAdd(app.id.getIdString(), app.getKey());
        return true;
    }
    public AppCacheEntity getApp(String idOrNo) {
        KeyPair keypair = this.getAppKey(idOrNo);
        if (keypair == null)
            return null;
        return this.appCache.Get<AppCacheEntity>(keypair.value);
    }

    public bool removeApp(String idOrNo) {
        KeyPair keypair = this.getAppKey(idOrNo);
        if(keypair!=null) {
            this.appCache.Remove(keypair.value);
            String result = null;
            this.appKeys.TryRemove(keypair.key, out result);
        }

        return true;
    }
    //--------------------Apps end-------------------------//


    //--------------------Subscribtions start-------------------------//
    private KeyPair getSubscriptionKey(String idOrNo) {
        ICollection<String> keys = this.subscribtionKeys.Keys;
        for (int i = 0; i < keys.Count; i++) {
            String key = keys.ElementAt<String>(i);
            if (key.Contains("_" + idOrNo + "_")) {
                String retrievedValue = null;
                this.subscribtionKeys.TryGetValue(key, out retrievedValue);
                KeyPair keypair = new KeyPair();
                keypair.key = key;
                keypair.value = retrievedValue;
                return keypair;
            }
        }
        return null;
    }
    public List<SubscribtionCacheEntity> getAllSubscribtions() {
        List<SubscribtionCacheEntity> subscribtions = new List<SubscribtionCacheEntity>();
        ICollection<String> keys = this.subscribtionKeys.Values;
        foreach (String key in keys) {
            SubscribtionCacheEntity cache = this.getSubscription(key);
            if (cache != null)
                subscribtions.Add(cache);
        }
        return subscribtions;
    }


    public bool addSubscription(SubscribtionCacheEntity sub) {
        if (sub == null || sub.id == null) {
            return false;
        }
        this.subscribtionCache.Put(sub.getKey(), sub);
        this.subscribtionKeys.TryAdd(sub.id.getIdString(), sub.getKey());
        return true;
    }

    public SubscribtionCacheEntity getSubscription(String idOrNo) {
        KeyPair keypair = this.getSubscriptionKey(idOrNo);
        if (keypair == null)
            return null;
        return this.subscribtionCache.Get<SubscribtionCacheEntity>(keypair.value);
    }


    public bool removeSubscription(String idOrNo) {
        KeyPair keypair = this.getSubscriptionKey(idOrNo);
        if (keypair != null) {
            this.subscribtionCache.Remove(keypair.value);
            String result = null;
            this.subscribtionKeys.TryRemove(keypair.key, out result);
        }
        return true;
    }
    //--------------------Subscribtions end-------------------------//


    //--------------------Group start-------------------------//
    private KeyPair getGroupKey(String idOrNo) {
        ICollection<String> keys = this.groupKeys.Keys;
        for (int i = 0; i < keys.Count; i++) {
            String key = keys.ElementAt<String>(i);
            if (key.Contains("_" + idOrNo + "_")) {
                String retrievedValue = null;
                this.groupKeys.TryGetValue(key, out retrievedValue);
                KeyPair keypair = new KeyPair();
                keypair.key = key;
                keypair.value = retrievedValue;
                return keypair;
            }
        }
        return null;
    }
    public bool addGroup(GroupCacheEntity group) {
        if (group == null || group.id == null) {
            return false;
        }
        this.groupsCache.Put(group.getKey(),group);
        this.groupKeys.TryAdd(group.id.getIdString(), group.getKey());
        return true;
    }


    public bool removeGroup(String idOrNo) {
        KeyPair keypair = this.getGroupKey(idOrNo);
        if (keypair != null) {
            this.groupsCache.Remove(keypair.value);
            String result = null;
            this.groupKeys.TryRemove(keypair.key, out result);
        }
        return true;
    }

    public GroupCacheEntity getGroup(String idOrNo) {
        KeyPair keypair = this.getGroupKey(idOrNo);
        if (keypair == null)
            return null;
        return this.groupsCache.Get<GroupCacheEntity>(keypair.value);
    }

    //--------------------Group end-------------------------//


    //--------------------GroupMember start-------------------------//


    public bool addGroupMember(String groupIdOrNo, GroupMemberCacheEntity groupMember) {
        GroupCacheEntity group = this.getGroup(groupIdOrNo);
        if (group == null || groupMember == null || groupMember.vcard == null || groupMember.vcard.id==null) {
            return false;
        }
        group.removeMember(groupMember);
        group.addMember(groupMember);
        return true;
    }

    public bool removeGroupMember(String groupIdOrNo, String userIdOrNo) {
        GroupCacheEntity group = this.getGroup(groupIdOrNo);
        if (group == null || userIdOrNo == null) {
            return false;
        }
        group.removeMember( groupIdOrNo,userIdOrNo);
        return true;
    }

    public GroupMemberCacheEntity getGroupMember(String groupIdOrNo, String userIdOrNo) {
        if (groupIdOrNo == null) {
            return null;
        }
        GroupCacheEntity group = this.getGroup(groupIdOrNo);
        if (group == null || userIdOrNo == null) {
            return null;
        }
        return group.getMember(userIdOrNo);

    }
    //--------------------GroupMember end-------------------------//



    //--------------------ContactSetting start-------------------------//
    public void clearContactSetting() {
        this.contactSettingCache.Clear();
        this.contactSettingKeys.Clear();
    }

    public List<ContactSettingCacheEntity> getAllContactSetting() {
        List<ContactSettingCacheEntity> contactSetting = new List<ContactSettingCacheEntity>();
        ICollection<String> keys = this.contactSettingKeys.Values;
        foreach (String key in keys) {
            ContactSettingCacheEntity cache = this.getContactSetting(key);
            if (cache != null)
                contactSetting.Add(cache);
        }
        return contactSetting;
    }

    public bool addContactSetting(ContactSettingCacheEntity contactSetting) {
        if (contactSetting == null || contactSetting.id == null) {
            return false;
        }
        this.contactSettingCache.Put(contactSetting.getKey(), contactSetting);
        this.contactSettingKeys.TryAdd(contactSetting.id.getIdString(), contactSetting.getKey());
        return true;
    }
    public ContactSettingCacheEntity getContactSetting(String idOrNo) {
        KeyPair keypair = this.getContactSettingKey(idOrNo);
        if (keypair == null)
            return null;
        return this.contactSettingCache.Get<ContactSettingCacheEntity>(keypair.value);
    }

    public bool removeContactSetting(String idOrNo) {
        KeyPair keypair = this.getContactSettingKey(idOrNo);
        if (keypair != null) {
            this.contactSettingCache.Remove(keypair.value);
            String result = null;
            this.contactSettingKeys.TryRemove(keypair.key, out result);
        }
        return true;
    }

    private KeyPair getContactSettingKey(String idOrNo) {
        ICollection<String> keys = this.contactSettingKeys.Keys;
        for (int i = 0; i < keys.Count; i++) {
            String key = keys.ElementAt<String>(i);
            if (key.Contains("_" + idOrNo + "_")) {
                String retrievedValue = null;
                this.contactSettingKeys.TryGetValue(key, out retrievedValue);
                KeyPair keypair = new KeyPair();
                keypair.key = key;
                keypair.value = retrievedValue;
                return keypair;
            }
        }
        return null;
    }



    //--------------------ContactSetting end-------------------------//



    //--------------------Contacts start-------------------------//
    public void clearContacts() {
        this.contactsCache.Clear();
        this.contactsKeys.Clear();
    }

    public List<ContactsCacheEntity> getAllContacts() {
        List<ContactsCacheEntity> contacts = new List<ContactsCacheEntity>();
        ICollection<String> keys = this.contactsKeys.Values;
        foreach (String key in keys) {
            ContactsCacheEntity cache = this.getContacts(key);
            if (cache != null)
                contacts.Add(cache);
        }
        return contacts;
    }

    public bool addContacts(ContactsCacheEntity contacts) {
        if (contacts == null || contacts.id == null) {
            return false;
        }
        this.contactsCache.Put(contacts.getKey(), contacts);
        this.contactsKeys.TryAdd(contacts.id.getIdString(), contacts.getKey());
        return true;
    }
    public ContactsCacheEntity getContacts(String idOrNo) {
        KeyPair keypair = this.getContactsKey(idOrNo);
        if (keypair == null)
            return null;
        return this.contactsCache.Get<ContactsCacheEntity>(keypair.value);
    }

    public bool removeContacts(String idOrNo) {
        KeyPair keypair = this.getContactsKey(idOrNo);
        if (keypair != null) {
            this.contactsCache.Remove(keypair.value);
            String result = null;
            this.contactsKeys.TryRemove(keypair.key, out result);
        }


        return true;
    }

    private KeyPair getContactsKey(String idOrNo) {
        ICollection<String> keys = this.contactsKeys.Keys;
        for (int i = 0; i < keys.Count; i++) {
            String key = keys.ElementAt<String>(i);
            if (key.Contains("_" + idOrNo + "_")) {
                String retrievedValue = null;
                this.contactsKeys.TryGetValue(key, out retrievedValue);
                KeyPair keypair = new KeyPair();
                keypair.key = key;
                keypair.value = retrievedValue;
                return keypair;
            }
        }
        return null;
    }



    //--------------------Contacts end-------------------------//



    //--------------------loadAll start-------------------------//
    private void loadAllFriendsVcard() {
        try {
            List<ContactsTable> friends = ContactsDao.getInstance().FindAllFriend(null, null);
            foreach (ContactsTable friend in friends) {
                VcardCacheEntity vcard = new VcardCacheEntity();
                IdNoCacheEntity idNo = new IdNoCacheEntity();
                idNo.id = friend.clientuserId.ToString();
                idNo.no = friend.no;

                vcard.table = VcardsDao.getInstance().findByNoFromDb(friend.no);

                vcard.id = idNo;
                vcard.IsFriend = true;
                if (friend.alias != null)
                    vcard.alias = friend.alias;
                else {
                    if (vcard.table != null)
                        vcard.alias = vcard.table.nickname;
                    else
                        vcard.alias = friend.alias;
                }


                this.addVcard(vcard);
            }
        } catch (Exception ex) {
            Log.Error(typeof(CacheHelper), ex);
        }
    }
    private void loadAllVcard() {
    }

    private void loadAllGroup() {
        try {
            List<MucTable> groups = MucDao.getInstance().FindAllGroup();
            foreach (MucTable group in groups) {
                GroupCacheEntity cgroup = new GroupCacheEntity();
                IdNoCacheEntity idNo = new IdNoCacheEntity();
                idNo.id = group.mucId.ToString();
                idNo.no = group.no;
                cgroup.id = idNo;
                this.addGroup(cgroup);

                List<MucMembersTable> members = MucMembersDao.getInstance().findByMucNoFromDb(group.no);
                foreach (MucMembersTable member in members) {
                    GroupMemberCacheEntity cmember = new GroupMemberCacheEntity();
                    cmember.name = member.nickname;
                    VcardCacheEntity vcard = this.getVcard(member.no);
                    if (vcard == null) {
                        vcard = new VcardCacheEntity();
                        VcardsTable card = VcardsDao.getInstance().findByNoFromDb(member.no);
                        if (card == null) {
                            continue;
                        }
                        IdNoCacheEntity cardIdNo = new IdNoCacheEntity();
                        cardIdNo.id = card.clientuserId.ToString();
                        cardIdNo.no = card.no;
                        vcard.id = cardIdNo;
                        vcard.IsFriend = false;
                        vcard.alias = card.nickname;
                        vcard.table = card;
                    }
                    cmember.vcard = vcard;
                    this.addGroupMember(group.no, cmember);
                }


            }
        } catch (Exception ex) {
            Log.Error(typeof(CacheHelper), ex);
        }
    }

    public void loadAllApp() {
        try {

            List<PublicWebTable> apps = PublicWebDao.getInstance().FindAllFromDb(null, null);
            foreach (PublicWebTable table in apps) {
                AppCacheEntity cache = new AppCacheEntity();
                IdNoCacheEntity idNo = new IdNoCacheEntity();
                idNo.no = table.appId;
                idNo.id = table.appId;
                cache.id = idNo;
                cache.table = table;
                this.addApp(cache);
            }
        } catch (Exception ex) {
            Log.Error(typeof(CacheHelper), ex);
        }
    }

    private void loadAllSubscription() {
        try {
            List<PublicAccountsTable> subscriptions = PublicAccountsDao.getInstance().FindAllFromDB("");
            foreach( PublicAccountsTable table in subscriptions) {
                SubscribtionCacheEntity cache = new SubscribtionCacheEntity();
                IdNoCacheEntity idNo = new IdNoCacheEntity();
                idNo.no = table.appid;
                idNo.id = table.appid;
                cache.id = idNo;
                cache.table = table;
                this.addSubscription(cache);
            }
        } catch (Exception ex) {
            Log.Error(typeof(CacheHelper), ex);
        }
    }

    public void loadAllOrganization() {
        try {
            // 清空组织
            this.clearOrganization();

            //先获取所有的组织，然后获取组织下的所有人员
            List<OrganizationTable> orgs = OrganizationDao.getInstance().FindAllOrganizationFromDB(null, null);

            foreach (OrganizationTable table in orgs) {
                //先把org加入缓存，然后获取组织用户
                OrganizationCacheEntity orgCache = new OrganizationCacheEntity();
                IdNoCacheEntity idNo = new IdNoCacheEntity();
                idNo.id = table.organizationId;
                idNo.no = table.organizationId;
                orgCache.id = idNo;
                orgCache.table = table;

                List<OrganizationMemberTable> members = OrganizationMemberDao.getInstance().FindOrganizationMemberByOrgIdFromDB(int.Parse(table.organizationId));
                foreach(OrganizationMemberTable mem in members) {
                    OrgMemberCacheEntity memCache = new OrgMemberCacheEntity();
                    IdNoCacheEntity memIdNo = new IdNoCacheEntity();
                    memIdNo.id = mem.no;
                    memIdNo.no = mem.no;
                    memCache.id = memIdNo;
                    memCache.sortNum = int.Parse(mem.sortNum);
                    if (mem.sortNum == null || mem.sortNum.Trim().Equals("")) {
                        memCache.sortNum = 100000;
                    }

                    VcardsDao.getInstance().findByNo(mem.no);
                    VcardCacheEntity vcard =  this.getVcard(mem.no);
                    memCache.vcard = vcard;
                    memCache.table = mem;
                    orgCache.addMember(memCache);
                }

                this.addOrganization(orgCache);
            }
        } catch (Exception ex) {
            Log.Error(typeof(CacheHelper), ex);
        }
    }

    public void loadAllContactSetting() {
        try {
            List<SettingsTable> settings = SettingDao.getInstance().FindAllFromDb();
            foreach (SettingsTable table in settings) {
                ContactSettingCacheEntity cache = new ContactSettingCacheEntity();
                IdNoCacheEntity idNo = new IdNoCacheEntity();
                idNo.no = table.no;
                idNo.id = table.id.ToString();
                cache.id = idNo;
                cache.table = table;
                this.addContactSetting(cache);
            }
        } catch (Exception ex) {
            Log.Error(typeof(CacheHelper), ex);
        }
    }

    public void loadAllContacts() {
        try {
            List<ContactsTable> contacts = ContactsDao.getInstance().FindAllFriend(null, null);
            foreach (ContactsTable table in contacts) {
                ContactsCacheEntity cache = new ContactsCacheEntity();
                IdNoCacheEntity idNo = new IdNoCacheEntity();
                idNo.no = table.no;
                idNo.id = table.clientuserId;
                cache.id = idNo;
                cache.table = table;
                this.addContacts(cache);
            }
        } catch (Exception ex) {
            Log.Error(typeof(CacheHelper), ex);
        }
    }

    //--------------------loadAll end-------------------------//



    /// <summary>
    /// 清空缓存
    /// </summary>
    public void resetCache() {
        try {
            //--------------------Cache-------------------------//
            this.vcardsCache.Clear();
            this.groupsCache.Clear();
            this.organizationCache.Clear();
            this.appCache.Clear();
            this.subscribtionCache.Clear();
            this.contactSettingCache.Clear();
            this.contactsCache.Clear();

            //--------------------Keys-------------------------//
            this.vcardKeys.Clear();
            this.groupKeys.Clear();
            this.organizationKeys.Clear();
            this.subscribtionKeys.Clear();
            this.appKeys.Clear();
            this.contactSettingKeys.Clear();
            this.contactsKeys.Clear();

        } catch (Exception ex) {
            Log.Error(typeof(CacheHelper), ex);
        }
    }

    public void loadAllToCache() {
        lock (locker) {
            Console.WriteLine("缓存数据加载：开始");
            this.resetCache();

            //加载所有的好友的Vcard
            this.loadAllFriendsVcard();

            //加载所有的应用
            this.loadAllApp();

            //加载所有的公众号
            this.loadAllSubscription();

            //加载所有的组织
            this.loadAllOrganization();

            // 加载所有的好友设置
            this.loadAllContactSetting();

            // 加载所有的好友
            this.loadAllContacts();

            //加载所有的群信息
            this.loadAllGroup();

            App.CacheLoadOk = true;
            Console.WriteLine("缓存数据加载：完毕");
            BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
            Businessdata.eventDataType = BusinessEventDataType.LoadingOk;
            EventBusHelper.getInstance().fireEvent(Businessdata);
        }

    }
}


}
