using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

    public class GameStateManager
    {
        private GameState m_gameState = GameState.Town;
        private CommonState m_stateData = null;
        private LoadingScreen m_loadingScreen;

        private LocalisationData m_localisationData;
        public LocalisationData LocalisationData
        {
            get { return m_localisationData; }
            private set 
            { 
                m_localisationData = value; 
            }

        }

        private ItemManager m_itemManager;
        public ItemManager ItemManager
        {
            get { return m_itemManager; }
            private set { m_itemManager = value; }
        }

        private TownManager m_townManager;
        public TownManager TownManager
        {
            get { return m_townManager; }
            private set { m_townManager = value; }
        }


        public void StartGame()
        {
            Application.targetFrameRate = 30;

            GameObject go = Resources.Load("Prefabs/LoadingScreenPrefab") as GameObject;
            //dfGUIManager.inAddPrefab
            dfControl dfc = dfGUIManager.ActiveManagers.FirstOrDefault().AddPrefab(go);
            m_loadingScreen = dfc.gameObject.GetComponent<LoadingScreen>();

            LocalisationData = new LocalisationData();
            LocalisationData.Load();

            ItemManager = new ItemManager();
            ItemManager.Load();

            TownManager = new TownManager();
            TownManager.Load();


            ActorGenerator.InitClassCategories();

        }


        //public static ItemManager ItemManager = new ItemManager();
        //public static EventLogger EventLogger = new EventLogger(null);

        public void SetStateData(CommonState newStateData)
        {
            // copy the current state data into the new?
            if (m_stateData != null)
            {
                m_stateData.StateCleanup();
            }
            
            newStateData.CopyState(m_stateData);
            m_stateData = newStateData;

            if(m_stateData != null)
            {
                m_stateData.StateInit();
            }
        }

        

        public void SetNewState(GameState gameState,object o)
        {
            if (m_gameState != gameState)
            {
                m_gameState = gameState;

                // transfer common over. bit yucky

                switch (m_gameState)
                {
                    case (GameState.Arena):
                        {
                            //Application.LoadLevel("ArenaScene");
                            m_loadingScreen.Load("ArenaScene");
                            break;
                        }
                    case(GameState.GameOverLose):
                        {
                            m_loadingScreen.Load("GameOverLose");
                            break;
                        }
                    case(GameState.GameOverWin):
                        {
                            m_loadingScreen.Load("GameOverLose");
                            break;
                        }
                    case(GameState.Town):
                        {
                            m_loadingScreen.Load("TownScene");
                            break;
                        }
                    case(GameState.OverlandImperia):
                        {
                            m_loadingScreen.Load("ImperiaWorldMap");
                            break;
                        }
                    case(GameState.OverlandNordargh):
                        {
                            m_loadingScreen.Load("NordarghWorldMap");
                            break;
                        }

                    }
                }
        }

        public UserControl UserControl
        {
            get;
            set;
        }

        public CommonState CurrentStateData
        {
            get
            {
                return m_stateData;
            }
        }

        public ArenaStateCommon ArenaStateCommon
        {
            get 
            {
                System.Diagnostics.Debug.Assert(m_stateData is ArenaStateCommon);
                return m_stateData as ArenaStateCommon; 
            }
        }

        public OverlandStateCommon OverlandStateCommon
        {
            get
            {
                System.Diagnostics.Debug.Assert(m_stateData is OverlandStateCommon);
                return m_stateData as OverlandStateCommon; 
            }

        }

        public TownStateCommon TownStateCommon
        {
            get
            {
                System.Diagnostics.Debug.Assert(m_stateData is TownStateCommon);
                return m_stateData as TownStateCommon; 
            }

        }

    }

    public class CommonState
    {
        public CameraManager CameraManager;
        public GladiatorSchool GladiatorSchool;
        public TownData TownData;
        public ArenaEncounter Encounter;
        public void CopyState(CommonState toCopy)
        {
            if(toCopy != null)
            {
                CameraManager = toCopy.CameraManager;
                GladiatorSchool = toCopy.GladiatorSchool;
                TownData = toCopy.TownData;
                Encounter = toCopy.Encounter;
            }
        }

        public virtual void StateInit()
        {
        }

        public virtual void StateCleanup()
        {

        }
    }

    public class ArenaStateCommon : CommonState
    {
        public TurnManager TurnManager;
        public Arena Arena;
        public MovementGrid MovementGrid;
        public CombatEngine CombatEngine;
        public CombatEngineUI CombatEngineUI;
        public Crowd Crowd;
        public BattleData BattleData;
        public LOSTester LOSTester;
    }

    public class OverlandStateCommon : CommonState
    {
    }

    public class TownStateCommon : CommonState
    {
    }

