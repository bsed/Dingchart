using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Services.core;
using FileDownloader;
namespace cn.lds.chatcore.pcw.Services {
public class DownloadFileServices {
    string fileStorageId = string.Empty;
    private String fileName = null;
    private Dictionary<String, Object> param = null;
    private IFileDownloader fileDownloader = null;
    private long currentDownloadSize = 0;
    public DownloadFileServices(string fileStorageId, String fileName,Dictionary<String, Object> param ) {
        if (string.IsNullOrEmpty(fileStorageId) ) {
            Log.Error(typeof(DownloadServices), "下载的文件id不正确，文件id：" + fileStorageId);
        }
        if (fileName == null || "".Equals(fileName)) {
            Log.Error(typeof(DownloadServices), "下载的文件路径，文件路径：" + fileName);
        }
        this.fileStorageId = fileStorageId;
        this.fileName = fileName;
        this.param = param;
        fileDownloader = new FileDownloader.FileDownloader();
        fileDownloader.DownloadFileCompleted += DownloadFileCompleted;
        fileDownloader.DownloadProgressChanged += OnDownloadProgressChanged;
    }

    public void StartAsync() {
        //需要把cookie加入
        fileDownloader.SetCookieContainer(App.CookieContainer);
        fileDownloader.DownloadFileAsync(new Uri(ServiceCode.getDownloadUrl(""+this.fileStorageId)),this.fileName );
    }

    public void CancelDownload() {
        fileDownloader.CancelDownloadAsync();
    }
    void OnDownloadProgressChanged(object sender, DownloadFileProgressChangedArgs args) {
        // Console.WriteLine("-------------------Downloaded {0} of {1} bytes", args.BytesReceived, args.TotalBytesToReceive);
        //Console.WriteLine("-------------------percent: {0}", args.ProgressPercentage);

        long process = args.ProgressPercentage % 2;

        if (process == 0) {
            this.fireDownloadProcess(args.TotalBytesToReceive, args.BytesReceived,args.ProgressPercentage);
        }



    }
    void DownloadFileCompleted(object sender, DownloadFileCompletedArgs eventArgs) {
        if (eventArgs.State == CompletedState.Succeeded) {
            try {
                WebHeaderCollection myWebHeaderCollection = eventArgs.responseHeaders;
                if (myWebHeaderCollection!=null) {
                    for (int i = 0; i < myWebHeaderCollection.Count; i++) {
                        String key = myWebHeaderCollection.GetKey(i);
                        if ("etag".Equals(key.ToLower())) {
                            String value = myWebHeaderCollection.Get(key);
                            if (value == null || "".Equals(value)) {
                                //下载失败
                                this.fireDownloadFail();
                                return;
                            }
                        }

                    }
                }
                // 获取文件后缀
                String downloadFileSuffix = ToolsHelper.getFileSuffix(this.fileName);
                String downloadFileName = "";
                if (this.param != null && this.param.ContainsKey(DownloadServices.DOWNLOADFILENAME_KEY)) {
                    downloadFileName = this.param[DownloadServices.DOWNLOADFILENAME_KEY].ToStr();
                } else {
                    downloadFileName = ToolsHelper.getFileNameFromFilePath(this.fileName);
                }

                // 获取消息ID
                String messageId = null;
                if (this.param != null && this.param.ContainsKey(DownloadServices.MESSAGEID_KEY)) {
                    messageId = this.param[DownloadServices.MESSAGEID_KEY].ToStr();
                }
                // 保存文件记录
                FilesService.getInstance().addFile(fileStorageId, this.fileName, downloadFileSuffix, eventArgs.BytesTotal, messageId, downloadFileName);
                //下载成功
                this.fireDownloadComplete();


            } catch (Exception e) {
                Log.Error(typeof(DownloadFileServices), e);
            }

            //在这里需要判断是否下载成功

        } else if (eventArgs.State == CompletedState.Failed) {
            this.fireDownloadFail();
        }
    }

    private void fireDownloadComplete() {
        try {
            //Console.WriteLine("DownloadFileServices：fireDownloadComplete");
            FileEventData fileBean = new FileEventData();
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            fileBean.fileStorageId = fileStorageId;
            if (param != null && param.ContainsKey("messageId")) {
                fileBean.businessId = param["messageId"].ToStr();
            }
            fileBean.extras = param;
            businessEvent.data = fileBean;
            businessEvent.eventDataType = BusinessEventDataType.FileDownloadedEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(DownloadServices), e);
        }
    }

    private void fireDownloadFail() {
        try {
            //Console.WriteLine("DownloadFileServices：fireDownloadFail");
            FileEventData fileBean = new FileEventData();
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            fileBean.fileStorageId = fileStorageId;
            fileBean.extras = param;
            if (param != null && param.ContainsKey("messageId")) {
                fileBean.businessId = param["messageId"].ToStr();
            }
            businessEvent.data = fileBean;
            businessEvent.eventDataType = BusinessEventDataType.FileDownloadErrorEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(DownloadFileServices), e);
        }

    }
    private void fireDownloadProcess(long totalSize,long received,int percent) {
        //  lock (this) {
        try {
            //  Console.WriteLine("DownloadFileServices：totalSize=" + totalSize + ",received=" + received);
            FileEventData fileBean = new FileEventData();
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            fileBean.fileStorageId = fileStorageId;
            fileBean.currentSize = received;
            fileBean.totalSize = totalSize;
            if (param != null && param.ContainsKey("messageId")) {
                fileBean.businessId = param["messageId"].ToStr();
            }
            fileBean.percent = percent;
            fileBean.extras = this.param;
            businessEvent.data = fileBean;
            businessEvent.eventDataType = BusinessEventDataType.FileDownloadingEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof (DownloadFileServices), e);
        }
        // }
    }

}
}
