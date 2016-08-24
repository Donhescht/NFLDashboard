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

        public class AggregateTeamStats 
        {
            public int StartYear { get; set; }
            public int EndYear { get; set; }
            public string Team { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public int Cost { get; set; }

        }

        // GET: TeamYearStats
        public ActionResult Index()
        {
            AggregateTeamStats AgTeamStats = new AggregateTeamStats();
            List<TeamYearStat> aTeamYearStatList = db.teamYearStats.ToList();

            List<AggregateTeamStats> aList = (
                from tystat in aTeamYearStatList
                where (tystat.season_year >= 2010 && tystat.season_year <= 2015)
                group tystat by tystat.team_id into AgStats
                orderby AgStats.Key
                select new AggregateTeamStats
                {
                    Team = AgStats.Key,
                    StartYear = AgStats.Min(_ => _.season_year),
                    EndYear = AgStats.Max(_ => _.season_year),
                    Wins = AgStats.Sum(_ => _.wins),
                    Losses = AgStats.Sum(_ => _.losses),
                    Cost = AgStats.Sum(_ => _.total_cost)
                }).ToList();
                

            return View(aList);
        }

        // GET: TeamYearStats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamYearStat teamYearStat = db.teamYearStats.Find(id);
            if (teamYearStat == null)
            {
                return HttpNotFound();
            }
            return View(teamYearStat);
        }

        // GET: TeamYearStats/Create
        public ActionResult Create()
        {
            return View();
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

        // GET: TeamYearStats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamYearStat teamYearStat = db.teamYearStats.Find(id);
            if (teamYearStat == null)
            {
                return HttpNotFound();
            }
            return View(teamYearStat);
        }

        // POST: TeamYearStats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,team_id,season_year,wins,losses,offense_cost,defense_cost,total_cost")] TeamYearStat teamYearStat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teamYearStat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teamYearStat);
        }

        // GET: TeamYearStats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamYearStat teamYearStat = db.teamYearStats.Find(id);
            if (teamYearStat == null)
            {
                return HttpNotFound();
            }
            return View(teamYearStat);
        }

        // POST: TeamYearStats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TeamYearStat teamYearStat = db.teamYearStats.Find(id);
            db.teamYearStats.Remove(teamYearStat);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
