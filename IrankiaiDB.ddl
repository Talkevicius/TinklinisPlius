-- PostgreSQL

-- Man iskarto nepavyko viso query ipastint i pgadmin tai po dvi lenteles dejau. Nors turetu leist ir visa (reikia naudot "Execute script" o ne "Execute query"

CREATE TABLE Tournament (
    id_Tournament SERIAL PRIMARY KEY,
    title VARCHAR(255),
    startDate DATE,
    endDate DATE,
    country VARCHAR(255),
    teamNr INT,
    creationType BOOLEAN,
    isActive BOOLEAN
);

CREATE TABLE "User" (
    id_User SERIAL PRIMARY KEY,
    name VARCHAR(255),
    surname VARCHAR(255),
    login VARCHAR(255),
    password VARCHAR(255),
    cardNumber VARCHAR(255),
    cardExpirationDate DATE,
    cardCVC INT,
    riskFactor FLOAT,
    lossRate FLOAT,
    betVariance FLOAT,
    highOddsFrequency FLOAT
);

CREATE TABLE Inspector (
    id_User INT PRIMARY KEY,
    name VARCHAR(255),
    surname VARCHAR(255),
    CONSTRAINT fk_inspector_user FOREIGN KEY(id_User) REFERENCES "User"(id_User)
);

CREATE TABLE TeamManager (
    id_User INT PRIMARY KEY,
    name VARCHAR(255),
    surname VARCHAR(255),
    CONSTRAINT fk_teammanager_user FOREIGN KEY(id_User) REFERENCES "User"(id_User)
);

CREATE TABLE Team (
    id_Team SERIAL PRIMARY KEY,
    name VARCHAR(255),
    country VARCHAR(255),
    trainer VARCHAR(255),
    elo INT,
    isParticipating BOOLEAN,
    fk_TeamManagerid_User INT UNIQUE,
    fk_Tournamentid_Tournament INT,
    CONSTRAINT fk_team_manager FOREIGN KEY(fk_TeamManagerid_User) REFERENCES TeamManager(id_User),
    CONSTRAINT fk_team_tournament FOREIGN KEY(fk_Tournamentid_Tournament) REFERENCES Tournament(id_Tournament)
);

CREATE TABLE Match (
    id_Match SERIAL,
    title VARCHAR(255),
    date DATE,
    team1Score INT,
    team2Score INT,
    hasHappened BOOLEAN,
    placeInTournament INT,
    fk_Teamid_Team INT NOT NULL,
    fk_Tournamentid_Tournament INT NOT NULL,
    fk_Matchid_Match INT,
    fk_Matchfk_Tournamentid_Tournament INT,
    fk_Inspectorid_User INT,
    PRIMARY KEY(id_Match, fk_Tournamentid_Tournament),
    CONSTRAINT uq_match_team UNIQUE(fk_Teamid_Team),
    CONSTRAINT fk_match_team FOREIGN KEY(fk_Teamid_Team) REFERENCES Team(id_Team),
    CONSTRAINT fk_match_tournament FOREIGN KEY(fk_Tournamentid_Tournament) REFERENCES Tournament(id_Tournament),
    CONSTRAINT fk_match_inspector FOREIGN KEY(fk_Inspectorid_User) REFERENCES Inspector(id_User)
);

CREATE TABLE Player (
    id_Player SERIAL PRIMARY KEY,
    name VARCHAR(255),
    surname VARCHAR(255),
    birthdate DATE,
    position VARCHAR(255),
    elo FLOAT,
    points INT,
    blocks INT,
    ace INT,
    mistakes INT,
    fk_Teamid_Team INT NOT NULL,
    CONSTRAINT fk_player_team FOREIGN KEY(fk_Teamid_Team) REFERENCES Team(id_Team)
);

CREATE TABLE participates (
    fk_Matchid_Match INT,
    fk_Matchfk_Tournamentid_Tournament INT,
    fk_Teamid_Team INT,
    PRIMARY KEY(fk_Matchid_Match, fk_Matchfk_Tournamentid_Tournament, fk_Teamid_Team),
    CONSTRAINT fk_participates_match FOREIGN KEY(fk_Matchid_Match, fk_Matchfk_Tournamentid_Tournament)
        REFERENCES Match(id_Match, fk_Tournamentid_Tournament)
);

CREATE TABLE Wager (
    id_Wager SERIAL PRIMARY KEY,
    chance INT,
    combinedSum INT,
    hasFinished BOOLEAN,
    fk_Matchid_Match INT NOT NULL,
    fk_Matchfk_Tournamentid_Tournament INT NOT NULL,
    CONSTRAINT uq_wager_match UNIQUE(fk_Matchid_Match, fk_Matchfk_Tournamentid_Tournament),
    CONSTRAINT fk_wager_match FOREIGN KEY(fk_Matchid_Match, fk_Matchfk_Tournamentid_Tournament)
        REFERENCES Match(id_Match, fk_Tournamentid_Tournament)
);

CREATE TABLE Bet (
    id_Bet SERIAL PRIMARY KEY,
    moneyBetted INT,
    chanceOfWinning INT,
    payout INT,
    betDate DATE,
    betType BOOLEAN,
    result BOOLEAN,
    fk_Wagerid_Wager INT NOT NULL,
    fk_Userid_User INT NOT NULL,
    CONSTRAINT fk_bet_wager FOREIGN KEY(fk_Wagerid_Wager) REFERENCES Wager(id_Wager),
    CONSTRAINT fk_bet_user FOREIGN KEY(fk_Userid_User) REFERENCES "User"(id_User)
);

CREATE TABLE PayoutTransaction (
    id_PayoutTransaction SERIAL PRIMARY KEY,
    sum INT,
    payoutDate DATE,
    fk_Betid_Bet INT NOT NULL,
    fk_Userid_User INT NOT NULL,
    CONSTRAINT uq_payout_bet UNIQUE(fk_Betid_Bet),
    CONSTRAINT fk_payout_bet FOREIGN KEY(fk_Betid_Bet) REFERENCES Bet(id_Bet),
    CONSTRAINT fk_payout_user FOREIGN KEY(fk_Userid_User) REFERENCES "User"(id_User)
);
