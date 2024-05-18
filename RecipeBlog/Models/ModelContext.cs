using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RecipeBlog.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<Recipecategory> Recipecategories { get; set; }

    public virtual DbSet<Reciperequest> Reciperequests { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("USER ID=C##RECBLOG;PASSWORD=Test321;DATA SOURCE=localhost:1521/xe");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##RECBLOG")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Paymentid).HasName("SYS_C008357");

            entity.ToTable("PAYMENTS");

            entity.Property(e => e.Paymentid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("PAYMENTID");
            entity.Property(e => e.Paymentamount)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PAYMENTAMOUNT");
            entity.Property(e => e.Paymentdate)
                .HasColumnType("DATE")
                .HasColumnName("PAYMENTDATE");
            entity.Property(e => e.Paymentstatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PAYMENTSTATUS");
            entity.Property(e => e.Recipeid)
                .HasColumnType("NUMBER")
                .HasColumnName("RECIPEID");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Recipeid)
                .HasConstraintName("SYS_C008359");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("SYS_C008358");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Recipeid).HasName("SYS_C008346");

            entity.ToTable("RECIPES");

            entity.Property(e => e.Recipeid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("RECIPEID");
            entity.Property(e => e.Categoryid)
                .HasColumnType("NUMBER")
                .HasColumnName("CATEGORYID");
            entity.Property(e => e.Chefid)
                .HasColumnType("NUMBER")
                .HasColumnName("CHEFID");
            entity.Property(e => e.Createdate)
                .HasDefaultValueSql("SYSDATE ")
                .HasColumnType("DATE")
                .HasColumnName("CREATEDATE");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("IMAGEPATH");
            entity.Property(e => e.Ingredients)
                .HasColumnType("CLOB")
                .HasColumnName("INGREDIENTS");
            entity.Property(e => e.Instructions)
                .HasColumnType("CLOB")
                .HasColumnName("INSTRUCTIONS");
            entity.Property(e => e.Isaccepted)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("ISACCEPTED");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PRICE");
            entity.Property(e => e.Recipename)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("RECIPENAME");

            entity.HasOne(d => d.Category).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.Categoryid)
                .HasConstraintName("SYS_C008348");

            entity.HasOne(d => d.Chef).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.Chefid)
                .HasConstraintName("SYS_C008347");
        });

        modelBuilder.Entity<Recipecategory>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("SYS_C008344");

            entity.ToTable("RECIPECATEGORIES");

            entity.Property(e => e.Categoryid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("CATEGORYID");
            entity.Property(e => e.Categoryname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CATEGORYNAME");
        });

        modelBuilder.Entity<Reciperequest>(entity =>
        {
            entity.HasKey(e => e.Requestid).HasName("SYS_C008353");

            entity.ToTable("RECIPEREQUESTS");

            entity.Property(e => e.Requestid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("REQUESTID");
            entity.Property(e => e.Recipeid)
                .HasColumnType("NUMBER")
                .HasColumnName("RECIPEID");
            entity.Property(e => e.Requestdate)
                .HasColumnType("DATE")
                .HasColumnName("REQUESTDATE");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Reciperequests)
                .HasForeignKey(d => d.Recipeid)
                .HasConstraintName("SYS_C008355");

            entity.HasOne(d => d.User).WithMany(p => p.Reciperequests)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("SYS_C008354");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Reportid).HasName("SYS_C008361");

            entity.ToTable("REPORTS");

            entity.Property(e => e.Reportid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("REPORTID");
            entity.Property(e => e.Generatedate)
                .HasColumnType("DATE")
                .HasColumnName("GENERATEDATE");
            entity.Property(e => e.Reportcontent)
                .HasColumnType("CLOB")
                .HasColumnName("REPORTCONTENT");
            entity.Property(e => e.Reporttype)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REPORTTYPE");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("SYS_C008334");

            entity.ToTable("ROLES");

            entity.Property(e => e.Roleid)
                .HasColumnType("NUMBER")
                .HasColumnName("ROLEID");
            entity.Property(e => e.Rolename)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ROLENAME");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.Testimonialid).HasName("SYS_C008350");

            entity.ToTable("TESTIMONIALS");

            entity.Property(e => e.Testimonialid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("TESTIMONIALID");
            entity.Property(e => e.Dateadded)
                .HasColumnType("DATE")
                .HasColumnName("DATEADDED");
            entity.Property(e => e.Testimonialtext)
                .HasColumnType("CLOB")
                .HasColumnName("TESTIMONIALTEXT");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.User).WithMany(p => p.Testimonials)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("SYS_C008351");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("SYS_C008339");

            entity.ToTable("USERS");

            entity.Property(e => e.Userid)
                .HasDefaultValueSql("\"C##RECBLOG\".\"USER_SEQ\".\"NEXTVAL\"")
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FIRSTNAME");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("IMAGEPATH");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LASTNAME");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Registrationdate)
                .HasDefaultValueSql("SYSDATE\n")
                .HasColumnType("DATE")
                .HasColumnName("REGISTRATIONDATE");
            entity.Property(e => e.Roleid)
                .HasColumnType("NUMBER")
                .HasColumnName("ROLEID");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USERNAME");
        });
        modelBuilder.HasSequence("USER_SEQ");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
