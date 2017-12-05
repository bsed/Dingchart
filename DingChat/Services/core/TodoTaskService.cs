using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.imtp.message;

namespace cn.lds.chatcore.pcw.Services.core {
public class TodoTaskService {
    private static TodoTaskService instance = null;

    public static TodoTaskService getInstance() {
        if (instance == null) {
            instance = new TodoTaskService();
        }
        return instance;
    }
    Thread _downLoadThread = null;

    private Boolean isNotify = false;
    /// <summary>
    /// 异步拉取待办信息
    /// </summary>
    public void requestForTodoTask(String tenantNo) {
        long t = TimeServices.getInstance().GetTime(TimestampType.TodoTask, tenantNo);
        ContactsApi.getRecentTodoTasks(t, TodoTaskStatusType.pending.ToStr(), tenantNo);
        ContactsApi.getRecentTodoTasks(t, TodoTaskStatusType.processed.ToStr(), tenantNo);
    }

    /// <summary>
    /// 处理IM消息
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {

        // 获取消息类型
        Message message = messageArrivedEvent.message;
        MsgType msgType = message.getType();
        switch (msgType) {
        // 收到待办消息
        case MsgType.TodoTask:
            //  收到消息后，标识可以提示
            this.isNotify = true;
            if (App.TenantNoList.Count > 0) {
                foreach (string tenantNo in App.TenantNoList) {
                    requestForTodoTask(tenantNo);
                }
            } else {
                requestForTodoTask(string.Empty);
            }
            // 登录进去后更新待办消息、拉取到的待办才提示，避免登录拉取数据时候就提示声音
            if ( LoginServices.getInstance().IsLogin() ) {
                NotificationHelper.NewMessage(Constants.TODO_TASK_FLAG);
            }
            break;
        default:
            break;
        }
    }

    /// <summary>
    /// API请求处理
    /// F001: getRecentTodoTasks 查找近期的（最早60天前）的待办事项
    /// F002: getHistoryTodoTasks 查找待办事项历史记录
    /// F003: processTodoTasks 处理待办事项（标记已处理）
    /// </summary>
    /// <param Name="eventData"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        switch (eventData.eventDataType) {
        // F001: getRecentTodoTasks 查找近期的（最早60天前）的待办事项
        case EventDataType.getRecentTodoTasks:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                F001(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // F002: getHistoryTodoTasks 查找待办事项历史记录
        case EventDataType.getHistoryTodoTasks:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                F002(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // F003: processTodoTasks 处理待办事项（标记已处理）
        case EventDataType.processTodoTasks:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                F003(eventData);
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
    /// F001: getRecentTodoTasks 查找近期的（最早60天前）的待办事项
    /// </summary>
    /// <param Name="eventData"></param>
    private void F001(EventData<Object> eventData) {
        try {
            // 获取API请求时的自定义参数
            String tenantNo = eventData.extras["tenantNo"].ToStr();
            // 解析API请求返回的消息
            Object todoData = eventData.data;
            List<TodoTaskBean> listTodoTask = JsonConvert.DeserializeObject<List<TodoTaskBean>>(todoData.ToStr(), new convertor<TodoTaskBean>());

            Boolean isSendNotice = false;
            if (listTodoTask != null) {

                foreach (TodoTaskBean todoTaskBean in listTodoTask) {
                    // 编号
                    string id = todoTaskBean.id;
                    // 创建人
                    String createdBy = todoTaskBean.createdBy;
                    // 创建日期
                    string createdDate = todoTaskBean.createdDate;
                    // 最后编辑者
                    String lastModifiedBy = todoTaskBean.lastModifiedBy;
                    // 最后编辑时间
                    string lastModifiedDate = todoTaskBean.lastModifiedDate;
                    // NO
                    String userNo = todoTaskBean.userNo;
                    // 应用编号
                    String appId = todoTaskBean.appId;
                    // 类型
                    String type = todoTaskBean.type;
                    // 内容
                    String content = JsonConvert.SerializeObject(todoTaskBean.content);
                    // 状态
                    String status = todoTaskBean.status;
                    if (TodoTaskStatusType.pending.ToStr().Equals(status)) {
                        isSendNotice = true;
                    }
                    // 地址
                    String detailUrl = todoTaskBean.detailUrl;


                    // 构建表对象
                    TodoTaskTable todoTaskTable = new TodoTaskTable();
                    todoTaskTable.todoTaskId = id.ToStr();
                    todoTaskTable.createdBy = createdBy;
                    todoTaskTable.createdDate = createdDate.ToStr();
                    todoTaskTable.lastModifiedBy = lastModifiedBy;
                    todoTaskTable.lastModifiedDate = lastModifiedDate.ToStr();
                    todoTaskTable.userNo = userNo;
                    todoTaskTable.appId = appId;
                    todoTaskTable.type = type;
                    todoTaskTable.content = content;
                    todoTaskTable.status = status;
                    todoTaskTable.detailUrl = detailUrl;
                    todoTaskTable.appLogoId = todoTaskBean.appLogoId;
                    todoTaskTable.appName = todoTaskBean.appName;
                    todoTaskTable.tenantNo = tenantNo;
                    // 保存表
                    TodoTaskDao.getInstance().save(todoTaskTable);

                    //下头像
                    DownloadServices.getInstance().DownloadMethod(todoTaskBean.appLogoId, DownloadType.SYSTEM_APP_IMAGE, null);

                }


                // 更新时间戳
                TimeServices.getInstance().SaveTime(TimestampType.TodoTask, eventData.timestamp,string.Empty);

                // 更新回来了待办
                if (listTodoTask.Count > 0) {
                    // TODO 是否需要看情况，通知画面变更
                    // EventBus.getDefault().post(new MessageChangedEvent());

                    // 发送EVENT
                    BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
                    Businessdata.eventDataType = BusinessEventDataType.TodoTaskAvailableEvent;
                    EventBusHelper.getInstance().fireEvent(Businessdata);


                }
            }
        } catch (Exception e) {
            Log.Error(typeof(TodoTaskService), e);
        } finally {
            this.isNotify = false;
        }
    }

    /// <summary>
    /// F002: getHistoryTodoTasks 查找待办事项历史记录
    /// </summary>
    /// <param Name="eventData"></param>
    private void F002(EventData<Object> eventData) {
        try {

        } catch (Exception e) {
            Log.Error(typeof(TodoTaskService), e);
        }
    }

    /// <summary>
    /// F003: processTodoTasks 处理待办事项（标记已处理）
    /// </summary>
    /// <param Name="eventData"></param>
    private void F003(EventData<Object> eventData) {
        try {
            Dictionary<String, Object> extras = eventData.extras;
            int todoId = extras["id"].ToStr().ToInt();
            string tenantNo = extras["tenantNo"].ToStr();
            TodoTaskTable dt = getTodoTaskTable(todoId, tenantNo);

            dt.status = TodoTaskStatusType.processed.ToStr();
            TodoTaskDao.getInstance().save(dt);

            //BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
            //Businessdata.eventDataType = BusinessEventDataType.TodoTaskAvailableEvent;
            //EventBusHelper.getInstance().fireEvent(Businessdata);

        } catch (Exception e) {
            Log.Error(typeof(TodoTaskService), e);
        }
    }


    /// <summary>
    /// 统计待办数
    /// </summary>
    /// <returns></returns>
    public int countOfTodoTaskPending() {

        try {
            return TodoTaskDao.getInstance().countOfTodoTaskPending();
        } catch (Exception e) {
            Log.Error(typeof(TodoTaskService), e);
            return 0;
        }
    }

    /// <summary>
    /// 通过待办ID获取待办数据
    /// </summary>
    /// <param Name="todoTaskId"></param>
    /// <returns></returns>
    public TodoTaskTable getTodoTaskTable(int todoTaskId, string tenantNo) {
        try {
            return TodoTaskDao.getInstance().getTodoTaskTable(todoTaskId,tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(TodoTaskService), e);
            return null;
        }
    }


    /// <summary>
    /// 获得最新的todotask
    /// </summary>
    /// <param Name="todoTaskId"></param>
    /// <returns></returns>
    public TodoTaskTable GetLastPendingTodoTask() {
        List<TodoTaskTable> list = null;
        try {
            list = TodoTaskDao.getInstance().getTodoTaskTablesByStatus(TodoTaskStatusType.pending);
            if (list.Count >0) {
                return list[0];
            }
            return null;

        } catch (Exception e) {
            Log.Error(typeof(TodoTaskService), e);
            return null;
        }
    }

    /// <summary>
    /// 根据应用状态查询
    /// </summary>
    /// <param Name="status"></param>
    /// <returns></returns>
    public List<TodoTaskTable> getTodoTaskTablesByStatus(TodoTaskStatusType status) {
        List<TodoTaskTable> list = null;
        try {
            list = TodoTaskDao.getInstance().getTodoTaskTablesByStatus(status);
        } catch (Exception e) {
            Log.Error(typeof(TodoTaskService), e);
            list = new List<TodoTaskTable>();
        }
        return list;
    }

    /// <summary>
    /// 根据应用状态分页查询
    /// </summary>
    /// <param Name="status"></param>
    /// <returns></returns>
    public List<TodoTaskTable> getTodoTaskTablesByStatusPager(TodoTaskStatusType status, long lastTimestamp) {
        List<TodoTaskTable> list = null;
        try {
            list = TodoTaskDao.getInstance().getTodoTaskTablesByStatusPager(status, lastTimestamp);
        } catch (Exception e) {
            Log.Error(typeof(TodoTaskService), e);
            list = new List<TodoTaskTable>();
        }
        return list;
    }

    /// <summary>
    /// 通过todoTaskId删除
    /// </summary>
    /// <param Name="dt"></param>
    public int deleteByTodoTaskId(String todoTaskId, string tenantNo) {
        try {
            return TodoTaskDao.getInstance().deleteByTodoTaskId(todoTaskId,tenantNo);
        } catch (Exception e) {
            Log.Error(typeof(TodoTaskService), e);
            return 0;
        }
    }


}
}
