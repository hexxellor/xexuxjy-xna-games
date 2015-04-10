using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;


public static class ArenaLoader
{
    public static void SetupArena(String arenaDataName,Arena arena)
    {
        //Arena arena = new Arena();
        List<String> lines = new List<String>();

        TextAsset textAsset = (TextAsset)Resources.Load(arenaDataName);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
        arena.PrefabName = doc.SelectSingleNode("Arena/Prefab").InnerText;
        arena.TextureName = doc.SelectSingleNode("Arena/Texture").InnerText;

        arena.ModelPosition = GladiusGlobals.ReadVector3("Arena/Position",doc);
        arena.ModelScale = GladiusGlobals.ReadVector3("Arena/Scale", doc);
        arena.ModelRotation = GladiusGlobals.ReadVector3("Arena/Rotation", doc);
            
        arena.SkyBoxMaterialName = doc.SelectSingleNode("Arena/SkyBoxMaterial").InnerText;

        XmlNodeList nodes = doc.SelectNodes("Arena/Layout//row");
        foreach (XmlNode node in nodes)
        {
            lines.Add(node.InnerText);
        }

        //arena.Width = lines[0].Length;
        //arena.Breadth = lines.Count;

        //arena.ArenaGrid = new SquareType[arena.Width, arena.Breadth];

        //for (int i = 0; i < arena.Width; ++i)
        //{
        //    for (int j = 0; j < arena.Breadth; ++j)
        //    {
        //        if (lines[j][i] == '#')
        //        {
        //            arena.ArenaGrid[j, i] = SquareType.Wall;
        //        }
        //        else if (lines[j][i] == 'P')
        //        {
        //            arena.ArenaGrid[j, i] = SquareType.Pillar;
        //        }
        //        else if (lines[j][i] == '1')
        //        {
        //            arena.ArenaGrid[j, i] = SquareType.Level1;
        //        }
        //        else if (lines[j][i] == '2')
        //        {
        //            arena.ArenaGrid[j, i] = SquareType.Level2;
        //        }
        //        else if (lines[j][i] == '3')
        //        {
        //            arena.ArenaGrid[j, i] = SquareType.Level3;
        //        }
        //    }
        //}


        arena.SetupScenery();

        PopulateList(doc, "Arena/PlacementPoints/Player//Point", arena.PlayerPointList);
        PopulateList(doc, "Arena/PlacementPoints/Team1//Point", arena.Team1PointList);
        PopulateList(doc, "Arena/PlacementPoints/Team2//Point", arena.Team2PointList);
        PopulateList(doc, "Arena/PlacementPoints/Team3//Point", arena.Team3PointList);
    }

    private static void PopulateList(XmlDocument doc, String xpath, List<Point> points)
    {
        XmlNodeList nodes = doc.SelectNodes(xpath);
        foreach (XmlNode node in nodes)
        {
            int x = Int32.Parse(node.Attributes["x"].Value);
            int y = Int32.Parse(node.Attributes["y"].Value);
            points.Add(new Point(x, y));
        }
    }

    public static void SetupArena(ArenaEncounter arenaEncounter,Arena arena)
    {
        // setup the arena based on the arenaEncounter data
        //arena.PrefabName = arenaEncounter.Encounter.
        //arena.TextureName = doc.SelectSingleNode("Arena/Texture").InnerText;




    }
}

public class GridFileReader
{

    public byte[] ReadFile(TextAsset textAsset)
    {
        byte[] arenaGrid = null;
        using (BinaryReader binReader = new BinaryReader(new MemoryStream(textAsset.bytes)))
        {
            int header = binReader.ReadInt32();
            int numEntries = binReader.ReadInt32();
            int totalEntries = 30;
            int entryOffset = totalEntries * 0x20;

            binReader.BaseStream.Position += entryOffset;
            int rowLength = 32;
            int offset = 0x540;
            arenaGrid= binReader.ReadBytes(rowLength * rowLength);
        }

        return arenaGrid;
    }

}



