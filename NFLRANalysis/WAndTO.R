# Analyze winning versus turnovers from 2011 - 2015
#Coefficients:
# (Intercept) ToWA$PDif
#library(stats)
library(plyr)
library(dplyr)
library(reshape2)
library(RPostgreSQL)
library(DBI)
library(ggplot2)


# Make connection to postgresql
drv <- dbDriver("PostgreSQL")
con <- dbConnect(drv, dbname = "nfldb", port = "5432", host = "localhost", user = "postgres", password = "Canon7D")

# Update the teamyearstats with cost information
if (!exists("pp")) {
    pp <- dbReadTable(con, c("public", "play_player"))
    pp <- select(pp, gsis_id, team, fumbles_lost, passing_int)
    pp <- mutate(pp, totto = fumbles_lost + passing_int)
    pp <- aggregate(totto ~ gsis_id + team, pp, sum)
}

# Update the teamyearstats with cost information
if (!exists("games")) {
    games <- dbReadTable(con, c("public", "game"))
    games <- filter(games, games$season_type == 'Regular')
    games <- select(games, gsis_id, home_team, home_score,
                    away_team, away_score)

}

# work to add in turnovers since the games turnovers were in correct.
TW <- merge(games, pp)   # [T]urnover [Wins]
TWHome <- filter(TW, team == home_team)
names(TWHome)[names(TWHome) == 'totto'] <- 'home_turnovers'
TWHome <- select(TWHome, gsis_id, home_team, home_score, home_turnovers)

TWAway <- filter(TW, team == away_team)
names(TWAway)[names(TWAway) == 'totto'] <- 'away_turnovers'
TWAway <- select(TWAway, gsis_id, away_team, away_score, away_turnovers)

TW <- merge(TWHome, TWAway)

# Now add in columns for score and turnover difference
# reverse to create a positive values for good.
TW <- mutate(TW, TODifAway =   home_turnovers - away_turnovers, 
                         TODifHome =   away_turnovers - home_turnovers)

TW <- mutate(TW, PointDifHome = home_score - away_score,
                         PointDifAway = away_score - home_score)

# Now reshape the data frame so the function [Turn[o]ver[W]ins
ToW <- transmute(TW, Team = home_team, TODif = TODifHome, PDif = PointDifHome)
ToWTemp <- transmute(TW, Team = away_team, TODif = TODifAway, PDif = PointDifAway)
ToW <- rbind(ToW, ToWTemp)
ToW <- filter(ToW, TODif >= 0 )

# lets test the idea that winning is inversely correlates with turn overs 
g <- ggplot(ToW, aes(TODif, PDif)) + geom_point(color = "firebrick")
g + stat_smooth(method = "lm", se = FALSE) + ggtitle("Turnover Versus Point Differentials Per Game")

ToWA <- aggregate(cbind(TODif, PDif) ~ Team, data = ToW, FUN = sum)
ToWA <- arrange(ToWA,desc(TODif))

# lets test the idea that point differences are inversely proportional to turnover advantages. 
g <- ggplot(ToWA, aes(TODif, PDif)) + geom_point(color = "firebrick")
g + stat_smooth(method = "lm", se = FALSE)

dbDisconnect(con)


