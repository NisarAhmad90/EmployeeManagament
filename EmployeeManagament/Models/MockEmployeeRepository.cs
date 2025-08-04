namespace EmployeeManagament.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {

        private List<Employee> _employeesList;
        public MockEmployeeRepository()
        {
            _employeesList = new List<Employee>()
            {
                new Employee {Id = 1,Name = "Nisar", Department = Dept.None, Email = "n@gmail.com" },
                new Employee {Id = 2,Name = "Nisar2",Department = Dept.Payroll, Email = "n2@gmail.com" },
                new Employee {Id = 3,Name = "Nisar3",Department = Dept.HR, Email = "n3@gmail.com" },
                new Employee {Id = 4,Name = "Nisar4",Department = Dept.IT, Email = "n4@gmail.com" }

            };
        }

        public Employee Add(Employee employee)
        {

            employee.Id = _employeesList.Max(x => x.Id) + 1;

            _employeesList.Add(employee);
            return employee;


        }

        public Employee Delete(int Id)
        {
            Employee employee = _employeesList.FirstOrDefault(x => x.Id == Id);
            if(employee != null)
            {
                _employeesList.Remove(employee);
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeesList;
        }

        public Employee? GetAllEmployee(int id)
        {
            Employee? employee = _employeesList.FirstOrDefault(e => e.Id == id);
            //if (employee == null)
            //    throw new Exception($"Employee with ID {id} not found.");
            return employee;
        }

        public Employee GetEmployee(int Id)
        {
            throw new NotImplementedException();
        }

        public Employee Update(Employee employeeChanges)
        {
            Employee employee = _employeesList.FirstOrDefault(e => e.Id == employeeChanges.Id);
            if (employee != null)
            {
                employee.Name = employeeChanges.Name;
                employee.Email = employeeChanges.Email;
                employee.Department = employeeChanges.Department;
            }
            return employee;
        }
    }
}
