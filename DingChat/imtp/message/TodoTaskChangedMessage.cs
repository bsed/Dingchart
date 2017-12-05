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
using java.util;
using cn.lds.im.sdk.message.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.imtp.message {
public class TodoTaskChangedMessage : Message {
    /** 待办编号 */
    public int todoTaskId;
    /**  创建时间 */
    public long createdTime;
    /** 待办类型*/
    public String todoTaskType;

    public int getTodoTaskId() {
        return todoTaskId;
    }

    public void setTodoTaskId(int todoTaskId) {
        this.todoTaskId = todoTaskId;
    }

    public long getCreatedTime() {
        return createdTime;
    }

    public void setCreatedTime(long createdTime) {
        this.createdTime = createdTime;
    }

    public String getTodoTaskType() {
        return todoTaskType;
    }

    public void setTodoTaskType(String todoTaskType) {
        this.todoTaskType = todoTaskType;
    }

    /**
     * 返回消息类型
     * @return
     */

    public override MsgType getType() {
        return MsgType.TodoTask;
    }

    /**
     * 填充消息属性
     * @param type
     * @param sendMessage
     */
    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            JObject json = JObject.Parse((String)sendMessage.getMessage());
            base.parse(type, sendMessage);
            /** 待办编号 */
            if (json.Property("todoTaskId") != null) {
                this.setTodoTaskId(json.GetValue("todoTaskId").ToObject<int>());
            }

            /**  创建时间 */
            if (json.Property("createdTime") != null) {
                this.setCreatedTime(json.GetValue("createdTime").ToObject<long>());
            }

            /** 待办类型*/
            this.setTodoTaskType(json.GetValue("todoTaskType").ToStr());
        } catch (Exception e) {
            Log.Error(typeof(TodoTaskChangedMessage), e);
            this.setParseError(true);
        }
    }

    /**
     * 构建JSON串（通知类消息、不用保存到数据库，所以未实现方法体）
     * @return
     */
    public override String createContentJsonStr() {
        return null;
    }
}
}
