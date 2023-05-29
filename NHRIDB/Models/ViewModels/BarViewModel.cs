using NHRIDB_DAL.DbModel;
using NHRIDB_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

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
        public List<SelectListItem> specimenSelect { get; set; }
        public List<SelectListItem> genderSelect { get; set; }
        public List<SelectListItem> ageSelect { get; set; }
        public List<SelectListItem> ageStSelect { get; set; }
        public List<SelectListItem> endYearSelect { get; set; }
        public List<SelectListItem> caseTimesSelect { get; set; }
        public NHRIDB_DAL.DAL.BarTubeDataType conditions { get; set; }
        public BarViewModel()
        {
            conditions = new NHRIDB_DAL.DAL.BarTubeDataType();
        }
    }



}