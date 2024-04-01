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

-- Sprite
INSERT INTO Sprite (Name, IsAddOn, Price) VALUES
("ElectroShips", true, 100),
("SpaceShips", true, 200),
("PirateShips", false, 0),
("AlienShips", true, 150),
("OpenSea", false, 0),
("ElectricStorm", false, 0),
("RiverOfFire", false, 0),
("ToxicSwamp", false, 0);

-- PurchasedSprite
INSERT INTO PurchasedSprite (PlayerId, SpriteId) VALUES
(1, 1),
(2, 2),
(3, 3);


-- Quality
INSERT INTO Quality (QualityName) VALUES 
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

-- Game
INSERT INTO Game (PlayerId, ArenaId, IsPlayerWon) VALUES
(1, 1, True),
(2, 1, false),
(3, 1, True),
(1, 2, True),
(2, 2, false),
(3, 2, True),
(1, 3, True),
(2, 3, false),
(3, 3, True),
(1, 4, True),
(2, 4, false),
(3, 4, True);


-- Play
INSERT INTO Play (PlayNumber, IsPlayerPlay, NumFieldsCovered, GameId, CardPlayedId) VALUES 
(1, true, 5, 1, 1), 
(2, false, 0, 1, 2), 
(3, true, 3, 1, 3), 
(4, true, 2, 2, 4), 
(5, false, 0, 2, 5), 
(6, true, 4, 2, 4), 
(7, true, 1, 3, 2), 
(2, false, 0, 3, 3), 
(3, true, 3, 3, 1), 
(6, true, 2, 4, 4), 
(11, false, 0, 4, 5), 
(12, true, 4, 4, 1);
