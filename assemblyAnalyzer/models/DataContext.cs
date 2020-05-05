using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using assemblyAnalyzer.models;

namespace assemblyAnalyzer
{
    class DataContext: DbContext
    {
        public DataContext() : base("DefaultConnection")
        {
            //Database.SetInitializer<DataContext>(new DropCreateDatabaseIfModelChanges<DataContext>());
        }
        public DbSet<Part> Parts { get; set; }
        public DbSet<PartFeature> PartFeatures { get; set; }
        public DbSet<Part_PartFeature> Part_partfeatures { get; set; }
    }
}
