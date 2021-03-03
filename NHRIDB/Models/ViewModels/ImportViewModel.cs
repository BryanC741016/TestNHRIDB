using NHRIDB_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class ViewDatasViewModel
    {
        public List<TubeDataType> datas { get; set; }
        public List<InfoColummns> columns { get; set; }
    }
}