using ClassLibrary;
using Newtonsoft.Json;
using NHRIDB_DAL.DbModel;
using NHRIDB_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
    public class DataTubeDA : DataAccess
    {  
        List<InfoColummns> _columns = TypeDescriptor.GetProperties(typeof(TubeDataType))
            .Cast<PropertyDescriptor>()
            .Select(e=> new InfoColummns { 
                Name = e.Name,
                DisplayName=e.DisplayName,
                Required= e.Attributes.Cast<Attribute>().Any(a => a.GetType() == typeof(RequiredAttribute)),
                PropertyType=e.PropertyType
             })
            .ToList();

        public List<InfoColummns> GetColummns() {
            return _columns;
        }
        /***
           * 1.欄位名稱是否相符
           * 必填欄位沒有填
           * 2.性別是否統一
           * 3.年齡是否統一
           * 主key重複
           * 資料型別(f:m , 數字 , 0:1)
           * **/
        public bool ImportCheck(DataTable table, out string msg)
        {
            msg = string.Empty;

            //1.欄位名稱是否相符
            if (!HasColumns(table))
            {
                msg = msg + "欄位名稱不符合，請參照範本" + Environment.NewLine;
                return false;
            }

            //必填欄位沒有填
            if (!CheckRequired(table, out msg))
            {
                return false;
            }

            //主key重複
            if (!MatchKey(table, out msg))
            {
                return false;
            }

            //各欄位的資料型別 f:m , 數字 , 0:1)  ,此判斷是否放在更前面點 ???????????????????
            if (!CheckType(table, out msg))
            {
                return false;
            }

            //性別是否統一、年齡是否統一
            if (!RepleData(table, out msg))
            {
                return false;
            }

            return true;
        }
        /***
           * 1.欄位名稱是否相符
           * 必填欄位沒有填
           * 2.性別是否統一
           * 3.年齡是否統一
           * 主key重複
           * 資料型別(f:m , 數字 , 0:1)
           * **/
        public bool ImportCheck(DataTable table, ref List<DataRow> rowNow, string fileex) 
        {
            bool result = true;
            string msg = string.Empty;

            //1.欄位名稱是否相符
            if (!HasColumns(table, out msg))
                {
                msg = msg + "欄位名稱不符合，請參照範本" + Environment.NewLine;
                var err = table.NewRow();
                err[0] = msg;
                rowNow.Add(err);
                result = false;
            }

            //必填欄位沒有填
            if (!CheckRequired(table, ref rowNow))
            {
                result = false;
            }

            //主key重複
            if (!MatchKey(table, ref rowNow))
            {
                result = false;
            }

            //各欄位的資料型別 f:m , 數字 , 0:1)  ,此判斷是否放在更前面點 ???????????????????
            if (!CheckType(table, ref rowNow))

            {
                result = false;
            }

            //性別是否統一、年齡是否統一
            if (!RepleData(table, ref rowNow, fileex))
            {
                result = false;
            }

            return result;
        }

        public bool HasColumns(DataTable dataTable)
        {
            string[] columnNames = dataTable.Columns.Cast<DataColumn>()
                                    .Select(x => x.ColumnName)
                                    .ToArray();
            string[] format = _columns.Select(e => e.DisplayName).ToArray();

            var diff = columnNames.Except(format).ToArray().Aggregate("", (current, s) => current + (s + ",")).Replace(" ", string.Empty).TrimEnd(',');
            // Bryan 需取消新增的11欄位 ,批次也是 ??????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
            return ((from item in format
                     where columnNames.Contains(item)
                     select item).Count() == format.Length);
        }
        public bool HasColumns(DataTable dataTable, out string msg)
        {
            string[] columnNames = dataTable.Columns.Cast<DataColumn>()
                                    .Select(x => x.ColumnName)
                                    .ToArray();
            string[] format = _columns.Select(e => e.DisplayName).ToArray();

            msg = columnNames.Except(format).ToArray().Aggregate("", (current, s) => current + (s + ",")).Replace(" ", string.Empty).TrimEnd(',');
            // Bryan 需取消新增的11欄位 ,批次也是 ??????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
            return ((from item in format
                     where columnNames.Contains(item)
                     select item).Count() == format.Length);
        }

        public bool CheckRequired(DataTable table,out string columnName) {
            // Bryan 需取消 '計畫代碼' ,批次也是 ??????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
            var info = _columns
                        .Where(e => e.Required == true)
                      .ToList();

            bool isSuccess = true;
            columnName = string.Empty;

            foreach (var column in info) 
            {            
                int nullCount=  table.AsEnumerable().Where(e =>  e.Field<string>(column.DisplayName).Equals("")).Count();

                if (nullCount > 0) 
                {
                    columnName = columnName + "必要欄位「" + column.DisplayName + "」未填寫" + Environment.NewLine;
                    isSuccess= false;
                }                
            }

            return isSuccess;
        }
        public bool CheckRequired(DataTable table, ref List<DataRow> row)
        {
            // Bryan 需取消 '計畫代碼' ,批次也是 ??????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
            var info = _columns
                        .Where(e => e.Required == true)
                      .ToList();

            bool isSuccess = true;

            foreach (var column in info)
            {
                var rowCurrent = table.AsEnumerable()
                     .Where(p => p[column.DisplayName] == DBNull.Value || string.IsNullOrEmpty(p[column.DisplayName].ToString()))
                     .ToList();

                if (rowCurrent.Count() > 0)
                {
                    DataRow drNew = table.NewRow();
                    drNew[0] = "必要欄位「" + column.DisplayName + "」未填寫";

                    row.Add(drNew);
                    row.AddRange(
                        rowCurrent.OrderBy(e => e.Field<string>("個案代碼"))
                        );
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        public bool MatchKey(DataTable table, out string msg)
        {
            msg = string.Empty;
            var data = table.AsEnumerable().GroupBy(e => new { key1 = e.Field<string>("個案代碼"), key2 = e.Field<string>("器官/部位代碼"), key3 = e.Field<string>("診斷代碼"), key4 = e.Field<string>("收案年份 (西元年)"), key5 = e.Field<string>("年齡 (歲)") })
                .Where(e => e.Count() > 1);

            if (data.Count() > 0)
            {

                foreach (var Item in data)
                {
                    msg = msg
                        + Item.Key.key1 + "(個案代碼),"
                        + Item.Key.key2 + "(器官/部位代碼),"
                        + Item.Key.key3 + "(診斷代碼),"
                        + Item.Key.key4 + "(收案年份 (西元年)),"
                        + Item.Key.key5 + "(年齡 (歲)) 資料重複"
                        + Environment.NewLine;
                }

                return false;
            }

            return true;
        }

        public bool MatchKey(DataTable table, ref List<DataRow> row)
        {
            var data = table.AsEnumerable().GroupBy(e => new { key1 = e["個案代碼"]?.ToString().ToUpper(), key2 = e["器官/部位代碼"], key3 = e["診斷代碼"], key4 = e["收案年份 (西元年)"], key5 = e["年齡 (歲)"] })
                            .Where(e => e.Count() > 1)
                            .Select(x => x.Key.key1)
                            .ToList();


            if (data.Count() > 0)
            {
                var rowCurrent = table.AsEnumerable().Where(x => data.Contains(x["個案代碼"]?.ToString().ToUpper())).ToList();

                DataRow drNew = table.NewRow();
                drNew[0] = "欄位值[個案代碼]、[器官/部位代碼]、[診斷代碼]、[收案年份 (西元年)]、[年齡(歲)] 資料重覆："
                    + data.ToArray().Aggregate("", (current, s) => current + (s + ",")).Replace(" ", string.Empty).TrimEnd(',');

                row.Add(drNew);
                row.AddRange(
                    rowCurrent.OrderBy(e => e.Field<string>("個案代碼")).ToList()
                    );
                return false;
            }

            return true;
        }

        public bool CheckType(DataTable table, out string msg)
        {
            msg = string.Empty;
            bool isSuccess = true;

            foreach (var column in _columns)
            {
                var datas = table.AsEnumerable().Where(e => !e.Field<string>(column.DisplayName).Equals(""));
                decimal age;
                int endYear;
                bool commit = true;
                switch (column.Name)
                {
                    case "planKey":
                    case "patientKey":
                    case "regionKey":
                    case "diagnosisKey":
                        break;
                    case "endYear":
                        commit = !datas.Where(e => e.Field<string>(column.DisplayName).Length != 4 || !int.TryParse(e.Field<string>(column.DisplayName), out endYear)).Any();
                        break;
                    case "age":
                        commit = !datas.Where(e => !decimal.TryParse(e.Field<string>(column.DisplayName), out age) || age < 0).Any();
                        break;
                    case "gender":
                        commit = !datas.Where(e => !e.Field<string>(column.DisplayName).Equals("F") && !e.Field<string>(column.DisplayName).Equals("M")).Any();
                        break;
                    default:
                        commit = !datas.Where(e => !e.Field<string>(column.DisplayName).Equals("0") && !e.Field<string>(column.DisplayName).Equals("1")).Any();
                        break;
                }

                if (!commit)
                {
                    msg = msg + column.DisplayName + "型別不正確" + Environment.NewLine;
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        public bool CheckType(DataTable table, ref List<DataRow> row)
        {
            bool isSuccess = true;

            foreach (var column in _columns)
            {
                if(table.Columns[column.DisplayName] == null)
                {
                    continue;
                }
                var datas = table.AsEnumerable()
                    .Where(p => p[column.DisplayName] != DBNull.Value || !string.IsNullOrEmpty(p[column.DisplayName].ToString()));
                List<DataRow> datas1 = new List<DataRow>();
                decimal age;
                int endYear;
                bool commit = true;
                switch (column.Name)
                {
                    case "planKey":
                    case "patientKey":
                    case "regionKey":
                    case "diagnosisKey":
                        break;
                    case "endYear":
                        datas1 = datas.Where(p => p[column.DisplayName].ToString().Length != 4 || !int.TryParse(p[column.DisplayName].ToString(), out endYear)).ToList();
                        commit = !datas1.Any();
                        break;
                    case "age":
                        datas1 = datas.Where(p => !decimal.TryParse(p[column.DisplayName].ToString(), out age) || age < 0).ToList();
                        commit = !datas1.Any();
                        break;
                    case "gender":
                        datas1 = datas.Where(p => !p[column.DisplayName].ToString().Equals("F") && !p[column.DisplayName].ToString().Equals("M")).ToList();
                        commit = !datas1.Any();
                        break;
                    default:
                        datas1 = datas.Where(p => !p[column.DisplayName].ToString().Equals("0") && !p[column.DisplayName].ToString().Equals("1")).ToList();
                        commit = !datas1.Any();
                        break;
                }

                if (!commit)
                {
                    var rowArr = row.Select(e => e["個案代碼"]).ToArray();
                    var exRow = datas1.Where(e => e["個案代碼"] != DBNull.Value && string.IsNullOrEmpty(e.Field<string>("個案代碼")));
                    exRow = exRow.Where(e => !rowArr.Contains(e["個案代碼"])).ToList();

                    if (datas1.Count > 0)
                    {
                        if (row.Count > 0)
                        {
                            if (exRow.Count() > 0)
                            {
                                DataRow drNew = table.NewRow();
                                drNew[0] = "「" + column.DisplayName + "」型別不正確";

                                row.Add(drNew);
                                row.AddRange(
                                    exRow
                                    );
                                isSuccess = false;
                            }
                        }
                        else
                        {
                            DataRow drNew = table.NewRow();
                            drNew[0] = "「" + column.DisplayName + "」型別不正確";

                            row.Add(drNew);
                            row.AddRange(
                                datas1
                                );
                            isSuccess = false;

                        }
                    }
                }
            }

            return isSuccess;
        }

        public bool RepleData(DataTable table,out string msg) 
        {
            msg = string.Empty;
            List<string> keys=  table.AsEnumerable().GroupBy(e => e.Field<string>("個案代碼")).Where(e => e.Count() > 1)
                .Select(e => e.Key).ToList();
            bool isSuccess = true;

            foreach (string key in keys) {
                var datas = table.AsEnumerable().Where(e => e.Field<string>("個案代碼").Trim().Equals(key.Trim()));
                int sexCount= datas.GroupBy(e => e.Field<string>("性別")).Count();

                if (sexCount > 1) {
                    msg = msg+ "個案代碼:" +key + "性別欄位輸入錯誤" + Environment.NewLine;
                    isSuccess= false;
                }

                double sings =0;
                foreach (var item in datas) {
                    double sing = double.Parse(item.Field<string>("收案年份 (西元年)")) - double.Parse(item.Field<string>("年齡 (歲)"));
                    if (sings == 0) {
                        sings = sing;
                    }

                    if ((sings - sing)>1.5 || (sings - sing)<-1.5) {
                        msg = msg+ "個案代碼:" + key + "年齡欄位輸入錯誤(誤差值大於+-1.5)" + Environment.NewLine;
                        isSuccess= false;
                    }
                }
            }//end foreach key

            return isSuccess;
        }

        public bool RepleData(DataTable table, ref List<DataRow> row, string fileex)
        {
            bool isSuccess = true;

            List<string> keys1 =
            table.AsEnumerable()
                    .Where(e => e.Field<string>("個案代碼") != null && !string.IsNullOrEmpty(e.Field<string>("個案代碼")))
                    .GroupBy(e => e.Field<string>("個案代碼"))
                    .Where(e => e.Count() > 1 && e.GroupBy(f => f.Field<string>("性別")).Count() > 1)
                    .Select(e => e.Key)
                    .ToList();

            if(keys1.Count > 0)
            {
                isSuccess = false;
                DataRow drNew = table.NewRow();
                drNew[0] = "性別不符";
                row.Add(drNew);
            }

            row.AddRange(
                table.AsEnumerable().Where(e => keys1.Contains(e.Field<string>("個案代碼"))).OrderBy(e => e.Field<string>("個案代碼")).ToList()
                );

            List<string> keys2;
            keys2 =
            table.AsEnumerable().GroupBy(e => e.Field<string>("個案代碼"))
                    .Where(e => e != null && !string.IsNullOrEmpty(e.Key) && e.Count() > 1
                        && (
                            (e.Max(f => double.Parse(f.Field<string>("收案年份 (西元年)")) - double.Parse(f.Field<string>("年齡 (歲)"))) -
                             e.Min(f => double.Parse(f.Field<string>("收案年份 (西元年)")) - double.Parse(f.Field<string>("年齡 (歲)")))
                            ) > 1.5 ||
                            (e.Max(f => double.Parse(f.Field<string>("收案年份 (西元年)")) - double.Parse(f.Field<string>("年齡 (歲)"))) -
                             e.Min(f => double.Parse(f.Field<string>("收案年份 (西元年)")) - double.Parse(f.Field<string>("年齡 (歲)")))
                            ) < -1.5
                            ))
                    .Select(e => e.Key)
                    .ToList();
            if(keys2.Count > 0)
            {
                isSuccess = false;
                DataRow drNew = table.NewRow();
                drNew[0] = "年齡不符";
                row.Add(drNew);
            }

            row.AddRange(
                table.AsEnumerable().Where(e => keys2.Contains(e.Field<string>("個案代碼"))).OrderBy(e => e.Field<string>("個案代碼")).ToList()
                );



            return isSuccess;
        }

        public bool BatchRepleData(DataTable table, out string msg,int intBatchStartIndex)
        {
            msg = string.Empty;            
            List<string> keys = table.AsEnumerable().GroupBy(e => e.Field<string>("個案代碼")).Where(e => e.Count() > 1)
                .Select(e => e.Key).ToList();
            bool isSuccess = true;
            int IntBatchRunCount = 1;

            //分批跑,只跑 1萬
            for (int i= intBatchStartIndex; i< keys.Count; i++)
            {
                if (IntBatchRunCount > 10000)
                {
                    intBatchStartIndex = i;

                    break;
                }

                var datas = table.AsEnumerable().Where(e => e.Field<string>("個案代碼").Trim().Equals(keys[i].Trim()));
                int sexCount = datas.GroupBy(e => e.Field<string>("性別")).Count();

                if (sexCount > 1)
                {
                    msg = msg + "個案代碼:" + keys[i] + "性別欄位輸入錯誤" + Environment.NewLine;
                    isSuccess = false;
                }

                double sings = 0;
                foreach (var item in datas)
                {
                    double sing = double.Parse(item.Field<string>("收案年份 (西元年)")) - double.Parse(item.Field<string>("年齡 (歲)"));
                    if (sings == 0)
                    {
                        sings = sing;
                    }

                    if ((sings - sing) > 1.5 || (sings - sing) < -1.5)
                    {
                        msg = msg + "個案代碼:" + keys[i] + "年齡欄位輸入錯誤(誤差值大於+-1.5)" + Environment.NewLine;
                        isSuccess = false;
                    }
                }

                IntBatchRunCount++;
            }

            //foreach (string key in keys)
            //{
            //    var datas = table.AsEnumerable().Where(e => e.Field<string>("個案代碼").Trim().Equals(key.Trim()));
            //    int sexCount = datas.GroupBy(e => e.Field<string>("性別")).Count();

            //    if (sexCount > 1)
            //    {
            //        msg = msg + "個案代碼:" + key + "性別欄位輸入錯誤" + Environment.NewLine;
            //        isSuccess = false;
            //    }

            //    double sings = 0;
            //    foreach (var item in datas)
            //    {
            //        double sing = double.Parse(item.Field<string>("收案年份 (西元年)")) - double.Parse(item.Field<string>("年齡 (歲)"));
            //        if (sings == 0)
            //        {
            //            sings = sing;
            //        }

            //        if ((sings - sing) > 1.5 || (sings - sing) < -1.5)
            //        {
            //            msg = msg + "個案代碼:" + key + "年齡欄位輸入錯誤(誤差值大於+-1.5)" + Environment.NewLine;
            //            isSuccess = false;
            //        }
            //    }
            //}//end foreach key

            return isSuccess;
        }        

        public List<TubeDataType> GetDatasByDataTable(DataTable table) {
            List<DataRow> tt = table.AsEnumerable().ToList();
            
            List<TubeDataType> datas = new List<TubeDataType>();
            foreach (DataRow dr in tt) {
                TubeDataType data = new TubeDataType();
                foreach (var info in _columns) {
                   
                        if (info.PropertyType.Name.Equals("Boolean"))
                        {
                            bool bo = dr[info.DisplayName].Equals("1") ? true : false;
                            data.GetType().GetProperty(info.Name).SetValue(data, Convert.ChangeType(bo, info.PropertyType), null);
                        }
                        else {
                            if(dr[info.DisplayName] != DBNull.Value)
                            {
                                data.GetType().GetProperty(info.Name).SetValue(data, Convert.ChangeType(dr[info.DisplayName], info.PropertyType), null);
                            }
                        }
                   
                   
                }
               
                datas.Add(data);
            }

            return datas;
        }

        //public DataSaveAns Create(List<TubeDataType> datas, Guid hkey, Guid uid)
        //{
        //    DataSaveAns _DataSaveAns = new DataSaveAns();

        //    try
        //    {
        //        List<TubeData> adds = new List<TubeData>();
        //        DateTime now = DateTime.Now;
        //        foreach (TubeDataType data in datas)
        //        {
        //            TubeData tube = new TubeData();
        //            foreach (var info in _columns)
        //            {
        //                var value = data.GetType().GetProperty(info.Name).GetValue(data);
        //                tube.GetType().GetProperty(info.Name).SetValue(tube, Convert.ChangeType(value, info.PropertyType), null);
        //            }
        //            tube.hospitalId = hkey;
        //            tube.createUser = uid;
        //            tube.createDate = now;

        //            adds.Add(tube);
        //        }

        //        List<TubeData> old = _db.TubeData.Where(e => e.hospitalId == hkey).ToList();
        //        TubeDataToLog(old, hkey, uid);
        //        _db.TubeData.RemoveRange(old);
        //        _db.TubeData.AddRange(adds);
        //        _db.SaveChanges();

        //        _DataSaveAns.isSuccess = true;
        //    }
        //    catch (Exception e)
        //    {
        //        _DataSaveAns.StrMsg = e.Message;
        //        _DataSaveAns.StackTrace = e.StackTrace;
        //        _DataSaveAns.isSuccess = false;
        //    }

        //    return _DataSaveAns;
        //}
        public DataSaveAns Create(List<TubeDataType> datas,Guid hkey,Guid uid) 
        {
            DataSaveAns _DataSaveAns = new DataSaveAns();

            var transaction = _db.Database.BeginTransaction();
            try
            {
                List<TubeData> adds = new List<TubeData>();
                DateTime now = DateTime.Now;
                adds.AddRange(
                    tubeDataTypeToListtubeDataList(datas, hkey, uid)
                    );

                using (_db = new NHRIDBEntitiesDB())
                {
                    List<TubeData> old = _db.TubeData.Where(e => e.hospitalId == hkey).ToList();
                    TubeDataToLog(old, hkey, uid);
                }
                int oldCursor = 0;
                int oldCount = 20000;

                using (_db = new NHRIDBEntitiesDB())
                {
                    List<TubeData> old = _db.TubeData.Where(e => e.hospitalId == hkey).ToList();

                    if (old.AsEnumerable().Count() > oldCount)
                    {
                        do
                        {
                            List<TubeData> oldItem = old.Skip(oldCount * oldCursor)
                                .Take(oldCount)
                                .ToList();
                            _db.TubeData.RemoveRange(oldItem);
                            oldCursor++;
                            _db.SaveChanges();

                        } while (old.AsEnumerable().Count() > oldCount * oldCursor);
                    }
                    else
                    {
                        _db.TubeData.RemoveRange(old);
                        _db.SaveChanges();
                    }
                }

                int addsCursor = 0;
                int addCount = 20000;

                using (_db = new NHRIDBEntitiesDB())
                {
                    if (adds.AsEnumerable().Count() > addCount)
                    {
                        do
                        {
                            List<TubeData> addItem = adds.Skip(addCount * addsCursor)
                                .Take(addCount)
                                .ToList();
                            _db.TubeData.AddRange(addItem);
                            addsCursor++;
                            _db.SaveChanges();

                        } while (adds.AsEnumerable().Count() > addCount * addsCursor);
                    }
                    else
                    {
                        _db.TubeData.AddRange(adds);
                        _db.SaveChanges();
                    }
                }

                transaction.Commit();
                _DataSaveAns.isSuccess = true;
            }
            catch(Exception e)
            {
                transaction.Rollback();
                NHRIDBEntitiesDB _db;
                ErrorLogDA _ErrorLogDA;
                _db = new NHRIDBEntitiesDB();
                _ErrorLogDA = new ErrorLogDA(_db);
                string id = DateTime.Now.ToString("yyyyMMddHHmmss");
                string message = string.Empty;
                if (e.InnerException != null && e.InnerException.InnerException != null)
                {
                    message = e.Message + Environment.NewLine + e.InnerException.InnerException.Message;
                }
                else
                {
                    message = e.Message;
                }
                _ErrorLogDA.Create(id: id, controller: string.Empty, action: string.Empty, message: message, stacktrace: e.StackTrace);
                _DataSaveAns.StrMsg = e.Message;
                _DataSaveAns.StackTrace= e.StackTrace;
                _DataSaveAns.isSuccess = false;
            }

            return _DataSaveAns;
        }
        private List<TubeData> tubeDataTypeToListtubeDataList(List<TubeDataType> datas, Guid hkey, Guid uid)
        {
            var result =
            datas.Select(e => new TubeData
            {
                planKey = e.planKey,
                patientKey = e.patientKey,
                regionKey = e.regionKey,
                diagnosisKey = e.diagnosisKey,
                endYear = e.endYear,
                age = e.age,
                gender = e.gender,
                blood = e.blood,
                frozenTissue = e.frozenTissue,
                waxBlock = e.waxBlock,
                urine = e.urine,
                tissueDNA = e.tissueDNA,
                tissueRNA = e.tissueRNA,
                sampleless = e.sampleless,
                pleuraleffusion = e.pleuraleffusion,
                CSF = e.CSF,
                ascites = e.ascites,
                boneMarrow = e.boneMarrow,
                serum = e.serum,
                plasma = e.plasma,
                buffyCoat = e.buffyCoat,
                paraffinSection = e.paraffinSection,
                bloodDNA = e.bloodDNA,
                CDM = e.CDM,
                questionnaire = e.questionnaire,
                CT = e.CT,
                MRI = e.MRI,
                ultrasound = e.ultrasound,
                digit_pathology_image_data = e.digit_pathology_image_data,
                DNA_WGS = e.DNA_WGS,
                DNA_WES = e.DNA_WES,
                DNA_pannel = e.DNA_pannel,
                RNA = e.RNA,
                hospitalId = hkey,
                createUser = uid,
                createDate = DateTime.Now
            }).ToList();

            return result;
        }

        public DataSaveAns BatchCreate(List<TubeDataType> datas, Guid hkey, Guid uid,bool isFirst)
        {
            DataSaveAns _DataSaveAns = new DataSaveAns();

            try
            {
                List<TubeData> adds = new List<TubeData>();
                DateTime now = DateTime.Now;
                foreach (TubeDataType data in datas)
                {
                    TubeData tube = new TubeData();
                    foreach (var info in _columns)
                    {
                        var value = data.GetType().GetProperty(info.Name).GetValue(data);
                        tube.GetType().GetProperty(info.Name).SetValue(tube, Convert.ChangeType(value, info.PropertyType), null);
                    }
                    tube.hospitalId = hkey;
                    tube.createUser = uid;
                    tube.createDate = now;

                    adds.Add(tube);
                }

                List<TubeData> old = _db.TubeData.Where(e => e.hospitalId == hkey).ToList();

                if (isFirst)
                    TubeDataToLog(old, hkey, uid);

                if(isFirst)
                    _db.TubeData.RemoveRange(old);

                _db.TubeData.AddRange(adds);
                _db.SaveChanges();

                _DataSaveAns.isSuccess = true;
            }
            catch (Exception e)
            {
                _DataSaveAns.StrMsg = e.Message;
                _DataSaveAns.StackTrace = e.StackTrace;
                _DataSaveAns.isSuccess = false;
            }

            return _DataSaveAns;
        }

        private void TubeDataToLog(List<TubeData> datas,Guid hoid,Guid uId)
        {
            List<TubeDataLog> adds = new List<TubeDataLog>();
         
            foreach (TubeData data in datas)
            {
                TubeDataLog tube = new TubeDataLog();
                foreach (var info in _columns)
                {
                    var value = data.GetType().GetProperty(info.Name).GetValue(data);
                    tube.GetType().GetProperty(info.Name).SetValue(tube, Convert.ChangeType(value, info.PropertyType), null);
                }

                tube.hospitalId = hoid;
                tube.createUser = data.createUser;
                tube.createDate = data.createDate;
                tube.age = data.age;
                tube.LogDate = DateTime.Now;

                adds.Add(tube);
            }

            int addsCursor = 0;
            int addCount = 20000;

            if (adds.AsEnumerable().Count() > addCount)
            {
                do
                {
                    List<TubeDataLog> addItem = adds.Skip(addCount * addsCursor)
                        .Take(addCount)
                        .ToList();
                    _db.TubeDataLog.AddRange(addItem);
                    addsCursor++;
                    _db.SaveChanges();

                } while (adds.AsEnumerable().Count() > addCount * addsCursor);
            }
            else
            {
                _db.TubeDataLog.AddRange(adds);
                _db.SaveChanges();
            }
            _db.Dispose();
        }

        public DataTable GetEmptyDataTable()
        {
            DataTable table = new DataTable();
             
            foreach (var item in _columns)
            {
                DataColumn column = new DataColumn();
                column.ColumnName = item.DisplayName;
                column.Caption = item.DisplayName;
                column.AllowDBNull = item.Required;
                table.Columns.Add(column);
            }

            return table;
        }

        public List<TubeData> getTubeData(Guid hospitalId)
        {
            List<TubeData> _LitTubeData = _db.TubeData.Where(e => e.hospitalId == hospitalId).ToList();

            return _LitTubeData;
        }
    }
}
