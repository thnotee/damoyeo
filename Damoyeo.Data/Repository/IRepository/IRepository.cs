using Damoyeo.Model.Model.Pager;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Data.Repository.IRepository
{
    public interface IRepository<T>
    {
        Task AddAsync(T entity);
        Task<T> GetAsync(string id);
        Task<PagedList<T>> GetPagedListAsync<U>(int page, int pageSize);
        Task UpdateAsync(T user);
        Task Remove(int id);
    }

}
