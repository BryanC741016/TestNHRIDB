using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class AccountViewModel
    {
        public string email { get; set; }
        public string passwd { get; set; }
        public string newpasswd { get; set; }
        public string repasswd { get; set; }

        public string name { get; set; }

        public string msg { get; set; }
    }

    public class ChangePasswd
    {
        [Required]
        //[DataType(DataType.Password)]
        public string passwd { get; set; }

        [Required]
        [Display(Name = "密碼")]
        //[DataType(DataType.Password)]
        //[MinLength(6, ErrorMessage = "密碼最少6個字")]
        public string newpasswd { get; set; }

        [Required]
        //[DataType(DataType.Password)]
        public string repasswd { get; set; }

    }

    public class ChangeData{
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string name { get; set; }
    }
}