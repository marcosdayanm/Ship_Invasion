
-- Creating Schema
DROP SCHEMA IF EXISTS ship_invasion;
CREATE SCHEMA ship_invasion;
USE ship_invasion;



-- Creating tables


-- Creating Player table
CREATE TABLE Player(
	Id SMALLINT NOT NULL AUTO_INCREMENT,
    Username VARCHAR(50) NOT NULL,
    Password VARCHAR(100) NOT NULL,
    CreationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Wins SMALLINT NOT NULL DEFAULT 0,
    Losses SMALLINT NOT NULL DEFAULT 0,
    Coins MEDIUMINT NOT NULL DEFAULT 200,
    PRIMARY KEY (Id),
    CONSTRAINT chk_Wins CHECK (Wins >= 0),
    CONSTRAINT chk_Losses CHECK (Losses >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


-- Creating CardType table
CREATE TABLE CardType(
	Id TINYINT NOT NULL AUTO_INCREMENT,
    TypeName VARCHAR(50) NOT NULL,
	IsAttackCard BOOL NOT NULL,
    PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


-- Creating Area table
CREATE TABLE Area(
	Id TINYINT NOT NULL AUTO_INCREMENT,
    LengthX TINYINT NOT NULL,
	LengthY TINYINT NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT chk_LengthX CHECK (LengthX >= 1 AND LengthX <= 12),
    CONSTRAINT chk_LengthY CHECK (LengthY >= 1 AND LengthY <= 12)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


-- Creating Sprite table
CREATE TABLE Sprite(
	Id SMALLINT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(50) NOT NULL,
    IsAddOn BOOL NOT NULL,
    Price SMALLINT NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT chk_Price CHECK (Price >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;



-- Creating PurchasedSprites table
CREATE TABLE PurchasedSprite(
	Id SMALLINT NOT NULL AUTO_INCREMENT,
    PlayerId SMALLINT NOT NULL,
    SpriteId SMALLINT NOT NULL,

    PRIMARY KEY (Id),
	CONSTRAINT fk_PurchasedSprite_Player FOREIGN KEY (PlayerId) REFERENCES Player(Id),
    CONSTRAINT fk_PurchasedSprite_Sprite FOREIGN KEY (SpriteId) REFERENCES Sprite(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;



-- Creating Quality table
CREATE TABLE Quality(
	Id TINYINT NOT NULL AUTO_INCREMENT,
    QualityName VARCHAR(20) NOT NULL,
    PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;



-- Creating Card table
CREATE TABLE Card(
	Id SMALLINT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(50) NOT NULL,
    
    QualityId TINYINT NOT NULL,
    CardTypeId TINYINT NOT NULL,
    AreaId TINYINT NOT NULL,
    SkinId SMALLINT,
    EffectId SMALLINT,
    
    PRIMARY KEY (Id),
    CONSTRAINT fk_Card_Quality FOREIGN KEY (QualityId) REFERENCES Quality(Id),
    CONSTRAINT fk_Card_CardType FOREIGN KEY (CardTypeId) REFERENCES CardType(Id),
    CONSTRAINT fk_Card_Area FOREIGN KEY (AreaId) REFERENCES Area(Id),
    CONSTRAINT fk_CardSkin_Sprite FOREIGN KEY (SkinId) REFERENCES Sprite(Id),
    CONSTRAINT fk_CardEffect_Sprite FOREIGN KEY (EffectId) REFERENCES Sprite(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


-- Creating Arena table
CREATE TABLE Arena(
	Id SMALLINT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(50) NOT NULL,
    Level TINYINT NOT NULL,
    MatchesRequired SMALLINT NOT NULL,
    MusicIdUnity SMALLINT,
    SpriteId SMALLINT,
    
    PRIMARY KEY (Id),
    CONSTRAINT fk_Arena_Sprite FOREIGN KEY (SpriteId) REFERENCES Sprite(Id),
    CONSTRAINT chk_Level CHECK (Level >= 1),
    CONSTRAINT chk_MatchesRequired CHECK (MatchesRequired >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;



-- Creating Game table
CREATE TABLE Game(
	Id SMALLINT NOT NULL AUTO_INCREMENT,
    Date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    IsPlayerWon BOOL NOT NULL,
    PlayerId SMALLINT NOT NULL,
    ArenaId SMALLINT NOT NULL,
    
    PRIMARY KEY (Id),
    CONSTRAINT fk_Game_Player FOREIGN KEY (PlayerId) REFERENCES Player(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;



-- Creating Play table
CREATE TABLE Play(
	Id SMALLINT NOT NULL AUTO_INCREMENT,
    
    PlayNumber MEDIUMINT NOT NULL,
    IsPlayerPlay BOOL NOT NULL,
    NumFieldsCovered TINYINT NOT NULL,

    GameId SMALLINT NOT NULL,
    CardPlayedId SMALLINT NOT NULL,
    
    PRIMARY KEY (Id),
    CONSTRAINT fk_Play_Game FOREIGN KEY (GameId) REFERENCES Game(Id),
    CONSTRAINT fk_Play_Card FOREIGN KEY (CardPlayedId) REFERENCES Card(Id),
    CONSTRAINT chk_PlayNumber CHECK (PlayNumber >= 1),
    CONSTRAINT chk_NumFieldsCovered CHECK (NumFieldsCovered >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;




