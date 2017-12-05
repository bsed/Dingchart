using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Beans;
using System.Threading;
using cn.lds.chatcore.pcw.Common.MediaHelper;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Extensions;
using cn.lds.chatcore.pcw.Models.Tables;

namespace cn.lds.chatcore.pcw.Services {

public class DownloadParam {
    public  string id;
    public DownloadType downloadType;
    public Dictionary<String, Object> extras;

}

public class DownloadServices {

    private static DownloadServices instance = null;
    public static DownloadServices getInstance() {
        if (instance == null) {
            instance = new DownloadServices();
        }
        //ThreadPool.SetMaxThreads(1, 2000);
        return instance;
    }

    //下载用的关键字KEY
    public static String DOWNLOADFILENAME_KEY = "downloadFileName";
    public static String MESSAGEID_KEY  = "messageId";

    // 下载缓存，目前主要用于用户初次登录时下载
    private Dictionary<String, DownloadBean> _DownloadCatch = new Dictionary<String, DownloadBean>();

    public void DownloadCatchAdd(DownloadBean downloadBean) {
        try {
            if (LoginServices.getInstance().IsLogin()) {
                this.DownloadMethod(downloadBean.id, downloadBean.downloadType, downloadBean.extras);
                return;
            }
            // 如果缓存中不纯在，则添加
            if (!_DownloadCatch.ContainsKey(downloadBean.id.ToStr())) {
                _DownloadCatch.Add(downloadBean.id.ToStr(), downloadBean);
            }

        } catch (Exception e) {
            Log.Error(typeof(DownloadServices), e);
        }
    }

    /// <summary>
    /// 执行缓存的下载
    /// </summary>
    public void DownloadCatchExcute() {
        try {
            foreach (KeyValuePair<String, DownloadBean> kvp in _DownloadCatch) {
                DownloadBean downloadBean = kvp.Value;
                this.DownloadMethod(downloadBean.id, downloadBean.downloadType, downloadBean.extras);
            }

        } catch (Exception e) {
            Log.Error(typeof(DownloadServices), e);
        }
    }



    /// <summary>
    /// 执行普通下载
    /// </summary>
    public bool DownloadMethod(string id, DownloadType downloadType, Dictionary<String, Object> extras) {

        try {
            // 忽略无意义的下载
            if (string.IsNullOrEmpty(id)|| "0".Equals(id.ToStr())) {
                return false;
            }
            Thread t = new Thread(new ThreadStart(() => {
                try {
                    string fileStorageId = id.ToStr();
                    // 如果本地存在文件，则不执行下载
                    if (FilesService.getInstance().existFile(fileStorageId)) {
                        FilesTable filesTable = FilesService.getInstance().getFile(fileStorageId);
                        // 下载完成事件
                        this.pushFileDownloadedEvent(fileStorageId, extras, filesTable.size, DownloadBusiness.COMMON);
                        return;
                    }

                    // 语音或视频
                    if (downloadType == DownloadType.MSG_FILE ||
                            downloadType == DownloadType.MSG_VIDEO) {
                        String downloadFileName = extras[DownloadServices.DOWNLOADFILENAME_KEY].ToStr();
                        String downloadFileSuffix = ToolsHelper.getFileSuffix(downloadFileName);
                        //int pointIndex = downloadFileName.LastIndexOf(".");
                        //if (pointIndex >= 0) {
                        //    downloadFileSuffix = downloadFileName.Substring(downloadFileName.LastIndexOf("."));
                        //}
                        FileType fileType = ToolsHelper.getFileTypeBySuffix(downloadFileSuffix);
                        String forderPath = App.CacheRootPath + ToolsHelper.getForderNameByDownloadType(downloadType) + "/" + fileType.ToStr();
                        //如果不存在就创建file文件夹
                        if (Directory.Exists(forderPath) == false) {
                            Directory.CreateDirectory(forderPath);
                        }
                        String filePath = forderPath + "/" + id + downloadFileSuffix;
                        DownloadFileServices downloadFileServices = new DownloadFileServices(id.ToStr(), filePath, extras);
                        downloadFileServices.StartAsync();
                    } else {
                        this.doDownload(id, downloadType, extras);
                    }
                } catch (Exception e) {
                    Log.Error(typeof(DownloadServices), e);
                }
            }));
            t.IsBackground = true;
            t.Start();
        } catch (Exception e) {
            Log.Error(typeof(DownloadServices), e);
        }
        //DownloadParam param = new DownloadParam();
        //param.id = id;
        //param.downloadType = downloadType;
        //param.extras = extras;
        //ThreadPool.QueueUserWorkItem(doDownload, param);
        //this.doDownload(id,downloadType,extras);


        return true;
    }


    private static Object  locker = new Object();

    private void doDownload(string id, DownloadType downloadType, Dictionary<String, Object> extras) {

        lock (locker) {

            string fileStorageId = string.Empty;
            try {

                if (string.IsNullOrEmpty(id) || id.Equals("")) {
                    return ;
                }
                fileStorageId = id.ToStr();

                Console.WriteLine("执行下载，文件编号为：" + id);

                string strImageUrl = "";
                if (downloadType == DownloadType.MSG_VOICE
                        || downloadType == DownloadType.MSG_VIDEO
                        || downloadType == DownloadType.MSG_FILE) {
                    strImageUrl = ServiceCode.getDownloadUrl(id.ToStr());
                } else if (downloadType == DownloadType.SYSTEM_IMAGE) {
                    // 头像类的下载缩略图
                    strImageUrl = ServiceCode.getDownloadUrl(id.ToStr()) + "?type=thumbnail";
                } else {
                    strImageUrl = ServiceCode.getDownloadUrl(id.ToStr()) + "?type=original";
                }
                if (strImageUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, certificate, chain, sslPolicyErrors) => true;
                }
                RestClient client = new RestClient(strImageUrl);
                RestRequest request = new RestRequest("", Method.GET);

                IRestResponse restResponse = client.Execute(request);

                try {
                    if (restResponse.ErrorException != null) {
                        // 下载失败事件
                        this.pushFileDownloadErrorEvent(fileStorageId, extras, DownloadBusiness.COMMON);
                        return;
                    }

                    if (restResponse.StatusCode.Equals(HttpStatusCode.OK)) {
                        String downloadFileName = "";
                        long fileSize = 0;
                        foreach (Parameter parameter in restResponse.Headers) {
                            if ("etag".Equals(parameter.Name.ToLower())) {
                                downloadFileName = parameter.Value.ToStr();
                            }
                            if ("content-length".Equals(parameter.Name.ToLower())) {
                                fileSize = long.Parse(parameter.Value.ToStr());
                            }
                        }
                        // 如果存在文件信息，则进行保存处理
                        if (!"".Equals(downloadFileName)) {
                            long blockSize = fileSize / 100;
                            long downLoadStep = 0;
                            String downloadFileSuffix = ToolsHelper.getFileSuffix(downloadFileName);
                            //int pointIndex = downloadFileName.LastIndexOf(".");
                            //if (pointIndex >= 0) {
                            //    downloadFileSuffix = downloadFileName.Substring(downloadFileName.LastIndexOf("."));
                            //}
                            FileType fileType = ToolsHelper.getFileTypeBySuffix(downloadFileSuffix);
                            if (fileType == FileType.IMAGES) {
                                downloadFileSuffix = ".jpg";
                            }
                            String forderPath = App.CacheRootPath + ToolsHelper.getForderNameByDownloadType(downloadType) + "/" + fileType.ToStr();
                            //如果不存在就创建file文件夹
                            if (Directory.Exists(forderPath) == false) {
                                Directory.CreateDirectory(forderPath);
                            }
                            String filePath = forderPath + "/" + fileStorageId + downloadFileSuffix;
                            System.IO.File.WriteAllBytes(filePath, restResponse.RawBytes);

                            // 获取消息ID
                            String messageId = null;
                            if (extras != null && extras.ContainsKey(DownloadServices.MESSAGEID_KEY)) {
                                messageId = extras[DownloadServices.MESSAGEID_KEY].ToStr();
                            }

                            // 保存文件记录
                            FilesService.getInstance().addFile(fileStorageId, filePath, downloadFileSuffix, fileSize, messageId, downloadFileName);
                            //// TODO:这里耦合业务了啊，如果是下载语音，则直接调用转换接口
                            //if (downloadType == DownloadType.TEMP && fileType == FileType.VOICE) {
                            //    VoiceHelper.CoverArmToWmv(fileStorageId);
                            //}
                            // 下载完成事件
                            this.pushFileDownloadedEvent(fileStorageId, extras, fileSize, DownloadBusiness.COMMON);
                        }

                    } else {
                    }

                } catch (Exception e) {
                    Log.Error(typeof (DownloadServices), e);
                    // 下载失败事件
                }
            } catch (Exception e) {
                Log.Error(typeof (DownloadServices), e);
                // 下载失败事件
                this.pushFileDownloadErrorEvent(fileStorageId, extras, DownloadBusiness.COMMON);
                return ;
            }
        }
        return ;
    }

    /// <summary>
    /// 执行二维码下载
    /// </summary>
    public bool DownloadQrcodeMethod(string id, DownloadType downloadType, Dictionary<String, Object> extras) {
        try {
            Thread t = new Thread(new ThreadStart(() => {
                try {

                    this.doDownloadBarcode(id, downloadType, extras);
                } catch (Exception e) {
                    Log.Error(typeof(DownloadServices), e);
                }
            }));
            t.IsBackground = true;
            t.Start();
        } catch (Exception e) {
            Log.Error(typeof(DownloadServices), e);
        }
        return true;
    }
    private void doDownloadBarcode(string id, DownloadType downloadType,Dictionary<String, Object> extras) {

        lock (locker) {

            string fileStorageId = string.Empty;
            try {

                if (string.IsNullOrEmpty(id) || id.Equals("")) {
                    return;
                }
                fileStorageId = id.ToStr();
                // 如果本地存在文件，则不执行下载
                if (FilesService.getInstance().existFile(fileStorageId)) {
                    // 下载完成事件
                    this.pushFileDownloadedEvent(fileStorageId, extras, 0, DownloadBusiness.BARCODE);
                    return;
                }
                Console.WriteLine("执行二维码下载，文件编号为：" + id);

                String downloadQRcodeUrl = ServiceCode.downloadQRcode;
                downloadQRcodeUrl = downloadQRcodeUrl.Replace("{id}", id.ToStr());
                if (downloadQRcodeUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, certificate, chain, sslPolicyErrors) => true;
                }
                RestClient client = new RestClient(downloadQRcodeUrl);
                if (App.CookieContainer != null) {
                    client.CookieContainer = App.CookieContainer;
                }
                RestRequest request = new RestRequest("", Method.GET);

                IRestResponse restResponse = client.Execute(request);

                try {
                    if (restResponse.ErrorException != null) {
                        // 下载失败事件
                        this.pushFileDownloadErrorEvent(fileStorageId, extras, DownloadBusiness.BARCODE);
                        return;
                    }

                    if (restResponse.StatusCode.Equals(HttpStatusCode.OK)) {
                        String downloadFileName = id+".jpg";

                        // 如果存在文件信息，则进行保存处理
                        if (!"".Equals(downloadFileName)) {
                            String downloadFileSuffix = ToolsHelper.getFileSuffix(downloadFileName);
                            //    int pointIndex = downloadFileName.LastIndexOf(".");
                            //if (pointIndex >= 0) {
                            //    downloadFileSuffix = downloadFileName.Substring(downloadFileName.LastIndexOf("."));
                            //}
                            FileType fileType = ToolsHelper.getFileTypeBySuffix(downloadFileSuffix);
                            if (fileType == FileType.IMAGES) {
                                downloadFileSuffix = ".jpg";
                            }
                            String forderPath = App.CacheRootPath + ToolsHelper.getForderNameByDownloadType(downloadType) + "/" + fileType.ToStr();
                            //如果不存在就创建file文件夹
                            if (Directory.Exists(forderPath) == false) {
                                Directory.CreateDirectory(forderPath);
                            }
                            String filePath = forderPath + "/" + fileStorageId + downloadFileSuffix;
                            System.IO.File.WriteAllBytes(filePath, restResponse.RawBytes);
                            // 获取消息ID
                            String messageId = null;
                            if (extras != null && extras.ContainsKey(DownloadServices.MESSAGEID_KEY)) {
                                messageId = extras[DownloadServices.MESSAGEID_KEY].ToStr();
                            }
                            // 保存文件记录
                            FilesService.getInstance().addFile(fileStorageId, filePath, downloadFileSuffix, 0, messageId, downloadFileName);
                            // 下载完成事件
                            this.pushFileDownloadedEvent(fileStorageId, extras, 0, DownloadBusiness.BARCODE);
                        }

                    } else {
                    }

                } catch (Exception e) {
                    Log.Error(typeof(DownloadServices), e);
                    // 下载失败事件
                }
            } catch (Exception e) {
                Log.Error(typeof(DownloadServices), e);
                // 下载失败事件
                this.pushFileDownloadErrorEvent(fileStorageId, extras, DownloadBusiness.BARCODE);
                return;
            }
        }
        return;
    }
    /// <summary>
    /// 下载中的事件
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="extras"></param>
    /// <param Name="currentSize"></param>
    /// <param Name="totalSize"></param>
    private void pushFileDownloadingEvent(string fileStorageId, Dictionary<String, Object> extras, long currentSize, long totalSize, DownloadBusiness downloadBusiness) {
        try {
            FileEventData fileBean = new FileEventData();
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            fileBean.fileStorageId = fileStorageId;
            fileBean.currentSize = currentSize;
            fileBean.totalSize = totalSize;
            if (extras != null && extras.ContainsKey("messageId")) {
                fileBean.businessId = extras["messageId"].ToStr();
            }
            fileBean.extras = extras;
            //Console.WriteLine("文件[" + fileBean.id+ "]的下载进度：" + fileBean.currentSize + "/" + fileBean.totalSize + "，" + fileBean.percent() + "%");
            businessEvent.data = fileBean;
            if (downloadBusiness == DownloadBusiness.COMMON) {
                businessEvent.eventDataType = BusinessEventDataType.FileDownloadingEvent;
            } else {
                businessEvent.eventDataType = BusinessEventDataType.BarCode2DDownloadingEvent;
            }

            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(DownloadServices), e);
        }
    }

    /// <summary>
    /// 下载完成的事件
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="extras"></param>
    /// <param Name="currentSize"></param>
    /// <param Name="totalSize"></param>
    private void pushFileDownloadedEvent(string fileStorageId, Dictionary<String, Object> extras, long totalSize, DownloadBusiness downloadBusiness) {
        try {
            FileEventData fileBean = new FileEventData();
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            fileBean.fileStorageId = fileStorageId;
            fileBean.currentSize = totalSize;
            fileBean.totalSize = totalSize;
            fileBean.extras = extras;
            if (extras!=null && extras.ContainsKey("messageId")) {
                fileBean.businessId = extras["messageId"].ToStr();
            }
            businessEvent.data = fileBean;
            if (downloadBusiness == DownloadBusiness.COMMON) {
                businessEvent.eventDataType = BusinessEventDataType.FileDownloadedEvent;
            } else {
                businessEvent.eventDataType = BusinessEventDataType.BarCode2DDownloadedEvent;
            }
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(DownloadServices), e);
        }
    }

    /// <summary>
    /// 下载异常的事件
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="extras"></param>
    /// <param Name="currentSize"></param>
    /// <param Name="totalSize"></param>
    private void pushFileDownloadErrorEvent(string fileStorageId, Dictionary<String, Object> extras, DownloadBusiness downloadBusiness) {
        try {
            FileEventData fileBean = new FileEventData();
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            fileBean.fileStorageId = fileStorageId;
            //fileBean.currentSize = currentSize;
            //fileBean.totalSize = totalSize;
            fileBean.extras = extras;
            if (extras != null && extras.ContainsKey("messageId")) {
                fileBean.businessId = extras["messageId"].ToStr();
            }
            businessEvent.data = fileBean;
            if (downloadBusiness == DownloadBusiness.COMMON) {
                businessEvent.eventDataType = BusinessEventDataType.FileDownloadErrorEvent;
            } else {
                businessEvent.eventDataType = BusinessEventDataType.BarCode2DDownloadErrorEvent;
            }
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(DownloadServices), e);
        }
    }


}
}
