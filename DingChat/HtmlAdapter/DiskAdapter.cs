using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.HtmlAdapter {
[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
[System.Runtime.InteropServices.ComVisibleAttribute(true)]
public class DiskAdapter : WebAdapter {

    ///// <summary>
    ///// 发送网盘文件消息
    ///// </summary>
    ///// <param name="fileStorageId"></param>
    ///// <param name="filename"></param>
    ///// <param name="filesize"></param>
    //public void sendDiskFileMessage(long fileStorageId,String filename, long filesize) {
    //    try {
    //        String msg = "";
    //        msg += "fileStorageId="+ fileStorageId;
    //        msg += ",filename=" + filename;
    //        msg += ",filesize=" + filesize;
    //        NotificationHelper.ShowInfoMessage(msg);
    //        this.closePopWebPageWindow();
    //    } catch (Exception ex) {
    //        Log.Error(typeof(DiskAdapter), ex);
    //    }
    //}

    /// <summary>
    /// 发送网盘文件消息
    /// </summary>
    /// <param name="fileStorageId"></param>
    /// <param name="filename"></param>
    /// <param name="filesize"></param>
    public void sendDiskFileMessage(String json) {
        try {
            List<DiskFileBean> jsonArray = JsonConvert.DeserializeObject<List<DiskFileBean>>(json, new convertor<DiskFileBean>());
            //foreach (DiskFileBean diskFileBean in jsonArray) {
            //    String msg = "";
            //    msg += "fileStorageId=" + diskFileBean.fileStorageId;
            //    msg += ",filename=" + diskFileBean.filename;
            //    msg += ",filesize=" + diskFileBean.filesize;
            //    NotificationHelper.ShowInfoMessage(msg);
            //}

            SendDiskFileEventData sendDiskFileEventData = new SendDiskFileEventData();
            sendDiskFileEventData.diskFiles = jsonArray;

            BusinessEvent<object> Businessdata = new BusinessEvent<object>();
            Businessdata.data = sendDiskFileEventData;
            Businessdata.eventDataType = BusinessEventDataType.ClickDiskFile;
            EventBusHelper.getInstance().fireEvent(Businessdata);

            this.closePopWebPageWindow();
        } catch (Exception ex) {
            Log.Error(typeof(DiskAdapter), ex);
        }
    }

}
}
