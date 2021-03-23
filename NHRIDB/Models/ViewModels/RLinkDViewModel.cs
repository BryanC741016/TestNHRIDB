using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Models.ViewModels
{
    public class RLinkDViewModel : PageModel
    {
        [Display(Name = "部位英文名稱")]
        public string searchRname { get; set; }

        [Display(Name = "診斷英文名稱")]
        public string searchDname { get; set; }

        [Display(Name = "部位編號")]
        public string searchRkey { get; set; }

        [Display(Name = "診斷編號")]
        public string searchDkey { get; set; }

        public List<RLinkDItem> items { get; set; }
    }
    public class RLinkDItem:RLinkD
    { 
    }
    public class RLinkDCreate {
        [Required]
        [Display(Name = "部位編號")]
        public string regionKey { get; set; }

        [Required]
        [Display(Name = "診斷編號")]
        public string diagnosisKey { get; set; }

        [Required]
        [Display(Name = "部位英文名稱")]
        public string rName { get; set; }

        [Required]
        [Display(Name = "診斷英文名稱")]
        public string dName { get; set; }
    }


    public class RLinkDEdit
    {
        [Required]
        [Display(Name = "部位編號")]
        public string regionKey { get; set; }

        [Required]
        [Display(Name = "診斷編號")]
        public string diagnosisKey { get; set; }

        

        [Required]
        [Display(Name = "部位英文名稱")]
        public string rName { get; set; }

        [Required]
        [Display(Name = "診斷英文名稱")]
        public string dName { get; set; }
    }

}