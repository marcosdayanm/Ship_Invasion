import express from "express";
import mysql from "mysql2/promise";
import dotenv from "dotenv/config";
import fs from "fs";
import { marked } from "marked";
import { comparePassword, hashPassword } from "./utils/hashPassword.js";

const PORT = process.env.PORT || 3000;

// Create an express application
const app = express();

// Middleware para configurar CORS
app.use((req, res, next) => {
  res.header("Access-Control-Allow-Origin", "*");
  res.header(
    "Access-Control-Allow-Headers",
    "Origin, X-Requested-With, Content-Type, Accept"
  );
  next();
});

// Enable JSON parsing
app.use(express.json());

app.use(express.static("./public"));

// Function to connect to the database
async function connectToDB() {
  return await mysql.createConnection({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME,
    port: process.env.DB_PORT,
  });
}

//Endopoints para pagina web
app.get("/", (req, res) => {
  fs.readFile("./public/play.html", "utf8", (err, html) => {
    if (err) res.status(500).send("There was an error: " + err);
    console.log("Loading page...");
    res.send(html);
  });
});

app.get("/stats", (req, res) => {
  fs.readFile("./public/stats.html", "utf8", (err, html) => {
    if (err) res.status(500).send("There was an error: " + err);
    console.log("Loading page...");
    res.send(html);
  });
});

app.get("/gdd", (req, res) => {
  fs.readFile("./public/gdd.html", "utf8", (err, md) => {
    if (err) res.status(500).send("There was an error: " + err);
    console.log("Loading page...");
    const html = marked(md);
    res.send(html);
  });
});

// Get all cards with their details
app.get("/api/cards", async (req, res) => {
  let connection = null;
  try {
    connection = await connectToDB();
    const [cards] = await connection.execute("SELECT * FROM view_carddetails");
    if (cards.length === 0) {
      return res.status(404).json({ error: "No cards found" });
    }
    // console.log(cards);
    res.status(200).json({ Items: cards });
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }
});

app.get("/api/arenas", async (req, res) => {
  let connection = null;
  try {
    connection = await connectToDB();
    const [arenas] = await connection.execute("SELECT * FROM Arena");
    // console.log(cards);
    res.status(200).json({ arenas });
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }
});

app.get("/api/games/total-games-by-arena", async (req, res) => {
  let connection = null;
  const { cardId } = req.params;
  try {
    connection = await connectToDB();
    const [rows] = await connection.execute(
      "SELECT ArenaName, COUNT(*) AS TotalGames FROM view_gamedetails GROUP BY ArenaId ORDER BY TotalGames DESC"
    );
    if (rows.length === 0) {
      return res.status(404).json({ error: "Data not found" });
    }
    res.status(200).json(rows);
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }
});

app.get("/api/games/total-games-today", async (req, res) => {
  let connection = null;
  const { cardId } = req.params;
  try {
    let yesterdayDate = new Date();
    yesterdayDate.setDate(yesterdayDate.getDate() - 1);
    let formattedDate = yesterdayDate.toISOString().split("T")[0];
    connection = await connectToDB();
    const [rows] = await connection.execute(
      "SELECT COUNT(*) AS TotalGames FROM view_gamedetails WHERE GameDate >= DATE_SUB(NOW(), INTERVAL 1 DAY);"
    );
    if (rows.length === 0) {
      return res.status(404).json({ error: "Data not found" });
    }
    res.status(200).json(rows[0]);
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }
});

app.get("/api/games/average-games-per-day", async (req, res) => {
  let connection = null;
  const { cardId } = req.params;
  try {
    let yesterdayDate = new Date();
    yesterdayDate.setDate(yesterdayDate.getDate() - 1);
    let formattedDate = yesterdayDate.toISOString().split("T")[0];
    connection = await connectToDB();
    const [rows] = await connection.execute(
      "SELECT AVG(GamesPerDay) AS AverageGamesPerDay FROM (SELECT DATE(GameDate) AS Day, COUNT(*) AS GamesPerDay FROM view_gamedetails GROUP BY DATE(GameDate)) AS DailyGames;"
    );
    if (rows.length === 0) {
      return res.status(404).json({ error: "Data not found" });
    }
    res.status(200).json(rows[0]);
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }
});

app.get("/api/plays/average", async (req, res) => {
  let connection = null;
  try {
    connection = await connectToDB();
    const [number] = await connection.execute(
      "SELECT AVG(PlayCount) AS PlayAverage FROM (SELECT GameId, COUNT(*) AS PlayCount FROM view_playdetails GROUP BY GameId) AS PlayCounts;"
    );
    if (number.length === 0) {
      return res.status(404).json({ error: "No number found" });
    }
    res.status(200).json({ number: number[0]?.PlayAverage });
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }
});

// Get all cards with their details
app.get("/api/cards/top5mostusedcards", async (req, res) => {
  let connection = null;
  try {
    connection = await connectToDB();
    const [cards] = await connection.execute(
      "SELECT CardId, CardName, CardType, CardQuality, COUNT(*) AS NumberOfPlays FROM view_playdetails GROUP BY CardId, CardName ORDER BY NumberOfPlays DESC LIMIT 5"
    );
    if (cards.length === 0) {
      return res.status(404).json({ error: "No cards found" });
    }
    // console.log(cards);
    res.status(200).json({ cards });
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }
});

app.get("/api/plays/playsrecord", async (req, res) => {
  let connection = null;
  try {
    connection = await connectToDB();
    const [number] = await connection.execute(
      "SELECT COUNT(*) AS NumberOfPlays FROM view_playdetails GROUP BY GameId ORDER BY NumberOfPlays DESC LIMIT 1"
    );
    if (number.length === 0) {
      return res.status(404).json({ error: "No number found" });
    }
    res.status(200).json({ number: number[0] });
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }
});

// Get one card with its details
app.get("/api/cards/:cardId", async (req, res) => {
  let connection = null;
  const { cardId } = req.params;
  try {
    connection = await connectToDB();
    const [rows] = await connection.execute(
      "SELECT * FROM view_carddetails WHERE CardId = ?",
      [cardId]
    );
    if (rows.length === 0) {
      return res.status(404).json({ error: "Card not found" });
    }
    res.status(200).json(rows[0]);
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }
});

// Route to manage one player (get one player, update one player, delete one player)
app
  .route("/api/players/:playerId")
  // Get one player with its details
  .get(async (req, res) => {
    let connection = null;
    const { playerId } = req.params;
    try {
      connection = await connectToDB();
      const [rows] = await connection.execute(
        "SELECT * FROM view_playerdetails WHERE PlayerId = ?",
        [playerId]
      );
      if (rows.length === 0) {
        return res.status(404).json({ error: "Player not found" });
      }
      res.status(200).json(rows[0]);
    } catch (error) {
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  })
  .put(async (req, res) => {
    let connection = null;
    const { playerId } = req.params;
    const [playerExists] = await connection.execute(
      "SELECT * FROM Player WHERE id = ?",
      [playerId]
    );
    if (playerExists.length === 0) {
      return res.status(404).json({ error: "Player not found" });
    }
    const keys = Object.keys(req.body);
    const changableFields = ["coins", "wins", "losses"];
    if (keys.some((key) => !changableFields.includes(key))) {
      return res.status(400).json({ error: "Invalid request" });
    }
    let query = "UPDATE Player SET";
    const values = [];
    keys.forEach((key, index) => {
      query += ` ${key} = ?`;
      values.push(req.body[key]);
      if (index < keys.length - 1) {
        query += ",";
      }
    });
    query += " WHERE id = ?";
    try {
      connection = await connectToDB();
      await connection.execute(query, [...values, playerId]);
      res.status(200).json({ msg: "Player updated successfully" });
    } catch (error) {
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  })
  // TODO: Delete one player (and all its games, plays, etc)
  .delete(async (req, res) => {
    let connection = null;
    const { playerId } = req.params;
    try {
      connection = await connectToDB();
      const [playerExists] = await connection.execute(
        "SELECT * FROM Player WHERE id = ?",
        [playerId]
      );
      if (playerExists.length === 0) {
        return res.status(404).json({ error: "Player not found" });
      }
      await connection.execute("DELETE FROM Player WHERE id = ?", [playerId]);
      res.status(200).json({ msg: "Player deleted successfully" });
    } catch (error) {
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  });

// route to get a player by is id and password by a post, for login purposes
app.route("/api/players/login").post(async (req, res) => {
  let connection = null;
  const { username, password } = req.body;
  if (!username || !password) {
    return res.status(400).json({ error: "Invalid request" });
  }
  try {
    connection = await connectToDB();
    const [rows] = await connection.execute(
      "SELECT * FROM Player WHERE Username = ?",
      [username]
    );
    const validPassword = await comparePassword(password, rows[0].Password);
    if (!validPassword) {
      return res.status(400).json({ error: "Credenciales incorrectas" });
    }
    if (rows.length === 0) {
      console.log("credencuiales incorrectas (API)");
      return res
        .status(200)
        .json({ error: "Username or/and password incorrect" });
    } else {
      const [player] = await connection.execute(
        "SELECT * FROM view_playerdetails WHERE PlayerUsername = ?",
        [username]
      );
      console.log(player);
      res.status(200).json(player[0]);
    }
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }
});

// Route to manage players (add new player or get all players)
app
  .route("/api/players")
  // Get all players
  .get(async (req, res) => {
    let connection = null;
    try {
      connection = await connectToDB();
      const [rows] = await connection.execute(
        "SELECT * FROM view_playerdetails"
      );
      res.status(200).json(rows);
    } catch (error) {
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  })
  // Save new player
  .post(async (req, res) => {
    let connection = null;
    const { username, password } = req.body;
    if (!username || !password) {
      return res.status(400).json({ error: "Invalid request" });
    }
    try {
      connection = await connectToDB();
      const [usernameExists] = await connection.execute(
        "SELECT * FROM Player WHERE Username = ?",
        [username]
      );
      if (usernameExists.length > 0) {
        return res.status(400).json({ error: "Username already exists" });
      }
      const hashedPassword = await hashPassword(password);
      const [result] = await connection.execute(
        "INSERT INTO Player (Username, Password) VALUES (?, ?)",
        [username, hashedPassword]
      );
      res.status(201).json({
        msg: "Player created successfully",
        PlayerId: result.insertId,
      });
    } catch (error) {
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  });

app.route("/api/player/edit-data").post(async (req, res) => {
  const { PlayerId, PlayerCoins, PlayerWins, PlayerLosses } = req.body;
  if (!PlayerId || !PlayerCoins) {
    return res.status(400).json({ error: "Invalid request" });
  }
  let connection = null;
  try {
    connection = await connectToDB();
    const [rows] = await connection.execute(
      `SELECT * FROM view_playerdetails WHERE PlayerId = ${PlayerId}`
    );
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }

  try {
    connection = await connectToDB();
    const [result] = await connection.execute(
      "UPDATE Player SET Coins = ?, Wins = ?, Losses = ? WHERE Id = ?",
      [PlayerCoins, PlayerWins, PlayerLosses, PlayerId]
    );
    if (result.affectedRows > 0) {
      res
        .status(200)
        .json({ success: true, message: "Player updated successfully" });
    } else {
      res
        .status(404)
        .json({ success: false, message: "No player found with the given ID" });
    }
    res.status(201).json({
      PlayerId: result.insertId,
    });
  } catch (error) {
    res.status(500).json({ error: error.message });
  } finally {
    if (connection) connection.end();
  }
});

// Route to manage all games of a player
app
  .route("/api/games")
  // Get all games of a player with their details
  .get(async (req, res) => {
    let connection = null;
    try {
      connection = await connectToDB();
      const [rows] = await connection.execute("SELECT * FROM view_gamedetails");
      if (rows.length === 0) {
        return res.status(404).json({ error: "No games found" });
      }
      res.status(200).json(rows);
    } catch (error) {
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  })
  // Save new game
  .post(async (req, res) => {
    let connection = null;
    const { IsPlayerWon, PlayerId, ArenaId } = req.body;
    if (!IsPlayerWon || !PlayerId || !ArenaId) {
      return res.status(400).json({ error: "Invalid request" });
    }
    try {
      connection = await connectToDB();
      const [result] = await connection.execute(
        "INSERT INTO Game (IsPlayerWon, PlayerId, ArenaId) VALUES (?, ?, ?)",
        [IsPlayerWon, PlayerId, ArenaId]
      );
      res.status(201).json({
        GameId: result.insertId,
      });
    } catch (error) {
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  });

// Route to manage one game with all its plays
app
  .route("/api/games/:GameId")
  // Get one game with all its plays
  .get(async (req, res) => {
    let connection = null;
    const { GameId } = req.params;

    try {
      connection = await connectToDB();
      const [currgame] = await connection.execute(
        "SELECT * FROM view_gamedetails WHERE GameId = ?",
        [GameId]
      );
      if (currgame.length === 0) {
        return res.status(404).json({ error: "Game not found" });
      }
      const [Plays] = await connection.execute(
        "SELECT * FROM view_playdetails WHERE GameId = ?",
        [GameId]
      );
      let Game = currgame[0];
      res.status(200).json({ Game, Plays });
    } catch (error) {
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  })

  // Updates IsPlayerWon of a game if the player won the game
  .post(async (req, res) => {
    let connection = null;
    const { GameId } = req.params;
    const { IsPlayerWon } = req.body;

    if (!IsPlayerWon) {
      return res.status(400).json({ error: "Invalid request" });
    }
    try {
      connection = await connectToDB();
      const [result] = await connection.execute(
        "UPDATE Game SET IsPlayerWon = ? WHERE Id = ?",
        [IsPlayerWon, GameId]
      );
      res.status(201).json({
        msg: "IsPlayerWon updated successfully",
        GameId: GameId,
      });
    } catch (error) {
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  });

// Route to manage all games of a player
app
  .route("/api/plays")
  .get(async (req, res) => {
    let connection = null;
    try {
      connection = await connectToDB();
      const [rows] = await connection.execute("SELECT * FROM view_playdetails");
      if (rows.length === 0) {
        return res.status(404).json({ error: "No plays found" });
      }
      res.status(200).json(rows);
    } catch (error) {
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  })
  // Save new play
  .post(async (req, res) => {
    let connection = null;
    const { PlayNumber, IsPlayerPlay, NumFieldsCovered, GameId, CardPlayedId } =
      req.body;
    if (
      !PlayNumber ||
      !IsPlayerPlay ||
      !NumFieldsCovered ||
      !GameId ||
      !CardPlayedId
    ) {
      return res.status(400).json({ error: "Invalid request" });
    }
    try {
      connection = await connectToDB();
      const [result] = await connection.execute(
        "INSERT INTO Play (PlayNumber, IsPlayerPlay, NumFieldsCovered, GameId, CardPlayedId) VALUES (?, ?, ?, ?, ?)",
        [PlayNumber, IsPlayerPlay, NumFieldsCovered, GameId, CardPlayedId]
      );
      res.status(201).json({
        msg: "Play created successfully",
        PlayId: result.insertId,
      });
    } catch (error) {
      console.log(error);
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  });

// TODO:
// - Add a new endpoint to authenticate a player (login)

// - Add a new endpoint to save a new sprite purchase of a player
// - Add a new endpoint to get all sprites of a player

// Endpoints overview
// GET /api/cards -> Get all cards with their details
// GET /api/cards/:id -> Get one card with its details
// GET /api/players/:id -> Get one player with its details
// PUT /api/players/:id -> Update one player info
// DELETE /api/players/:id -> Delete one player
// GET /api/players -> Get all players
// POST /api/players -> Save new player
// GET /api/games -> Get all games of a player with their details
// POST /api/games -> Save new game
// GET /api/games/:id -> Get a game with all its plays
// POST /api/games/:id -> Updates IsPlayerWon of a game if the player won the game
// POST /api/play -> Save a new play

// Run the server
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
