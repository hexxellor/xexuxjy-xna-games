﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public TurnManager()
    {
    }
    public bool AllActorsHadTurn()
    {
        foreach (BaseActor ba in m_allActors)
        {
            if (!ba.MovedThisRound)
            {
                return false;
            }
        }
        return true;
    }


    public void Update()
    {
        if (AllPartyDead())
        {
            Application.LoadLevel("GameOverLose");
        }
        else if (AllOpponentsDead())
        {
            Application.LoadLevel("GameOverWin");
        }

        if (AllActorsHadTurn())
        {
            EndRound();
            StartRound();
        }

        //if (GladiusGlobals.ArenaComponentsInitialised)
        {

            if (CurrentActor == null || CurrentActor.TurnComplete)
            {
                EndTurn();
                StartTurn();
            }
        }
    }

    public void StartRound()
    {
        m_turnOrders.Clear();
        //Crowd.RoundStarted();
        foreach (BaseActor actor in m_allActors)
        {
            if (!actor.Dead)
            {
                actor.RoundStarted();
                m_turnOrders.Add(actor);
            }
        }
        // sort so that highest ini goes first.
        m_turnOrders.Sort((ba1, ba2) => ba1.INI.CompareTo(ba2.INI));
        TurnCount++;

    }

    public void EndRound()
    {
        foreach (BaseActor actor in m_allActors)
        {
            actor.RoundEnded();
        }

    }


    public void StartTurn()
    {
        //Debug.Log("StartTurn");
        if (CurrentActor != null)
        {
            // tell all the actors that this actor is starting their turn to allow for status updates..
            foreach (BaseActor actor in m_allActors)
            {
                actor.ActorTurnStarted(CurrentActor);
            }

            CurrentActor.StartTurn();

            GladiusGlobals.GameStateManager.CurrentStateData.CameraManager.ReparentTarget(CurrentActor.gameObject);
            GladiusGlobals.GameStateManager.CurrentStateData.CameraManager.CurrentCameraMode = CameraMode.Normal;

            //GladiusGlobals.CameraManager.TargetObject = CurrentActor.gameObject;
            if (CurrentActor.PlayerControlled)
            {
                //ArenaScreen.SetPlayerChoiceBarVisible(true);
                WaitingOnPlayerControl = true;
            }
        }
    }

    public void EndTurn()
    {
        //Debug.Log("EndTurn");

        //ArenaScreen.MovementGrid.CurrentActor = CurrentActor;
        if (CurrentActor != null && CurrentActor.TurnComplete)
        {
            CurrentActor.EndTurn();
            if (CurrentActor.PlayerControlled)
            {
                //ArenaScreen.SetPlayerChoiceBarVisible(false);
            }
        }

        if (m_turnOrders.Count > 0)
        {
            System.Diagnostics.Debug.Assert(m_turnOrders.Count > 0);
            //Debug.Log("TurnCount : " + m_turns.Count);

            EventManager.ChangeActor(this, CurrentActor, m_turnOrders[0]);
            CurrentActor = m_turnOrders[0];
            m_turnOrders.RemoveAt(0);
        }
    }

    public void AddActor(BaseActor actor)
    {
        //Debug.Log("QueueActor : " + actor.name);
        // to do  - figure out time of last turn, and use initiative values etc
        // to possibly insert this ahead of others.
        actor.TurnManager = this;
        m_allActors.Add(actor);
        m_schools.Add(actor.School);
    }

    BaseActor m_currentActor;
    public BaseActor CurrentActor
    {
        get
        {
            return m_currentActor;
        }
        set
        {
            m_currentActor = value;
        }
    }

    bool m_waitForControlResult;
    public bool WaitingOnPlayerControl
    {
        get
        {
            return m_waitForControlResult;
        }

        set
        {
            m_waitForControlResult = value;
        }
    }

    public Arena Arena
    {
        get;
        set;
    }

    public bool AllPartyDead()
    {
        foreach (BaseActor ba in m_allActors)
        {
            // someone still alive.
            if (ba.TeamName == GladiusGlobals.PlayerTeam && !ba.Dead)
            {
                return false;
            }
        }
        return true;
    }

    public bool AllOpponentsDead()
    {
        foreach (BaseActor ba in m_allActors)
        {
            // someone still alive.
            if (ba.TeamName != GladiusGlobals.PlayerTeam && !ba.Dead)
            {
                return false;
            }
        }
        return true;
    }

    public HashSet<BaseActor> AllActors
    {
        get { return m_allActors; }
    }

    public HashSet<GladiatorSchool> AllSchools
    {
        get { return m_schools; }
    }


    public int TurnCount
    { get; set; }

    List<BaseActor> m_turnOrders = new List<BaseActor>();
    HashSet<BaseActor> m_allActors = new HashSet<BaseActor>();
    HashSet<GladiatorSchool> m_schools = new HashSet<GladiatorSchool>();
}


