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
    public class DamoyeoCommunityCommentRepository : IDamoyeoCommunityCommentRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoCommunityCommentRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<DamoyeoCommunityComment> GetAsync(DamoyeoCommunityComment entity)
        {
            var sql = @"
SELECT * FROM Damoyeo_Community_Comment
  where 
comment_id = @comment_id;
  
";
            return await _connection.QueryFirstOrDefaultAsync<DamoyeoCommunityComment>(sql, transaction: _transaction);
        }

        public async Task<int> AddAsync(DamoyeoCommunityComment entity)
        {
            var sql = @"
INSERT INTO Damoyeo_Community_Comment
(parent_commentid, board_id, user_id, Content, comment_date)
VALUES
(@parent_commentid, @board_id, @user_id, @content, @comment_date);
";
           return  await _connection.QuerySingleAsync<int>(sql, entity, transaction: _transaction);
        }



        public async Task<PagedList<DamoyeoCommunityComment>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {
            int startRange = ((page - 1) * pageSize) + 1;
            int endRange = startRange + pageSize - 1;

            

            var searchSql = "";
            if (!string.IsNullOrEmpty(searchString)) {
                searchSql = $" AND title like '%'{searchSql}'%' ";
            }

            var sql = $@"
WITH CommentTree AS (
    SELECT 
        comment_id, 
        parent_commentid, 
        content, 
        1 AS Level, 
        RIGHT('00000' + CAST(comment_id AS VARCHAR(MAX)), 5) AS ord
    FROM Damoyeo_Community_Comment
    WHERE parent_commentid IS NULL AND board_id = 1
    UNION ALL
    SELECT 
        c.comment_id, 
        c.parent_commentid, 
        c.content, 
        Level + 1, 
        ord + '>' + RIGHT('00000' + CAST(c.comment_id AS VARCHAR(MAX)), 5) as ord
    FROM Damoyeo_Community_Comment c
    INNER JOIN CommentTree ct ON c.parent_commentid = ct.comment_id
    WHERE c.comment_id > ct.comment_id
)
SELECT * FROM (
	SELECT 
    ROW_NUMBER() OVER(ORDER BY A.ord ) AS row_num, 
	COUNT(*) OVER() AS total_count,
	A.*
  FROM CommentTree  A
) B
WHERE
B.ROW_NUM BETWEEN  @startRange AND @endRange;


";
            var items = await _connection.QueryAsync<DamoyeoCommunityComment>(sql, new { startRange, endRange }, transaction: _transaction);
            if (items.Any())
            {
                return new PagedList<DamoyeoCommunityComment>(items, items.FirstOrDefault().total_count, page, pageSize);
            }
            else 
            {
                return new PagedList<DamoyeoCommunityComment>(Enumerable.Empty<DamoyeoCommunityComment>(),0,0,0);
            }
            
        }

        public async Task RemoveAsync(int id)
        {
            var sql = @"
UPDATE Damoyeo_Community_Comment
SET  use_tf = '0'
WHERE comment_id = @comment_id;
";
            await _connection.ExecuteAsync(sql, new { comment_id = id}, transaction: _transaction);
        }

        public async Task UpdateAsync(DamoyeoCommunityComment entity)
        {

            var sql = @"
UPDATE Damoyeo_Community_Comment
SET content = @content
WHERE comment_id = @comment_id;
";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }
    }
}
