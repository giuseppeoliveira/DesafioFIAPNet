using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SchoolAPI.Domain.Entities;

namespace SchoolAPI.Infrastructure;

public partial class SchoolAPIContext : DbContext
{
    public SchoolAPIContext(DbContextOptions<SchoolAPIContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aluno> Aluno { get; set; }

    public virtual DbSet<Matricula> Matricula { get; set; }

    public virtual DbSet<Turma> Turma { get; set; }

    public virtual DbSet<Usuarioadmin> Usuarioadmin { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aluno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aluno_pkey");

            entity.ToTable("aluno");

            entity.HasIndex(e => e.Cpf, "aluno_cpf_key").IsUnique();

            entity.HasIndex(e => e.Email, "aluno_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("cpf");
            entity.Property(e => e.Datacriado)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("datacriado");
            entity.Property(e => e.Datanascimento).HasColumnName("datanascimento");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
            entity.Property(e => e.Salt).HasColumnName("salt");
            entity.Property(e => e.Senha).HasColumnName("senha");
        });

        modelBuilder.Entity<Matricula>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("matricula_pkey");

            entity.ToTable("matricula");

            entity.HasIndex(e => new { e.Alunoid, e.Turmaid }, "matricula_alunoid_turmaid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Alunoid).HasColumnName("alunoid");
            entity.Property(e => e.Datamatricula)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("datamatricula");
            entity.Property(e => e.Turmaid).HasColumnName("turmaid");

            entity.HasOne(d => d.Aluno).WithMany(p => p.Matricula)
                .HasForeignKey(d => d.Alunoid)
                .HasConstraintName("matricula_alunoid_fkey");

            entity.HasOne(d => d.Turma).WithMany(p => p.Matricula)
                .HasForeignKey(d => d.Turmaid)
                .HasConstraintName("matricula_turmaid_fkey");
        });

        modelBuilder.Entity<Turma>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("turma_pkey");

            entity.ToTable("turma");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datacriado)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("datacriado");
            entity.Property(e => e.Descricao)
                .HasMaxLength(250)
                .HasColumnName("descricao");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Usuarioadmin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuarioadmin_pkey");

            entity.ToTable("usuarioadmin");

            entity.HasIndex(e => e.Email, "usuarioadmin_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datacriado)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("datacriado");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Salt).HasColumnName("salt");
            entity.Property(e => e.Senha).HasColumnName("senha");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
