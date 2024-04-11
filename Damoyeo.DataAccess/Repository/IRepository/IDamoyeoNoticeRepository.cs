using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.option;
using Damoyeo.Model.Model.Pager;
using System.Threading.Tasks;

namespace Damoyeo.DataAccess.Repository
{
    public interface IDamoyeoNoticeRepository : IRepository<DamoyeoNotice>
    {
        Task<PagedList<DamoyeoNotice>> GetPagedListAsync(int page, int pageSize, CommunitySearchOpt option);
    }
}