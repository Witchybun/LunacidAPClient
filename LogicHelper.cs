using System.Collections.Generic;
using BepInEx.Logging;
using LunacidAP.Data;

namespace LunacidAP
{
    public class LogicHelper
    {
        private ManualLogSource _log;

        public LogicHelper(ManualLogSource log)
        {
            _log = log;
        }

        public Dictionary<string, bool> LocationToLogic { get; private set; }


        public bool LogicCases(string location, string sceneName)
        {
            switch (location)
            {
                case "WR: Rafters":
                    {
                        return true;

                    }
                case "WR: Clive's Gift":
                    {
                        return true;

                    }
                case "WR: Demi's Victory Gift":
                    {
                        return ConnectionData.EnteredScenes.Contains("ARENA2");

                    }
                case "HB: Starting Weapon":
                    {
                        return true;

                    }
                case "HB: Rightmost Water Room (Right)":
                    {
                        return true;

                    }
                case "HB: Rightmost Water Room (Left)":
                    {
                        return true;

                    }
                case "HB: Leftmost Water Room":
                    {
                        return true;

                    }
                case "HB: Chest Near Demi":
                    {
                        return true;

                    }
                case "HB: Near Enchanted Door":
                    {
                        return true;

                    }
                case "HB: Dark Tunnel After Enchanted Door":
                    {
                        return !ArchipelagoClient.AP.SlotData.Shopsanity || WasItemReceived("Enchanted Key");
                    }
                case "HB: Temple Fountain":
                    {
                        return CanEnterTemple();

                    }
                case "HB: Temple Ritual Table":
                    {
                        return CanEnterTemple();

                    }
                case "HB: Temple Altar Chest":
                    {
                        return CanEnterTemple();

                    }
                case "HB: Temple Hidden Room Behind Pillar (Left)":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key") && HasDustyOrb();

                    }
                case "HB: Temple Hidden Room Behind Pillar (Right)":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key") && HasDustyOrb();

                    }
                case "HB: Temple Ritual Table After Bridge":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key");

                    }
                case "HB: Temple Small Pillar Top":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key");

                    }
                case "HB: Temple Pillar Room Left":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key") && HasDustyOrb();

                    }
                case "HB: Temple Pillar Room Back Left":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key") && HasDustyOrb();

                    }
                case "HB: Temple Pillar Room Back Right":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key") && HasDustyOrb();

                    }
                case "HB: Temple Pillar Room Hidden Room":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key") && HasDustyOrb();

                    }
                case "HB: Temple Hidden Room In Sewer":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key") && HasDustyOrb();

                    }
                case "HB: Temple Table in Sewer":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key");

                    }
                case "HB: Temple Sewer Puzzle":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key") && WasItemReceived("VHS Tape") && ConnectionData.EnteredScenes.Contains("Accursed Tomb") && HasElement("Light") && HasDustyOrb();

                    }
                case "HB: Temple Blood Altar":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key") && WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"});
                    }
                case "HB: Alcove on Path to Yosei Forest":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key");

                    }
                case "FM: Room Left of Foyer":
                    {
                        return true;

                    }
                case "FM: Hidden Slimey Chest Near Entrance":
                    {
                        return HasDustyOrb();

                    }
                case "FM: Hidden Upper Overlook (Left)":
                    {
                        return HasDustyOrb();

                    }
                case "FM: Hidden Upper Overlook (Right)":
                    {
                        return HasDustyOrb();

                    }
                case "FM: Bonenard's Trash":
                    {
                        return true;

                    }
                case "FM: Rubble Near Overlook Bridge":
                    {
                        return true;

                    }
                case "FM: Slime Skeleton Chest":
                    {
                        return true;

                    }
                case "FM: Jellisha's Trash":
                    {
                        return HasDustyOrb();

                    }
                case "FM: Jellisha's Quest Reward":
                    {
                        return HasDustyOrb();

                    }
                case "FM: Hidden Chest Near Underworks":
                    {
                        return HasDustyOrb();

                    }
                case "FM: Rubble Near Illusory Wall":
                    {
                        return true;

                    }
                case "FM: Underwater Pipe":
                    {
                        return true;

                    }
                case "FM: Underworks Waterfall":
                    {
                        return true;

                    }
                case "FM: Underworks Skeleton":
                    {
                        return true;

                    }
                case "FM: Path to Sanguine Sea (Left)":
                    {
                        return true;

                    }
                case "FM: Path to Sanguine Sea (Right)":
                    {
                        return true;

                    }
                case "SS: Pillar In Front of Castle Le Fanu":
                    {
                        return WereAnyItemsReceived(new List<string>(){"Icarian Flight", "Rock Bridge", "Barrier"});
                }
                case "SS: Underblood Near Castle Le Fanu":
                    {
                        return true;

                    }
                case "SS: Fairy Circle":
                    {
                        return true;

                    }
                case "SS: Killing the Jotunn":
                    {
                        return !ArchipelagoClient.AP.SlotData.Shopsanity && 
                        WereAllItemsReceived(new List<string>(){"Water Talisman", "Earth Talisman"}) || 
                        WasItemReceived("Jotunn Slayer");
                }
                case "AT: Catacombs Coffins Near Stairs":
                    {
                        return HasLightSource();

                    }
                case "AT: Catacombs Coffins With Blue Light":
                    {
                        return HasLightSource();

                    }
                case "AT: Corrupted Room":
                    {
                        return HasLightSource() && WasItemReceived("Corrupt Key");

                    }
                case "AT: Gated Tomb Near Corrupted Room":
                    {
                        return HasLightSource();

                    }
                case "AT: Catacombs Hidden Room":
                    {
                        return HasLightSource() && HasDustyOrb();

                    }
                case "AT: Deep Coffin Storage":
                    {
                        return HasLightSource();

                    }
                case "AT: Red Skeleton":
                    {
                        return HasLightSource() && HasElement("Light") && WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"});

                    }
                case "AT: Mausoleum Hidden Chest":
                    {
                        return HasLightSource() && HasDustyOrb();

                    }
                case "AT: Mausoleum Upper Alcove Table":
                    {
                        return HasLightSource() && HasElement("Light") && CanJumpHeight("Medium");

                    }
                case "AT: Mausoleum Maze (Early)":
                    {
                        return HasLightSource() && HasElement("Light");

                    }
                case "AT: Mausoleum Maze (Middle)":
                    {
                        return HasLightSource() && HasElement("Light");

                    }
                case "AT: Mausoleum Central Room (Right)":
                    {
                        return HasLightSource() && HasElement("Light");

                    }
                case "AT: Mausoleum Central Room (Left)":
                    {
                        return HasLightSource() && HasElement("Light");

                    }
                case "AT: Mausoleum Central Room (Back)":
                    {
                        return HasLightSource() && HasElement("Light");

                    }
                case "AT: Mausoleum Central Room (Left Path)":
                    {
                        return HasLightSource() && HasElement("Light");

                    }
                case "AT: Mausoleum Central Room (Right Path)":
                    {
                        return HasLightSource() && HasElement("Light");

                    }
                case "AT: Kill Death":
                    {
                        return HasLightSource() && HasElement("Light") && WasItemReceived("Broken Sword") && 
                        WasItemReceived("Fractured Death") && WasItemReceived("Fractured Life");

                    }
                case "AT: Tomb With Switch":
                    {
                        return HasLightSource();

                    }
                case "AT: Tomb With Sitting Corpse":
                    {
                        return HasLightSource();

                    }
                case "AT: Demi Chest":
                    {
                        return HasLightSource() && CanJumpHeight("Medium");

                    }
                case "AT: Near Light Switch":
                    {
                        return HasLightSource();

                    }
                case "AT: Hidden Room in Tomb":
                    {
                        return HasLightSource() && HasDustyOrb();

                    }
                case "AT: Hidden Chest in Tomb":
                    {
                        return HasLightSource() && HasDustyOrb();

                    }
                case "YF: Barrel Group":
                    {
                        return true;

                    }
                case "YF: Blood Pool":
                    {
                        return true;

                    }
                case "YF: Banches Within Tree":
                    {
                        return true;

                    }
                case "YF: Chest Near Tree":
                    {
                        return true;

                    }
                case "YF: Blood Plant's Insides":
                    {
                        return WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"});
                }
                case "YF: Hanging In The Trees":
                    {
                        return true;

                    }
                case "YF: Hidden Chest":
                    {
                        return HasDustyOrb() && (CanJumpHeight("Medium") || WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"}));
                }
                case "YF: Room Defended by Blood Plant":
                    {
                        return WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"});
                }
                case "YF: Patchouli's Canopy Offer":
                    {
                        return CanJumpHeight("Medium") || WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"});
                }
                case "YF: Patchouli's Reward":
                    {
                        return CanJumpHeight("Medium") || WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"}) && WasItemReceived("Skull of Josiah");
                }
                case "FC: Branch Lower Edge":
                    {
                        return true;

                    }
                case "FC: Branch Cave":
                    {
                        return true;

                    }
                case "FC: Chest":
                    {
                        return true;

                    }
                case "FC: Wooden Statue (Josiah)":
                    {
                        return true;

                    }
                case "FC: Wooden Statue (Sitting)":
                    {
                        return true;

                    }
                case "FbA: Back Room Past Bridge":
                    {
                        return true;

                    }
                case "FbA: Strange Corpse":
                    {
                        return HasDustyOrb();

                    }
                case "FbA: Short Wall Near Trees":
                    {
                        return true;

                    }
                case "FbA: Against Wall Near Trees":
                    {
                        return true;

                    }
                case "FbA: Snail Lectern (Near)":
                    {
                        return CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring");

                    }
                case "FbA: Snail Lectern (Far)":
                    {
                        return CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring");

                    }
                case "FbA: Rug on Balcony":
                    {
                        return CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring");

                    }
                case "FbA: Rooftops":
                    {
                        return CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring");

                    }
                case "FbA: Hidden Room Upper Floor":
                    {
                        return HasDustyOrb() && (CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring"));

                    }
                case "FbA: Hidden Room Lower Floor":
                    {
                        return HasDustyOrb() && (CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring") || WasItemReceived("Spirit Warp"));

                    }
                case "FbA: Near Twisty Tree":
                    {
                        return CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring") || WasItemReceived("Spirit Warp");

                    }
                case "FbA: uwu":
                    {
                        return CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring") || WasItemReceived("Spirit Warp");

                    }
                case "FbA: Daedalus Knowledge (First)":
                    {
                        return (CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring") || WasItemReceived("Spirit Warp")) && ArchipelagoClient.AP.WasItemCountReceived("Black Book", 1);
                }
                case "FbA: Daedalus Knowledge (Second)":
                    {
                        return (CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring") || WasItemReceived("Spirit Warp")) && ArchipelagoClient.AP.WasItemCountReceived("Black Book", 2);
                }
                case "FbA: Daedalus Knowledge (Third)":
                    {
                        return (CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring") || WasItemReceived("Spirit Warp")) && ArchipelagoClient.AP.WasItemCountReceived("Black Book", 3);
                }
                case "FbA: Corner Near Daedalus":
                    {
                        return CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring") || WasItemReceived("Spirit Warp");

                    }
                case "CLF: Outside Corner":
                    {
                        return true;

                    }
                case "CLF: Cattle Cell (South)":
                    {
                        return WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"});
                }
                case "CLF: Cattle Cell (West)":
                    {
                        return WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"});
                }
                case "CLF: Cattle Cell (Center)":
                    {
                        return WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"});
                }
                case "CLF: Cattle Cell (North)":
                    {
                        return WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"});
                }
                case "CLF: Hidden Cattle Cell":
                    {
                        return WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"}) && HasDustyOrb();
                }
                case "CLF: Hallway Rubble Room":
                    {
                        return ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 1);
                }
                case "CLF: Hallway Dining Room":
                    {
                        return ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 1);
                }
                case "CLF: Garrat Resting Room (Fountain)":
                    {
                        return ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 1);
                }
                case "CLF: Garrat Resting Room (Wall)":
                    {
                        return ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 1);
                }
                case "CLF: Hallway Dead End Before Blue Doors":
                    {
                        return ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 1);
                }
                case "CLF: Upper Floor Coffin Room (Small Room)":
                    {
                        return ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 2);
                }
                case "CLF: Upper Floor Coffin Room (Large Room)":
                    {
                        return ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 2);
                }
                case "CLF: Upper Floor Coffin Room (Double)":
                    {
                        return ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 3) && HasDustyOrb();
                }
                case "CLF:  Upper Floor Coffin Room (Halllway)":
                    {
                        return ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 2);
                }
                case "SB: Entry Small Room Lounge":
                    {
                        return HasDoorKey("Ballroom Side Rooms Keyring");

                    }
                case "SB: Entry Hidden Couch Top":
                    {
                        return HasDoorKey("Ballroom Side Rooms Keyring") && HasDustyOrb();

                    }
                case "SB: Entry Hidden Couch Bottom":
                    {
                        return HasDoorKey("Ballroom Side Rooms Keyring") && HasDustyOrb();

                    }
                case "SB: Entry Hidden Cave in a Lounge":
                    {
                        return HasDoorKey("Ballroom Side Rooms Keyring") && HasDustyOrb();

                    }
                case "SB: Entry Lounge Long Table":
                    {
                        return HasDoorKey("Ballroom Side Rooms Keyring");

                    }
                case "SB: Side Hidden Cave":
                    {
                        return HasDustyOrb();

                    }
                case "SB: Side Chest Near Switch":
                    {
                        return HasDoorKey("Ballroom Side Rooms Keyring");

                    }
                case "SB: Side Painting Viewing Room":
                    {
                        return HasDoorKey("Ballroom Side Rooms Keyring");

                    }
                case "SB: Side Hidden Casket Room":
                    {
                        return HasDustyOrb();

                    }
                case "SB: Side XP Drain Party Room":
                    {
                        return HasDoorKey("Ballroom Side Rooms Keyring");

                    }
                case "LC: Hidden Room":
                    {
                        return HasDustyOrb();

                    }
                case "LC: Invisible Path to Cliffside":
                    {
                        return WereAnyItemsReceived(new List<string>(){"Coffin", "Icarian Flight"});
                }
                case "GWS: Demi's Gift":
                    {
                        return true;

                    }
                case "TC: Crilall's Book Repository":
                    {
                        return true;

                    }
                case "AHB: Sngula Umbra's Remains":
                    {
                        return true;

                    }
                case "BG: Slab of a Broken Bridge":
                    {
                        return true;

                    }
                case "BG: Hidden Chest":
                    {
                        return HasDustyOrb();

                    }
                case "BG: Corpse Beneath Entrance":
                    {
                        return true;

                    }
                case "BG: Triple Hidden Chest":
                    {
                        return HasDustyOrb();

                    }
                case "BG: Lava Overseeing Dragon Switch":
                    {
                        return true;

                    }
                case "BG: Through Dragon Switch Tunnel":
                    {
                        return true;

                    }
                case "ST: Room Buried in Sand":
                    {
                        return HasSwitch("Grotto Fire Switch Keyring");

                    }
                case "ST: Top Right Sarcophagus":
                    {
                        return HasSwitch("Grotto Fire Switch Keyring");

                    }
                case "ST: Second Floor Snake Room":
                    {
                        return CanJumpHeight("Medium") && HasSwitch("Grotto Fire Switch Keyring");

                    }
                case "ST: Basement Snake Pit":
                    {
                        return CanJumpHeight("High") && HasSwitch("Grotto Fire Switch Keyring") && HasDustyOrb();

                    }
                case "ST: Hidden Sarcophagus":
                    {
                        return HasDustyOrb() && HasSwitch("Grotto Fire Switch Keyring");

                    }
                case "ST: Second Floor Dead End":
                    {
                        return CanJumpHeight("Medium") && HasSwitch("Grotto Fire Switch Keyring");

                    }
                case "ST: Lunacid Sandwich":
                    {
                        return WasItemReceived("Spirit Warp") && HasSwitch("Grotto Fire Switch Keyring");

                    }
                case "ST: Chest Near Switch":
                    {
                        return HasSwitch("Grotto Fire Switch Keyring");

                    }
                case "ST: Chest Overlooking Crypt":
                    {
                        return CanJumpHeight("High") && HasSwitch("Grotto Fire Switch Keyring");

                    }
                case "ST: Floor Switch Maze":
                    {
                        return HasSwitch("Grotto Fire Switch Keyring");

                    }
                case "ST: Basement Stone Rubble":
                    {
                        return HasSwitch("Grotto Fire Switch Keyring");

                    }
                case "ST: Triple Sarcophagus":
                    {
                        return HasSwitch("Grotto Fire Switch Keyring");

                    }
                case "TA: Floor 5 Chest":
                    {
                        return HasDoorKey("Tower of Abyss Keyring");

                    }
                case "TA: Floor 10 Chest":
                    {
                        return HasDoorKey("Tower of Abyss Keyring");

                    }
                case "TA: Floor 15 Chest":
                    {
                        return HasDoorKey("Tower of Abyss Keyring");

                    }
                case "TA: Floor 20 Chest":
                    {
                        return HasDoorKey("Tower of Abyss Keyring");

                    }
                case "TA: Floor 25 Chest":
                    {
                        return HasDoorKey("Tower of Abyss Keyring");

                    }
                case "TA: Floor 30 Chest":
                    {
                        return HasDoorKey("Tower of Abyss Keyring");

                    }
                case "TA: Floor 35 Chest":
                    {
                        return HasDoorKey("Tower of Abyss Keyring");

                    }
                case "TA: Floor 40 Chest":
                    {
                        return HasDoorKey("Tower of Abyss Keyring");

                    }
                case "TA: Floor 45 Chest":
                    {
                        return HasDoorKey("Tower of Abyss Keyring");

                    }
                case "TA: Floor 50 Chest":
                    {
                        return HasDoorKey("Tower of Abyss Keyring");

                    }
                case "TA: Prize Beneath Tree":
                    {
                        return HasDoorKey("Tower of Abyss Keyring");

                    }
                case "TP: Third Floor Locked Cell Left":
                    {
                        return WasItemReceived("Terminus Prison Key");

                    }
                case "TP: Third Floor Locked Cell Right":
                    {
                        return WasItemReceived("Terminus Prison Key");

                    }
                case "TP: Third Floor Locked Cell South":
                    {
                        return WasItemReceived("Terminus Prison Key");

                    }
                case "TP: Almost Bottomless Pit":
                    {
                        return WasItemReceived("Terminus Prison Key") && 
                        WereAnyItemsReceived(new List<string>(){"Spirit Warp", "Icarian Flight"});
                }
                case "TP: Second Floor Broken Cell":
                    {
                        return true;

                    }
                case "TP: Second Floor Jailer's Table":
                    {
                        return true;

                    }
                case "TP: First Floor Hidden Cell":
                    {
                        return HasDustyOrb() && HasLightSource();

                    }
                case "TP: First Floor Hidden Debris Room":
                    {
                        return HasDustyOrb() && HasLightSource();

                    }
                case "TP: First Floor Remains":
                    {
                        return HasLightSource();

                    }
                case "TP: Green Asylum Guarded Alcove (Left)":
                    {
                        return HasLightSource();

                    }
                case "TP: Green Asylum Guarded Alcove (Right)":
                    {
                        return HasLightSource();

                    }
                case "TP: Green Asylum Long Alcove":
                    {
                        return HasLightSource();

                    }
                case "TP: Green Asylum Bone Pit":
                    {
                        return HasLightSource();

                    }
                case "TP: Egg's Resting Place":
                    {
                        return HasLightSource() && WasItemReceived("Skeleton Egg");

                    }
                case "TP: Fourth Floor Cell Hanging Remains":
                    {
                        return WasItemReceived("Terminus Prison Key");

                    }
                case "TP: Fourth Floor Maledictus Secret":
                    {
                        return WasItemReceived("Terminus Prison Key") && HasDustyOrb();

                    }
                case "TP: Fourth Floor Hidden Jailer Sleeping Spot":
                    {
                        return WasItemReceived("Terminus Prison Key") && HasDustyOrb();

                    }
                case "TP: Fourth Floor Jailer Break Room":
                    {
                        return WasItemReceived("Terminus Prison Key");

                    }
                case "TP: Etna's Resting Place Item 1":
                    {
                        return WasItemReceived("Terminus Prison Key");

                    }
                case "TP: Etna's Resting Place Item 2":
                    {
                        return WasItemReceived("Terminus Prison Key");

                    }
                case "TP: Etna's Resting Place Item 3":
                    {
                        return WasItemReceived("Terminus Prison Key");

                    }
                case "TP: Fourth Floor Collapsed Tunnel":
                    {
                        return WasItemReceived("Terminus Prison Key");

                    }
                case "FlA: Corpse Waiting For A Full Moon":
                    {
                        return true;

                    }
                case "FlA: Entry Rock Parkour":
                    {
                        return true;

                    }
                case "FlA: Temple of Earth Hidden Plant Haven":
                    {
                        return HasDustyOrb();

                    }
                case "FlA: Temple of Earth Hidden Room":
                    {
                        return HasDustyOrb();

                    }
                case "FlA: Temple of Earth Fractured Chest":
                    {
                        return HasDustyOrb() && CanJumpHeight("High");

                    }
                case "FlA: Temple of Earth Chest Near Switch":
                    {
                        return true;

                    }
                case "FlA: Temple of Water Room Near Water":
                    {
                        return true;

                    }
                case "FlA: Temple of Water Corner Near Water":
                    {
                        return true;

                    }
                case "FlA: Temple of Water Collapsed End Near Balcony":
                    {
                        return true;

                    }
                case "FlA: Temple of Water Hidden Basement (Left)":
                    {
                        return HasDustyOrb();

                    }
                case "FlA: Temple of Water Hidden Basement (Right)":
                    {
                        return HasDustyOrb();

                    }
                case "FlA: Temple of Water Hidden Laser Room":
                    {
                        return HasDustyOrb();

                    }
                case "FlA: Temple of Water Hidden Alcove Before Stairs":
                    {
                        return HasDustyOrb();

                    }
                case "FlA: Temple of Water Hidden Alcove (Left)":
                    {
                        return HasDustyOrb();

                    }
                case "FlA: Temple of Water Hidden Alcove (Right)":
                    {
                        return HasDustyOrb();

                    }
                case "FlA: Temple of Water Hidden Alcove Before Switch":
                    {
                        return HasDustyOrb();

                    }
                case "FlA: Temple of Water Fractured Chest":
                    {
                        return HasDustyOrb() && CanJumpHeight("High");

                    }
                case "FlA: Temple of Water Chest Near Switch":
                    {
                        return true;

                    }
                case "LA: Entry Coffin":
                    {
                        return true;

                    }
                case "LA: Giant Remains":
                    {
                        return true;

                    }
                case "LA: Behind Statue":
                    {
                        return true;

                    }
                case "LA: Rocks Near Switch":
                    {
                        return true;

                    }
                case "LA: Forbidden Light Chest":
                    {
                        return true;

                    }
                case "LA: Hidden Light Stash":
                    {
                        return HasDustyOrb();

                    }
                case "LA: NNSNSSNSNN Lost Maze":
                    {
                        return HasDustyOrb();

                    }
                case "CF: Calamis' Weapon of Choice":
                    {
                        return true;

                    }
                case "Buy Enchanted Key":
                    {
                        return true;

                    }
                case "Buy Rapier":
                    {
                        return WasItemReceived("Sheryl's Initial Offerings Voucher");

                    }
                case "Buy Steel Needle":
                    {
                        return WasItemReceived("Sheryl's Initial Offerings Voucher");
                    }
                case "Buy Crossbow":
                    {
                        return WasItemReceived("Sheryl's Initial Offerings Voucher");

                    }
                case "Buy Oil Lantern":
                    {
                        return WasItemReceived("Ignis Calor") && ConnectionData.EnteredScenes.Contains("Boiling Grotto") && WasItemReceived("Sheryl's Golden Armor Voucher");
                    }
                case "Buy Ocean Elixir (Patchouli)":
                    {
                        return (CanJumpHeight("Medium") || WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"})) && WasItemReceived("Patchouli's Drink Voucher");;

                    }
                case "Buy Privateer Musket":
                    {
                        return WasItemReceived("Ignis Calor") && ConnectionData.EnteredScenes.Contains("Boiling Grotto") && WasItemReceived("Sheryl's Golden Armor Voucher");
                    }
                case "Buy Jotunn Slayer":
                    {
                        return WasItemReceived("Sheryl's Dreamer Voucher") && CanDefeatSucasrius();
                    }
            }
            var firstBit = location.Split(':')[0];
            if (LunacidEnemies.EnemyNames.Contains(firstBit))
            {
                return ObtainDropLogic(firstBit, sceneName);
            }
            return false;
        }

        private bool ObtainDropLogic(string enemyName, string sceneName)
        {
            switch (enemyName)
            {
                case "Snail":
                    {
                        return true;
                    }
                case "Milk Snail":
                    {
                        if (sceneName == "ARCHIVES")
                        {
                            return CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring");
                        }
                        return true;
                    }
                case "Mummy":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key");
                    }
                case "Mummy Knight":
                    {
                        return CanEnterTemple() && HasSwitch("Temple of Silence Switch Key");
                    }
                case "Shulker":
                    {
                        return true;
                    }
                case "Necronomicon":
                    {
                        return true;
                    }
                case "Chimera":
                    {
                        return CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring");
                    }
                case "Enlightened One":
                    {
                        return CanJumpHeight("High") || HasSwitch("Forbidden Archives Elevator Switch Keyring");
                    }
                case "Slime Skeleton":
                    {
                        return true;
                    }
                case "Skeleton":
                    {
                        if (sceneName == "HAUNT")
                        {
                            return (CanJumpHeight("Medium") || HasElement("Light")) && HasLightSource();
                        }
                        return true;
                    }
                case "Rat King":
                    {
                        return true;
                    }
                case "Rat Queen":
                    {
                        return true;
                    }
                case "Rat":
                    {
                        return true;
                    }
                case "Kodama":
                    {
                        return true;
                    }
                case "Yakul":
                    {
                        return true;
                    }
                case "Venus":
                    {
                        if (sceneName == "ARENA")
                        {
                            return HasDustyOrb();
                        }
                        return true;
                    }
                case "Neptune":
                    {
                        if (sceneName == "FOREST_B1")
                        {
                            return true;
                        }
                        return CanJumpHeight("Medium") || WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"});
                    }
                case "Unilateralis":
                    {
                        return true;
                    }
                case "Mimic":
                    {
                        return HasDustyOrb();
                    }
                case "Hemalith":
                    {
                        return true;
                    }
                case "Mare":
                    {
                        return HasLightSource();
                    }
                case "Mi-Go":
                    {
                        return HasLightSource();
                    }
                case "Phantom":
                    {
                        if (sceneName == "HAUNT")
                        {
                            return HasLightSource() && HasElement("Light");
                        }
                        return true;
                    }
                case "Cursed Painting":
                    {
                        if (sceneName == "HAUNT")
                        {
                            return HasLightSource();
                        }
                        return ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 2);
                    }
                case "Malformed":
                    {
                        return (WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"}) && HasDustyOrb()) || 
                        ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 2);
                    }
                case "Poltergeist":
                    {
                        return ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 2);
                    }
                case "Great Bat":
                    {
                        return IsVampire() || ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 1);
                    }
                case "Vampire":
                    {
                        return IsVampire() || WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"});
                    }
                case "Vampire Page":
                    {
                        return IsVampire() || WereAnyItemsReceived(new List<string>(){"Blood Strike", "Blood Drain"}) || 
                        ArchipelagoClient.AP.WasItemCountReceived("Progressive Vampiric Symbol", 1);
                    }
                case "Malformed Horse":
                    {
                        return true;
                    }
                case "Hallowed Husk":
                    {
                        return true;
                    }
                case "Ikurr'Ilb":
                    {
                        return true;
                    }
                case "Obsidian Skeleton":
                    {
                        if (sceneName == "Prison")
                        {
                            return HasLightSource();
                        }
                        return true;
                    }
                case "Serpent":
                    {
                        return HasSwitch("Grotto Fire Switch Keyring");
                    }
                case "Anpu":
                    {
                        return HasSwitch("Grotto Fire Switch Keyring");
                    }
                case "Embalmed":
                    {
                        return HasSwitch("Grotto Fire Switch Keyring");
                    }
                case "Cerritulus Lunam":
                    {
                        return HasLightSource();
                    }
                case "Jailor":
                    {
                        return true;
                    }
                case "Lupine Skeleton":
                    {
                        return HasLightSource() || WasItemReceived("Terminus Prison Key");
                    }
                case "Giant Skeleton":
                    {
                        return HasLightSource();
                    }
                case "Sucsarian":
                    {
                        return true;
                    }
                case "Ceres":
                    {
                        return CanJumpHeight("High");
                    }
                case "Vesta":
                    {
                        return CanJumpHeight("High");
                    }
                case "Gloom Wood":
                    {
                        return true;
                    }
                case "Cetea":
                    {
                        return true;
                    }
                case "Abyssal Demon":
                    {
                        if (sceneName == "HAUNT")
                        {
                            return HasLightSource() && HasElement("Light");
                        }
                        return true;
                    }
            }
            return false;
        }


        private static readonly List<string> QuestionableLocations = new(){
            "YF: Hanging In The Trees"
        };

        private static bool IsLocationQuestionable(string location)
        {
            return QuestionableLocations.Contains(location);
        }

        public string ColorLogicLocation(string location, string sceneName)
        {
            var logic = LogicCases(location, sceneName);
            if (IsLocationQuestionable(location))
            {
                return "#FFA500";
            }
            if (logic)
            {
                return "#00FF00";
            }
            return "#FF0000";
        }

        private bool CanBuyInitialShopItems()
        {
            var allowedScenesForMoney = new List<string>(){"Boiling Grotto", "Accursed Tomb", "Forbidden Archives"};
            foreach (var scene in allowedScenesForMoney)
            {
                if (ConnectionData.EnteredScenes.Contains(scene))
                return true;
            }
            return false;
        }

        private bool IsVampire()
        {
            return ArchipelagoClient.AP.SlotData.StartingClass == 3;
        }

        private bool CanDefeatSucasrius()
        {
            var hasTalismans = WasItemReceived("Water Talisman") && WasItemReceived("Earth Talisman");
            if (ArchipelagoClient.AP.SlotData.Doorlock)
            {
                return hasTalismans && WasItemReceived("Sucsarian Key");
            }
            return hasTalismans;
        }

        private bool CanEnterTemple()
        {
            return (!ArchipelagoClient.AP.SlotData.Shopsanity || ArchipelagoClient.AP.WasItemCountReceived("Enchanted Key", 1)) && HasLightSource();
        }

        private bool HasDustyOrb()
        {
            return !ArchipelagoClient.AP.SlotData.FalseWalls || ArchipelagoClient.AP.WasItemReceived("Dusty Crystal Orb");
        }

        private bool HasLightSource()
        {
            return WereAnyItemsReceived(new List<string>() { "Torch", "Crystal Lantern", "Oil Lantern", "Flame Flare", "Ghost Light", "Twisted Staff", "Moonlight", "Broken Hilt" });
        }

        private bool HasElement(string element)
        {
            if (WasItemReceived("Wand of Power"))
            {
                return true;
            }
            foreach (var weapon in ConnectionData.Elements)
            {
                if (weapon.Value.Contains(element))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CanJumpHeight(string height)
        {
            switch (height)
            {
                case "High":
                    return WereAnyItemsReceived(new List<string>() { "Icarian Flight", "Rock Bridge", "Barrier" });

                case "Medium":
                    return WereAnyItemsReceived(new List<string>() { "Icarian Flight", "Rock Bridge", "Barrier", "Coffin", "Summon Snail" });
            }
            return true;
        }

        private bool HasSwitch(string item)
        {
            return !ArchipelagoClient.AP.SlotData.Switchlock || ArchipelagoClient.AP.WasItemReceived(item);
        }

        private bool HasDoorKey(string item)
        {
            return !ArchipelagoClient.AP.SlotData.Doorlock || ArchipelagoClient.AP.WasItemReceived(item);
        }

        private bool WasItemReceived(string item)
        {
            return ArchipelagoClient.AP.WasItemReceived(item);
        }

        private bool WereAllItemsReceived(List<string> items)
        {
            var result = true;
            foreach (var item in items)
            {
                result = result && ArchipelagoClient.AP.WasItemReceived(item);
            }
            return result;
        }

        private bool WereAnyItemsReceived(List<string> items)
        {
            var result = false;
            foreach (var item in items)
            {
                result = result || ArchipelagoClient.AP.WasItemReceived(item);
            }
            return result;
        }
    }
}