using System;

namespace cn.lds.chatcore.pcw.Common.Utils {
public class Pageable {
    /// <summary>
    /// 数据总件数
    /// </summary>
    private Int32 dataCount = 0;

    public Int32 DataCount {
        get {
            return dataCount;
        } set {
            dataCount = value;
        }
    }

    /// <summary>
    /// 开始位置
    /// </summary>
    private Int32 startPos = 0;

    public Int32 StartPos {
        get {
            return startPos;
        } set {
            startPos = value;
        }
    }

    /// <summary>
    /// 结束位置
    /// </summary>
    private Int32 endPos = 0;

    public Int32 EndPos {
        get {
            return endPos;
        } set {
            endPos = value;
        }
    }

    /// <summary>
    /// 当前页号
    /// </summary>
    private Int32 pagePos = 0;

    public Int32 PagePos {
        get {
            return pagePos;
        } set {
            pagePos = value;
        }
    }

    // 总页数
    private Int32 pageCount = 0;

    public Int32 PageCount {
        get {
            return pageCount;
        } set {
            pageCount = value;
        }
    }

    // 每页表示件数
    private Int32 singlePageCount = 10;

    public Int32 SinglePageCount {
        get {
            return singlePageCount;
        } set {
            singlePageCount = value;
        }
    }

    // 排序项目名
    private String sortName;

    public String SortName {
        get {
            return sortName;
        } set {
            sortName = value;
        }
    }

    // 是否为升序
    private Boolean isAsc = true;

    public Boolean IsAsc {
        get {
            return isAsc;
        } set {
            isAsc = value;
        }
    }

    // 是否不改页,只排序
    private Boolean isSortOnly = false;

    public Boolean IsSortOnly {
        get {
            return isSortOnly;
        } set {
            isSortOnly = value;
        }
    }
}
}
