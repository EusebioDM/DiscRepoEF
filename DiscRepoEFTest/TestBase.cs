using System;
using System.Collections.Generic;
using DiscRepoEF;
using DiscRepoEFTest.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DiscRepoEFTest
{
    public class TestBase
    {
        protected Sport football;
        protected Team boca;
        protected Team river;
        protected Team tomba;
        protected User messi;
        protected User maradona;
        protected Encounter bocaRiver;
        protected Encounter riverTomba;
        protected IDesignTimeDbContextFactory<Context> contextFactory;
        protected Repository<Encounter> encounterRepo;
        protected Repository<Team> teamRepo;

        protected void Initialize()
        {
            football = CreateFootballSport();
            boca = CreateBocaTeam();
            river = CreateRiverTeam();
            tomba = CreateTombaTeam();
            messi = CreateMessiUser();
            maradona = CreateMaradonaUser();
            bocaRiver = CreateRiverBocaEncounter();
            riverTomba = CreateRiverTombaEncounter();
            contextFactory = new InMemoryContextFactory();
            encounterRepo = CreateEncounterRepo();
            teamRepo = CreateTeamRepo();
        }

        private Repository<Team> CreateTeamRepo()
        {
            Func<DbContext, DbSet<Team>> getSet = c => ((Context) c).Teams;
            return new Repository<Team>(getSet, contextFactory);
        }

        private Repository<Encounter> CreateEncounterRepo()
        {
            Func<DbContext, DbSet<Encounter>> getDbSet = c => ((Context) c).Encounters;
            return new Repository<Encounter>(getDbSet, contextFactory);
        }

        protected Encounter CreateRiverBocaEncounter()
        {
            
            Encounter encounter = new Encounter()
            {
                Sport = football,
                Id = Guid.NewGuid(),
                AwayTeam = boca,
                HomeTeam = river,
                DateTime = new DateTime(3000,10,10)
            };
            encounter.Comments.Add(new Comment()
            {
                User = messi,
                Id = Guid.NewGuid(),
                Message = "Aguante Boca",
            });
            encounter.Comments.Add(new Comment()
            {
                User = maradona,
                Id = Guid.NewGuid(),
                Message = "Aguante River",
            });
            return encounter;
        }

        protected Encounter CreateRiverTombaEncounter()
        {
            return new Encounter()
            {
                Sport = football,
                Id = Guid.NewGuid(),
                AwayTeam = river,
                HomeTeam = tomba,
                DateTime = new DateTime(3000,10,11)
            };
        }

        protected User CreateMaradonaUser()
        {
            User user = new User()
            {
                UserName = "Maradona",
                Password = "maradona123"
            };
            user.TeamUsers = new List<TeamUser>() {new TeamUser(river, user)};
            return user;
        }

        protected User CreateMessiUser()
        {
            User user = new User()
            {
                UserName = "Messi",
                Password = "messi123"
            };
            user.TeamUsers = new List<TeamUser>() {new TeamUser(boca, user), new TeamUser(tomba, user)};
            return user;
        }

        protected Sport CreateFootballSport()
        {
            return new Sport()
            {
                SportName = "Football"
            };
        }

        protected Team CreateTombaTeam()
        {
            return new Team()
            {
                Name = "Boca",
                Sport = football,
                SportName = football.SportName,
                Description = "Boca desc"
            };
        }

        protected Team CreateRiverTeam()
        {
            return new Team()
            {
                Name = "River",
                Sport = football,
                SportName = football.SportName,
                Description = "River desc"
            };
        }

        protected Team CreateBocaTeam()
        {
            return new Team()
            {
                Name = "Tomba",
                Sport = football,
                SportName = football.SportName,
                Description = "Tomba desc"
            };
        }
    }
}