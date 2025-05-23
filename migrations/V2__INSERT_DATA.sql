INSERT INTO Galaxy_Type (name, description) VALUES
('Spiral', 'A galaxy with a spiral structure like the Milky Way'),
('Elliptical', 'A smooth, featureless elliptical shape'),
('Irregular', 'A galaxy with no distinct shape');

INSERT INTO Galaxy (name, galaxy_type_id, distance_from_earth, description) VALUES
('Milky Way', 1, 0, 'Our home galaxy'),
('Andromeda', 1, 2537000, 'Nearest major spiral galaxy to the Milky Way'),
('Sombrero Galaxy', 2, 29000000, 'Bright galaxy with a large central bulge'),
('Large Magellanic Cloud', 3, 163000, 'A satellite galaxy of the Milky Way');

INSERT INTO Planet_Type (name, description) VALUES
('Terrestrial', 'Rocky surface, like Earth or Mars'),
('Gas Giant', 'Composed mainly of hydrogen and helium, like Jupiter'),
('Ice Giant', 'Contains heavier volatile substances like Neptune'),
('Dwarf', 'Small planetary bodies like Pluto');

INSERT INTO Planets (name, galaxy_id, planet_type_id, has_life, coordinates) VALUES
('Earth', 1, 1, TRUE, 'RA: 0h Dec: 0°'),
('Mars', 1, 1, FALSE, 'RA: 23h Dec: -15°'),
('Jupiter', 1, 2, FALSE, 'RA: 16h Dec: -22°'),
('Neptune', 1, 3, FALSE, 'RA: 1h Dec: -6°'),
('Proxima b', 2, 1, FALSE, 'RA: 14h Dec: -62°'),
('Pluto', 1, 4, FALSE, 'RA: 19h Dec: -16°');

INSERT INTO Rank (title, description) VALUES
('Director', 'Assigns missions, crew and oversees the whole organization'),
('Commander', 'Leads missions and oversees strategy.'),
('Captain', 'Navigates and flies spacecraft'),
('Officer', 'Standard operational rank.'),
('Admiral', 'Highest strategic command rank.'),
('Cadet', 'Entry-level rank for new recruits.');

INSERT INTO Status (name, description) VALUES
('Planned', 'Mission is being planned'),
('Launched', 'Mission has launched'),
('Completed', 'Mission successfully completed'),
('Failed', 'Mission failed'),
('Aborted', 'Mission was canceled');

INSERT INTO Mission_Type (name, description) VALUES
('Exploration', 'Investigating new planets and galaxies'),
('Colonization', 'Establishing colonies on habitable planets'),
('Research', 'Conducting scientific experiments and studies'),
('Rescue', 'Emergency missions to retrieve crew or equipment'),
('Supply', 'Delivering essential resources to established bases');
