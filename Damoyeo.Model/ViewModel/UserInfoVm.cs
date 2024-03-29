using Damoyeo.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.ViewModel
{
    public class UserInfoVm
    {
        
        public IEnumerable<DamoyeoUserInterestCategory> interestCategoryList { get; set; }

        public DamoyeoUser userInfo { get; set; }

        public string newPassword { get; set; }

    }
}
