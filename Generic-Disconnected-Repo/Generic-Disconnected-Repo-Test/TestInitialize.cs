using System;
using System.Collections.Generic;
using Generic_Disconnected_Repo;
using Generic_Disconnected_Repo_Test.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Generic_Disconnected_Repo_Test
{
    [TestClass]
    public class TestInitialize
    {
        protected Sport football;
        protected Team boca;
        protected Team river;
        protected Team tomba;
        protected User messi;
        protected User maradona;
        protected Encounter bocaRiver;
        protected Encounter riverTomba;
        protected Repository<Encounter> encounterRepo;

        protected void Initialize()
        {
            football = CreateFootballSport();
            boca = CreateBocaTeam();
            river = CreateRiverTeam();
            tomba = CreateTombaTeam();
            messi = CreateMessiUser();
            maradona = CreateMaradonaUser();
            bocaRiver = CreateBocaRiverEncounter();
            riverTomba = CreateRiverTombaEncounter();
            encounterRepo = CreateEncounterRepo();
        }

        private Repository<Encounter> CreateEncounterRepo()
        {
            throw new NotImplementedException();
        }

        protected Encounter CreateRiverTombaEncounter()
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

        protected Encounter CreateBocaRiverEncounter()
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