# Documentation

Welcome to the documentation for **Exterrestrial Space Thrift**! Here you will find detailed information about the project's code structure and functionalities.

## Table of Contents

- [Documentation](#documentation)
  - [Table of Contents](#table-of-contents)
  - [Audio](#audio)
  - [CameraController](#cameracontroller)
  - [Enemy](#enemy)
  - [Entity](#entity)
  - [GameManager](#gamemanager)
  - [InterActions](#interactions)
  - [Level](#level)
  - [LootTables](#loottables)
  - [Player](#player)
  - [Stats](#stats)
  - [Temporary](#temporary)
  - [Tweening](#tweening)

## Audio

- [MusicManager.cs](/Docs/Audio/MusicManager.md): Responsible for managing the game's music.

## CameraController

- [RotateToCloseCam.cs](/Docs/CameraController/RotateToCloseCam.md): A script that allows the camera to rotate towards a closed space.
- [RotateToCloseEntity.cs](/Docs/CameraController/RotateToCloseEntity.md): Enables the camera to rotate towards a close entity.

## Enemy

- [EnemyController.cs](/Docs/Enemy/EnemyController.md): Handles enemy behavior and AI.
- [EnemyProjectile.cs](/Docs/Enemy/EnemyProjectile.md): Controls enemy projectile behavior.

## Entity

- [BulletController.cs](/Docs/Entity/BulletController.md): Controls bullet behavior.
- [EntityCombatController.cs](/Docs/Entity/EntityCombatController.md): Manages entity combat logic.
- [EntityController.cs](/Docs/Entity/EntityController.md): The main script for controlling entities in the game.
- [EntityMovementController.cs](/Docs/Entity/EntityMovementController.md): Manages entity movement.
- [EntityState.cs](/Docs/Entity/EntityState.md): Defines the different states an entity can be in.

## GameManager

- [GameManager.cs](/Docs/GameManager.md): Manages the overall game state and flow.

## InterActions

- [AnimStarter.cs](/Docs/InterActions/AnimStarter.md): Triggers animations for specific events.
- [HighScoreDisplay.cs](/Docs/InterActions/HighScoreDisplay.md): Displays the high score on the UI.
- [Interacter.cs](/Docs/InterActions/Interacter.md): Allows interaction with objects and entities.
- [PickUpInteraction.cs](/Docs/InterActions/PickUpInteraction.md): Handles interactions for picking up objects.
- [XPToken.cs](/Docs/InterActions/XPToken.md): Represents experience points as a collectible token.

## Level

- [LevelController.cs](/Docs/Level/LevelController.md): Manages the current level's logic and events.
- [LevelManager.cs](/Docs/Level/LevelManager.md): Handles the level management system.
- [LevelScaler.cs](/Docs/Level/LevelScaler.md): Scales the level dynamically based on player progress.
- [Wall.cs](/Docs/Level/Wall.md): Controls the behavior of walls in the level.

## LootTables

- [LootTable.cs](/Docs/LootTables/LootTable.md): Defines the loot table logic.
- [LootTableObj.cs](/Docs/LootTables/LootTableObj.md): Represents an individual object in the loot table.

## Player

- [PlayerActions.inputactions](/Docs/Player/PlayerActions.inputactions): Input actions for the player.
- [PlayerAnimationController.cs](/Docs/Player/PlayerAnimationController.md): Manages the player's animations.
- [PlayerInputManager.cs](/Docs/Player/PlayerInputManager.md): Handles player input.
- [PlayerInteractManager.cs](/Docs/Player/PlayerInteractManager.md): Manages player interactions with objects and entities.
- [PlayerMovementController.cs](/Docs/Player/PlayerMovementController.md): Controls player movement.
- [PlayerSoundController.cs](/Docs/Player/PlayerSoundController.md): Manages player sound effects.
- [PlayerSpawnManager.cs](/Docs/Player/PlayerSpawnManager.md): Handles player spawning logic.
- [UiCamScaler.cs](/Docs/Player/UiCamScaler.md): Scales the UI camera based on player distance.
- [UiController.cs](/Docs/Player/UiController.md): Controls the user interface.
- [Weapon](/Docs/Player/Weapon): Directory for player weapon-related scripts.

## Stats

- [InventoryStats.cs](/Docs/Stats/InventoryStats.md): Manages the player's inventory stats.

## Temporary

- [TestCameraContoller.cs](/Docs/Temporary/TestCameraContoller.md): A temporary script for testing camera functionality.
- [UiRotater.cs](/Docs/Temporary/UiRotater.md): A temporary script for UI rotation.

## Tweening

- [LeanTween](/Docs/Tweening/LeanTween): Directory for tweening scripts using LeanTween.

