using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class LoginViewModel
    {
        
        public string message { get; set; }
        [Required]
        [Display(Name = "帳號")]
        public string userName { get; set; }
        [Required]
        [Display(Name = "密碼")]
        public string passwd { get; set; }
        public List<string> imgUrl { get; set; }

        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public string tonken { get; set; }
        public string textshowCode { get; set; }

        public bool isLock { get; set; }
    }
}