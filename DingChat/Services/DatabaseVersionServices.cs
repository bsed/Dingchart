using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Beans;
using System.Threading;
using cn.lds.chatcore.pcw.Models.Tables;
namespace cn.lds.chatcore.pcw.Services {
public class DatabaseVersionServices {

    private static DatabaseVersionServices instance = null;
    public static DatabaseVersionServices getInstance() {
        if (instance == null) {
            instance = new DatabaseVersionServices();
        }
        return instance;
    }

    public Int64 getCurrentVersion() {
        DatabaseVersion databaseVersion = DatabaseVersionDao.getInstance().findOne();
        if (databaseVersion == null) {
            return -1;
        } else {
            return databaseVersion.databaseVersion;
        }
    }

    public void save(Int64 version) {
        DatabaseVersion table = new DatabaseVersion();
        table.databaseVersion = version;
        DatabaseVersionDao.getInstance().save(table);
    }

}
}
