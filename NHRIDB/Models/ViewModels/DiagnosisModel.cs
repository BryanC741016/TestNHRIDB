using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Models.ViewModels
{
    public class DiagnosisModel : PageModel { 
      public List<Diagnosis> items { get; set; }
        public string dKey { get; set; }
         
        public string name_tw { get; set; }
         
        public string name_en { get; set; }
    }


    public class DiagnosisCreateModel
    {
        public string type { get; set; }
         
        [Display(Name = "編號")]
        [Required]
        public string dKey { get; set; }

        [Display(Name = "中文名稱")]
        public string name_tw { get; set; }

        [Required]
        [Display(Name = "英文名稱")]
        public string name_en { get; set; }

       
        public List<SelectListItem> rList { get; set; }

        [Required]
        [Display(Name = "器官編號")]
        public List<string> checks { get; set; }
    }
}