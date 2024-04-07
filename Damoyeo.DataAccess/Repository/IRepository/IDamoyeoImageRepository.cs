using Damoyeo.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Damoyeo.DataAccess.Repository.IRepository
{
    public interface IDamoyeoImageRepository : IRepository<DamoyeoImage>
    {       
        Task<IEnumerable<DamoyeoImage>> GetAllAsync(DamoyeoImage entity);

       
        Task<int> RemoveTableIdAsync(DamoyeoImage entity, string separationValue = "");
    }
}
