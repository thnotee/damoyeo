using Damoyeo.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.Pager;
using static Dapper.SqlMapper;

namespace Damoyeo.DataAccess.Repository
{
    public class DamoyeoCategoryRepository : IDamoyeoCategoryRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoCategoryRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public Task<int> AddAsync(DamoyeoCategory entity)
        {
            throw new NotImplementedException();
        }

        public Task<DamoyeoCategory> GetAsync(DamoyeoCategory entity)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<DamoyeoCategory>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {

            int startRange = ((page - 1) * pageSize) + 1;
            int endRange = startRange + pageSize - 1;
            var sql = @"
SELECT * FROM (
	SELECT 
   	ROW_NUMBER() over(order by A.category_id) as row_num
	, COUNT(A.category_id) over() total_count
     ,category_id
     ,category_name
  FROM Damoyeo_Category A
 ) B
 WHERE 
 B.row_num between @startRange and @endRange
";

            IEnumerable<DamoyeoCategory> items = await _connection.QueryAsync<DamoyeoCategory>(sql, new { startRange, endRange }, transaction: _transaction);
            if (items.Any())
            {
                return new PagedList<DamoyeoCategory>(items, items.FirstOrDefault().total_count, page, pageSize);
            }
            else
            {
                return new PagedList<DamoyeoCategory>(Enumerable.Empty<DamoyeoCategory>(), 0, 0, 0);
            }
        }

        public Task RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(DamoyeoCategory entity)
        {
            throw new NotImplementedException();
        }
    }
}
