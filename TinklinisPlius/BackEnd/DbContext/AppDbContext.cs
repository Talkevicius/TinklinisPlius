using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TinklinisPlius.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bet> Bets { get; set; }

    public virtual DbSet<Inspector> Inspectors { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Participate> Participates { get; set; }

    public virtual DbSet<Payouttransaction> Payouttransactions { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<Teammanager> Teammanagers { get; set; }

    public virtual DbSet<Tournament> Tournaments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wager> Wagers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=IrankiaiDB;Username=postgres;Password=ktu123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bet>(entity =>
        {
            entity.HasKey(e => e.IdBet).HasName("bet_pkey");

            entity.ToTable("bet");

            entity.Property(e => e.IdBet).HasColumnName("id_bet");
            entity.Property(e => e.Betdate).HasColumnName("betdate");
            entity.Property(e => e.Bettype).HasColumnName("bettype");
            entity.Property(e => e.Chanceofwinning).HasColumnName("chanceofwinning");
            entity.Property(e => e.FkUseridUser).HasColumnName("fk_userid_user");
            entity.Property(e => e.FkWageridWager).HasColumnName("fk_wagerid_wager");
            entity.Property(e => e.Moneybetted).HasColumnName("moneybetted");
            entity.Property(e => e.Payout).HasColumnName("payout");
            entity.Property(e => e.Result).HasColumnName("result");

            entity.HasOne(d => d.FkUseridUserNavigation).WithMany(p => p.Bets)
                .HasForeignKey(d => d.FkUseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bet_user");

            entity.HasOne(d => d.FkWageridWagerNavigation).WithMany(p => p.Bets)
                .HasForeignKey(d => d.FkWageridWager)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bet_wager");
        });

        modelBuilder.Entity<Inspector>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("inspector_pkey");

            entity.ToTable("inspector");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("id_user");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");

            entity.HasOne(d => d.IdUserNavigation).WithOne(p => p.Inspector)
                .HasForeignKey<Inspector>(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_inspector_user");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => new { e.IdMatch, e.FkTournamentidTournament }).HasName("match_pkey");

            entity.ToTable("match");

            entity.HasIndex(e => e.FkTeamidTeam, "uq_match_team").IsUnique();

            entity.Property(e => e.IdMatch)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_match");
            entity.Property(e => e.FkTournamentidTournament).HasColumnName("fk_tournamentid_tournament");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.FkInspectoridUser).HasColumnName("fk_inspectorid_user");
            entity.Property(e => e.FkMatchfkTournamentidTournament).HasColumnName("fk_matchfk_tournamentid_tournament");
            entity.Property(e => e.FkMatchidMatch).HasColumnName("fk_matchid_match");
            entity.Property(e => e.FkTeamidTeam).HasColumnName("fk_teamid_team").IsRequired(false);
            entity.Property(e => e.Hashappened).HasColumnName("hashappened");
            entity.Property(e => e.Placeintournament).HasColumnName("placeintournament");
            entity.Property(e => e.Team1score).HasColumnName("team1score");
            entity.Property(e => e.Team2score).HasColumnName("team2score");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.FkInspectoridUserNavigation).WithMany(p => p.Matches)
                .HasForeignKey(d => d.FkInspectoridUser)
                .HasConstraintName("fk_match_inspector");

            entity.HasOne(d => d.FkTeamidTeamNavigation).WithOne(p => p.Match)
                .HasForeignKey<Match>(d => d.FkTeamidTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_match_team");

            entity.HasOne(d => d.FkTournamentidTournamentNavigation).WithMany(p => p.Matches)
                .HasForeignKey(d => d.FkTournamentidTournament)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_match_tournament");
        });

        modelBuilder.Entity<Participate>(entity =>
        {
            entity.HasKey(e => new { e.FkMatchidMatch, e.FkMatchfkTournamentidTournament, e.FkTeamidTeam }).HasName("participates_pkey");

            entity.ToTable("participates");

            entity.Property(e => e.FkMatchidMatch).HasColumnName("fk_matchid_match");
            entity.Property(e => e.FkMatchfkTournamentidTournament).HasColumnName("fk_matchfk_tournamentid_tournament");
            entity.Property(e => e.FkTeamidTeam).HasColumnName("fk_teamid_team");

            entity.HasOne(d => d.Match).WithMany(p => p.Participates)
                .HasForeignKey(d => new { d.FkMatchidMatch, d.FkMatchfkTournamentidTournament })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_participates_match");
            // Add relationship with Team
            entity.HasOne(d => d.Team)
                .WithMany()  // If Team does NOT have ICollection<Participate> navigation, otherwise replace with .WithMany(t => t.Participates)
                .HasForeignKey(d => d.FkTeamidTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_participates_team");
            

        });

        modelBuilder.Entity<Payouttransaction>(entity =>
        {
            entity.HasKey(e => e.IdPayouttransaction).HasName("payouttransaction_pkey");

            entity.ToTable("payouttransaction");

            entity.HasIndex(e => e.FkBetidBet, "uq_payout_bet").IsUnique();

            entity.Property(e => e.IdPayouttransaction).HasColumnName("id_payouttransaction");
            entity.Property(e => e.FkBetidBet).HasColumnName("fk_betid_bet");
            entity.Property(e => e.FkUseridUser).HasColumnName("fk_userid_user");
            entity.Property(e => e.Payoutdate).HasColumnName("payoutdate");
            entity.Property(e => e.Sum).HasColumnName("sum");

            entity.HasOne(d => d.FkBetidBetNavigation).WithOne(p => p.Payouttransaction)
                .HasForeignKey<Payouttransaction>(d => d.FkBetidBet)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payout_bet");

            entity.HasOne(d => d.FkUseridUserNavigation).WithMany(p => p.Payouttransactions)
                .HasForeignKey(d => d.FkUseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payout_user");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.IdPlayer).HasName("player_pkey");

            entity.ToTable("player");

            entity.Property(e => e.IdPlayer).HasColumnName("id_player");
            entity.Property(e => e.Ace).HasColumnName("ace");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Blocks).HasColumnName("blocks");
            entity.Property(e => e.Elo).HasColumnName("elo");
            entity.Property(e => e.FkTeamidTeam).HasColumnName("fk_teamid_team");
            entity.Property(e => e.Mistakes).HasColumnName("mistakes");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.Position)
                .HasMaxLength(255)
                .HasColumnName("position");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");

            entity.HasOne(d => d.FkTeamidTeamNavigation).WithMany(p => p.Players)
                .HasForeignKey(d => d.FkTeamidTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_player_team");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.IdTeam).HasName("team_pkey");

            entity.ToTable("team");

            entity.HasIndex(e => e.FkTeammanageridUser, "team_fk_teammanagerid_user_key").IsUnique();

            entity.Property(e => e.IdTeam).HasColumnName("id_team");
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .HasColumnName("country");
            entity.Property(e => e.Elo).HasColumnName("elo");
            entity.Property(e => e.FkTeammanageridUser).HasColumnName("fk_teammanagerid_user");
            entity.Property(e => e.FkTournamentidTournament).HasColumnName("fk_tournamentid_tournament");
            entity.Property(e => e.Isparticipating).HasColumnName("isparticipating");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Trainer)
                .HasMaxLength(255)
                .HasColumnName("trainer");

            entity.HasOne(d => d.FkTeammanageridUserNavigation).WithOne(p => p.Team)
                .HasForeignKey<Team>(d => d.FkTeammanageridUser)
                .HasConstraintName("fk_team_manager");

            entity.HasOne(d => d.FkTournamentidTournamentNavigation).WithMany(p => p.Teams)
                .HasForeignKey(d => d.FkTournamentidTournament)
                .HasConstraintName("fk_team_tournament");
        });

        modelBuilder.Entity<Teammanager>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("teammanager_pkey");

            entity.ToTable("teammanager");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("id_user");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");

            entity.HasOne(d => d.IdUserNavigation).WithOne(p => p.Teammanager)
                .HasForeignKey<Teammanager>(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_teammanager_user");
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.IdTournament).HasName("tournament_pkey");

            entity.ToTable("tournament");

            entity.Property(e => e.IdTournament).HasColumnName("id_tournament");
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .HasColumnName("country");
            entity.Property(e => e.Creationtype).HasColumnName("creationtype");
            entity.Property(e => e.Enddate).HasColumnName("enddate");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Startdate).HasColumnName("startdate");
            entity.Property(e => e.Teamnr).HasColumnName("teamnr");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("User_pkey");

            entity.ToTable("User");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Betvariance).HasColumnName("betvariance");
            entity.Property(e => e.Cardcvc).HasColumnName("cardcvc");
            entity.Property(e => e.Cardexpirationdate).HasColumnName("cardexpirationdate");
            entity.Property(e => e.Cardnumber)
                .HasMaxLength(255)
                .HasColumnName("cardnumber");
            entity.Property(e => e.Highoddsfrequency).HasColumnName("highoddsfrequency");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.Lossrate).HasColumnName("lossrate");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Riskfactor).HasColumnName("riskfactor");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
        });

        modelBuilder.Entity<Wager>(entity =>
        {
            entity.HasKey(e => e.IdWager).HasName("wager_pkey");

            entity.ToTable("wager");

            //entity.HasIndex(e => new { e.FkMatchidMatch, e.FkMatchfkTournamentidTournament }, "uq_wager_match").IsUnique();

            entity.Property(e => e.IdWager).HasColumnName("id_wager");
            entity.Property(e => e.Chance).HasColumnName("chance");
            entity.Property(e => e.Combinedsum).HasColumnName("combinedsum");
            entity.Property(e => e.FkMatchfkTournamentidTournament).HasColumnName("fk_matchfk_tournamentid_tournament");
            entity.Property(e => e.FkMatchidMatch).HasColumnName("fk_matchid_match");
            entity.Property(e => e.Hasfinished).HasColumnName("hasfinished");

            entity.HasOne(d => d.Match).WithOne(p => p.Wager)
                .HasForeignKey<Wager>(d => new { d.FkMatchidMatch, d.FkMatchfkTournamentidTournament })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_wager_match");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
