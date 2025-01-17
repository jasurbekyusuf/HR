﻿//===================================================
// Copyright (c)  coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Pease
//===================================================

using HR.API.Models.Employees;
using HR.DataAccess.Addressess;
using HR.DataAccess.Employees;
using HR.DataAccess.Entities;

namespace HR.API.Services
{
    public class EmployeeCRUDService : IGenericCRUDService<EmployeeModel>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IAccountNumberValidationService _accountNumberValidationService;
        public EmployeeCRUDService(IEmployeeRepository employeeRepository ,
            IAddressRepository addressRepository,
            IAccountNumberValidationService accountNumberValidationService)
        {
            _employeeRepository = employeeRepository;
            _addressRepository = addressRepository;
            _accountNumberValidationService = accountNumberValidationService;
        }
        public async Task<EmployeeModel> Create(EmployeeModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.AccountNumber) && !_accountNumberValidationService.IsValid(
                model.AccountNumber))
            {
                throw new Exception("Invalid account number");
            }
            var existingAddress = await _addressRepository.GetAddress(model.AddressId);
            var employee = new Employee
            {
                FullName = model.FullName,
                Department = model.Department,
                Email = model.Email,
                Salary= model.Salary,
            };
            if (existingAddress != null)
                employee.Address = existingAddress;

            var createdEmployee = await _employeeRepository.CreateEmployee(employee);
            var result = new EmployeeModel
            {
                FullName = createdEmployee.FullName,
                Department = createdEmployee.Department,
                Email = createdEmployee.Email,
                Id = createdEmployee.Id,
                Salary = createdEmployee.Salary,
                AddressId = createdEmployee.Address.Id
            };
            return result;
        }

        public async Task<bool> Delete(int id)
        {
            return await _employeeRepository.DeleteEmployee(id);
        }

        public async Task<EmployeeModel> Get(int id)
        {
            var employee = await _employeeRepository.GetEmployee(id);
            var model = new EmployeeModel
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Department = employee.Department,
                Email = employee.Email,
                Salary = employee.Salary
            };
            return model;
        }

        public async Task<IEnumerable<EmployeeModel>> GetAll()
        {
            var result = new List<EmployeeModel>();
            var employees = await _employeeRepository.GetEmployees();
            foreach (var employee in employees) 
            {
                var model = new EmployeeModel
                {
                    FullName = employee.FullName,
                    Department = employee.Department,
                    Email = employee.Email,
                    Id = employee.Id,
                    Salary = employee.Salary
                };
                result.Add(model);
            }
            return result;
        }

        public async Task<EmployeeModel> Update(int id, EmployeeModel model)
        {
            var employee = new Employee
            {
                FullName = model.FullName,
                Department = model.Department,
                Email = model.Email,
                Id= model.Id,
                Salary = model.Salary
            };
            var updatedEmployee = await _employeeRepository.UpdateEmployee(id, employee);
            var result = new EmployeeModel
            {
                FullName = updatedEmployee.FullName,
                Department = updatedEmployee.Department,
                Email = updatedEmployee.Email,
                Id = updatedEmployee.Id,
                Salary = updatedEmployee.Salary
            };
            return result;
        }
    }
}
