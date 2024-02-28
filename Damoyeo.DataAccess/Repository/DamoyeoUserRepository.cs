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
    public class DamoyeoUserRepository : IDamoyeoUserRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoUserRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<DamoyeoUser> GetAsync(DamoyeoUser entity)
        {
            var sql = @"
SELECT * FROM Damoyeo_User
  where 
  use_tf = '1'
";
            //and email = @Email
            //new { Email = entity.Email }
            return await _connection.QueryFirstOrDefaultAsync<DamoyeoUser>(sql, transaction: _transaction);
        }

        public async Task AddAsync(DamoyeoUser entity)
        {
            var sql = @"
INSERT INTO Damoyeo_User (email, password, profile_image, slf_Intro, nickname, use_tf, reg_date)
VALUES (@Email, @Password, @ProfileImage, @Slf_Intro, @Nickname, @Use_Tf, @Reg_Date);
";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
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
