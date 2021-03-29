﻿using NHRIDB_DAL.DbModel;
using NHRIDB_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Models.ViewModels
{
    public class ImportViewModel {
        public SelectList hospitalSelect { get; set; }
    }
    public class ViewDatasViewModel
    {
        public Guid hId { get; set; }
        public List<TubeDataType> datas { get; set; }
        public List<InfoColummns> columns { get; set; }
        public string fileName { get; set; }
    }
    public class DiffViewModel {
        public List<GetDifferentTotal_Result> datas { get; set; }
        public List<InfoColummns> columns { get; set; }
    }
}