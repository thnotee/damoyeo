using Damoyeo.Model.Model;
using Damoyeo.Model.Model.Pager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.DataAccess.Repository.IRepository
{
    public interface IDamoyeoUserRepository : IRepository<DamoyeoUser>
    {
        Task<DamoyeoUser> GetNicknameAsync(DamoyeoUser entity);


        Task<PagedList<DamoyeoUser>> GetUserPagedListAsync(int page, int pageSize, string searchString = "", int type = 0);

    }
    
}
