using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FoodSearch.Models;

public partial class FoodSearchContext : DbContext
{
    public FoodSearchContext()
    {
    }

    public FoodSearchContext(DbContextOptions<FoodSearchContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Factory> Factories { get; set; }

    public virtual DbSet<Fixation> Fixations { get; set; }

    public virtual DbSet<FixationItem> FixationItems { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Worker> Workers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("FoodSearchDatabase"));
    } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Factory>(entity =>
        {
            entity.ToTable("Factory");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Fixation>(entity =>
        {
            entity.ToTable("Fixation");

            entity.Property(e => e.DateOdFixation).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Fixations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Fixation_User");
        });

        modelBuilder.Entity<FixationItem>(entity =>
        {
            entity.ToTable("FixationItem");

            entity.HasOne(d => d.Fixation).WithMany(p => p.FixationItems)
                .HasForeignKey(d => d.FixationId)
                .HasConstraintName("FK_FixationItem_Fixation");

            entity.HasOne(d => d.Product).WithMany(p => p.FixationItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_FixationItem_Product");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.ToTable("Organization");

            entity.Property(e => e.EMail)
                .HasMaxLength(50)
                .HasColumnName("E-mail");
            entity.Property(e => e.Inn)
                .HasMaxLength(50)
                .HasColumnName("INN");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.ImageSource).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.Url)
                .HasMaxLength(100)
                .HasColumnName("URL");

            entity.HasOne(d => d.Factoty).WithMany(p => p.Products)
                .HasForeignKey(d => d.FactotyId)
                .HasConstraintName("FK_Product_Factory");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.ToTable("Subscription");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("money");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(200);

            entity.HasOne(d => d.Organization).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK_User_Organization");

            entity.HasOne(d => d.Subscription).WithMany(p => p.Users)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK_User_Subscription");
        });

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.ToTable("Worker");

            entity.HasOne(d => d.Role).WithMany(p => p.Workers)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Worker_Role");

            entity.HasOne(d => d.User).WithMany(p => p.Workers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Worker_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
