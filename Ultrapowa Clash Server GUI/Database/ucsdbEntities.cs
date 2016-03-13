using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace UCS.Database
{
    class ucsdbEntities : DbContext
    {

        public ucsdbEntities(string connectionString)
            : base("name=" + connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<clan> clan { get; set; }
        public virtual DbSet<player> player { get; set; }
    }
}
