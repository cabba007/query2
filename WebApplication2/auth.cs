using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Web;

namespace WebApplication2
{
    public class auth
    {
        public static bool isAuthorized(string id, string servicename)
        {
            string[] group = System.Configuration.ConfigurationManager.AppSettings["superusers"].Split(',');
            if (Array.IndexOf(group, id) != -1)
                return true;
            else if (servicename == "ruyuanchuyuan")
            {
                group = System.Configuration.ConfigurationManager.AppSettings[servicename].Split(',');
                if (Array.IndexOf(group, id) != -1) return true;
                OracleConnection myConn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.10.4.3)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME = orcl)));User ID=system;Password=manager");
                OracleCommand myCmd = new OracleCommand("SELECT * FROM SECURITY_USERS WHERE (group_name ='科室主任组' or group_name ='护士长组') and name ='" + id + "'", myConn);
                myConn.Open();
                if (myCmd.ExecuteScalar() != null)
                {
                    myConn.Close();
                    return true;
                }
                else
                {
                    myConn.Close();
                    return false;
                }
            }
            else if (servicename == "menzhenliang")
            {
                group = System.Configuration.ConfigurationManager.AppSettings[servicename].Split(',');
                if (Array.IndexOf(group, id) != -1) return true;
                OracleConnection myConn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.10.4.3)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME = orcl)));User ID=system;Password=manager");
                OracleCommand myCmd = new OracleCommand("select * from staff_dict,dept_dict where dept_dict.dept_code=staff_dict.dept_code and dept_dict.dept_name ='门诊办公室' and staff_dict.emp_no= '" + id + "'", myConn);
                myConn.Open();
                if (myCmd.ExecuteScalar() != null)
                {
                    myConn.Close();
                    return true;
                }
                else
                {
                    myConn.Close();
                    return false;
                }
            }
            else if (servicename == "yiliaotongji")
            {
                group = System.Configuration.ConfigurationManager.AppSettings[servicename].Split(',');
                if (Array.IndexOf(group, id) != -1) return true;
                OracleConnection myConn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.10.4.3)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME = orcl)));User ID=system;Password=manager");
                OracleCommand myCmd = new OracleCommand("SELECT * FROM SECURITY_USERS WHERE (group_name ='科室主任组' or group_name ='护士长组') and name ='" + id + "'", myConn);
                myConn.Open();
                if (myCmd.ExecuteScalar() != null)
                {
                    myConn.Close();
                    return true;
                }
                else
                {
                    myConn.Close();
                    return false;
                }
            }


            return false;
        }
    }
}