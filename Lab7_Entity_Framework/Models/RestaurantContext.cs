using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7_Entity_Framework.Models
{
    public class RestaurantContext : DbContext
    {
        public DbSet<Food> Foods { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleAccount> RoleAccounts { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder) // Change method name and signature
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Food>()
                .HasRequired(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.FoodCategoryId)
                .WillCascadeOnDelete(true);

            // Account: use AccountName as primary key (string)
            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountName);

            // RoleAccount: composite primary key (RoleID, AccountName)
            modelBuilder.Entity<RoleAccount>()
                .HasKey(ra => new { ra.RoleID, ra.AccountName });

            // RoleAccount -> Role (required)
            modelBuilder.Entity<RoleAccount>()
                .HasRequired(ra => ra.Role)
                .WithMany(r => r.RoleAccounts)
                .HasForeignKey(ra => ra.RoleID)
                .WillCascadeOnDelete(false);

            // RoleAccount -> Account (optional because RoleAccount may reference an AccountName that doesn't exist)
            modelBuilder.Entity<RoleAccount>()
                .HasRequired(ra => ra.Account)
                .WithMany()
                .HasForeignKey(ra => ra.AccountName)
                .WillCascadeOnDelete(false);
        }
    }
}
