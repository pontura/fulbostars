# Firebase Functions API

Base URL de las funciones (reemplazar con la URL real al deployar):
```
https://REGION-PROJECT_ID.cloudfunctions.net
```
La URL real aparece en la consola de Firebase después del deploy, o en la salida del comando `firebase deploy`.

---

## `submitMatchResult`

Suma los goles del partido al acumulado del torneo. Usa transacción atómica, por lo que no hay riesgo de pérdida de datos si dos usuarios envían resultados al mismo tiempo.

**Método:** `POST`  
**Endpoint:** `/submitMatchResult`

### Request

```
Content-Type: application/json
```

```json
{
  "torneoId": "torneo1",
  "golesTeam1": 2,
  "golesTeam2": 1
}
```

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `torneoId` | string | ID del torneo activo |
| `golesTeam1` | int ≥ 0 | Goles del equipo 1 en este partido |
| `golesTeam2` | int ≥ 0 | Goles del equipo 2 en este partido |

### Response

**200 OK**
```json
{ "ok": true }
```

**400 Bad Request** — faltan campos o valores inválidos
```json
{ "error": "torneoId, golesTeam1 y golesTeam2 son requeridos" }
```

### Ejemplo C# (UnityWebRequest)

```csharp
IEnumerator SubmitMatchResult(string torneoId, int golesTeam1, int golesTeam2, System.Action<bool> onDone)
{
    string url = "https://REGION-PROJECT_ID.cloudfunctions.net/submitMatchResult";
    string body = $"{{\"torneoId\":\"{torneoId}\",\"golesTeam1\":{golesTeam1},\"golesTeam2\":{golesTeam2}}}";
    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);

    using UnityWebRequest request = new UnityWebRequest(url, "POST");
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    bool ok = request.result == UnityWebRequest.Result.Success;
    onDone?.Invoke(ok);
}
```

Llamada desde un MonoBehaviour:
```csharp
StartCoroutine(SubmitMatchResult("torneo1", golesTeam1, golesTeam2, ok => {
    Debug.Log(ok ? "Resultado enviado" : "Error al enviar resultado");
}));
```

---

## `getTournamentData`

Devuelve el estado actual del torneo: goles acumulados de ambos equipos y cantidad de partidos jugados.

**Método:** `GET`  
**Endpoint:** `/getTournamentData?torneoId=torneo1`

### Query params

| Param | Tipo | Descripción |
|-------|------|-------------|
| `torneoId` | string | ID del torneo a consultar |

### Response

**200 OK**
```json
{
  "torneoId": "torneo1",
  "team1": { "goles": 1200 },
  "team2": { "goles": 1500 },
  "partidos_jugados": 342
}
```

Si el torneo todavía no tiene ningún partido registrado, devuelve todo en 0 (no devuelve 404).

**400 Bad Request**
```json
{ "error": "torneoId es requerido" }
```

---

## Estructura en Realtime Database

```
torneos/
  torneo1/
    team1/
      goles: 1200
    team2/
      goles: 1500
    partidos_jugados: 342
  torneo2/
    ...
```

Para agregar un nuevo torneo simplemente se llama a `submitMatchResult` con un `torneoId` nuevo — se crea solo.

---

## Notas de implementación

- Ambos endpoints tienen CORS abierto (`*`) para ser llamados desde WebGL.
- `submitMatchResult` valida que los goles sean enteros no negativos.
- El partido se registra completo (ambos equipos a la vez) en una sola transacción; nunca queda un team actualizado sin el otro.
