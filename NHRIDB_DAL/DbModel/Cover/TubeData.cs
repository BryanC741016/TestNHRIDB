using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(TubeDataModel))]
    public partial class TubeData
    {
    }
    public class TubeDataModel
    {
        public System.Guid hospitalId { get; set; }

        [DisplayName("識別ID")]
        public string patientKey { get; set; }
        public string regionKey { get; set; }
        public string diagnosisKey { get; set; }
        public string endYear { get; set; }
        public decimal age { get; set; }
        public string gender { get; set; }
        public bool blood { get; set; }
        public bool sampleblood { get; set; }
        public bool frozenTissue { get; set; }
        public bool samplefrozenTissue { get; set; }
        public bool waxBlock { get; set; }
        public bool samplewaxBlock { get; set; }
        public bool urine { get; set; }
        public bool sampleurine { get; set; }
        public bool dna { get; set; }
        public bool sampledna { get; set; }
        public bool stool { get; set; }
        public bool samplestool { get; set; }
        public bool sampleless { get; set; }
        public bool pleuraleffusion { get; set; }
        public bool samplepleuraleffusion { get; set; }
        public bool CSF { get; set; }
        public bool sampleCSF { get; set; }
        public bool ascites { get; set; }
        public bool sampleAscites { get; set; }
        public bool boneMarrow { get; set; }
        public bool sampleBoneMarrow { get; set; }
        public bool others_bilejuice_hair_saliva { get; set; }
        public System.Guid createUser { get; set; }
        public System.DateTime createDate { get; set; }

        public virtual Hospital Hospital { get; set; }
    }
}

