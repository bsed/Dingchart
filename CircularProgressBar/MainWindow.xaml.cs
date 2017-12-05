using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace CircularProgressBar {
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private readonly BackgroundWorker worker;
    private MainViewModel dataModel = null;
    public MainWindow() {
        InitializeComponent();
        //因为使用了mvvm，因此需要做设定--start----
        dataModel = new MainViewModel();
        this.DataContext = dataModel;
        //因为使用了mvvm，因此需要做设定--end----

        this.worker = new BackgroundWorker();
        this.worker.WorkerReportsProgress = true;
        this.worker.DoWork += this.DoWork;
        this.worker.ProgressChanged += this.ProgressChanged;
    }

    private void DoWork(object sender, DoWorkEventArgs e) {
        for (int i = 0; i <= 100; i++) {
            Thread.Sleep(100);//Do your Work Here instead of sleeping
            worker.ReportProgress(i);
        }
    }

    private void ProgressChanged(object sender, ProgressChangedEventArgs e) {
        //this.ProgressValue = e.ProgressPercentage;
        //更新进度条
        this.dataModel.SetProcessValue(e.ProgressPercentage);
    }

    private void BtnStart_OnClick(object sender, RoutedEventArgs e) {
        this.worker.RunWorkerAsync();
    }
}
}
