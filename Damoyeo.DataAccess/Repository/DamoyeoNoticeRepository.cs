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
    public class DamoyeoNoticeRepository : IDamoyeoNoticeRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoNoticeRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<DamoyeoNotice> GetAsync(DamoyeoNotice entity)
        {
            var sql = @"
  SELECT * FROM Damoyeo_Notice
  where 
        use_tf = '1'
        and board_id = @board_id;
";
            
            return await _connection.QueryFirstOrDefaultAsync<DamoyeoNotice>(sql, entity, transaction: _transaction);
        }

        public async Task AddAsync(DamoyeoNotice entity)
        {
            var sql = @"
INSERT INTO Damoyeo_Notice
(user_id, Title, Content, use_tf, post_date)
VALUES
(@user_id, @title, @content, @use_tf, GETDATE());
";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }



        public async Task<PagedList<DamoyeoNotice>> GetPagedListAsync(int page, int pageSize)
        {
            int startRange = ((page - 1) * pageSize) + 1;
            int endRange = startRange + pageSize - 1;

            var sql = @"
SELECT * FROM (
	SELECT ROW_NUMBER() over(order by board_id) as row_num, COUNT(*) over() total_count, board_id, user_id, title,content, use_tf, post_date FROM Damoyeo_Notice
) A
WHERE 
A.use_tf = '1'
and A.row_num BETWEEN @startRange AND @endRange
";
            var items = await _connection.QueryAsync<DamoyeoNotice>(sql, new { startRange, endRange }, transaction: _transaction);
            if (items.Any())
            {
                return new PagedList<DamoyeoNotice>(items, items.FirstOrDefault().total_count, page, pageSize);
            }
            else
            {
                return new PagedList<DamoyeoNotice>(Enumerable.Empty<DamoyeoNotice>(), 0, 0, 0);
            }
        }

        public async Task RemoveAsync(int id)
        {
            var sql = @"
UPDATE Damoyeo_Notice
SET  use_tf = '0'
WHERE comment_id = @comment_id;
";
            await _connection.ExecuteAsync(sql, new { comment_id = id }, transaction: _transaction);
        }

        public async Task UpdateAsync(DamoyeoNotice entity)
        {
            var sql = @"
UPDATE Damoyeo_Notice
SET title = @title,
    content = @content
WHERE board_id = @board_id
";

            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }
    }
}
