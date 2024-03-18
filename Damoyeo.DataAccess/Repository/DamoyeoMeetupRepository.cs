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

            var sql  = @"
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
    meetup_display
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
    @meetup_display
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
