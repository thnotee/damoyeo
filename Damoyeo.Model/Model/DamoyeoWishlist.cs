﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.Model
{
    public class DamoyeoWishlist
    {

        public int wish_id { get; set; }

        public int user_id { get; set; }

        public int meetup_id { get; set; }

        public DateTime wish_date { get; set; }

        public DamoyeoUser application_user { get; set; }

        public DamoyeoMeetup damoyeo_meetup { get; set; }
    }
}