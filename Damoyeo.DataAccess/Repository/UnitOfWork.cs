using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.DataAccess.Repository
{

    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public IDamoyeoUserRepository Users { get; private set; }

        public IDamoyeoCommunityRepository Community { get; private set; }

        public IDamoyeoCommunityCommentRepository CommunityComment { get; private set; }

        public IDamoyeoNoticeRepository Notice { get; private set; }

        public UnitOfWork(IDbConnection connection)
        {
            _connection = connection;
            _connection.Open();
            _transaction = _connection.BeginTransaction();
            Users = new DamoyeoUserRepository(_transaction);
            Community = new DamoyeoCommunityRepository(_transaction);
            CommunityComment = new DamoyeoCommunityCommentRepository(_transaction);
            Notice = new DamoyeoNoticeRepository(_transaction);

        }

   

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch (Exception ex)
            {
                
                _transaction.Rollback();
                throw;
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Connection?.Close();
                _transaction.Connection?.Dispose();
                _transaction.Dispose();
            }
        }
    }
   
}
