using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace clothes_shop_api.Data.Entities
{
    public partial class ecommerce_decryptedContext : DbContext
    {
        public ecommerce_decryptedContext()
        {
        }

        public ecommerce_decryptedContext(DbContextOptions<ecommerce_decryptedContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<CartItem> CartItems { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Color> Colors { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductColor> ProductColors { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<Quantity> Quantities { get; set; } = null!;
        public virtual DbSet<Size> Sizes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:SQLServerCon");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("cart");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CART_USER");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("cart_item");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CartId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cart_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.QuantityId).HasColumnName("quantity_id");

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartId)
                    .HasConstraintName("FK_CART_ITEM_CART");

                entity.HasOne(d => d.QuantityNavigation)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.QuantityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CART_ITEM_QUANTITY");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.ToTable("color");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ColorCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("color_code");

                entity.Property(e => e.Name)
                    .HasMaxLength(15)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("order");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(3000)
                    .HasColumnName("address");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliveryMethod).HasColumnName("delivery_method");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(100)
                    .HasColumnName("fullname");

                entity.Property(e => e.Note)
                    .HasMaxLength(300)
                    .HasColumnName("note");

                entity.Property(e => e.PaymentId).HasColumnName("payment_id");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .HasColumnName("phone_number")
                    .IsFixedLength();

                entity.Property(e => e.ShippingFee).HasColumnName("shipping_fee");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_ORDER_PAYMENT");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ORDER_USER");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_item");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.QuantityId).HasColumnName("quantity_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_ORDER_ITEM_ORDER");

                entity.HasOne(d => d.QuantityNavigation)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.QuantityId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_ORDER_ITEM_QUANTITY");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Method)
                    .HasMaxLength(50)
                    .HasColumnName("method");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Provider)
                    .HasMaxLength(50)
                    .HasColumnName("provider");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("transaction_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_PAYMENT_USER");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .HasMaxLength(3000)
                    .HasColumnName("description");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.IsVisible)
                    .IsRequired()
                    .HasColumnName("is_visible")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Slug)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("slug");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_product_category");
            });

            modelBuilder.Entity<ProductColor>(entity =>
            {
                entity.ToTable("product_color");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ColorId).HasColumnName("color_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.ProductColors)
                    .HasForeignKey(d => d.ColorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_product_color_color");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductColors)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_product_color_product");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("product_image");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("image_url");

                entity.Property(e => e.IsMain).HasColumnName("is_main");

                entity.Property(e => e.IsSub).HasColumnName("is_sub");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.PublicId)
                    .HasMaxLength(100)
                    .HasColumnName("public_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_product_image_product");
            });

            modelBuilder.Entity<Quantity>(entity =>
            {
                entity.ToTable("quantity");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.ProductColorId).HasColumnName("product_color_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.SizeId).HasColumnName("size_id");

                entity.HasOne(d => d.ProductColor)
                    .WithMany(p => p.Quantities)
                    .HasForeignKey(d => d.ProductColorId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_quantity_product_color");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Quantities)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_quantity_product");

                entity.HasOne(d => d.Size)
                    .WithMany(p => p.Quantities)
                    .HasForeignKey(d => d.SizeId)
                    .HasConstraintName("FK_quantity_size");
            });

            modelBuilder.Entity<Size>(entity =>
            {
                entity.ToTable("size");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(15)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("date_of_birth");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Firstname)
                    .HasMaxLength(20)
                    .HasColumnName("firstname");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.IsAuthenticated).HasColumnName("is_authenticated");

                entity.Property(e => e.Lastname)
                    .HasMaxLength(20)
                    .HasColumnName("lastname");

                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");

                entity.Property(e => e.PasswordSalt).HasColumnName("password_salt");

                entity.Property(e => e.Role)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("role")
                    .HasDefaultValueSql("('Customer')");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
