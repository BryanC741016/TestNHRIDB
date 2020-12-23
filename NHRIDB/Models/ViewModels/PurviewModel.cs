using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public enum PermissionsKind
    {
        [Display(Name = "無權限")]
        None = 0,
        [Display(Name = "唯讀")]
        Read = 1,
        [Display(Name = "可編輯")]
        Write = 2,
    }
    public class PurviewModel
    {
        public int menuId { get; set; }
        public string controllName { get; set; }
        public string menuText { get; set; }
        public PermissionsKind purview { get; set; }

        public int? parentMenu { get; set; }

        public int sortIndex { get; set; }
    }
}