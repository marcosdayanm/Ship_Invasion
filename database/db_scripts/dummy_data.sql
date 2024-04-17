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

-- Area
INSERT INTO Area (LengthX, LengthY) VALUES 
(1, 1),
(2, 1),
(1, 2),
(3, 1),
(1, 3),
(4, 1),
(1, 4),
(5, 1),
(1, 5),
(6, 1),
(1, 6);
SELECT * FROM Area;

-- Card
INSERT INTO Card (Name, QualityId, CardTypeId, AreaId) VALUES 
('Dardo', 1, 1, 1), 
('Duplo', 1, 1, 2), 
('Flecha', 1, 1, 3), 
('Tridente', 1, 1, 4), 
('Áncora', 1, 1, 5), 
('Halberd', 2, 1, 6), 
('Javelin', 2, 1, 7), 
('Kraken', 2, 1, 8),
('Zeppelin', 2, 1, 9),
('Leviatán', 3, 1, 10),
('Ballistic', 3, 1, 11),

('Yate', 1, 2, 1),
('Boat', 1, 2, 2),
('Raft', 1, 2, 3),
('Dinghy', 1, 2, 4),
('Skiff', 1, 2, 5),
('Galeón', 2, 2, 6), -- 6
('Clipper', 2, 2, 7), -- 7
('Corbeta', 2, 2, 8), -- 8
('Fragata', 2, 2, 9),
('Búnker', 3, 2, 10),
('DeathStar', 3, 2, 11);
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
