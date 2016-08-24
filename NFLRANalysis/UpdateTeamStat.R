update <- function(i) {
    drv <- dbDriver("PostgreSQL")
    con <- dbConnect(drv, dbname = "nfldb", host = "localhost", port = "5432", user = "postgres", password = "Canon7D")
    txt <- paste("UPDATE teamyearstats SET offense_cost=", teamyearstats$offense_cost[i], 
        ",defense_cost=", teamyearstats$defense_cost[i], 
        ",total_cost=", teamyearstats$total_cost[i], 
        " where id=", teamyearstats$id[i])
    dbGetQuery(con, txt)
    dbDisconnect(con)
}