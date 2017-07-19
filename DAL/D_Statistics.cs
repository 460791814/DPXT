using Model.Statistics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Comp;

namespace DAL
{
    /// <summary>
    /// 统计
    /// </summary>
    public class D_Statistics
    {
        /// <summary>
        /// 点评项综合统计
        /// </summary>
        /// <returns></returns>
        public List<E_ItemTotal> GetItemTotal(E_StatisticsParameter eStatisticsParameter)
        {
            List<E_ItemTotal> list = new List<E_ItemTotal>();

            StringBuilder strwhere =new StringBuilder();
            if (eStatisticsParameter.areaid > 0)
            {
                strwhere.AddWhere($"D.areaid={eStatisticsParameter.areaid}");
            }
            if (eStatisticsParameter.classinfoid > 0)
            {
                strwhere.AddWhere($"D.classinfoid={eStatisticsParameter.classinfoid}");
            }
            if (eStatisticsParameter.starttime != null)
            {
                strwhere.AddWhere($"D.addtime>cast('{Convert.ToDateTime(eStatisticsParameter.starttime).ToString("yyyy-MM-dd")}' as datetime)");
            }
            if (eStatisticsParameter.endtime != null)
            {
                strwhere.AddWhere($"D.addtime<cast('{Convert.ToDateTime(eStatisticsParameter.endtime).ToString("yyyy-MM-dd")}' as datetime)");
            }
            
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select 
                            typename,
                            title,											--点评维度、点评项标题
                            sum(B.expect) as sumexpect,						--期望值总和
                            sum(B.real) as sumreal,							--感知值总和
                            (sum(B.real)-sum(B.expect)) as sumdifferent     --差值 
                            from dp_commentitem as A 
                            left join dp_commentcommentitem as B on A.commentitemid=B.commentitemid
                            left join dp_commenttype as C on A.commenttypeid=C.commenttypeid 
                            left join dp_comment as D on D.commentid=B.commentid {0}
                            group by A.commentitemid,A.title,C.typename", strwhere.ToString());
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_ItemTotal>(strSql.ToString())?.ToList();
            }
            return list;
        }

        /// <summary>
        /// 点评项综合平均统计
        /// </summary>
        /// <returns></returns>
        public List<E_ItemTotalAvg> GetItemTotalAvg(E_StatisticsParameter eStatisticsParameter)
        {
            List<E_ItemTotalAvg> list = new List<E_ItemTotalAvg>();

            StringBuilder strwhere = new StringBuilder();
            if (eStatisticsParameter.sex > 0)
            {
                strwhere.AddWhere($"D.sex={eStatisticsParameter.sex}");
            }
            if (eStatisticsParameter.ageround > 0)
            {
                strwhere.AddWhere($"D.ageround={eStatisticsParameter.ageround}");
            }
            if (eStatisticsParameter.domain > 0)
            {
                strwhere.AddWhere($"D.domain={eStatisticsParameter.domain}");
            }
            if (eStatisticsParameter.job > 0)
            {
                strwhere.AddWhere($"D.job={eStatisticsParameter.job}");
            }
            if (eStatisticsParameter.areaid > 0)
            {
                strwhere.AddWhere($"D.areaid={eStatisticsParameter.areaid}");
            }
            if (eStatisticsParameter.classinfoid > 0)
            {
                strwhere.AddWhere($"D.classinfoid={eStatisticsParameter.classinfoid}");
            }
            if (eStatisticsParameter.starttime != null)
            {
                strwhere.AddWhere($"D.addtime>cast('{Convert.ToDateTime(eStatisticsParameter.starttime).ToString("yyyy-MM-dd")}' as datetime)");
            }
            if (eStatisticsParameter.endtime != null)
            {
                strwhere.AddWhere($"D.addtime<cast('{Convert.ToDateTime(eStatisticsParameter.endtime).ToString("yyyy-MM-dd")}' as datetime)");
            }

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select 
                            C.typename,																									                        --点评维度
                            cast(avg(cast(B.expect as float)) as decimal(18,2)) as avgexpect,																	--期望均值(E)
                            cast(avg(cast(B.real as float)) as decimal(18,2))  as avgreal,																		--感知均值(P)
                            cast(avg(cast(B.real as float))-avg(cast(B.expect as float))as decimal(18,2)) as avgdifferent,									    --差值（P-E）均值
                            cast((avg(cast(B.real as float))-avg(cast(B.expect as float)))/avg(cast(B.expect as float))as decimal(18,2)) as expectdeviation,	--期望偏离度（（P-E）/E*100%）
                            cast((avg(cast(B.real as float))-avg(cast(B.expect as float)))/avg(cast(B.expect as float))+1 as decimal(18,2)) as satisfaction	    --顾客满意度（期望偏离度+100%）
                            from dp_commentitem as A 
                            left join dp_commentcommentitem as B on A.commentitemid=B.commentitemid
                            left join dp_commenttype as C on A.commenttypeid=C.commenttypeid 
                            left join dp_comment as D on D.commentid=B.commentid {0} 
                            group by C.commenttypeid,C.typename", strwhere.ToString());
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_ItemTotalAvg>(strSql.ToString())?.ToList();
            }
            return list;
        }

        /// <summary>
        /// 综合统计
        /// </summary>
        /// <param name="eStatisticsParameter"></param>
        /// <returns></returns>
        public List<E_ColligateReport> GetColligateReport(E_StatisticsParameter eStatisticsParameter)
        {
            List<E_ColligateReport> list = new List<E_ColligateReport>();

            StringBuilder strwhere = new StringBuilder();
            if (eStatisticsParameter.areaid > 0)
            {
                strwhere.AddWhere($"D.areaid={eStatisticsParameter.areaid}");
            }
            if (eStatisticsParameter.classinfoid > 0)
            {
                strwhere.AddWhere($"D.classinfoid={eStatisticsParameter.classinfoid}");
            }
            if (eStatisticsParameter.starttime != null)
            {
                strwhere.AddWhere($"D.addtime>cast('{Convert.ToDateTime(eStatisticsParameter.starttime).ToString("yyyy-MM-dd")}' as datetime)");
            }
            if (eStatisticsParameter.endtime != null)
            {
                strwhere.AddWhere($"D.addtime<cast('{Convert.ToDateTime(eStatisticsParameter.endtime).ToString("yyyy-MM-dd")}' as datetime)");
            }

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select 
                                    *,
                                    (sex_1_real-sex_1_expect) as sex_1_different,		--男差值(P-E)
                                    (sex_2_real-sex_2_expect) as sex_2_different,		--女差值（P-E）

                                    (ageround_1_real-ageround_1_expect) as ageround_1_different,	--18-30岁 差值(P-E)
                                    (ageround_2_real-ageround_2_expect) as ageround_2_different,	--30-45岁 差值（P-E）
                                    (ageround_3_real-ageround_3_expect) as ageround_3_different,	--45岁以上 差值（P-E）

                                    (domain_1_real-domain_1_expect) as domain_1_different,		--南方 差值（P-E）
                                    (domain_2_real-domain_2_expect) as domain_2_different,		--北方 差值（P-E）

                                    (job_1_real-job_1_expect) as job_1_different,	--定员 差值（P-E）
                                    (job_2_real-job_2_expect) as job_2_different		--访客 差值（P-E）
                                    from
                                    (
	                                    select 
	                                    typename,
	                                    title,
	                                    sum(case when D.sex=1 then B.expect else 0 end) as sex_1_expect,    --男，期望值（E）
	                                    sum(case when D.sex=2 then B.expect else 0 end) as sex_2_expect,    --女，期望值（E）
	                                    sum(case when D.sex=1 then B.real else 0 end) as sex_1_real,		--男，感知值（P）
	                                    sum(case when D.sex=2 then B.real else 0 end) as sex_2_real,		--女，感知值（P）
	
	                                    sum(case when D.ageround=1 then B.expect else 0 end) as ageround_1_expect,  --18-30岁，期望值（E）
	                                    sum(case when D.ageround=2 then B.expect else 0 end) as ageround_2_expect,	--30-45岁，期望值（E）
	                                    sum(case when D.ageround=3 then B.expect else 0 end) as ageround_3_expect,  --45岁以上，期望值（E）
	                                    sum(case when D.ageround=1 then B.real else 0 end) as ageround_1_real,		--18-30岁，感知值（P）
	                                    sum(case when D.ageround=2 then B.real else 0 end) as ageround_2_real,		--30-45岁，感知值（P）
	                                    sum(case when D.ageround=3 then B.real else 0 end) as ageround_3_real,		--45岁以上，感知值（P）
	
	                                    sum(case when D.domain=1 then B.expect else 0 end) as domain_1_expect,		--南方，期望值（E）
	                                    sum(case when D.domain=2 then B.expect else 0 end) as domain_2_expect,		--北方，期望值（E）
	                                    sum(case when D.domain=1 then B.real else 0 end) as domain_1_real,			--南方，感知值（P）
	                                    sum(case when D.domain=2 then B.real else 0 end) as domain_2_real,			--北方，感知值（P）
	
	                                    sum(case when D.job=1 then B.expect else 0 end) as job_1_expect,	--定员，期望值（E）
	                                    sum(case when D.job=2 then B.expect else 0 end) as job_2_expect,		--访客，期望值（E）
	                                    sum(case when D.job=1 then B.real else 0 end) as job_1_real,		--定员，感知值（P）
	                                    sum(case when D.job=2 then B.real else 0 end) as job_2_real			--访客，感知值（P）
	
	                                    from dp_commentitem as A 
	                                    left join dp_commentcommentitem as B on A.commentitemid=B.commentitemid
	                                    left join dp_commenttype as C on A.commenttypeid=C.commenttypeid
	                                    left join dp_comment as D on D.commentid=B.commentid
	                                    {0}
	                                    group by A.commentitemid,A.title,C.typename
                                    ) as T", strwhere.ToString());
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_ColligateReport>(strSql.ToString())?.ToList();
            }
            return list;
        }

        /// <summary>
        /// 人员统计
        /// </summary>
        /// <param name="eStatisticsParameter"></param>
        /// <returns></returns>
        public List<E_PersonReport> GetPersonReport(E_StatisticsParameter eStatisticsParameter)
        {
            List<E_PersonReport> list = new List<E_PersonReport>();

            StringBuilder strwhere = new StringBuilder();
            if (eStatisticsParameter.areaid > 0)
            {
                strwhere.AddWhere($"A.areaid={eStatisticsParameter.areaid}");
            }
            if (eStatisticsParameter.classinfoid > 0)
            {
                strwhere.AddWhere($"A.classinfoid={eStatisticsParameter.classinfoid}");
            }

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select 
                                    *,
                                    case when total>0 then cast(cast(perfect as float)/cast(total as float) as decimal(18,2)) else 0 end as perfectrate,	--好评率
                                    case when total>0 then cast(cast(bad as float)/cast(total as float) as decimal(18,2)) else 0 end as badrate			--差评率
                                    from
                                    (
	                                    select  A.*,(perfect+good+bad) as total,B.CName as classname from dp_person as A
	                                    left join ClassInfo as B on A.classinfoid=B.id {0}
                                    ) as T", strwhere.ToString());
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_PersonReport>(strSql.ToString())?.ToList();
            }
            return list;
        }
    }
}
