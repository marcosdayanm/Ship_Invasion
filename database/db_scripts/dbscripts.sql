USE ship_invasion;


-- Updating Arena table adding Cost column and adding data
ALTER TABLE Arena ADD Cost INT;

UPDATE Arena SET Cost=0 WHERE Id=1;
UPDATE Arena SET Cost=10 WHERE Id=2;
UPDATE Arena SET Cost=20 WHERE Id=3;
UPDATE Arena SET Cost=50 WHERE Id=4;


