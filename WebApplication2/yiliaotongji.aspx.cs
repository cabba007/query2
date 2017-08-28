using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class yiliaotongji : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["id"] == null) return;
            if (!auth.isAuthorized(Session["id"].ToString(), "yiliaotongji")) return;
            if (Request.QueryString["querydept"] == "querydept")
            {
                string[] chuangjianban = System.Configuration.ConfigurationManager.AppSettings["chuangjianban"].Split(',');
                if (Array.IndexOf(chuangjianban, Session["id"].ToString()) != -1)
                {
                    Response.Write("全部科室");
                    //Response.End();                
                    Response.Flush();

                    Response.SuppressContent = true;
                    Context.ApplicationInstance.CompleteRequest(); 
                    return;
                }
                Response.Write(Session["deptname"].ToString());
                //Response.End();                
                Response.Flush();

                Response.SuppressContent = true;
                Context.ApplicationInstance.CompleteRequest(); 
                return;
            }

            xiangmu[] xms = new xiangmu[14];
            int i = 0;
            string strt = Request.QueryString["strt"];
            string end = Request.QueryString["end"];
            if (string.IsNullOrEmpty(strt) || string.IsNullOrEmpty(end)) return;
            OracleConnection conn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.10.4.80)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME = mhis)));User ID=mhis;Password=mhis");
            conn.Open();
            OracleCommand myCmd = new OracleCommand(null, conn);
            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = "select count(*),round(sum(nvl(a.fbihda,0))/count(*),2),round(count(*)*count(*)/sum(b.fbqnt),1) from tmrdde a, toffim b where a.fooffi = b.foffn and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') ";
            else
                myCmd.CommandText = "select count(*),round(sum(nvl(a.fbihda,0))/count(*),2),round(count(*)*count(*)/sum(b.fbqnt),1) from tmrdde a, toffim b where a.fooffi = b.foffn and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and b.fdesc='" + Request.QueryString["dept"] + "'";
            OracleDataReader dr = myCmd.ExecuteReader();
            dr.Read();
            xms[i++] = new xiangmu("出院人次", string.IsNullOrEmpty(dr[0].ToString()) ? "N/A" : dr[0].ToString(),"green");
            xms[i++] = new xiangmu("平均住院日", string.IsNullOrEmpty(dr[1].ToString()) ? "N/A" : dr[1].ToString(), "green");
            xms[i++] = new xiangmu("病床周转次数", string.IsNullOrEmpty(dr[2].ToString()) ? "N/A" : dr[2].ToString(), "green");

            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = "select count(*) from tmrdde a, toffim b where a.fooffi = b.foffn and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and a.flevway = '4'";
            else
                myCmd.CommandText = "select count(*) from tmrdde a, toffim b where a.fooffi = b.foffn and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and b.fdesc='" + Request.QueryString["dept"] + "' and a.flevway = '4'";
            dr = myCmd.ExecuteReader();
            dr.Read();
            xms[i++] = new xiangmu("自动出院人数", dr[0].ToString(), "green");

            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = "select sum(nvl(a.fsalcu,0)) from tmrdde a, toffim b where a.fooffi = b.foffn and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') ";
            else
                myCmd.CommandText = "select sum(nvl(a.fsalcu,0)) from tmrdde a, toffim b where a.fooffi = b.foffn and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and b.fdesc='" + Request.QueryString["dept"] + "'";
            string qjcs = myCmd.ExecuteScalar().ToString();
            xms[i++] = new xiangmu("住院危重病人抢救次数", string.IsNullOrEmpty(qjcs) ? "0" : qjcs, "green");
            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = "select sum(nvl(a.fscucu,0))  from tmrdde a, toffim b where a.fooffi = b.foffn and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') ";
            else
                myCmd.CommandText = "select sum(nvl(a.fscucu,0))  from tmrdde a, toffim b where a.fooffi = b.foffn and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and b.fdesc='" + Request.QueryString["dept"] + "'";
            string qjcgcs = myCmd.ExecuteScalar().ToString();
            xms[i++] = new xiangmu("抢救成功次数", string.IsNullOrEmpty(qjcgcs) ? "0" : qjcgcs, "green");
            int nqjcs = 0;
            int.TryParse(qjcs, out nqjcs);
            int nqjcgcs = 0;
            int.TryParse(qjcgcs, out nqjcgcs);
            if (nqjcs != 0)
            {
                double fqjcgl = nqjcgcs * 100.0 / nqjcs * 1.0; xms[i++] = new xiangmu("抢救成功率", fqjcgl.ToString("F2") + "%", "green");
            }
            else
                xms[i++] = new xiangmu("抢救成功率", "N/A", "green");

            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = "select 100 * round((sum(nvl(c.f0613,0)) + sum(nvl(c.f0714,0)))/sum(nvl(c.ffsum,0)),4)  from tmrdde a, toffim b, tmrdde_charge c where a.fooffi = b.foffn(+) and a.fmrdid = c.fmrdid(+) and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss')";
            else
                myCmd.CommandText = "select 100 * round((sum(nvl(c.f0613,0)) + sum(nvl(c.f0714,0)))/sum(nvl(c.ffsum,0)),4)  from tmrdde a, toffim b, tmrdde_charge c where a.fooffi = b.foffn(+) and a.fmrdid = c.fmrdid(+) and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and b.fdesc='" + Request.QueryString["dept"] + "'";
            xms[i++] = new xiangmu("药品收入占医疗收入比例", string.IsNullOrEmpty(myCmd.ExecuteScalar().ToString()) ? "N/A" : myCmd.ExecuteScalar().ToString() + "%", "green");

            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = "select count(*)  from tmrdde a, toffim b where a.fooffi = b.foffn(+) and a.fbihda > 30 and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss')";
            else
                myCmd.CommandText = "select count(*)  from tmrdde a, toffim b where a.fooffi = b.foffn(+) and a.fbihda > 30 and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and b.fdesc='" + Request.QueryString["dept"] + "'";
            xms[i++] = new xiangmu("住院时间超过30天的患者人数", myCmd.ExecuteScalar().ToString(), "green");

            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = "select count(*)  from tmrdde a, toffim b where a.fooffi = b.foffn(+) and a.fsxfy in ('1','2') and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss')";
            else
                myCmd.CommandText = "select count(*)  from tmrdde a, toffim b where a.fooffi = b.foffn(+) and a.fsxfy in ('1','2') and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and b.fdesc='" + Request.QueryString["dept"] + "'";
            xms[i++] = new xiangmu("输血例次", myCmd.ExecuteScalar().ToString(), "green");
            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = "select count(*)  from tmrdde a, toffim b where a.fooffi = b.foffn(+) and a.fsxfy in ('1') and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss')";
            else
                myCmd.CommandText = "select count(*)  from tmrdde a, toffim b where a.fooffi = b.foffn(+) and a.fsxfy in ('1') and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and b.fdesc='" + Request.QueryString["dept"] + "'";
            xms[i++] = new xiangmu("输血反应发生例次", myCmd.ExecuteScalar().ToString(), "green");
            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = "select count(decode(a.fryycy,1,1,null)),count(*) from tmrdde a, toffim b where a.fooffi = b.foffn(+) and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss')";
            else
                myCmd.CommandText = "select count(decode(a.fryycy,1,1,null)),count(*) from tmrdde a, toffim b where a.fooffi = b.foffn(+) and a.fodate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.fodate <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and b.fdesc='" + Request.QueryString["dept"] + "'";
            dr = myCmd.ExecuteReader();
            dr.Read();
            int nzdzs = 0;
            int.TryParse(dr[1].ToString(), out nzdzs);
            int nzdfhs = 0;
            int.TryParse(dr[0].ToString(), out nzdfhs);
            if (nzdzs != 0)
            {
                double fzdfhl = nzdfhs * 100.0 / nzdzs * 1.0; xms[i++] = new xiangmu("入出院诊断符合率", fzdfhl.ToString("F2") + "%", "green");
            }
            else
                xms[i++] = new xiangmu("入出院诊断符合率", "N/A", "green");
            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = "select decode(sum(nvl(b.fbqnt,0)),0, '无开放床位', 100 * round(sum(nvl(c.fxyrs,0))/sum(nvl(b.fbqnt,0)),4)) from  toffim b, tipsi c where b.foffn = c.foffi(+) and c.foffi not in ('02','0228') and c.fdate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and c.fdate <= to_date('" + end + " 00:00:00','yyyy-mm-dd hh24:mi:ss')";
            else
                myCmd.CommandText = "select decode(sum(nvl(b.fbqnt,0)),0, '无开放床位', 100 * round(sum(nvl(c.fxyrs,0))/sum(nvl(b.fbqnt,0)),4)) from  toffim b, tipsi c where b.foffn = c.foffi(+) and c.foffi not in ('02','0228') and c.fdate >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and c.fdate <= to_date('" + end + " 00:00:00','yyyy-mm-dd hh24:mi:ss')and b.fdesc='" + Request.QueryString["dept"] + "'";
            xms[i++] = new xiangmu("病床使用率", string.IsNullOrEmpty(myCmd.ExecuteScalar().ToString()) ? "N/A" : myCmd.ExecuteScalar().ToString() + "%", "green");

            conn.Close();

            conn.ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.10.4.3)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME = orcl)));User ID=system;Password=manager";
            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = @"select  sum(cc)
  from (select d.dept_name,1 cc from ORDERS o, dept_dict d
         where o.ordering_dept = d.dept_code
           and o.start_date_time >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and o.start_date_time <= to_date('" + end + @" 23:59:59','yyyy-mm-dd hh24:mi:ss')
           and o.order_status in ('2', '3', '4')
         group by o.patient_id, o.visit_id, o.order_no, d.dept_name
         order by o.doctor)";
            else
            myCmd.CommandText = @"select  sum(cc)
  from (select d.dept_name,1 cc from ORDERS o, dept_dict d
         where o.ordering_dept = d.dept_code
           and o.start_date_time >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and o.start_date_time <= to_date('" + end + @" 23:59:59','yyyy-mm-dd hh24:mi:ss')
           and o.order_status in ('2', '3', '4')
         group by o.patient_id, o.visit_id, o.order_no, d.dept_name
         order by o.doctor)
 where dept_name ='" + Request.QueryString["dept"] + "'";
            conn.Open();
            dr = myCmd.ExecuteReader();
            dr.Read();
            xms[i++] = new xiangmu("医嘱数", dr[0].ToString(), "green");

            /*
            if (Request.QueryString["depto"] == "全部科室")
                myCmd.CommandText = "select count(*) from drug_presc_master a, drug_presc_detail b where a.presc_date = b.presc_date and a.presc_no = b.presc_no and a.presc_source = 0 and a.presc_date >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.presc_date <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss')";
            else
                myCmd.CommandText = "select count(*) from drug_presc_master a, drug_presc_detail b,dept_dict where a.ORDERED_BY = dept_dict.dept_code and a.presc_date = b.presc_date and a.presc_no = b.presc_no and a.presc_source = 0 and a.presc_date >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.presc_date <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and dept_dict.dept_name='" + Request.QueryString["depto"] + "'";
            string mzcf = myCmd.ExecuteScalar().ToString();
            int nmzcf = 0;
            int.TryParse(mzcf, out nmzcf);
            if (Request.QueryString["depto"] == "全部科室")
                myCmd.CommandText = "select count(*) from drug_presc_master a, drug_presc_detail b where b.drug_code in (select distinct drug_code from DRUG_PRICE_LIST where class_on_mr = '1301') and a.presc_date = b.presc_date and a.presc_no = b.presc_no and a.presc_source = 0 and a.presc_date >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.presc_date <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss')";
            else
                myCmd.CommandText = "select count(*) from drug_presc_master a, drug_presc_detail b,dept_dict where b.drug_code in (select distinct drug_code from DRUG_PRICE_LIST where class_on_mr = '1301') and a.ORDERED_BY = dept_dict.dept_code and a.presc_date = b.presc_date and a.presc_no = b.presc_no and a.presc_source = 0 and a.presc_date >= to_date('" + strt + " 00:00:00','yyyy-mm-dd hh24:mi:ss') and a.presc_date <= to_date('" + end + " 23:59:59','yyyy-mm-dd hh24:mi:ss') and dept_dict.dept_name='" + Request.QueryString["depto"] + "'";
            string mzkjcf = myCmd.ExecuteScalar().ToString();
            int nmzkjcf = 0;
            int.TryParse(mzkjcf, out nmzkjcf);
            if (nmzcf != 0)
            {
                double fmzkjcfbl = nmzkjcf * 100.0 / nmzcf * 1.0; xms[i++] = new xiangmu("门诊抗菌药物处方比例", fmzkjcfbl.ToString("F2") + "%", "red");
            }
            else
                xms[i++] = new xiangmu("门诊抗菌药物处方比例", "N/A", "red");
            


            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = @"select count(*)
                                      from (select *
                                              from (select d.patient_id, d.visit_id
                                                      from pat_visit d
                                                     where d.admission_date_time >=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss')
                                                       and d.admission_date_time <=to_date('" + end + @" 23:59:59','yyyy-mm-dd hh24:mi:ss')
                                                    union all
                                                    select d.patient_id, d.visit_id
                                                      from pat_visit d
                                                     where d.admission_date_time <=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss')
                                                       and (d.discharge_date_time is null or d.discharge_date_time >=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss'))
                                                    )
                                             group by patient_id, visit_id)";
            else
                myCmd.CommandText = @"select count(*)
                                      from (select *
                                              from (select d.patient_id, d.visit_id
                                                      from pat_visit d,dept_dict
                                                     where d.admission_date_time >=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss')
                                                       and d.admission_date_time <=to_date('" + end + @" 23:59:59','yyyy-mm-dd hh24:mi:ss')
                                                       and d.DEPT_ADMISSION_TO = dept_dict.dept_code and dept_dict.dept_name ='" + Request.QueryString["dept"] + @"'
                                                    union all
                                                    select d.patient_id, d.visit_id
                                                      from pat_visit d,dept_dict
                                                     where d.admission_date_time <=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss')
                                                       and (d.discharge_date_time is null or d.discharge_date_time >=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss'))
                                                       and d.DEPT_ADMISSION_TO = dept_dict.dept_code and dept_dict.dept_name ='" + Request.QueryString["dept"] + @"'
                                                    )
                                             group by patient_id, visit_id)";
            string zyyw = myCmd.ExecuteScalar().ToString();
            int nzyyw = 0;
            int.TryParse(zyyw, out nzyyw);
            if (Request.QueryString["dept"] == "全部科室")
                myCmd.CommandText = @"        select  count(*) 
          from (select b.patient_id, b.visit_id
                  from (select patient_id, visit_id
                          from (select d.patient_id, d.visit_id
                                  from pat_visit d
                                 where d.admission_date_time >=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss')
                                   and d.admission_date_time <=to_date('" + end + @" 23:59:59','yyyy-mm-dd hh24:mi:ss')
                                union all
                                select d.patient_id, d.visit_id
                                  from pat_visit d
                                 where d.admission_date_time <=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss')
                                   and (d.discharge_date_time is null or 
                                        d.discharge_date_time >=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss'))
                                )
                         group by patient_id, visit_id) a,
                       inp_bill_detail b
                 where a.patient_id = b.patient_id
                   and a.visit_id = b.visit_id
                   and b.item_code in
                       (select drug_code
                          from DRUG_PRICE_LIST
                         where class_on_mr = '1301')
                 group by b.patient_id, b.visit_id)";
            else
                myCmd.CommandText = @"        select  count(*) 
          from (select b.patient_id, b.visit_id
                  from (select patient_id, visit_id
                          from (select d.patient_id, d.visit_id
                                  from pat_visit d,dept_dict
                                 where d.admission_date_time >=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss')
                                   and d.admission_date_time <=to_date('" + end + @" 23:59:59','yyyy-mm-dd hh24:mi:ss')
and d.DEPT_ADMISSION_TO = dept_dict.dept_code and dept_dict.dept_name ='" + Request.QueryString["dept"] + @"'
                                union all
                                select d.patient_id, d.visit_id
                                  from pat_visit d,dept_dict
                                 where d.admission_date_time <=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss')
and d.DEPT_ADMISSION_TO = dept_dict.dept_code and dept_dict.dept_name ='" + Request.QueryString["dept"] + @"'
                                   and (d.discharge_date_time is null or d.discharge_date_time >=to_date('" + strt + @" 00:00:00','yyyy-mm-dd hh24:mi:ss'))
                                )
                         group by patient_id, visit_id) a,
                       inp_bill_detail b
                 where a.patient_id = b.patient_id
                   and a.visit_id = b.visit_id
                   and b.item_code in
                       (select drug_code
                          from DRUG_PRICE_LIST
                         where class_on_mr = '1301')
                 group by b.patient_id, b.visit_id)"; 
            string zykjyw = myCmd.ExecuteScalar().ToString();
            int nzykjyw = 0;
            int.TryParse(zykjyw, out nzykjyw);
            if (nzyyw != 0)
            {
                double fzykjywbl = nzykjyw * 100.0 / nzyyw * 1.0; xms[i++] = new xiangmu("住院抗菌药物使用率", fzykjywbl.ToString("F2") + "%", "green");
            }
            else
                xms[i++] = new xiangmu("住院抗菌药物使用率", "N/A", "green");
            */
/*
            string deptout = string.Empty;
            string deptin = string.Empty;
            if (Request.QueryString["dept"] != "全部科室")
            {
                myCmd.CommandText = "select dept_code from dept_dict where dept_name ='" + Request.QueryString["dept"] + "'";
                deptin = myCmd.ExecuteScalar().ToString();
            }
            if (Request.QueryString["depto"] != "全部科室")
            {
                myCmd.CommandText = "select dept_code from dept_dict where dept_name ='" + Request.QueryString["depto"] + "'";
                deptout = myCmd.ExecuteScalar().ToString();
            }

            conn.Close();





            DateTime dt1 = DateTime.ParseExact(end, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
            string end1 = dt1.AddDays(1).ToString("yyyy-MM-dd");
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
            scsb.DataSource = "10.10.4.20";
            scsb.IntegratedSecurity = false;
            scsb.InitialCatalog = "PASSPA2DB";
            scsb.UserID = "HLYY";
            scsb.Password = "123";
            SqlConnection conn1 = new SqlConnection(scsb.ConnectionString);
            SqlCommand cmd1 = new SqlCommand("proc_outpatient_cfhgl", conn1);
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.AddWithValue("@startdate", strt);
            cmd1.Parameters.AddWithValue("@enddate", end);
            cmd1.Parameters.AddWithValue("@deptcode", deptout);
            conn1.Open();
            SqlDataReader dr1 = cmd1.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr1);
            cmd1.CommandText = "proc_outpatient_anti";
            dr1 = cmd1.ExecuteReader();
            dt.Load(dr1);
            cmd1.CommandText = "proc_inpatient_anti";
            cmd1.Parameters["@deptcode"].Value = deptin;
            dr1 = cmd1.ExecuteReader();
            dt.Load(dr1);
            cmd1.CommandText = "proc_inpatient_lab";
            dr1 = cmd1.ExecuteReader();
            dt.Load(dr1);
            cmd1.CommandText = "proc_inpatient_syqd";
            dr1 = cmd1.ExecuteReader();
            DataTable dtdt = new DataTable();
            dtdt.Load(dr1);
            conn1.Close();

            string r0num = dt.Rows[0]["num"].ToString();
            string r0num0 = dt.Rows[0]["num0"].ToString();
            string r1mznum = dt.Rows[1]["mznum"].ToString();
            string r1mzantinum = dt.Rows[1]["mzantinum"].ToString();
            string r1jznum = dt.Rows[1]["jznum"].ToString();
            string r1jzantinum = dt.Rows[1]["jzantinum"].ToString();
            string r2num = dt.Rows[2]["num"].ToString();
            string r2antinum = dt.Rows[2]["antinum"].ToString();
            string r3num = dt.Rows[3]["num"].ToString();
            string r3labnum = dt.Rows[3]["labnum"].ToString();
            string r4ddds = dtdt.Rows[0]["ddds"].ToString();
            string r4syqd = dtdt.Rows[0]["syqd"].ToString();

            int nr0num = 0;
            int.TryParse(r0num, out nr0num);
            int nr0num0 = 0;
            int.TryParse(r0num0, out nr0num0);
            if (nr0num != 0)
            {
                double fproc_outpatient_cfhgl = (nr0num - nr0num0) * 100.0 / nr0num * 1.0; xms[i++] = new xiangmu("门诊处方合格率", fproc_outpatient_cfhgl.ToString("F2") + "%", "red");
            }
            else
                xms[i++] = new xiangmu("门诊处方合格率", "N/A", "red");

            int nr1mznum = 0;
            int.TryParse(r1mznum, out nr1mznum);
            int nr1mzantinum = 0;
            int.TryParse(r1mzantinum, out nr1mzantinum);
            if (nr1mznum != 0)
            {
                double fproc_outpatient_anti = nr1mzantinum * 100.0 / nr1mznum * 1.0; xms[i++] = new xiangmu("门诊抗菌药处方比例", fproc_outpatient_anti.ToString("F2") + "%", "red");
            }
            else
                xms[i++] = new xiangmu("门诊抗菌药处方比例", "N/A", "red");

            int nr2num = 0;
            int.TryParse(r2num, out nr2num);
            int nr2antinum = 0;
            int.TryParse(r2antinum, out nr2antinum);
            if (nr2num != 0)
            {
                double fproc_inpatient_anti = nr2antinum * 100.0 / nr2num * 1.0; xms[i++] = new xiangmu("住院抗菌药使用率", fproc_inpatient_anti.ToString("F2") + "%", "green");
            }
            else
                xms[i++] = new xiangmu("住院抗菌药使用率", "N/A", "green");

            int nr3num = 0;
            int.TryParse(r3num, out nr3num);
            int nr3labnum = 0;
            int.TryParse(r3labnum, out nr3labnum);
            if (nr3num != 0)
            {
                double fproc_inpatient_lab = nr3labnum * 100.0 / nr3num * 1.0; xms[i++] = new xiangmu("限制级抗菌药物微生物送检率", fproc_inpatient_lab.ToString("F2") + "%", "green");
            }
            else
                xms[i++] = new xiangmu("限制级抗菌药物微生物送检率", "N/A", "green");

            //int nr4ddds = 0;
            //int.TryParse(r4ddds, out nr4ddds);
            //int nr4syqd = 0;
            //int.TryParse(r3labnum, out nr4syqd);

            if (!string.IsNullOrEmpty(deptin))
            {

                xms[i++] = new xiangmu("抗菌药物DDDs", System.Text.RegularExpressions.Regex.Replace(r4ddds, @"(^\d{1,}\.\d{2})\d{1,}$", "$1"), "green");
            }
            else
                xms[i++] = new xiangmu("抗菌药物DDDs", "N/A", "green");

            if (!string.IsNullOrEmpty(deptin))
            {
                xms[i++] = new xiangmu("抗菌药物使用强度", System.Text.RegularExpressions.Regex.Replace(r4syqd, @"(^\d{1,}\.\d{2})\d{1,}$", "$1"), "green");
            }
            else
                xms[i++] = new xiangmu("抗菌药物使用强度", "N/A", "green");
*/
            string json = JsonConvert.SerializeObject(xms);
            Response.Write(json);
            //Response.End();                
            Response.Flush();

            Response.SuppressContent = true;
            Context.ApplicationInstance.CompleteRequest(); 
        }
    }

    public class xiangmu
    {
        public xiangmu(string k, string v, string c)
        {
            this.xiangmuming = k;
            this.xiangmuzhi = v;
            this.xiangmuse = c;
        }
        public string xiangmuming { get; set; }
        public string xiangmuzhi { get; set; }
        public string xiangmuse { get; set; }
    }
}