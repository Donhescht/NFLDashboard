using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace NFLDashboard.Models
{
    [Table("teamyearstats",Schema ="public")]
    public class TeamYearStat
    {
        [Key]
        public int id { get; set; }
        public string team_id { get; set; }
        public int season_year { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public int offense_cost{ get; set; }
        public int defense_cost { get; set; }
        public int total_cost { get; set; }
    }


    public class TeamYearStatsDBContext : DbContext
    {
        public TeamYearStatsDBContext() : base(nameOrConnectionString: "PostgresqlDB")
        {
            Debug.Write(Database.Connection.ConnectionString);
        }
        public DbSet<TeamYearStat> teamYearStats { get; set; }
    }
}