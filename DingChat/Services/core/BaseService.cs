using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Models;

namespace cn.lds.chatcore.pcw.Services.core {
/// <summary>
/// service 基类
/// </summary>
public class BaseService {

    // 变量定义
    public Boolean DataLoadComplete = false;

    /// <summary>
    /// 标识数据拉取完成状态(通过判断API请求结果)
    /// </summary>
    /// <param Name="data"></param>
    protected void MarkDataLoadComplete(EventData<Object> eventData) {
        try {
            if (eventData != null) {
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    // 标识数据拉取完成
                    this.DataLoadComplete = true;
                }
                // API请求失败（包含业务级别的失败和异常情况的失败）
                else {
                    foreach (RestRequestError error in eventData.errors) {
                        // 判断是否是API请求失败
                        if (TostMessage.MSG_E9999_CODE.Equals(error.errcode)) {
                            // 标识数据拉取未完成
                            this.DataLoadComplete = false;
                            return;
                        }
                    }
                    // 标识数据拉取完成
                    this.DataLoadComplete = true;
                }
            }

        } catch (Exception e) {
            Log.Error(typeof(BaseService), e);
        }

    }
}
}
