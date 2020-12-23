using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHRIDB.Models
{
    public class PageModel
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }

        public int pageEnd { get; set; }

        public string sortColumn { get; set; }

        public string sortType { get; set; }

        public PageModel() {
            pageSize = 2;
            pageNumber = 1;
            pageEnd = 1;
            sortType = "asc";
        }
        
      

        public int getCurr() {
          return  (pageNumber - 1) * pageSize;
        }

        public void setPageEnd(double count) {
            pageEnd = Convert.ToInt16(Math.Ceiling(count / pageSize));
            if (pageEnd == 0)
                pageEnd = 1;
        }

        public void setData(int pageNumber, string sortColumn, string sortType) {
            if (!string.IsNullOrEmpty(sortColumn))
            {
                this.sortColumn = sortColumn;
            }


            if (!string.IsNullOrEmpty(sortType))
            {
                this.sortType = sortType;
            }



            this.pageNumber = pageNumber;
        }

        public List<T> GetSortColumnList<T>(List<T> lists,string defaultSortColumn){
            if (string.IsNullOrEmpty(sortColumn)) {
                sortColumn = defaultSortColumn;
            }
            
            switch (this.sortType) {
                case "desc":
                    var propertyInfo = typeof(T).GetProperty(this.sortColumn);
                    lists = lists.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
                    break;
                case "asc":
                    var propertyInfox = typeof(T).GetProperty(this.sortColumn);
                    lists = lists.OrderBy(x => propertyInfox.GetValue(x, null)).ToList();
                    break;
                default:
                    break;
            }

           setPageEnd(lists.Count());
           this.pageNumber = pageNumber;

            lists = lists.Skip(getCurr())
                  .Take(pageSize).ToList();

            return lists;
        }
    }
}