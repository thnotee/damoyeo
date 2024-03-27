﻿using Damoyeo.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.Pager;
using static Dapper.SqlMapper;

namespace Damoyeo.DataAccess.Repository
{
    public class DamoyeoImageRepository : IDamoyeoImageRepository
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DamoyeoImageRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<int> AddAsync(DamoyeoImage entity)
        {
            var sql = @"
INSERT INTO Damoyeo_Image (save_filename, origin_filename, table_name, directory_path, table_id)
OUTPUT INSERTED.ID
VALUES (@save_filename, @origin_filename, @table_name, @directory_path, @table_id);
";
            return await _connection.QuerySingleAsync<int>(sql, entity, _transaction);
        }

        public Task<IEnumerable<DamoyeoImage>> GetAllAsync(DamoyeoImage entity)
        {
            var sql = @"
SELECT Id
      ,save_filename
      ,origin_filename
      ,table_name
      ,directory_path
      ,table_id
  FROM Damoyeo_Image
  where 
  table_id = @table_id
  and table_name = @table_name

";
            return _connection.QueryAsync<DamoyeoImage>(sql, entity, _transaction);
        }

        public Task<DamoyeoImage> GetAsync(DamoyeoImage entity)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<DamoyeoImage>> GetPagedListAsync(int page, int pageSize, string searchString = "")
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RemoveTableIdAsync(DamoyeoImage entity)
        {
            var sql = @"
    delete from Damoyeo_Image where  table_id = @table_id and table_name = @table_name
";
            return await _connection.ExecuteAsync(sql, entity, _transaction); 
        }

        public Task UpdateAsync(DamoyeoImage entity)
        {
            throw new NotImplementedException();
        }
    }
}
