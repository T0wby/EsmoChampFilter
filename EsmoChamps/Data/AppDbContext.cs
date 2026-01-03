using EsmoChamps.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EsmoChamps.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Champion> Champions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RangeType> RangeTypes { get; set; }
        public DbSet<ChampType> ChampTypes { get; set; }
        public DbSet<StrengthTitle> StrengthTitles { get; set; }
        public DbSet<ChampionStrength> ChampionStrengths { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EsmoChamps",
            "Champions.db");

            PrepareDatabase(dbPath);

            options.UseSqlite($"Data Source={dbPath}");
        }

        private void PrepareDatabase(string dbPath)
        {
            var directory = Path.GetDirectoryName(dbPath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(dbPath))
            {
                var sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "MasterData", "Champions.db");

                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, dbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Seed database not found at {sourcePath}");
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Top Lane" },
                new Role { Id = 2, Name = "Jungle" },
                new Role { Id = 3, Name = "Mid Lane" },
                new Role { Id = 4, Name = "Bot Lane" },
                new Role { Id = 5, Name = "Support" }
            );

            modelBuilder.Entity<RangeType>().HasData(
                new RangeType { Id = 1, Name = "Melee" },
                new RangeType { Id = 2, Name = "Ranged" }
            );

            modelBuilder.Entity<ChampType>().HasData(
                new ChampType { Id = 1, Name = "Artillery Mage" },
                new ChampType { Id = 2, Name = "Burst Mage" },
                new ChampType { Id = 3, Name = "Bruiser" },
                new ChampType { Id = 4, Name = "Defensive Tank" },
                new ChampType { Id = 5, Name = "Offensive Tank" },
                new ChampType { Id = 6, Name = "Assassin" },
                new ChampType { Id = 7, Name = "Battle Mage" },
                new ChampType { Id = 8, Name = "Marksman" },
                new ChampType { Id = 9, Name = "Enchanter" },
                new ChampType { Id = 10, Name = "Catcher" },
                new ChampType { Id = 11, Name = "Diver" },
                new ChampType { Id = 12, Name = "Duelist" }
            );

            modelBuilder.Entity<ChampionStrength>().HasKey(cs => new { cs.ChampionId, cs.StrengthTitleId });

            modelBuilder.Entity<ChampionStrength>()
                .HasOne(cs => cs.Champion)
                .WithMany(c => c.Strengths)
                .HasForeignKey(cs => cs.ChampionId);

            modelBuilder.Entity<ChampionStrength>()
                .HasOne(cs => cs.StrengthTitle)
                .WithMany(st => st.ChampionStrengths)
                .HasForeignKey(cs => cs.StrengthTitleId);

            modelBuilder.Entity<StrengthTitle>().HasData(
                new StrengthTitle { Id = 1, Title = "Engage", Description = "The ability to force a fight to start, usually by closing the gap quickly." },
                new StrengthTitle { Id = 2, Title = "Crowd Control", Description = "The ability to limit enemy movement or actions through stuns, slows or roots." },
                new StrengthTitle { Id = 3, Title = "Pick Potential", Description = "The ability to catch out a single enemy and eliminate them before a teamfight starts." },
                new StrengthTitle { Id = 4, Title = "Objective Control", Description = "The ability to secure major objectives like Dragon, Baron or Ram." },
                new StrengthTitle { Id = 5, Title = "Teamfight", Description = "Strength in large-scale 5v5 battles involving the entire team." },
                new StrengthTitle { Id = 6, Title = "Area Damage", Description = "The ability to deal significant damage to multiple targets simultaneously." },
                new StrengthTitle { Id = 7, Title = "Dueling", Description = "Strength in 1v1 combat scenarios against other heroes." },
                new StrengthTitle { Id = 8, Title = "Clear Speed", Description = "The speed at which the hero can kill jungle camps or minion waves." },
                new StrengthTitle { Id = 9, Title = "Skirmishing", Description = "Strength in small-scale, chaotic fights (e.g. 2v2 or 3v3)." },
                new StrengthTitle { Id = 10, Title = "Ganking", Description = "The ability to ambush enemies in their lanes to secure kills or advantages." },
                new StrengthTitle { Id = 11, Title = "Zoning", Description = "The ability to control space and deny enemies access to specific areas." },
                new StrengthTitle { Id = 12, Title = "Wave Clear", Description = "The ability to kill minion waves quickly to defend turrets or push lanes." },
                new StrengthTitle { Id = 13, Title = "Siege", Description = "The ability to pressure enemy turrets and force enemies to defend under them." },
                new StrengthTitle { Id = 14, Title = "Peel", Description = "The ability to protect vulnerable allies by intercepting or disabling enemy divers." },
                new StrengthTitle { Id = 15, Title = "Roaming", Description = "The ability to quickly move between lanes to impact other parts of the map." },
                new StrengthTitle { Id = 16, Title = "Split Push", Description = "The ability to apply pressure in a side lane alone while the team fights elsewhere." },
                new StrengthTitle { Id = 17, Title = "Cleanup", Description = "The ability to chase down and finish off low-health enemies after a fight." },
                new StrengthTitle { Id = 18, Title = "Poke", Description = "The ability to deal damage from a long range without fully committing to a fight." },
                new StrengthTitle { Id = 19, Title = "Lane Dominance", Description = "Strength in controlling the early laning phase and suppressing the opponent." },
                new StrengthTitle { Id = 20, Title = "Snowball", Description = "The ability to convert an early lead into an unstoppable advantage." },
                new StrengthTitle { Id = 21, Title = "Disengage", Description = "The ability to stop a fight, push enemies away and retreat safely." },
                new StrengthTitle { Id = 22, Title = "Stealth", Description = "The ability to become invisible or camouflaged to surprise enemies or escape." },
                new StrengthTitle { Id = 23, Title = "Invasion", Description = "The ability to enter enemy territory tp steal resources or kill the enemy jungler." },
                new StrengthTitle { Id = 24, Title = "Kiting", Description = "The ability to deal damage while moving away to maintain a safe distance from enemies." },
                new StrengthTitle { Id = 25, Title = "Fight Resets", Description = "Mechanics that refresh ability cooldowns or resources upon getting a takedown." }
            );
        }
    }
}
