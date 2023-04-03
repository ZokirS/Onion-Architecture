﻿using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company == null)
                throw new CompanyNotFoundException(companyId);

            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            return employeesDto;
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId,Guid id, bool trackChanges)       
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);

            var employeeFromDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employeeFromDb is null)
                throw new EmployeeNotFoundException(id);

            var employeeDto = _mapper.Map<EmployeeDto>(employeeFromDb);
            return employeeDto;
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(Guid companyId, EmployeeForCreationDto employee, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);

            var employeeEntity = _mapper.Map<Employee>(employee);

             _repository.Employee.CreateEmployee(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToRetun = _mapper.Map<EmployeeDto>(employeeEntity);
            return employeeToRetun;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);

            var employeeForCompany = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employeeForCompany is null)
                throw new EmployeeNotFoundException(id);

            _repository.Employee.DeleteEmployee(employeeForCompany);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges);
            if(company is null)
                throw new CompanyNotFoundException(companyId);

            var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges);
            if(employeeEntity is null)
                throw new EmployeeNotFoundException(id);

            _mapper.Map(employeeForUpdate, employeeEntity);
            await _repository.SaveAsync();
        }

        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(
            Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges);
            if (company is null) 
                throw new CompanyNotFoundException(companyId);
            
            var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges);
            
            if (employeeEntity is null) 
                throw new EmployeeNotFoundException(companyId); 
            
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity); 
            return (employeeToPatch, employeeEntity);
        }

        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();
        }
    }
}
