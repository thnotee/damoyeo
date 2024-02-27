using System;
using System.Collections.Generic;
using System.Text;

namespace Damoyeo.Model.Model
{
    public class DamoyeoUser
    {
        public int UserId { get; set; } // '키'
        public string Email { get; set; } // '이메일'
        public string Password { get; set; } // '비밀번호'
        public string ProfileImage { get; set; } // '이미지'
        public string Slf_Intro { get; set; } // '자기소개'
        public string Nickname { get; set; } // '닉네임'
        public string Use_Tf { get; set; } // '사용여부'
        public DateTime Reg_Date { get; set; } // '가입일'
    }
}
