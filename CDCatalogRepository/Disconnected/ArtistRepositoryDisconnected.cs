using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CDCatalogDALModel;

namespace CDCatalogRepository.Disconnected
{
    class ArtistRepositoryDisconnected : IRepository<Artist>
    {
        public List<Artist> find(Expression<Func<Artist, bool>> query, int skip = 0, int take = 300)
        {
            throw new NotImplementedException();
        }

        public List<Artist> findOne(Expression<Func<Artist, bool>> query)
        {
            throw new NotImplementedException();
        }

        public bool insert(Artist item)
        {
            throw new NotImplementedException();
        }

        public bool update(Artist item)
        {
            throw new NotImplementedException();
        }

        public bool delete(Artist item)
        {
            throw new NotImplementedException();
        }
    }
}
