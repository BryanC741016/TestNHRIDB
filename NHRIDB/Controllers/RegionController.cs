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
    public class RegionController : BasicController
    {
        private RegionDA _nodeDA;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _nodeDA = new RegionDA(_db);
        }

        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: Form
        public ActionResult Index()
        {
            RegionIndexModel model = new RegionIndexModel();
            Node root = new Node();
            Tree(null, root);
            model.json = root;
            return View(model);
        }

        [MvcAdminRightAuthorizeFilter(param = 'r')]
        private Node Tree(Nullable<Guid> parentId, Node node)
        {
            IQueryable<Region> childe = _nodeDA.GetChild(parentId);
            node.children = childe.Select(e => new Node
            {
                id = e.regionId,
                parentId = e.parentId,
                text = e.name_en,
                type = "node"
            }).ToList();
            if (node.children.Count() == 0)
            {
                node.type = "leaf";
            }
            foreach (Node item in node.children)
            {
                Tree(item.id, item);
            }

            return node;
        }
    }
}