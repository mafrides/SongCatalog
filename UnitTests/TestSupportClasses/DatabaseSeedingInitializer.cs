using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using CDCatalogDAL;
using CDCatalogModel;

namespace CDCatalogTests
{
    public class DatabaseSeedingInitializer : DropCreateDatabaseAlways<SongCatalogContext>
    {
        protected override void Seed(SongCatalogContext context)
        {
            //initialization code here.........
            base.Seed(context);
        }
    }
}
