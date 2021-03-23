using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class ProjectSetViewModel
    {
        [Required]
        [Display(Name = "開始填寫時間")]
        public DateTime? startDate { get; set; }

        [Required]
        [Display(Name = "結束填寫時間")]
        public DateTime? endDate { get; set; }

        [Required]
        [Display(Name = "密碼強度Regex")]
        public string regex { get; set; }

       
        [Display(Name = "Regex訊息說明")]
        public string regexMsg { get; set; }

        [Required]
        [Display(Name = "登入錯誤幾次將被鎖")]
        public int errorOutCount { get; set; }

        
    }
}