using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Business {
public class PinyinHelper {
    /// <summary>
    /// 获得全拼
    /// </summary>
    /// <param Name="strHanzi"></param>
    /// <returns></returns>
    public static String getTotalPinyin(String strHanzi) {
        String s = null;
        try {
            s = HanBmHelper.GetPyqp(strHanzi);

        } catch (Exception e) {
            Log.Error(typeof(PinyinHelper), e);
        }
        return s;
    }

    /// <summary>
    /// 获得首字母
    /// </summary>
    /// <param Name="strHanzi"></param>
    /// <returns></returns>
    public static String getFristPinyin(String strHanzi) {
        String s = null;
        try {
            s = HanBmHelper.GetPyBm(strHanzi);

        } catch (Exception e) {
            Log.Error(typeof(PinyinHelper), e);
        }
        return s;
    }
}
}
