using Damoyeo.Model.Model;
using Damoyeo.Model.Model.option;
using Damoyeo.Model.Model.Pager;
using Damoyeo.Model.Model.Procedure;
using System.Threading.Tasks;

namespace Damoyeo.DataAccess.Repository.IRepository
{
    public interface IDamoyeoCommunityCommentRepository : IRepository<DamoyeoCommunityComment>
    {

        Task<PagedList<GetCommentTree>> GetPagedListAsync(int page, int pageSize, CommunitySearchOpt option);

        Task<PagedList<GetCommentTree>> GetUserCommentPagedListAsync(int page, int pageSize, CommunitySearchOpt option);
    }
}