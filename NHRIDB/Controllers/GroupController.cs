using MakeHTML.Models;
using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Controllers
{
    public class GroupController : BasicController
    {
        private GroupDA _groupDA;
        private List<MenuName> _menu;
        private PermissionsDA _permissionsDA;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _groupDA = new GroupDA(_db);
            MenuDA menuDa = new MenuDA(_db);
            _menu= menuDa.GetQuery().ToList();
             _permissionsDA = new PermissionsDA(_db);
        }
        // GET: Group
        public ActionResult Index()
        {
            GroupViewModel model = new GroupViewModel();
            model.items = _groupDA.GetQuery()
                           .Select(e=>new GroupItems { 
                             gId=e.groupId,
                             gName=e.gName
                           })
                         .ToList();
          
            return View(model);
        }

        [HttpGet]
        public ActionResult Create() {
            CreateGroup model = new CreateGroup();
            model.menu = _menu.Select(e=>new PurviewModel
            { 
                menuId=e.menuId,
              controllName=e.controllerName,
              menuText=e.menuText,
              parentMenu=e.parentMenu,
              purview= PermissionsKind.None,
              sortIndex=e.sortIndex
            }).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateGroup model) {
            model.menu = _menu.Select(e => new PurviewModel
            {
                menuId = e.menuId,
                controllName = e.controllerName,
                menuText = e.menuText,
                parentMenu = e.parentMenu,
                purview = PermissionsKind.None,
                sortIndex=e.sortIndex
            }).ToList();

            if (!ModelState.IsValid) {
                return View(model);
            }
           int count=  _groupDA.GetQuery(model.gName).Count();
            if (count > 0) {
                ModelState.AddModelError(string.Empty, "此名稱已被使用過");
                return View(model);
            }
         Guid gid=   _groupDA.Create(model.gName, (int)model.leapProject, (int)model.alwaysOpen);
         List<Permissions> adds=   model.setMenu.Select(e => new Permissions
            {
              groupId=gid,
             menuId= e.menuId,
              purview=(short)e.purview
            }).ToList();
            _permissionsDA.AddRange(adds);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            GroupUser group = _groupDA.GetGroupUser(id);
            EditGroup model = new EditGroup();
            model.gId = id;
            model.gName = group.gName;
            model.leapProject = group.leapProject? YesOrNo.Yes : YesOrNo.NO;
            model.alwaysOpen= group.alwaysOpen ? YesOrNo.Yes : YesOrNo.NO;
            List<PurviewModel> purviewModels= _menu.Select(e => new PurviewModel
            {
                menuId = e.menuId,
                controllName = e.controllerName,
                menuText = e.menuText,
                parentMenu = e.parentMenu,
                purview = PermissionsKind.None,
                sortIndex=e.sortIndex
            }).ToList();
            IQueryable<Permissions> list= _permissionsDA.GetQuery(gid: id);
            foreach (Permissions item in list) {
                PurviewModel purview = purviewModels.Where(e => e.controllName!=null && e.menuId==item.menuId)
                    .SingleOrDefault();
                purview.purview = (PermissionsKind)item.purview;
            }

            model.menu = purviewModels;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditGroup model)
        {
            List<PurviewModel> purviewModels = _menu.Select(e => new PurviewModel
            {
                menuId = e.menuId,
                controllName = e.controllerName,
                menuText = e.menuText,
                parentMenu = e.parentMenu,
                purview = PermissionsKind.None,
                sortIndex=e.sortIndex
            }).ToList();
            IQueryable<Permissions> list = _permissionsDA.GetQuery(gid: model.gId);
            foreach (Permissions item in list)
            {
                PurviewModel purview = purviewModels.Where(e => e.controllName != null && e.menuId==item.menuId)
                    .SingleOrDefault();
                purview.purview = (PermissionsKind)item.purview;
            }
            model.menu = purviewModels;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            int count = _groupDA.GetQuery(gName:model.gName,noID:model.gId).Count();
            if (count > 0)
            {
                ModelState.AddModelError(string.Empty, "此名稱已被使用過");
                return View(model);
            }
            _groupDA.Edit(model.gId,model.gName, (int)model.leapProject, (int)model.alwaysOpen);
            List<Permissions> adds = model.setMenu.Select(e => new Permissions
            {
                groupId = model.gId,
                menuId = e.menuId,
                purview = (short)e.purview
            }).ToList();

            _permissionsDA.UpdateRange(model.gId,adds);
            return RedirectToAction("Index");
        }

        [AjaxValidateAntiForgeryToken]
        public JsonResult Delete(Guid id)
        {
            Rs rs = new Rs();
            rs.isSuccess = false;
            GroupUser hos = _groupDA.GetGroupUser(id);
            if (hos == null)
            {
                rs.message = "查無此資料";
                return Json(rs);
            }

            _groupDA.Delete(id);
            _permissionsDA.Delete(id);
            rs.isSuccess = true;
            return Json(rs);
        }
    }
}