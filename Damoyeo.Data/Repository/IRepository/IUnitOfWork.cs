using Damoyeo.Model.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Damoyeo.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<DamoyeoUser> Users { get; }
        void BeginTransaction();
        void Commit();
    }
}
