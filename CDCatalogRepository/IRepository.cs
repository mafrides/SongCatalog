using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CDCatalogRepository
{
    public interface IRepository<T> where T : class
    {
        public List<T> find(Expression<Func<T,bool>> query, int skip = 0, int take = 300);
        public List<T> findOne(Expression<Func<T, bool>> query);
        public bool insert(T item);
        public bool update(T item);
        public bool delete(T item);
    }
}
