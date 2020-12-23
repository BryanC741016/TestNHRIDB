using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class HospitalViewModel:PageModel
    {
        public string searchText { get; set; }

        public List<HospitalItem> items { get; set; }
    }
    public class HospitalItem
    {
        public Guid id { get; set; }
        public string name_en { get; set; }
        public string name_tw { get; set; }
    }

    public class HospitalDetail {
        public Nullable<Guid> id { get; set; }

        [Required]
        [Display(Name = "英文名稱")]
        public string name_en { get; set; }

        [Required]
        [Display(Name = "中文名稱")]
        public string name_tw { get; set; }

        public HttpPostedFileBase img { get; set; }

        public string errorMsg {get;set;}

        public string imgUrl { get; set; }
    }
}