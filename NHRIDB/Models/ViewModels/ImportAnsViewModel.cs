using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class ImportAnsViewModel
    {
        public int OldCount { get; set; }

        public int NewCount { get; set; }

        public string StrMesage { get; set; }

        public string StrisSuccess { get; set; }
    }
}