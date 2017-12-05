using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace cn.lds.chatcore.capture {
public interface ICaptureCallback {
    void CaptureCallback(Bitmap bitmap);
}
}
