# Lunacid Randomizer Mechanics

Listed here are a few things I felt would be a good idea to note for the end user.  Likely will be expanded and changed around as development continues.

## Starting Weapn

Every seed gives you a weapon to start with.  This is just to keep logic involving combat reasonable and to not make every weapon/spell progression.  Any weapon or spell a player can find in areas up to the Sanguine Sea are possible:

Base Weapons - Replica Sword, Battle Axe, Stone Club, Ritual Dagger, Torch, Steel Spear, Wooden Shield, Broken Hilt, Elfen Bow, Elfen Sword
Base Spells - Flame Spear, Ice Spear, Wind Slicer, Slime Orb, Earth Strike

Shop Weapons - Crossbow, Rapier, Steel Needle

Drop Weapons - Skeleton Axe, Rusted Sword, Ice Sickle
Drop Spells - Dark Skull

Do note that the shop/drop weapon/spells are only given options if shopsanity/dropsanity are turned on, respectively.

## Wooden Barricade

The initial wooden barricade that blocks the player is removed.  This is done so a player starting as Forsaken can use the above weapons and it is a spell whose cost is higher than what they are capable of casting, though it may still result in a softlock and requiring a reset to another build if something more suitable cannot be found.

## Light Sources

There are several dark areas which require a source of light, as the game makes them too unbearably dark to traverse normally.  Any of the following are use in logic as a light source:

Flame Flare, Ghost Light, Moonlight, Twisted Staff, Torch, Crystal Lantern, Oil Lantern


## Forbidden Archives Access

The access to this area is logically open by default, as starting as Shinobi alone is enough to give enough SPD and DEX to make the jump.

## Other Jumps

Several places in the game (other than the path to Forbidden Archives) can require a jump normally not possible by a player.  Depending on the type of jump, the following are in logic to be used: 

Icarian Flight, Rock Bridge, Coffin.

Note later that Barrier will likely be included in this logic.

## Hints

The Bestial Communion spell has been modified to instead give players hints about random locations in the game, given some flair and obfuscated using the game's cipher and elf runes.  If an item is progression, it will auto-hint for you so you can see it in the tracker, but otherwise you will have to parse it on your own.

## Shop Locations (Prices)

Any shop location outside of the Enchanted Key assumes you can at least reach The Sanguine Sea, in order to give you more access to means of collecting funds to purchase them.

## Dying

When you die in Lunacid, the game will disconnect you from the server, to avoid a state where an attempt to send you items fails.  Reviving autoconnects you.

## Switch Locking

The setting "Switch Locks" goes to almost all switch locations in the game, and bars you from using them unless the appropriate switch item has been located.  Some progression markers are obvious, some not so much, so I will note a few:

- **Forbidden Archives (1F -2F) being non-progression**: The elevator starts in a position where you can get on it, and if it was left at the bottom floor it is trivial to reach the bottom without dying.  Its moreso annoying.
- **Terminus Prison Back Alley Switch being progression**: You may not have an alternative way to get back to the starting floor (such as lack of Icarius Flight).