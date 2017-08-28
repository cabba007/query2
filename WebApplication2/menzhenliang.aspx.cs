using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class menzhenliang : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["id"] == null) return;
            if (!auth.isAuthorized(Session["id"].ToString(), "menzhenliang")) return;
            if (Request.QueryString["type"] == "1")
            {
                string date = Request.QueryString["date"];

                OracleConnection conn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.10.4.3)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME = orcl)));User ID=system;Password=manager");
                OracleCommand myCmd = new OracleCommand(null, conn);
                if (Request.QueryString["dept"] == "全部科室")
                    myCmd.CommandText = @"select b.hour_dict shijian, nvl(count(a.visit_date),0) jiuzhenrenci from clinic_master a right join (select level-1 hour_dict from dual connect by level <=24) b on to_char(a.registering_date,'yyyy-mm-dd') = '" + date + "' and to_char(a.registering_date,'hh24') = b.hour_dict group by b.hour_dict order by b.hour_dict";
                else
                    myCmd.CommandText = @"select b.hour_dict shijian, nvl(count(a.visit_date),0) jiuzhenrenci from (clinic_master a left join dept_dict aa on a.visit_dept = aa.dept_code) right join (select level-1 hour_dict from dual connect by level <=24) b on to_char(a.registering_date,'yyyy-mm-dd') = '" + date + "' and to_char(a.registering_date,'hh24') = b.hour_dict and aa.dept_name = '" + Request.QueryString["dept"] + "' group by b.hour_dict order by b.hour_dict";

                OracleDataAdapter da = new OracleDataAdapter();
                da.SelectCommand = myCmd;
                DataSet dataSet11 = new DataSet();
                da.Fill(dataSet11);
                string JsonString = string.Empty;
                JsonString = JsonConvert.SerializeObject(dataSet11.Tables[0]);
                Response.Write(JsonString);
                //Response.End();
                Response.Flush();
                Response.SuppressContent = true;
                Context.ApplicationInstance.CompleteRequest(); 
            }
            else if (Request.QueryString["type"] == "2")
            {
                string d1 = Request.QueryString["date1"];
                string d2 = Request.QueryString["date2"];
                DateTime time;
                if (DateTime.TryParse(d1 + " 00:00:00", out time))
                {
                    if ((time > DateTime.Now.AddDays(1)) || (time < DateTime.Now.AddYears(-3)))
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                if (DateTime.TryParse(d2 + " 00:00:00", out time))
                {
                    if ((time > DateTime.Now.AddDays(1)) || (time < DateTime.Now.AddYears(-3)))
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
                OracleConnection conn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.10.4.3)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME = orcl)));User ID=system;Password=manager");
                conn.Open();
                OracleCommand myCmd = new OracleCommand(null, conn);
                myCmd.CommandText = "select count(*) from CLINIC_master where clinic_type in" + System.Configuration.ConfigurationManager.AppSettings["zhuanjia"] + "and returned_date is null and VISIT_DATE >= to_date('" + d1 + " 00:00:00', 'yyyy-mm-dd hh24:mi:ss') and VISIT_DATE <= to_date('" + d2 + " 23:59:59', 'yyyy-mm-dd hh24:mi:ss')";
                string s1 = myCmd.ExecuteScalar().ToString();
                myCmd.CommandText = "select count(*) from CLINIC_master where clinic_type in" + System.Configuration.ConfigurationManager.AppSettings["putong"] + "and returned_date is null and VISIT_DATE >= to_date('" + d1 + " 00:00:00', 'yyyy-mm-dd hh24:mi:ss') and VISIT_DATE <= to_date('" + d2 + " 23:59:59', 'yyyy-mm-dd hh24:mi:ss')";
                string s2 = myCmd.ExecuteScalar().ToString();
                myCmd.CommandText = "select count(*) from CLINIC_master where clinic_type in" + System.Configuration.ConfigurationManager.AppSettings["jizhen"] + "and returned_date is null and VISIT_DATE >= to_date('" + d1 + " 00:00:00', 'yyyy-mm-dd hh24:mi:ss') and VISIT_DATE <= to_date('" + d2 + " 23:59:59', 'yyyy-mm-dd hh24:mi:ss')";
                string s3 = myCmd.ExecuteScalar().ToString();
                myCmd.CommandText = "select count(*) from CLINIC_master where clinic_type in" + System.Configuration.ConfigurationManager.AppSettings["tijian"] + "and returned_date is null and VISIT_DATE >= to_date('" + d1 + " 00:00:00', 'yyyy-mm-dd hh24:mi:ss') and VISIT_DATE <= to_date('" + d2 + " 23:59:59', 'yyyy-mm-dd hh24:mi:ss')";
                string s4 = myCmd.ExecuteScalar().ToString();
                conn.Close();
                Response.Write(d1 + "到" + d2 + "的专家门诊数为" + s1 + "，普通门诊数为" + s2 + "，急诊数为" + s3 + "，免费体检数为" + s4);

                //Response.End();
                Response.Flush();
                Response.SuppressContent = true;
                Context.ApplicationInstance.CompleteRequest(); 
            }
        }
    }
}