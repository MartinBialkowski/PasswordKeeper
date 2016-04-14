using System;
using System.Collections.Generic;

namespace DataAccessLayer
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Insert(T element);
        void Delete(int id);
        void Update(T element);
        void Save();
    }
}
