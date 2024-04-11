using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.Model
{
    public class DamoyeoNotice
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
        /// 보드 코드
        /// </summary>
        public int board_id { get; set; }
        /// <summary>
        /// 작성유저
        /// </summary>
        public int user_id { get; set; }
        /// <summary>
        /// 제목
        /// </summary>
        public string title { get; set; }
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

        public int view_count { get; set; }

        /// <summary>
        /// 작성일
        /// </summary>
        public DateTime post_date { get; set; }

        public DamoyeoUser User { get; set; }
        
    }
}
