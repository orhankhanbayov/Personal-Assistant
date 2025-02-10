﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersonalAssistant.Services;

#nullable disable

namespace PersonalAssistant.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PersonalAssistant.Models.AuditLog", b =>
                {
                    b.Property<int>("LogID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LogID"));

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.HasKey("LogID");

                    b.HasIndex("UserID");

                    b.ToTable("AuditLogs");
                });

            modelBuilder.Entity("PersonalAssistant.Models.CallConversationLog", b =>
                {
                    b.Property<int>("CallConversationLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CallConversationLogId"));

                    b.Property<int>("CallHistoryId")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<string>("Message1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message10")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message11")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message12")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message13")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message14")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message15")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message16")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message17")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message18")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message19")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message20")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message3")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message4")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message5")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message6")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message7")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message8")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message9")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.HasKey("CallConversationLogId");

                    b.HasIndex("CallHistoryId")
                        .IsUnique();

                    b.HasIndex("UserID");

                    b.ToTable("CallConversationLog");
                });

            modelBuilder.Entity("PersonalAssistant.Models.CallHistory", b =>
                {
                    b.Property<int>("CallHistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CallHistoryId"));

                    b.Property<string>("CallSid")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("CallHistoryId");

                    b.HasIndex("UserID");

                    b.ToTable("CallHistories");
                });

            modelBuilder.Entity("PersonalAssistant.Models.Category", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryID"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("CategoryID");

                    b.HasIndex("UserID");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("PersonalAssistant.Models.EventDetails", b =>
                {
                    b.Property<int>("EventID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventID"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("IsRecurring")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("RecurrencePattern")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("EventID");

                    b.HasIndex("UserID");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("PersonalAssistant.Models.Notification", b =>
                {
                    b.Property<int>("NotificationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotificationID"));

                    b.Property<int?>("EventID")
                        .HasColumnType("int");

                    b.Property<bool?>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TaskID")
                        .HasColumnType("int");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.HasKey("NotificationID");

                    b.HasIndex("EventID");

                    b.HasIndex("TaskID");

                    b.HasIndex("UserID");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("PersonalAssistant.Models.TaskDetails", b =>
                {
                    b.Property<int>("TaskID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TaskID"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<string>("Priority")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("TaskID");

                    b.HasIndex("UserID");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("PersonalAssistant.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("UserID");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PersonalAssistant.Models.AuditLog", b =>
                {
                    b.HasOne("PersonalAssistant.Models.User", "User")
                        .WithMany("AuditLogs")
                        .HasForeignKey("UserID");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalAssistant.Models.CallConversationLog", b =>
                {
                    b.HasOne("PersonalAssistant.Models.CallHistory", "CallHistory")
                        .WithOne("CallConversationLog")
                        .HasForeignKey("PersonalAssistant.Models.CallConversationLog", "CallHistoryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PersonalAssistant.Models.User", null)
                        .WithMany("CallConversationLogs")
                        .HasForeignKey("UserID");

                    b.Navigation("CallHistory");
                });

            modelBuilder.Entity("PersonalAssistant.Models.CallHistory", b =>
                {
                    b.HasOne("PersonalAssistant.Models.User", "User")
                        .WithMany("CallHistories")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalAssistant.Models.Category", b =>
                {
                    b.HasOne("PersonalAssistant.Models.User", "User")
                        .WithMany("Categories")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalAssistant.Models.EventDetails", b =>
                {
                    b.HasOne("PersonalAssistant.Models.User", "User")
                        .WithMany("Events")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalAssistant.Models.Notification", b =>
                {
                    b.HasOne("PersonalAssistant.Models.EventDetails", "EventDetail")
                        .WithMany("Notifications")
                        .HasForeignKey("EventID")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("PersonalAssistant.Models.TaskDetails", "TaskDetail")
                        .WithMany("Notifications")
                        .HasForeignKey("TaskID")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("PersonalAssistant.Models.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("EventDetail");

                    b.Navigation("TaskDetail");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalAssistant.Models.TaskDetails", b =>
                {
                    b.HasOne("PersonalAssistant.Models.User", "User")
                        .WithMany("Tasks")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalAssistant.Models.CallHistory", b =>
                {
                    b.Navigation("CallConversationLog")
                        .IsRequired();
                });

            modelBuilder.Entity("PersonalAssistant.Models.EventDetails", b =>
                {
                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("PersonalAssistant.Models.TaskDetails", b =>
                {
                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("PersonalAssistant.Models.User", b =>
                {
                    b.Navigation("AuditLogs");

                    b.Navigation("CallConversationLogs");

                    b.Navigation("CallHistories");

                    b.Navigation("Categories");

                    b.Navigation("Events");

                    b.Navigation("Notifications");

                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
