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
    public class DamoyeoApplicationsRepository : IDamoyeoApplicationsRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoApplicationsRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<int> AddAsync(DamoyeoApplications entity)
        {
            var sql = @"
INSERT INTO Damoyeo_Applications (user_id, meetup_id, application_date)
VALUES (@user_id, @meetup_id, @application_date)
";
            
            return await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public async Task<IEnumerable<DamoyeoApplications>> GetAllAsync(DamoyeoApplications entity)
        {
            var sql = @"
SELECT application_id, user_id, meetup_id, application_date  FROM Damoyeo_Applications
WHERE 
meetup_id = @meetup_id
";

            return await _connection.QueryAsync<DamoyeoApplications>(sql, entity, _transaction);
        }

        public async Task<DamoyeoApplications> GetAsync(DamoyeoApplications entity)
        {

            var sql = @"
SELECT application_id, user_id, meetup_id, application_date  FROM Damoyeo_Applications
WHERE 
meetup_id = @meetup_id
AND user_id = @user_id
";

            return await _connection.QueryFirstOrDefaultAsync<DamoyeoApplications>(sql, entity, _transaction);
        }

        public Task<PagedList<DamoyeoApplications>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(int id)
        {
            var sql = @"
delete from Damoyeo_Applications 
where 
application_id = @id
";

            await _connection.ExecuteAsync(sql, new { id }, _transaction);
        }

        public Task UpdateAsync(DamoyeoApplications entity)
        {
            throw new NotImplementedException();
        }
    }
}
