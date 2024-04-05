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
    public class MeetupListVm
    {
        public PagedList<DamoyeoMeetup> list { get; set; }
        public MeetupSearchOpt MeetupSearchOpt { get; set; }

        public PagedList<DamoyeoCategory> categoryList { get; set; }

        public IEnumerable<DamoyeoWishlist> WishList { get; set; } = new List<DamoyeoWishlist>();
    }
}
