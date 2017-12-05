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
using System.Windows.Shapes;
using System.Windows.Threading;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// LoadingWait.xaml 的交互逻辑
/// </summary>
public partial class LoadingWait : UserControl {

    private readonly DispatcherTimer animationTimer;
    private System.Timers.Timer timer = new System.Timers.Timer();
    public string eventDataType;
    //private static LoadingWait instance = null;
    //public static LoadingWait getInstance() {
    //    if (instance == null) {
    //        instance = new LoadingWait();
    //    }
    //    return instance;
    //}

    public LoadingWait() {
        InitializeComponent();

        animationTimer = new DispatcherTimer(
            DispatcherPriority.ContextIdle, Dispatcher);
        animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 90);
    }


    #region Private Methods
    private void Start() {
        animationTimer.Tick += HandleAnimationTick;
        animationTimer.Start();

    }
    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
        timer.Stop();
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.eventDataType = BusinessEventDataType.LoadingWaitClose;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }
    private void Stop() {

        animationTimer.Stop();
        animationTimer.Tick -= HandleAnimationTick;
    }

    private void HandleAnimationTick(object sender, EventArgs e) {
        SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
    }

    private void HandleLoaded(object sender, RoutedEventArgs e) {
        const double offset = Math.PI;
        const double step = Math.PI * 2 / 10.0;

        SetPosition(C0, offset, 0.0, step);
        SetPosition(C1, offset, 1.0, step);
        SetPosition(C2, offset, 2.0, step);
        SetPosition(C3, offset, 3.0, step);
        SetPosition(C4, offset, 4.0, step);
        SetPosition(C5, offset, 5.0, step);
        SetPosition(C6, offset, 6.0, step);
        SetPosition(C7, offset, 7.0, step);
        SetPosition(C8, offset, 8.0, step);
    }

    private void SetPosition(Ellipse ellipse, double offset,
                             double posOffSet, double step) {
        ellipse.SetValue(Canvas.LeftProperty, 50.0
                         + Math.Sin(offset + posOffSet * step) * 50.0);

        ellipse.SetValue(Canvas.TopProperty, 50
                         + Math.Cos(offset + posOffSet * step) * 50.0);
    }

    private void HandleUnloaded(object sender, RoutedEventArgs e) {
        Stop();
    }

    private void HandleVisibleChanged(object sender,   DependencyPropertyChangedEventArgs e) {
        bool isVisible = (bool)e.NewValue;

        if (isVisible) {
            //设置timer可用
            timer.Enabled = true;
            //设置timer
            timer.Interval = 6000;

            //设置是否重复计时，如果该属性设为False,则只执行timer_Elapsed方法一次。
            timer.AutoReset = false;

            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            timer.Start();
            Start();
        }

        else {
            timer.Stop();
            Stop();
        }

    }
    #endregion
}
}
