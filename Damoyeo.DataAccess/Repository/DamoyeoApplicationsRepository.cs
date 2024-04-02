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
using static System.Net.Mime.MediaTypeNames;

namespace Damoyeo.DataAccess.Repository
{
    public class DamoyeoApplicationsRepository : IDamoyeoApplicationsRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoApplicationsRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<int> AddAsync(DamoyeoApplications entity)
        {
            var sql = @"
INSERT INTO Damoyeo_Applications (user_id, meetup_id, application_date)
VALUES (@user_id, @meetup_id, @application_date)
";
            
            return await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public async Task<IEnumerable<DamoyeoApplications>> GetAllAsync(DamoyeoApplications entity)
        {

            var whereSql = "";
            if (entity.meetup_id != 0) 
            {
                whereSql = "AND A.meetup_id = @meetup_id";            
            }

            if (entity.user_id != 0)
            {
                whereSql = "AND A.user_id = @user_id";
            }

            var sql = $@"
SELECT A.application_id
      ,A.user_id
      ,A.meetup_id
      ,A.application_date
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
      ,(select count(application_id) from Damoyeo_Applications where meetup_id = A.meetup_id) as user_count
	  ,D.category_name
FROM Damoyeo_Applications A INNER JOIN  Damoyeo_User  B ON (A.user_id = B.user_id)
INNER JOIN 
Damoyeo_Meetup C ON (A.meetup_id = C.meetup_id)
INNER JOIN 
Damoyeo_Category D ON (C.category_id = D.category_id)
WHERE 
B.use_tf = 1
{whereSql}
";

            return await _connection.QueryAsync<DamoyeoApplications, DamoyeoUser, DamoyeoMeetup, DamoyeoCategory, DamoyeoApplications>(sql, 
                (applications, user, meetup, category) => 
                {
                    applications.application_user = user;
                    meetup.Category = category;
                    applications.damoyeo_meetup = meetup;
                    return applications;
                }
                ,entity, _transaction
                ,splitOn : "nickname,max_user_count,category_name");
        }

        public async Task<DamoyeoApplications> GetAsync(DamoyeoApplications entity)
        {

            var sql = @"
SELECT application_id, user_id, meetup_id, application_date  FROM Damoyeo_Applications
WHERE 
meetup_id = @meetup_id
AND user_id = @user_id
";

            return await _connection.QueryFirstOrDefaultAsync<DamoyeoApplications>(sql, entity, _transaction);
        }

        public Task<PagedList<DamoyeoApplications>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(int id)
        {
            var sql = @"
delete from Damoyeo_Applications 
where 
application_id = @id
";

            await _connection.ExecuteAsync(sql, new { id }, _transaction);
        }

        public Task UpdateAsync(DamoyeoApplications entity)
        {
            throw new NotImplementedException();
        }
    }
}
