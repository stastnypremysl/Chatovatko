using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Premy.Chatovatko.Client.Libs.UserData;

namespace Premy.Chatovatko.Client.Libs.Database.Models
{
    public partial class SqlClientContext : DbContext
    {
        IClientDatabaseConfig config = null;
        public SqlClientContext(IClientDatabaseConfig config)
        {
            this.config = config;
        }

        public SqlClientContext(DbContextOptions<SqlClientContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Alarms> Alarms { get; set; }
        public virtual DbSet<BlobMessages> BlobMessages { get; set; }
        public virtual DbSet<Contacts> Contacts { get; set; }
        public virtual DbSet<ContactsDetail> ContactsDetail { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<MessagesThread> MessagesThread { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(String.Format("Data Source={0}", config.DatabaseAddress));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alarms>(entity =>
            {
                entity.ToTable("alarms");

                entity.HasIndex(e => e.BlobMessagesId)
                    .HasName("fk_alarms_blob_messages_id_idx")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.BlobMessagesId)
                    .HasColumnName("blob_messages_id")
                    .HasColumnType("INT");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnName("text")
                    .HasColumnType("MEDIUMTEXT");

                entity.Property(e => e.Time)
                    .IsRequired()
                    .HasColumnName("time")
                    .HasColumnType("DATETIME");

                entity.HasOne(d => d.BlobMessages)
                    .WithOne(p => p.Alarms)
                    .HasForeignKey<Alarms>(d => d.BlobMessagesId);
            });

            modelBuilder.Entity<BlobMessages>(entity =>
            {
                entity.ToTable("blob_messages");

                entity.HasIndex(e => e.PublicId)
                    .HasName("fk_blob_messages_public_id_idx")
                    .IsUnique();

                entity.HasIndex(e => e.RecepientId)
                    .HasName("fk_blob_messages_contacts1_idx");

                entity.HasIndex(e => e.SenderId)
                    .HasName("fk_blob_messages_contacts2_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Blob).HasColumnName("blob");

                entity.Property(e => e.DoDelete)
                    .IsRequired()
                    .HasColumnName("do_delete")
                    .HasColumnType("BIT(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Downloaded)
                    .IsRequired()
                    .HasColumnName("downloaded")
                    .HasColumnType("BIT(1)");

                entity.Property(e => e.PublicId)
                    .HasColumnName("public_id")
                    .HasColumnType("INT");

                entity.Property(e => e.RecepientId)
                    .HasColumnName("recepient_id")
                    .HasColumnType("INT");

                entity.Property(e => e.SenderId)
                    .HasColumnName("sender_id")
                    .HasColumnType("INT");

                entity.Property(e => e.Uploaded)
                    .IsRequired()
                    .HasColumnName("uploaded")
                    .HasColumnType("BIT(1)");

                entity.HasOne(d => d.Recepient)
                    .WithMany(p => p.BlobMessagesRecepient)
                    .HasForeignKey(d => d.RecepientId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.BlobMessagesSender)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Contacts>(entity =>
            {
                entity.HasKey(e => e.PublicId);

                entity.ToTable("contacts");

                entity.Property(e => e.PublicId)
                    .HasColumnName("public_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.PublicKey)
                    .IsRequired()
                    .HasColumnName("public_key")
                    .HasColumnType("VARBINARY(2000)");

                entity.Property(e => e.SymmetricKey)
                    .IsRequired()
                    .HasColumnName("symmetric_key")
                    .HasColumnType("VARBINARY(2000)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("VARCHAR(45)");
            });

            modelBuilder.Entity<ContactsDetail>(entity =>
            {
                entity.HasKey(e => e.ContactId);

                entity.ToTable("contacts_detail");

                entity.HasIndex(e => e.BlobMessagesId)
                    .HasName("fk_contacts_detail_blob_messages_id_idx")
                    .IsUnique();

                entity.HasIndex(e => e.ContactId)
                    .HasName("fk_contacts_detail_contacts_idx")
                    .IsUnique();

                entity.Property(e => e.ContactId)
                    .HasColumnName("contact_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.AlarmPermission)
                    .IsRequired()
                    .HasColumnName("alarm_permission")
                    .HasColumnType("BIT(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.BlobMessagesId)
                    .HasColumnName("blob_messages_id")
                    .HasColumnType("INT");

                entity.Property(e => e.ChangeContactsPermission)
                    .IsRequired()
                    .HasColumnName("change_contacts_permission")
                    .HasColumnType("BIT(1)");

                entity.Property(e => e.NickName)
                    .IsRequired()
                    .HasColumnName("nick_name")
                    .HasColumnType("VARCHAR(200)");

                entity.HasOne(d => d.BlobMessages)
                    .WithOne(p => p.ContactsDetail)
                    .HasForeignKey<ContactsDetail>(d => d.BlobMessagesId);

                entity.HasOne(d => d.Contact)
                    .WithOne(p => p.ContactsDetail)
                    .HasForeignKey<ContactsDetail>(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Messages>(entity =>
            {
                entity.ToTable("messages");

                entity.HasIndex(e => e.BlobMessagesId)
                    .HasName("fk_messages_blob_messages1_idx")
                    .IsUnique();

                entity.HasIndex(e => e.IdMessagesThread)
                    .HasName("fk_messages_messages_thread1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.BlobMessagesId)
                    .HasColumnName("blob_messages_id")
                    .HasColumnType("INT");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("date")
                    .HasColumnType("DATETIME");

                entity.Property(e => e.IdMessagesThread)
                    .HasColumnName("id_messages_thread")
                    .HasColumnType("INT");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnName("text")
                    .HasColumnType("MEDIUMTEXT");

                entity.HasOne(d => d.BlobMessages)
                    .WithOne(p => p.Messages)
                    .HasForeignKey<Messages>(d => d.BlobMessagesId);

                entity.HasOne(d => d.IdMessagesThreadNavigation)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.IdMessagesThread)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<MessagesThread>(entity =>
            {
                entity.ToTable("messages_thread");

                entity.HasIndex(e => e.BlobMessagesId)
                    .HasName("fk_mess_thr_blob_mess_id_idx")
                    .IsUnique();

                entity.HasIndex(e => new { e.RecepientId, e.PublicId })
                    .HasName("fk_mess_thr_public_id_idx")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Archived)
                    .IsRequired()
                    .HasColumnName("archived")
                    .HasColumnType("BIT(1)");

                entity.Property(e => e.BlobMessagesId)
                    .HasColumnName("blob_messages_id")
                    .HasColumnType("INT");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("VARCHAR(45)");

                entity.Property(e => e.Onlive)
                    .IsRequired()
                    .HasColumnName("onlive")
                    .HasColumnType("BIT(1)");

                entity.Property(e => e.PublicId)
                    .HasColumnName("public_id")
                    .HasColumnType("INT");

                entity.Property(e => e.RecepientId)
                    .HasColumnName("recepient_id")
                    .HasColumnType("INT");

                entity.HasOne(d => d.BlobMessages)
                    .WithOne(p => p.MessagesThread)
                    .HasForeignKey<MessagesThread>(d => d.BlobMessagesId);
            });

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

                entity.Property(e => e.PublicKey)
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

                entity.Property(e => e.ServerPublicKey)
                    .IsRequired()
                    .HasColumnName("server_public_key")
                    .HasColumnType("VARBINARY(2000)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("VARCHAR(45)");

                entity.Property(e => e.UserPublicId)
                    .HasColumnName("user_public_id")
                    .HasColumnType("INT");
            });
        }
    }
}
