using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POS.Data.Entities;
using POS.Domain.Entities;
using POS.Infrastructure.Data.Seed;
using POS.Infrastructure.Identity;

namespace POS.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Expense> Expenses => Set<Expense>();

    public DbSet<StoreSettings> StoreSettings => Set<StoreSettings>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>().HasData(
            IdentityRoleSeed.Roles.Select(r => new IdentityRole
            {
                Id = r.Id,
                Name = r.Name,
                NormalizedName = r.Name.ToUpperInvariant(),
                ConcurrencyStamp = r.Id
            }));

        builder.Entity<Branch>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Code).IsUnique();
        });

        builder.Entity<Branch>().HasData(new Branch
        {
            Id = BranchSeed.DefaultBranchId,
            Name = "Main Branch",
            Code = "MAIN",
            Address = null,
            IsActive = true,
            CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
        });

        builder.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.HasOne(x => x.Branch)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.BranchId, x.Name }).IsUnique();
        });

        builder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Sku).HasMaxLength(80);
            e.Property(x => x.Barcode).HasMaxLength(80);
            e.Property(x => x.CostPrice).HasPrecision(18, 2);
            e.Property(x => x.SellingPrice).HasPrecision(18, 2);
            e.Property(x => x.CurrentStock).HasPrecision(18, 3);
            e.Property(x => x.ReorderLevel).HasPrecision(18, 3);

            e.HasOne(x => x.Branch)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.BranchId, x.Sku }).IsUnique().HasFilter("[Sku] IS NOT NULL");
            e.HasIndex(x => new { x.BranchId, x.Barcode }).IsUnique().HasFilter("[Barcode] IS NOT NULL");
        });

        builder.Entity<Customer>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.Phone).HasMaxLength(40);
            e.HasOne(x => x.Branch)
                .WithMany(x => x.Customers)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Supplier>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.Phone).HasMaxLength(40);
            e.HasOne(x => x.Branch)
                .WithMany(x => x.Suppliers)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Sale>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ReceiptNumber).HasMaxLength(60).IsRequired();
            e.Property(x => x.Subtotal).HasPrecision(18, 2);
            e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            e.Property(x => x.TaxAmount).HasPrecision(18, 2);
            e.Property(x => x.Total).HasPrecision(18, 2);
            e.Property(x => x.PaidAmount).HasPrecision(18, 2);
            e.Property(x => x.ChangeAmount).HasPrecision(18, 2);

            e.HasOne(x => x.Branch)
                .WithMany(x => x.Sales)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Customer)
                .WithMany(x => x.Sales)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(x => new { x.BranchId, x.ReceiptNumber }).IsUnique();
        });

        builder.Entity<SaleItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Quantity).HasPrecision(18, 3);
            e.Property(x => x.UnitPrice).HasPrecision(18, 2);
            e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            e.Property(x => x.TaxAmount).HasPrecision(18, 2);
            e.Property(x => x.LineTotal).HasPrecision(18, 2);

            e.HasOne(x => x.Branch)
                .WithMany()
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Sale)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Product)
                .WithMany(x => x.SaleItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Purchase>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ReferenceNumber).HasMaxLength(60).IsRequired();
            e.Property(x => x.Subtotal).HasPrecision(18, 2);
            e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            e.Property(x => x.TaxAmount).HasPrecision(18, 2);
            e.Property(x => x.Total).HasPrecision(18, 2);

            e.HasOne(x => x.Branch)
                .WithMany(x => x.Purchases)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Supplier)
                .WithMany(x => x.Purchases)
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.BranchId, x.ReferenceNumber }).IsUnique();
        });

        builder.Entity<PurchaseItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Quantity).HasPrecision(18, 3);
            e.Property(x => x.UnitCost).HasPrecision(18, 2);
            e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            e.Property(x => x.TaxAmount).HasPrecision(18, 2);
            e.Property(x => x.LineTotal).HasPrecision(18, 2);

            e.HasOne(x => x.Branch)
                .WithMany()
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Purchase)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Product)
                .WithMany(x => x.PurchaseItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<StockMovement>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Quantity).HasPrecision(18, 3);
            e.Property(x => x.Reference).HasMaxLength(100);

            e.HasOne(x => x.Branch)
                .WithMany(x => x.StockMovements)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Product)
                .WithMany(x => x.StockMovements)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Sale)
                .WithMany(x => x.StockMovements)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasOne(x => x.Purchase)
                .WithMany(x => x.StockMovements)
                .HasForeignKey(x => x.PurchaseId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Expense>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Category).HasMaxLength(100);
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.HasOne(x => x.Branch)
                .WithMany(x => x.Expenses)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<StoreSettings>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.BranchId).IsUnique();
            e.Property(x => x.StoreName).HasMaxLength(200).IsRequired();
            e.Property(x => x.StoreAddress).HasMaxLength(500);
            e.Property(x => x.StorePhone).HasMaxLength(40);
            e.Property(x => x.StoreEmail).HasMaxLength(256);
            e.Property(x => x.TaxRatePercent).HasPrecision(9, 4);
            e.Property(x => x.ReceiptFooter).HasMaxLength(1000);
            e.Property(x => x.CurrencyCode).HasMaxLength(8).IsRequired();
            e.Property(x => x.CurrencySymbol).HasMaxLength(8);

            e.HasOne(x => x.Branch)
                .WithOne(x => x.StoreSettings)
                .HasForeignKey<StoreSettings>(x => x.BranchId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<StoreSettings>().HasData(new StoreSettings
        {
            Id = StoreSettingsSeed.DefaultSettingsId,
            BranchId = BranchSeed.DefaultBranchId,
            StoreName = "POS Store",
            StoreAddress = null,
            StorePhone = null,
            StoreEmail = null,
            TaxRatePercent = 0m,
            ReceiptFooter = "Thank you for your purchase.",
            CurrencyCode = "USD",
            CurrencySymbol = "$",
            CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        TouchAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        TouchAuditFields();
        return base.SaveChanges();
    }

    private void TouchAuditFields()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}

