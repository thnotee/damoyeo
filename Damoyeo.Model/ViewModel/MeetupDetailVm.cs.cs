using Damoyeo.Model.Model.Pager;
using Damoyeo.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.ViewModel
{
    public class MeetupDetailVm
    {
        
        /// <summary>
        /// 소모임정보
        /// </summary>
        public DamoyeoMeetup detail { get; set; }

        /// <summary>
        /// 소모임이미지
        /// </summary>

        public IEnumerable<DamoyeoImage> image { get; set; }
        

        /// <summary>
        /// 소모임태그
        /// </summary>

        public IEnumerable<DamoyeoMeetupTags> Tags { get; set; }

        /// <summary>
        /// 신청자목록
        /// </summary>

        public IEnumerable<DamoyeoApplications> applicationList { get; set; }

        /// <summary>
        /// 위시리스트 
        /// </summary>

        public IEnumerable<DamoyeoWishlist> WishList { get; set; } = new List<DamoyeoWishlist>();

    }
}
