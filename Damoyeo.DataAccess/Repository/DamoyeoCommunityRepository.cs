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
    public class DamoyeoCommunityRepository : IDamoyeoCommunityRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoCommunityRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<DamoyeoCommunity> GetAsync(DamoyeoCommunity entity)
        {
            var sql = @"
SELECT * FROM Damoyeo_Community
  where 
  use_tf = '1'
  AND board_id = @board_id
";
            
            return await _connection.QueryFirstOrDefaultAsync<DamoyeoCommunity>(sql, entity, transaction: _transaction);
        }

        public async Task AddAsync(DamoyeoCommunity entity)
        {
            var sql = @"
INSERT INTO Damoyeo_Community(user_id, title, content, use_tf, post_date)
VALUES (@user_id, @title, @content, '1', @post_date);
";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }



        public async Task<PagedList<DamoyeoCommunity>> GetPagedListAsync(int page, int pageSize)
        {
            int startRange = ((page - 1) * pageSize) + 1;
            int endRange = startRange + pageSize - 1;

            var sql = @"
SELECT * FROM (
	SELECT ROW_NUMBER() over(order by board_id) as row_num, COUNT(*) over() total_count, board_id, user_id, title,content, use_tf, post_date FROM Damoyeo_Community
) A
WHERE 
A.use_tf = '1'
and A.row_num BETWEEN 1 AND 10
";
            var items = await _connection.QueryAsync<DamoyeoCommunity>(sql, new { startRange, endRange }, transaction: _transaction);
            if (items.Any())
            {
                return new PagedList<DamoyeoCommunity>(items, items.FirstOrDefault().total_count, page, pageSize);
            }
            else
            {
                return new PagedList<DamoyeoCommunity>(Enumerable.Empty<DamoyeoCommunity>(), 0, 0, 0);
            }
        }

        public async Task RemoveAsync(int id)
        {
            var sql = @"
UPDATE Damoyeo_Community
SET  use_tf = '0'
WHERE comment_id = @comment_id;
";
            await _connection.ExecuteAsync(sql, new { comment_id = id }, transaction: _transaction);
        }

        public async Task UpdateAsync(DamoyeoCommunity entity)
        {
            var sql = @"
UPDATE Damoyeo_Community
SET title = @title,
    content = @content
WHERE board_id = @board_id
";

            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }
    }
}
