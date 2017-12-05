using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CircularProgressBar.mvvmSupport;

namespace CircularProgressBar {
public class MainViewModel : INotifyPropertyChanged {
    //public ICommand StartCommand {
    //    get;
    //    private set;
    //}

    private int _progressValue;
    public int ProgressValue {
        get {
            return _progressValue;
        }
        set {
            _progressValue = value;
            OnPropertyChanged("ProgressValue");
            OnPropertyChanged("ProgressText");
        }
    }

    public string ProgressText {
        get {
            return string.Format("{0}", _progressValue);
        }
    }


    public MainViewModel() {
        //this.worker = new BackgroundWorker();
        //this.worker.WorkerReportsProgress = true;
        //this.worker.DoWork += this.DoWork;
        //this.worker.ProgressChanged += this.ProgressChanged;
        //StartCommand = new CommandHandler(() => {
        //    this.worker.RunWorkerAsync();
        //}, () => {
        //    return !this.worker.IsBusy;
        //});

    }

    public void SetProcessValue(int processValue) {
        this.ProgressValue = processValue;
    }



    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
    }
    #endregion
}
}
