using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
    public class TemplateExcelUpDataRecordDA : DataAccess
    {
        public void Create()
        {
            List<TemplateExcelUpDataRecord> old = _db.TemplateExcelUpDataRecord.Where(m=>m.id!=null).ToList();
            _db.TemplateExcelUpDataRecord.RemoveRange(old);

            List<TemplateExcelUpDataRecord> Add = new List<TemplateExcelUpDataRecord>();
            Add.Add(new TemplateExcelUpDataRecord() { UpDateTime=DateTime.Now});
            _db.TemplateExcelUpDataRecord.AddRange(Add);

            _db.SaveChanges();
        }

        public List<TemplateExcelUpDataRecord> getAllList()
        {
            List<TemplateExcelUpDataRecord> old = _db.TemplateExcelUpDataRecord.Where(m => m.id != null).ToList();

            return old;
        }
    }
}
