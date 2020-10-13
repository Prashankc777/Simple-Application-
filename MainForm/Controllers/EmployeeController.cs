using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using Modals.CustomValidation;
using Modals.Models;

namespace MainForm.Controllers
{
    [Authorize]

    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDataProtector protector;



        public EmployeeController(IEmployeeRepository employeeRepository, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            this._employeeRepository = employeeRepository;
            this.protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
        }



        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmpoloyee().Select(e =>
                            {
                                // Encrypt the ID value and store in EncryptedId property
                                e.EncrypteId = protector.Protect(e.Id.ToString());
                                return e;
                            });
            return View(model);


        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Edit(string id)
                
        {
            if (ModelState.IsValid)
            {
                string decriptId = protector.Unprotect(id);
                var one = _employeeRepository.GetOneEmployee(Convert.ToInt32(decriptId));
                View(one);
            }


            return View();

        }



        [HttpPost]
        [Authorize]
        public IActionResult Edit(Employee employeeChanges)
        {
            if (!ModelState.IsValid) return View();
            var emp = _employeeRepository.Update(employeeChanges);
            return RedirectToAction("Index");

        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(Employee Employee)
        {
            if (!ModelState.IsValid) return View();
            var emp = _employeeRepository.Add(Employee);
            return RedirectToAction("Index");

        }
        [HttpGet]
        public IActionResult Delete(string id)
        {
            if (ModelState.IsValid)
            {
                string decriptId = protector.Unprotect(id);
                var one = _employeeRepository.GetOneEmployee(Convert.ToInt32(decriptId));
                View(one);
            }


            return View();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid) return View();
            var emp = _employeeRepository.Delete(id);
            return RedirectToAction("Index");
        }


    }
}
