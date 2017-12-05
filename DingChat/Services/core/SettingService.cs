using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using java.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.Publisher;

namespace cn.lds.chatcore.pcw.Services.core {
public class SettingService {
    private static SettingService instance = null;
    public static SettingService getInstance() {
        if (instance == null) {
            instance = new SettingService();
        }
        return instance;
    }

    // 设置的缓存
    private Dictionary<String, SettingsTable> settingPool = new Dictionary<string, SettingsTable>();

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param Name="table"></param>
    private void addSettingPool(SettingsTable table) {
        try {
            if(settingPool.ContainsKey(table.no)) {
                settingPool.Remove(table.no);
            }
            settingPool.Add(table.no,table);
        } catch (Exception e) {
            Log.Error(typeof(SettingService), e);
        }
    }

    /// <summary>
    /// C010:   设置单聊置顶
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="topmost"></param>
    public void SetTopmost(String id, Boolean topmost) {
        ContactsApi.setTopmost( id,  topmost);
    }
    /// <summary>
    ///  C012: 设置单聊免打扰 enableNoDisturbFriend
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="enableNoDisturb"></param>
    public void EnableNoDisturbFriend(String id, Boolean enableNoDisturb) {
        ContactsApi.enableNoDisturbFriend(id, enableNoDisturb);
    }
    /// <summary>
    /// API请求处理
    /// C007:删除好友
    /// C010:   设置单聊置顶 setTopmost
    /// C012: 设置单聊免打扰 enableNoDisturbFriend
    /// </summary>
    /// <param Name="eventData"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        switch (eventData.eventDataType) {
        // 删除好友
        case EventDataType.deleteFriend:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                C007(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // C010:   设置单聊置顶 setTopmost
        case EventDataType.setTopmost:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                C010(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // C012: 设置单聊免打扰 enableNoDisturbFriend
        case EventDataType.enableNoDisturbFriend:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                C012(eventData);
            }
            // API请求失败
            else {

            }
            break;

        default:
            break;
        }

    }

    /// <summary>
    /// C007 删除好友后、会话中的消息也跟着删除
    /// </summary>
    /// <param Name="extras"></param>
    private void C007(EventData<Object> eventData) {
        try {
            // 获取跟踪参数
            Dictionary<String, Object> extras = eventData.extras;
            String no = extras["no"].ToStr();
            SettingDao.getInstance().deleteByNo(no);

        } catch (Exception e) {
            Log.Error(typeof(SettingService), e);
        }
    }

    /// <summary>
    /// C010:   设置单聊置顶 setTopmost
    /// </summary>
    /// <param Name="eventData"></param>
    private void C010(EventData<Object> eventData) {
        try {
            // 好友ID
            String id = eventData.extras["id"].ToString();
            VcardsTable dt=VcardsDao.getInstance().findByClientuserId(id);
            String no = dt.no;
            Boolean topmost = Boolean.Parse(eventData.extras["topmost"].ToString());
            this.setTop(no, topmost);
        } catch (Exception e) {
            Log.Error(typeof(SettingService), e);
        }
    }

    /// <summary>
    /// C012: 设置单聊免打扰 enableNoDisturbFriend
    /// </summary>
    /// <param Name="eventData"></param>
    private void C012(EventData<Object> eventData) {
        try {
            // 好友ID
            String id = eventData.extras["id"].ToString();
            VcardsTable dt=VcardsDao.getInstance().findByClientuserId(id);
            String no = dt.no;
            Boolean quiet = Boolean.Parse(eventData.extras["Quiet"].ToString());
            this.setQuiet(no, quiet);
        } catch (Exception e) {
            Log.Error(typeof(SettingService), e);
        }
    }

    /// <summary>
    /// 获取某人的设置
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public SettingsTable get(String user) {
        try {

            if (settingPool.ContainsKey(user)) {
                return settingPool[user];
            } else {
                return SettingDao.getInstance().findByNo(user);
            }
        } catch (Exception e) {
            Log.Error(typeof(SettingService), e);
        }
        return null;
    }

    /// <summary>
    /// 新的设置
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public SettingsTable createSettingsTable(String user) {
        SettingsTable table = new SettingsTable();
        table = new SettingsTable();
        table.top = false;
        table.no = user;
        table.quiet = false;
        table.backgroundurl = "";
        table.enabaledraft = true;
        return table;
    }
    /// <summary>
    /// 设置置顶
    /// </summary>
    /// <param Name="user"></param>
    /// <param Name="Top"></param>
    public void setTop(String user, Boolean top) {
        try {
            SettingsTable table = SettingDao.getInstance().findByNo(user);
            if (table==null) {
                table = this.createSettingsTable(user);
            }
            table.top = top;
            SettingDao.getInstance().save(table);

            this.addSettingPool(table);
            if (top) {
                //置顶，才出现在聊天记录中
                //添加置顶，且没有参与聊天的窗口，默认添加一条空回话
                ChatSessionService chatSessionManager = ChatSessionService.getInstance();
                ChatSessionTable chatSessionTable = ChatSessionService.getInstance().findByNo(user);
                if (chatSessionTable==null) {
                    chatSessionManager.getChatSessionUserOrMuc(user);
                }
            }
            //通知界面修改
            //更新chartsession
            BusinessEvent<object> businessEvent = new BusinessEvent<object>();
            businessEvent.data = user;
            businessEvent.eventDataType = BusinessEventDataType.ChatTopEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(SettingService), e);
        }
    }

    /// <summary>
    /// 是否置顶
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public Boolean isTop(String user) {
        SettingsTable settingsTable = this.get(user);
        if (null == settingsTable)
            return false;
        return settingsTable.top;
    }

    /// <summary>
    /// 设置 个人/群 免打扰
    /// </summary>
    /// <param Name="user"></param>
    /// <param Name="Quiet"></param>
    public void setQuiet(String user, Boolean quiet) {
        try {
            SettingsTable table = SettingDao.getInstance().findByNo(user);
            if (table == null) {
                table = this.createSettingsTable(user);
            }
            table.quiet = quiet;
            SettingDao.getInstance().save(table);
            this.addSettingPool(table);
            //通知界面修改
            //更新chartsession
            BusinessEvent<object> businessEvent = new BusinessEvent<object>();
            businessEvent.data = user;
            businessEvent.eventDataType = BusinessEventDataType.ChatDetailedChangeEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(SettingService), e);
        }
    }

    /// <summary>
    /// 个人/群是否免打扰
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public Boolean isQuiet(String user) {
        SettingsTable settingsTable = this.get(user);
        if (settingsTable==null)
            return false;
        return settingsTable.quiet;
    }

}
}
