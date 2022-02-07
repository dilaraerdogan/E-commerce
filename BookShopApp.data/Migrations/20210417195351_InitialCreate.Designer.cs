﻿// <auto-generated />
using System;
using BookShopApp.data.Concrete.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BookShopApp.data.Migrations
{
    [DbContext(typeof(ShopContext))]
    [Migration("20210417195351_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("BookShopApp.entity.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("MainCategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.HasKey("CategoryId");

                    b.HasIndex("MainCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("BookShopApp.entity.MainCategory", b =>
                {
                    b.Property<int>("MainCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("MainCategoryId");

                    b.ToTable("MainCategories");
                });

            modelBuilder.Entity("BookShopApp.entity.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("BaskiYili")
                        .HasColumnType("integer");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Cevirmen")
                        .HasColumnType("text");

                    b.Property<string>("Dil")
                        .HasColumnType("text");

                    b.Property<string>("Icerik")
                        .HasColumnType("text");

                    b.Property<bool>("IsHome")
                        .HasColumnType("boolean");

                    b.Property<string>("KitapAdi")
                        .HasColumnType("text");

                    b.Property<bool>("Onay")
                        .HasColumnType("boolean");

                    b.Property<string>("ResimUrl")
                        .HasColumnType("text");

                    b.Property<int>("SayfaSayisi")
                        .HasColumnType("integer");

                    b.Property<double?>("Ucret")
                        .HasColumnType("double precision");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.Property<string>("YayınEvi")
                        .HasColumnType("text");

                    b.Property<string>("Yazar")
                        .HasColumnType("text");

                    b.HasKey("ProductId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("BookShopApp.entity.ProductCategory", b =>
                {
                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.HasKey("CategoryId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductCategory");
                });

            modelBuilder.Entity("BookShopApp.entity.Category", b =>
                {
                    b.HasOne("BookShopApp.entity.MainCategory", "MainCategory")
                        .WithMany("Categories")
                        .HasForeignKey("MainCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MainCategory");
                });

            modelBuilder.Entity("BookShopApp.entity.ProductCategory", b =>
                {
                    b.HasOne("BookShopApp.entity.Category", "Category")
                        .WithMany("ProductCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookShopApp.entity.Product", "Product")
                        .WithMany("ProductCategories")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("BookShopApp.entity.Category", b =>
                {
                    b.Navigation("ProductCategories");
                });

            modelBuilder.Entity("BookShopApp.entity.MainCategory", b =>
                {
                    b.Navigation("Categories");
                });

            modelBuilder.Entity("BookShopApp.entity.Product", b =>
                {
                    b.Navigation("ProductCategories");
                });
#pragma warning restore 612, 618
        }
    }
}
