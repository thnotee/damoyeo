using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.Model
{
    public class AuthMdoel
    {
        public int UserId { get; set; } // '키'
        public string Email { get; set; } // '이메일'
        public string ProfileImage { get; set; } // '이미지'
        public string SlfIntro { get; set; } // '자기소개'
        public string Nickname { get; set; } // '닉네임'

        /// <summary>
        /// 회원가입 타입 
        /// 0 : 다모여 회원가입 회원
        /// 1 : 다모여 카카오톡 가입 회원
        /// 2 : 다모여 관리자 
        /// </summary>
        public string SignupType { get; set; }
    }
}
