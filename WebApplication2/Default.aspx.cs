using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.Form["pw"]) || string.IsNullOrEmpty(Request.Form["id"]))
            { Response.Write("<script>location.href='Login.html';</script>"); return; }
            OracleConnection myConn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.10.4.3)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME = orcl)));User ID=system;Password=manager");
            OracleCommand myCmd = new OracleCommand("SELECT DECODE(comm.f_descript(staff_dict.password),:p0,'1','0') , staff_dict.DEPT_CODE,dept_dict.dept_name from staff_dict left join dept_dict on staff_dict.dept_code = dept_dict.dept_code WHERE emp_no = :p1", myConn);
            myCmd.Parameters.Add(new OracleParameter("p0", OracleType.Char) { Value = Request.Form["pw"] });
            myCmd.Parameters.Add(new OracleParameter("p1", OracleType.Char) { Value = Request.Form["id"] });
            myConn.Open();
            OracleDataReader myReader = myCmd.ExecuteReader();
            if (myReader.Read())
            {
                if ("1" == myReader[0].ToString())
                {
                    Session.Timeout = 10;
                    Session["dept"] = myReader[1].ToString().TrimEnd(new char[] { 'H', 'L' });
                    Session["deptname"] = myReader[2].ToString().TrimEnd(new char[] { '护', '理', '站' });
                    Session["id"] = Request.Form["id"];
                    myConn.Close();
                }
                else
                {
                    myConn.Close();
                    Response.Write("<script>alert(\"密码不正确！\");location.href='Login.html';</script>");
                }
            }
            else
            {
                myConn.Close();
                Response.Write("<script>alert(\"您的工号没有访问权限！\");location.href='Login.html';</script>");
            }
        }
    }
}