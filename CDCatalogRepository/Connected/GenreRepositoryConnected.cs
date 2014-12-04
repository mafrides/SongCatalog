using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CDCatalogDALModel;

namespace CDCatalogRepository.Connected
{
    class GenreRepositoryConnected : IRepository<Genre>
    {
        public List<Genre> find(Expression<Func<Genre, bool>> query, int skip = 0, int take = 300)
        {
            throw new NotImplementedException();
        }

        public List<Genre> findOne(Expression<Func<Genre, bool>> query)
        {
            throw new NotImplementedException();
        }

        public bool insert(Genre item)
        {
            throw new NotImplementedException();
        }

        public bool update(Genre item)
        {
            throw new NotImplementedException();
        }

        public bool delete(Genre item)
        {
            throw new NotImplementedException();
        }
    }
}
