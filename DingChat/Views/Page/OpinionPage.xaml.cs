using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control;
using EventBus;
using Microsoft.Win32;


namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// OpinionPage.xaml 的交互逻辑
/// </summary>
public partial class OpinionPage : System.Windows.Controls.Page {
    private OpinionPage() {
        InitializeComponent();
    }

    private static OpinionPage instance = null;
    public static OpinionPage getInstance() {
        if (instance == null) {
            instance = new OpinionPage();
        }
        return instance;
    }
    /// <summary>
    /// 业务处理通知消息
    /// </summary>
    /// <param Name="businessEvent"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> businessEvent) {
        try {
            switch (businessEvent.eventDataType) {
            // 文件上传完成
            case BusinessEventDataType.FileUploadedEvent:
                DoFileUploadedEvent(businessEvent);
                break;

            }
        } catch (Exception ex) {
            Log.Error(typeof(OpinionPage), ex);
        }

    }
    List<int> list = new List<int>();
    private int count = 0;
    private void DoFileUploadedEvent(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            FileUploadEventData item = data.data as FileUploadEventData;
            if (item == null) return;

            list.Add(item.id.ToStr().ToInt());

            Thread.Sleep(1000);
            this.Dispatcher.BeginInvoke((Action) delegate {

                if (list.Count == count) {
                    ContactsApi.feedback(App.AccountsModel.clientuserId.ToStr().ToInt(), advice.Text, tel.Text, list);
                    count = 0;
                    Clear();
                }
            });


        } catch
            (Exception
                    ex) {
            Log.Error(typeof (OpinionPage), ex);
        }

    }
    private void Clear() {
        advice.Text = string.Empty;
        tel.Text =  string.Empty;
        AddImage1.clear();
        AddImage2.clear();
        AddImage3.clear();
    }

    private void Btn_Click(object sender, RoutedEventArgs e) {

        if(advice.Text==string.Empty||tel.Text==string.Empty) {
            NotificationHelper.ShowErrorMessage("意见和联系方式不能为空！");
            return;
        }
        bool email = ValidatorHelper.IsEmail(tel.Text);

        bool telNo = ValidatorHelper.IsMobile(tel.Text);
        if(email==false && telNo == false) {
            NotificationHelper.ShowErrorMessage("请填写正确格式联系方式");
            return;
        }
        count = 0;
        list = new List<int>();
        if (AddImage1.ImagePath != string.Empty) {
            count++;
        }
        if (AddImage2.ImagePath != string.Empty) {
            count++;
        }
        if (AddImage3.ImagePath != string.Empty) {
            count++;
        }

        UploadFileService uploadFileService1 = null;
        UploadFileService uploadFileService2 = null;
        UploadFileService uploadFileService3 = null;
        if (AddImage1.ImagePath!=string.Empty) {
            // 上传图片
            uploadFileService1 = new UploadFileService(UploadFileType.MSG_IMAGE,
                    "", AddImage1.ImagePath, null);
            uploadFileService1.uploadAsync();

        }
        if (AddImage2.ImagePath != string.Empty) {
            // 上传图片
            uploadFileService2 = new UploadFileService(UploadFileType.MSG_IMAGE,
                    "", AddImage2.ImagePath, null);
            uploadFileService2.uploadAsync();

        }
        if (AddImage3.ImagePath != string.Empty) {
            // 上传图片
            uploadFileService3 = new UploadFileService(UploadFileType.MSG_IMAGE,
                    "", AddImage3.ImagePath, null);
            uploadFileService3.uploadAsync();
        }
        if(count==0) {
            ContactsApi.feedback(App.AccountsModel.clientuserId.ToStr().ToInt(), advice.Text, tel.Text, list);
            count = 0;
            Clear();
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e) {

    }

    private void Page_Unloaded(object sender, RoutedEventArgs e) {
        Clear();
    }

    /// <summary>
    /// API请求处理
    /// 意见反馈
    /// </summary>
    /// <param Name="eventData"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        switch (eventData.eventDataType) {
        // 意见反馈
        case EventDataType.feedback:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                this.Dispatcher.Invoke(new Action(() => {
                    NotificationHelper.ShowSuccessMessage("提交完成！");
                }));

            }
            // API请求失败
            else {

            }
            break;
        default:
            break;
        }

    }
}
}
