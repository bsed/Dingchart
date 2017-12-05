using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Common.Enums;

namespace cn.lds.chatcore.pcw.Models {

/// <summary>
/// 联系人，公众号，群聊，聊天记录等查询框的结果Model
/// </summary>
public class SearchResultItem {
    public SearchResultItem() {
        // set default value
        Avatar = "pack://application:,,,/ResourceDictionary;Component/Images/Default_avatar.jpg";
    }

    /// <summary>
    /// 每个项目的ID信息（用户Id，群Id，公众号Id等）
    /// </summary>
    public string ItemNo {
        set;
        get;
    }

    public ChatSessionType Type {
        set;
        get;
    }
    /// <summary>
    /// 分组名（联系人，公众号，群聊，聊天记录等）
    /// </summary>
    public string GroupName {
        set;
        get;
    }

    /// <summary>
    /// 头像路径
    /// </summary>
    public string Avatar {
        set;
        get;
    }

    /// <summary>
    /// 显示的标题（联系人昵称，公众号名称，群名称等）
    /// </summary>
    public string Title {
        set;
        get;
    }

    /// <summary>
    /// 查询结果的匹配内容描述
    /// 如：昵称：xxxxxx
    /// </summary>
    public string MatchedDesc {
        set;
        get;
    }
}

}
