using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Enums;

namespace LunacidAP.Data
{
    public class ArchipelagoGames
    {
        public static readonly Dictionary<string, string> GameToProtagonist = new(){
            // Supported
            {"Ocarina of Time", "the Hero of Time"},
            {"Link to the Past", "the Hero of Legend"},
            {"Link's Awakening DX", "the Hero of Legend"},
            {"Lingo", "a mysterious soul"},
            {"Muse Dash", "many women of notoriety"},
            {"Stardew Valley", "a simple farmer"},
            {"Adventure", "a simple dot"},
            {"Blasphemous", "a penitent one"},
            {"Dark Souls III", "a feeble cursed one"},
            {"DLCQuest", "a soul driven to spend silver"},
            {"Donkey Kong 3", "a resourceful monkey and her cousin"},
            {"DOOM 1993", "a man known to rip and tear"},
            {"DOOM II", "a man known to rip and tear"},
            {"Final Fantasy", "a band of four heroes"},
            {"Final Fantasy Mystic Quest", "a simple-minded adventurer"},
            {"Super Metroid", "a space bounty hunter"},
            {"Heretic", "an elven man"},
            {"Hollow Knight", "a small insectoid knight"},
            {"Hylics 2", "a moon shaped entity and their anarchic cohorts"},
            {"Kingdom Hearts 2", "a young boy and his two animal companions"},
            {"Landstalker - The Treasure of King Nole", "an intrepid treasure hunter"},
            {"The Legend of Zelda", "the Hero of Hyrule"},
            {"Lufia II Ancient Cave", "a swordsman and father"},
            {"MegaMan Battle Network 3", "A young boy and his computer program"},
            {"Meritous", "a talented PSI user"},
            {"The Messenger", "a young ninja"},
            {"Minecraft", "a cuboid figure"},
            {"Noita", "an unnamed wizard"},
            {"Overcooked! 2", "a group of persistent chefs"},
            {"Pokemon Emerald", "a young trainer"},
            {"Pokemon Red and Blue", "a young trainer"},
            {"Raft", "a poor soul lost at sea"},
            {"Risk of Rain 2", "a lost survivalist"},
            {"Rogue Legacy", "an esteemed family lineage"},
            {"Secret of Evermore", "a boy and his dog"},
            {"Shivers", "a mad professor"},
            {"Factorio", "a lost colonist"},
            {"Slay the Spire", "a group of brave adventurers"},
            {"SMZ3", "the Hero of Legend and a bounty hunter"},
            {"Sonic Adventure 2 Battle", "a blue hedgehog and his friends"},
            {"Starcraft 2", "a group of soldiers"},
            {"Subnautica", "a stranded soul"},
            {"Super Mario 64", "a waterway technician"},
            {"Super Mario World", "a waterway technician"},
            {"Terraria", "an alleged band of wanderers"},
            {"Timespinner", "a nomadic woman"},
            {"Undertale", "a lost child"},
            {"VVVVVV", "a band of colorful explorers"},
            {"Wargroove", "a group of soldiers"},
            {"The Witness", "a non-descript individual"},
            {"Zillion", "a group of peacekeepers"},
            {"A Short Hike", "a simple blue bird"},
            {"Celeste 64", "a short, bouncy girl"},
            {"Yoshi's Island", "a group of dinosaurs and a small child"},
            {"Zork Grand Inquisitor", "a bumbling salesperson of an old electric company"},
            // Unsupported
            {"Lunacid", "an outcast"},
            {"Wario Land 4", "a plump, greedy man"},
            {"A Hat in Time", "a rambunctious girl"},
            {"The Wind Waker", "the Hero of Winds"},
            {"Little Witch Nobeta", "a little girl"},
            {"Ender Lilies", "a little girl"},
            {"Pseudoregalia", "a goat-rabbit woman"},
            {"Old School Runescape", "an quirky adventurer"},
            {"Mario & Luigi Superstar Saga", "two dynamic brothers"},
            {"Ultrakill", "a lifeless being, desperate for blood,"},
            {"Resident Evil 2 Remake", "two souls desperate for answers"},
            {"Night of 100 Frights", "a group of sleuthing teenagers and their dog"},
            {"Duke Nukem 3D", "a total badass in search for a holiday but found ass to kick"},
            {"The Legend of Zelda - Oracle of Seasons", "the Hero of Essences of Time and Nature"},
            {"Banjo-Tooie", "a bear and their bird"},
            {"Celeste", "a short, bouncy girl"},
            {"Hades", "the son of Hades"},
            {"An Untitled Story", "an egg"},
            {"Mega Man X", "a robotic boy with myriad of powers"},
            {"Yacht Dice", "a group of dice"},
            {"Final Fantasy V Career Day", "an adventerous boy and his trusted large chicken"},
            {"Final Fantasy 6 Worlds Collide", "an esper and her allies"},
            {"Inscryption", "a curious man"},
            {"Chrono Trigger Jets of Time", "a young boy and his friends"},
            {"Pokemon Crystal", "a young trainer"},
            {"Paper Mario", "a plumber and their myriad of platonic partners"},
            {"Civilization VI", "several well-known historical figures"},
            {"Aquaria", "an amnesiac aquatic woman"},
            {"Final Fantasy 12 Open World", "a street rat"},
            {"Battle for Bikini Bottom", "a yellow sponge"},
            {"Metroid Prime", "a space bounty hunter"},
            {"Super Metroid Map Rando", "a space bounty hunter"},
            {"Super Junkoid", "a young girl"},
        };
        public static readonly Dictionary<string, string> GameToItemBlurb = new(){
            // Supported
            {"Ocarina of Time", "the land of Hyrule."},
            {"Link to the Past", "the land of Hyrle."},
            {"Link's Awakening DX", "the island of Koholint."},
            {"Lingo", "a strange land, of words and puzzles."},
            {"Muse Dash", "a musical land filled with joy."},
            {"Stardew Valley", "a town named after the long-extinct pelican."},
            {"Adventure", "a simple, small land"},
            {"Blasphemous", "the land of Cvstodia."},
            {"Dark Souls III", "the land of Lothric."},
            {"DLCQuest", "a land bogged down by financial expansion."},
            {"Donkey Kong 3", "the northern Krimisphere"},
            {"DOOM 1993", "upon a distant planet's surface."},
            {"DOOM II", "upon a distant planet's surface."},
            {"Final Fantasy", "the land of Cornelia."},
            {"Final Fantasy Mystic Quest", "the land of Foresta and its surrounding lands."},
            {"Super Metroid", "upon a distant planet's surface."},
            {"Heretic", "a city filled with the damned, yet to fall into the Great Well."},
            {"Hollow Knight", "beneath the fading town of Dirthmouth."},
            {"Hylics 2", "near the city of New Muldul."},
            {"Kingdom Hearts 2", "the isles of Destiny."},
            {"Landstalker - The Treasures of King Nole", "the island of Mercator."},
            {"The Legend of Zelda", "the land of Hyrule."},
            {"Lufia II Ancient Cave", "the small town of Elcid."},
            {"MegaMan Battle Network 3", "the cyberland of the Net."},
            {"Meritous", "deep within the fabled Orcus Dome."},
            {"The Messenger", "a secretive village of ninjas."},
            {"Minecraft", "a uncharted land filled with creativity."},
            {"Noita", "deep beneath the Great Tree."},
            {"Pokemon Emerald", "the town of Littleroot."},
            {"Pokemon Red and Blue", "the town of Pallet."},
            {"Raft", "an unknown land amongst the tide."},
            {"Risk of Rain 2", "a chaotic, strange world."},
            {"Secret of Evermore", "the land of Evermore."},
            {"Overcooked! 2", "the old Kingdom of Onions."},
            {"Rogue Legacy", "a land cursed by an endless castle."},
            {"Shivers", "the lost museum of Windlenot."},
            {"Factorio", "an alien world."},
            {"Slay the Spire", "afar who attempt to conquer a cursed Spire."},
            {"SMZ3", "from the land of Hyrule and a distant planet's surface."},
            {"Sonic Adventure 2 Battle", "the land of Mobius."},
            {"Starcraft 2", "distant planets across the Milky Way."},
            {"Subnautica", "an alien world, adrift at sea."},
            {"Super Mario 64", "a princess' castle."},
            {"Super Mario World", "the Kingdom of Mushroom."},
            {"Terraria", "a land of creativity, wonder, and danger."},
            {"Timespinner", "a tribe seiged by imperial conflict."},
            {"Undertale", "above the surface, stuck below."},
            {"VVVVVV", "a place far away amongst the stars."},
            {"Wargroove", "the lands of Aurania."},
            {"The Witness", "a land of puzzles, lines, and mystery."},
            {"Zillion", "a distant base upon a distant land."},
            {"A Short Hike", "a simple vacation island."},
            {"Celeste 64", "the tall peaks of Celeste Mountain."},
            {"Yoshi's Island", "the vast lands of Yoshi's Island."},
            {"Zork Grand Inquisitor", "the oppressed lands of the kingdom of Quendor."},
            // Unsupported
            {"Lunacid", "a world, similar to this one, yet different."},
            {"Wario Land 4", "deep within an ancient pyramid."},
            {"A Hat in Time", "a distant planet oppressed by the mafia."},
            {"The Wind Waker", "a great blue sea above long destroyed kingdom."},
            {"Little Witch Nobeta", "a long lost lab she inevitably returns to."},
            {"Ender Lilies", "the kingdom of Land's End."},
            {"Pseudoregalia", "deep within a dilapidated dungeon."},
            {"Old School Runescape", "the town of Lumbridge."},
            {"Mario & Luigi Superstar Saga", "the kingdom of Beans, though they were only visiting."},
            {"Ultrakill", "the depths of hell."},
            {"Resident Evil 2 Remake", "the remains of the city of Raccoon."},
            {"Night of 100 Frights", "an old, spooky mansion."},
            {"Duke Nukem 3D", "the city of Los Angeles."},
            {"The Legend of Zelda - Oracle of Seasons", "the lands of Holodrum."},
            {"Banjo-Tooie", "the area surrounding Spiral Mountain."},
            {"Celeste", "the tall peaks of Celeste Mountain."},
            {"Hades", "deep within the Underworld."},
            {"An Untitled Story", "the lands surrounding their comfortable nest."},
            {"Mega Man X", "the far future during a war against the Reploids."},
            {"Yacht Dice", "the tumbing cup within your very hands."},
            {"Final Fantasy V Career Day", "a far off land protected by four crystals."},
            {"Final Fantasy 6 Worlds Collide", "a land of magitek ruled by a tyrant."},
            {"Inscryption", "your home town, sucked into a mystery surrounding a floppy disk."},
            {"Chrono Trigger Jets of Time", "from the town of Truce within the kingdom of Guardia."},
            {"Pokemon Crystal", "from the town of New Bark."},
            {"Paper Mario", "from the Kingdom of Mushroom."},
            {"Civilization VI", "from their respective lands, fighting for glory."},
            {"Aquaria", "from deep beneath the waves"},
            {"Final Fantasy 12 Open World", "from the lands of Ivalice."},
            {"Battle for Bikini Bottom", "the underwater city of Bikini Bottom."},
            {"Metroid Prime", "on the remote planet of Pallon IV."},
            {"Super Metroid Map Rando", "upon a distant planet's surface."},
            {"Super Junkoid", "within their own eternal dreams, induced by a serpent."}

        };

        public static string KeywordToItem(ArchipelagoItem archipelagoItem)
        {
            var game = archipelagoItem.Game;
            var itemName = archipelagoItem.Name;
            var random = new Random(itemName.GetHashCode());
            switch (game)
            {
                case "Muse Dash":
                {
                    return "VHS Tape";
                }
                case "ArchipIDLE":
                {
                    return "VHS Tape";
                }

            }
            if (itemName.Contains("Key") || itemName.Contains("Red key") || itemName.Contains("Blue key") || itemName.Contains("Yellow key") ||
            itemName.Contains("Green key"))
            {
                return "Enchanted Key";
            }
            if (itemName.Contains("Chaos Emerald"))
            {
                return "Wisp Heart";
            }
            if (itemName.Contains("Feather"))
            {
                return "Angel Feather";
            }
            else if (itemName.Contains("Rupees") || itemName.Contains("Money") || itemName.Contains("Soul") || 
                    itemName.Contains("Geo") || itemName.Contains("Bones") || itemName.Contains("Coin"))
            {
                return "Silver";
            }
            else if (itemName.Contains("Arrows") || itemName.Contains("Ammo"))
            {
                return "VHS Tape";
            }
            else if (itemName.Contains("Sword") || itemName.Contains("Dagger") || itemName.Contains("Blade") || itemName.Contains("Knife"))
            {
                return LunacidItems.Swords[random.Next(0, LunacidItems.Swords.Count)];
            }
            else if (itemName.Contains("Spear") || itemName.Contains("Lance"))
            {
                return LunacidItems.Spears[random.Next(0, LunacidItems.Spears.Count)];
            }
            else if (itemName.Contains("Axe"))
            {
                return LunacidItems.Axes[random.Next(0, LunacidItems.Axes.Count)];
            }
            else if (itemName.Contains("Bow") || itemName.Contains("Crossbow") || 
            itemName.Contains("Gun") || itemName.Contains("Launcher") || itemName.Contains("Shotgun") || itemName.Contains("Chaingun")
            )
            {
                return LunacidItems.Bows[random.Next(0, LunacidItems.Axes.Count)];
            }
            else if (itemName.Contains("Bomb") || itemName.Contains("Grenade") || itemName.Contains("Rocket"))
            {
                return "Bomb";
            }
            else if (itemName.Contains("Glove") || itemName.Contains("Strength") || itemName.Contains("Bracelet"))
            {
                return LunacidItems.Gloves[random.Next(0, LunacidItems.Gloves.Count)];
            }
            else if (itemName.Contains("Shield"))
            {
                return LunacidItems.Shields[random.Next(0, LunacidItems.Shields.Count)];
            }
            else if (itemName.Contains("Staff") || itemName.Contains("Rod") || itemName.Contains("Wand"))
            {
                return new List<string>(){"Twisted Staff", "Wand of Power"}[random.Next(2)];
            }
            else if (itemName.Contains("Magic Meter") || itemName.Contains("Heart Container") || itemName.Contains("Energy Tank"))
            {
                return "Earth Elixir";
            }
            else if (itemName.Contains("Magic") || itemName.Contains("Spell") || itemName.Contains("Orb") || itemName.Contains("TM") ||
            itemName.Contains("HM"))
            {
                return "Flame Flare";
            }
            else if (itemName.Contains("Song") || itemName.Contains("Book") || itemName.Contains("Tome"))
            {
                return "Black Book";
            }
            else if (itemName.Contains("Bottle") || itemName.Contains("Potion") || itemName.Contains("Flask"))
            {
                return "Holy Water";
            }
            else if (archipelagoItem.Classification.HasFlag(ItemFlags.None))
            {
                return "Ashes";
            }
            
            return "NULL";

        }

        public static readonly Dictionary<string, string[]> GameToCliveLore = new(){
            {"", new string[4]{"Truth be told, I don't quite remember most of the details.",
                               "Not for a lack of trying mind you!  Perhaps I just never heard the story in particular before.",
                               "But there is one thing I do remember at least.  I hope that much will do you some good.",
                               "Not much, but its enough."}
            },
            {"Adventure", new string[4]{"While this story has some holes, its certainly a curious tale.  It surrounds the machinations of an evil magician.",
                                        "They had desires to ruin the kingdom.  For what purpose was lost to time, but the method was known: stealing the Golden Chalice.",
                                        "The kingdom sent their strongest, and the magician, three beasts: Rhindle, Grundle, and Yorgle.  A long journey fell before the hero.",
                                        "Using all sorts of trickery and cunning, they did all they could to avoid destruction, to return the chalice to the Golden Castle."
                                        }
            },
            {"Lunacid", new string[4]{"This story, I can't quite recall if it was in this place, or another similar to this one.",
                                     "After the deep fog had consumed the land, a certain individual was placed on a cart, to be discarded here.",
                                     "Cast into the Great Well, they found a sign: to find a path to a slumbering creature, for they were the only exit.",
                                     "A dreamwoven tale to be sure.  Though I have the foggiest of all the details yet.  I could've sworn it was recently.",
                                     }
            },
            {"Stardew Valley", new string[4]{"A story of a much gentler time, in the Ferngill Republic before its ruin.",
                                            "The grandchild of a farmer was given the deed to a farm upon his passing, to tend as they see fit.",
                                            "And so they did, forging bonds with the local people, and even the very spirits of the land.",
                                            "Bringing everyone together, they were able to bring joy and merriment to that place."}
            },
        };
    }
}