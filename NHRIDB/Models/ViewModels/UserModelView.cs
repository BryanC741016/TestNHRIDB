using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Models.ViewModels
{
    public class UserModelView:PageModel
    {
        public Nullable<Guid> searchHospitalID { get; set; }

        public SelectList hospitalSelect { get; set; }
        public string searchUserName { get; set; }
        public string searchEmail { get; set; }

        public List<UserItem> items { get; set; }
    }
    public class UserItem
    {
        public Guid id { get; set; }
        public string userName { get; set; }
        public string email { get; set; }

        public string hosName { get; set; }
    }

    public class UserCreate {
       
        [Required]
        [Display(Name = "帳號")]
        public string username { get; set; }

        [Required]
        [Display(Name = "密碼")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "密碼最少6個字")]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]

        public string repassword { get; set; }

        public SelectList hospitalSelect { get; set; }

        public Guid hospitalId { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        public SelectList msSelect { get; set; }

        public Guid groupId { get; set; }

        public string errorMsg { get; set; }

        [Required]
        public string name { get; set; }

    }
    public class UserEdit
    {
        public Guid uid { get; set; }

        [Required]
        [Display(Name = "帳號")]
        public string username { get; set; }

        public SelectList msSelect { get; set; }

       public Guid groupId { get; set; }

        public SelectList hospitalSelect { get; set; }


        public Guid hospitalId { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string name { get; set; }

        public string errorMsg { get; set; }

        
        public string passwd { get; set; }
       
  

        public string newpasswd { get; set; }

        
        public string repasswd { get; set; }

    }



   
}