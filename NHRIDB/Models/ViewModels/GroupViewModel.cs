using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class GroupViewModel
    {
       public List<GroupItems> items { get; set; }
    }
    public class GroupItems { 
      public Guid gId { get; set; }
        public string gName { get; set; }
       
    }

    public enum YesOrNo {
        [Display(Name = "否")]
        NO=0,
        [Display(Name = "是")]
        Yes=1
    }

   
    public class CreateGroup {
        public YesOrNo leapProject { get; set; }
        public YesOrNo alwaysOpen { get; set; }

        [Required]
        [Display(Name = "群組名稱")]
        public string gName { get; set; }

        public List<PurviewModel> menu { get; set; }
        public List<PurviewModel> setMenu { get; set; }
    }

    public class EditGroup : CreateGroup { 
      public Guid gId { get; set; }
    }
}