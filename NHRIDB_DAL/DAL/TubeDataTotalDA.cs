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
    public class TubeDataTotalDA : DataAccess
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

        public List<GetTotal_Result> GetTotal(BarTubeDataType model)
        {
            if (!model.hosId.HasValue)
            {
                var para = _db.GetTotal(
                    model.hosId?.ToString(),
                    model.hospitalId?.ToString(),
                    model.regionKey,
                    model.diagnosisKey,
                    model.endYearSt,
                    model.endYearEd,
                    model.ageSt?.ToString(),
                    model.ageEt?.ToString(),
                    model.gender,
                    model.blood?model.blood:(bool?)null,
                    model.frozenTissue? model.frozenTissue:(bool?)null,
                    model.paraffinSection? model.paraffinSection:(bool?) null,
                    model.urine? model.urine :(bool?) null,
                    model.paraffinSection? model.paraffinSection :(bool?) null,
                    model.ascites? model.ascites :(bool?) null,
                    model.boneMarrow? model.boneMarrow :(bool?) null,
                    model.CSF? model.CSF :(bool?) null,
                    model.caseTimes
                    )
                    .Select(e => new GetTotal_Result
                    {
                        rId = e.rId,
                        dId = e.dId,
                        rName = e.rName,
                        dName = e.dName,
                        sum_blood = e.sum_blood,
                        sum_frozenTissue = e.sum_frozenTissue,
                        sum_waxBlock = e.sum_waxBlock,
                        sum_urine = e.sum_urine,
                        sum_urineClearLiquid = e.sum_urineClearLiquid,
                        sum_urinePellet = e.sum_urinePellet,
                        sum_tissueDNA = e.sum_tissueDNA,
                        sum_tissueRNA = e.sum_tissueRNA,
                        sum_stool = e.sum_stool,
                        sum_stoolDNA = e.sum_stoolDNA,
                        sum_sampleless = e.sum_sampleless,
                        sum_pleuraleffusion = e.sum_pleuraleffusion,
                        sum_CSF = e.sum_CSF,
                        sum_ascites = e.sum_ascites,
                        sum_boneMarrow = e.sum_boneMarrow,
                        sum_serum = e.sum_serum,
                        sum_plasma = e.sum_plasma,
                        sum_buffyCoat = e.sum_buffyCoat,
                        sum_wholeBlood = e.sum_wholeBlood,
                        sum_paraffinSection = e.sum_paraffinSection,
                        sum_bile = e.sum_bile,
                        sum_hair = e.sum_hair,
                        sum_saliva = e.sum_saliva,
                        sum_bloodDNA = e.sum_bloodDNA,
                        sum_CDM = e.sum_CDM,
                        sum_questionnaire = e.sum_questionnaire,
                        sum_CT = e.sum_CT,
                        sum_MRI = e.sum_MRI,
                        sum_ultrasound = e.sum_ultrasound,
                        sum_digit_pathology_image_data = e.sum_digit_pathology_image_data,
                        sum_DNA_WGS = e.sum_DNA_WGS,
                        sum_DNA_WES = e.sum_DNA_WES,
                        sum_DNA_pannel = e.sum_DNA_pannel,
                        sum_RNA = e.sum_RNA
                    })
                    .ToList();
                return para;
            }
            else
            {
                var para =
                _db.GetTotal(
                    model.hosId?.ToString(),
                    model.hospitalId?.ToString(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    model.caseTimes
                    )
                    .Select(e => new GetTotal_Result
                    {
                        rId = e.rId,
                        dId = e.dId,
                        rName = e.rName,
                        dName = e.dName,
                        sum_blood = e.sum_blood,
                        sum_frozenTissue = e.sum_frozenTissue,
                        sum_waxBlock = e.sum_waxBlock,
                        sum_urine = e.sum_urine,
                        sum_urineClearLiquid = e.sum_urineClearLiquid,
                        sum_urinePellet = e.sum_urinePellet,
                        sum_tissueDNA = e.sum_tissueDNA,
                        sum_tissueRNA = e.sum_tissueRNA,
                        sum_stool = e.sum_stool,
                        sum_stoolDNA = e.sum_stoolDNA,
                        sum_sampleless = e.sum_sampleless,
                        sum_pleuraleffusion = e.sum_pleuraleffusion,
                        sum_CSF = e.sum_CSF,
                        sum_ascites = e.sum_ascites,
                        sum_boneMarrow = e.sum_boneMarrow,
                        sum_serum = e.sum_serum,
                        sum_plasma = e.sum_plasma,
                        sum_buffyCoat = e.sum_buffyCoat,
                        sum_wholeBlood = e.sum_wholeBlood,
                        sum_paraffinSection = e.sum_paraffinSection,
                        sum_bile = e.sum_bile,
                        sum_hair = e.sum_hair,
                        sum_saliva = e.sum_saliva,
                        sum_bloodDNA = e.sum_bloodDNA,
                        sum_CDM = e.sum_CDM,
                        sum_questionnaire = e.sum_questionnaire,
                        sum_CT = e.sum_CT,
                        sum_MRI = e.sum_MRI,
                        sum_ultrasound = e.sum_ultrasound,
                        sum_digit_pathology_image_data = e.sum_digit_pathology_image_data,
                        sum_DNA_WGS = e.sum_DNA_WGS,
                        sum_DNA_WES = e.sum_DNA_WES,
                        sum_DNA_pannel = e.sum_DNA_pannel,
                        sum_RNA = e.sum_RNA
                    })
                    .ToList();
                return para;                
                //.ToList();
            }

        }

        public List<GetDifferentTotal_Result> GetDifferent(Guid hospitalId)
        {
            return _db.GetDifferentTotal(hospitalId).ToList();
        }

    }
    public class BarTubeDataType : TubeDataType
    {
        [DisplayName("機構代號")]
        public Nullable<Guid> hospitalId { get; set; }

        public Nullable<Guid> hosId { get; set; }

        [DisplayName("收案年份 (西元年)起")]
        [Required]
        public string endYearSt { get; set; }

        [DisplayName("收案年份 (西元年)迄")]
        [Required]
        public string endYearEd { get; set; }

        [DisplayName("收案時年齡 (歲)起")]
        [Required]
        public string ageSt { get; set; }

        [DisplayName("收案時年齡 (歲)迄")]
        [Required]
        public string ageEt { get; set; }
        [DisplayName("收案次數")]
        [Required]
        public int caseTimes { get; set; }

    }
}
