using UnityEngine;
using System.Collections;
using Gladius;

public class OverlandStartup : CommonStartup
{
    public GameObject PlayerParty;
    public string SchoolName = "Orins-School";
    // Use this for initialization
    public override void ChildStart()
    {
        OverlandStateCommon state = new OverlandStateCommon();
        state.CameraManager = GameObject.Find("Main Camera").GetComponent<CameraManager>();
        state.GladiatorSchool = new GladiatorSchool();
        GladiusGlobals.GameStateManager.SetStateData(state);

        state.GladiatorSchool.Load(SchoolName);

        state.CameraManager.CameraTarget = PlayerParty;
        state.CameraManager.CurrentCameraMode = CameraMode.Overland;

        GameObject[] towns = GameObject.FindGameObjectsWithTag("Town");

    }

    // Update is called once per frame
    void Update()
    {

    }
}
