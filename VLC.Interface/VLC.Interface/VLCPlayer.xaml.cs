using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Declarations;
using Declarations.Players;
using Declarations.Media;
using Implementation;
using Declarations.Events;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using Vlc.DotNet.Core.Medias;

namespace VLC.Interface {
/// <summary>
/// UserControl1.xaml 的交互逻辑
/// </summary>
public partial class VLCPlayer : UserControl {


    public VLCPlayer() {
        InitializeComponent();
    }

    public string Path = string.Empty;
    IMediaPlayerFactory m_factory;
    IVideoPlayer m_player;
    IMediaFromFile m_media;
    private volatile bool m_isDrag;

    Window winFull = new Window();//全屏时用的窗体

    public void DestroyPlayer( ) { //close the window
        winFull.Close();
    }

    private void VLCPlayer_Loaded(object sender, RoutedEventArgs e) {
        m_factory = new MediaPlayerFactory(true);
        m_player = m_factory.CreatePlayer<IVideoPlayer>();
        m_videoImage.Initialize(m_player.CustomRendererEx);

        m_player.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(Events_PlayerPositionChanged);
        m_player.Events.TimeChanged += new EventHandler<MediaPlayerTimeChanged>(Events_TimeChanged);
        m_player.Events.MediaEnded += new EventHandler(Events_MediaEnded);
        m_player.Events.PlayerStopped += new EventHandler(Events_PlayerStopped);

        //init window
        winFull.AllowsTransparency = true;
        winFull.WindowStyle = WindowStyle.None;
        winFull.WindowState = WindowState.Maximized;
        winFull.MouseDoubleClick += winFull_MouseDoubleClick;



        if(Path!=string.Empty) {
            m_media = m_factory.CreateMedia<IMediaFromFile>(Path);
            m_media.Events.DurationChanged += new EventHandler<MediaDurationChange>(Events_DurationChanged);
            m_media.Events.StateChanged += new EventHandler<MediaStateChange>(Events_StateChanged);

            m_player.Open(m_media);
            m_media.Parse(false);
            m_player.Play();
        }

    }

    void Events_PlayerStopped(object sender, EventArgs e) {
        this.Dispatcher.BeginInvoke(new Action(delegate {
            m_videoImage.Clear();
            InitControls();
        }));
    }

    void Events_MediaEnded(object sender, EventArgs e) {
        this.Dispatcher.BeginInvoke(new Action(delegate {
            InitControls();
        }));
    }

    private void InitControls() {
        sldPosition.Value = 0;
        labTime.Content = "00:00:00";
        labDuration.Content = "00:00:00";
    }

    void Events_TimeChanged(object sender, MediaPlayerTimeChanged e) {
        this.Dispatcher.BeginInvoke(new Action(delegate {
            labTime.Content = TimeSpan.FromMilliseconds(e.NewTime).ToString().Substring(0, 8);
        }));
    }

    void Events_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e) {
        this.Dispatcher.BeginInvoke(new Action(delegate {
            if (!m_isDrag) {
                sldPosition.Value = (double)e.NewPosition;
            }
        }));
    }

    private void btnOpen_Click(object sender, RoutedEventArgs e) {
        OpenFileDialog ofd = new OpenFileDialog();
        if (ofd.ShowDialog() == true) {
            txbMediaName.Text = ofd.FileName;
            m_media = m_factory.CreateMedia<IMediaFromFile>(ofd.FileName);
            m_media.Events.DurationChanged += new EventHandler<MediaDurationChange>(Events_DurationChanged);
            m_media.Events.StateChanged += new EventHandler<MediaStateChange>(Events_StateChanged);

            m_player.Open(m_media);
            m_media.Parse(false);
            m_player.Play();
        }
    }

    private void btnPlay_Click(object sender, RoutedEventArgs e) {
        m_player.Play();
    }

    void Events_StateChanged(object sender, MediaStateChange e) {
        this.Dispatcher.BeginInvoke(new Action(delegate {

        }));
    }

    void Events_DurationChanged(object sender, MediaDurationChange e) {
        this.Dispatcher.BeginInvoke(new Action(delegate {
            labDuration.Content = TimeSpan.FromMilliseconds(e.NewDuration).ToString().Substring(0, 8);
        }));
    }

    private void btnPause_Click(object sender, RoutedEventArgs e) {
        m_player.Pause();
    }

    private void btnStop_Click(object sender, RoutedEventArgs e) {
        Task.Factory.StartNew(() => {
            m_player.Stop();
        });
    }

    private void btnMute_Click(object sender, RoutedEventArgs e) {
        m_player.ToggleMute();
    }

    private void sldVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
        if (m_player != null) {
            m_player.Volume = (int)e.NewValue;
        }
    }

    private void sldPosition_DragCompleted(object sender, DragCompletedEventArgs e) {
        m_player.Position = (float)sldPosition.Value;
        m_isDrag = false;
    }

    private void sldPosition_DragStarted(object sender, DragStartedEventArgs e) {
        m_isDrag = true;
    }

    private void btnFullSrceen_Click(object sender, RoutedEventArgs e) {
        FullScreen();
    }

    #region  FullScreen
    private bool isFullScreen = false;
    private void FullScreen() {
        isFullScreen = true;

        hostGrid.Children.Remove(playerGrid);
        winFull.Content = playerGrid;

        winFull.KeyDown += winFull_KeyDown;
        winFull.Show();
    }

    private void ExitFullScreen() {
        isFullScreen = false;

        winFull.KeyDown -= winFull_KeyDown;
        winFull.Content = null;
        hostGrid.Children.Add(playerGrid);
        winFull.Hide();
    }
    void winFull_KeyDown(object sender, KeyEventArgs e) {
        switch (e.Key) {
        case Key.Escape:
            ExitFullScreen();
            break;
        default:
            ExitFullScreen();
            break;
        }
    }

    public void ToggleFullScreen() {
        if (isFullScreen) {
            ExitFullScreen();
        } else {
            FullScreen();
        }
    }

    private void m_videoImage_MouseDown(object sender, MouseButtonEventArgs e) {
        FullScreen();
    }


    void winFull_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
        //if (e.ClickCount == 2)
        //{
        //    if (isFullScreen)
        //    {
        //        ExitFullScreen();
        //    }
        //    else
        //    {
        //       FullScreen();
        //    }
        //}
    }
    #endregion
}
}
