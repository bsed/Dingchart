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
using System.Threading;
using cn.lds.im.sdk.message.util;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Business.Cache;
using EventBus;
using cn.lds.chatcore.pcw.imtp;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Common.Utils;
namespace cn.lds.chatcore.pcw.imtp {
public class ImTest {



    //public static void Main(string[] args) {
    //    ImTest test = new ImTest();
    //    EventBusHelper demo = EventBusHelper.getInstance();
    //    demo.register(test);
    //    Console.WriteLine("----1:" + DateTimeHelper.getTimeStamp());
    //    BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
    //    Businessdata.eventDataType = BusinessEventDataType.LoadingOk;
    //    EventBusHelper.getInstance().fireEvent(Businessdata);
    //    Console.WriteLine("----2:" + DateTimeHelper.getTimeStamp());
    //    Thread.Sleep(1000000);
    //    Console.WriteLine("----4:" + DateTimeHelper.getTimeStamp());
    //    Console.ReadKey();

    //}
    //[EventSubscriber]
    //public void onMucEvent(BusinessEvent<Object> data) {
    //    Console.WriteLine("----3:" + DateTimeHelper.getTimeStamp());
    //    Console.WriteLine("xxxxxxxxxxxxxxx");
    //}



    //public static void Main(string[] args) {
    //    CacheHelper cache = CacheHelper.getInstance();
    //    addVcard();
    //    addGroup();
    //    addGroupMember("gno_0");
    //    addGroupMember("gno_1");
    //    addGroupMember("gno_2");



    //    VcardCacheEntity e =  cache.getVcard("id_1");
    //    GroupCacheEntity g = cache.getGroup("gno_1");
    //    GroupMemberCacheEntity gm= cache.getGroupMember("gno_2", "id_2");

    //    cache.removeGroup("gno_3");

    //    cache.removeGroupMember("gno_2","id_1");

    //    GroupCacheEntity g1 = cache.getGroup("gno_2");

    //    Console.ReadKey();



    //}
    //public static void addGroupMember(String groupNo) {
    //    GroupMemberCacheEntity member = new GroupMemberCacheEntity();
    //    member.Name="mbername_0";
    //    member.vcard = CacheHelper.getInstance().getVcard("no_0");

    //    GroupMemberCacheEntity member1 = new GroupMemberCacheEntity();
    //    member1.Name="mbername_1";
    //    member1.vcard = CacheHelper.getInstance().getVcard("no_1");

    //    GroupMemberCacheEntity member2 = new GroupMemberCacheEntity();
    //    member2.Name = "mbername_2";
    //    member2.vcard = CacheHelper.getInstance().getVcard("no_2");

    //    CacheHelper.getInstance().addGroupMember(groupNo, member);
    //    CacheHelper.getInstance().addGroupMember(groupNo, member1);
    //    CacheHelper.getInstance().addGroupMember(groupNo, member2);
    //}
    //public static void addGroup() {
    //    GroupCacheEntity group = new GroupCacheEntity();
    //    IdNoCacheEntity id = new IdNoCacheEntity();
    //    id.no = "gno_0";
    //    id.id = "gid_0";
    //    group.id = id;
    //    group.Name = "groupname_0";


    //    GroupCacheEntity group1 = new GroupCacheEntity();
    //    IdNoCacheEntity id1 = new IdNoCacheEntity();
    //    id1.no = "gno_1";
    //    id1.id = "gid_1";
    //    group1.id = id1;
    //    group1.Name = "groupname_1";


    //    GroupCacheEntity group2 = new GroupCacheEntity();
    //    IdNoCacheEntity id2 = new IdNoCacheEntity();
    //    id2.no = "gno_2";
    //    id2.id = "gid_2";
    //    group2.id = id2;
    //    group2.Name = "groupname_2";

    //    GroupCacheEntity group3 = new GroupCacheEntity();
    //    IdNoCacheEntity id3 = new IdNoCacheEntity();
    //    id3.no = "gno_3";
    //    id3.id = "gid_3";
    //    group3.id = id3;
    //    group3.Name = "groupname_3";

    //    CacheHelper.getInstance().addGroup(group);
    //    CacheHelper.getInstance().addGroup(group1);
    //    CacheHelper.getInstance().addGroup(group2);
    //    CacheHelper.getInstance().addGroup(group3);

    //}

    //public static void addVcard() {
    //    VcardCacheEntity card = new VcardCacheEntity();
    //    IdNoCacheEntity id = new IdNoCacheEntity();
    //    id.id = "id_0";
    //    id.no = "no_0";
    //    card.avatarId = "avatarId_0";
    //    card.IsFriend = true;
    //    card.nickName = "nickname_0";
    //    card.id = id;

    //    VcardCacheEntity card1 = new VcardCacheEntity();
    //    IdNoCacheEntity id1 = new IdNoCacheEntity();
    //    id1.id = "id_1";
    //    id1.no = "no_1";
    //    card1.avatarId = "avatarId_1";
    //    card1.nickName = "nickname_1";
    //    card1.id = id1;

    //    VcardCacheEntity card2 = new VcardCacheEntity();
    //    IdNoCacheEntity id2 = new IdNoCacheEntity();
    //    id2.id = "id_2";
    //    id2.no = "no_2";
    //    card2.avatarId = "avatarId_2";
    //    card2.nickName = "nickname_2";
    //    card2.id = id2;

    //    CacheHelper.getInstance().addVcard(card);
    //    CacheHelper.getInstance().addVcard(card1);
    //    CacheHelper.getInstance().addVcard(card2);
    //}


    //public static void Main(string[] args)
    //{
    //    //Type modelType = typeof(cn.lds.chatcore.pcw.Models.Tables.MucTable);
    //    //Type beanType = typeof(cn.lds.chatcore.pcw.Beans.MucTableBean);
    //    //createBean2ModelGetSet("model", "beanList[i]", modelType, beanType);

    //    Type modelType = typeof(cn.lds.chatcore.pcw.Models.Tables.OrganizationMemberTable);
    //    cn.lds.chatcore.pcw.Beans.MucTableBean bean = new cn.lds.chatcore.pcw.Beans.MucTableBean();
    //    //bean.no = "no";
    //    //bean.avatarId = "avatar";
    //    //bean.jobDescription = "job";
    //    //bean.id = 11;
    //    //bean.sortNum = 234;


    //    bean.AvatarStorageRecordId = "1";
    //    bean.count = 11;
    //    bean.no = "no";

    //    for (int i = 0; i < 2000; i++)
    //    {
    //        cn.lds.chatcore.pcw.Models.Tables.MucTable table = new cn.lds.chatcore.pcw.Models.Tables.MucTable();

    //        BeanUtils.copyProperties(table, bean);
    //    }
    //    Console.ReadKey();
    //}


    public static void createDaoGetSet(Type type) {
        foreach (System.Reflection.PropertyInfo info in type.GetProperties()) {
            Console.WriteLine("entity.Add(\"" + info.Name + "\", table." + info.Name + ");");

        }
    }

    public static void createBean2ModelGetSet(String modelStr,String beanStr,Type modelType,Type beanType) {
        foreach (System.Reflection.PropertyInfo info in modelType.GetProperties()) {
            System.Reflection.PropertyInfo[] pros = modelType.GetProperties();
            foreach (System.Reflection.PropertyInfo pf in beanType.GetProperties()) {
                if (pf.Name.Equals(info.Name)) {
                    Console.WriteLine(modelStr + "." + info.Name + " = " + beanStr + "." + info.Name + ";");
                }
            }
        }
    }

    //public static void Main(string[] args)
    //{
    //    //C1N76X1JYF7WA   shijun
    //    //C1N76X1JYF8GK 曲玮

    //    bool isSend = false;

    //    ImClientService imclient = ImClientService.getInstance();
    //    imclient.connectToIm();

    //    while (!imclient.isImConnected())
    //    {
    //        Thread.Sleep(100);
    //    }
    //    if (isSend)
    //    {
    //        for (int i = 0; i < 2; i++)
    //        {
    //            SendMessage msg = new SendMessage();
    //            msg.setMessage("{\"text\":\"第【" + (i + 1) + "】主设备给从设备发====\"}");
    //            String msgid = MessageUtil.generateMessageId();
    //            msg.setMessageId(msgid);
    //            msg.setFromClientId("C1N76X1JYF7WA");
    //            msg.setToClientId("C1N76X1JYF8GK");
    //            msg.setMessageType(1);
    //            imclient.sendMessage(msg);
    //            System.Console.WriteLine("消息--" + i);
    //            Thread.Sleep(10);
    //        }
    //    }



    //    //bool isSend = false;

    //    //ImConnectOptions options = new ImConnectOptions("172.17.3.113", 1883, "C1N76X1JYF8GK", "device_id", "token", "session", OsType.WEB, "osver", DeviceType.PHONE);
    //    //options.setPingInterval(1);
    //    //options.setCallback(new ImtpCallbackListener());
    //    ////options.setQrCodeType(QRCodeType.WINDOWS);

    //    //client = new ImClient(options);
    //    //client.connect();


    //    //while (!client.isConnected())
    //    //{
    //    //    Thread.Sleep(100);
    //    //}

    //    //Thread.Sleep(1000);
    //    //System.Console.WriteLine("连接状态-***********************" + client.isConnected());

    //    //if (isSend)
    //    //{
    //    //    for (int i = 0; i < 10000; i++)
    //    //    {
    //    //        SendMessage msg = new SendMessage();
    //    //        msg.setMessage("{\"text\":\"第【" + (i + 1) + "】主设备给从设备发====\"}");
    //    //        String msgid = MessageUtil.generateMessageId();
    //    //        msg.setMessageId(msgid);
    //    //        msg.setFromClientId("C1N76X1JYF7WA");
    //    //        msg.setToClientId("C1N76X1JYF8GK");
    //    //        msg.setMessageType(1);
    //    //        //msg.setToClientId("G_group1");
    //    //        client.sendMessage(msg);
    //    //        System.Console.WriteLine("消息--" + i);
    //    //        Thread.Sleep(10);
    //    //    }
    //    //}
    //}
}
}
