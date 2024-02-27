using Damoyeo.Data.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.Pager;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Damoyeo.Data.DataAccess
{
    public class DamoyeoUserRepository : IRepository<DamoyeoUser>
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoUserRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }
        public async Task AddAsync(DamoyeoUser entity)
        {
            var sql = @"
INSERT INTO Damoyeo_User (email, password, profile_image, slf_Intro, nickname, use_tf, reg_date)
VALUES (@Email, @Password, @ProfileImage, @Slf_Intro, @Nickname, @Use_Tf, @Reg_Date);
";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }

        public async Task<DamoyeoUser> GetAsync(string id)
        {
            var sql = "SELECT * FROM Users";
            return await _connection.QueryFirstOrDefaultAsync<DamoyeoUser>(sql, transaction: _transaction);
        }

        public Task<PagedList<DamoyeoUser>> GetPagedListAsync<U>(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task Remove(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(DamoyeoUser user)
        {
            throw new NotImplementedException();
        }
    }
}
