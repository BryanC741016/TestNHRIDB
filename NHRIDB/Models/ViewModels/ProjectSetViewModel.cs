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
        [Display(Name = "開始時間")]
        public DateTime? startDate { get; set; }

        [Required]
        [Display(Name = "結束時間")]
        public DateTime? endDate { get; set; }
    }
}