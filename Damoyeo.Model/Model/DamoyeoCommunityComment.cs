using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.Model
{
    public class DamoyeoCommunityComment
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
        public string use_tf { get; set; }
        
        /// <summary>
        /// 작성일 
        /// </summary>
        public DateTime comment_date { get; set; }
    }
}
