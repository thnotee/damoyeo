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
    public class DamoyeoMeetupTagsRepository : IDamoyeoMeetupTagsRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoMeetupTagsRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<int> AddAsync(DamoyeoMeetupTags entity)
        {
            var sql = @"
INSERT INTO Damoyeo_Meetup_Tags (meetup_id, tag_id)
VALUES (@meetup_id, @tag_id);
";
            return await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public Task<DamoyeoMeetupTags> GetAsync(DamoyeoMeetupTags entity)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<DamoyeoMeetupTags>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(DamoyeoMeetupTags entity)
        {
            throw new NotImplementedException();
        }
    }
}
