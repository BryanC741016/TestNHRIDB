using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DbModel
{
    [MetadataType(typeof(TotalModel))]
    public partial class GetTotal_Result
    {
    }
    public class TotalModel{
        [DisplayName("病例數")]
        public Nullable<int> sumCount { get; set; }

      [DisplayName("血液")]
        public Nullable<int> sumBlood { get; set; }

 [DisplayName("血液-檢體少")]
      
        public Nullable<int> sumSampleblood { get; set; }

  [DisplayName("冷凍組織")]
    
        public Nullable<int> sumFrozenTissue { get; set; }

    [DisplayName("冷凍組織-檢體少")]
        public Nullable<int> sumSamplefrozenTissue { get; set; }

  [DisplayName("蠟塊")]
        public Nullable<int> sumWaxBlock { get; set; }
   [DisplayName("蠟塊-檢體少")]
     
        public Nullable<int> sumSamplewaxBlock { get; set; }

   [DisplayName("尿液")]  
        public Nullable<int> sumUrine { get; set; }

       [DisplayName("尿液-檢體少")]
        public Nullable<int> sumSampleurine { get; set; }

        [DisplayName("DNA")]
        public Nullable<int> sumDna { get; set; }

        [DisplayName("DNA-檢體少")]
        public Nullable<int> sumSampledna { get; set; }

        [DisplayName("糞便")]
        public Nullable<int> sumStool { get; set; }

        [DisplayName("糞便-檢體少")]
        public Nullable<int> sumSamplestool { get; set; }

        [DisplayName("沒有檢體")]
        public Nullable<int> sumSampleless { get; set; }

        [DisplayName("胸水")]
        public Nullable<int> sumPleuraleffusion { get; set; }

        [DisplayName("胸水-檢體少")]
        public Nullable<int> sumSamplepleuraleffusion { get; set; }

        [DisplayName("腦脊髓液")]
        public Nullable<int> sumCSF { get; set; }

        [DisplayName("腦脊髓液-檢體少")]
        public Nullable<int> sumSampleCSF { get; set; }

        [DisplayName("腹水")]
        public Nullable<int> sumAscites { get; set; }

        [DisplayName("腹水-檢體少")]
        public Nullable<int> sumSampleAscites { get; set; }

        [DisplayName("骨髓液")]
        public Nullable<int> sumBoneMarrow { get; set; }

        [DisplayName("骨髓液-檢體少")]
        public Nullable<int> sumSampleBoneMarrow { get; set; }

        [DisplayName("其他")]
        public Nullable<int> sumOthers_bilejuice_hair_saliva { get; set; }
    }
}
