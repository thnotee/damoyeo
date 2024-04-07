using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.Model.Procedure
{
    public class GetCommentTree
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
        /// 댓글 ID
        /// </summary>
        public int comment_id { get; set; }
        /// <summary>
        /// 부모 댓글
        /// </summary>
        public int parent_commentid { get; set; }
        /// <summary>
        /// 보드 코드
        /// </summary>
        public int board_id { get; set; }
        /// <summary>
        /// 작성자
        /// </summary>
        public int user_id { get; set; }
        /// <summary>
        /// 내용
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 사용여부 
        ///          1 : 사용 
        ///          0 : 삭제
        /// </summary>
        public string comment_use_tf { get; set; }

        /// <summary>
        /// 작성일 
        /// </summary>
        public DateTime comment_date { get; set; }

        public int level { get; set; }

        public string email { get; set; } // '이메일'
        public string profile_image { get; set; } // '이미지'
        public string slf_Intro { get; set; } // '자기소개'
        public string nickname { get; set; } // '닉네임'
        public string user_use_tf { get; set; } // '사용여부'

    }

}
