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

        public List<GetTotal_Result> GetTotal(Nullable<Guid> hospitalId)
        {
            if (hospitalId.HasValue)
            {
                //return _db.GetTotal(hospitalId.Value.ToString()).ToList();
                return new List<GetTotal_Result>();
            }
            else
            {
                //return _db.GetTotal("").ToList();
                return new List<GetTotal_Result>();
            }

        }

        public List<GetDifferentTotal_Result> GetDifferent(Guid hospitalId)
        {       
                return _db.GetDifferentTotal(hospitalId).ToList();
        }

    }
}
