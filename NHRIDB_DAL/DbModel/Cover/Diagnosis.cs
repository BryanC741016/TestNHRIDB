using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DbModel
{
    [MetadataType(typeof(DiagnosisType))]
    public partial class Diagnosis
    {
    }
    public class DiagnosisType
    {
        [DisplayName("診斷編號")]
        public string diagnosisKey { get; set; }
        public string dname_tw { get; set; }

        [DisplayName("診斷名稱")]
        public string dname_en { get; set; }

       
    }
}
