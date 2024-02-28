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
        public string Slf_Intro { get; set; } // '자기소개'
        public string Nickname { get; set; } // '닉네임'
    }
}
