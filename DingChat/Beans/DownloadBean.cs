using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Common.Enums;

namespace cn.lds.chatcore.pcw.Beans {
public class DownloadBean {
    public DownloadBean(string id, DownloadType downloadType, Dictionary<String, Object> extras) {
        this.id = id;
        this.downloadType = downloadType;
        this.extras = extras;
    }

    public string id;
    public DownloadType downloadType;
    public Dictionary<String, Object> extras;
}
}
