using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
    public class PlanDA : DataAccess
    {
        public Plan GetPlan(string planKey)
        {
            return _db.Plan.Where(e => e.planKey.Equals(planKey)).SingleOrDefault();
        }

        public bool HasAny(string planKey)
        {
            IQueryable<Plan> qu = _db.Plan;

            if (!string.IsNullOrEmpty(planKey))
            {
                qu = qu.Where(e => e.planKey.Equals(planKey));
            }

            return qu.Count() > 1;
        }

        public IQueryable<Plan> GetQuery(string planKey, string planName)
        {
            IQueryable<Plan> qu = _db.Plan;

            if (!string.IsNullOrEmpty(planKey))
            {
                qu = qu.Where(e => e.planKey.Contains(planKey));
            }

            if (!string.IsNullOrEmpty(planName))
            {
                qu = qu.Where(e => e.planName.Contains(planName));
            }

            return qu;
        }

        public void Create(string planKey, string planName, string Remark)
        {
            Plan _Plan = new Plan();

            _Plan.planKey = planKey;
            _Plan.planName = planName;
            _Plan.Remark = Remark;

            _db.Plan.Add(_Plan);

            _db.SaveChanges();
        }

        public void Edit(string planKey, string planName, string Remark)
        {
            Plan _Plan = GetPlan(planKey);

            if (_Plan == null)
                return;

            _Plan.planName = planName;
            _Plan.Remark = Remark;

            _db.SaveChanges();
        }

        public void Delete(string planKey)
        {
            Plan _Plan = GetPlan(planKey);
            _db.Plan.Remove(_Plan);

            _db.SaveChanges();
        }

        public bool CheckPlan(DataTable table, out string msg)
        {
            msg = string.Empty;
            bool isSuccess = true;
            var datas = table.AsEnumerable().Where(e => !e.Field<string>("計畫代碼").Equals(""));           
            bool commit = true;
            IQueryable<Plan> qu = _db.Plan;
            List<Plan> _LitPlan = qu.ToList<Plan>();
            string[] StrArry = new string[_LitPlan.Count()];

            for(int i=0;i< StrArry.Length;i++)
            {
                StrArry[i] = _LitPlan[i].planKey;
            }

            commit = !datas.Where(e => !string.IsNullOrEmpty(e.Field<string>("計畫代碼")) && !StrArry.Contains(e.Field<string>("計畫代碼"))).Any();

            if (!commit)
            {
                msg = msg + "計畫代碼" + "型別不正確" + Environment.NewLine;
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool CheckPlan(DataTable table, ref List<DataRow> row)
        {
            bool isSuccess = true;
            var datas = table.AsEnumerable().Where(e => e["計畫代碼"] != DBNull.Value && !e.Field<string>("計畫代碼").Equals(""));
            bool commit = true;
            IQueryable<Plan> qu = _db.Plan;
            List<Plan> _LitPlan = qu.ToList<Plan>();
            string[] StrArry = new string[_LitPlan.Count()];

            for (int i = 0; i < StrArry.Length; i++)
            {
                StrArry[i] = _LitPlan[i].planKey;
            }

            commit = !datas.Where(e => !string.IsNullOrEmpty(e.Field<string>("計畫代碼")) && !StrArry.Contains(e.Field<string>("計畫代碼"))).Any();

            if (!commit)
            {
                row.AddRange(
                    row.Except(
                        datas.Where(e => !string.IsNullOrEmpty(e.Field<string>("計畫代碼")) && !StrArry.Contains(e.Field<string>("計畫代碼"))).ToList()
                    ).ToList()
                    );
                isSuccess = false;
            }

            return isSuccess;
        }
    }
}
