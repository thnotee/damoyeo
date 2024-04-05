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


namespace Damoyeo.DataAccess.Repository
{
    public class DamoyeoWishlistRepository : IDamoyeoWishlistRepository
    {

        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoWishlistRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }
        public async Task<int> AddAsync(DamoyeoWishlist entity)
        {
            var sql = @"
INSERT INTO Damoyeo_Wishlist (user_id, meetup_id, wish_date)
VALUES (@user_id, @meetup_id, @wish_date)
";
            return await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public async Task<IEnumerable<DamoyeoWishlist>> GetAllAsync(DamoyeoWishlist entity)
        {
            var whereSql = "";
            
            if (entity.user_id != 0)
            {
                whereSql = "AND A.user_id = @user_id";
            }

            var sql = $@"
SELECT A.wish_id
      ,A.user_id
      ,A.meetup_id
      ,A.wish_date
	  ,B.nickname
      ,B.email
	  ,B.profile_image
      ,C.max_user_count
	  ,C.meetup_id 
	  ,C.meetup_name
	  ,C.meetup_image
	  ,C.kakao_openchat_link
	  ,C.bname
      ,C.application_edate
	  ,D.category_name
FROM Damoyeo_Wishlist A INNER JOIN  Damoyeo_User  B ON (A.user_id = B.user_id)
INNER JOIN 
Damoyeo_Meetup C ON (A.meetup_id = C.meetup_id)
INNER JOIN 
Damoyeo_Category D ON (C.category_id = D.category_id)
WHERE 
B.use_tf = 1
{whereSql}
";

            return await _connection.QueryAsync<DamoyeoWishlist, DamoyeoUser, DamoyeoMeetup, DamoyeoCategory, DamoyeoWishlist>(
                sql,
                (wish, user, meetup, category) =>
                {
                    wish.application_user = user;
                    meetup.Category = category;
                    wish.damoyeo_meetup = meetup;
                    return wish;
                }
                , entity, _transaction
                , splitOn: "nickname,max_user_count,category_name");

        }

        public async Task<DamoyeoWishlist> GetAsync(DamoyeoWishlist entity)
        {
            var sql = @"
SELECT wish_id, user_id, meetup_id, wish_date  FROM Damoyeo_Wishlist
WHERE 
meetup_id = @meetup_id
AND user_id = @user_id
";


            return await _connection.QueryFirstOrDefaultAsync<DamoyeoWishlist>(sql, entity, _transaction);
        }

        public Task<PagedList<DamoyeoWishlist>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(int id)
        {

            var sql = @"
delete from Damoyeo_Wishlist 
where 
wish_id = @id
";

            await _connection.ExecuteAsync(sql, new { id }, _transaction);
        }

        public Task UpdateAsync(DamoyeoWishlist entity)
        {
            throw new NotImplementedException();
        }
    }
}
