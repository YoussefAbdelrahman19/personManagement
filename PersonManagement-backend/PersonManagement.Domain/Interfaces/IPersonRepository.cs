using System.Collections.Generic;
using System.Threading.Tasks;
using PersonManagement.Domain.Entities;

namespace PersonManagement.Domain.Interfaces
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetAllAsync();
        Task<Person> GetByIdAsync(int id);
        Task<Person> CreateAsync(Person person);
        Task<Person> UpdateAsync(Person person);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}