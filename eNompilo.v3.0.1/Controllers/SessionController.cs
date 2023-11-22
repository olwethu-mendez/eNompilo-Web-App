using eNompilo.v3._0._1.Areas.Identity.Data;
using eNompilo.v3._0._1.Models;
using eNompilo.v3._0._1.Models.SystemUsers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace eNompiloCounselling.Controllers
{
    public class SessionController : Controller
    {
        private readonly ApplicationDbContext dbContext;
		private readonly IHttpContextAccessor _contextAccessor;

		public SessionController(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            dbContext = context;
			_contextAccessor = contextAccessor;
		}
        public IActionResult Index()
		{
			ViewBag.SessionSuccessMessage = TempData["SessionSuccessMessage"] as string;
			IEnumerable<Session> objList = dbContext.tblSession.Include(x=>x.Patient).ThenInclude(x=>x.Users).Include(x=>x.Practitioner).ThenInclude(x => x.Users);
            return View(objList);
        }

        public IActionResult NewSession()
        {
            ViewBag.gaId = Convert.ToString(TempData["GASession"]);
            ViewBag.caId = Convert.ToString(TempData["CASession"]);
            ViewBag.fpaId = Convert.ToString(TempData["FPASession"]);
            ViewBag.vaId = Convert.ToString(TempData["VASession"]);
            string gaPatId = Convert.ToString(TempData["GASessPatient"]);
            string caPatId = Convert.ToString(TempData["CASessPatient"]);
            string fpaPatId = Convert.ToString(TempData["FPASessPatient"]);
            string vaPatId = Convert.ToString(TempData["VASessPatient"]);
            if (gaPatId != "")
            {
                _contextAccessor.HttpContext.Session.SetString("SessPatId", gaPatId);
                return View();
            }
            else if (caPatId != "")
            {
				_contextAccessor.HttpContext.Session.SetString("SessPatId", caPatId); 
                return View();
            }
            else if (fpaPatId != "")
            {
				_contextAccessor.HttpContext.Session.SetString("SessPatId", fpaPatId);
				return View();
            }
            else if (vaPatId != "")
			{
				_contextAccessor.HttpContext.Session.SetString("SessPatId", vaPatId);
				return View();
            }
            return View();
        }

        [HttpPost]
        public IActionResult NewSession(Session model)
        {
			if (ModelState.IsValid)
			{
				dbContext.tblSession.Add(model);
				dbContext.SaveChanges();
				TempData["SessionSuccessMessage"] = "Session for patient saved and recorded successfully!";
				return RedirectToAction("Index");
			}
            return View(model);
		}

        public IActionResult Details(int? Id) 
        {
            if(Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = dbContext.tblSession.Where(x=>x.Id == Id).Include(x=>x.Patient).ThenInclude(x=>x.Users).Include(x=>x.Practitioner).ThenInclude(x=>x.Users).FirstOrDefault();
            if(obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
    }
}
