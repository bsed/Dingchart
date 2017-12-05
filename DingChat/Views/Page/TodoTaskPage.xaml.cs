using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control.Message;
using EventBus;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// TodoTaskPage.xaml 的交互逻辑
/// </summary>
public partial class TodoTaskPage : System.Windows.Controls.Page {
    private TodoTaskPage() {
        InitializeComponent();
    }
    private static TodoTaskPage instance = null;
    public static TodoTaskPage getInstance() {
        if (instance == null) {
            instance = new TodoTaskPage();
        }
        return instance;
    }

    private bool load = false;
    private List<TodoTaskTable> dtList = new List<TodoTaskTable>();
    private long lastTimestamp = 0;

    private void Page_Loaded(object sender, RoutedEventArgs e) {

        ChkNotOk.IsChecked = true;
        LoadPanel(TodoTaskStatusType.pending);
        load = true;
    }

    private TodoTaskStatusType type;

    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {
        try {
            switch (data.eventDataType) {
            //来消息
            case BusinessEventDataType.TodoTaskAvailableEvent:
                DoTodoTaskAvailableEvent(data);
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(AppMsgPage), ex);
        }

    }

    private void DoTodoTaskAvailableEvent(BusinessEvent<object> data) {
        try {

            if (App.SelectChartSessionNo == Constants.TODO_TASK_FLAG) {
                this.Dispatcher.BeginInvoke((Action) delegate() {
                    LoadPanel(type);
                    ScrollViewer.ScrollToBottom();
                });
            }
            ChatSessionTable item = new ChatSessionTable();
            item.user = Constants.TODO_TASK_FLAG;
            //更新chartsession
            BusinessEvent<object> businessEvent = new BusinessEvent<object>();
            businessEvent.data = item;
            businessEvent.eventDataType = BusinessEventDataType.ChartSessionChangeEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception ex) {
            Log.Error(typeof(AppMsgPage), ex);
        }
    }
    private void LoadPanel(TodoTaskStatusType  type) {
        StackMain.Children.Clear();
        dtList = TodoTaskService.getInstance().getTodoTaskTablesByStatusPager(type, DateTimeHelper.getTimeStamp());
        Image imageNull = new Image();
        string HeadPortrait = string.Empty;
        if (type == TodoTaskStatusType.pending) {
            HeadPortrait = @"Todo/Todo_nocontent.png";
        } else {
            HeadPortrait = @"Todo/Done_nocontent.png";
        }
        if (dtList.Count == 0) {
            ImageHelper.loadSysImage(HeadPortrait, imageNull);
            imageNull.Stretch = Stretch.None;
            imageNull.Margin = new Thickness(0, 100, 0, 0);
            imageNull.HorizontalAlignment = HorizontalAlignment.Center;
            imageNull.VerticalAlignment = VerticalAlignment.Center;
            StackMain.Children.Add(imageNull);
        }
        LoadMessage(dtList,type);

    }

    private void LoadMessage(List<TodoTaskTable> messagesList,TodoTaskStatusType  type) {
        for (int i = 0; i < messagesList.Count; i++) {
            TodoTaskTable item = messagesList[i];

            TodoTaskMessageControl control = new TodoTaskMessageControl();
            control.Type = type;
            control.TodoTable = item;
            control.HorizontalAlignment = HorizontalAlignment.Stretch;
            control.Margin = new Thickness(10, 0, 0, 20);
            StackMain.Children.Insert(0, control);


            //取最上面一条的时间
            if (i == messagesList.Count - 1) {
                //itema = item;
                lastTimestamp =long.Parse( item.createdDate);

            }

        }
        ScrollViewer.ScrollToBottom();
    }


    private void ChkNotOk_Checked(object sender, RoutedEventArgs e) {

        try {
            type = TodoTaskStatusType.pending;
            LoadPanel(TodoTaskStatusType.pending);
        } catch (Exception ex) {
            Log.Error(typeof(TodoTaskPage), ex);
        }


    }

    private void ChkOk_Checked(object sender, RoutedEventArgs e) {
        try {
            type = TodoTaskStatusType.processed;
            LoadPanel(TodoTaskStatusType.processed);
        } catch (Exception ex) {
            Log.Error(typeof(TodoTaskPage), ex);
        }

    }


    public event Action<ChatSessionType> BtnSetClick;
    //点击设置
    private void BtnSet_Click(object sender, RoutedEventArgs e) {
        if (BtnSetClick != null && e != null) {
            BtnSetClick.Invoke(ChatSessionType.TODO_TASK);
        }
    }

    private void scrollViewer_MouseWheel(object sender, MouseWheelEventArgs e) {
        //向下就return
        if (e.Delta < 0) return;
        if (load == false) return;

        //滚到最顶
        if (ScrollViewer.ScrollableHeight > 0 && ScrollViewer.VerticalOffset == 0) {
            dtList = TodoTaskService.getInstance().getTodoTaskTablesByStatusPager(type, lastTimestamp);

            LoadMessage(dtList,type);

            //scrollViewer1.ScrollToHome();
        }
    }
}
}
