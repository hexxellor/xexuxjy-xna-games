using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;


//namespace Gladius
//{
public static class ActorGenerator
{
    static ActorGenerator()
    {
        InitClassCategories();
        LoadAllStatsFiles();
        LoadAllItemsFiles();
        LoadAllSkillsFiles();
        LoadAllGladiators();
        int ibreak = 0;
    }

    public static void Test()
    {

    }

    
    public static void InitClassCategories()
    {

        ClassDataMap[ActorClass.Amazon] = new ActorClassData(ActorClass.Amazon, "Amazon", "amazon", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Support));
        ClassDataMap[ActorClass.Archer] = new ActorClassData(ActorClass.Archer, "Archer", "archer", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Support));
        ClassDataMap[ActorClass.ArcherF] = new ActorClassData(ActorClass.ArcherF, "Archer", "archerF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Support));
        ClassDataMap[ActorClass.BanditA] = new ActorClassData(ActorClass.BanditA, "Bandit", "banditA", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BanditAF] = new ActorClassData(ActorClass.BanditAF, "Bandit", "banditAF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BanditB] = new ActorClassData(ActorClass.BanditB, "Bandit", "banditB", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.Barbarian] = new ActorClassData(ActorClass.Barbarian, "Barbarian", "barbarian", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BarbarianF] = new ActorClassData(ActorClass.BarbarianF, "Barbarian", "barbarianF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.Bear] = new ActorClassData(ActorClass.Bear, "Bear", "bear", (ActorClassAttributes.Beast | ActorClassAttributes.Heavy | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BearGreater] = new ActorClassData(ActorClass.BearGreater, "Bear", "beargreater", (ActorClassAttributes.Beast | ActorClassAttributes.Heavy | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.Berserker] = new ActorClassData(ActorClass.Berserker, "Berserker", "berserker", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BerserkerF] = new ActorClassData(ActorClass.BerserkerF, "Berserker", "berserkerf", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.Cat] = new ActorClassData(ActorClass.Cat, "Cat", "cat", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.CatGreater] = new ActorClassData(ActorClass.CatGreater, "Cat", "catgreater", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.Centurion] = new ActorClassData(ActorClass.Centurion, "Centurion", "centurion", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.CenturionF] = new ActorClassData(ActorClass.CenturionF, "Centurion", "centurionF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.ChannelerImp] = new ActorClassData(ActorClass.ChannelerImp, "Channeler", "channelerImp", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Arcane | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.Cyclops] = new ActorClassData(ActorClass.Cyclops, "Cyclops", "cyclops", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.CyclopsGreater] = new ActorClassData(ActorClass.CyclopsGreater, "Cyclops", "cyclopsgreater", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.DarkLegionnaire] = new ActorClassData(ActorClass.DarkLegionnaire, "DarkLegionnaire", "legionnaireDark", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.Dervish] = new ActorClassData(ActorClass.Dervish, "Dervish", "dervish", (ActorClassAttributes.Male | ActorClassAttributes.Light | ActorClassAttributes.Expanse | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.DervishF] = new ActorClassData(ActorClass.DervishF, "Dervish", "dervishF", (ActorClassAttributes.Female | ActorClassAttributes.Light | ActorClassAttributes.Expanse | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.Dummy] = new ActorClassData(ActorClass.Dummy, "Dummy", "prop_practicepost1", (ActorClassAttributes.Male | ActorClassAttributes.Light));


        ClassDataMap[ActorClass.Eiji] = new ActorClassData(ActorClass.Eiji, "Eiji", "eiji", (ActorClassAttributes.Female | ActorClassAttributes.Support | ActorClassAttributes.Steppes | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.Galdr] = new ActorClassData(ActorClass.Galdr, "Galdr", "galdr", (ActorClassAttributes.Female | ActorClassAttributes.Arcane | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.Galverg] = new ActorClassData(ActorClass.Galverg, "Galverg", "galverg", (ActorClassAttributes.Male | ActorClassAttributes.Heavy | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.Gungnir] = new ActorClassData(ActorClass.Gungnir, "Gungir", "gungir", (ActorClassAttributes.Male | ActorClassAttributes.Support | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.GungnirF] = new ActorClassData(ActorClass.GungnirF, "Gungir", "gungirF", (ActorClassAttributes.Female | ActorClassAttributes.Support | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.Gwazi] = new ActorClassData(ActorClass.Gwazi, "Gwazi", "gwazi", (ActorClassAttributes.Male | ActorClassAttributes.Light | ActorClassAttributes.Expanse | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.Legionnaire] = new ActorClassData(ActorClass.Legionnaire, "Legionnarie", "legionnaire", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.LegionnaireF] = new ActorClassData(ActorClass.LegionnaireF, "Legionnarie", "legionanaireF", (ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.Ludo] = new ActorClassData(ActorClass.Ludo, "Ludo", "ludo", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.Minotaur] = new ActorClassData(ActorClass.Minotaur, "Minotaur", "minotaur", (ActorClassAttributes.Beast | ActorClassAttributes.Heavy));
        ClassDataMap[ActorClass.Mongrel] = new ActorClassData(ActorClass.Mongrel, "Mongrel", "mongrel", (ActorClassAttributes.Male | ActorClassAttributes.Light | ActorClassAttributes.Nordargh | ActorClassAttributes.Beast));
        ClassDataMap[ActorClass.MongrelShaman] = new ActorClassData(ActorClass.MongrelShaman, "MongrelShaman", "mongrelshaman", (ActorClassAttributes.Male | ActorClassAttributes.Arcane | ActorClassAttributes.Nordargh | ActorClassAttributes.Beast));
        ClassDataMap[ActorClass.Murmillo] = new ActorClassData(ActorClass.Murmillo, "Murmillo", "murmillo", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.MurmilloF] = new ActorClassData(ActorClass.MurmilloF, "Murmillo", "murmilloF", (ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.Mutuus] = new ActorClassData(ActorClass.Mutuus, "Mutuus", "mutuus", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.Ogre] = new ActorClassData(ActorClass.Ogre, "Ogre", "ogre", (ActorClassAttributes.Beast | ActorClassAttributes.Heavy));
        ClassDataMap[ActorClass.Peltast] = new ActorClassData(ActorClass.Peltast, "Peltast", "peltast", (ActorClassAttributes.Male | ActorClassAttributes.Support | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.PeltastF] = new ActorClassData(ActorClass.PeltastF, "Peltast", "peltastF", (ActorClassAttributes.Female | ActorClassAttributes.Support | ActorClassAttributes.Imperia | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.SamniteExp] = new ActorClassData(ActorClass.SamniteExp, "Samnite", "samniteexp", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Expanse));
        ClassDataMap[ActorClass.SamniteExpF] = new ActorClassData(ActorClass.SamniteExpF, "Samnite", "samniteexpF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Expanse));
        ClassDataMap[ActorClass.SamniteImp] = new ActorClassData(ActorClass.SamniteImp, "Samnite", "samniteimp", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.SamniteImpF] = new ActorClassData(ActorClass.SamniteImpF, "Samnite", "samniteimpF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.SamniteSte] = new ActorClassData(ActorClass.SamniteSte, "Samnite", "samniteste", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Steppes));
        ClassDataMap[ActorClass.SamniteSteF] = new ActorClassData(ActorClass.SamniteSteF, "Samnite", "samnitesteF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Steppes));

        ClassDataMap[ActorClass.Satyr] = new ActorClassData(ActorClass.Satyr, "Satyr", "satyr", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Arcane));
        ClassDataMap[ActorClass.Scarab] = new ActorClassData(ActorClass.Scarab, "Scarab", "scarab", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.Scorpion] = new ActorClassData(ActorClass.Scorpion, "Scorpion", "scorpion", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.SecutorImp] = new ActorClassData(ActorClass.SecutorImp, "Secutor", "secutorimp", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.SecutorImpF] = new ActorClassData(ActorClass.SecutorImpF, "Secutor", "secutorimpF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.SecutorSte] = new ActorClassData(ActorClass.SecutorSte, "Secutor", "secutorste", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Steppes));
        ClassDataMap[ActorClass.SecutorSteF] = new ActorClassData(ActorClass.SecutorSteF, "Secutor", "secutorsteF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Steppes));
        ClassDataMap[ActorClass.Summoner] = new ActorClassData(ActorClass.Summoner, "Summoner", "summoner", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Arcane));
        ClassDataMap[ActorClass.UndeadMeleeImpA] = new ActorClassData(ActorClass.UndeadMeleeImpA, "UndeadLegionnaire", "skeletonimp2", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.UndeadCasterA] = new ActorClassData(ActorClass.UndeadCasterA, "UndeadSummoner", "skeletonimp2", (ActorClassAttributes.Male | ActorClassAttributes.Arcane));
        ClassDataMap[ActorClass.Urlan] = new ActorClassData(ActorClass.Urlan, "Urlan", "urlan", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.UrsulaCostumeA] = new ActorClassData(ActorClass.UrsulaCostumeA, "Ursula", "ursula", (ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.UrsulaCostumeB] = new ActorClassData(ActorClass.UrsulaCostumeB, "Ursula", "ursula", (ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.Ursula] = new ActorClassData(ActorClass.Ursula, "Ursula", "ursula", (ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.ValensCostumeA] = new ActorClassData(ActorClass.ValensCostumeA, "Valens", "valens", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.ValensCostumeB] = new ActorClassData(ActorClass.ValensCostumeB, "Valens", "valens", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.Valens] = new ActorClassData(ActorClass.Valens, "Valens", "valens", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.Wolf] = new ActorClassData(ActorClass.Wolf, "Wolf", "wolf", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Light));
        ClassDataMap[ActorClass.WolfGreater] = new ActorClassData(ActorClass.WolfGreater, "Wolf", "wolfgreater", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Light));
        ClassDataMap[ActorClass.Yeti] = new ActorClassData(ActorClass.Yeti, "Yeti", "yeti", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Heavy | ActorClassAttributes.Nordargh));
    }

    public static bool CheckLevelUp(CharacterData characterData, int xpGained)
    {
        return false;
    }

    public static CharacterData CreateRandomCharacterForLevel(CharacterData characterData)
    {
        // choose a class based on mask.
        List<ActorClassData> validClasses = new List<ActorClassData>();
        foreach (ActorClassData actorClassData in ClassDataMap.Values)
        {
            if ((((int)actorClassData.Mask) & characterData.RequiredMask) != 0)
            {
                validClasses.Add(actorClassData);
            }
        }
        // pick one at random?
        int randomChar = UnityEngine.Random.Range(0, validClasses.Count);
        ActorClassData classData = validClasses[randomChar];
        characterData.ActorClass = classData.ActorClass;

        String skillSetName = "";
        StatsSet coreStat = StatForClassLevel(characterData.ActorClassData.Name, skillSetName, characterData.Level);
        characterData.CopyModCoreStat(coreStat);


        return characterData;

    }


    public static CharacterData ParseUNITDB(String[] tokens)
    {
        CharacterData characterData = new CharacterData();
        // skip first token (unitdb)
        int counter = 0;
        String name = tokens[counter++];
        int val1 = int.Parse(tokens[counter++]);
        String startSlot = tokens[counter++];

        characterData.MinLevel = int.Parse(tokens[counter++]);
        characterData.MaxLevel = int.Parse(tokens[counter++]);

        int val4 = int.Parse(tokens[counter++]);

        int level = -1; // base of player level?u

        if (characterData.MinLevel > 0 && characterData.MaxLevel > 0)
        {
            level = GladiusGlobals.Random.Next(characterData.MinLevel, characterData.MaxLevel);
        }


        characterData.Level = level;
        characterData.Name = name;

        // seems to be a required mask
        // 0 = mongel? nah
        // 1 = light
        // 2 = legionnaire
        // 4 - centurion?  or heavy?
        // 8 = arcane
        // 16 = support required
        // 24 = prohibited arcane + support
        // 32 = beast
        // 56 = prohibited beast, required arcane + support
        // 64 = male
        // 128 = female
        // heavy = 56 (32+16+8)
        // 256 = Nordargh only?
        // 512 = Imperia only
        // 4096 = human
        // 4160 = human male        4096+64
        // 4224 = human female      4096+128

        int mask1 = int.Parse(tokens[counter++]);
        characterData.RequiredMask = mask1;
        // affinity requirement
        // 5,2,1,4  ?? 
        // 1  = earth
        // 2 = water
        // 3 = light
        // 4 = air
        // 5 = fire
        // 6 = dark


        DamageType damageType = DamageType.Physical;
        int affinityRequirement = int.Parse(tokens[counter++]);
        if (affinityRequirement != -1)
        {
            switch (affinityRequirement)
            {
                case 1:
                    damageType = DamageType.Earth;
                    break;
                case 2:
                    damageType = DamageType.Water;
                    break;
                case 3:
                    damageType = DamageType.Air;
                    break;
                case 4:
                    damageType = DamageType.Fire;
                    break;
                default:
                    damageType = DamageType.Physical;
                    break;
            }
        }


        String class1 = tokens[counter++];
        String class2 = tokens[counter++];
        String class3 = tokens[counter++];
        String class4 = tokens[counter++];
        int val7 = int.Parse(tokens[counter++]);
        int requiredProhbited = int.Parse(tokens[counter++]);

        HashSet<String> allowedClasses = new HashSet<string>();
        allowedClasses.Add(class1);
        allowedClasses.Add(class2);
        allowedClasses.Add(class3);
        allowedClasses.Add(class4);

        int val9 = int.Parse(tokens[counter++]);
        int val10 = int.Parse(tokens[counter++]);
        int val11 = int.Parse(tokens[counter++]);
        int val12 = int.Parse(tokens[counter++]);

        // character data can act as an empty slot saying whats allowed , as well as being an actual character slot?
        String className = characterData.ActorClassData.Name;
        string skillSetName = "";
        StatsSet statData = StatForClassLevel(className, skillSetName, level);
        if (statData != null)
        {
            characterData.CON = statData.CON;
            characterData.PWR = statData.PWR;
            characterData.ACC = statData.ACC;
            characterData.DEF = statData.DEF;
            characterData.INI = statData.INI;
            characterData.MOV = statData.MOV;
        }

        return characterData;
    }

    public static void LoadAllStatsFiles()
    {

        TextAsset[] allFiles = Resources.LoadAll<TextAsset>(GladiusGlobals.DataRoot + "StatFiles");
        foreach (TextAsset file in allFiles)
        {
            try
            {
                String[] lines = file.text.Split('\n');
                LoadStatsFile(lines);
            }
            catch (Exception e)
            {

            }
        }
    }


    public static void LoadStatsFile(String[] fileData)
    {
        if (fileData.Length > 0)
        {
            String className = "unk";
            StatsSetBlock setBlock = new StatsSetBlock(fileData[0]);
            List<String> shortList = new List<String>();
            char[] splitTokens = new char[] { ' ', ',' };
            for (int i = 1; i < fileData.Length; ++i)
            {
                if (fileData[i].StartsWith("MODCORESTATSCOMP:"))
                {
                    String[] tokens = fileData[i].Split(splitTokens);
                    shortList.Clear();
                    foreach (String token in tokens)
                    {
                        if (!String.IsNullOrEmpty(token))
                        {
                            shortList.Add(token);
                        }
                    }
                    int counter = 0;

                    String pad = shortList[counter++];
                    String variant = shortList[counter++];
                    String level = shortList[counter++];
                    String con = shortList[counter++];
                    String pow = shortList[counter++];
                    String acc = shortList[counter++];
                    String def = shortList[counter++];
                    String ini = shortList[counter++];
                    String mov = shortList[counter++];

                    StatsSet item = new StatsSet();
                    item.ClassName = className;
                    item.VariantName = variant;
                    item.Level = int.Parse(level);
                    item.CON = int.Parse(con);
                    item.PWR = int.Parse(pow);
                    item.ACC = int.Parse(acc);
                    item.DEF = int.Parse(def);
                    item.INI = int.Parse(ini);
                    item.MOV = float.Parse(mov);

                    setBlock.AddItem(item);

                    int ibreak = 0;
                }
            }
        }
    }

    public static StatsSet StatForClassLevel(string className, string skillsetName, int level)
    {
        StatsSet set = null;
        StatsSetBlock setBlock = null;
        if (ClassStatsMap.TryGetValue(className, out setBlock))
        {
            List<StatsSet> statRow = null;
            if (setBlock.SubBlockMap.TryGetValue(skillsetName, out statRow))
            {
                if (level < statRow.Count)
                {
                    set = statRow[level - 1];
                }
            }

        }
        return set;
    }

    public static ItemSet ItemsForClassLevel(string className, string skillsetName, int level)
    {
        ItemSet set = null;
        ItemSetBlock setBlock = null;
        if (ClassItemsMap.TryGetValue(className, out setBlock))
        {
            List<ItemSet> statRow = null;
            if (setBlock.SubBlockMap.TryGetValue(skillsetName, out statRow))
            {
                if (level < statRow.Count)
                {
                    set = statRow[level - 1];
                }
            }

        }
        return set;
    }

    public static List<SkillSetItem> SkillSetForClassLevel(string className, string skillsetName, int level)
    {
        List<SkillSetItem> setList = null;
        SkillSetBlock setBlock = null;
        if (ClassSkillsMap.TryGetValue(className, out setBlock))
        {
            if (setBlock.SubBlockMap.TryGetValue(skillsetName, out setList))
            {
            }
            else
            {
            }

        }
        return setList;
    }


    /*
//name, class, customize info, stats, items, skills, school
NUMUNITS: 2173
"","","","","","",""
"Aaden","BanditAF","4#School_RedYellow1","Statset0","ItemSetNE","skillsetaffE","Fliuch Falcons"
"Aapehty","BeastEarth","","StatSet0","ItemSetEE","skillsetSumEarth","The Fearsome Foursome"
"Abagai","Amazon","2#","StatSet3","ItemSetSU","SkillSetComb","WildernessSteppesOutlaw"
"Abbah","BanditB","3#","StatSet0","ItemSetEA","SkillSetAffA","WildernessExpanseOutlaw"
     */

    public static void LoadAllGladiators()
    {
        TextAsset data = Resources.Load<TextAsset>(GladiusGlobals.DataRoot + "GladiatorNames");
        string[] lines = data.text.Split('\n');
        // data starts on the 3rd line
        for (int counter = 3; counter < lines.Length; counter++)
        {
            string line = lines[counter];

            string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ',' });
            if (lineTokens.Length == 7)
            {
                GladiatorData gladiatorData = new GladiatorData();
                gladiatorData.name = lineTokens[0];
                gladiatorData.className = lineTokens[1];
                gladiatorData.customizeInfo = lineTokens[2];
                gladiatorData.statSetName = lineTokens[3];
                gladiatorData.itemSetName = lineTokens[4];
                gladiatorData.skillSetName = lineTokens[5];
                gladiatorData.schoolName = lineTokens[6];

                List<GladiatorData> gladiators = null;
                if (!SchoolsAndGladiators.TryGetValue(gladiatorData.schoolName, out gladiators))
                {
                    gladiators = new List<GladiatorData>();
                    SchoolsAndGladiators[gladiatorData.schoolName] = gladiators;
                }
                gladiators.Add(gladiatorData);

            }
            else
            {
                // error
                int ibreak = 0;
            }
        }
    }


    public static CharacterData FromGladiatorData(GladiatorData gd)
    {
        // find the mod stat;
        StatsSet statData = StatForClassLevel(gd.className, gd.statSetName, gd.level);
        ItemSet itemData = ItemsForClassLevel(gd.className, gd.statSetName, gd.level);
        List<SkillSetItem> skillSetList = SkillSetForClassLevel(gd.className, gd.statSetName, gd.level);

        CharacterData cd = new CharacterData();
        cd.Name = gd.name;
        cd.TeamName = gd.schoolName;
        statData.Apply(cd);
        itemData.Apply(cd);

        foreach (SkillSetItem skill in skillSetList)
        {
            cd.AddSkill(skill.SkillName);
        }
        return cd;
    }


    public static void LoadAllItemsFiles()
    {

        TextAsset[] allFiles = Resources.LoadAll<TextAsset>(GladiusGlobals.DataRoot + "ItemFiles");
        foreach (TextAsset file in allFiles)
        {
            try
            {
                String[] lines = file.text.Split('\n');
                ItemSetBlock setBlock = LoadItemsFile(lines);
                ClassItemsMap[setBlock.MainClass] = setBlock;
            }
            catch (Exception e)
            {

            }

        }
    }

    public static ItemSetBlock LoadItemsFile(String[] lines)
    {
        ////// ITEMSET export Class: Galverg Galverg  Region: Imperia Affinity: Earth
        //          :      variant, Lv,Lv,                          weapon,                           armor,                          shield,                          helmet             accessory
        //MODITEMSCOMP:    itemsetIE,  1, 1,                              "",                              "",                              "",                              "",                              "" 	//, Cost, 000000,000000,Acc,Def,0000,0000
        //MODITEMSCOMP:    itemsetIE,  2, 2,                              "",                              "",                              "",                              "",                 "Earth Berkana" 	//, Cost, 001188,001188,Acc,Def,0000,0000


        ItemSetBlock currentItemSetBlock = new ItemSetBlock(lines[0]);
        foreach (String line in lines)
        {
            if (line.StartsWith("MODITEMSCOMP:"))
            {

                string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ',', ':' });
                if (lineTokens.Length == 9)
                {
                    ItemSet modItemStat = new ItemSet(lineTokens);
                    currentItemSetBlock.AddItem(modItemStat);
                }

            }
        }
        return currentItemSetBlock;
    }


    public static void LoadAllSkillsFiles()
    {
        TextAsset[] allFiles = Resources.LoadAll<TextAsset>(GladiusGlobals.DataRoot + "SkillFiles");
        foreach (TextAsset file in allFiles)
        {
            try
            {
                String[] lines = file.text.Split('\n');
                SkillSetBlock setBlock = LoadSkillsFile(lines);
                ClassSkillsMap[setBlock.MainClass] = setBlock;
            }
            catch (Exception e)
            {

            }

        }
    }

    public static SkillSetBlock LoadSkillsFile(String[] lines)
    {
        ////// SKILLSET export Class: Scorpion Scorpion  Affinity: None
        //MODLEVELRANGESKILL:  skillsetcomb,  0, 30, "Scorpion Attack"                 // JP:   0/  0	AccMod -2
        //MODLEVELRANGESKILL:  skillsetcomb,  0, 30, "Scorpion Evade"                  // JP:   0/  0	AccMod 0

        SkillSetBlock currentSetBlock = new SkillSetBlock(lines[0]);

        foreach (String line in lines)
        {
            //if (line.StartsWith("////// SKILLSET"))
            //{
            //    skillSets.Add(currentSetBlock);
            //}

            if (line.StartsWith("MODLEVELRANGESKILL:"))
            {

                string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ',', ':' });
                if (lineTokens.Length == 9)
                {
                    SkillSetItem skill = new SkillSetItem(lineTokens);
                    currentSetBlock.AddItem(skill);
                }

            }
        }
        return currentSetBlock;
    }


    public static Dictionary<String, SkillSetBlock> ClassSkillsMap = new Dictionary<string, SkillSetBlock>();
    public static Dictionary<String, ItemSetBlock> ClassItemsMap = new Dictionary<string, ItemSetBlock>();
    public static Dictionary<String, StatsSetBlock> ClassStatsMap = new Dictionary<string, StatsSetBlock>();


    public static Dictionary<String, List<GladiatorData>> SchoolsAndGladiators = new Dictionary<string, List<GladiatorData>>();
    public static Dictionary<ActorClass, ActorClassData> ClassDataMap = new Dictionary<ActorClass, ActorClassData>();
    public static Dictionary<ActorClass, int[]> ActorXPLevels = new Dictionary<ActorClass, int[]>();

}

public class GladiatorData
{
    public string name;
    public string className;
    public string customizeInfo;
    public string statSetName;
    public string itemSetName;
    public string skillSetName;
    public string schoolName;
    public int level;
}

public class ActorClassData
{
    public String Name;
    public String MeshName;
    public ActorClass ActorClass;
    public ActorClassAttributes Mask;

    public ActorClassData(ActorClass actorClass, String name, String meshName, ActorClassAttributes mask)
    {
        ActorClass = actorClass;
        Name = name;
        MeshName = meshName;
        Mask = mask;
    }

    // 0 = mongel? nah
    // 1 = light
    // 2 = medium
    // 4 = heavy
    // 8 = arcane
    // 16 = support 
    // 24 = prohibited arcane + support
    // 32 = beast
    // 56 = prohibited beast, required arcane + support
    // 64 = male
    // 128 = female
    // heavy = 56 (32+16+8)
    // 256 = Nordargh only?
    // 512 = Imperia only
    // 4096 = human
    // 4160 = human male        4096+64
    // 4224 = human female      4096+128

    public bool IsMale { get { return (Mask & ActorClassAttributes.Male) > 0; } }
    public bool IsFemale { get { return (Mask & ActorClassAttributes.Female) > 0; } }
    public bool IsLight { get { return (Mask & ActorClassAttributes.Light) > 0; } }
    public bool IsMedium { get { return (Mask & ActorClassAttributes.Medium) > 0; } }
    public bool IsHeavy { get { return (Mask & ActorClassAttributes.Heavy) > 0; } }
    public bool IsArcane { get { return (Mask & ActorClassAttributes.Arcane) > 0; } }
    public bool IsSupport { get { return (Mask & ActorClassAttributes.Support) > 0; } }
    public bool IsBeast { get { return (Mask & ActorClassAttributes.Beast) > 0; } }
    public bool IsNordargh { get { return (Mask & ActorClassAttributes.Nordargh) > 0; } }
    public bool IsImperia { get { return (Mask & ActorClassAttributes.Imperia) > 0; } }
}
[Flags]
public enum ActorClassAttributes
{
    Light = 1 << 0,
    Medium = 1 << 1,
    Heavy = 1 << 2,
    Arcane = 1 << 3,
    Support = 1 << 4,
    Beast = 1 << 5,
    Male = 1 << 6,
    Female = 1 << 7,
    Nordargh = 1 << 8,
    Imperia = 1 << 9,
    Steppes = 1 << 10,
    Expanse = 1 << 11,
    Human = 1 << 12
}


public class StatsSetBlock
{
    public StatsSetBlock(String line)
    {
       string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ' ' }, false);
        if (lineTokens.Length == 5)
        {
            MainClass = lineTokens[4];
        }
        else
        {
            int ibreak = 0;
        }
 
    }

    public void AddItem(StatsSet set)
    {
        List<StatsSet> setList = null;
        if (!SubBlockMap.TryGetValue(set.VariantName, out setList))
        {
            setList = new List<StatsSet>();
            SubBlockMap[set.VariantName] = setList;
        }

        setList.Add(set);

    }

    string MainClass;
    public Dictionary<String, List<StatsSet>> SubBlockMap = new Dictionary<string, List<StatsSet>>();

}


public class StatsSet
{
    public String ClassName;
    public String VariantName;
    public int Level;
    public int CON;
    public int PWR;
    public int ACC;
    public int DEF;
    public int INI;
    public float MOV;


    public void Apply(CharacterData characterData)
    {
        characterData.Level = Level;
        characterData.CON = CON;
        characterData.PWR = PWR;
        characterData.ACC = ACC;
        characterData.DEF = DEF;
        characterData.INI = INI;
        characterData.MOV = MOV;
    }


}

public class ItemSet
{
    //public ModITEMStat(ItemSetSubBlock ownerBlock, String[] fields)
    public ItemSet(String[] fields)
    {
        if (fields.Length == 9)
        {
            //OwnerBlock = ownerBlock;
            int index = 1;
            VariantName = fields[index++];
            MinLevel = int.Parse(fields[index++]);
            MaxLevel = int.Parse(fields[index++]);
            Weapon = fields[index++];
            Armor = fields[index++];
            Shield = fields[index++];
            Helmet = fields[index++];
            Accessory = fields[index++];
        }
        else
        {
            int ibreak = 0;
        }
    }

    public void Apply(CharacterData characterData)
    {
        if (!String.IsNullOrEmpty(Weapon))
        {
            characterData.AddItemByNameAndLoc(Weapon, ItemLocation.Weapon);
        }
        if (!String.IsNullOrEmpty(Armor))
        {
            characterData.AddItemByNameAndLoc(Armor, ItemLocation.Armor);
        }
        if (!String.IsNullOrEmpty(Shield))
        {
            characterData.AddItemByNameAndLoc(Shield, ItemLocation.Shield);
        }
        if (!String.IsNullOrEmpty(Helmet))
        {
            characterData.AddItemByNameAndLoc(Helmet, ItemLocation.Helmet);
        }
        if (!String.IsNullOrEmpty(Accessory))
        {
            characterData.AddItemByNameAndLoc(Accessory, ItemLocation.Accessory);
        }
    }

    public string VariantName;
    //ItemSetSubBlock OwnerBlock;
    public int MinLevel;
    public int MaxLevel;
    public string Weapon;
    public string Armor;
    public string Shield;
    public string Helmet;
    public string Accessory;

}

public class ItemSetBlock
{
    public ItemSetBlock(String line)
    {
        string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ' ' }, false);
        if (lineTokens.Length == 9)
        {

            MainClass = lineTokens[4];
            SubClass = lineTokens[5];
            Region = lineTokens[7];
            //Affinity = lineTokens[8];
        }
        else
        {
            int ibreak = 0;
        }
    }

    public void AddItem(ItemSet itemStat)
    {
        List<ItemSet> statList = null;
        if (!SubBlockMap.TryGetValue(itemStat.VariantName, out statList))
        {
            statList = new List<ItemSet>();
            SubBlockMap[itemStat.VariantName] = statList;
        }

        statList.Add(itemStat);

    }


    public string MainClass;
    public string SubClass;
    public string Region;
    //string Affinity;
    public Dictionary<String, List<ItemSet>> SubBlockMap = new Dictionary<string, List<ItemSet>>();

}


public class SkillSetBlock
{
    public SkillSetBlock(String line)
    {
        string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ' ' }, false);
        if (lineTokens.Length == 9)
        {
            MainClass = lineTokens[4];
            SubClass = lineTokens[5];
      //      Affinity = lineTokens[7];
        }
        else
        {
            int ibreak = 0;
        }
    }

    public void AddItem(SkillSetItem skillStat)
    {
        List<SkillSetItem> statList = null;
        if (!SubBlockMap.TryGetValue(skillStat.VariantName, out statList))
        {
            statList = new List<SkillSetItem>();
            SubBlockMap[skillStat.VariantName] = statList;
        }

        statList.Add(skillStat);

    }


    public string MainClass;
    public string SubClass;
    //string Affinity;
    public Dictionary<String, List<SkillSetItem>> SubBlockMap = new Dictionary<string, List<SkillSetItem>>();

}

//MODLEVELRANGESKILL:  skillsetaffE,  0, 30, "Scorpion Attack"                 // JP:   0/  0	AccMod -2
public class SkillSetItem
{
    public SkillSetItem(String[] tokens)
    {
        int index = 1;
        VariantName = tokens[index++];
        MinLevel = int.Parse(tokens[index++]);
        MaxLevel = int.Parse(tokens[index++]);
        SkillName = tokens[index++];
    }

    public string VariantName;
    public int MinLevel;
    public int MaxLevel;
    public String SkillName;

}

//}
