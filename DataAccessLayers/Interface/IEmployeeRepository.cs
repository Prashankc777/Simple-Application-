using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using Modals.Models;

namespace DataAccessLayers.Interface
{
   public  interface IEmployeeRepository
   {
       IEnumerable<Employee> GetAllEmpoloyee();

       Employee GetOneEmployee(int id);

       Employee Update(Employee employeeChanges);

       Employee Add(Employee Employee);

       Employee Delete(int id);


   }
}
