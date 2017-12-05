using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace cn.lds.chatcore.pcw.Common.Utils {
/// <summary> SQL语句帮助类
/// </summary>
public class SqlHelper {
    /// <summary> 为现有Select语句SQL添加新过滤条件。
    /// 例如：参数oldSql为select a, b, c from t where b=1 order by c;
    /// 参数filter为a=1
    /// 那么得到的结果为select a, b, c from t where b=1 and a=1 order by c
    /// </summary>
    /// <param name="oldSql">现有SQL语句(例如select a, b, c from t</param>
    /// <param name="filter">新的条件语句。例如:a=1</param>
    /// <returns>带有新条件的SQL</returns>
    public static string ProcessSql(string oldSql, string filter) {
        string buffSql = oldSql;

        MatchCollection mc = Regex.Matches(buffSql,
                                           @"(?:\((?>[^()]+|\((?<PAREN>)|\)(?<-PAREN>))*(?(PAREN)(?!))\))",
                                           RegexOptions.Singleline | RegexOptions.IgnoreCase);

        for (int i = 0; i < mc.Count; i++) {
            buffSql = buffSql.Replace(mc[i].Value, "{#" + i + "}");
        }

        const string whereRegexTemp = @"(?<=\s){0}\s[\s\S]*?(?=\sorder|group\sby\s)|{0}.*";

        //匹配是否有where条件
        Match m = Regex.Match(buffSql, string.Format(whereRegexTemp, "WHERE"),
                              RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (m.Success) {
            filter = string.Format(" AND {0} ", filter);
        } else {
            m = Regex.Match(buffSql, string.Format(whereRegexTemp, "FROM"),
                            RegexOptions.Singleline | RegexOptions.IgnoreCase);
            filter = string.Format(" WHERE {0} ", filter);
        }

        string newSql = string.Empty;

        if (m.Success) {
            newSql = buffSql.Insert(m.Index + m.Length, filter);
        }

        for (int i = 0; i < mc.Count; i++) {
            newSql = newSql.Replace("{#" + i + "}", mc[i].Value);
        }

        return newSql;
    }

    /// <summary> 获取select语句中的from的表名，仅支持单表。
    /// </summary>
    /// <returns>表名</returns>
    public static string GetTableNameFromSql(string sql) {
        string buffSql = sql;

        MatchCollection mc = Regex.Matches(buffSql,
                                           @"(?:\((?>[^()]+|\((?<PAREN>)|\)(?<-PAREN>))*(?(PAREN)(?!))\))",
                                           RegexOptions.Singleline | RegexOptions.IgnoreCase);

        const string whereRegexTemp = @"(?<=\s){0}\s[\s\S]*?(?=\swhere|order|group\sby\s)|{0}.*";


        for (int i = 0; i < mc.Count; i++) {
            buffSql = buffSql.Replace(mc[i].Value, "{#" + i + "}");
        }

        var m = Regex.Match(buffSql, String.Format(whereRegexTemp, "FROM"),
                            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        string tableName = String.Empty;

        if (m.Success) {
            tableName = m.Value.Substring(5);
        }

        return tableName;

    }

    /// <summary> 返回SQL的永假SQL(向现有查询中添加1=2的过滤)
    /// </summary>
    /// <param name="oldSql">现有SQL语句</param>
    /// <returns>带有新条件的SQL</returns>
    public static string FakeSql(string oldSql) {
        string newSql = ProcessSql(oldSql, "1<>1");

        return newSql;
    }


    /// <summary>生成SQL语句的IN子句</summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string GetInClausePart(List<string> list) {
        return string.Format("'{0}'", string.Join("','", list.ToArray()));
    }

    /// <summary>生成SQL语句的IN子句</summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string GetInClausePart(List<int> list) {
        List<string> listStr = new List<string>();
        foreach (int i in list) {
            listStr.Add(i.ToStr());
        }

        return string.Format("{0}", string.Join(",", listStr.ToArray()));
    }

    /// <summary>生成SQL语句的IN子句</summary>
    /// <param name="list"></param>
    /// <param name="isStrOrChar">是否需要引号引起来的字符类型</param>
    /// <returns></returns>
    public static string GetInClausePart(List<object> list, bool isStrOrChar) {
        List<string> listStr = new List<string>();

        foreach (object i in list) {
            listStr.Add(i.ToStr());
        }

        if (isStrOrChar) {
            return string.Format("'{0}'", string.Join("','", listStr.ToArray()));
        }

        return string.Format("{0}", string.Join(",", listStr.ToArray()));

    }

    /// <summary>生成SQL语句的IN子句</summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string GetInClausePart(string[] list) {
        return string.Format("'{0}'", string.Join("','", list));
    }

    /// <summary>生成SQL语句的IN子句（本方法返回的语句可以绕过Oracle in 子句1000项限制）</summary>
    /// <param name="field">要拼接的字段名(有别名请带别名)</param>
    /// <param name="list">由于ORACLE SQL语句长度限制，列表项数量最好不要大于1万</param>
    /// <returns></returns>
    public static string GetInClause(string field, List<string> list) {
        if (list.Count == 0) {
            return "(1<>1)";
        }

        string fsql = field + " IN ('";

        StringBuilder sb = new StringBuilder();

        sb.Append(fsql);

        int j = 0;
        for (int i = 0; i < list.Count; i++) {
            j++;

            //最后一个
            if (i == list.Count-1) {
                sb.Append(list[i]);
                sb.Append("')");
                continue;
            }

            if (j == 1000) {
                j = 0;
                sb.Append(list[i]);
                sb.Append("') OR ");
                sb.Append(fsql);

            } else {
                sb.Append(list[i]);
                sb.Append("','");
            }

        }

        string sql= string.Format("({0})", sb.ToString());

        return sql;
    }
}
}