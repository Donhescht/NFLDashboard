# Analyze spending versus winning from 2011-2016
library(stats)
library(dplyr)
library(RPostgreSQL)
library(DBI)
library(ggplot2)


# Make connection to postgresql
drv <- dbDriver("PostgreSQL")
con <- dbConnect(drv, dbname = "nfldb", port = "5432", host = "localhost", user = "postgres", password = "Canon7D")

# Update the teamyearstats with cost information
tys <- dbReadTable(con, c("public", "teamyearstats"))
tys <- filter(tys, total_cost > 0)
tys <- select(tys, - id)
tysg <- group_by(tys, team_id)
tysgmean = aggregate(tysg, list(Team = tys$team_id), mean)
tysgmean <- select(tysgmean, - team_id)
tysgmean <- select(tysgmean, - season_year)

ggplot(data = tys, aes(x = season_year, y = total_cost, color = wins)) +
scale_colour_gradient(low = "red", high = "green") +
geom_point() +
geom_smooth(method = "lm", color = "green", linetype = 2) +
labs(title = "Cost and Wins Per Year", x = "Year", y = "Total Cost", color = "Wins")

special = tysgmean[tysgmean$Team == "CLE" | tysgmean$Team == "NE",]
ggplot(data = cle, aes(x = (wins / (losses + wins)), y = total_cost)) +
geom_point() +
geom_smooth(method = "lm", color = "red", linetype = 2) +
labs(title = "Cle Winning and Total Cost (AAV)", x = "Win Percentage", y = "Total Cost")

ggplot() +
geom_point(data = cle, aes(x = wins, y = total_cost, colour = "orange", size = 6)) +
geom_point(data = ne, aes(x = wins, y = total_cost, colour = "blue", size = 4)) +
geom_point(data = tys, aes(x = wins, y = total_cost)) +
labs(title = "CLE/NE Winning and Total Cost (AAV)", x = "Wins", y = "Total Cost", colour = "NE,CLE", size = "NE,CLE")

ggplot(data = tysgmean, aes(x = tysgmean$wins, y = tysgmean$total_cost)) +
geom_point() +
geom_text(aes(label = tysgmean$Team), size = 6, colour = "red") +
labs(title = "2011-2015 Mean Wins and Total Cost (AAV)", x = "Wins", y = "Total Cost")

ggplot(data = tysgmean, aes(x = wins, y = total_cost)) +
#scale_colour_gradient(low = "red", high = "green") +
geom_bar() +
labs(title = "Cost and Wins Per Year", x = "Team", y = "Total Cost")

ggplot(tysgmean, aes(x = total_cost/1000000, y = (wins + losses)-16, label = Team)) +
    geom_point(aes(size = wins), shape=1) +
    geom_text(hjust = 2, size = 2, color="black") +
    scale_size(range = c(1, 15)) +
    labs(title = "Cost, Wins, Post Season Games 2011-2015", x = "Avg Yearly Cost (AAV in Millions)", y = "Avg Post Season Games") +
    theme_bw()