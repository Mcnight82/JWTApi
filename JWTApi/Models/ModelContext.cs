using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JWTApi.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Iperiodo> Iperiodos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("DBJSON")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Iperiodo>(entity =>
        {
            entity.HasKey(e => e.Idperiodo).HasName("SYS_C008839");

            entity.ToTable("IPERIODO");

            entity.Property(e => e.Idperiodo)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("IDPERIODO");
            entity.Property(e => e.Datosi)
                .IsUnicode(false)
                .HasColumnName("DATOSI");
            entity.Property(e => e.Numperdiodo)
                .HasColumnType("NUMBER")
                .HasColumnName("NUMPERDIODO");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Iduser).HasName("SYS_C008837");

            entity.ToTable("USERS");

            entity.Property(e => e.Iduser)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("IDUSER");
            entity.Property(e => e.Datos)
                .IsUnicode(false)
                .HasColumnName("DATOS");
        });
        modelBuilder.HasSequence("IDPERIODO");
        modelBuilder.HasSequence("IDUSERS");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
