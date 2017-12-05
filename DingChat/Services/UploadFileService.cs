using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.Common.ServicesHelper;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Services.core;
using EventBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cn.lds.chatcore.pcw.Services {
class UploadFileService {
    private String businessId;
    private String fileName;
    private Dictionary<String, Object> param;
    private String requestUUID = null;
    private UploadFileType uploadFileType;

    public UploadFileService(UploadFileType uploadFileType,String businessId, String fileName,Dictionary<String, Object> param) {
        this.businessId = businessId;
        this.fileName = fileName;
        this.param = param;
        this.requestUUID = Guid.NewGuid().ToString();
        this.uploadFileType = uploadFileType;
        EventBusHelper eventbus = EventBusHelper.getInstance();
        eventbus.register(this);
        if( ProgramSettingHelper.Version30==false) {
            String uploadFileName = ToolsHelper.getFileNameFromFilePath(fileName);
            FilesService.getInstance().addFileWhenUpload(DateTimeHelper.getTimeStamp().ToStr(), fileName, uploadFileType.ToStr(), 0, businessId, uploadFileName);
        }

    }

    ~UploadFileService() {
        EventBusHelper eventbus = EventBusHelper.getInstance();
        eventbus.unRegister(this);
    }

    /// <summary>
    /// API请求处理
    /// 1、上传文件第一步，获取上传路径完成，紧接着上传文件 getUploadUrl
    /// 2、”注册存储记录并执行内容标准化处理“完成 registerFile
    /// C036:扫描群成员的二维码加入群聊组
    /// </summary>
    /// <param Name="eventData"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {

        switch (eventData.eventDataType) {
        // 创建群
        case EventDataType.getUploadUrl:
            String uploadurlUuid = eventData.extras["requestUUID"].ToStr();
            if (this.requestUUID.Equals(uploadurlUuid)) {
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    getUploadUrl(eventData);
                }
                // API请求失败
                else {
                    this.pushFileUploadErrorEvent(this.param);
                }

            }
            break;
        case EventDataType.registerFile:
            // API请求成功
            String registerFileUuid = eventData.extras["requestUUID"].ToStr();
            if (this.requestUUID.Equals(registerFileUuid)) {
                if (eventData.eventType == EventType.HttpRequest) {
                    registerFile(eventData);
                }
                // API请求失败
                else {
                    this.pushFileUploadErrorEvent(this.param);
                }
            }
            break;
        default:
            break;
        }

    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param Name="owner">文件所属者（如messageId）</param>
    /// <param Name="filePath">本地文件路径</param>
    public void uploadAsync() {
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("requestUUID",this.requestUUID);
        extras.Add("filePath", this.fileName);
        //第一步：获取上传地址
        ContactsApi.getUploadUrl(extras);

    }

    /// <summary>
    /// 获取到文件上传的地址
    /// </summary>
    /// <param Name="eventData"></param>
    private void getUploadUrl(EventData<Object> eventData) {
        try {
            FileUploadEventData fileUploadBean = JsonConvert.DeserializeObject<FileUploadEventData>(eventData.data.ToStr());
            if (fileUploadBean != null) {
                // 文件上传地址
                String uploadUrl = fileUploadBean.url;
                // 本地文件地址
                String localFilePath = eventData.extras["filePath"].ToStr();
                this.startUpload(uploadUrl);

            }
        } catch (Exception e) {
            this.pushFileUploadErrorEvent(this.param);
            Log.Error(typeof(UploadFileService), e);
        }
    }

    /// <summary>
    /// 注册文件完成
    /// </summary>
    /// <param Name="eventData"></param>
    private void registerFile(EventData<Object> eventData) {
        try {
            FileUploadEventData fileUploadBean = JsonConvert.DeserializeObject<FileUploadEventData>(eventData.data.ToStr());
            if (fileUploadBean != null) {
                fileUploadBean.businessId = this.businessId;
                fileUploadBean.extras = this.param;
                fileUploadBean.uploadFileType = this.uploadFileType;

                this.pushFileUploadedEvent(fileUploadBean);
            }
        } catch (Exception e) {
            this.pushFileUploadErrorEvent(this.param);
            Log.Error(typeof(UploadFileService), e);
        }
    }

    private void startUpload(String uploadUrl) {
        CookedWebClient webClient = new CookedWebClient();

        webClient.UploadFileCompleted += new UploadFileCompletedEventHandler(uploadCompleted);
        webClient.UploadProgressChanged += new UploadProgressChangedEventHandler(uploadProgress);
        if(ProgramSettingHelper .Version30== false) {
            webClient.UploadFileAsync(new Uri(uploadUrl), this.fileName);
        } else {
            webClient.UploadFileAsync(new Uri(uploadUrl), this.fileName);
        }

    }

    void uploadCompleted(object sender, UploadFileCompletedEventArgs e) {
        if (e.Error != null) {
            this.pushFileUploadErrorEvent(this.param);
            return;
        }
        try {
            String result = System.Text.Encoding.Default.GetString(e.Result);
            JObject obj = JObject.Parse(result);
            JToken jtoken = obj.GetValue("status");
            String status = jtoken.ToString();
            if (!"success".Equals(status)) {
                // 上传失败
                this.pushFileUploadErrorEvent(this.param);
            } else {


                if (ProgramSettingHelper.Version30 == false) {
                    JObject data = JObject.Parse(obj["data"].ToString());
                    String filePath = data["filePath"].ToStr();
                    Dictionary<String, Object> json = new Dictionary<String, Object>();
                    json.Add("fileName", this.fileName);
                    json.Add("filePath", filePath);
                    json.Add("normalization", "sync");
                    //第三步：注册
                    Dictionary<String, Object> extras = new Dictionary<String, Object>();
                    extras.Add("requestUUID", this.requestUUID);
                    ContactsApi.registerFile(json, extras);
                } else {
                    var mJObj = JArray.Parse(obj["data"].ToString());
                    JObject data = (JObject) mJObj[0];
                    FileUploadEventData fileUploadBean = JsonConvert.DeserializeObject<FileUploadEventData>(data.ToStr());
                    if (fileUploadBean != null) {
                        fileUploadBean.businessId = this.businessId;
                        fileUploadBean.extras = this.param;
                        fileUploadBean.uploadFileType = this.uploadFileType;

                        String uploadFileName = ToolsHelper.getFileNameFromFilePath(fileName);
                        FilesService.getInstance().addFileWhenUpload(fileUploadBean.id, fileName, uploadFileType.ToStr(), 0, businessId, uploadFileName);

                        this.pushFileUploadedEvent(fileUploadBean);
                    }

                }
            }
        } catch (Exception ex) {
            this.pushFileUploadErrorEvent(this.param);
            Log.Error(typeof(UploadFileService), ex);

        }

    }

    void uploadProgress(object sender, UploadProgressChangedEventArgs e) {
        this.pushFileUploadingEvent(this.param,e.BytesSent, e.TotalBytesToSend);
    }

    /// <summary>
    /// 上传中的事件
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="extras"></param>
    /// <param Name="currentSize"></param>
    /// <param Name="totalSize"></param>
    private void pushFileUploadingEvent( Dictionary<String, Object> extras, long currentSize, long totalSize) {
        try {
            FileEventData fileBean = new FileEventData();
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            fileBean.currentSize = currentSize;
            fileBean.totalSize = totalSize;
            fileBean.businessId = this.businessId;
            fileBean.extras = extras;
            //Console.WriteLine("文件[" + this.businessId + "]的上传进度：" + fileBean.currentSize + "/" + fileBean.totalSize + "，" + fileBean.percent() + "%");
            businessEvent.data = fileBean;
            businessEvent.eventDataType = BusinessEventDataType.FileUploadingEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof (UploadFileService), e);
        }
    }

    /// <summary>
    /// 上传完成的事件
    /// </summary>

    private void pushFileUploadedEvent(FileUploadEventData fileUploadBean) {
        try {
            //Console.WriteLine("文件[" + this.businessId + "]的上传进度：上传完成");
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = fileUploadBean;
            businessEvent.eventDataType = BusinessEventDataType.FileUploadedEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
            // 更细文件表
            FilesService.getInstance().setFileStorageIdByOwner(fileUploadBean.id, this.businessId);
        } catch (Exception e) {
            Log.Error(typeof (UploadFileService), e);
        } finally {
            EventBusHelper eventbus = EventBusHelper.getInstance();
            eventbus.unRegister(this);
        }
    }

    /// <summary>
    /// 上传异常的事件
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="extras"></param>
    /// <param Name="currentSize"></param>
    /// <param Name="totalSize"></param>
    private void pushFileUploadErrorEvent(Dictionary<String, Object> extras) {
        try {
            //Console.WriteLine("文件[" + this.businessId + "]的上传进度：上传失败");
            FileEventData fileBean = new FileEventData();
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            fileBean.businessId = this.businessId;
            fileBean.extras = extras;
            businessEvent.data = fileBean;
            businessEvent.eventDataType = BusinessEventDataType.FileUploadErrorEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);


        } catch (Exception e) {
            Log.Error(typeof(UploadFileService), e);
        } finally {
            EventBusHelper eventbus = EventBusHelper.getInstance();
            eventbus.unRegister(this);
        }
    }

}


}
