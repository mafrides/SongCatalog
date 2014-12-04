﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CDCatalogDALModel;

namespace CDCatalogRepository.Disconnected
{
    class AlbumRepositoryDisconnected : IRepository<Album>
    {
        public List<Album> find(Expression<Func<Album, bool>> query, int skip = 0, int take = 300)
        {
            throw new NotImplementedException();
        }

        public List<Album> findOne(Expression<Func<Album, bool>> query)
        {
            throw new NotImplementedException();
        }

        public bool insert(Album item)
        {
            throw new NotImplementedException();
        }

        public bool update(Album item)
        {
            throw new NotImplementedException();
        }

        public bool delete(Album item)
        {
            throw new NotImplementedException();
        }
    }
}