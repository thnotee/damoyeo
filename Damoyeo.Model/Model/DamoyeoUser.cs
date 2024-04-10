using System;
using System.Collections.Generic;
using System.Text;

namespace Damoyeo.Model.Model
{
    public class DamoyeoUser
    {
        /// <summary>
        /// row_num
        /// </summary>
        public int row_num { get; set; }
        /// <summary>
        /// 토탈카운트
        /// </summary>
        public int total_count { get; set; }
        public int user_id { get; set; } // '키'
        public string email { get; set; } // '이메일'
        public string password { get; set; } // '비밀번호'
        public string profile_image { get; set; } // '이미지'
        public string slf_Intro { get; set; } // '자기소개'
        public string nickname { get; set; } // '닉네임'
        public string use_tf { get; set; } // '사용여부'

        public string stop_tf { get; set; } // '사용여부'

        /// <summary>
        /// 회원가입 타입 
        ///         1: 카카오톡 회원가입
        /// </summary>
        public int signup_type { get; set; }
        public DateTime reg_date { get; set; } // '가입일'
    }
}
