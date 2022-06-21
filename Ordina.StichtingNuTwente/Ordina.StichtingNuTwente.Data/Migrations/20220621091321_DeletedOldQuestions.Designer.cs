﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ordina.StichtingNuTwente.Data;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    [DbContext(typeof(NuTwenteContext))]
    [Migration("20220621091321_DeletedOldQuestions")]
    partial class DeletedOldQuestions
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CommentType")
                        .HasColumnType("int");

                    b.Property<int>("CommenterId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<int?>("GastgezinId")
                        .HasColumnType("int");

                    b.Property<int?>("ReactieId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CommenterId");

                    b.HasIndex("GastgezinId");

                    b.HasIndex("ReactieId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.ContactLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ContacterId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("GastgezinId")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContacterId");

                    b.HasIndex("GastgezinId");

                    b.ToTable("ContactLog");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Gastgezin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("AanmeldFormulierId")
                        .HasColumnType("int");

                    b.Property<int?>("BegeleiderId")
                        .HasColumnType("int");

                    b.Property<bool>("BekekenDoorBuddy")
                        .HasColumnType("bit");

                    b.Property<bool>("BekekenDoorIntaker")
                        .HasColumnType("bit");

                    b.Property<int?>("BuddyId")
                        .HasColumnType("int");

                    b.Property<int>("ContactId")
                        .HasColumnType("int");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<bool?>("HasVOG")
                        .HasColumnType("bit");

                    b.Property<int?>("IntakeFormulierId")
                        .HasColumnType("int");

                    b.Property<int?>("MaxAdults")
                        .HasColumnType("int");

                    b.Property<int?>("MaxChildren")
                        .HasColumnType("int");

                    b.Property<bool>("NoodOpvang")
                        .HasColumnType("bit");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("OnHold")
                        .HasColumnType("bit");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<string>("VrijwilligerOpmerkingen")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("fkPlaatsingsId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AanmeldFormulierId");

                    b.HasIndex("BegeleiderId");

                    b.HasIndex("BuddyId");

                    b.HasIndex("ContactId");

                    b.HasIndex("IntakeFormulierId");

                    b.HasIndex("fkPlaatsingsId");

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

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<int>("AgeGroup")
                        .HasColumnType("int");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

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

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.PlaatsingsInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AdresVanLocatie")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Allergieen")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BasisscholenAanwezig")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Beperkingen")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DaglichtSlaapkamer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EigenToegangsdeur")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Faciliteiten")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GezinsLeeftijden")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GezinsSamenstelling")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HuisdierenAanwezig")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HuisdierenMogelijk")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("KleineKinderen")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("KoelkastRuimte")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Opbergruimte")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OverigeOpmerkingen")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlaatsnaamVanLocatie")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostcodeVanLocatie")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Privacy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Roken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SlaapkamerRuimte")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SlaapplaatsOpmerking")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TelefoonnummerVanLocatie")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VeiligeOpbergruimte")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VluchtelingOphalen")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VolwassenenGrotereKinderen")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ZelfKoken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("fkReactieId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("fkReactieId");

                    b.ToTable("PlaatsingsInfos");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Reactie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DatumIngevuld")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

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

                    b.Property<bool>("InDropdown")
                        .HasColumnType("bit");

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

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Comment", b =>
                {
                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.UserDetails", "Commenter")
                        .WithMany()
                        .HasForeignKey("CommenterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Gastgezin", null)
                        .WithMany("Comments")
                        .HasForeignKey("GastgezinId");

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Reactie", null)
                        .WithMany("Comments")
                        .HasForeignKey("ReactieId");

                    b.Navigation("Commenter");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.ContactLog", b =>
                {
                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.UserDetails", "Contacter")
                        .WithMany()
                        .HasForeignKey("ContacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Gastgezin", null)
                        .WithMany("ContactLogs")
                        .HasForeignKey("GastgezinId");

                    b.Navigation("Contacter");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Gastgezin", b =>
                {
                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Reactie", "AanmeldFormulier")
                        .WithMany()
                        .HasForeignKey("AanmeldFormulierId");

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.UserDetails", "Begeleider")
                        .WithMany()
                        .HasForeignKey("BegeleiderId");

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.UserDetails", "Buddy")
                        .WithMany()
                        .HasForeignKey("BuddyId");

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Persoon", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Reactie", "IntakeFormulier")
                        .WithMany()
                        .HasForeignKey("IntakeFormulierId");

                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.PlaatsingsInfo", "PlaatsingsInfo")
                        .WithMany()
                        .HasForeignKey("fkPlaatsingsId");

                    b.Navigation("AanmeldFormulier");

                    b.Navigation("Begeleider");

                    b.Navigation("Buddy");

                    b.Navigation("Contact");

                    b.Navigation("IntakeFormulier");

                    b.Navigation("PlaatsingsInfo");
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

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.PlaatsingsInfo", b =>
                {
                    b.HasOne("Ordina.StichtingNuTwente.Models.Models.Reactie", "Reactie")
                        .WithMany()
                        .HasForeignKey("fkReactieId");

                    b.Navigation("Reactie");
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
                    b.Navigation("Comments");

                    b.Navigation("ContactLogs");

                    b.Navigation("Plaatsingen");

                    b.Navigation("Vluchtelingen");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.Reactie", b =>
                {
                    b.Navigation("Antwoorden");

                    b.Navigation("Comments");
                });

            modelBuilder.Entity("Ordina.StichtingNuTwente.Models.Models.UserDetails", b =>
                {
                    b.Navigation("Reacties");
                });
#pragma warning restore 612, 618
        }
    }
}
