using System.Collections.Generic;
using System.Threading.Tasks;
using PersonManagement.Application.DTOs;

namespace PersonManagement.Application.Interfaces
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonDto>> GetAllPersonsAsync();
        Task<PersonDto> GetPersonByIdAsync(int id);
        Task<PersonDto> CreatePersonAsync(CreatePersonDto createPersonDto);
        Task<PersonDto> UpdatePersonAsync(int id, UpdatePersonDto updatePersonDto);
        Task<bool> DeletePersonAsync(int id);
    }
}