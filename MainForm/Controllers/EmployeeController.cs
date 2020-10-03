using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using Modals.Models;

namespace MainForm.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
     

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
          
        }
       
        public IActionResult Index()
        {
            return View(_employeeRepository.GetAllEmpoloyee());
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            return ModelState.IsValid ? View(_employeeRepository.GetOneEmployee(id)) : View();

        }


        [HttpPost]
        public IActionResult Edit(Employee employeeChanges)
        {
            if (!ModelState.IsValid) return View();
            var emp = _employeeRepository.Update(employeeChanges);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee Employee)
        {
            if (!ModelState.IsValid) return View();
            var emp = _employeeRepository.Add(Employee);
            return RedirectToAction("Index");

        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            return ModelState.IsValid ? View(_employeeRepository.GetOneEmployee(id.Value)) : View();
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
