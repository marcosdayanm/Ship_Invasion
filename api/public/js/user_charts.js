/**
 * @param {number} alpha Indicated the transparency of the color
 * @returns {string} A string of the form 'rgba(240, 50, 123, 1.0)' that represents a color
 */
function random_color(alpha = 1.0) {
  const r_c = () => Math.round(Math.random() * 255);
  return `rgba(${r_c()}, ${r_c()}, ${r_c()}, ${alpha}`;
}

Chart.defaults.font.size = 16;

// async function getPlaysByGame(gameId) {
//   try {
//     const playsByGame = await fetch(
//       `https://ship-invasion.onrender.com/api/games/${gameId}`,
//       {
//         method: "GET",
//       }
//     );
//     return playsByGame.json();
//   } catch (error) {
//     console.log(error);
//   }
// }

async function getGames(gameId) {
  try {
    const games = await fetch("https://ship-invasion.onrender.com/api/games", {
      method: "GET",
    });
    return games.json();
  } catch (error) {
    console.log(error);
  }
}

async function getPlayers(gameId) {
  try {
    const players = await fetch(
      "https://ship-invasion.onrender.com/api/players",
      {
        method: "GET",
      }
    );
    return players.json();
  } catch (error) {
    console.log(error);
  }
}

async function getPlays(gameId) {
  try {
    const plays = await fetch("https://ship-invasion.onrender.com/api/plays", {
      method: "GET",
    });
    return plays.json();
  } catch (error) {
    console.log(error);
  }
}

async function getTotalGamesOnEachArena(gameId) {
  try {
    const total = await fetch(
      "https://ship-invasion.onrender.com/api/games/total-games-by-arena",
      {
        method: "GET",
      }
    );
    return total.json();
  } catch (error) {
    console.log(error);
  }
}

async function getTop5MostUsedCards(gameId) {
  try {
    const cards = await fetch(
      "https://ship-invasion.onrender.com/api/cards/top5mostusedcards",
      {
        method: "GET",
      }
    );
    return cards.json();
  } catch (error) {
    console.log(error);
  }
}

async function getPlaysRecord(gameId) {
  try {
    const total = await fetch(
      "https://ship-invasion.onrender.com/api/plays/playsrecord",
      {
        method: "GET",
      }
    );
    return total.json();
  } catch (error) {
    console.log(error);
  }
}

async function logIn(username, password) {
  try {
    const player = await fetch(
      "https://ship-invasion.onrender.com/api/players",
      {
        method: "POST",
        body: JSON.stringify({ username: username, password: password }),
      }
    );
    return player.json();
  } catch (error) {
    console.log(error);
  }
}

async function fetchData() {
  try {
    const [players, games, plays, top5cards, totalGames, totalPlaysRecord] =
      await Promise.all([
        getPlayers(),
        getGames(),
        getPlays(),
        getTop5MostUsedCards(),
        getTotalGamesOnEachArena(),
        getPlaysRecord(),
      ]);
    console.log({
      players,
      games,
      plays,
      top5cards,
      totalGames,
      totalPlaysRecord,
    });
    return [
      players,
      games,
      plays,
      top5cards.cards,
      totalGames,
      totalPlaysRecord,
    ];
  } catch (error) {
    console.log("Error fetching data:", error);
  }
}

try {
  // fetch simultaneo con función que usa promise para más eficiencia y no esperar a que cada fetch termine
  const [players, games, plays, top5cards, totalGames, totalPlaysRecord] =
    await fetchData();

  const players_colors = players.map((e) => random_color(0.8));
  const players_borders = players.map((e) => "rgba(0, 0, 0, 1.0)");
  const players_names = players.map((e) => e["PlayerUsername"]);
  const players_wins = players.map((e) => e["PlayerWins"]);
  const players_losses = players.map((e) => e["PlayerLosses"]);
  const players_coins = players.map((e) => e["PlayerCoins"]);

  console.log(players_wins);
  console.log(players_losses);

  const playersRatio = players.map(
    (e) => e["PlayerWins"] / (e["PlayerWins"] + e["PlayerLosses"])
  );

  console.log(playersRatio);

  const totalPlayersWins = players_wins.reduce((ac, e) => ac + e, 0);
  const totalPlayersLosses = players_losses.reduce((ac, e) => ac + e, 0);

  // Average win ratio
  const playersAverageWinRatio = (
    totalPlayersWins /
    (totalPlayersWins + totalPlayersLosses)
  ).toFixed(2);
  console.log(playersAverageWinRatio);

  document.getElementById("winLossRatio").textContent = playersAverageWinRatio;

  const totalGamesPlayed = totalGames.reduce(
    (ac, e) => ac + e["TotalGames"],
    0
  );

  document.getElementById("totalGamesPlayed").textContent = totalGamesPlayed;

  const totalPlayers = players.length;

  document.getElementById("totalPlayers").textContent = totalPlayers;

  document.getElementById("totalPlaysRecord").textContent =
    totalPlaysRecord.number.NumberOfPlays;

  // Average Coins Amount per player
  const playerAverageCoins = players_coins / players.length;

  // Pie chart that shows Wins per player
  const winsChart = document
    .getElementById("winsAmountPerPlayer")
    .getContext("2d");
  const levelChart1 = new Chart(winsChart, {
    type: "pie",
    data: {
      labels: players_names,
      datasets: [
        {
          label: "Wins",
          backgroundColor: players_colors,
          borderColor: players_borders,
          data: players_wins,
        },
      ],
    },
  });

  console.log(totalGames);

  const total_games_colors = totalGames.map((e) => random_color(0.8));
  const total_games_borders = totalGames.map((e) => "rgba(0, 0, 0, 1.0)");
  const total_games_arena = totalGames.map((e) => e["ArenaName"]);
  const total_games_number = totalGames.map((e) => e["TotalGames"]);

  const ctx_levels2 = document
    .getElementById("gamesPlayedOnEachArena")
    .getContext("2d");
  const levelChart2 = new Chart(ctx_levels2, {
    type: "line",
    data: {
      labels: total_games_arena,
      datasets: [
        {
          label: "Total de juegos por arena",
          backgroundColor: total_games_colors,
          pointRadius: 10,
          data: total_games_number,
        },
      ],
    },
  });

  const top5cards_colors = top5cards.map((e) => random_color(0.8));
  const top5cards_borders = top5cards.map((e) => "rgba(0, 0, 0, 1.0)");
  const top5cards_cardname = top5cards.map((e) => e["CardName"]);
  const top5cards_cardtype = top5cards.map((e) => e["CardType"]);
  const top5cards_cardquality = top5cards.map((e) => e["CardQuality"]);
  const top5cards_count = top5cards.map((e) => e["NumberOfPlays"]);

  // console.log(top5cards_cardname);
  // console.log(top5cards_count);

  const ctx_levels3 = document.getElementById("mostUsedCards").getContext("2d");
  const levelChart3 = new Chart(ctx_levels3, {
    type: "bar",
    data: {
      labels: top5cards_cardname,
      datasets: [
        {
          label: "Top 5 cartas más usadas",
          backgroundColor: top5cards_colors,
          borderColor: top5cards_borders,
          borderWidth: 2,
          data: top5cards_count,
        },
      ],
    },
  });
} catch (error) {
  console.log(`Error desconocido: ${error}`);
}
