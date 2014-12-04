using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CDCatalogDALModel;

namespace CDCatalogRepository.Connected
{
    class SongRepositoryConnected : IRepository<Song>
    {
        public List<Song> find(Expression<Func<Song, bool>> query, int skip = 0, int take = 300)
        {
            throw new NotImplementedException();
        }

        public List<Song> findOne(Expression<Func<Song, bool>> query)
        {
            throw new NotImplementedException();
        }

        public bool insert(Song item)
        {
            throw new NotImplementedException();
        }

        public bool update(Song item)
        {
            throw new NotImplementedException();
        }

        public bool delete(Song item)
        {
            throw new NotImplementedException();
        }
    }
}
