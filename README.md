# Game Lab 2025

Un projet de jeu Unity développé dans le cadre du Game Lab 2025.

## Description

Ce projet est un jeu développé avec Unity, incluant un système de menu en jeu avec :
- Un menu pause
- Un menu d'options
- Un système de compteurs d'objets ramassés (sticks et fragments)

## Avancement du projet (Juin 2024)

- **Menus fonctionnels** :
  - Menu pause accessible avec la touche Échap
  - Menu d'options opérationnel
- **Système d'inventaire retiré** :
  - L'inventaire visuel a été supprimé pour se concentrer sur des compteurs d'objets ramassés
- **Compteurs d'objets** :
  - Deux compteurs affichent en temps réel le nombre de bâtons (sticks) et de fragments ramassés
  - Les compteurs sont visibles en jeu et animés avec le bouton pause
- **UI** :
  - Interface utilisateur responsive
  - Les compteurs et le bouton pause disparaissent/réapparaissent ensemble selon l'état du jeu
- **Interactions** :
  - Les objets ramassables mettent à jour les compteurs via le script d'interaction

## Prérequis

- Unity 2022.3 ou version ultérieure
- Visual Studio 2019/2022 ou Visual Studio Code

## Installation

1. Clonez ce dépôt :
```bash
git clone https://github.com/votre-username/Game_Lab_2025.git
```

2. Ouvrez le projet dans Unity Hub

3. Ouvrez la scène principale dans le dossier Scenes

## Structure du Projet

```
Assets/
├── Script/           # Scripts C#
├── Scenes/           # Scènes du jeu
├── Prefabs/          # Préfabriqués
├── Audios/           # Effets sonores et musiques
├── Sprites/          # Images et sprites du jeu
└── UI/               # Éléments d'interface utilisateur
```

## Contribution

Les contributions sont les bienvenues ! N'hésitez pas à :
1. Fork le projet
2. Créer une branche pour votre fonctionnalité
3. Commiter vos changements
4. Pousser vers la branche
5. Ouvrir une Pull Request

## Licence

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de détails. 