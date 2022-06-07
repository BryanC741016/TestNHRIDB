using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class PlanViewModel : PageModel
    {
        [Display(Name = "計畫代碼")]
        public string planKey { get; set; }

        [Display(Name = "計畫名稱")]
        public string planName { get; set; }

        [Display(Name = "備註")]
        public string Remark { get; set; }

        public List<PlanItem> items { get; set; }
    }

    public class PlanItem : Plan
    {
    }

    public class PlanCreate
    {
        [Required]
        [Display(Name = "計畫代碼")]
        public string planKey { get; set; }

        [Required]
        [Display(Name = "計畫名稱")]
        public string planName { get; set; }
        
        [Display(Name = "備註")]
        public string Remark { get; set; }
    }

    public class PlanEdit
    {
        [Required]
        [Display(Name = "計畫代碼")]
        public string planKey { get; set; }

        [Required]
        [Display(Name = "計畫名稱")]
        public string planName { get; set; }

        [Display(Name = "備註")]
        public string Remark { get; set; }
    }
}