using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonManagement.Domain.Entities;
using PersonManagement.Domain.Interfaces;
using PersonManagement.Infrastructure.Data;

namespace PersonManagement.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.Persons
                .OrderBy(p => p.PersonId)
                .ToListAsync();
        }

        public async Task<Person> GetByIdAsync(int id)
        {
            return await _context.Persons.FindAsync(id);
        }

        public async Task<Person> CreateAsync(Person person)
        {
            person.CreatedAt = DateTime.UtcNow;
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
            return person;
        }

        public async Task<Person> UpdateAsync(Person person)
        {
            person.UpdatedAt = DateTime.UtcNow;
            _context.Entry(person).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
                return false;

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Persons.AnyAsync(p => p.PersonId == id);
        }
    }
}