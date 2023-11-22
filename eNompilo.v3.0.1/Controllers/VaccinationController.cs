﻿using eNompilo.v3._0._1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Linq;
using eNompilo.v3._0._1.Models.SystemUsers;
using eNompilo.v3._0._1.Areas.Identity.Data;
using eNompilo.v3._0._1.Constants;
using Microsoft.EntityFrameworkCore;
using eNompilo.v3._0._1.Models.ViewModels;
using eNompilo.v3._0._1.Models.Vaccination;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eNompilo.v3._0._1.Controllers
{
    public class VaccinationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VaccinationController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;


        public VaccinationController(ILogger<VaccinationController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            if (User.IsInRole(RoleConstants.Practitioner))
            {
                IEnumerable<VaccinationInventory> objList = _context.tblVaccinationInventory.Where(ds => ds.Archived == false);
                return View(objList);
            }
            return View();
        }

        public IActionResult DoseTracking()
        {
            return View();
        }

        public IActionResult ViewDoseTrackings()
        {
            if (User.IsInRole(RoleConstants.Practitioner))
            {
                IEnumerable<DoseTracking> doses = _context.tblDoseTracking.Where(ds => ds.Archived == false).Include(d => d.Patient).Include(d => d.VaccinationInventory);
                return View(doses);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoseTracking(DoseTracking model)
        {
            Random rnd = new Random();
            int randomNo = rnd.Next(1000, 100000);
            string formattedNo = "";

            bool isUnique = !_context.tblDoseTracking.Any(item => item.CertificateNo == randomNo.ToString());
            if (isUnique)
            {
                formattedNo = string.Format("GRP-03-28-{0:D5}$", randomNo);
                model.CertificateNo = formattedNo;
            }


            if (model.PatientId != null && model.VaccineInventoryId != null && model.DateAdministered != null && model.SiteAddress != null)
            {
                //DoseTracking doseTracking = new DoseTracking 
                //{
                //    PatientId = model.PatientId,
                //    VaccineInventoryId = model.VaccineInventoryId,
                //    DateAdministered = model.DateAdministered,
                //    SecondDose = model.SecondDose,
                //    SiteAddress = model.SiteAddress,
                //};

                VaccinationInventory vaxInv = _context.tblVaccinationInventory.Where(x => x.ID == model.VaccineInventoryId).FirstOrDefault();

                if (vaxInv != null && model.SecondDose == null && vaxInv.Quantity > 0)
                {
                    vaxInv.Quantity = vaxInv.Quantity - 1;
                }
                else if (vaxInv != null && model.SecondDose != null && vaxInv.Quantity > 0)
                {
                    vaxInv.Quantity = vaxInv.Quantity - 2;
                }
                else
                {
                    ViewBag.OutOfStockMessage = "Vaccine Out Of Stock! Please Restock!".ToString();
                    return View();
                }


                _context.tblDoseTracking.Add(model);
                _context.SaveChanges();
                return RedirectToAction("ViewDoseTrackings");
            }
            return View();
        }

        public IActionResult VaccinationInventory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VaccinationInventory(VaccinationInventory model)
        {
            if (ModelState.IsValid)
            {
                _context.tblVaccinationInventory.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult UpdateVax(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _context.tblVaccinationInventory.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateVax(VaccinationInventory model)
        {
            if (model.Quantity == null && model.ExpirationDate == null)
                return View(model);

            var obj = _context.tblVaccinationInventory.Where(vi => vi.ID == model.ID).FirstOrDefault();

            if (obj == null)
                return NotFound();

            obj.Quantity = model.Quantity;
            obj.ExpirationDate = model.ExpirationDate;

            _context.tblVaccinationInventory.Update(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult PrivacyandSecurity()
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole(RoleConstants.Patient))
            {
                var userId = _userManager.GetUserId(User);
                var patient = _context.tblPatient.SingleOrDefault(c => c.UserId == userId);
                var patientId = patient.Id;
                HttpContext.Session.SetInt32("PatientId", patientId);

                var patientFile = _context.tblPatientFile.SingleOrDefault(c => c.PatientId == patientId);
                var patientFileId = patientFile.Id;
                HttpContext.Session.SetInt32("PatientFileId", patientFileId);

                ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;

                //var generalAppointment = _context.tblGeneralAppointment.Where(p => p.PatientId == patientId).Include(p => p.Patient).ToList();
                //var counsellingAppointment = _context.tblCounsellingAppointment.Where(p => p.PatientId == patientId).Include(p => p.Patient).ToList();
                //var fpAppointment = _context.tblFamilyPlanningAppointment.Where(p => p.PatientId == patientId).Include(p => p.Patient).ToList();
                //var vaccinationAppointment = _context.tblVaccinationAppointment.Where(p => p.PatientId == patientId).Include(p => p.Patient).ToList();
                var personalDetails = _context.tblPersonalDetails.Where(p => p.PatientId == patientId).FirstOrDefault();

                HomePageViewModel viewModel = new HomePageViewModel()
                {
                    //GeneralAppointments = generalAppointment,
                    //CounsellingAppointments = counsellingAppointment,
                    //FamilyPlanningAppointments = fpAppointment,
                    //VaccinationAppointments = vaccinationAppointment,
                    PersonalDetails = personalDetails
                };

                return View(viewModel);
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole(RoleConstants.Admin))
            {
                var userId = _userManager.GetUserId(User);
                var patient = _context.tblAdmin.SingleOrDefault(c => c.UserId == userId);
                var patientId = patient.Id;
                HttpContext.Session.SetInt32("AdminId", patientId);

                var admin = _context.tblAdmin.Where(p => p.Id == patientId).FirstOrDefault();

                HomePageViewModel viewModel = new HomePageViewModel()
                {
                    Admin = admin
                };

                return View(viewModel);
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole(RoleConstants.Practitioner))
            {
                var userId = _userManager.GetUserId(User);
                var patient = _context.tblPractitioner.SingleOrDefault(c => c.UserId == userId);
                var patientId = patient.Id;
                HttpContext.Session.SetInt32("PractitionerId", patientId);

                var practitioner = _context.tblPractitioner.Where(p => p.Id == patientId).FirstOrDefault();

                HomePageViewModel viewModel = new HomePageViewModel()
                {
                    Practitioner = practitioner
                };

                return View(viewModel);
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole(RoleConstants.Receptionist))
            {
                var userId = _userManager.GetUserId(User);
                var patient = _context.tblReceptionist.SingleOrDefault(c => c.UserId == userId);
                var patientId = patient.Id;
                HttpContext.Session.SetInt32("ReceptionistId", patientId);

                var receptionist = _context.tblReceptionist.Where(p => p.Id == patientId).FirstOrDefault();

                HomePageViewModel viewModel = new HomePageViewModel()
                {
                    Receptionist = receptionist
                };

                return View(viewModel);
            }
            return View();
        }

        public IActionResult Certificate(int? Id)
        {
            if (Id == 0 || Id == null)
                return NotFound();
            var obj = _context.tblDoseTracking.Where(d => d.ID == Id).Include(d => d.Patient).Include(d => d.VaccinationInventory).FirstOrDefault();
            if (obj == null)
                return NotFound();
            return View(obj);
        }


        //Remove
        public IActionResult Remove(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            var obj = _context.tblDoseTracking.Where(va => va.ID == id).Include(d => d.Patient).Include(d => d.VaccinationInventory).FirstOrDefault();
            if (obj == null)
            {
                return NotFound();
            }

            var model = new ArchiveItemViewModel
            {
                Id = obj.ID,
                DoseTrackingID = obj.ID,
                Archived = obj.Archived,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(ArchiveItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var obj = _context.tblDoseTracking.Where(va => va.ID == model.Id).FirstOrDefault();

            if (obj == null)
            {
                return NotFound();
            }

            obj.Archived = model.Archived;

            _context.tblDoseTracking.Update(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult RemoveVax(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            var obj = _context.tblVaccinationInventory.Where(va => va.ID == id).FirstOrDefault();
            if (obj == null)
            {
                return NotFound();
            }

            var model = new ArchiveItemViewModel
            {
                Id = obj.ID,
                VaxInventoryID = obj.ID,
                Archived = obj.Archived,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveVax(ArchiveItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var obj = _context.tblVaccinationInventory.Where(va => va.ID == model.Id).FirstOrDefault();

            if (obj == null)
            {
                return NotFound();
            }

            obj.Archived = model.Archived;

            _context.tblVaccinationInventory.Update(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
