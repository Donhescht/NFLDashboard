# Analyze point differential versus average starting field position from 2009 - 2015
library(stats)
library(plyr)
library(dplyr)
library(reshape2)
library(RPostgreSQL)
library(DBI)
library(ggplot2)


# Make connection to postgresql
drv <- dbDriver("PostgreSQL")
con <- dbConnect(drv, dbname = "nfldb", port = "5432", host = "localhost", user = "postgres", password = "Canon7D")

# The play_player table provides play information in detailed fashion.
if (!exists("D")) {
    D <- dbReadTable(con, c("public", "drive"))
}

# Create a data frame for big playes in games, season and team
# Removed receiving and rushing big plays as correlation was 0.06
# Now just represent special teams and defense.  Still correlation is 0.17...not very good.
big <- select(pp, gsis_id, team, puntret_yds, kickret_yds, defense_int_yds)
big <- mutate(big, pyds = puntret_yds + kickret_yds + defense_int_yds)
big <- filter(big, pyds >= 25)

aBig <- aggregate(pyds ~ team, big, sum)
aBig <- arrange(aBig, desc(pyds))

g <- ggplot(aBig, aes(reorder(team, pyds), pyds)) + geom_bar(stat = "identity")
g + coord_flip() + ylab("Total Yards") + xlab("Team") + ggtitle("Big Plays >=25Yds / 2009-2015")

# Now get game information so wins can be correlated big plays
if (!exists("games")) {
    games <- dbReadTable(con, c("public", "game"))
    games <- filter(games, games$season_type == 'Regular' & games$season_year > 2012)
    games <- select(games, gsis_id, season_year, home_team, home_score,
             away_team, away_score)
}

BPGW <- merge(big, games) # [B]ig [P]lay [G]ames [W]ins 
BPGWHome <- filter(BPGW, team == home_team)
BPGWHome <- mutate(BPGWHome, PDif = home_score - away_score)
BPGWHome <- select(BPGWHome, gsis_id, season_year, PDif, pyds)
BPGWAway <- filter(BPGW, team == away_team)
BPGWAway <- mutate(BPGWAway, PDif = away_score - home_score)
BPGWAway <- select(BPGWAway, gsis_id, season_year, PDif, pyds)
BPGW <- rbind(BPGWHome, BPGWAway)


# lets test the idea that winning is inversely correlates with turn overs 
g <- ggplot(BPGW, aes(pyds, PDif)) + geom_point(color = "firebrick")
g + stat_smooth(method = "lm", se = FALSE)

dbDisconnect(con)
