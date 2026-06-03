using LojaDoImovel.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LojaDoImovel.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Enterprise> Enterprises { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- Identity ---
        builder.Entity<ApplicationUser>().ToTable("users");
        builder.Entity<IdentityRole<Guid>>().ToTable("roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");

        // Seed default roles
        builder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Name = "admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000001"
            },
            new IdentityRole<Guid>
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                Name = "userapproved",
                NormalizedName = "USERAPPROVED",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000002"
            },
            new IdentityRole<Guid>
            {
                Id = new Guid("00000000-0000-0000-0000-000000000003"),
                Name = "userunapproved",
                NormalizedName = "USERUNAPPROVED",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000003"
            }
        );

        // --- Enterprise ---
        builder.Entity<Enterprise>(e =>
        {
            e.ToTable("enterprises");
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Slug).IsUnique();
        });

        // --- Property ---
        builder.Entity<Property>(e =>
        {
            e.ToTable("properties");

            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasColumnType("text");

            e.Property(x => x.SalePrice).HasColumnType("numeric(18,2)");
            e.Property(x => x.RentalPrice).HasColumnType("numeric(18,2)");
            e.Property(x => x.CondoFee).HasColumnType("numeric(18,2)");
            e.Property(x => x.PropertyTax).HasColumnType("numeric(18,2)");

            e.Property(x => x.Street).HasMaxLength(200);
            e.Property(x => x.Number).HasMaxLength(10);
            e.Property(x => x.Complement).HasMaxLength(100);
            e.Property(x => x.Neighborhood).HasMaxLength(100);
            e.Property(x => x.City).HasMaxLength(100);
            e.Property(x => x.State).HasMaxLength(2);
            e.Property(x => x.ZipCode).HasMaxLength(9);

            e.Property(x => x.PrivateArea).HasColumnType("numeric(10,2)");
            e.Property(x => x.TotalArea).HasColumnType("numeric(10,2)");

            e.Property(x => x.PropertyType).HasMaxLength(50).IsRequired();
            e.Property(x => x.Purpose).HasMaxLength(50).IsRequired();

            e.HasOne(x => x.Enterprise)
             .WithMany(x => x.Properties)
             .HasForeignKey(x => x.EnterpriseId)
             .IsRequired();
        });

        // --- PropertyImage ---
        builder.Entity<PropertyImage>(e =>
        {
            e.ToTable("property_images");
        });
    }
}
