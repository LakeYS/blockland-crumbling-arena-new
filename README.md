# Notice
This version of Crumbling Arena is based off a much older version of the game-mode. Development on this version of the game-mode is no-longer active. See here for the newest version by TheBlackParrot: https://github.com/TheBlackParrot/blockland-crumbling-arena

# Crumbling Arena
This game-mode adds numerous enhancements to TheBlackParrot's Crumbling Arena game-mode for Blockland.

This version of the game-mode was modified from the one available here: https://forum.blockland.us/index.php?topic=237666.0

# Change Log
* Removed "sun exploded" modifier
* Updated the HUD. It is now on by default with a new font
* Improved the remaining bricks display on the HUD. Bricks are now immediately removed from the count when stepped on
* The time it takes for the arena to become unstable is now based on the player count
* Player deaths are now recorded
* Added the /stats command
* Fixed "Set::getObject index out of range on BrickGroup_888888" console spam
* Added "slowpoke" and "speed" modifiers
* The game-mode now records how many bricks you destroy. You can view this with the /stats command.
* Added low gravity modifier
* Improved the fake kill effect; Bricks now stay visible for about three seconds after being destroyed. Destroyed bricks turn transparent to avoid obstructing player view
* Added an achievement system
* Added an RTB pref to toggle music
* The game-mode now handles empty servers more efficiently.
* Added "quicksand" modifier. (Bricks crumble faster)

# Achievements
Credit to KnockedOnWood (168791) and other various players for helping with achievement ideas.
* **Nerf This!** - Die from an explosive brick
* **Mario** - Jump on someone's head
* **Mario II** - Jump on two heads in a row
* **Peace Treaty** - Drop your weapon off the arena in a sword or broom round.
* **Aggressive** - Kill three or more opponents in a sword fight round.
* _(Not implemented)_ **Worm** - Tunnel under the arena and dig your way back to the surface! *(This won't be easy to implement.)*
* _(Not implemented)_ **Totally Unfair!** - Tie with another player
* _(Not implemented)_ **Determined** - Survive with only 10% of bricks remaining. (with at least two active players) *(Percentage subject to change)*
* _(Not implemented)_ **Rocket Jumper** - Launch yourself into the air with an explosive brick and survive.
* _(Not implemented)_ **Cautious** - Play an entire round without touching more than one brick at a time.
* _(Not implemented)_ **KABOOM!** - Die from an explosion with another player.

# Optional Stuff

## Music
A random song from your music list is chosen for each round. You can configure your music by editing the list in the Custom game-mode. You can turn music off by typing */toggleMusic* or with the RTB pref.

## Hats
If Server_HatMod is in your add-ons folder, the game-mode will automatically enable it along with all of your hats!
