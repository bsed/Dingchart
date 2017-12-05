using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Common.Enums {
enum AVMeetingBussinessType {
    // 通话完成，显示：通话时长s%
    complete,
    // 通话取消，显示：已取消
    cancel_by_myself,
    // 通话取消，显示：对方已取消
    cancel_by_other_side,
    // 通话拒绝，显示：已拒绝
    refuse_by_myself,
    // 通话拒绝，显示：对方已拒绝
    refuse_by_other_side,
    // 通话拒绝，显示：对方忙
    busy_by_other_side,
    // 通话超时，显示：对方未应答
    timeout_by_other_side,
    // 视频切语音
    AVMeetingSwitch
}
}
