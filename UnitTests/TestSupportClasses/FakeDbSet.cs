using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CDCatalogDAL;

namespace CDCatalogTests
{
    //T must have Id property of type int
    public class FakeDbSet<T> : DbSet<T>, IQueryable, IEnumerable<T> where T : class, new()
    {
        readonly ObservableCollection<T> items;
        readonly IQueryable query;

        public FakeDbSet()
        {
            items = new ObservableCollection<T>();
            query = items.AsQueryable();
        }

        public override T Add(T entity)
        {
            items.Add(entity);
            return entity;
        }

        public override T Attach(T entity)
        {
            items.Add(entity);
            return entity;
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override T Create()
        {
            return new T();
        }

        public override T Find(params object[] keyValues)
        {
            var keyvalue = (int)keyValues.FirstOrDefault();
            var idProperty = typeof(T).GetProperty("Id"); //convention of all Model Objects
            return items.SingleOrDefault(o => (int)(idProperty.GetValue(o)) == keyvalue);
        }

        public override T Remove(T entity)
        {
            items.Remove(entity);
            return entity;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public Type ElementType
        {
            get { return query.ElementType; }
        }

        public Expression Expression
        {
            get { return query.Expression; }
        }

        Expression IQueryable.Expression
        {
            get { return query.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return query.Provider; }
        }

        public override IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            if (entities == null) return null;
            foreach (var entity in entities)
            {
                items.Add(entity);
            }
            return items;
        }

        public override IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null) return null;
            foreach (var entity in entities)
            {
                items.Remove(entity);
            }
            return items;
        }

        public override ObservableCollection<T> Local
        {
            get { return items; }
        }
    }
}
