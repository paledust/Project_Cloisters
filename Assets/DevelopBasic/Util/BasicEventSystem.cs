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

    public static event Action E_OnNextInteraction;
    public static void Call_OnNextInteraction()=>E_OnNextInteraction?.Invoke();
    public static event Action<IC_Basic> E_OnEndInteraction;
    public static void Call_OnEndInteraction(IC_Basic interactionController)=>E_OnEndInteraction?.Invoke(interactionController);
    public static event Action<IC_Basic> E_OnInteractionUnreachable;
    public static void Call_OnInteractionUnreachable(IC_Basic interactionController)=>E_OnInteractionUnreachable?.Invoke(interactionController);
    public static event Action<IC_Basic> E_OnInteractionReachable;
    public static void Call_OnInteractionReachable(IC_Basic interactionController)=>E_OnInteractionReachable?.Invoke(interactionController);


#region Interaction Event
    public static event Action E_OnTransitionBegin;
    public static void Call_OnTransitionBegin()=>E_OnTransitionBegin?.Invoke();
    public static event Action E_OnTransitionEnd;
    public static void Call_OnTransitionEnd()=>E_OnTransitionEnd?.Invoke();
#endregion
}