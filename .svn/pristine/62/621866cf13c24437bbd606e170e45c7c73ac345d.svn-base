using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MailHelperLib.Helper
{
    public class SQLiteHelper
    {
        
        private string connStr = "";

        /// <summary>
        /// 使用数据文件路径和数据库连接密码初始化SQLiteHelper,
        /// 没有数据库文件的时候将创建数据库文件
        /// </summary>
        /// <param name="filePath">数据库文件路径</param>
        /// <param name="password">数据库连接密码</param>
        public SQLiteHelper(string filePath, string password, params string[] commands)
        {
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = filePath;
            connstr.Password = password;
            this.connStr = connstr.ToString();
            if (!File.Exists(filePath))
            {
                SQLiteConnection.CreateFile(filePath);
                if (commands != null)
                {
                    foreach (var sql in commands)
                    {
                        this.ExecuteNonQuery(sql);
                    }
                }
            }
        }

        /// <summary>
        /// 执行sql命令并返回影响的行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="paras">参数数组</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string sql, params object[] paras)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    if (paras != null && paras.Length > 0)
                    {
                        MatchCollection mc = Regex.Matches(sql, "\\@\\w+");
                        for (int i = 0; i < mc.Count; i++)
                        {
                            cmd.Parameters.Add(new SQLiteParameter(mc[i].Value, paras[i]));
                        }
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        #region 事务操作封装,用户数据批量添加和批量修改
        private DbTransaction batchTransaction;
        private SQLiteConnection batchConn;
        private SQLiteCommand batchCmds;
        private string[] batchParaNames;
        private int batchResult;
        private string batchSQL;
        /// <summary>
        /// 使用sql命令开始一个批量数据操作事务
        /// 主要用于批量添加和修改
        /// </summary>
        public void BatchBegion(string sql)
        {
            this.batchResult = 0;
            this.batchConn = new SQLiteConnection(connStr);
            this.batchConn.Open();
            this.batchTransaction = batchConn.BeginTransaction();
            this.batchCmds = new SQLiteCommand(sql, this.batchConn);
            MatchCollection mc = Regex.Matches(sql, "\\@\\w+");
            this.batchParaNames = new string[mc.Count];
            for (int i = 0; i < mc.Count; i++)
            {
                this.batchParaNames[i] = mc[i].Value.Replace("@", "");
            }
            this.batchSQL = sql;
        }
        /// <summary>
        /// 在事务中执行任意命令
        /// </summary>
        public void BatchExecuteNonQuery(string sql, params object[] paras)
        {
            this.batchCmds.CommandText = sql;
            if (paras != null && paras.Length > 0)
            {
                MatchCollection mc = Regex.Matches(sql, "\\@\\w+");
                for (int i = 0; i < mc.Count; i++)
                {
                    this.batchCmds.Parameters.Add(new SQLiteParameter(mc[i].Value, paras[i]));
                }
            }
            this.batchCmds.ExecuteNonQuery();
        }
        /// <summary>
        /// 添加批量操作需要使用的参数
        /// </summary>
        public int BatchExecute(params object[] paras)
        {
            this.batchCmds.CommandText = this.batchSQL;
            if (paras != null)
            {
                for (int i = 0; i < this.batchParaNames.Length; i++)
                {
                    this.batchCmds.Parameters.Add(new SQLiteParameter("@" + this.batchParaNames[i], paras[i]));
                }
            }
            int result = this.batchCmds.ExecuteNonQuery();
            this.batchResult += result;
            return result;
        }
        public void BatchExecuteT<T>(T obj)
        {
            this.batchCmds.CommandText = this.batchSQL;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Type type = typeof(T);
            PropertyInfo[] pis = type.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                if (this.batchParaNames.Contains(pi.Name))
                {
                    dic.Add(pi.Name, pi.GetValue(obj, null));
                }
            }
            foreach (var kv in dic)
            {
                this.batchCmds.Parameters.Add(new SQLiteParameter("@" + kv.Key, kv.Value));
            }
            this.batchResult += this.batchCmds.ExecuteNonQuery();
        }
        /// <summary>
        /// 提交批量操作，并返回整个操作影响的行数
        /// </summary>
        public int BatchSubmit()
        {
            if (this.batchTransaction != null)
            {
                this.batchTransaction.Commit();
                this.batchConn.Close();
                this.batchCmds = null;
                this.batchTransaction = null;
                this.batchConn = null;
                this.batchSQL = null;
                GC.Collect();
            }
            return this.batchResult;
        }
        /// <summary>
        /// 回滚事物
        /// </summary>
        public void BatchRollback()
        {
            if (this.batchTransaction != null)
            {
                this.batchTransaction.Rollback();
                this.batchConn.Close();
                this.batchCmds = null;
                this.batchTransaction = null;
                this.batchConn = null;
                this.batchSQL = null;
                GC.Collect();
            }
        }
        #endregion

        /// <summary>
        /// 执行一个查询语句，返回一个包含查询结果的DataTable
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <param name="paras">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param>
        /// <returns>DataTable</returns>
        public DataTable Query(string sql, params object[] paras)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    if (paras != null && paras.Length > 0)
                    {
                        MatchCollection mc = Regex.Matches(sql, "\\@\\w+");
                        for (int i = 0; i < mc.Count; i++)
                        {
                            cmd.Parameters.Add(new SQLiteParameter(mc[i].Value, paras[i]));
                        }
                    }
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    DataTable data = new DataTable();
                    adapter.Fill(data);
                    return data;
                }
            }
        }

        /// <summary>
        /// 查询一个对象集合
        /// </summary>
        public List<T> QueryT<T>(string sql, params object[] paras)
        {
            DataTable table = this.Query(sql, paras);
            List<T> list = new List<T>();
            T obj = default(T);
            Type type = typeof(T);
            if (type == typeof(string))
            {
                foreach (DataRow row in table.Rows)
                {
                    list.Add((T)Convert.ChangeType(row[0], typeof(T)));
                }
            }
            else
            {
                Dictionary<string, int> columnName_Index = new Dictionary<string, int>();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    columnName_Index.Add(table.Columns[i].ColumnName, i);
                }
                PropertyInfo[] pis = type.GetProperties();
                foreach (DataRow row in table.Rows)
                {
                    obj = (T)Activator.CreateInstance(type, true);
                    foreach (PropertyInfo pi in pis)
                    {
                        Type tp = pi.PropertyType;
                        if (!columnName_Index.ContainsKey(pi.Name))
                            continue;
                        object value = row[columnName_Index[pi.Name]];
                        pi.SetValue(obj, Convert.ChangeType(value, pi.PropertyType), null);
                    }
                    list.Add(obj);
                }
            }
            return list;
        }

        /// <summary>
        /// 泛型聚合函数查询
        /// </summary>
        public T ExecuteScalarT<T>(string sql, params object[] paras)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    if (paras != null && paras.Length > 0)
                    {
                        MatchCollection mc = Regex.Matches(sql, "\\@\\w+");
                        for (int i = 0; i < mc.Count; i++)
                        {
                            cmd.Parameters.Add(new SQLiteParameter(mc[i].Value, paras[i]));
                        }
                    }
                    object obj = cmd.ExecuteScalar();
                    return (T)Convert.ChangeType(obj, typeof(T));
                }
            }
        }

        /// <summary>
        /// 添加一个对象到数据库
        /// </summary>
        public int ExecuteNonQueryT<T>(string sql, T obj)
        {
            MatchCollection mc = Regex.Matches(sql, "\\@\\w+");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            for (int i = 0; i < mc.Count; i++)
            {
                dic.Add(mc[i].Value.Replace("@", ""), null);
            }
            List<object> paras = new List<object>();
            Type type = typeof(T);
            PropertyInfo[] pis = type.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                if (dic.ContainsKey(pi.Name))
                {
                    dic[pi.Name] = pi.GetValue(obj, null);
                }
            }
            return this.ExecuteNonQuery(sql, dic.Values.ToArray());
        }
    }
}
