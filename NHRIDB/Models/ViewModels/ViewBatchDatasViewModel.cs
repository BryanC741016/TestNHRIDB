using NHRIDB_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class ViewBatchDatasViewModel
    {
        public Guid hId { get; set; }
        public List<TubeDataType> datas { get; set; }
        public List<InfoColummns> columns { get; set; }
        public string fileName { get; set; }

        public string StrBatchMsgNext { get; set; }
        public string StrCheckMsg { get; set; }
        public string StrAnsError { get; set; }
        public bool isFirst { get; set; }
        public bool isEnd { get; set; }
        public bool isExeSetTimeOut { get; set; }
    }
}