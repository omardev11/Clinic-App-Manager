using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer
{
    public class PersonData : DataLayerInterfaces.IPersonData
    {
        private readonly AppDbContext _context;

        public PersonData(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<int?> AddNewPerson(Entities.Person person)
        {
          
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();

            return person.PersonId;
        }
        public async Task<Entities.Person> GetPersonInfoById(int PersonId)
        {
            var person = await _context.Persons
            .Where(p => p.PersonId == PersonId)
            .FirstOrDefaultAsync();
            
            return person;
        }
        public async Task<bool> UpdatePersonInfo(DTO.PersonDTO person)
        {
            //if (person == null || person.PersonId == null)
            //{
            //    return false; // Invalid person data, return false
            //}


            var ExistingPerson = await _context.Persons.FindAsync(person.PersonId);
            if (ExistingPerson == null)
            {
                return false; // Person not found, return false
            }


            ExistingPerson.FirstName = string.IsNullOrEmpty(person.FirstName) ? ExistingPerson.FirstName : person.FirstName;
            ExistingPerson.LastName = string.IsNullOrEmpty(person.LastName) ? ExistingPerson.LastName : person.LastName;
            ExistingPerson.DateOfBirth = person.DateOfBirth ?? ExistingPerson.DateOfBirth;
            ExistingPerson.Address = string.IsNullOrEmpty(person.Address) ? ExistingPerson.Address : person.Address;
            ExistingPerson.Gender = person.Gender ?? ExistingPerson.Gender;
            
            await _context.SaveChangesAsync();
            return true; // Update successful
        }
        
    
    }
}
