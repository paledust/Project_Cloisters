using System;
using UnityEngine;

//A basic C# Event System
public static class EventHandler
{
    public static event Action E_BeforeUnloadScene;
    public static void Call_BeforeUnloadScene() { E_BeforeUnloadScene?.Invoke(); }
    public static event Action E_AfterLoadScene;
    public static void Call_AfterLoadScene() { E_AfterLoadScene?.Invoke(); }
    public static event Action E_OnBeginSave;
    public static void Call_OnBeginSave() => E_OnBeginSave?.Invoke();
    public static event Action E_OnCompleteSave;
    public static void Call_OnCompleteSave() => E_OnCompleteSave?.Invoke();

    public static event Action E_OnNextInteraction;
    public static void Call_OnNextInteraction() => E_OnNextInteraction?.Invoke();
    public static event Action<IC_Basic> E_OnEndInteraction;
    public static void Call_OnEndInteraction(IC_Basic interactionController) => E_OnEndInteraction?.Invoke(interactionController);
    public static event Action<IC_Basic> E_OnInteractionUnreachable;
    public static void Call_OnInteractionUnreachable(IC_Basic interactionController) => E_OnInteractionUnreachable?.Invoke(interactionController);
    public static event Action<IC_Basic> E_OnInteractionReachable;
    public static void Call_OnInteractionReachable(IC_Basic interactionController) => E_OnInteractionReachable?.Invoke(interactionController);
    public static event Action E_OnFlushInput;
    public static void Call_OnFlushInput() => E_OnFlushInput();

    #region Interaction Event
    public static event Action E_OnBassChargeBeat;
    public static void Call_OnBassChargeBeat() => E_OnBassChargeBeat?.Invoke();
    public static event Action E_OnDrumBeat;
    public static void Call_OnDrumBeat() => E_OnDrumBeat?.Invoke();
    public static event Action<float> E_OnDrumKnocked;
    public static void Call_OnDrumKnocked(float strength) => E_OnDrumKnocked?.Invoke(strength);
    public static event Action<CollidableCircle> E_OnNarrativeExplode;
    public static void Call_OnNarrativeExplode(CollidableCircle circle) => E_OnNarrativeExplode?.Invoke(circle);
    public static event Action<bool> E_OnChargeText;
    public static void Call_OnChargeText(bool isCharge) => E_OnChargeText?.Invoke(isCharge);
    public static event Action<MirrorText> E_OnMirrorText;
    public static void Call_OnMirrorText(MirrorText c) => E_OnMirrorText?.Invoke(c);
    public static event Action E_OnTransitionBegin;
    public static void Call_OnTransitionBegin() => E_OnTransitionBegin?.Invoke();
    public static event Action E_OnTransitionEnd;
    public static void Call_OnTransitionEnd() => E_OnTransitionEnd?.Invoke();
    public static event Action<Clickable_Circle, Clickable_Circle, Collision> E_OnClickableCircleCollide;
    public static void Call_OnClickableCircleCollide(Clickable_Circle collidedCircle, Clickable_Circle controlledCircle,Collision collision) => E_OnClickableCircleCollide?.Invoke(collidedCircle, controlledCircle, collision);
    public static event Action<ConnectTrigger, ConnectTrigger> E_OnShapeConnect;
    public static void Call_OnShapeConnect(ConnectTrigger main, ConnectTrigger other) => E_OnShapeConnect?.Invoke(main, other);
    public static event Action<CollectableText> E_OnCollectExperimentalText;
    public static void Call_OnCollectExperimentalText(CollectableText collectTextParam) => E_OnCollectExperimentalText?.Invoke(collectTextParam);
    public static event Action<Clickable_ConnectionBreaker> E_OnBuildConnectionBreaker;
    public static void Call_OnBuildConnectionBreaker(Clickable_ConnectionBreaker connectionBreaker) => E_OnBuildConnectionBreaker?.Invoke(connectionBreaker);
    public static event Action<Clickable_ConnectionBreaker, Vector3> E_OnBreakConnectionBreaker;
    public static void Call_OnBreakConnectionBreaker(Clickable_ConnectionBreaker connectionBreaker, Vector3 breakPoint) => E_OnBreakConnectionBreaker?.Invoke(connectionBreaker, breakPoint);
    public static event Action<Collectable> E_OnCollect;
    public static void Call_OnCollect(Collectable collectable) => E_OnCollect?.Invoke(collectable);
    public static event Action E_OnBallFall;
    public static void Call_OnBallFall() => E_OnBallFall?.Invoke();
    public static event Action<bool> E_OnHitGoal;
    public static void Call_OnHitGoal(bool isCritical) => E_OnHitGoal?.Invoke(isCritical);
    public static event Action E_OnGoalBreak;
    public static void Call_OnGoalBreak() => E_OnGoalBreak?.Invoke();
    public static event Action E_OnBallHeavyBounce;
    public static void Call_OnBallHeavyBounce() => E_OnBallHeavyBounce?.Invoke();
    #endregion
}