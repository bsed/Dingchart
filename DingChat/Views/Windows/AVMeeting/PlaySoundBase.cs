using System;
using System.Timers;
using System.Windows.Media;
using cn.lds.chatcore.pcw.Common.Enums;

namespace cn.lds.chatcore.pcw.Views.Windows.AVMeeting {
public class PlaySoundBase {
    private MediaPlayer waitSoundPlayer = null;
    private Timer timerWaitSoundPlayer;
    private AVMeetingType meetingType;
    public PlaySoundBase(AVMeetingType type = AVMeetingType.audio) {
        meetingType = type;
    }
    public void PlayWaitSound() {
        waitSoundPlayer = new MediaPlayer();
        this.timerWaitSoundPlayer = new Timer();
        if (meetingType.Equals(AVMeetingType.video)) {
            waitSoundPlayer.Open(new Uri(@"SysConfig\Sound\bin119.wav", UriKind.Relative));
            timerWaitSoundPlayer.Interval = 2500;
        } else {
            waitSoundPlayer.Open(new Uri(@"SysConfig\Sound\bin107.wav", UriKind.Relative));
            timerWaitSoundPlayer.Interval = 2500;
        }
        timerWaitSoundPlayer.Elapsed += delegate (object o, ElapsedEventArgs args) {
            waitSoundPlayer.Dispatcher.BeginInvoke(new Action(() => {
                waitSoundPlayer.Position = new TimeSpan(0, 0, 0);
            }));

        };
        timerWaitSoundPlayer.Start();
        waitSoundPlayer.Play();
    }

    public void PlayCancelSound() {
        MediaPlayer playSound = new MediaPlayer();
        playSound.Open(new Uri(@"SysConfig\Sound\bin130.wav", UriKind.Relative));
        playSound.Play();
    }

    public void StopWaitSound() {
        if (timerWaitSoundPlayer != null) timerWaitSoundPlayer.Stop();
            if (waitSoundPlayer != null)
                waitSoundPlayer.Dispatcher.BeginInvoke(new Action(() => {
                    waitSoundPlayer.Stop();
            }));
 
    }

}
}
