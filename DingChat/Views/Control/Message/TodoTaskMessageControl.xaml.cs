using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using Newtonsoft.Json;


namespace cn.lds.chatcore.pcw.Views.Control.Message {
/// <summary>
/// TextMessageControl.xaml 的交互逻辑
/// </summary>
public partial class TodoTaskMessageControl : MessageBase {
    public TodoTaskMessageControl() {
        InitializeComponent();

    }





    public TodoTaskTable TodoTable;

    public TodoTaskStatusType Type;



    private TodoTaskContentBean listTodoTask;

    private void UserControl_Loaded_1(object sender, RoutedEventArgs e) {

        try {

            if (Type == TodoTaskStatusType.pending) {
                LbLook.Content = "进行审批";

                LbLook.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#269dd3"));
            } else if (Type == TodoTaskStatusType.processed) {
                LbLook.Content = "查看详情";
                LbLook.Foreground = (SolidColorBrush)this.FindResource("foreground33");
            }

            listTodoTask = JsonConvert.DeserializeObject<TodoTaskContentBean>(TodoTable.content.ToStr());

            ImageHelper.loadAvatarImageBrush(TodoTable.appLogoId, Ico);

            List<KeyValueBean> list = listTodoTask.properties;
            for (int i=0; i<list.Count; i++) {
                KeyValueBean bean = list[i];
                GridText.RowDefinitions.Add(new RowDefinition());

                //添加标题
                TextBlock btnKey = new TextBlock ();
                btnKey.FontSize = 14;
                btnKey.Margin =(Thickness)this.FindResource("Margin") ;
                btnKey.Foreground = (SolidColorBrush)this.FindResource("foreground99");
                btnKey.TextTrimming = TextTrimming.CharacterEllipsis;
                btnKey.Text = bean.key.ToStr()+":";
                btnKey.SetValue(Grid.RowProperty, i);
                btnKey.SetValue(Grid.ColumnProperty, 0);
                GridText.Children.Add(btnKey);


                TextBlock btnValue = new TextBlock();
                btnValue.FontSize = 14;
                btnValue.Margin = (Thickness)this.FindResource("Margin");
                btnValue.TextTrimming = TextTrimming.CharacterEllipsis;
                btnValue.Foreground = (SolidColorBrush)this.FindResource("foreground33");
                btnValue.Text = bean.value.ToStr();
                btnValue.SetValue(Grid.RowProperty, i);
                btnValue.SetValue(Grid.ColumnProperty, 1);
                GridText.Children.Add(btnValue);
            }




            LbTitle.Content = listTodoTask.title;

            //聊天人名
            NameLb.Content = TodoTable.appName;

            //日期

            LableD.Content = DateTimeHelper.getDate(TodoTable.createdDate).ToString("yyyy-MM-dd  HH:mm:ss");

        } catch (Exception ex) {
            Log.Error(typeof(TodoTaskMessageControl), ex);
        }
    }


    /// <summary>
    /// 滑轮滚动（解决子控件滑轮不滚动问题）
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void LayoutRoot_PreviewMouseWheel_1(object sender, MouseWheelEventArgs e) {


        try {
            var scroll =
                ((this.Parent as StackPanel).Parent as ScrollViewer);
            if (scroll != null) {
                //指定父级ScrollViewer滚动偏移量
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
            }
        } catch (Exception ex) {
            Log.Error(typeof(TodoTaskMessageControl), ex);
        }
    }

    public event Action<string> ClickXq;

    private void Btnxq_Click(object sender, RoutedEventArgs e) {
        try {
            if (string.IsNullOrEmpty(listTodoTask.pcDetailUrl)) {
                //CommonMessageBox.Msg.Show("没有对应的应用信息", CommonMessageBox.MsgBtn.OK);
                NotificationHelper.ShowWarningMessage("没有对应的应用信息!");
                return;
            }
            if (listTodoTask.pcDetailUrl.Contains("/meeting/information/view.html")) {
                ContactsApi.processTodoTasks(TodoTable.todoTaskId,TodoTable.tenantNo);
            }
            listTodoTask.tenantNo = TodoTable.tenantNo;
            BusinessEvent<object> Businessdata = new BusinessEvent<object>();
            Businessdata.data = listTodoTask;
            Businessdata.eventDataType = BusinessEventDataType.ClickTodoTaskXq;
            EventBusHelper.getInstance().fireEvent(Businessdata);
        } catch (Exception ex) {
            Log.Error(typeof(TodoTaskMessageControl), ex);
        }
    }



}
}
