using Damoyeo.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.DataAccess.Repository.IRepository
{
    public interface IDamoyeoMeetupTagsRepository : IRepository<DamoyeoMeetupTags>
    {
        Task<IEnumerable<DamoyeoMeetupTags>> GetAllAsync(DamoyeoMeetupTags entity);
    }
    
}
