using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using assemblyAnalyzer.models;

namespace assemblyAnalyzer
{
    class ApplicationContext: DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
        }

        public DbSet<Dct_partfeature> PartFeatures { get; set; }
        public DbSet<Mtm_part_partfeature> Part_partfeatures { get; set; }
        public DbSet<Part> Parts { get; set; }
    }
}
