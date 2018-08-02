using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Premy.Chatovatko.Client.Console.Models
{
    public partial class clientContext : DbContext
    {
        public clientContext()
        {
        }

        public clientContext(DbContextOptions<clientContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Settings> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlite("Data Source=C:/chatovatko/client.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Settings>(entity =>
            {
                entity.ToTable("settings");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.PrivateKey)
                    .IsRequired()
                    .HasColumnName("private_key")
                    .HasColumnType("VARBINARY(2000)");

                entity.Property(e => e.PublicId)
                    .HasColumnName("public_id")
                    .HasColumnType("INT");

                entity.Property(e => e.Publicy)
                    .IsRequired()
                    .HasColumnName("public_key")
                    .HasColumnType("VARBINARY(2000)");

                entity.Property(e => e.ServerAddress)
                    .IsRequired()
                    .HasColumnName("server_address")
                    .HasColumnType("VARCHAR(200)");

                entity.Property(e => e.ServerName)
                    .IsRequired()
                    .HasColumnName("server_name")
                    .HasColumnType("VARCHAR(200)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("VARCHAR(45)");
            });
        }
    }
}
