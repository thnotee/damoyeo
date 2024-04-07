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
using Damoyeo.Model.Model.option;
using Damoyeo.Model.Model.Procedure;

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
(parent_commentid, board_id, user_id, Content, comment_date, use_tf)
VALUES
(@parent_commentid, @board_id, @user_id, @content, @comment_date, @use_tf);
";
           return  await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }


        public async Task<PagedList<GetCommentTree>> GetPagedListAsync(int page, int pageSize, CommunitySearchOpt option)
        {

            int startRange = ((page - 1) * pageSize) + 1;
            int endRange = startRange + pageSize - 1;

            // 프로시저 매개변수
            
            var parameters = new DynamicParameters();
            parameters.Add("@board_id", option.board_id);
            parameters.Add("@startRange", startRange);
            parameters.Add("@endRange", endRange);            
            var items = await _connection.QueryAsync<GetCommentTree>("SP_GetCommentTree", parameters, commandType: CommandType.StoredProcedure, transaction : _transaction);

            if (items.Any())
            {
                return new PagedList<GetCommentTree>(items, items.FirstOrDefault().total_count, page, pageSize);
            }
            else
            {
                return new PagedList<GetCommentTree>(Enumerable.Empty<GetCommentTree>(), 0, 0, 0);
            }
            
        }


        public async Task<PagedList<GetCommentTree>> GetUserCommentPagedListAsync(int page, int pageSize, CommunitySearchOpt option)
        {

            int startRange = ((page - 1) * pageSize) + 1;
            int endRange = startRange + pageSize - 1;

            var sql = @"
 
SELECT A.* FROM (

 SELECT 
	  ROW_NUMBER() over(order by comment_date desc) as row_num
	  ,COUNT(comment_id) over() total_count
	  ,comment_id
      ,parent_commentid
      ,board_id
      ,user_id
      ,content
      ,comment_date
      ,use_tf
  FROM Damoyeo_Community_Comment 
  WHERE 
  user_id = @user_id
  and use_tf = 1
) A
WHERE
A.row_num BETWEEN @startRange AND @endRange
";


            var items = await _connection.QueryAsync<GetCommentTree>(sql,
         new { startRange, endRange, option.searchString, option.user_id },
         transaction: _transaction);


            if (items.Any())
            {
                return new PagedList<GetCommentTree>(items, items.FirstOrDefault().total_count, page, pageSize);
            }
            else
            {
                return new PagedList<GetCommentTree>(Enumerable.Empty<GetCommentTree>(), 0, 0, 0);
            }
        }

        public  Task<PagedList<DamoyeoCommunityComment>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {

            throw new NotImplementedException();
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
