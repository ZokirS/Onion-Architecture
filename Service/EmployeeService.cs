using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Dynamic;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IEmployeeLinks _employeeLinks;
        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, 
            IMapper mapper, IEmployeeLinks employeeLinks)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _employeeLinks = employeeLinks;
        }

        public async Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync(Guid companyId,
            LinkParameters linkParameters, bool trackChanges)
        {
            if (!linkParameters.EmployeeParameters.ValidAgeRange)
                throw new MaxAgeRangeBadRequestException();

            await CheckCompanyIsExcist(companyId, trackChanges);

            var employeesWithMetaData = await _repository.Employee.GetEmployeesAsync(companyId, linkParameters.EmployeeParameters, trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);

            var links = _employeeLinks.TryGenerateLinks(employeesDto,
                linkParameters.EmployeeParameters.Fields, companyId, linkParameters.Context);

            return (linkResponse: links, metaData: employeesWithMetaData.MetaData);
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId,Guid id, bool trackChanges)       
        {
            await CheckCompanyIsExcist(companyId, trackChanges);
            var employeeFromDb = await GetEmployeeForComapnyAndCheckIfItExcist(companyId, id, trackChanges);

            var employeeDto = _mapper.Map<EmployeeDto>(employeeFromDb);
            return employeeDto;
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(Guid companyId, EmployeeForCreationDto employee, bool trackChanges)
        {
            await CheckCompanyIsExcist(companyId, trackChanges);

            var employeeEntity = _mapper.Map<Employee>(employee);

             _repository.Employee.CreateEmployee(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToRetun = _mapper.Map<EmployeeDto>(employeeEntity);
            return employeeToRetun;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckCompanyIsExcist(companyId, trackChanges);

            var employeeForCompany = await GetEmployeeForComapnyAndCheckIfItExcist(companyId, id, trackChanges);

            _repository.Employee.DeleteEmployee(employeeForCompany);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckCompanyIsExcist(companyId, compTrackChanges);

            var employeeEntity = await GetEmployeeForComapnyAndCheckIfItExcist(companyId, id, empTrackChanges);

            _mapper.Map(employeeForUpdate, employeeEntity);
            await _repository.SaveAsync();
        }

        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(
            Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckCompanyIsExcist(companyId, compTrackChanges);

            var employeeEntity = await GetEmployeeForComapnyAndCheckIfItExcist(companyId, id, empTrackChanges);
            
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity); 
            return (employeeToPatch, employeeEntity);
        }

        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();
        }

        private async Task CheckCompanyIsExcist(Guid companyId, bool trackChanges)
        {
            var company =  _repository.Company.GetCompany(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
        }

        private async Task<Employee> GetEmployeeForComapnyAndCheckIfItExcist(Guid companyId, Guid id, bool trackChanges)
        {
            var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employeeDb is null)
                throw new EmployeeNotFoundException(id);

            return employeeDb;
        }
    }
}
