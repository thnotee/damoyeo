using Damoyeo.Model.Model;
using Damoyeo.Model.Model.option;
using Damoyeo.Model.Model.Pager;
using Damoyeo.Model.Model.Procedure;
using System.Threading.Tasks;

namespace Damoyeo.DataAccess.Repository.IRepository
{
    public interface IDamoyeoCommunityRepository : IRepository<DamoyeoCommunity>
    {
        Task<PagedList<DamoyeoCommunity>> GetPagedListAsync(int page, int pageSize, CommunitySearchOpt option);
    }
}