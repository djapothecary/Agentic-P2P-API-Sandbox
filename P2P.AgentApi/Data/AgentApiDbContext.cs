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
        public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
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
                .HasMany(v => v.PurchaseOrders)
                .WithOne(po => po.Vendor)
                .HasForeignKey(po => po.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vendor>()
                .HasMany(v => v.Invoices)
                .WithOne(invoice => invoice.Vendor)
                .HasForeignKey(invoice => invoice.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseOrder>()
                .HasMany(po => po.LineItems)
                .WithOne(lineItem => lineItem.PurchaseOrder)
                .HasForeignKey(lineItem => lineItem.PurchaseOrderId)
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

            //  Table properties
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

            //  Table Constraints
            builder.Entity<POLineItem>()
                .ToTable(table => table.HasCheckConstraint(
                    "CK_POLineItem_QuantityOrdered_Positive",
                    "quantity_ordered > 0"
                ));

            builder.Entity<GoodsReceiptLine>()
                .ToTable(table => table.HasCheckConstraint(
                    "CK_GoodsReceiptLine_QuantityReceived_Positive",
                    "quantity_received > 0"
                ));

            builder.Entity<GLEntry>()
                .ToTable(table => table.HasCheckConstraint(
                    "CK_GLEntry_NotBothDebitAndCredit",
                    "(debit = 0 AND credit >= 0) OR " +
                    "(credit = 0 AND debit >= 0)"
                ));

            //  By default EF Core stores enums as integers, so force conversion
            builder.Entity<PurchaseOrder>()
                .Property(po => po.Status)
                .HasConversion<string>()
                .HasMaxLength(32);

            builder.Entity<Invoice>()
                .Property(invoice => invoice.Status)
                .HasConversion<string>()
                .HasMaxLength(32);

            builder.Entity<Vendor>()
                .Property(vendor => vendor.PaymentTerms)
                .HasConversion<string>()
                .HasMaxLength(32);
        }
    }
}