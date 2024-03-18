USE ship_invasion;

-- Player
INSERT INTO Player (Username, Password) VALUES 
('Remy', 'Ship'), 
('Emily', 'Ship'), 
('Marcos', 'Ship');
SELECT * FROM Player;


-- CardType
INSERT INTO CardType (TypeName, IsAttackCard) VALUES 
("Attack", true),
("Defense", false);
SELECT * FROM CardType;


-- Area
INSERT INTO Area (LengthX, LengthY) VALUES 
(1, 1),
(1, 2),
(2, 1),
(1, 3),
(3, 1),
(1, 5),
(5, 1),
(10, 1),
(1, 10),
(3, 3);
SELECT * FROM Area;


-- Quality
INSERT INTO Quality (Quality) VALUES 
("Bronze"),
("Silver"),
("Gold");
SELECT * FROM Quality;



-- Card
INSERT INTO Card (Name, QualityId, CardTypeId, AreaId) VALUES 
('Canonball', 1, 1, 1), 
('Thunder', 2, 1, 4), 
('Electric storm', 3, 1, 9),
('Sailboat', 1, 2, 1),
('Cruiser', 2, 2, 5),
('Ship', 3, 2, 10);
SELECT * FROM Card;


-- Arena
INSERT INTO Arena (Name, Level, MatchesRequired) VALUES
("Mar Abierto", 1, 0),
("Tormenta Eléctrica", 2, 5),
("Río de Fuego", 3, 20),
("Pantano tóxico", 4, 50);
SELECT * FROM Arena;





