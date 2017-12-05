using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.DatabaseUpdate {
public class DatabaseVersion {
    /// <summary>
    /// 数据库版本，每次发布时，数据库变更，需要对该值加1
    /// </summary>
    public readonly static int DATABASE_VERSION = 9;
}
}
