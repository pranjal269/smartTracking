using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SmartTracking.Api.Data;

#nullable disable

namespace SmartTracking.Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250530180657_FixSenderIdType")]
    partial class FixSenderIdType
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Parcel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RecipientAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RecipientName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SenderId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SenderId1")
                        .HasColumnType("text");

                    b.Property<string>("SpecialInstructions")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("TrackingNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("Id");

                    b.HasIndex("SenderId");

                    b.HasIndex("SenderId1");

                    b.ToTable("Parcels");
                });

            modelBuilder.Entity("SmartTracking.Api.Models.HandlerAlert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsResolved")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<int>("ParcelId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("ResolvedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ResolvedBy")
                        .HasColumnType("text");

                    b.Property<string>("TrackingNumber")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("HandlerAlerts");
                });

            modelBuilder.Entity("SmartTracking.Api.Models.HandlerLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<DateTime>("AssignedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("HandlerId")
                        .HasColumnType("text");

                    b.Property<string>("HandlerId1")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("LocationName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("HandlerId");

                    b.HasIndex("HandlerId1");

                    b.ToTable("HandlerLocations");
                });

            modelBuilder.Entity("SmartTracking.Api.Models.HandoverLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("HandlerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("HandoverTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<string>("ParcelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("HandlerId");

                    b.HasIndex("ParcelId");

                    b.ToTable("HandoverLogs");
                });

            modelBuilder.Entity("SmartTracking.Api.Models.UserEntry", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserEntries");
                });

            modelBuilder.Entity("Parcel", b =>
                {
                    b.HasOne("SmartTracking.Api.Models.UserEntry", null)
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.HasOne("SmartTracking.Api.Models.UserEntry", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId1");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("SmartTracking.Api.Models.HandlerLocation", b =>
                {
                    b.HasOne("SmartTracking.Api.Models.UserEntry", null)
                        .WithMany()
                        .HasForeignKey("HandlerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SmartTracking.Api.Models.UserEntry", "Handler")
                        .WithMany()
                        .HasForeignKey("HandlerId1");

                    b.Navigation("Handler");
                });

            modelBuilder.Entity("SmartTracking.Api.Models.HandoverLog", b =>
                {
                    b.HasOne("SmartTracking.Api.Models.UserEntry", "Handler")
                        .WithMany()
                        .HasForeignKey("HandlerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Parcel", "Parcel")
                        .WithMany()
                        .HasForeignKey("ParcelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Handler");

                    b.Navigation("Parcel");
                });
#pragma warning restore 612, 618
        }
    }
}
