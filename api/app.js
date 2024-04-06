import express from "express";
import mysql from "mysql2/promise";
import dotenv from "dotenv/config";

import { hashPassword } from "./utils/hashPassword.js";

const PORT = process.env.PORT || 3000;

// Create an express application
const app = express();

// Enable JSON parsing
app.use(express.json());

// Function to connect to the database
async function connectToDB() {
  return await mysql.createConnection({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME,
  });
}

// Get all cards with their details
app.get("/api/cards", async (req, res) => {
  let connection = null;
  try {
    connection = await connectToDB();
    const [cards] = await connection.execute("SELECT * FROM view_carddetails");
    if (cards.length === 0) {
      return res.status(404).json({ error: "No cards found" });
    }
    res.status(200).json({Items: cards});
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
    const [playerExists] = await connection.execute("SELECT * FROM Player WHERE id = ?", [playerId]);
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
      const [playerExists] = await connection.execute("SELECT * FROM Player WHERE id = ?", [playerId]);
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

// Route to manage all games of a player
app
  .route("/api/games")
  // Get all games of a player with their details
  .get(async (req, res) => {
    let connection = null;
    const { playerId } = req.query;
    if (!playerId) {
      return res.status(400).json({ error: "Invalid request" });
    }
    try {
      connection = await connectToDB();
      const [rows] = await connection.execute(
        "SELECT * FROM view_gamedetails WHERE PlayerId = ?",
        [playerId]
      );
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
    const { isPlayerWon, playerId, arenaId } = req.body;
    if (!isPlayerWon || !playerId || !arenaId) {
      return res.status(400).json({ error: "Invalid request" });
    }
    try {
      connection = await connectToDB();
      const [result] = await connection.execute(
        "INSERT INTO Game (IsPlayerWon, PlayerId, ArenaId) VALUES (?, ?, ?)",
        [isPlayerWon, playerId, arenaId]
      );
      res.status(201).json({
        msg: "Game created successfully",
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
  .route("/api/games/:gameId")
  // Get one game with all its plays
  .get(async (req, res) => {
    let connection = null;
    const { gameId } = req.params;

    try {
      connection = await connectToDB();
      const [currgame] = await connection.execute(
        "SELECT * FROM view_gamedetails WHERE GameId = ?",
        [gameId]
      );
      if (currgame.length === 0) {
        return res.status(404).json({ error: "Game not found" });
      }
      const [Plays] = await connection.execute(
        "SELECT * FROM view_playdetails WHERE GameId = ?",
        [gameId]
      );
      let Game = currgame[0];
      res.status(200).json({ Game, Plays });
    } catch (error) {
      res.status(500).json({ error: error.message });
    } finally {
      if (connection) connection.end();
    }
  });

// Route to manage all games of a player
app
  .route("/api/plays")
  // Save new play
  .post(async (req, res) => {
    let connection = null;
    const { playNumber, isPlayerPlay, numFieldsCovered, gameId, cardPlayedId } =
      req.body;
    if (
      !playNumber ||
      !isPlayerPlay ||
      !numFieldsCovered ||
      !gameId ||
      !cardPlayedId
    ) {
      return res.status(400).json({ error: "Invalid request" });
    }
    try {
      connection = await connectToDB();
      const [result] = await connection.execute(
        "INSERT INTO Play (PlayNumber, IsPlayerPlay, NumFieldsCovered, GameId, CardPlayedId) VALUES (?, ?, ?, ?, ?)",
        [playNumber, isPlayerPlay, numFieldsCovered, gameId, cardPlayedId]
      );
      res.status(201).json({
        msg: "Game created successfully",
        GameId: result.insertId,
      });
    } catch (error) {
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
// POST /api/play -> Save a new play

// Run the server
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
