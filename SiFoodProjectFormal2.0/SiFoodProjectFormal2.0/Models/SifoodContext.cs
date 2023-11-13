using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SiFoodProjectFormal2._0.Models;

public partial class SifoodContext : DbContext
{
    public SifoodContext()
    {
    }

    public SifoodContext(DbContextOptions<SifoodContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Sifood;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ProductId }).HasName("PK__Cart__51BCD7978DC3971B");

            entity.ToTable("Cart");

            entity.HasIndex(e => e.ProductId, "IX_Cart_ProductID");

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UserID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_Products");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_Users");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2B1A31B7A5");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(6);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAA2641C5EE");

            entity.HasIndex(e => e.OrderId, "IX_Comments_OrderID");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.CommentTime).HasColumnType("datetime");
            entity.Property(e => e.Contents).HasMaxLength(255);
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OrderID");

            entity.HasOne(d => d.Order).WithMany(p => p.Comments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Orders");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.DriveId).HasName("PK__Drivers__9610CA387C354DCE");

            entity.Property(e => e.DriveId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DriveID");
            entity.Property(e => e.Email)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(10);
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PhotoPath).HasMaxLength(50);
            entity.Property(e => e.PlateNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__CE74FAF5BC2D09AB");

            entity.HasIndex(e => e.StoreId, "IX_Favorites_StoreID");

            entity.HasIndex(e => e.UserId, "IX_Favorites_UserID");

            entity.Property(e => e.FavoriteId).HasColumnName("FavoriteID");
            entity.Property(e => e.StoreId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("StoreID");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.Store).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorites_Stores");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorites_Users");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFD4236324");

            entity.HasIndex(e => e.DriverId, "IX_Orders_DriverID");

            entity.HasIndex(e => e.UserId, "IX_Orders_UserID");

            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OrderID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.DeliveryMethod).HasMaxLength(10);
            entity.Property(e => e.DriverId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DriverID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.ShippingFee).HasColumnType("money");
            entity.Property(e => e.StoreId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("StoreID");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.Driver).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Drivers");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__C3905BAF33426597");

            entity.HasIndex(e => e.OrderId, "IX_OrderDetails_OrderID");

            entity.HasIndex(e => e.StatusId, "IX_OrderDetails_StatusID");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.Status).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Status");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A58F797A561");

            entity.ToTable("Payment");

            entity.HasIndex(e => e.OrderId, "IX_Payment_OrderID");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OrderID");
            entity.Property(e => e.PaymentMethodＮame).HasMaxLength(10);
            entity.Property(e => e.PaymentStatusＮame).HasMaxLength(10);
            entity.Property(e => e.PaymentTime).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Orders");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6EDC62401DF");

            entity.HasIndex(e => e.CategoryId, "IX_Products_CategoryID");

            entity.HasIndex(e => e.StoreId, "IX_Products_StoreID");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(15);
            entity.Property(e => e.PhotoPath).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(15);
            entity.Property(e => e.RealeasedTime).HasColumnType("datetime");
            entity.Property(e => e.StoreId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("StoreID");
            entity.Property(e => e.UnitPrice).HasColumnType("money");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Category");

            entity.HasOne(d => d.Store).WithMany(p => p.Products)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Stores");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName).HasMaxLength(8);
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.Property(e => e.StoreId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("StoreID");
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.City).HasMaxLength(10);
            entity.Property(e => e.ContactName).HasMaxLength(10);
            entity.Property(e => e.Description).HasMaxLength(20);
            entity.Property(e => e.Email)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.EnrollDate).HasColumnType("date");
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 8)");
            entity.Property(e => e.LogoPath).HasMaxLength(50);
            entity.Property(e => e.Longitude).HasColumnType("decimal(11, 8)");
            entity.Property(e => e.OpeningDay).HasMaxLength(15);
            entity.Property(e => e.OpeningTime).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(64);
            entity.Property(e => e.PasswordSalt).HasMaxLength(64);
            entity.Property(e => e.Phone).HasMaxLength(10);
            entity.Property(e => e.PhotosPath).HasMaxLength(50);
            entity.Property(e => e.Region).HasMaxLength(10);
            entity.Property(e => e.StoreName).HasMaxLength(15);
            entity.Property(e => e.TaxId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TaxID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC7F13BEFB");

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UserID");
            entity.Property(e => e.TotalOrderAmount)
                .HasDefaultValueSql("((0))")
                .HasColumnType("money");
            entity.Property(e => e.UserBirthDate).HasColumnType("date");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UserEnrollDate).HasColumnType("date");
            entity.Property(e => e.UserLatestLogInDate).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(15);
            entity.Property(e => e.UserPasswordHash).HasMaxLength(64);
            entity.Property(e => e.UserPasswordSalt).HasMaxLength(64);
            entity.Property(e => e.UserPhone)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.UserAddressId).HasName("PK__UserAddr__5961BB97573B1078");

            entity.ToTable("UserAddress");

            entity.HasIndex(e => e.UserId, "IX_UserAddress_UserID");

            entity.Property(e => e.UserAddressId).HasColumnName("UserAddressID");
            entity.Property(e => e.UserCity).HasMaxLength(50);
            entity.Property(e => e.UserDetailAddress).HasMaxLength(50);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UserID");
            entity.Property(e => e.UserLatitude).HasColumnType("decimal(10, 8)");
            entity.Property(e => e.UserLongitude).HasColumnType("decimal(11, 8)");
            entity.Property(e => e.UserRegion).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAddresses_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
