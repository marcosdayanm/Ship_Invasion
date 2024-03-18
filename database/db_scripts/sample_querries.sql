-- Sample querries to views

SELECT * FROM view_carddetails WHERE CardQuality = 'Gold';
SELECT * FROM view_carddetails WHERE CardType = 'Defense';
SELECT * FROM view_carddetails WHERE (LengthX > 1 AND LengthY > 1);

SELECT * FROM view_playerdetails WHERE PlayerId = 3;
SELECT * FROM view_playerdetails WHERE PlayerCreationDate > '2024-02-18';

