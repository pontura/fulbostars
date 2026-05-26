const { onRequest } = require("firebase-functions/v2/https");
const { initializeApp } = require("firebase-admin/app");
const { getDatabase } = require("firebase-admin/database");

initializeApp();

const CORS_HEADERS = {
  "Access-Control-Allow-Origin": "*",
  "Access-Control-Allow-Methods": "GET, POST, OPTIONS",
  "Access-Control-Allow-Headers": "Content-Type",
};

function handleCors(req, res) {
  Object.entries(CORS_HEADERS).forEach(([k, v]) => res.set(k, v));
  if (req.method === "OPTIONS") {
    res.status(204).send("");
    return true;
  }
  return false;
}

/**
 * POST /submitMatchResult
 * Body: { torneoId: string, golesTeam1: number, golesTeam2: number }
 *
 * Suma atómicamente los goles del partido al acumulado del torneo
 * e incrementa el contador de partidos jugados.
 */
exports.submitMatchResult = onRequest({ invoker: "public" }, async (req, res) => {
  if (handleCors(req, res)) return;

  if (req.method !== "POST") {
    res.status(405).json({ error: "Method not allowed" });
    return;
  }

  const { torneoId, golesTeam1, golesTeam2 } = req.body;

  if (!torneoId || golesTeam1 == null || golesTeam2 == null) {
    res.status(400).json({ error: "torneoId, golesTeam1 y golesTeam2 son requeridos" });
    return;
  }

  if (typeof golesTeam1 !== "number" || typeof golesTeam2 !== "number" ||
      golesTeam1 < 0 || golesTeam2 < 0 ||
      !Number.isInteger(golesTeam1) || !Number.isInteger(golesTeam2)) {
    res.status(400).json({ error: "golesTeam1 y golesTeam2 deben ser enteros no negativos" });
    return;
  }

  const db = getDatabase();
  const torneoRef = db.ref(`torneos/${torneoId}`);

  await torneoRef.transaction((current) => {
    if (current === null) {
      return {
        team1: { goles: golesTeam1 },
        team2: { goles: golesTeam2 },
        partidos_jugados: 1,
      };
    }
    return {
      team1: { goles: (current.team1?.goles ?? 0) + golesTeam1 },
      team2: { goles: (current.team2?.goles ?? 0) + golesTeam2 },
      partidos_jugados: (current.partidos_jugados ?? 0) + 1,
    };
  });

  res.status(200).json({ ok: true });
});

/**
 * GET /getTournamentData?torneoId=torneo1
 *
 * Devuelve los goles acumulados de ambos equipos y los partidos jugados.
 */
exports.getTournamentData = onRequest(async (req, res) => {
  if (handleCors(req, res)) return;

  if (req.method !== "GET") {
    res.status(405).json({ error: "Method not allowed" });
    return;
  }

  const { torneoId } = req.query;

  if (!torneoId) {
    res.status(400).json({ error: "torneoId es requerido" });
    return;
  }

  const db = getDatabase();
  const snapshot = await db.ref(`torneos/${torneoId}`).once("value");

  if (!snapshot.exists()) {
    res.status(200).json({
      torneoId,
      team1: { goles: 0 },
      team2: { goles: 0 },
      partidos_jugados: 0,
    });
    return;
  }

  const data = snapshot.val();
  res.status(200).json({
    torneoId,
    team1: { goles: data.team1?.goles ?? 0 },
    team2: { goles: data.team2?.goles ?? 0 },
    partidos_jugados: data.partidos_jugados ?? 0,
  });
});
