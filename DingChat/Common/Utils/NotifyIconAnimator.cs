using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace cn.lds.chatcore.pcw.Common.Utils {
public class NotifyIconAnimator {
    #region Fields

    int _currLoopCount;
    int _currLoopIndex;
    Icon[] _icons;
    int _loopCount;
    NotifyIcon _notifyIcon;
    Icon _prevIcon;
    Timer _timer;

    #endregion Fields

    #region Constructors

    public NotifyIconAnimator(NotifyIcon icon) {
        if (icon == null) {
            throw new ArgumentNullException("icon");
        }
        _notifyIcon = icon;
    }

    #endregion Constructors

    #region Methods

    //开始闪烁，icons是图标列表，interval是Timer间隔，loopCount是闪烁次数，-1代表永远循环
    public void StartAnimation(Icon[] icons, int interval, int loopCount) {
        if (icons == null) {
            throw new ArgumentNullException("icons");
        }
        if (_timer != null)
            StopAnimation();
        _prevIcon = _notifyIcon.Icon;
        _icons = icons;
        _timer = new Timer();
        _timer.Interval = interval;
        _loopCount = loopCount;
        _timer.Tick += _timer_Tick;
        _timer.Start();
    }

    public void StopAnimation() {
        _timer.Stop();
        _timer.Tick -= _timer_Tick;
        _timer.Dispose();
        foreach (var icon in _icons)
            icon.Dispose();
        _notifyIcon.Icon = _prevIcon;
        _timer = null;
    }

    void _timer_Tick(object sender, EventArgs e) {
        _currLoopCount++;
        if (_loopCount != -1 && _currLoopCount > _loopCount) {
            StopAnimation();
            return;
        }
        if (_currLoopIndex >= _icons.Length)
            _currLoopIndex = 0;
        _notifyIcon.Icon = _icons[_currLoopIndex];

        _currLoopIndex++;
    }

    #endregion Methods
}
}