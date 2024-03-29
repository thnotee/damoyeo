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
INSERT INTO Damoyeo_User (email, password, profile_image, slf_Intro, nickname, use_tf, signup_type, reg_date)
OUTPUT INSERTED.user_id
VALUES (@email, @password, @profile_image, @slf_Intro, @nickname, @use_tf, @signup_type, @reg_date);
";
           return await _connection.QuerySingleAsync<int>(sql, entity, transaction: _transaction);
        }

   

        public async Task<PagedList<DamoyeoUser>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {
            int startRange = ((page - 1) * pageSize) + 1;
            int endRange = startRange + pageSize - 1;

            var sql = @"
SELECT * FROM (
	SELECT 
		ROW_NUMBER() over(order by A.board_id) as row_num
		, COUNT(A.board_id) over() total_count
		, A.board_id
		, A.user_id
		, A.title
		, A.content
		, A.post_date 
		, B.nickname
		FROM Damoyeo_Community A INNER JOIN Damoyeo_User B ON A.user_id = B.user_id
		WHERE 
		A.use_tf = 1
) Z
WHERE 
Z.row_num BETWEEN @startRange AND @endRange;
";



            var items = await _connection.QueryAsync<DamoyeoUser>(sql, new { startRange, endRange }, transaction: _transaction);
            if (items.Any())
            {
                return new PagedList<DamoyeoUser>(items, items.FirstOrDefault().total_count, page, pageSize);
            }
            else
            {
                return new PagedList<DamoyeoUser>(Enumerable.Empty<DamoyeoUser>(), 0, 0, 0);
            }
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
    signup_type = @signup_type
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
And nickname = @nickname
";

            return await _connection.QueryFirstOrDefaultAsync<DamoyeoUser>(sql, entity, transaction: _transaction);
        }
    }
}
