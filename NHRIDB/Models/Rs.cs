using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MakeHTML.Models
{
    public class Rs
    {
        /// <summary>
        /// 是否成功: Y or N
        /// </summary>
        public bool isSuccess { get; set; }

        /// <summary>
        /// 成功或失敗訊息
        /// </summary>
        public string message { get; set; }

     
        public Rs()
        {

        }
    }
}