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
using System.Data.Common;
using Damoyeo.Model.Model.Procedure;
using Damoyeo.Model.Model.option;

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
SELECT A.board_id
      ,A.user_id
      ,A.title
      ,A.content
      ,A.use_tf
      ,A.post_date
      ,A.view_count
	  ,B.email
	  ,B.profile_image
	  ,B.slf_Intro
	  ,B.nickname
  FROM Damoyeo_Community A INNER JOIN Damoyeo_User B ON A.user_id = B.user_id
  WHERE 
  A.use_tf = 1
  AND B.use_tf =1
  AND A.board_id  = @board_id

";

            var item = await _connection.QueryAsync<DamoyeoCommunity, DamoyeoUser, DamoyeoCommunity>(
       sql,
       (community, user) =>
       {
           community.User = user;
           return community;
       },
       entity,
       transaction: _transaction,
       splitOn: "email");

            return item.FirstOrDefault();
        }

        public async Task<int> AddAsync(DamoyeoCommunity entity)
        {
            var sql = @"
INSERT INTO Damoyeo_Community(user_id, title, content, use_tf, post_date)
OUTPUT INSERTED.board_id
VALUES (@user_id, @title, @content, '1', @post_date);
";
            return await _connection.QuerySingleAsync<int>(sql, entity, transaction: _transaction);
        }



    
        public async Task<PagedList<DamoyeoCommunity>> GetPagedListAsync(int page, int pageSize, CommunitySearchOpt option)
        {
            int startRange = ((page - 1) * pageSize) + 1;
            int endRange = startRange + pageSize - 1;


            var searchSql = "";
            if (!string.IsNullOrEmpty(option.searchString))
            {
                searchSql = $" AND title like '%'+@searchString+'%' ";
            }

            
            if (option.user_id > 0)
            {
                searchSql = $" AND A.user_id = @user_id";
            }

            var sql = $@"
SELECT * FROM (
	SELECT 
		ROW_NUMBER() over(order by A.post_date desc) as row_num
		, COUNT(A.board_id) over() total_count
		, A.board_id
		, A.user_id
		, A.title
		, A.content
		, A.post_date 
        , A.view_count
        , (SELECT COUNT(*) as count FROM Damoyeo_Community_Comment WHERE  board_id = A.board_id) as comment_count
		, B.nickname
		FROM Damoyeo_Community A INNER JOIN Damoyeo_User B ON A.user_id = B.user_id
		WHERE 
		A.use_tf = 1
        {searchSql}
) Z
WHERE 
Z.row_num BETWEEN @startRange AND @endRange;
";


            var items = await _connection.QueryAsync<DamoyeoCommunity, DamoyeoUser, DamoyeoCommunity>(
               sql,
               (community, user) =>
               {
                   community.User = user;
                   return community;
               },
               new { startRange, endRange, option.searchString, option.user_id },
               transaction: _transaction,
               splitOn: "nickname"
           );


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
WHERE board_id = @board_id;
";
            await _connection.ExecuteAsync(sql, new { board_id = id }, transaction: _transaction);
        }

        public async Task UpdateAsync(DamoyeoCommunity entity)
        {
            var sql = @"
UPDATE Damoyeo_Community
SET title = @title,
    content = @content,
    view_count = @view_count
WHERE board_id = @board_id;
";

            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }

        public Task<PagedList<DamoyeoCommunity>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {
            throw new NotImplementedException();
        }
    }
}
