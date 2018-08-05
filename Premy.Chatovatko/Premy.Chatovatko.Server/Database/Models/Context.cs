﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Premy.Chatovatko.Server.Database.Models
{
    public partial class Context : DbContext
    {
        private ServerConfig config = null;
        public Context(ServerConfig config)
        {
            this.config = config;
        }

        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public virtual DbSet<BlobMessages> BlobMessages { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<PublicCertificates> PublicCertificates { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlobMessages>(entity =>
            {
                entity.ToTable("blob_messages");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.SenderId)
                    .HasName("sender_id");

                entity.HasIndex(e => new { e.RecepientId, e.Id })
                    .HasName("recepiant_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content")
                    .HasColumnType("blob");

                entity.Property(e => e.RecepientId)
                    .HasColumnName("recepient_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SenderId)
                    .HasColumnName("sender_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Recepient)
                    .WithMany(p => p.BlobMessagesRecepient)
                    .HasForeignKey(d => d.RecepientId)
                    .HasConstraintName("recepient_id_foreign_key");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.BlobMessagesSender)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("sender_id_foreign_key");
            });

            modelBuilder.Entity<Logs>(entity =>
            {
                entity.ToTable("logs");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasColumnName("class")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Error)
                    .HasColumnName("error")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message")
                    .HasColumnType("mediumtext");

                entity.Property(e => e.Source)
                    .HasColumnName("source")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.TimeOfCreation)
                    .HasColumnName("time_of_creation")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<PublicCertificates>(entity =>
            {
                entity.ToTable("public_certificates");

                entity.HasIndex(e => e.SenderId)
                    .HasName("fk_public_certificates_users2_idx");

                entity.HasIndex(e => new { e.RecepientId, e.SenderId })
                    .HasName("user_id_keys")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.EncryptedAesKey)
                    .IsRequired()
                    .HasColumnName("encrypted_aes_key")
                    .HasColumnType("varchar(2000)");

                entity.Property(e => e.RecepientId)
                    .HasColumnName("recepient_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SenderId)
                    .HasColumnName("sender_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Trusted)
                    .HasColumnName("trusted")
                    .HasColumnType("bit(1)");

                entity.HasOne(d => d.Recepient)
                    .WithMany(p => p.PublicCertificatesRecepient)
                    .HasForeignKey(d => d.RecepientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_public_keys_users1");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.PublicCertificatesSender)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_public_certificates_users2");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.PublicCertificateSha1)
                    .HasName("public_certificate_sha1_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserName)
                    .HasName("user_name_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PublicCertificate)
                    .IsRequired()
                    .HasColumnName("public_certificate")
                    .HasColumnType("mediumtext");

                entity.Property(e => e.PublicCertificateSha1)
                    .IsRequired()
                    .HasColumnName("public_certificate_sha1")
                    .HasMaxLength(20);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("varchar(45)");
            });
        }
    }
}
