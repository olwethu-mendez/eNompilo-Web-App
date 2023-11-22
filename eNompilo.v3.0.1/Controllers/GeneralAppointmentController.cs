﻿using eNompilo.v3._0._1.Areas.Identity.Data;
using eNompilo.v3._0._1.Constants;
using eNompilo.v3._0._1.Models;
using eNompilo.v3._0._1.Models.SystemUsers;
using eNompilo.v3._0._1.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eNompiloCounselling.Controllers
{
	[Authorize]
	public class GeneralAppointmentController : Controller
	{
		private readonly ApplicationDbContext dbContext;
		private readonly IHttpContextAccessor _contextAccessor;

		public GeneralAppointmentController(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
		{
			dbContext = context;
			_contextAccessor = contextAccessor;
		}
		public IActionResult Index()
		{
			var patientId = _contextAccessor.HttpContext.Session.GetInt32("PatientId");

			ViewBag.GASuccessMessage = TempData["GASuccessMessage"] as string;
			ViewBag.GAUpdateSuccessMessage = TempData["GAUpdateSuccessMessage"] as string;


            IEnumerable<GeneralAppointment> objList = dbContext.tblGeneralAppointment.Where(x=>x.Archived == false).Include(pr=>pr.Practitioner).ThenInclude(u=>u.Users).Include(p=>p.Patient).ThenInclude(u => u.Users);

			//var patient = dbContext.tblPatient.Select(p => new SelectListItem
			//{
			//	Value = p.Id.ToString(),
			//	Text = p.FirstName + " " + p.LastName,
			//}).ToList();
			//var practitioner = dbContext.tblPractitioner.Select(p => new SelectListItem
			//{
			//	Value = p.Id.ToString(),
			//	Text = p.Users.FirstName + " " + p.Users.LastName,
			//}).ToList();

			//ViewBag.ddlPatient = patient;
			//ViewBag.ddlPractitioner = practitioner;

			return View(objList);
		}

		public IActionResult Book()
		{
			var bookedAppointments = dbContext.tblGeneralAppointment
				.Select(a => new { a.PractitionerId, a.PreferredDate, a.PreferredTime })
				.ToList();

			ViewBag.BookedAppointments = bookedAppointments;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Book(GeneralAppointment model)
		{
			if (model.PatientIssues != null && model.PreferredDate != null && model.PreferredTime != null && model.PatientId != null)
			{
				GeneralAppointment ga = new GeneralAppointment()
				{
					PatientIssues = model.PatientIssues,
					PreferredDate = model.PreferredDate,
					PreferredTime = model.PreferredTime,
					PatientId = model.PatientId,
					PractitionerId = model.PractitionerId,
					SessionConfirmed = model.SessionConfirmed,
				};
				dbContext.tblGeneralAppointment.Add(ga);
				dbContext.SaveChanges();
                TempData["GASuccessMessage"] = "Appointment for date: " + ga.PreferredDate + " successfully booked!";
                return RedirectToAction("Index");
			}
			return View(model);
		}

		public IActionResult Update(int? Id)
		{
			if (Id == null || Id == 0)
			{
				return NotFound();
			}
			var obj = new GeneralAppointment();
			if(User.IsInRole(RoleConstants.Practitioner) || User.IsInRole(RoleConstants.Receptionist))
				obj = dbContext.tblGeneralAppointment.Where(x=>x.Id == Id && x.Archived == false).FirstOrDefault();
			else if(User.IsInRole(RoleConstants.Patient))
				obj = dbContext.tblGeneralAppointment.Where(x=>x.Id == Id && x.Archived == false && x.SessionConfirmed == false).FirstOrDefault();
			else if(User.IsInRole(RoleConstants.Admin))
				obj = dbContext.tblGeneralAppointment.FirstOrDefault();

			if (obj == null)
			{
				return NotFound();
			}
			return View(obj);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Update(GeneralAppointment model)
		{
			if (model.PatientIssues == null && model.PatientId == null)
				return View(model);

			var obj = dbContext.tblGeneralAppointment.Where(va => va.Id == model.Id).FirstOrDefault();

            if (obj == null)
            {
                return NotFound();
            }

			if(model.PreferredDate == null)
			{
				model.PreferredDate = obj.PreferredDate;
			}
			if(model.PreferredTime == null)
			{
				model.PreferredTime = obj.PreferredTime;
			}

			obj.Id = model.Id;
			obj.PatientIssues = model.PatientIssues;
			obj.PreferredDate = model.PreferredDate;
			obj.PreferredTime = model.PreferredTime;
			obj.PatientId = model.PatientId;
			obj.PractitionerId = model.PractitionerId;
			if (obj.PractitionerId != null)
				obj.SessionConfirmed = true;

            dbContext.tblGeneralAppointment.Update(obj);
			dbContext.SaveChanges();
            TempData["GAUpdateSuccessMessage"] = "Appointment for date: " + obj.PreferredDate + " successfully update!";
            return RedirectToAction("Index");
		}

		public IActionResult Details(int? Id)
		{
			var obj = dbContext.tblGeneralAppointment.Where(ga=>ga.Id == Id).Include(ga=>ga.Patient).ThenInclude(ga=>ga.Users).Include(ga=>ga.Patient).ThenInclude(ga=>ga.Users).FirstOrDefault();
			if (obj == null)
				return View("PageNotFound", "Home");
			return View(obj);
		}

		public IActionResult Cancel(int? Id)
		{
			if (Id == 0 || Id == null)
			{
				return NotFound();
			}
			var obj = dbContext.tblGeneralAppointment.Find(Id);
			if (obj == null)
			{
				return NotFound();
			}
			var model = new ArchiveItemViewModel
			{
				Id = obj.Id,
				GenAppointmentId = obj.Id,
				Archived = obj.Archived
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Cancel(ArchiveItemViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var obj = dbContext.tblGeneralAppointment.Where(va => va.Id == model.Id).FirstOrDefault();

			if (obj == null)
			{
				return NotFound();
			}

			obj.Archived = model.Archived;

			dbContext.tblGeneralAppointment.Update(obj);
			dbContext.SaveChanges();
			return RedirectToAction("Index");
		}

		public IActionResult StartSession(int Id) //throws error here!
		{
			if(Id == null || Id == 0)
			{
				return NotFound();
			}
			TempData["GASession"] = Id;
			int patientId = dbContext.tblGeneralAppointment.Where(x => x.Id == Id).FirstOrDefault().PatientId;
			TempData["GASessPatient"] = patientId;

            return RedirectToAction("NewSession", "Session");
        }
	}
}
