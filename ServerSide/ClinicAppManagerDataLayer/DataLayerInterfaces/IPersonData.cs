using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DataLayerInterfaces
{
    public interface IPersonData
    {
        public Task<int?> AddNewPerson(Entities.Person person);
        public Task<Entities.Person> GetPersonInfoById(int PersonId);
        public Task<bool> UpdatePersonInfo(DTO.PersonDTO person);
    }
}
