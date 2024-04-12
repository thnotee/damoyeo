using Damoyeo.Model.Model;
using Damoyeo.Model.Model.option;
using Damoyeo.Model.Model.Pager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.ViewModel
{
    public class MainVm
    {

        /// <summary>
        /// 최신순 리스트
        /// </summary>
        public PagedList<DamoyeoMeetup> latestList { get; set; }

        /// <summary>
        /// 인기순 리스트
        /// </summary>
        public PagedList<DamoyeoMeetup> popularityList { get; set; }

        
        public MeetupSearchOpt MeetupSearchOpt { get; set; }

        /// <summary>
        /// 카테고리 리스트
        /// </summary>
        public PagedList<DamoyeoCategory> categoryList { get; set; }
        

        /// <summary>
        /// 커뮤니티 리스트
        /// </summary>
        public PagedList<DamoyeoCommunity> communityList { get; set; }

        
        /// <summary>
        /// 위시리스트 
        /// </summary>

        public IEnumerable<DamoyeoWishlist> WishList { get; set; } = new List<DamoyeoWishlist>();


        /// <summary>
        /// 공지사항 리스트
        /// </summary>
        public PagedList<DamoyeoNotice> noticeList { get; set; }
    }
}
