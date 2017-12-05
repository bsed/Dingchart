using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace cn.lds.chatcore.pcw.Views.Control {
public  class DingFrame:Frame {


    public DingFrame() {
        this.Navigating += FrameChat_Navigating;
        this.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
        this.JournalOwnership = System.Windows.Navigation.JournalOwnership.OwnsJournal;
    }
    private void FrameChat_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e) {
        if (Keyboard.IsKeyDown(Key.Back)) {
            this.NavigationService.StopLoading();
            return;
        }
    }
}
}
