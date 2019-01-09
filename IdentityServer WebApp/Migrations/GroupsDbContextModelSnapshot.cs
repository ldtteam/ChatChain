﻿// <auto-generated />
using IdentityServer_WebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IdentityServerWebApp.Migrations
{
    [DbContext(typeof(GroupsDbContext))]
    partial class GroupsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065");

            modelBuilder.Entity("IdentityServer_WebApp.Data.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientGuid");

                    b.Property<int>("ClientId");

                    b.Property<string>("OwnerId");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("IdentityServer_WebApp.Data.ClientGroup", b =>
                {
                    b.Property<int>("ClientId");

                    b.Property<int>("GroupId");

                    b.HasKey("ClientId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("ClientGroup");
                });

            modelBuilder.Entity("IdentityServer_WebApp.Data.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GroupId");

                    b.Property<string>("GroupName");

                    b.Property<string>("OwnerId");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("IdentityServer_WebApp.Data.ClientGroup", b =>
                {
                    b.HasOne("IdentityServer_WebApp.Data.Client", "Client")
                        .WithMany("ClientGroups")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("IdentityServer_WebApp.Data.Group", "Group")
                        .WithMany("ClientGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}