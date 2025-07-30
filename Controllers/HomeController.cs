using EmployeeManagament.Models;
using EmployeeManagament.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Reflection;


namespace EmployeeManagament.Controllers
{


    public class HomeController : Controller
    {
        
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment webHostEnvironment)
        {
            _employeeRepository = employeeRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public ViewResult Index()
        {
            List<Employee>  list_emp = (List<Employee>)_employeeRepository.GetAllEmployee();
            return View(list_emp);
        }
        public ViewResult Details(int id)
        {
            //Employee model=  _employeeRepository.GetEmployee(1);
            //ViewData["Employee"] = model;
            //ViewBag.Employee = model;
            Employee  employee = _employeeRepository.GetEmployee(id);

           

            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                employee = _employeeRepository.GetEmployee(id),
                PageTitle = "Detail Page "

            };

            //return View(model);
            return View(homeDetailsViewModel);
    }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)

        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                // If the Photo property on the incoming model object is not null, then the user
                // has selected an image to upload.
                if (model.Photo != null )
                {

                    //ProcessUploadedFile(model);

                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                     
              
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

                    
                }


                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    // Store the file name in PhotoPath property of the employee object
                    // which gets saved to the Employees database table
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();



        }


        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if (model.Photo != null)
                {
                    // First upload new photo
                    string newPhotoPath = ProcessUploadedFile(model);

                    // Then delete the old photo if it exists and is different
                    if (model.ExistingPhotoPath != null)
                    {
                        string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                            catch (IOException ex)
                            {
                                // Optional: log error and show user-friendly message
                                ModelState.AddModelError("", "Unable to delete old photo. Please try again.");
                                return View(model);
                            }
                        }
                    }

                    // Update employee's photo path
                    employee.PhotoPath = newPhotoPath;
                }

                _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }

            return View(model);
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;

            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }


    }
}
