-- Sample querries to views

use ship_invasion;

SELECT * FROM view_carddetails WHERE CardQuality = 'Gold';
SELECT * FROM view_carddetails WHERE CardType = 'Defense';
SELECT * FROM view_carddetails WHERE (LengthX > 1 AND LengthY > 1);

SELECT * FROM view_playerdetails WHERE PlayerId = 3;
SELECT * FROM view_playerdetails WHERE PlayerCreationDate > '2024-02-18';

UPDATE Player SET Wins=15 WHERE Id=1;
UPDATE Player SET Wins=22 WHERE Id=2;
UPDATE Player SET Wins=19 WHERE Id=3;

UPDATE Player SET Losses=4 WHERE Id=1;
UPDATE Player SET Losses=10 WHERE Id=2;
UPDATE Player SET Losses=9 WHERE Id=3;

SELECT * FROM cartas WHERE language = "Ingles" LIMIT 5;