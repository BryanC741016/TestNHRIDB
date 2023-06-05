using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Controllers
{
    public class BarController : BasicController
    {
        private HospitalDA _hospitalDA;
        private TubeDataTotalDA _tubeTotal;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            _hospitalDA = new HospitalDA(_db);
            _tubeTotal = new TubeDataTotalDA();
        }

        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: Bar
        public ActionResult Index(Nullable<Guid> hosId = null)
        {
            BarViewModel model = new BarViewModel();

            model.conditions.caseTimes = 1;
            model.leapProject = _leapProject;
            model.selfHos = _hospitalDA.GetHospital(_hos);
            model.conditions.hosId = model.selfHos.id;
            model.hospitalSelect = new SelectList(_hospitalDA.GetQuery().ToList(), "id", "name_tw", model.selfHos.id.ToString());
            model.genderSelect = new List<SelectListItem>
            {
                new SelectListItem { Text = "男" , Value = "M" },
                new SelectListItem { Text = "女" , Value = "F" },
                new SelectListItem { Text = "不限" , Value = string.Empty, Selected = true }
            };
            model.ageSelect = new List<SelectListItem>();
            for (int i = 1; i <= 100; i++)
            {
                model.ageSelect.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }
            model.endYearSelect = new List<SelectListItem>();
            for (int i = 2002; i <= DateTime.Now.Year; i++)
            {
                model.endYearSelect.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                });
            }
            model.caseTimesSelect = new List<SelectListItem>();
            for (int i = 1; i <= 5; i++)
            {
                model.caseTimesSelect.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }
            model.specimenSelect = new List<SelectListItem> {
                new SelectListItem { Text = "血液" , Value = "blood" , Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("blood") >= 0?true:false},
                new SelectListItem { Text = "冷凍組織" , Value = "frozenTissue", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("frozenTissue") >= 0?true:false},
                new SelectListItem { Text = "石蠟切片" , Value = "paraffinSection", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("paraffinSection") >= 0?true:false},
                new SelectListItem { Text = "尿液" , Value = "waxBlock", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("waxBlock") >= 0?true:false},
                new SelectListItem { Text = "胸水" , Value = "pleuraleffusion", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("pleuraleffusion") >= 0?true:false},
                new SelectListItem { Text = "腹水" , Value = "ascites", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("ascites") >= 0?true:false},
                new SelectListItem { Text = "骨髓液" ,Value = "boneMarrow", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("boneMarrow") >= 0?true:false},
                new SelectListItem { Text = "腦脊髓液" , Value = "CSF", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("CSF") >= 0?true:false}
            };

            if(hosId != null)
            //if (Request.Form["hosId"] != null && !string.IsNullOrEmpty(Request.Form["hosId"]))
            {
                //model.conditions.hospitalId = model.selfHos.id;
                model.conditions.hospitalId = hosId;
            }
            else
            {
                //model.conditions.hospitalId = _hos;
                model.conditions.hospitalId = model.selfHos.id;
            }

            model.datas = _tubeTotal.GetTotal(model.conditions);
            model.columns = _tubeTotal.GetColummns();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(BarViewModel model)
        {
            model.selfHos = _hospitalDA.GetHospital(_hos);
            model.hospitalSelect = new SelectList(_hospitalDA.GetQuery().ToList(), "id", "name_tw");
            model.genderSelect = new List<SelectListItem>
            {
                new SelectListItem { Text = "男" , Value = "M" },
                new SelectListItem { Text = "女" , Value = "F" },
                new SelectListItem { Text = "不限" , Value = string.Empty, Selected = true }
            };
            model.ageSelect = new List<SelectListItem>();
            for (int i = 1; i <= 100; i++)
            {
                model.ageSelect.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }
            model.endYearSelect = new List<SelectListItem>();
            for (int i = 2002; i <= DateTime.Now.Year; i++)
            {
                model.endYearSelect.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }
            model.caseTimesSelect = new List<SelectListItem>();
            for (int i = 1; i < 6; i++)
            {
                model.caseTimesSelect.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }
            model.specimenSelect = new List<SelectListItem> {
                new SelectListItem { Text = "血液" , Value = "blood" , Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("blood") >= 0?true:false},
                new SelectListItem { Text = "冷凍組織" , Value = "frozenTissue", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("frozenTissue") >= 0?true:false},
                new SelectListItem { Text = "石蠟切片" , Value = "paraffinSection", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("paraffinSection") >= 0?true:false},
                new SelectListItem { Text = "尿液" , Value = "waxBlock", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("waxBlock") >= 0?true:false},
                new SelectListItem { Text = "胸水" , Value = "pleuraleffusion", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("pleuraleffusion") >= 0?true:false},
                new SelectListItem { Text = "腹水" , Value = "ascites", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("ascites") >= 0?true:false},
                new SelectListItem { Text = "骨髓液" ,Value = "boneMarrow", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("boneMarrow") >= 0?true:false},
                new SelectListItem { Text = "腦脊髓液" , Value = "CSF", Selected = Request.Form["specimen"] == null? false: Request.Form["specimen"].IndexOf("CSF") >= 0?true:false}
                };
            model.conditions.blood = model.specimenSelect.Where(e => e.Value.Equals("blood")).SingleOrDefault().Selected;
            model.conditions.frozenTissue = model.specimenSelect.Where(e => e.Value.Equals("frozenTissue")).SingleOrDefault().Selected;
            model.conditions.paraffinSection = model.specimenSelect.Where(e => e.Value.Equals("paraffinSection")).SingleOrDefault().Selected;
            model.conditions.waxBlock = model.specimenSelect.Where(e => e.Value.Equals("waxBlock")).SingleOrDefault().Selected;
            model.conditions.pleuraleffusion = model.specimenSelect.Where(e => e.Value.Equals("pleuraleffusion")).SingleOrDefault().Selected;
            model.conditions.ascites = model.specimenSelect.Where(e => e.Value.Equals("ascites")).SingleOrDefault().Selected;
            model.conditions.boneMarrow = model.specimenSelect.Where(e => e.Value.Equals("boneMarrow")).SingleOrDefault().Selected;
            model.conditions.CSF = model.specimenSelect.Where(e => e.Value.Equals("CSF")).SingleOrDefault().Selected;

            if (Request.Form["hosId"] != null && !string.IsNullOrEmpty(Request.Form["hosId"]))
            {
                model.conditions.hospitalId = Guid.Parse(Request.Form["hosId"]);
            }
            model.datas = _tubeTotal.GetTotal(model.conditions);

            model.columns = _tubeTotal.GetColummns();

            return View(model);
        }
    }
}