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

namespace Damoyeo.DataAccess.Repository
{
    public class DamoyeoUserInterestCategoryRepository : IDamoyeoUserInterestCategoryRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;

        public DamoyeoUserInterestCategoryRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<int> AddAsync(DamoyeoUserInterestCategory entity)
        {
            var sql = @"
INSERT INTO Damoyeo_User_InterestCategory (user_id, category_id)
VALUES (@user_id, @category_id);
";
            return await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public async Task<IEnumerable<DamoyeoUserInterestCategory>> GetAllAsync(DamoyeoUserInterestCategory entity)
        {
            var sql = @"
SELECT interest_id, user_id, category_id FROM Damoyeo_User_InterestCategory
WHERE 
user_id = @user_id;
";
            return await _connection.QueryAsync<DamoyeoUserInterestCategory>(sql, entity, _transaction);
        }

        public Task<DamoyeoUserInterestCategory> GetAsync(DamoyeoUserInterestCategory entity)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<DamoyeoUserInterestCategory>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(int id)
        {

            var sql = @"
DELETE FROM Damoyeo_User_InterestCategory
WHERE 
user_id = @id;
";
            await _connection.ExecuteAsync(sql, new { id }, _transaction);
        }

        public Task UpdateAsync(DamoyeoUserInterestCategory entity)
        {
            throw new NotImplementedException();
        }
    }
}
