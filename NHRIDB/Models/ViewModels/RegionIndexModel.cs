﻿using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class RegionViewModel:PageModel { 
      public List<Region> items { get; set; }
        public string rKey { get; set; }
         
        public string name_tw { get; set; }
         
        public string name_en { get; set; }
    }


    public class RegionCreateModel {
        public string type { get; set; }
         
        [Display(Name = "編號")]
        [Required]
        public string rKey { get; set; }

        [Display(Name = "中文名稱")]
        public string name_tw { get; set; }

        [Required]
        [Display(Name = "英文名稱")]
        public string name_en { get; set; }
    }
}