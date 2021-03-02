﻿using System;
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

    [MetadataType(typeof(TubeDataType))]
    public partial class TubeData
    {
    }
    public class TubeDataType
    {
     

        [DisplayName("識別編號")]
        [Required]
        public string patientKey { get; set; }

        [DisplayName("部位編號")]
        [Required]
        public string regionKey { get; set; }

        [DisplayName("診斷編號")]
        [Required]
        public string diagnosisKey { get; set; }

        [DisplayName("收案年份")]
        [Required]
        public string endYear { get; set; }

        [DisplayName("年齡")]
        [Required]
        public decimal age { get; set; }

        [DisplayName("性別")]
        [Required]
        public string gender { get; set; }

        [DisplayName("血液")]
        public bool blood { get; set; }

        [DisplayName("冷凍組織")]
        public bool frozenTissue { get; set; }
        [DisplayName("蠟塊")]
        public bool waxBlock { get; set; }
        [DisplayName("尿液")]
        public bool urine { get; set; }
        [DisplayName("尿液清液")]
        public bool urineClearLiquid { get; set; }
        [DisplayName("尿液顆粒")]
        public bool urinePellet { get; set; }
        [DisplayName("tissue DNA")]
        public bool tissueDNA { get; set; }
        [DisplayName("tissue RNA")]
        public bool tissueRNA { get; set; }
        [DisplayName("糞便")]
        public bool stool { get; set; }
        [DisplayName("糞便DNA")]
        public bool stoolDNA { get; set; }
        [DisplayName("沒有檢體")]
        public bool sampleless { get; set; }
        [DisplayName("胸水")]
        public bool pleuraleffusion { get; set; }
        [DisplayName("腦脊髓液")]
        public bool CSF { get; set; }
        [DisplayName("腹水")]
        public bool ascites { get; set; }
        [DisplayName("骨髓液")]
        public bool boneMarrow { get; set; }
        [DisplayName("血清")]
        public bool serum { get; set; }
        [DisplayName("血漿")]
        public bool plasma { get; set; }
        [DisplayName("buffy coat")]
        public bool buffyCoat { get; set; }
        [DisplayName("全血")]
        public bool wholeBlood { get; set; }
        [DisplayName("石蠟切片")]
        public bool paraffinSection { get; set; }
        [DisplayName("膽汁")]
        public bool bile { get; set; }
        [DisplayName("毛髮")]
        public bool hair { get; set; }
        [DisplayName("口水")]
        public bool saliva { get; set; }
       
    }
}

