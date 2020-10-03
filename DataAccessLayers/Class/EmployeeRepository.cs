using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using DataAccessLayers.Interface;
using Modals.Models;
using static DataAccessLayers.ORm.Dapper;

namespace DataAccessLayers.Class
{
    public class EmployeeRepository : IEmployeeRepository
    {
        public IEnumerable<Employee> GetAllEmpoloyee()
        {
            var param = new DynamicParameters();
            param.Add("@status", "ALL");
            var lst = (ReturnList<Employee>("SEARCH", param));
            return lst;
        }

        public Employee GetOneEmployee(int id)
        {
           return (GetAllEmpoloyee().FirstOrDefault(x => x.Id == id));
        }

        public Employee Update(Employee employeeChanges)
        {
            var emp = GetAllEmpoloyee().FirstOrDefault(q => q.Id == employeeChanges.Id);
            if (emp is null) return emp;
            var Param = new DynamicParameters();
            Param.Add("@Id", employeeChanges.Id);
            Param.Add("@FirstName", employeeChanges.FirstName);
            Param.Add("@LastName", employeeChanges.LastName);
            Param.Add("@Gender", employeeChanges.Gender);
            Param.Add("@Address", employeeChanges.Address);
            DataAccessLayers.ORm.Dapper.ExceptionWithoutReturn("EMPLOYEE_EDIT", param: Param);
            return emp;
        }

        public Employee Add(Employee Employee)
        {
            var Param = new DynamicParameters();
            Param.Add("@FirstName", Employee.FirstName);
            Param.Add("@LastName", Employee.FirstName);
            Param.Add("@Gender", Employee.FirstName);
            Param.Add("@Adress", Employee.FirstName);
            ORm.Dapper.ExceptionWithoutReturn("[EMPLOYEE_CREATE]",param:Param);
            return Employee;
        }

        public Employee Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
