﻿// <auto-generated />
using Domain.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Domain.Migrations
{
    [DbContext(typeof(TwitchBotDbContext))]
    [Migration("20230517115235_AddSocialMedias")]
    partial class AddSocialMedias
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.4");

            modelBuilder.Entity("Contracts.Database.Command", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("CommandName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("command_name");

                    b.Property<string>("CommandOutput")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("command_output");

                    b.HasKey("Id");

                    b.ToTable("tbl_commands");
                });

            modelBuilder.Entity("Contracts.Database.SocialMedia", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("link");

                    b.Property<string>("SocialNetworkName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("social_network");

                    b.HasKey("Id");

                    b.ToTable("tbl_social_media");
                });
#pragma warning restore 612, 618
        }
    }
}