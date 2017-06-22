using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DeltaWorkMonitoring.Models;

namespace DeltaWorkMonitoring.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DeltaWorkMonitoring.Models.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("DeltaWorkMonitoring.Models.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("CompanyId");

                    b.Property<DateTime?>("Created");

                    b.Property<DateTime?>("Modified");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("DeltaWorkMonitoring.Models.TaskHistoryItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Created");

                    b.Property<int>("Status");

                    b.Property<Guid>("TaskId");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskHistory");
                });

            modelBuilder.Entity("DeltaWorkMonitoring.Models.WorkTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comment");

                    b.Property<DateTime?>("Created");

                    b.Property<string>("Description");

                    b.Property<int>("Estimate");

                    b.Property<DateTime?>("Modified");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("ParentId");

                    b.Property<int>("Priority");

                    b.Property<Guid?>("ProjectId");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("DeltaWorkMonitoring.Models.Project", b =>
                {
                    b.HasOne("DeltaWorkMonitoring.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("DeltaWorkMonitoring.Models.TaskHistoryItem", b =>
                {
                    b.HasOne("DeltaWorkMonitoring.Models.WorkTask", "Task")
                        .WithMany("TaskHistoryItems")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DeltaWorkMonitoring.Models.WorkTask", b =>
                {
                    b.HasOne("DeltaWorkMonitoring.Models.WorkTask", "Parent")
                        .WithMany("Tasks")
                        .HasForeignKey("ParentId");

                    b.HasOne("DeltaWorkMonitoring.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");
                });
        }
    }
}
