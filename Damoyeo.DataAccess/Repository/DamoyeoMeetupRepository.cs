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

namespace Damoyeo.DataAccess.Repository
{
    public class DamoyeoMeetupRepository : IDamoyeoMeetupRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoMeetupRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<int> AddAsync(DamoyeoMeetup entity)
        {

            var sql = @"
INSERT INTO Damoyeo_Meetup (
    meetup_name,
    meetup_master_id,
    view_count,
    user_count,
    max_user_count,
    use_tf,
    reg_date,
    meetup_image,
    meetup_description,
    category_id,
    phone_number,
    email,
    meeting_intro,
    person_name,
    meeting_sdate,
    meeting_edate,
    application_sdate,
    application_edate,
    kakao_openchat_link,
    post_code,
    post_name,
    post_detail,
    over_capacity,
    meetup_display,
    bname
) 
OUTPUT INSERTED.meetup_id
VALUES (
    @meetup_name, 
    @meetup_master_id,
    @view_count, 
    @user_count, 
    @max_user_count, 
    @use_tf, 
    @reg_date, 
    @meetup_image, 
    @meetup_description, 
    @category_id, 
    @phone_number,
    @email, 
    @meeting_intro, 
    @person_name, 
    @meeting_sdate, 
    @meeting_edate, 
    @application_sdate,
    @application_edate, 
    @kakao_openchat_link,
    @post_code, 
    @post_name,
    @post_detail, 
    @over_capacity, 
    @meetup_display,
    @bname

);


";
            return await _connection.QuerySingleAsync<int>(sql, entity, _transaction);
        }

        public Task<DamoyeoMeetup> GetAsync(DamoyeoMeetup entity)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<DamoyeoMeetup>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {

            throw new NotImplementedException();
        }



        public async Task<PagedList<DamoyeoMeetup>> GetPagedListAsync(int page, int pageSize, MeetupSearchOpt option)
        {
            
            var whereSql = "";
            var orderby = "";

            if (option.searchOrder == 1) { orderby = "order by reg_date desc"; }
            else if (option.searchOrder == 2) { orderby = "order by view_count desc"; }
            else { orderby = "order by meeting_edate desc"; }


            int startRange = ((page - 1) * pageSize) + 1;
            int endRange = startRange + pageSize - 1;

            var sql = $@"
SELECT * FROM (
	SELECT  
			ROW_NUMBER() over({orderby}) as row_num
		   , COUNT(meetup_id) over() total_count
		  ,meetup_id
		  ,meetup_name
		  ,meetup_master_id
		  ,view_count
		  ,user_count
		  ,max_user_count
		  ,use_tf
		  ,reg_date
		  ,meetup_image
		  ,meetup_description
		  ,category_id
		  ,phone_number
		  ,email
		  ,meeting_intro
		  ,person_name
		  ,meeting_sdate
		  ,meeting_edate
		  ,application_sdate
		  ,application_edate
		  ,kakao_openchat_link
		  ,post_code
		  ,post_name
		  ,post_detail
		  ,over_capacity
		  ,meetup_display
		  ,bname
	  FROM Damoyeo_Meetup
	  where 
		application_sdate >= '2023/03/18'
		and application_edate <= '2024/05/30'
) X
WHERE 
use_tf = 1
AND X.row_num BETWEEN 1 AND 10 
";



            var items = await _connection.QueryAsync<DamoyeoMeetup>(sql, new { startRange, endRange }, transaction: _transaction);
            if (items.Any())
            {
                return new PagedList<DamoyeoMeetup>(items, items.FirstOrDefault().total_count, page, pageSize);
            }
            else
            {
                return new PagedList<DamoyeoMeetup>(Enumerable.Empty<DamoyeoMeetup>(), 0, 0, 0);
            }

            throw new NotImplementedException();
        }

        public Task RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(DamoyeoMeetup entity)
        {
            throw new NotImplementedException();
        }
    }
}
