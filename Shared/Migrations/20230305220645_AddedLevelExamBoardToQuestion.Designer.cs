﻿// <auto-generated />
using System;
using CourseworkPastPaperApplication2.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CourseworkPastPaperApplication2.Shared.Migrations
{
    [DbContext(typeof(PapersDbContext))]
    [Migration("20230305220645_AddedLevelExamBoardToQuestion")]
    partial class AddedLevelExamBoardToQuestion
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AssignmentQuestion", b =>
                {
                    b.Property<Guid>("AssignmentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("QuestionsId")
                        .HasColumnType("uuid");

                    b.HasKey("AssignmentId", "QuestionsId");

                    b.HasIndex("QuestionsId");

                    b.ToTable("AssignmentQuestion", "public");
                });

            modelBuilder.Entity("ClassStudent", b =>
                {
                    b.Property<Guid>("CurrentClassesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("StudentsId")
                        .HasColumnType("uuid");

                    b.HasKey("CurrentClassesId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("ClassStudent", "public");
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.Assignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Due")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Set")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("StudentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("StudentId");

                    b.ToTable("Assignments", "public");
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.Class", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("TeacherId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("TeacherId");

                    b.ToTable("Classes", "public");
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.PaperResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AssignmentId")
                        .HasColumnType("uuid");

                    b.Property<int>("Score")
                        .HasColumnType("integer");

                    b.Property<Guid?>("StudentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentId");

                    b.HasIndex("Id");

                    b.HasIndex("StudentId");

                    b.ToTable("PaperResults", "public");
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.Question", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("ExamBoard")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ReadData")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("Questions", "public");
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.Student", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("Students", "public");
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.Teacher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("Teachers", "public");
                });

            modelBuilder.Entity("AssignmentQuestion", b =>
                {
                    b.HasOne("CourseworkPastPaperApplication2.Shared.Assignment", null)
                        .WithMany()
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CourseworkPastPaperApplication2.Shared.Question", null)
                        .WithMany()
                        .HasForeignKey("QuestionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ClassStudent", b =>
                {
                    b.HasOne("CourseworkPastPaperApplication2.Shared.Class", null)
                        .WithMany()
                        .HasForeignKey("CurrentClassesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CourseworkPastPaperApplication2.Shared.Student", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.Assignment", b =>
                {
                    b.HasOne("CourseworkPastPaperApplication2.Shared.Student", "Student")
                        .WithMany("Assignments")
                        .HasForeignKey("StudentId");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.Class", b =>
                {
                    b.HasOne("CourseworkPastPaperApplication2.Shared.Teacher", "TeacherNavigation")
                        .WithMany("Classes")
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TeacherNavigation");
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.PaperResult", b =>
                {
                    b.HasOne("CourseworkPastPaperApplication2.Shared.Assignment", "Assignment")
                        .WithMany("PaperResults")
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CourseworkPastPaperApplication2.Shared.Student", "Student")
                        .WithMany("PaperResults")
                        .HasForeignKey("StudentId");

                    b.Navigation("Assignment");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.Assignment", b =>
                {
                    b.Navigation("PaperResults");
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.Student", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("PaperResults");
                });

            modelBuilder.Entity("CourseworkPastPaperApplication2.Shared.Teacher", b =>
                {
                    b.Navigation("Classes");
                });
#pragma warning restore 612, 618
        }
    }
}
