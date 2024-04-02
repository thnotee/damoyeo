using Damoyeo.Model.Model;
using Damoyeo.Model.Model.option;
using Damoyeo.Model.Model.Pager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.DataAccess.Repository.IRepository
{
    public interface IDamoyeoMeetupRepository : IRepository<DamoyeoMeetup>
    {

        Task<PagedList<DamoyeoMeetup>> GetPagedListAsync(int page, int pageSize, MeetupSearchOpt option);

    }
}
