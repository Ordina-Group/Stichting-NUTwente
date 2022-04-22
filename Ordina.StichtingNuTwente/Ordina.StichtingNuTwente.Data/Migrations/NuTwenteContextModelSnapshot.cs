﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ordina.StichtingNuTwente.Data;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    [DbContext(typeof(NuTwenteContext))]
    partial class NuTwenteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Adres", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Straat")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Woonplaats")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("fkReactieId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("fkReactieId");

                    b.ToTable("Adres");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Antwoord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("IdVanVraag")
                        .HasColumnType("int");

                    b.Property<int?>("ReactieId")
                        .HasColumnType("int");

                    b.Property<string>("Response")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ReactieId");

                    b.ToTable("Antwoorden");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Gastgezin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("BegeleiderId")
                        .HasColumnType("int");

                    b.Property<int>("ContactId")
                        .HasColumnType("int");

                    b.Property<int?>("IntakeFormulierId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BegeleiderId");

                    b.HasIndex("ContactId");

                    b.HasIndex("IntakeFormulierId");

                    b.ToTable("Gastgezinnen");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Persoon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Achternaam")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GeboorteDatum")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Geboorteplaats")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mobiel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Naam")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nationaliteit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Talen")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefoonnummer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefoonnummer2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("fkAdresId")
                        .HasColumnType("int");

                    b.Property<int?>("fkGastgezinId")
                        .HasColumnType("int");

                    b.Property<int?>("fkReactieId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("fkAdresId");

                    b.HasIndex("fkGastgezinId");

                    b.HasIndex("fkReactieId");

                    b.ToTable("Persoon");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Plaatsing", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AgeGroup")
                        .HasColumnType("int");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("PlacementType")
                        .HasColumnType("int");

                    b.Property<int>("VrijwilligerId")
                        .HasColumnType("int");

                    b.Property<int>("fkGastgezinId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VrijwilligerId");

                    b.HasIndex("fkGastgezinId");

                    b.ToTable("Plaatsingen");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Reactie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DatumIngevuld")
                        .HasColumnType("datetime2");

                    b.Property<int>("FormulierId")
                        .HasColumnType("int");

                    b.Property<int?>("UserDetailsId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserDetailsId");

                    b.ToTable("Reacties");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.UserDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AADId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Roles")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Adres", b =>
                {
                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Reactie", "Reactie")
                        .WithMany()
                        .HasForeignKey("fkReactieId");

                    b.Navigation("Reactie");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Antwoord", b =>
                {
                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Reactie", "Reactie")
                        .WithMany("Antwoorden")
                        .HasForeignKey("ReactieId");

                    b.Navigation("Reactie");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Gastgezin", b =>
                {
                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.UserDetails", "Begeleider")
                        .WithMany()
                        .HasForeignKey("BegeleiderId");

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Persoon", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Reactie", "IntakeFormulier")
                        .WithMany()
                        .HasForeignKey("IntakeFormulierId");

                    b.Navigation("Begeleider");

                    b.Navigation("Contact");

                    b.Navigation("IntakeFormulier");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Persoon", b =>
                {
                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Adres", "Adres")
                        .WithMany()
                        .HasForeignKey("fkAdresId");

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Gastgezin", "Gastgezin")
                        .WithMany("Vluchtelingen")
                        .HasForeignKey("fkGastgezinId");

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Reactie", "Reactie")
                        .WithMany()
                        .HasForeignKey("fkReactieId");

                    b.Navigation("Adres");

                    b.Navigation("Gastgezin");

                    b.Navigation("Reactie");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Plaatsing", b =>
                {
                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.UserDetails", "Vrijwilliger")
                        .WithMany()
                        .HasForeignKey("VrijwilligerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Gastgezin", "Gastgezin")
                        .WithMany("Plaatsingen")
                        .HasForeignKey("fkGastgezinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Gastgezin");

                    b.Navigation("Vrijwilliger");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Reactie", b =>
                {
                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.UserDetails", "UserDetails")
                        .WithMany("Reacties")
                        .HasForeignKey("UserDetailsId");

                    b.Navigation("UserDetails");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Gastgezin", b =>
                {
                    b.Navigation("Plaatsingen");

                    b.Navigation("Vluchtelingen");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Reactie", b =>
                {
                    b.Navigation("Antwoorden");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.UserDetails", b =>
                {
                    b.Navigation("Reacties");
                });
#pragma warning restore 612, 618
        }
    }
}
