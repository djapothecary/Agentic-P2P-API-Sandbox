using System.Formats.Tar;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using P2P.AgentApi.Entities;

namespace P2P.AgentApi.Data
{
    public class AgentApiDbContext : DbContext
    {
        public DbSet<GLEntry> GLEntries => Set<GLEntry>();
        public DbSet<GoodsReceipt> GoodReceipts => Set<GoodsReceipt>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<POLineItem> POLineItems => Set<POLineItem>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<Vendor> Vendors => Set<Vendor>();

        public AgentApiDbContext(
            DbContextOptions<AgentApiDbContext> options
        ) : base(options)
        { }

        protected override void OnModelCreating(
            ModelBuilder builder
        )
        {
            base.OnModelCreating(builder);

            builder.Entity<Vendor>()
                .HasMany(v => v.Purchaseorders)
                .WithOne(po => po.Vendor)
                .HasForeignKey(po => po.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vendor>()
                .HasMany(v => v.Invoices)
                .WithOne(invoice => invoice.Vendor)
                .HasForeignKey(invoice => invoice.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseOrder>()
                .HasMany(po => po.LineItemEntities)
                .WithOne(lineItem => lineItem.PurchaseOrder)
                .HasForeignKey(lineItem => lineItem.PurchaseOrder)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PurchaseOrder>()
                .HasMany(po => po.GoodsReceipts)
                .WithOne(receipt => receipt.PurchaseOrder)
                .HasForeignKey(receipt => receipt.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseOrder>()
                .HasMany(po => po.Invoices)
                .WithOne(invoice => invoice.PurchaseOrder)
                .HasForeignKey(invoice => invoice.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Invoice>()
                .HasMany(invoice => invoice.GLEntries)
                .WithOne(glEntry => glEntry.Invoice)
                .HasForeignKey(glEntry => glEntry.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Invoice>()
                .Property(invoice => invoice.Amount)
                .HasPrecision(18, 2);

            builder.Entity<POLineItem>()
                .Property(lineItem => lineItem.UnitCost)
                .HasPrecision(18, 2);

            builder.Entity<GoodsReceipt>()
                .Property(receipt => receipt.UnitCost)
                .HasPrecision(18, 2);

            builder.Entity<GLEntry>()
                .Property(glEntry => glEntry.Debit)
                .HasPrecision(18, 2);

            builder.Entity<GLEntry>()
                .Property(glEntry => glEntry.Credit)
                .HasPrecision(18, 2);
        }
    }
}