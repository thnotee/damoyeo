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
        /// <summary>
        /// 소모임 id
        /// </summary>
        public int meetup_id { get; set; }
        /// <summary>
        /// 소모임이름
        /// </summary>
        public string meetup_name { get; set; }

        /// <summary>
        /// 마스터 user_id
        /// </summary>
        public int meetup_master_id { get; set; }
        /// <summary>
        /// 조회수
        /// </summary>
        public int view_count { get; set; }
        /// <summary>
        /// 신청자수
        /// </summary>
        public int user_count { get; set; }
        /// <summary>
        /// 좋아요 수
        /// </summary>
        public int wish_count { get; set; }
        /// <summary>
        /// 최대 정원
        /// </summary>
        public int max_user_count { get; set; }
        public int use_tf { get; set; }
        public DateTime reg_date { get; set; }
        public string meetup_image { get; set; }
        
        public int category_id { get; set; }

        /// <summary>
        /// 간단 소개글
        /// </summary>
        public string meeting_intro { get; set; }
        /// <summary>
        /// 상세 소개글
        /// </summary>
        public string meetup_description { get; set; }

        /// <summary>
        /// 미팅 시작일
        /// </summary>
        public string meeting_sdate { get; set; }

        /// <summary>
        /// 미팅 시작일
        /// </summary>
        public string meeting_edate { get; set; }

        /// <summary>
        /// 신청 시작일
        /// </summary>
        public string application_sdate { get; set; }

        /// <summary>
        /// 신청 종료일
        /// </summary>
        public string application_edate { get; set; }

        /// <summary>
        /// 카카오톡 오픈챗 링크
        /// </summary>
        public string kakao_openchat_link { get; set; }

        /// <summary>
        /// 우편번호
        /// </summary>
        public string post_code { get; set; }
        /// <summary>
        /// 주소지명
        /// </summary>
        public string post_name { get; set; }
        /// <summary>
        /// 상세주소
        /// </summary>
        public string post_detail { get; set; }

        /// <summary>
        /// 핸드폰 번호
        /// </summary>
        public string phone_number { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 담당자 명
        /// </summary>
        public string person_name { get; set; }

        /// <summary>
        /// 정원초과 모집여부
        /// </summary>
        public string over_capacity { get; set; }

        /// <summary>
        /// 모임 노출여부
        /// </summary>
        public string meetup_display { get; set; }


        /// <summary>
        /// 법정동
        /// </summary>
        public string bname { get; set; }

        /// <summary>
        /// 경도
        /// </summary>
        public string longitude { get; set; }

        /// <summary>
        /// 위도
        /// </summary>
        public string latitude { get; set; }

        ///신청자 카운트
        public int applications_count  { get; set; }
    

        public DamoyeoCategory Category { get; set; }
        public DamoyeoUser User { get; set; }
    }
}
