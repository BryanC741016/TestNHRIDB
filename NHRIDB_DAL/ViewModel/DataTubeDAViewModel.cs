using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.ViewModel
{
    public class InfoColummns { 
       public string Name { get; set; }
       public string DisplayName { get; set; }
       public bool Required { get; set; }

       public Type PropertyType { get; set; }
    }
    public class TubeDataType
    {
        // Bryanc 2022/5/9
        [DisplayName("計畫代碼")]
        //[Required]
        public string planKey { get; set; }

        [DisplayName("個案代碼")]
        [Required]
        public string patientKey { get; set; }

        [DisplayName("器官/部位代碼")]
        [Required]
        public string regionKey { get; set; }

        [DisplayName("診斷代碼")]
        [Required]
        public string diagnosisKey { get; set; }

        [DisplayName("收案年份 (西元年)")]
        [Required]
        public string endYear { get; set; }

        [DisplayName("年齡 (歲)")]
        [Required]
        public decimal age { get; set; }

        [DisplayName("性別")]
        [Required]
        public string gender { get; set; }

        [DisplayName("沒有收到檢體")]
        public bool sampleless { get; set; }

        [DisplayName("血液")]
        public bool blood { get; set; }

        [DisplayName("血清")]
        public bool serum { get; set; }

        [DisplayName("血漿")]
        public bool plasma { get; set; }

        [DisplayName("白血球層buffy coat")]
        public bool buffyCoat { get; set; }

         [DisplayName("血液DNA")]
         public bool bloodDNA { get; set; }

        [DisplayName("冷凍組織")]
        public bool frozenTissue { get; set; }

        [DisplayName("組織DNA")]
        public bool tissueDNA { get; set; }

        [DisplayName("組織 RNA")]
        public bool tissueRNA { get; set; }

        [DisplayName("蠟塊")]
        public bool waxBlock { get; set; }

        [DisplayName("石蠟切片")]
        public bool paraffinSection { get; set; }

        [DisplayName("尿液")]
        public bool urine { get; set; }

        [DisplayName("胸水")]
        public bool pleuraleffusion { get; set; }

        [DisplayName("腹水")]
        public bool ascites { get; set; }

        [DisplayName("骨髓液")]
        public bool boneMarrow { get; set; }

        [DisplayName("腦脊髓液")]
        public bool CSF { get; set; }

        #region Bryanc 2022/4/28
        [DisplayName("醫療數據 (CDM)")]
        public bool CDM { get; set; }

        [DisplayName("問卷")]
        public bool questionnaire { get; set; }

        [DisplayName("醫療影像-CT")]
        public bool CT { get; set; }

        [DisplayName("醫療影像-MRI")]
        public bool MRI { get; set; }

        [DisplayName("醫療影像-超音波")]
        public bool ultrasound { get; set; }

        [DisplayName("數位病理影像資料")]
        public bool digit_pathology_image_data { get; set; }

        [DisplayName("DNA 基因定序數據 1: WGS")]
        public bool DNA_WGS { get; set; }

        [DisplayName("DNA 基因定序數據 2: WES")]
        public bool DNA_WES { get; set; }

        [DisplayName("DNA 基因定序數據 3: pannel")]
        public bool DNA_pannel { get; set; }

        [DisplayName("RNA 基因定序數據")]
        public bool RNA { get; set; }
        #endregion
    }

    public class TubeDataTotalType
    {         
        [DisplayName("沒有收到檢體")]
        public bool sum_sampleless { get; set; }

        [DisplayName("血液")]
        public bool sum_blood { get; set; }

        [DisplayName("血清")]
        public bool sum_serum { get; set; }

        [DisplayName("血漿")]
        public bool sum_plasma { get; set; }

        [DisplayName("白血球層buffy coat")]
        public bool sum_buffyCoat { get; set; }

        [DisplayName("血液DNA")]
        public bool sum_bloodDNA { get; set; }

        [DisplayName("冷凍組織")]
        public bool sum_frozenTissue { get; set; }

        [DisplayName("組織DNA")]
        public bool sum_tissueDNA { get; set; }

        [DisplayName("組織 RNA")]
        public bool sum_tissueRNA { get; set; }

        [DisplayName("蠟塊")]
        public bool sum_waxBlock { get; set; }

        [DisplayName("石蠟切片")]
        public bool sum_paraffinSection { get; set; }

        [DisplayName("尿液")]
        public bool sum_urine { get; set; }

        [DisplayName("胸水")]
        public bool sum_pleuraleffusion { get; set; }

        [DisplayName("腹水")]
        public bool sum_ascites { get; set; }

        [DisplayName("骨髓液")]
        public bool sum_boneMarrow { get; set; }

        [DisplayName("腦脊髓液")]
        public bool sum_CSF { get; set; }

        #region Bryanc 2022/4/28
        [DisplayName("醫療數據 (CDM)")]
        public bool sum_CDM { get; set; }

        [DisplayName("問卷")]
        public bool sum_questionnaire { get; set; }

        [DisplayName("醫療影像-CT")]
        public bool sum_CT { get; set; }

        [DisplayName("醫療影像-MRI")]
        public bool sum_MRI { get; set; }

        [DisplayName("醫療影像-超音波")]
        public bool sum_ultrasound { get; set; }

        [DisplayName("數位病理影像資料")]
        public bool sum_digit_pathology_image_data { get; set; }

        [DisplayName("DNA 基因定序數據 1: WGS")]
        public bool sum_DNA_WGS { get; set; }

        [DisplayName("DNA 基因定序數據 2: WES")]
        public bool sum_DNA_WES { get; set; }

        [DisplayName("DNA 基因定序數據 3: pannel")]
        public bool sum_DNA_pannel { get; set; }

        [DisplayName("RNA 基因定序數據")]
        public bool sum_RNA { get; set; }
        #endregion
    }
}
