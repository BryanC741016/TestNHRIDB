using NHRIDB_DAL.DbModel;
using NHRIDB_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Models.ViewModels
{
    public class BarViewModel
    {
       
        public Nullable<Guid> hosId { get; set; }

        public bool leapProject { get; set; }
        public Hospital selfHos { get; set; }
        public SelectList hospitalSelect { get; set; }

        public List<GetTotal_Result> datas { get; set; }

        public List<InfoColummns> columns { get; set; }
    }

    


}