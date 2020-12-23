using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Controllers
{
    public class UpdateController : BasicController
    {
        // GET: Update
        public ActionResult UpdateImg()
        {
           List<Hospital> hos= _db.Hospital.ToList();
            foreach (var ho in hos) {
                string oldPath = Path.Combine(_imgDirPath, ho.name_en+".jpg");
                string newPath = Path.Combine(_imgDirPath, ho.id.ToString() + ".jpg");
                if (System.IO.File.Exists(oldPath)) {
                    System.IO.File.Move(oldPath, newPath);
                
                }
            }
            return View();
        }
    }
}