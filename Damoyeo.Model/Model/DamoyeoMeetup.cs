using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.Model
{
    public class DamoyeoMeetup
    {
        /// <summary>
        /// row_num
        /// </summary>
        public int row_num { get; set; }
        /// <summary>
        /// 토탈카운트
        /// </summary>
        public int total_count { get; set; }
        public int meetup_id { get; set; }
        public string meetup_name { get; set; }
        public int meetup_master_id { get; set; }
        public int view_count { get; set; }
        public int user_count { get; set; }
        public int max_user_count { get; set; }
        public int use_tf { get; set; }
        public DateTime reg_date { get; set; }
        public string meetup_image { get; set; }
        public string meetup_description { get; set; }
        public int category_id { get; set; }
        //public Category Category { get; set; }
    }
}
