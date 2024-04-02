using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.Model.option
{
    public class MeetupSearchOpt
    {

     

        /// <summary>
        /// 신청시작일
        /// </summary>
        public string applicationSdate { get; set; }
        /// <summary>
        /// 신청종료일
        /// </summary>
        public string applicationEdate { get; set; }

        /// <summary>
        /// 검색어
        /// </summary>
        public string searchString { get; set; }
        /// <summary>
        /// 지역(시,도)
        /// </summary>
        public string searchArea { get; set; }

        /// <summary>
        /// 카테고리
        /// </summary>
        public int searchCategory { get; set; }

        /// <summary>
        /// 정렬 순서
        ///         1:인기순
        ///         2:최신순
        ///         3:마감임박순
        /// </summary>
        public int searchOrder { get; set; }

        /*
         *  dapper 이용 데이터
         */
        public int startRange { get; set; }

        public int endRange { get; set; }

        public string temp1 { get; set; }

        public string temp2 { get; set; }

        
        /// <summary>
        /// 마스터 id 가져오기위해
        /// 내가 open한 소모임 판단여부
        /// </summary>
        public int userId { get; set; }
    }
}
