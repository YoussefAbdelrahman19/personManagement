using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PersonManagement.Application.DTOs;
using PersonManagement.Application.Interfaces;
using PersonManagement.Domain.Entities;
using PersonManagement.Domain.Interfaces;

namespace PersonManagement.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _repository;
        private readonly IMapper _mapper;

        public PersonService(IPersonRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PersonDto>> GetAllPersonsAsync()
        {
            var persons = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PersonDto>>(persons);
        }

        public async Task<PersonDto> GetPersonByIdAsync(int id)
        {
            var person = await _repository.GetByIdAsync(id);
            if (person == null)
                throw new KeyNotFoundException($"Person with ID {id} not found");

            return _mapper.Map<PersonDto>(person);
        }

        public async Task<PersonDto> CreatePersonAsync(CreatePersonDto createPersonDto)
        {
            var person = _mapper.Map<Person>(createPersonDto);
            var createdPerson = await _repository.CreateAsync(person);
            return _mapper.Map<PersonDto>(createdPerson);
        }

        public async Task<PersonDto> UpdatePersonAsync(int id, UpdatePersonDto updatePersonDto)
        {
            var person = await _repository.GetByIdAsync(id);
            if (person == null)
                throw new KeyNotFoundException($"Person with ID {id} not found");

            _mapper.Map(updatePersonDto, person);
            var updatedPerson = await _repository.UpdateAsync(person);
            return _mapper.Map<PersonDto>(updatedPerson);
        }

        public async Task<bool> DeletePersonAsync(int id)
        {
            var exists = await _repository.ExistsAsync(id);
            if (!exists)
                throw new KeyNotFoundException($"Person with ID {id} not found");

            return await _repository.DeleteAsync(id);
        }
    }
}