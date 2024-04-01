USE ship_invasion;


-- View for all the card details
CREATE OR REPLACE VIEW view_carddetails AS
SELECT 
    c.Id AS CardId,
    c.Name AS CardName,
    ct.TypeName AS CardType,
    q.QualityName AS CardQuality,
    a.LengthX AS LengthX,
    a.LengthY AS LengthY
FROM Card c
JOIN CardType ct ON c.CardTypeId = ct.Id
JOIN Quality q ON c.QualityId = q.Id
JOIN Area a ON c.AreaId = a.Id
LEFT JOIN Sprite s1 ON c.SkinId = s1.Id
LEFT JOIN Sprite s2 ON c.EffectId = s2.Id;

-- Validación
SELECT * FROM view_carddetails;




-- View for all the player details
CREATE OR REPLACE VIEW view_playerdetails AS
SELECT 
    u.Id AS PlayerId,
    u.Username AS PlayerUsername,
    u.CreationDate AS PlayerCreationDate,
    u.Wins AS PlayerWins,
    u.Losses AS PlayerLosses,
    u.Coins AS PlayerCoins,
    ps.PlayerId AS PurchasedSpritePlayerId,
    ps.SpriteId AS PurchasedSpriteSpriteId,
    s.Id AS SpriteId,
    s.IsAddOn AS SpriteIsAddOn,
    s.Price AS SpritePrice
FROM Player u
LEFT JOIN PurchasedSprite ps ON u.Id = ps.PlayerId
LEFT JOIN Sprite s ON s.Id = ps.SpriteId;

-- Validación
SELECT * FROM view_playerdetails;




-- View for all the game details
CREATE OR REPLACE VIEW view_gamedetails AS
SELECT 
    g.Id AS GameId,
    g.Date AS GameDate,
    g.IsPlayerWon AS GameIsPlayerWonGame,
    
    u.Id AS PlayerId,
    u.Username AS PlayerUsername,
    u.Wins AS PlayerWins,
    u.Losses AS PlayerLosses,
    u.Coins AS PlayerCoins,
    
    p.Id AS PlayId,
    p.PlayNumber AS PlayNumber,
    p.IsPlayerPlay AS PlayIsPlayerPlay,
    p.NumFieldsCovered AS PlayNumFieldsCovered,
    
    a.Id AS ArenaId,
    a.Name AS ArenaName,
    a.Level AS ArenaLevel,
    a.MatchesRequired AS ArenaMatchesRequired
    
    FROM Game g
    LEFT JOIN Player u ON g.PlayerId = u.Id
    LEFT JOIN Play p ON g.Id = p.GameId
    LEFT JOIN Arena a ON a.Id = g.ArenaId;


-- Validación
SELECT * FROM view_gamedetails;


