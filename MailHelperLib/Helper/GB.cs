using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailHelperLib.Helper
{
    public static class GB
    {
        /// <summary>
        /// 程序数据保存根路径
        /// </summary>
        public static string DataRoot = AppDomain.CurrentDomain.BaseDirectory + "\\Databases";
        /// <summary>
        /// 回复邮件记录表sql语句
        /// id，年月，头像path，名字，号码，还时间，bool,
        /// </summary>
        public const string Tb_CardEmails =
            "create table Tb_CardEmails(" +
            "Id text primary key," +
            "YearsMonth varchar(200)," +
            "ProfilePhoto varchar(200)," +
            "CardName text," +
            "CardNumber text," +
            "CardSignDateTime datetime," +
            "CardSign bool);";

        /// <summary>
        /// 数据库操作对象
        /// </summary>
        public static SQLiteHelper DB_SQLite = new SQLiteHelper(DataRoot + "\\CardEmails.db3", "K2u%EgkD6RvE",
            Tb_CardEmails);
    }
}
