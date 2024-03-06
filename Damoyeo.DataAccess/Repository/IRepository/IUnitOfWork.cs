using Damoyeo.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork 
    {
        IDamoyeoUserRepository Users { get; }

        IDamoyeoCommunityRepository Community { get; }
        IDamoyeoCommunityCommentRepository CommunityComment { get; }

        IDamoyeoNoticeRepository Notice { get; }

        void Commit();
    }
}
