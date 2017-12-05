using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using cn.lds.chatcore.pcw.Attributes;

namespace cn.lds.chatcore.pcw.Common.Enums {
/**
 * 时间戳类型
 */
public enum TimestampType {
    /// <summary>
    /// 未知
    /// </summary>
    UNKNOWN,
    /// <summary>
    /// 联系人
    /// </summary>
    CONTACT,
    /// <summary>
    /// 单聊
    /// </summary>
    CHAT,
    /// <summary>
    /// 群聊
    /// </summary>
    MUC,
    /// <summary>
    /// 公众号
    /// </summary>
    PUBLIC,
    /// <summary>
    /// 第三方应用
    /// </summary>
    PUBLICWEB,
    /// <summary>
    /// 标签
    /// </summary>
    TAG,
    /// <summary>
    /// 码表-地区
    /// </summary>
    Region,
    /// <summary>
    /// 码表-性别
    /// </summary>
    Sexuality,

    /// <summary>
    /// 组织
    /// </summary>
    Organization,
    /// <summary>
    /// 组织成员
    /// </summary>
    OrganizationMember,
    /// <summary>
    /// 职务
    /// </summary>
    post,

    location,

    /// <summary>
    /// 第三方应用类别的分组
    /// </summary>
    ThirdAppClass,
    /// <summary>
    /// 第三方应用类别的分组
    /// </summary>
    ThirdAppGroup,
    /// <summary>
    /// 基础分类
    /// </summary>
    BaseAppGroup,
    /// <summary>
    /// 待办
    /// </summary>
    TodoTask
}
}
