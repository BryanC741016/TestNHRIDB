using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class BatchTableViewModel
    {
        public string StrBatchMsg;
        public string StrBatchMsgNext;
        public string StrCheckMsg;

        public bool isExeSetTimeOut { get; set; }

        public string StrAnsError { get; set; }
    }
}