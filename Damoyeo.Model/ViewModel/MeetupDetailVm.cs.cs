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

        public DamoyeoMeetup detail { get; set; }

        public IEnumerable<DamoyeoImage> image { get; set; }

        public IEnumerable<DamoyeoMeetupTags> Tags { get; set; }

        public IEnumerable<DamoyeoApplications> applicationList { get; set; }
}
}
