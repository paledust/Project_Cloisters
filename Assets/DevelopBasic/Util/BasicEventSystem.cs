using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A basic C# Event System
public static class EventHandler
{
    public static event Action E_BeforeUnloadScene;
    public static void Call_BeforeUnloadScene(){E_BeforeUnloadScene?.Invoke();}
    public static event Action E_AfterLoadScene;
    public static void Call_AfterLoadScene(){E_AfterLoadScene?.Invoke();}
    public static event Action E_OnBeginSave;
    public static void Call_OnBeginSave()=>E_OnBeginSave?.Invoke();
    public static event Action E_OnCompleteSave;
    public static void Call_OnCompleteSave()=>E_OnCompleteSave?.Invoke();

#region Interaction Event
    public static event Action E_OnTransitionBegin;
    public static void Call_OnTransitionBegin()=>E_OnTransitionBegin?.Invoke();
    public static event Action E_OnTransitionEnd;
    public static void Call_OnTransitionEnd()=>E_OnTransitionEnd?.Invoke();
    public static event Action E_OnPlanetReachPos;
    public static void Call_OnPlanetReachPos()=>E_OnPlanetReachPos?.Invoke();
#endregion
}