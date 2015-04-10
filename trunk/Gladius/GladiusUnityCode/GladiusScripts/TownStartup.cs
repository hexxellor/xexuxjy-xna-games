using System;
using System.Text;
using UnityEngine;

public class TownStartup: CommonStartup
{
    public String TownName = "Trikata";
    public TownData TownData;

        // Use this for initialization
    public override void ChildStart()
    {
        //GladiusGlobals.GameStateManager.SetNewState(GameState.Arena, null);
        TownStateCommon state = new TownStateCommon();
        GladiusGlobals.GameStateManager.SetStateData(state);

        TownData = GladiusGlobals.GameStateManager.TownManager.Find(TownName);
        GetComponent<TownGUIController>().SetTownData(TownData);
    }
}
