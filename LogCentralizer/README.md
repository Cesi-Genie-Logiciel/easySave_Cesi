# LogCentralizer - Service Docker P4

Service centralisé pour recevoir et stocker les logs de tous les clients EasySave.

## Architecture

- **API REST** : ASP.NET Core Web API
- **Port** : 5000
- **Stockage** : Fichiers JSON journaliers (`centralized_log_YYYY-MM-DD.json`)
- **Multi-utilisateurs** : Chaque log inclut `clientId` (machine + user)

## Endpoints

- `POST /api/log` : Recevoir un log d'un client
- `GET /api/log/{date}` : Récupérer les logs d'une date (format: `yyyy-MM-dd`)
- `GET /api/health` : Health check

## Déploiement Docker

```bash
cd LogCentralizer
docker-compose up -d
```

## Configuration client

Dans EasySave, utiliser `LoggerFactory.Create()` avec :
- `LogDestination.Centralized` ou `LogDestination.Both`
- `serverUrl: "http://localhost:5000"`

## Volume

Les logs sont stockés dans `./logs` (monté depuis l'hôte).
