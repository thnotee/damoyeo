using Damoyeo.Model.Model.Pager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.DataAccess.Repository.IRepository
{
    public interface IRepository<T>
    {
        Task AddAsync(T entity);
        Task<T> GetAsync(T entity);
        Task<PagedList<T>> GetPagedListAsync(int page, int pageSize, string searchString = "");
        Task UpdateAsync(T entity);
        Task RemoveAsync(int id);
    }
}
