using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TownGUIController : MonoBehaviour
{
    float time;
    float maxTime = 5f;
    int panelCounter = 0;

    BaseGUIPanel m_currentPanel = null;
    TownMenuPanel m_townPanel;
    SchoolMenuPanel m_schoolPanel;
    ArenaMenuPanel m_arenaMenuPanel;
    LeagueMenuPanel m_leaguePanel;
    ShopMenuPanel m_shopPanel;
    ShopItemMenuPanel m_shopItemPanel;
    CharacterPanel m_characterPanel;
    EncounterPanel m_encounterPanel;
    TeamSelectionPanel m_teamSelectionPanel;

    //GladiatorSchool m_school;
    CharacterData m_currentCharacterData;
    LeagueData m_selectedLeagueData;
    //ArenaEncounter m_selectedEncounter;

    TownStateCommon m_townStateCommon;


    public static void LayoutControlsInContainer(List<dfControl> controls, dfControl container, int x, int y)
    {
        int width = (int)(container.Width / x);
        int height = (int)(container.Height / y);

        int counter = 0;
        //string background1 = "king of the hill ke.tga";
        //string background2 = "mystical zo.tga";
        for (int i = 0; i < y; ++i)
        {
            for (int j = 0; j < x; ++j)
            {
                int index = (i * x) + j;
                controls[index].Width = width;
                controls[index].Height = height;
                controls[index].RelativePosition = new Vector3(j * width, i * height);
            }
        }
    }


    // Use this for initialization
    void Start()
    {
        SetupMainMenu();
        SetupPanels();
        SwitchPanel(m_townPanel);
        //SwitchPanel(m_characterPanel);
        //SwitchPanel(m_shopItemPanel);
        m_townStateCommon = GladiusGlobals.GameStateManager.TownStateCommon;

        m_townStateCommon.GladiatorSchool = new GladiatorSchool();
        //m_school.Load("Orins-school");
        m_townStateCommon.GladiatorSchool.Load("Legionaires-School");
        if (m_townStateCommon.GladiatorSchool.Gladiators.Count > 0)
        {
            foreach (CharacterData cd in m_townStateCommon.GladiatorSchool.Gladiators.Values)
            {
                SetCharacterData(cd);
                break;
            }
        }

    }

    void SetupMainMenu()
    {
        m_townPanel = new TownMenuPanel(null, this);
    }


    void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
    {
        m_currentPanel.ActionPressed(sender, e);
    }


    public void SetupPanels()
    {
        m_schoolPanel = new SchoolMenuPanel(m_townPanel, this);
        m_arenaMenuPanel = new ArenaMenuPanel(m_townPanel, this);
        m_shopPanel = new ShopMenuPanel(m_townPanel, this);
        m_characterPanel = new CharacterPanel(m_townPanel, this);
    }


    public void HandleUpDownList(List<dfControl> controls, bool up)
    {
        int i = 0;
        // if main menu
        for (i = 0; i < controls.Count; ++i)
        {
            if (controls[i].HasFocus)
            {
                break;
            }
        }

        if (up)
        {
            i = (i + 1) % controls.Count;
        }
        else
        {
            i -= 1;
            if (i < 0)
            {
                i += controls.Count;
            }
        }
        controls[i].Focus();
    }

    public void OnKeyPressed()
    {
        bool up = true;
        //HandleUpDownList(m_mainMenuButtonList, up);
    }


    void MainMenuLeaveButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {

    }

    void MainMenuLeagueButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        SwitchPanel(m_leaguePanel);
    }

    void MainMenuShopButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        SwitchPanel(m_shopPanel);
    }

    void MainMenuSchoolButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        SwitchPanel(m_schoolPanel);
    }



    public void MainMenuAction(object sender, ActionButtonPressedArgs e)
    {
        switch (e.ActionButton)
        {
            case (ActionButton.Move1Up):
                {
                    break;
                }
            case (ActionButton.Move1Down):
                {
                    break;
                }
        }



    }

    public void SetCharacterData(CharacterData characterData)
    {
        if (characterData != m_currentCharacterData)
        {
            m_currentCharacterData = characterData;
        }
    }

    public void SetTownData(TownData townData)
    {
        if (townData != m_townData)
        {
            m_townData = townData;
            m_townData.BuildData();
        }
    }

    string lastPanelName = "";
    private Vector3 lastPanelPosition = new Vector3();
    public void SwitchPanel(BaseGUIPanel newPanel)
    {
        if (m_currentPanel != null)
        {
            Debug.Log("Moving panel : " + m_currentPanel.PanelName + " to " + newPanel.PanelName);
            m_currentPanel.PanelDeactivated();
            m_currentPanel.m_panel.RelativePosition = lastPanelPosition;
            m_currentPanel.m_panel.IsVisible = false;
        }
        if (newPanel != null)
        {
            newPanel.m_panel.Focus();
            lastPanelPosition = newPanel.m_panel.RelativePosition;
            newPanel.m_panel.RelativePosition = new Vector3();
            newPanel.m_panel.BringToFront();
            newPanel.PanelActivated();
            //newPanel.SetTownData(m_townData);
            newPanel.m_panel.IsVisible = true;
        }
        m_currentPanel = newPanel;
    }


    public void Update()
    {
    }


    private TownData m_townData = null;

    public abstract class BaseGUIPanel
    {
        public TownGUIController m_townGuiController;
        public dfPanel m_panel;
        public String m_panelName;
        public int m_currentSelection;
        public BaseGUIPanel m_parentPanel;
        public List<dfControl> m_controlsList = new List<dfControl>();

        public String m_leaveName = "LeaveButton";
        public dfControl m_leaveButton;
        protected TownData m_townData;

        public BaseGUIPanel(String panelName, BaseGUIPanel parentPanel, TownGUIController townGuiController)
        {
            m_panelName = panelName;
            m_parentPanel = parentPanel;
            m_townGuiController = townGuiController;
            m_panel = GameObject.Find(m_panelName).GetComponent<dfPanel>();

            dfControl leaveButton = m_panel.Find(m_leaveName);
            if (leaveButton != null)
            {
                m_leaveButton = leaveButton;
                m_leaveButton.Click += leaveButton_Click;
            }
            //m_panel.gameObject.SetActive(false);
            //m_panel.gameObject.GetComponent<Renderer>().enabled = false;
            m_panel.IsVisible = false;
            m_panel.IsVisibleChanged += m_panel_IsVisibleChanged;
        }

        public virtual void PanelActivated()
        {
        }

        public virtual void PanelDeactivated()
        {

        }

        public TownData TownData
        { get { return m_townGuiController.m_townData; } }

        public CharacterData CharacterData
        { get { return m_townGuiController.m_currentCharacterData; } }

        void m_panel_IsVisibleChanged(dfControl control, bool value)
        {
            if (value == true)
            {
                int ibreak = 0;
            }
        }

        public void leaveButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == m_leaveName)
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }
        }

        public virtual void ActionPressed(object sender, ActionButtonPressedArgs e)
        {
        }

        public String PanelName
        { get { return m_panelName; } }
    }

    public class TownMenuPanel : BaseGUIPanel
    {

        public TownMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("MainMenuPanel", parentPanel, townGuiController)
        {
            dfPanel townPanel = m_panel.Find<dfPanel>("TownMenuPanel");

            m_controlsList.Add(m_panel.FindPath<dfButton>("TownMenuPanel/ShopButton"));
            m_controlsList.Add(m_panel.FindPath<dfButton>("TownMenuPanel/ArenaButton"));
            m_controlsList.Add(m_panel.FindPath<dfButton>("TownMenuPanel/SchoolButton"));
            m_controlsList.Add(m_panel.FindPath<dfButton>("TownMenuPanel/LeaveButton"));

            LayoutControlsInContainer(m_controlsList, townPanel, 1,m_controlsList.Count);

            m_controlsList[0].Click += TownMenuPanel_Click;
            m_controlsList[1].Click += TownMenuPanel_Click;
            m_controlsList[2].Click += TownMenuPanel_Click;
            m_controlsList[3].Click += TownMenuPanel_Click;
        }

        void TownMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "ShopButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_shopPanel);
            }
            else if (control.gameObject.name == "SchoolButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_schoolPanel);
            }
            else if (control.gameObject.name == "ArenaButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_arenaMenuPanel);
            }
        }

        public override void ActionPressed(object sender, ActionButtonPressedArgs e)
        {
        }

        public override void PanelActivated()
        {
            m_panel.Find<dfRichTextLabel>("TownTitleLabel").Text = TownData.Name;
            m_panel.Find<dfButton>("ShopButton").Text = TownData.Shop.Name;
            m_panel.Find<dfButton>("ArenaButton").Text = TownData.ArenaData.ArenaName;
            m_panel.Find<dfButton>("SchoolButton").Text = "School";
            m_panel.Find<dfButton>("LeaveButton").Text = "Leave Town";

            string regionTextureName = "GladiusUI/TownOverland/TownBackground/";
            switch (TownData.Region)
            {
                case "expanse":
                    regionTextureName += "town_headere.tga";
                    break;
                case "imperia":
                    regionTextureName += "town_headeri.tga";
                    break;
                case "nordargh":
                    regionTextureName += "town_headern.tga";
                    break;
                case "steppes":
                    regionTextureName += "town_headers.tga";
                    break;
            }
            m_panel.Find<dfTextureSprite>("TownRegionImage").Texture = Resources.Load<Texture2D>(regionTextureName);
            m_panel.Find<dfTextureSprite>("TownImage").Texture = Resources.Load<Texture2D>(TownData.BackgroundTextureName);


        }

    }


    public class ShopMenuPanel : BaseGUIPanel
    {
        public ShopMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("ShopPanel", parentPanel, townGuiController)
        {
            dfControl panel = m_panel.Find<dfControl>("ShopMenuPanel");
            m_controlsList.Add(GameObject.Find("ItemButton").GetComponent<dfButton>());
            m_controlsList.Add(GameObject.Find("ChatButton").GetComponent<dfButton>());
            m_controlsList.Add(GameObject.Find("LeaveButton").GetComponent<dfButton>());

            LayoutControlsInContainer(m_controlsList, panel, 1, m_controlsList.Count);

            foreach (dfControl dfc in m_controlsList)
            {
                dfButton button = dfc as dfButton;
                if (button != null)
                {
                    button.Click += ShopMenuPanel_Click;
                }
            }
            townGuiController.m_shopItemPanel = new ShopItemMenuPanel(this, townGuiController);

        }

        void ShopMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "ItemButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_shopItemPanel);
            }
        }

        public override void PanelActivated()
        {
            m_panel.Find<dfLabel>("Label").Text = TownData.Shop.Name;
            m_panel.Find<dfRichTextLabel>("ShopKeeperDialogueLabel").Text = GladiusGlobals.GameStateManager.LocalisationData[TownData.Shop.Opening];
            m_panel.FindPath<dfTextureSprite>("TownPicture/Texture").Texture = Resources.Load<Texture>(TownData.Shop.Image);
        }

    }

    public class ShopItemMenuPanel : BaseGUIPanel
    {
        public ShopItemMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("ShopItemPanel", parentPanel, townGuiController)
        {
        }

        public override void PanelActivated()
        {
            m_panel.GetComponent<ItemPanelController>().SetData(TownData.Shop);
        }
    }

    public class ArenaMenuPanel : BaseGUIPanel
    {

        public ArenaMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("ArenaPanel", parentPanel, townGuiController)
        {
            townGuiController.m_leaguePanel = new LeagueMenuPanel(this, townGuiController);
            dfControl panel = m_panel.Find<dfPanel>("LeagueMenuPanel");
            m_controlsList.Add(m_panel.Find("ChampionshipButton").GetComponent<dfButton>());
            m_controlsList.Add(m_panel.Find("TournamentButton").GetComponent<dfButton>());
            m_controlsList.Add(m_panel.Find("LeaguesButton").GetComponent<dfButton>());
            m_controlsList.Add(m_panel.Find("RecruitingButton").GetComponent<dfButton>());
            m_controlsList.Add(m_panel.Find("HistoryButton").GetComponent<dfButton>());
            m_controlsList.Add(m_panel.Find("LeaveButton").GetComponent<dfButton>());

            LayoutControlsInContainer(m_controlsList, panel, 1, m_controlsList.Count);

            foreach (dfControl control in m_controlsList)
            {
                control.Click += ArenaMenuPanel_Click;
            }

        }

        void ArenaMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "ChampionshipButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_leaguePanel);
            }
            if (control.gameObject.name == "TournamentButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_leaguePanel);
            }
            if (control.gameObject.name == "LeaguesButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_leaguePanel);
            }

        }

        public override void PanelActivated()
        {
            //m_panel.Find<dfLabel>("Label").Text = "League Data";
            m_panel.Find<dfLabel>("LeagueName").Text = TownData.ArenaData.ArenaName;
            m_panel.Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(TownData.ArenaData.BackgroundTextureName);
            m_panel.Find<dfTextureSprite>("OwnerThumbnail").Texture = Resources.Load<Texture2D>(TownData.ArenaData.OwnerThumbnailName);
        }
    }



    public class LeagueMenuPanel : BaseGUIPanel
    {
        public GameObject SlotPrefab;
        private List<dfButton> m_leaguePanels = new List<dfButton>();
        int SlotsH = 3;
        int SlotsV = 2;
        public LeagueMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("LeaguePanel", parentPanel, townGuiController)
        {
            SlotPrefab = Resources.Load("Prefabs/LeagueSlotPrefab") as GameObject;
            townGuiController.m_encounterPanel = new EncounterPanel(this, townGuiController);
            dfPanel leagueDisplayPanel = m_panel.Find<dfPanel>("LeagueDisplay");
            int width = (int)(leagueDisplayPanel.Width / SlotsH);
            int height = (int)(leagueDisplayPanel.Height / SlotsV);

            int counter = 0;
            //string background1 = "king of the hill ke.tga";
            //string background2 = "mystical zo.tga";
            for (int i = 0; i < SlotsV; ++i)
            {
                for (int j = 0; j < SlotsH; ++j)
                {
                    int index = (i * SlotsH) + j;

                    dfButton panel = leagueDisplayPanel.AddPrefab(SlotPrefab) as dfButton;
                    m_leaguePanels.Add(panel);
                    panel.Width = width;
                    panel.Height = height;
                    //panel.Position = new Vector3();
                    panel.RelativePosition = new Vector3(j * width, i * height);
                    panel.Click += panel_Click;
                }
            }
        }

        public override void PanelActivated()
        {
            dfPanel headerPanel = m_panel.Find<dfPanel>("LeagueHeaderPanel");
            //m_panel.Find<dfLabel>("Label").Text = "League Data";
            headerPanel.Find<dfRichTextLabel>("ArenaName").Text = TownData.ArenaData.ArenaName;
            headerPanel.Find<dfSprite>("OwnerThumbnail").SpriteName = TownData.ArenaData.OwnerThumbnailName;

            dfPanel leagueDisplayPanel = m_panel.Find<dfPanel>("LeagueDisplay");
            int width = (int)(leagueDisplayPanel.Width / SlotsH);
            int height = (int)(leagueDisplayPanel.Height / SlotsV);

            int counter = 0;
            //string background1 = "king of the hill ke.tga";
            //string background2 = "mystical zo.tga";
            for (int i = 0; i < SlotsV; ++i)
            {
                for (int j = 0; j < SlotsH; ++j)
                {
                    int index = (i * SlotsH) + j;
                    dfButton panel = m_leaguePanels[index];

                    if (index < TownData.ArenaData.Leagues.Count)
                    {
                        panel.enabled = true;
                        LeagueData leagueData = TownData.ArenaData.Leagues[index];
                        leagueData.ImageName = leagueData.Name + ".tga";//(index % 2 == 0) ? background1 : background2;
                        panel.Width = width;
                        panel.Height = height;
                        panel.Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
                        panel.Find<dfRichTextLabel>("LeagueStatus").Text = leagueData.Name;
                        panel.Tag = leagueData;
                    }
                    else
                    {
                        panel.IsVisible = false;
                    }
                }
            }
            m_townGuiController.m_selectedLeagueData = null;
        }

        void panel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            LeagueData leagueData = control.Tag as LeagueData;
            if (m_townGuiController.m_selectedLeagueData == leagueData) // we've selected the same one again
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_encounterPanel);
            }
            else
            {
                m_panel.Find("LeagueDataPanel").Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
                m_panel.Find("LeagueDataPanel").Find<dfRichTextLabel>("LeagueName").Text = GladiusGlobals.GameStateManager.LocalisationData[leagueData.DescriptionId];
            }
            m_townGuiController.m_selectedLeagueData = leagueData;
        }

    }

    public class SchoolMenuPanel : BaseGUIPanel
    {
        public SchoolMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("SchoolPanel", parentPanel, townGuiController)
        {
        }


    }


    public class CharacterPanel : BaseGUIPanel
    {
        String[] names = new String[] { "LVL", "XP", "Next", "HP", "DAM", "PWR", "ACC", "DEF", "INI", "CON", "MOV", "Arm", "Wpn" };

        public CharacterPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("CharacterPanel", parentPanel, townGuiController)
        {
            dfPanel statsPanel = m_panel.Find<dfPanel>("StatsPanel");
            Vector3 dims = new Vector3();
            Vector3 pos = new Vector3();

            GameObject statRowPrefab = Resources.Load("Prefabs/StatRowPrefab") as GameObject;

            float offset = 0;
            foreach (String name in names)
            {
                dfPanel statPanel = statsPanel.AddPrefab(statRowPrefab) as dfPanel;
                statPanel.name = "Panel" + name;
                statPanel.Find<dfLabel>("Label").Text = name;
                statPanel.Position = new Vector3(0, -offset, 0);
                offset += statPanel.Height;
            }

        }

        public void UpdatePanel()
        {
            m_panel.FindPath<dfRichTextLabel>("NameAndClass").GetComponent<dfRichTextLabel>().Text = "" + CharacterData.Name + "\n" + CharacterData.ActorClass;
            //GameObject.FindPath(PanelName + "NameAndClass").GetComponent<dfRichTextLabel>().Text = "" + characterData.Name;
            foreach (String name in names)
            {
                m_panel.FindPath<dfLabel>("StatsPanel/Panel" + name + "/Value").Text = CharacterData.ValFromName(name);
            }
            // Try and get a class image?
            Texture2D classTex = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "ClassImages/" + CharacterData.ActorClassData.MeshName);
            if (classTex != null)
            {
                m_panel.FindPath<dfTextureSprite>("CharacterSprite").Texture = classTex;
            }


        }

        public override void PanelActivated()
        {
            UpdatePanel();
        }
    }

    public class TeamSelectionPanel : BaseGUIPanel
    {
        SelectionGrid m_availableGrid;
        SelectionGrid m_selectedGrid;
        dfPanel m_requiredPanel;
        dfPanel m_prohibitedPanel;
        dfPanel m_rhsPanel;
        //dfPanel m_characterPanel;

        public TeamSelectionPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("TeamSelectionPanel", parentPanel, townGuiController)
        {
            m_availableGrid = m_panel.gameObject.AddComponent<SelectionGrid>();
            m_selectedGrid = m_panel.gameObject.AddComponent<SelectionGrid>();
            m_availableGrid.Init(m_panel.Find("AvailablePanel"), "Available", AvailableGrid_Click);
            m_selectedGrid.Init(m_panel.Find("SelectedPanel"), "Selected", SelectedGrid_Click);
            m_rhsPanel = m_panel.Find<dfPanel>("RHSPanel");
            m_requiredPanel = m_rhsPanel.Find<dfPanel>("RequiredPanel");
            m_prohibitedPanel = m_rhsPanel.Find<dfPanel>("ProhibitedPanel");

            dfControl panel = m_panel.Find<dfControl>("ButtonPanel");

            dfButton proceedButton = m_panel.Find<dfButton>("ProceedButton");
            proceedButton.Click += proceedButton_Click;
            m_controlsList.Add(proceedButton);
            m_controlsList.Add(m_leaveButton);

            LayoutControlsInContainer(m_controlsList, panel, m_controlsList.Count,1);


        }



        public override void PanelActivated()
        {
            base.PanelActivated();
            if (m_townGuiController.m_townStateCommon.Encounter != null)
            {
                m_townGuiController.m_townStateCommon.Encounter.LoadEncounterData();

                // fill in available panel with school gladiators.
                int count = 0;
                //m_availableGrid.SetStartDefault(0, 0);
                foreach (CharacterData characterData in m_townGuiController.m_townStateCommon.GladiatorSchool.Gladiators.Values)
                {
                    if (count < m_availableGrid.MaxSize)
                    {
                        m_availableGrid.SetSlot(count++, characterData);
                    }
                }

                EncounterSide heroSide = m_townGuiController.m_townStateCommon.Encounter.Encounter.Sides[0];
                // side 0 is player/ heros team?
                m_selectedGrid.SetStartDefault(heroSide.CharacterDataList);

                //m_selectedGrid.SetStartDefault(2, 4);

                // setup required and probhibited text.
                m_requiredPanel.Find<dfRichTextLabel>("RTL").Text = BuildStringForReqPro(heroSide.CharacterDataList[0], true);
                m_prohibitedPanel.Find<dfRichTextLabel>("RTL").Text = BuildStringForReqPro(heroSide.CharacterDataList[0], false);
            }
        }

        public String BuildStringForReqPro(CharacterData characterData, bool required)
        {



            return required ? "Required" : "Prohibited";
        }


        void AvailableGrid_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            // some work here to turn character panel into a prefab?
            // need to display image of character when selected...?
            SlotData slotData = control.Tag as SlotData;
            CharacterData characterData = slotData.Current;
            if (characterData != null)
            {
                m_townGuiController.m_currentCharacterData = characterData;
                ShowCharacterPanel();
                // move control
                AvailableToSelected(control);
            }


        }

        void SelectedGrid_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            SelectedToAvailable(control);
        }

        void ShowCharacterPanel()
        {
            m_requiredPanel.IsVisible = false;
            m_prohibitedPanel.IsVisible = false;
            m_townGuiController.m_characterPanel.m_panel.IsVisible = true;
            m_townGuiController.m_characterPanel.m_panel.transform.parent = m_rhsPanel.transform;
            m_townGuiController.m_characterPanel.m_panel.Size = m_rhsPanel.Size;
            m_townGuiController.m_characterPanel.UpdatePanel();
            //m_townGuiController.m_characterPanel.Yield;
            //m_townGuiController.m_characterPanel.m_panel.transform.localPosition = new Vector3(0f, 0f, 0f);
            m_townGuiController.m_characterPanel.m_panel.Position = new Vector3();// transform.localPosition = new Vector3(0f, 0f, 0f);
            // size to fit.?
        }

        void HideCharacterPanel()
        {
            m_requiredPanel.IsVisible = true;
            m_prohibitedPanel.IsVisible = true;
            m_townGuiController.m_characterPanel.m_panel.IsVisible = false;
            m_townGuiController.m_characterPanel.m_panel.transform.parent = m_rhsPanel.transform;
            m_townGuiController.m_characterPanel.m_panel.Size = m_rhsPanel.Size;
            // size to fit.?
        }

        void AvailableToSelected(dfControl control)
        {
            if (m_selectedGrid.EmptySlots > 0)
            {
                SlotData slotData = control.Tag as SlotData;
                CharacterData characterData = slotData.Current;

                if (characterData != null)
                {
                    int nextSlot = m_selectedGrid.NextAvailableSlot();
                    if (nextSlot >= 0)
                    {
                        m_selectedGrid.SetSlot(nextSlot, characterData);
                        m_availableGrid.SetSlot(control as dfButton, null);
                    }
                }
            }
        }

        void SelectedToAvailable(dfControl control)
        {
            SlotData slotData = control.Tag as SlotData;
            CharacterData characterData = slotData.Current;

            if (characterData != null)
            {
                int nextSlot = m_availableGrid.NextAvailableSlot();
                if (nextSlot >= 0)
                {
                    m_availableGrid.SetSlot(nextSlot, characterData);
                    m_selectedGrid.SetSlot(control as dfButton, null);
                }
            }
        }

        void proceedButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            // do some checks here to make sure all slots are filled and that we have a team to fight with...
            List<CharacterData> newParty = new List<CharacterData>();
            m_selectedGrid.FillParty(newParty);
            GladiusGlobals.GameStateManager.CurrentStateData.GladiatorSchool.SetCurrentParty(newParty);
            GladiusGlobals.GameStateManager.SetNewState(GameState.Arena, null);
        }

    }

    public class EncounterPanel : BaseGUIPanel
    {
        private List<dfPanel> m_encounterPanels = new List<dfPanel>();
        public GameObject SlotPrefab;
        public int NumEncounters = 8;

        public EncounterPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("EncounterPanel", parentPanel, townGuiController)
        {
            SlotPrefab = Resources.Load("Prefabs/EncounterListPrefab") as GameObject;
            townGuiController.m_teamSelectionPanel = new TeamSelectionPanel(this, townGuiController);

            dfPanel leagueDisplayPanel = m_panel.Find<dfPanel>("EncounterDisplay");
            int width = (int)(leagueDisplayPanel.Width);
            int height = (int)(leagueDisplayPanel.Height / NumEncounters);

            int counter = 0;
            //string background1 = "king of the hill ke.tga";
            //string background2 = "mystical zo.tga";
            for (int i = 0; i < NumEncounters; ++i)
            {
                int index = i;
                dfPanel panel = leagueDisplayPanel.AddPrefab(SlotPrefab) as dfPanel;
                m_encounterPanels.Add(panel);
                panel.Width = width;
                panel.Height = height;
                //panel.Position = new Vector3();
                panel.RelativePosition = new Vector3(0, i * height);
                panel.Click += panel_Click;
            }

        }

        void proceedButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            // do some checks here to make sure all slots are filled and that we have a team to fight with...
            List<CharacterData> newParty = new List<CharacterData>();

            GladiusGlobals.GameStateManager.CurrentStateData.GladiatorSchool.SetCurrentParty(newParty);
        }

        public override void PanelActivated()
        {
            dfPanel headerPanel = m_panel.Find<dfPanel>("EncounterHeaderPanel");
            //m_panel.Find<dfLabel>("Label").Text = "League Data";
            headerPanel.Find<dfRichTextLabel>("ArenaName").Text = TownData.ArenaData.ArenaName;
            headerPanel.Find<dfSprite>("OwnerThumbnail").SpriteName = TownData.ArenaData.OwnerThumbnailName;

            // use the data in selected league to populate the encounter
            m_panel.Find<dfTextureSprite>("EncounterImage").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + m_townGuiController.m_selectedLeagueData.ImageName);
            m_panel.Find<dfRichTextLabel>("EncounterName").Text = "";

            for (int i = 0; i < NumEncounters; ++i)
            {
                dfPanel panel = m_encounterPanels[i];
                if (i < m_townGuiController.m_selectedLeagueData.ArenaEncounters.Count)
                {
                    panel.IsVisible = true;
                    ArenaEncounter encounter = m_townGuiController.m_selectedLeagueData.ArenaEncounters[i];
                    panel.Find<dfRichTextLabel>("Name").Text = GladiusGlobals.GameStateManager.LocalisationData[encounter.Id];
                    //panel.Find<dfTextureSprite>("Points").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
                    panel.Tag = encounter;
                }
                else
                {
                    panel.IsVisible = false;
                }

            }
            m_townGuiController.m_townStateCommon.Encounter = null;

        }

        void panel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            ArenaEncounter encounter = control.Tag as ArenaEncounter;
            //m_panel.Find("EncounterDataPanel").Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
            String info = GladiusGlobals.GameStateManager.LocalisationData[encounter.EncounterDescId];
            m_panel.Find("EncounterDataPanel").Find<dfRichTextLabel>("EncounterName").Text = info;
            if (encounter == m_townGuiController.m_townStateCommon.Encounter)
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_teamSelectionPanel);
            }

            m_townGuiController.m_townStateCommon.Encounter = encounter;

        }
    }





}








