using UnityEngine;
using System.Collections.Generic;

public class Encounter 
{
/*
ENCOUNTERNAME: 		"CircleOfElites1Fliuch1"
URSULAEASE: 	-1
VALENSEASE: 	-1
PRIZETIER	"CircleTier1EventExp" 0
PRIZETIER	"CircleTier1EventValensExp" 1
PRIZETIER	"CircleTier1EventProExp" 2
SCENE: 		"Nordagh\thefen\thefen.scn"
GRIDFILE: 	"$l\Nordagh\thefen\thefennomove.grd"
PROPSFILE: 	"$d\encounters\Nordagh\TheFen\theKeep.prp"
MUSIC: 		"$s\music\nordagh_loop06.wav"
CAMERATRACK: 	"intro","$l\Nordagh\thefen\thefenIntroAnim.pan"
CAMERATRACK: 	"outro","$l\Nordagh\thefen\thefenOutroAnim.pan"
BATTLESCRIPT: 	"battle_circle1fliuch1.scp"
TOTALPOP:	50
 */

    public string EncounterName
    { get; set; }

    public string SceneFile
    { get; set; }

    public string GridFile
    {get;set;}

    public int TotalPopularity
    { get; set; }

    public static Encounter Load(string name)
    {
        string filename = GladiusGlobals.EncountersPath + name;
        TextAsset textAsset = (TextAsset)Resources.Load(filename);
        Encounter encounter = new Encounter();
        encounter.Parse(textAsset.text);
        return encounter;
    }

    public void Parse(string data)
    {
        string[] lines = data.Split('\n');

        EncounterSide side = null;
        for (int counter = 0; counter < lines.Length; counter++)
        {
            string line = lines[counter];
            if (line.StartsWith("//"))
            {
                continue;
            }

            string[] lineTokens = GladiusGlobals.SplitAndTidyString(line,new char[] { ',', ':' });
			if(lineTokens.Length > 0)
			{
                if (lineTokens[0] == "ENCOUNTERTNAME")
                {
                    EncounterName = lineTokens[1];
                }
                else if (lineTokens[0] == "SCENE")
                {
                    SceneFile = lineTokens[1];
                    if (SceneFile.EndsWith(".scn"))
                    {
                        SceneFile = SceneFile.Replace(".scn", "");
                    }
                }
                else if (lineTokens[0] == "GRIDFILE")
                {
                    GridFile = lineTokens[1];
                }
				else if (lineTokens[0] == "TEAM")
				{
                    side = new EncounterSide();
                    Sides.Add(side);
                    side.TeamName = lineTokens[1];
                    if (side.TeamName == "0")
                    {
                        side.TeamName = GladiusGlobals.PlayerTeam;
                    }
					//heroName = lineTokens[1];
				}
                else if (lineTokens[0] == "SCHOOL")
                {
                    side.SchoolName = lineTokens[1];
                }
                else if (lineTokens[0] == "GLADIATOR")
                {
                    side.ChosenGladiators.Add(lineTokens[1]);
                }
                else if (lineTokens[0] == "UNITDB")
                {
                    string shrunkLine = line.Replace("UNITDB: \t", "");
                    //string[] subtokens = shrunkLine.Split(',');
                    string[] subtokens = GladiusGlobals.SplitAndTidyString(shrunkLine, new char[] { ',' }, removeComments: true, removeEmpty: false);
                    CharacterData characterData = ActorGenerator.ParseUNITDB(subtokens);
                    characterData.TeamName = side.SchoolName;

                    CharacterData completeCharacterData = ActorGenerator.CreateRandomCharacterForLevel(characterData);

                    side.CharacterDataList.Add(completeCharacterData);
                }
            }
        }
	}


    public List<EncounterSide> Sides = new List<EncounterSide>();



}
public class EncounterSide
{
    public EncounterSide()
    {
        ChosenGladiators = new List<string>();
        SchoolName = "ImperiaOrusOrcusDomini";
    }

    public GladiatorSchool School
    {
        get;
        set;

    }

    public string TeamName
    {
        get;
        set;
    }

    public string SchoolName
    {
        get;
        set;
    }


    public List<string> ChosenGladiators
    {
        get;
        set;
    }

    public List<CharacterData> CharacterDataList = new List<CharacterData>();

}
