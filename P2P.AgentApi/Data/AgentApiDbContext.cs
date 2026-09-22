using System.Formats.Tar;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using P2P.AgentApi.Entities;

namespace P2P.AgentApi.Data
{
    public class AgentApiDbContext : DbContext
    {
        public DbSet<GLEntry> GLEntries { get; set; }
        public DbSet<GoodsReceipt> GoodReceipts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<POLineItem> POLineItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Vendor> Vendors { get; set; }

        public AgentApiDbContext(
            DbContextOptions<AgentApiDbContext> options
        ) : base(options)
        { }

        protected override void OnModelCreating(
            ModelBuilder builder
        )
        {
            base.OnModelCreating(builder);


        }
    }
}