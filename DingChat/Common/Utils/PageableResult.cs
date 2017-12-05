using System.Collections.Generic;

namespace cn.lds.chatcore.pcw.Common.Utils {
public class PageableResult<T> {
    /// <summary>
    /// 分页+排序
    /// </summary>
    private Pageable pageable;

    public Pageable Pageable {
        get {
            return pageable;
        } set {
            pageable = value;
        }
    }

    // 数据列表
    private List<T> dataList = new List<T>();

    public List<T> DataList {
        get {
            return dataList;
        } set {
            dataList = value;
        }
    }


}
}
