using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace cn.lds.chatcore.AutoUpdater {
/// <summary>
/// 升级信息的具体包装
/// </summary>
[Serializable]
public class UpdateInfo {
    public string md5 {
        get;
        set;
    }
    public Int64 fileId {
        get;
        set;
    }
    public Int64 forceUpdate {
        get;
        set;
    }

    /// <summary>
    /// 应用程序版本
    /// </summary>
    public Int64 versionNo {
        get;
        set;
    }
    //对应接口的版面名称
    public string versionName {
        get;
        set;
    }




    private string _desc;
    /// <summary>
    /// 更新描述
    /// </summary>
    public string description {
        get {
            return _desc;
        } set {
            _desc = string.Join(Environment.NewLine, value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
}
