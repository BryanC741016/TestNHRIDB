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

        [Required]
        [Display(Name = "醫院代號")]
        public string hkey { get; set; }

        public string imgUrl { get; set; }
    }
    public class HospitalUploadViewModel
    {
        public List<HospitalUploadlist> items { get; set; }
    }
        public class HospitalUploadlist
    {
        [Required]
        [Display(Name = "醫療機構代號")]
        public Guid HospitalId { get; set; }

        [Required]
        [Display(Name = "英文名稱")]
        public string name_en { get; set; }

        [Required]
        [Display(Name = "中文名稱")]
        public string name_tw { get; set; }
        [Display(Name = "檔案筆數")]
        public decimal fileUploadCount { get; set; }
        [Display(Name = "是否有筆數")]
        public bool fileUploadHasRow { get; set; }
        [Display(Name = "最後上傳時間")]
        public DateTime? fileUploadLastDate { get; set; }
        [Display(Name = "筆數")]
        public decimal Count { get; set; }
        [Display(Name = "是否有筆數")]
        public bool HasRow { get; set; }
        [Display(Name = "最後上傳時間")]
        public DateTime? LastDate { get; set; }
    }
    public class HospitalPieItem
    {
        public string label { get; set; }
        public int value { get; set; }
    }
    public class HospitalPie
    {
        public string tName { get; set; }
        public string qContext { get; set; }
        public Guid qId { get; set; }


        public int N { get; set; }
        public int notfillinN { get; set; }

        public List<HospitalPieItem> items { get; set; }

        public string type { get; set; }

    }
    public class HospitalJson : MakeHTML.Models.Rs
    {
        public HospitalPie hospitalPie { get; set; }
    }
}