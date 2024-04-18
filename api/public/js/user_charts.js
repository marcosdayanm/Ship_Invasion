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
//       `http://localhost:3000/api/games/${gameId}`,
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
    const games = await fetch("http://localhost:3000/api/games", {
      method: "GET",
    });
    return games.json();
  } catch (error) {
    console.log(error);
  }
}

async function getPlayers(gameId) {
  try {
    const players = await fetch("http://localhost:3000/api/players", {
      method: "GET",
    });
    return players.json();
  } catch (error) {
    console.log(error);
  }
}

async function getPlays(gameId) {
  try {
    const plays = await fetch("http://localhost:3000/api/plays", {
      method: "GET",
    });
    return plays.json();
  } catch (error) {
    console.log(error);
  }
}

async function getTop5MostUsedCards(gameId) {
  try {
    const cards = await fetch(
      "http://localhost:3000/api/cards/top5mostusedcards",
      {
        method: "GET",
      }
    );
    return cards.json();
  } catch (error) {
    console.log(error);
  }
}

async function logIn(username, password) {
  try {
    const player = await fetch("http://localhost:3000/api/players", {
      method: "POST",
      body: JSON.stringify({ username: username, password: password }),
    });
    return player.json();
  } catch (error) {
    console.log(error);
  }
}

// We obtain a reference to the canvas that we are going to use to plot the chart.
// const ctx = document.getElementById("firstChart").getContext("2d");

// To plot a chart, we need a configuration object that has all the information that the chart needs.
// const firstChart = new Chart(ctx, {
//   type: "bar",
//   data: {
//     labels: ["Red", "Blue", "Yellow", "Green", "Purple", "Orange"],
//     datasets: [
//       {
//         label: "# of Votes",
//         data: [12, 19, 3, 5, 2, 3],
//         backgroundColor: [
//           "rgba(255, 99, 132, 0.2)",
//           "rgba(54, 162, 235, 0.2)",
//           "rgba(255, 206, 86, 0.2)",
//           "rgba(75, 192, 192, 0.2)",
//           "rgba(153, 102, 255, 0.2)",
//           "rgba(255, 159, 64, 0.2)",
//         ],
//         borderColor: [
//           "rgba(255, 99, 132, 1)",
//           "rgba(54, 162, 235, 1)",
//           "rgba(255, 206, 86, 1)",
//           "rgba(75, 192, 192, 1)",
//           "rgba(153, 102, 255, 1)",
//           "rgba(255, 159, 64, 1)",
//         ],
//         borderWidth: 1,
//       },
//     ],
//   },
//   options: {
//     scales: {
//       y: {
//         beginAtZero: true,
//       },
//     },
//   },
// });

// To plot data from an API, we first need to fetch a request, and then process the data.
try {
  // Fetching API's data
  const players = await getPlayers();
  const games = await getGames();
  // const playsByGame = await getPlaysByGame(games[0].id);
  const plays = await getPlays();
  const top5cards = await getTop5MostUsedCards();

  games.forEach(async (game) => {
    const plays = await getPlays(game.id);
    game["plays"] = plays;
  });

  console.log(players);
  console.log(games);
  // console.log(playsByGame);
  console.log(plays);
  console.log(top5cards);

  // In this case, we just separate the data into different arrays using the map method of the values array. This creates new arrays that hold only the data that we need.
  const players_colors = players.map((e) => random_color(0.8));
  const players_borders = players.map((e) => "rgba(0, 0, 0, 1.0)");
  const players_names = players.map((e) => e["PlayerUsername"]);
  const players_wins = players.map((e) => e["PlayerWins"]);
  const players_losses = players.map((e) => e["PlayerLosses"]);
  const players_coins = players.map((e) => e["PlayerCoins"]);

  // Average win ratio
  const playersAverageWinRatio = players_wins / (players_wins + players_losses);

  // Average Coins Amount per player
  const playerAverageCoins = players_coins / players.length;

  // Pie chart that shows Wins per player
  const winsChart = document
    .getElementById("winsAmountPerPlayer")
    .getContext("2d");
  const levelChart1 = new Chart(winsChart, {
    type: "pie",
    data: {
      labels: "Wins",
      datasets: [
        {
          label: players_names,
          backgroundColor: players_colors,
          borderColor: players_borders,
          data: players_wins,
        },
      ],
    },
  });

  const games_colors = games.map((e) => random_color(0.8));
  const games_borders = games.map((e) => "rgba(0, 0, 0, 1.0)");
  const games_arena = games.map((e) => e["ArenaName"]);
  const games_dates = games.map((e) => e["GameDate"]);

  const totalGamesPlayedOnEachArena = [
    games_arena.filter((e) => e === 1).length,
    games_arena.filter((e) => e === 2).length,
    games_arena.filter((e) => e === 3).length,
    games_arena.filter((e) => e === 4).length,
  ];

  const ctx_levels2 = document
    .getElementById("gamesPlayedOnEachArena")
    .getContext("2d");
  const levelChart2 = new Chart(ctx_levels2, {
    type: "line",
    data: {
      labels: level_names,
      datasets: [
        {
          label: [
            "Mar Abierto",
            "Tormenta Eléctrica",
            "Río de Fuego",
            "Pantano Tóxico",
          ],
          backgroundColor: games_colors,
          pointRadius: 10,
          data: totalGamesPlayedOnEachArena,
        },
      ],
    },
  });

  const top5cards_colors = players.map((e) => random_color(0.8));
  const top5cards_borders = players.map((e) => "rgba(0, 0, 0, 1.0)");
  const top5cards_cardname = players.map((e) => e["CardName"]);
  const top5cards_cardtype = players.map((e) => e["CardType"]);
  const top5cards_cardquality = players.map((e) => e["CardQuality"]);
  const top5cards_count = players.map((e) => e["NumberOfPlays"]);

  const ctx_levels3 = document.getElementById("mostUsedCards").getContext("2d");
  const levelChart3 = new Chart(ctx_levels3, {
    type: "bar",
    data: {
      labels: level_names,
      datasets: [
        {
          label: top5cards_cardname,
          backgroundColor: top5cards_colors,
          borderColor: top5cards_borders,
          borderWidth: 2,
          data: top5cards_count,
        },
      ],
    },
  });
} catch (error) {
  console.log(error);
}
