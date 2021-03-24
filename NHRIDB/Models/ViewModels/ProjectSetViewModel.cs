using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml;

namespace NHRIDB.Models.ViewModels
{
    public class ProjectSet
    {
       
        public DateTime startDate { get; set; }

        
        public DateTime endDate { get; set; }

        [Required]
        [Display(Name = "密碼強度Regex")]
        public string regex { get; set; }


        [Display(Name = "Regex訊息說明")]
        public string regexMsg { get; set; }

        [Required]
        [Display(Name = "登入錯誤幾次將被鎖")]
        public int errorOutCount { get; set; }

        public ProjectSet(){
        }
        public ProjectSet(string path) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode root = xmlDoc.SelectSingleNode("set");
            string startDateXML = root.SelectSingleNode("startDate").InnerText;
            string endDateXML = root.SelectSingleNode("endDate").InnerText;
            string regex = root.SelectSingleNode("regex").InnerText;
            string regexMsg = root.SelectSingleNode("regexMsg").InnerText;
            string errorOutCount = root.SelectSingleNode("errorOutCount").InnerText;
            
           this.endDate = DateTime.Parse(endDateXML);
             this.startDate = DateTime.Parse(startDateXML);
             
            this.regex = regex;
            this.errorOutCount = string.IsNullOrEmpty(errorOutCount) ? 0 : int.Parse(errorOutCount);
            this.regexMsg = regexMsg;
        }
    }

    public class ProjectSetViewModel : ProjectSet
    {
        public ProjectSetViewModel(string path) : base(path) {
            this.startDate = base.startDate;
            this.endDate = base.endDate;
        }

        public ProjectSetViewModel():base(){}

        [Required]
        [Display(Name = "開始填寫時間")]
        public new DateTime? startDate { get; set; }

        [Required]
        [Display(Name = "結束填寫時間")]
        public new DateTime? endDate { get; set; }
    }



}