using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BancoVirtualEstudantilWeb.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    public class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "3.0.0").HasAnnotation("Relational:MaxIdentifierLength", 128).HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BancoVirtualEstudantilWeb.Data.ApplicationUser", x =>
            {
                x.Property<string>("Id").HasColumnType("nvarchar(450)");
                x.Property<int>("AccessFailedCount").HasColumnType("int");
                x.Property<string>("ConcurrencyStamp").IsConcurrencyToken().HasColumnType("nvarchar(max)");
                x.Property<DateTime>("DataCriacao").HasColumnType("datetime2");
                x.Property<string>("Email").HasColumnType("nvarchar(256)").HasMaxLength(256);
                x.Property<bool>("EmailConfirmed").HasColumnType("bit");
                x.Property<bool>("LockoutEnabled").HasColumnType("bit");
                x.Property<DateTimeOffset?>("LockoutEnd").HasColumnType("datetimeoffset");
                x.Property<string>("NormalizedEmail").HasColumnType("nvarchar(256)").HasMaxLength(256);
                x.Property<string>("NormalizedUserName").HasColumnType("nvarchar(256)").HasMaxLength(256);
                x.Property<string>("PasswordHash").HasColumnType("nvarchar(max)");
                x.Property<string>("PhoneNumber").HasColumnType("nvarchar(max)");
                x.Property<bool>("PhoneNumberConfirmed").HasColumnType("bit");
                x.Property<string>("SecurityStamp").HasColumnType("nvarchar(max)");
                x.Property<bool>("TwoFactorEnabled").HasColumnType("bit");
                x.Property<string>("UserName").HasColumnType("nvarchar(256)").HasMaxLength(256);
                x.HasKey("Id");
                x.HasIndex("NormalizedEmail").HasName("EmailIndex");
                x.HasIndex("NormalizedUserName").IsUnique().HasName("UserNameIndex").HasFilter("[NormalizedUserName] IS NOT NULL");
                x.ToTable("AspNetUsers");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", x =>
            {
                x.Property<string>("Id").HasColumnType("nvarchar(450)");
                x.Property<string>("ConcurrencyStamp").IsConcurrencyToken().HasColumnType("nvarchar(max)");
                x.Property<string>("Name").HasColumnType("nvarchar(256)").HasMaxLength(256);
                x.Property<string>("NormalizedName").HasColumnType("nvarchar(256)").HasMaxLength(256);
                x.HasKey("Id");
                x.HasIndex("NormalizedName").IsUnique().HasName("RoleNameIndex").HasFilter("[NormalizedName] IS NOT NULL");
                x.ToTable("AspNetRoles");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", x =>
            {
                x.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int").HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
                x.Property<string>("ClaimType").HasColumnType("nvarchar(max)");
                x.Property<string>("ClaimValue").HasColumnType("nvarchar(max)");
                x.Property<string>("RoleId").IsRequired().HasColumnType("nvarchar(450)");
                x.HasKey("Id");
                x.HasIndex("RoleId");
                x.ToTable("AspNetRoleClaims");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", x =>
            {
                x.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int").HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
                x.Property<string>("ClaimType").HasColumnType("nvarchar(max)");
                x.Property<string>("ClaimValue").HasColumnType("nvarchar(max)");
                x.Property<string>("UserId").IsRequired().HasColumnType("nvarchar(450)");
                x.HasKey("Id");
                x.HasIndex("UserId");
                x.ToTable("AspNetUserClaims");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", x =>
            {
                x.Property<string>("LoginProvider").HasColumnType("nvarchar(450)");
                x.Property<string>("ProviderKey").HasColumnType("nvarchar(450)");
                x.Property<string>("ProviderDisplayName").HasColumnType("nvarchar(max)");
                x.Property<string>("UserId").IsRequired().HasColumnType("nvarchar(450)");
                x.HasKey("LoginProvider", "ProviderKey");
                x.HasIndex("UserId");
                x.ToTable("AspNetUserLogins");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>",
            x =>
            {
                x.Property<string>("UserId").HasColumnType("nvarchar(450)");
                x.Property<string>("RoleId").HasColumnType("nvarchar(450)");
                x.HasKey("UserId", "RoleId");
                x.HasIndex("RoleId");
                x.ToTable("AspNetUserRoles");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", x =>
            {
                x.Property<string>("UserId").HasColumnType("nvarchar(450)");
                x.Property<string>("LoginProvider").HasColumnType("nvarchar(450)");
                x.Property<string>("Name").HasColumnType("nvarchar(450)");
                x.Property<string>("Value").HasColumnType("nvarchar(max)");
                x.HasKey("UserId", "LoginProvider", "Name");
                x.ToTable("AspNetUserTokens");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", x =>
            {
                x.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null).WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade).IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", x =>
            {
                x.HasOne("BancoVirtualEstudantilWeb.Data.ApplicationUser", null).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", x =>
            {
                x.HasOne("BancoVirtualEstudantilWeb.Data.ApplicationUser", null).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", x =>
            {
                x.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null).WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade).IsRequired();

                x.HasOne("BancoVirtualEstudantilWeb.Data.ApplicationUser", null).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", x =>
           {
               x.HasOne("BancoVirtualEstudantilWeb.Data.ApplicationUser", null).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired();
           });
        }
    }
}