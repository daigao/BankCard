﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MailHelperLib
{
    public class CreateButtons
    {
        public bool CreateButton(Action<string> CreateButtonAction)
        {
            //InsertDB();
            //查询数据库
            var table=Helper.GB.DB_SQLite.Query("select YearsMonth,COUNT(*) from Tb_CardEmails group by YearsMonth having COUNT(*)>1");
            table.DefaultView.Sort = "YearsMonth DESC";
            var dt = table;
            //结果循环使用
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Thread.Sleep(50);
                CreateButtonAction?.Invoke(dt.Rows[i][0].ToString());
            }
            return true;
        }

        public int InsertDB()
        {
            var j = Helper.GB.DB_SQLite.ExecuteNonQuery("insert into Tb_CardEmails values (@Id,@YearsMonth,@ProfilePhoto,@CardName,@CardNumber,@CardSignDateTime,@CardSign);",
                Guid.NewGuid().ToString(), "2019-01", "touxiang", "广发银行", "623****325", DateTime.Now, "false");
            return j;
        }
        public bool SelectYearsMonth(string yearsMonth,Action<string> YearsMonthAction)
        {
            var table = Helper.GB.DB_SQLite.Query("select * from Tb_CardEmails where YearsMonth=@YearsMonth", yearsMonth);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                YearsMonthAction?.Invoke(table.Rows[i][4].ToString());
            }

            return true;
        }
    }
}
