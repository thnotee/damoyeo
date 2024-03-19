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
    public class DamoyeoTagsRepository : IDamoyeoTagsRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoTagsRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<int> AddAsync(DamoyeoTags entity)
        {
            var sql = @"
INSERT INTO Damoyeo_Tags (tag_name)
    OUTPUT INSERTED.tag_id
VALUES (@tag_name);
";
            return await _connection.QuerySingleAsync<int>(sql, entity, _transaction);
        }

        public async Task<DamoyeoTags> GetAsync(DamoyeoTags entity)
        {

            var sql = @"
SELECT tag_id, tag_name FROM Damoyeo_Tags
WHERE 
tag_name = @tag_name;
";
            return await _connection.QueryFirstOrDefaultAsync<DamoyeoTags>(sql, entity, _transaction);
        }

        public Task<PagedList<DamoyeoTags>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(DamoyeoTags entity)
        {
            throw new NotImplementedException();
        }
    }
}
