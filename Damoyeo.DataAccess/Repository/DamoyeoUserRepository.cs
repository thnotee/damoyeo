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
  and email = @email;
";
            return await _connection.QueryFirstOrDefaultAsync<DamoyeoUser>(sql, entity, transaction: _transaction);
        }

        public async Task<int> AddAsync(DamoyeoUser entity)
        {
            var sql = @"
INSERT INTO Damoyeo_User (email, password, profile_image, slf_Intro, nickname, use_tf, signup_type, reg_date, stop_tf)
OUTPUT INSERTED.user_id
VALUES (@email, @password, @profile_image, @slf_Intro, @nickname, @use_tf, @signup_type, @reg_date, @stop_tf);
";
           return await _connection.QuerySingleAsync<int>(sql, entity, transaction: _transaction);
        }

   

        public async Task<PagedList<DamoyeoUser>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(DamoyeoUser entity)
        {

            var sql = @"
UPDATE Damoyeo_User
SET profile_image = @profile_image,
    password = @password,
    slf_Intro = @slf_Intro,
    nickname = @nickname,
    use_tf = @use_tf,
    signup_type = @signup_type,
    stop_tf = @stop_tf
WHERE user_id = @user_id;

";
            await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public async Task<DamoyeoUser> GetNicknameAsync(DamoyeoUser entity)
        {
            var sql = @"
SELECT * FROM Damoyeo_User
WHERE 
use_tf = '1'
And signup_type != '3'
And nickname = @nickname
";

            return await _connection.QueryFirstOrDefaultAsync<DamoyeoUser>(sql, entity, transaction: _transaction);
        }

        public async Task<PagedList<DamoyeoUser>> GetUserPagedListAsync(int page, int pageSize, string searchString = "", int type = 0)
        {
            int startRange = ((page - 1) * pageSize) + 1;
            int endRange = startRange + pageSize - 1;
            string whereSql = "AND A.signup_type != 3";
            //관리자 목록 가져오기
            if (type != 0) 
            {
                whereSql = "AND A.signup_type = 3";
            }
            if (!string.IsNullOrEmpty(searchString)) 
            {
                whereSql += "AND A.nickname Like '%'+@searchString+'%'";
            }


            var sql = $@"
  SELECT * FROM (
  	SELECT 
		ROW_NUMBER() over(order by A.user_id) as row_num
		,COUNT(A.user_id) over() total_count
		,A.user_id
		,A.email
		,A.profile_image
		,A.slf_Intro
		,A.nickname
		,A.use_tf
		,A.reg_date
		,A.signup_type
		,A.stop_tf
		FROM Damoyeo_User A 
		WHERE 
		A.use_tf = 1
		{whereSql}
		) Z
WHERE 
Z.row_num BETWEEN 1 AND 10;
";



            var items = await _connection.QueryAsync<DamoyeoUser>(sql, new { startRange, endRange, searchString }, transaction: _transaction);
            if (items.Any())
            {
                return new PagedList<DamoyeoUser>(items, items.FirstOrDefault().total_count, page, pageSize);
            }
            else
            {
                return new PagedList<DamoyeoUser>(Enumerable.Empty<DamoyeoUser>(), 0, 0, 0);
            }
        }
    }
}
