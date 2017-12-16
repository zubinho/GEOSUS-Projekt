using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GEOSUS_Projekt.Models;

namespace GEOSUS_Projekt.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();
            Services servis = new Services();
            ViewBag.OdabraniGrad = 0;
            model.gradovi = servis.DohvatiListuGradova();
            model.odasiljaci = null;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(int? gradId)
        {
            if(gradId == null)
            {
                return RedirectToAction("Index");
            }
            int grad = gradId ?? default(int);
            HomeViewModel model = new HomeViewModel();
            Services servis = new Services();
            ViewBag.OdabraniGrad = grad;
            model.gradovi = servis.DohvatiListuGradova();
            model.odasiljaci = servis.VratiOdasiljace(grad);
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}