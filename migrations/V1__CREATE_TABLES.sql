CREATE TABLE Galaxy_Type (
    galaxy_type_id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255)
);

CREATE TABLE Galaxy (
    galaxy_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    galaxy_type_id INT REFERENCES Galaxy_Type(galaxy_type_id),
    distance_from_earth INT,
    description VARCHAR(255)
);

CREATE TABLE Planet_Type (
    planet_type_id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255)
);

CREATE TABLE Planets (
    planet_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    galaxy_id INT NOT NULL REFERENCES Galaxy(galaxy_id),
    planet_type_id INT REFERENCES Planet_Type(planet_type_id),
    has_life BOOLEAN NOT NULL,
    coordinates VARCHAR(64)
);

CREATE TABLE Rank (
    rank_id SERIAL PRIMARY KEY,
    title VARCHAR(25) NOT NULL UNIQUE,
    description VARCHAR(255)
);

CREATE TABLE Users (
    user_id SERIAL PRIMARY KEY,
    full_name VARCHAR(64) NOT NULL UNIQUE,
    email_address VARCHAR(64) NOT NULL UNIQUE,
    google_id VARCHAR(255) NOT NULL UNIQUE,
    rank_id INT NOT NULL REFERENCES Rank(rank_id),
    is_active BOOLEAN NOT NULL,
    created_at TIMESTAMP NOT NULL
);

CREATE TABLE Status (
    status_id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255)
);

CREATE TABLE Mission_Type (
    mission_type_id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255)
);

CREATE TABLE Missions (
    mission_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    mission_type_id INT NOT NULL REFERENCES Mission_Type(mission_type_id),
    launch_date TIMESTAMP NOT NULL,
    destination_planet_id INT NOT NULL REFERENCES Planets(planet_id),
    status_id INT NOT NULL REFERENCES Status(status_id),
    reward_credit VARCHAR(64),
    feedback VARCHAR(255),
    created_by VARCHAR(64) NOT NULL REFERENCES Users(full_name)
);

CREATE TABLE Crew (
    crew_id SERIAL PRIMARY KEY,
    name VARCHAR(64) NOT NULL,
    is_available BOOLEAN NOT NULL
);

CREATE TABLE Mission_Crew (
    mission_crew_id SERIAL PRIMARY KEY,
    mission_id INT NOT NULL REFERENCES Missions(mission_id),
    crew_id INT NOT NULL REFERENCES Crew(crew_id),
    assigned_at TIMESTAMP NOT NULL
);

CREATE TABLE User_Crew (
    user_crew_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL UNIQUE REFERENCES Users(user_id),
    crew_id INT NOT NULL REFERENCES Crew(crew_id)
);
