using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.Model
{

    /// <summary>
    /// 유저 관심 카테고리 
    /// </summary>
    public class DamoyeoUserInterestCategory
    {
        public int Interest_id { get; set; }
        public int user_id { get; set; }
        public int category_id { get; set; }
    }
}

