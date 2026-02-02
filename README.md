# EasySave (CESI - Génie Logiciel)

EasySave est un logiciel de sauvegarde développé dans le cadre du projet fil rouge ProSoft (Programmation Système).
L’objectif est de concevoir et faire évoluer une solution de sauvegarde au fil de plusieurs versions, avec une attention particulière portée à la qualité, la maintenabilité, la traçabilité (logs/états) et la gestion de versions.

## Objectifs
- Développer les versions successives d’EasySave selon le cahier des charges
- Assurer une gestion propre du versioning et des livrables
- Produire les documentations demandées (utilisateur, support, release notes)
- Garantir un code lisible et maintenable (conventions, réduction des duplications, bonnes pratiques)

## Méthodologie Git (Workflow)
Nous utilisons le workflow suivant :

`feat/<FeatureName>` → `dev` → `main`

- **main** : branche stable (versions livrables / releases)
- **dev** : branche d’intégration (regroupe les fonctionnalités validées)
- **feat/<FeatureName>** : branche de développement d’une fonctionnalité (une feature = une branche)

### Règles de travail
1. Créer une branche à partir de `dev` :
   - `feat/<FeatureName>` (ex: `feat/log-json`)
2. Développer et committer sur la branche `feat/...`
3. Ouvrir une **Pull Request** : `feat/...` → `dev`
4. Après validation, intégrer `dev` vers `main` via **Pull Request** (pour les versions stables / livrables)

## Règles GitHub (Branch rules)
Les règles suivantes sont appliquées (ou à appliquer) sur `main` (et idéalement sur `dev`) :

- PR obligatoire avant merge
- Minimum **1 approbation** avant merge
- Résolution des conversations obligatoire
- Force push interdit
- Suppression de branche interdite (recommandé)

## Conventions
- Les messages de commit doivent être clairs et courts
- Préférer des commits petits et cohérents
- Toute modification significative passe par Pull Request

## Licence
Projet académique (CESI). Usage interne à l’équipe et à l’évaluation.
