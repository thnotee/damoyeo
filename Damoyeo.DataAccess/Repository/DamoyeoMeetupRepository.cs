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
    bname,
    longitude,
    latitude
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
    @bname,
    @longitude,
    @latitude

);


";
            return await _connection.QuerySingleAsync<int>(sql, entity, _transaction);
        }

        public async Task<DamoyeoMeetup> GetAsync(DamoyeoMeetup entity)
        {
            var sql = @"
SELECT A.meetup_id
      ,A.meetup_name
      ,A.meetup_master_id
      ,A.view_count
      ,A.user_count
      ,A.max_user_count
      ,A.use_tf
      ,A.reg_date
      ,A.meetup_image
      ,A.meetup_description
      ,A.category_id
      ,A.phone_number
      ,A.email
      ,A.meeting_intro
      ,A.person_name
      ,A.meeting_sdate
      ,A.meeting_edate
      ,A.application_sdate
      ,A.application_edate
      ,A.kakao_openchat_link
      ,A.post_code
      ,A.post_name
      ,A.post_detail
      ,A.over_capacity
      ,A.meetup_display
      ,A.bname
      ,A.longitude
      ,A.latitude
	  ,B.category_name
  FROM Damoyeo_Meetup A inner join Damoyeo_Category B on A.category_id = b.category_id
  where 
  use_tf = 1
  and meetup_id = @meetup_id

";



            var item = await _connection.QueryAsync<DamoyeoMeetup, DamoyeoCategory, DamoyeoMeetup>(
             sql,
             (meetup, category) =>
             {
                 meetup.Category = category;
                 return meetup;
             },
             entity,
             transaction: _transaction,
             splitOn: "category_name");

            return item.FirstOrDefault();
          
        }


        public Task<PagedList<DamoyeoMeetup>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {

            throw new NotImplementedException();
        }



        public async Task<PagedList<DamoyeoMeetup>> GetPagedListAsync(int page, int pageSize, MeetupSearchOpt option)
        {
            
            var whereSql = "";
            var orderby = "";


            //신청일자 존재시
            if (!string.IsNullOrEmpty(option.applicationSdate) && !string.IsNullOrEmpty(option.applicationEdate))
            {
                whereSql += " AND @applicationSdate BETWEEN A.application_sdate AND A.application_edate ";
            }


            //검색어 존재시
            if (!string.IsNullOrEmpty(option.searchString)) 
            {
                whereSql += "AND A.meetup_name LIKE '%'+@searchString+'%'";
            }

            //검색지역 존재시
            if (!string.IsNullOrEmpty(option.searchArea))
            {

                if (option.searchArea.IndexOf("/") != -1)
                {
                    var tempArr = option.searchArea.Trim().Split('/');
                    option.temp1 = tempArr[0];
                    option.temp2 = tempArr[1];
                    whereSql += "AND (A.post_name LIKE '%'+@temp1+'%' OR A.post_name LIKE '%'+@temp2+'%')";
                }
                else 
                {
                    whereSql += "AND A.post_name LIKE '%'+@searchArea+'%'";
                }
              
            }

            //카테고리 검색 시 
            if (option.searchCategory != 0) 
            {
                whereSql += "AND A.category_id = @searchCategory";
            }


            //모임 마스터 검색
            if (option.userId != 0)
            {
                whereSql += "AND  A.meetup_master_id = @userId";
            }




            if (option.searchOrder == 1) { orderby = "order by A.reg_date desc"; }
            else if (option.searchOrder == 2) { orderby = "order by A.view_count desc"; }
            else { orderby = "order by A.meeting_edate desc"; }


            option.startRange = ((page - 1) * pageSize) + 1;
            option.endRange = option.startRange + pageSize - 1;

            var sql = $@"
SELECT * FROM (
	SELECT  
			ROW_NUMBER() over({orderby}) as row_num
		   , COUNT(A.meetup_id) over() total_count
		  ,A.meetup_id
		  ,A.meetup_name
		  ,A.meetup_master_id
		  ,A.view_count
		  ,A.user_count
		  ,A.max_user_count
		  ,A.use_tf
		  ,A.reg_date
		  ,A.meetup_image
		  ,A.meetup_description
		  ,A.category_id
		  ,A.phone_number
		  ,A.email
		  ,A.meeting_intro
		  ,A.person_name
		  ,A.meeting_sdate
		  ,A.meeting_edate
		  ,A.application_sdate
		  ,A.application_edate
		  ,A.kakao_openchat_link
		  ,A.post_code
		  ,A.post_name
		  ,A.post_detail
		  ,A.over_capacity
		  ,A.meetup_display
		  ,A.bname
          ,A.longitude
          ,A.latitude
          ,(SELECT COUNT(application_id) FROM Damoyeo_Applications where meetup_id = A.meetup_id ) as applications_count
          ,B.category_name
	  FROM Damoyeo_Meetup A INNER JOIN Damoyeo_Category B on A.category_id = b.category_id
	  where 
        A.use_tf = 1
        {whereSql}
) X
WHERE 
 X.row_num BETWEEN @startRange AND @endRange
";



            var items = await _connection.QueryAsync<DamoyeoMeetup, DamoyeoCategory, DamoyeoMeetup>(sql,
                (meetup, category) =>
                {
                     meetup.Category = category;
                     return meetup;
                },
                option, transaction: _transaction, splitOn: "category_name");
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

        public async Task UpdateAsync(DamoyeoMeetup entity)
        {

            var sql = @"

UPDATE Damoyeo_Meetup
SET 
    meetup_name = @meetup_name, 
    meetup_master_id = @meetup_master_id,
    view_count = @view_count, 
    user_count = @user_count, 
    max_user_count = @max_user_count, 
    use_tf = @use_tf, 
    reg_date = @reg_date, 
    meetup_image = @meetup_image, 
    meetup_description = @meetup_description, 
    category_id = @category_id, 
    phone_number = @phone_number,
    email = @email, 
    meeting_intro = @meeting_intro, 
    person_name = @person_name, 
    meeting_sdate = @meeting_sdate, 
    meeting_edate = @meeting_edate, 
    application_sdate = @application_sdate,
    application_edate = @application_edate, 
    kakao_openchat_link = @kakao_openchat_link,
    post_code = @post_code, 
    post_name = @post_name,
    post_detail = @post_detail, 
    over_capacity = @over_capacity, 
    meetup_display = @meetup_display,
    bname = @bname,
    longitude = @longitude,
    latitude = @latitude
WHERE meetup_id = @meetup_id;

";


            await _connection.ExecuteAsync(sql, entity, _transaction);
        }
    }
}
