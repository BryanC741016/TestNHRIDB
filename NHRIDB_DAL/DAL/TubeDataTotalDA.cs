using NHRIDB_DAL.DbModel;
using NHRIDB_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
  public  class TubeDataTotalDA : DataAccess
    {
      
        public List<InfoColummns> GetColummns()
        {
            return TypeDescriptor.GetProperties(typeof(TubeDataTotalType))
              .Cast<PropertyDescriptor>()
              .Select(e => new InfoColummns
              {
                  Name = e.Name,
                  DisplayName = e.DisplayName,
                  Required = e.Attributes.Cast<Attribute>().Any(a => a.GetType() == typeof(RequiredAttribute)),
                  PropertyType = e.PropertyType
              })
              .ToList();
        }

        public List<TubeDataTotal_Result> GetQuery(Nullable<Guid> hospitalId) {
            if (hospitalId.HasValue)
            {
                return _db.TubeDataTotal(hospitalId.Value.ToString()).ToList();
            }
            else {
                return _db.TubeDataTotal("").ToList();
            }
          
        }
    }
}
