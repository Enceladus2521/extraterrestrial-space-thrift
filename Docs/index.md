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

- [MusicManager.cs](/Audio/MusicManager.cs): Responsible for managing the game's music.

## CameraController

- [RotateToCloseCam.cs](/CameraController/RotateToCloseCam.cs): A script that allows the camera to rotate towards a closed space.
- [RotateToCloseEntity.cs](/CameraController/RotateToCloseEntity.cs): Enables the camera to rotate towards a close entity.

## Enemy

- [EnemyController.cs](/Enemy/EnemyController.cs): Handles enemy behavior and AI.
- [EnemyProjectile.cs](/Enemy/EnemyProjectile.cs): Controls enemy projectile behavior.

## Entity

- [BulletController.cs](/Entity/BulletController.cs): Controls bullet behavior.
- [EntityCombatController.cs](/Entity/EntityCombatController.cs): Manages entity combat logic.
- [EntityController.cs](/Entity/EntityController.cs): The main script for controlling entities in the game.
- [EntityMovementController.cs](/Entity/EntityMovementController.cs): Manages entity movement.
- [EntityState.cs](/Entity/EntityState.cs): Defines the different states an entity can be in.

## GameManager

- [GameManager.cs](/GameManager.cs): Manages the overall game state and flow.

## InterActions

- [AnimStarter.cs](/InterActions/AnimStarter.cs): Triggers animations for specific events.
- [HighScoreDisplay.cs](/InterActions/HighScoreDisplay.cs): Displays the high score on the UI.
- [Interacter.cs](/InterActions/Interacter.cs): Allows interaction with objects and entities.
- [PickUpInteraction.cs](/InterActions/PickUpInteraction.cs): Handles interactions for picking up objects.
- [XPToken.cs](/InterActions/XPToken.cs): Represents experience points as a collectible token.

## Level

- [LevelController.cs](/Level/LevelController.cs): Manages the current level's logic and events.
- [LevelManager.cs](/Level/LevelManager.cs): Handles the level management system.
- [LevelScaler.cs](/Level/LevelScaler.cs): Scales the level dynamically based on player progress.
- [Wall.cs](/Level/Wall.cs): Controls the behavior of walls in the level.

## LootTables

- [LootTable.cs](/LootTables/LootTable.cs): Defines the loot table logic.
- [LootTableObj.cs](/LootTables/LootTableObj.cs): Represents an individual object in the loot table.

## Player

- [PlayerActions.inputactions](/Player/PlayerActions.inputactions): Input actions for the player.
- [PlayerAnimationController.cs](/Player/PlayerAnimationController.cs): Manages the player's animations.
- [PlayerInputManager.cs](/Player/PlayerInputManager.cs): Handles player input.
- [PlayerInteractManager.cs](/Player/PlayerInteractManager.cs): Manages player interactions with objects and entities.
- [PlayerMovementController.cs](/Player/PlayerMovementController.cs): Controls player movement.
- [PlayerSoundController.cs](/Player/PlayerSoundController.cs): Manages player sound effects.
- [PlayerSpawnManager.cs](/Player/PlayerSpawnManager.cs): Handles player spawning logic.
- [UiCamScaler.cs](/Player/UiCamScaler.cs): Scales the UI camera based on player distance.
- [UiController.cs](/Player/UiController.cs): Controls the user interface.
- [Weapon](/Player/Weapon): Directory for player weapon-related scripts.

## Stats

- [InventoryStats.cs](/Stats/InventoryStats.cs): Manages the player's inventory stats.

## Temporary

- [TestCameraContoller.cs](/Temporary/TestCameraContoller.cs): A temporary script for testing camera functionality.
- [UiRotater.cs](/Temporary/UiRotater.cs): A temporary script for UI rotation.

## Tweening

- [LeanTween](/Tweening/LeanTween): Directory for tweening scripts using LeanTween.

