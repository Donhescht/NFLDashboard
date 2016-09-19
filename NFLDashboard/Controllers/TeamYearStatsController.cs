using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NFLDashboard.Models;
using System.Collections.Generic;

namespace NFLDashboard.Controllers
{
    public class TeamYearStatsController : Controller
    {
        private TeamYearStatsDBContext db = new TeamYearStatsDBContext();

        public class StatTimeOptions
        {
            public StatTimeOptions(int aStartYear, int aEndYear)
            {
                StartAtThisYear = aStartYear;
                EndAtThisYear = aEndYear;
            }
            public int StartAtThisYear { get; set; }
            public int EndAtThisYear { get; set; }
        }

        public class AggregateTeamStats 
        {
            public string Team { get; set; }
            public List<int> Years { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public int Cost { get; set; }
           
            public List<int> WinsEachYear { get; set; }
            public List<int> LossesEachYear { get; set; }
            public List<int> CostEachYear { get; set; }
        }

        private StatTimeOptions yearsForStatData = new StatTimeOptions(2011, 2015);
        // GET: TeamYearStats
        public ActionResult Index(int startYear = 2011, int endYear = 2015)
        {
            AggregateTeamStats AgTeamStats = new AggregateTeamStats();
            List<TeamYearStat> aTeamYearStatList = db.teamYearStats.ToList();
            List<AggregateTeamStats> aList;

            aList = CreateAggStats(aTeamYearStatList, startYear, endYear);

            return View(aList);
        }



        // POST: TeamYearStats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,team_id,season_year,wins,losses,offense_cost,defense_cost,total_cost")] TeamYearStat teamYearStat)
        {
            if (ModelState.IsValid)
            {
                db.teamYearStats.Add(teamYearStat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teamYearStat);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private List<AggregateTeamStats> CreateAggStats(
        List<TeamYearStat> aTeamYearStatList,
        int startYear,
        int endYear)
        {
            List<AggregateTeamStats> aList = (
                from tystat in aTeamYearStatList
                where (tystat.season_year >= startYear && tystat.season_year <= endYear)
                group tystat by  tystat.team_id into AgStats
                orderby AgStats.Key
                select new AggregateTeamStats
                {
                    Team = AgStats.Key,
                    Years = AgStats.Select(_ => _.season_year).ToList(),
                    Wins = AgStats.Sum(_ => _.wins),
                    Losses = AgStats.Sum(_ => _.losses),
                    Cost = AgStats.Sum(_ => _.total_cost),
                    WinsEachYear = AgStats.Select(_ => _.wins).ToList(),
                    CostEachYear = AgStats.Select(_ => _.total_cost).ToList(),
                    LossesEachYear = AgStats.Select(_ => _.losses).ToList(),
                }).ToList();

            return aList;
        }
    }
}
