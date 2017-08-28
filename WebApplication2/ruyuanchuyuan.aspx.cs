using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class ruyuanchuyuan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["id"] == null) return;
            if (!auth.isAuthorized(Session["id"].ToString(), "ruyuanchuyuan")) return;
            string strt = Request.QueryString["strt"];
            string end = Request.QueryString["end"];
            if (string.IsNullOrEmpty(strt) || string.IsNullOrEmpty(end)) return;
            OracleDataAdapter da = new OracleDataAdapter(new OracleCommand(@"select fbihid 住院号,
                        fbincu 住院次数,
                        fid id号,
                        fname 姓名,
                        decode(fsex,'1','男','2','女','其他') 性别,
                        fage 年龄,
                        faged 年龄单位,
                        FLTEL 电话,
                        FMADD 现住址,
                        TO_CHAR(fihdat,'yyyy-mm-dd') 入院日期,
                        (select fdesc from ticd10 where ticd10.ficd10=FMZZD) 入院诊断,
                        TO_CHAR(fodate,'yyyy-mm-dd') 出院日期,
                        (select a.fdesc from ticd10 a,tmrdzd b where a.ficd10=b.ficd and b.fseq='1' and b.fmrdid =tmrdde.fmrdid) 出院诊断,
                        fzrys 责任医生
                        from tmrdde 
                        where fodate <= to_date('" + end + @" 23:59:59', 'yyyy-mm-dd hh24:mi:ss')
                        and fodate >= to_date('" + strt + @" 00:00:00', 'yyyy-mm-dd hh24:mi:ss')
                        and fioffi = " + Session["dept"],
                        new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.10.4.80)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME = mhis)));User ID=ysmhis;Password=ysmhis")));
            DataTable dt = new DataTable();
            da.Fill(dt);
            OracleConnection myConn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.10.4.3)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME = orcl)));User ID=system;Password=manager");
            OracleCommand myCmd = new OracleCommand("", myConn);
            myConn.Open();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["年龄单位"].ToString() == "D")
                {
                    int y = int.Parse(dr["年龄"].ToString()) / 365;
                    dr["年龄"] = y.ToString();
                }
                else if (dr["年龄单位"].ToString() == "M")
                {
                    int y = int.Parse(dr["年龄"].ToString()) / 12;
                    dr["年龄"] = y.ToString();
                }
                myCmd.CommandText = "select discharge_bed_no from pat_visit where patient_id=" + dr["id号"] + " and visit_id=" + dr["住院次数"];
                dr["年龄单位"] = myCmd.ExecuteScalar().ToString();
            }
            dt.Columns["年龄单位"].ColumnName = "床号";
            dt.Columns.Remove("住院次数");
            myConn.Close();
            GridView1.DataSource = dt.DefaultView;
            GridView1.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=入院出院病人列表.doc");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-word";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            DataGrid excel = new DataGrid();
            excel.DataSource = GridView1.DataSource;
            excel.DataBind();
            excel.RenderControl(hw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            //Response.End();
            Response.SuppressContent = true;
            Context.ApplicationInstance.CompleteRequest(); 
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=入院出院病人列表.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            DataGrid excel = new DataGrid();
            excel.DataSource = GridView1.DataSource;
            excel.DataBind();
            excel.RenderControl(hw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            //Response.End();
            Response.SuppressContent = true;
            Context.ApplicationInstance.CompleteRequest(); 
        }
    }
}