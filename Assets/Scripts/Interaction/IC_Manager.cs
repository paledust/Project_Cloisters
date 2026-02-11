using UnityEngine;

[AddComponentMenu("InteractionController_Manager")]
public class IC_Manager : MonoBehaviour
{
    [SerializeField] private IC_Basic[] interactionControllers;
[Header("Debug Option")]
    [SerializeField] private int StartIndex = 0;
    [SerializeField, ShowOnly] private int interactionIndex = 0;
    private int loadedIC_Count = 0;

    void Awake(){
        EventHandler.E_OnInteractionReachable   += PreloadInteraction; 
        EventHandler.E_OnNextInteraction        += NextInteraction;
        EventHandler.E_OnEndInteraction         += EndInteraction;
        EventHandler.E_OnInteractionUnreachable += CleanUpInteraction;
    }
    void OnDestroy(){
        EventHandler.E_OnInteractionReachable   -= PreloadInteraction; 
        EventHandler.E_OnNextInteraction        -= NextInteraction;
        EventHandler.E_OnEndInteraction         -= EndInteraction;
        EventHandler.E_OnInteractionUnreachable -= CleanUpInteraction; 
    }
    void Start()
    {
    #if UNITY_EDITOR
        StartAtInteraction(StartIndex);
    #else
        StartAtInteraction(0);
    #endif
    }

#if UNITY_EDITOR
    [ContextMenu("Set Up Scene To Interactions")]
    public void Editor_SetUpInteractions(){
        CleanUpAllInteractions();
        Editor_ActivateInteractions(StartIndex);
    }
#endif
    void NextInteraction(){
        interactionIndex ++;
        if(interactionIndex >= interactionControllers.Length){
            interactionIndex --;
            Debug.LogWarning("No Interaction");
        //To Do: End Game.
            return;
        }
    //Make sure to load interaction before enter
        if(!interactionControllers[interactionIndex].m_isLoaded) PreloadInteraction(interactionControllers[interactionIndex]);
        interactionControllers[interactionIndex].EnterInteraction();

    //If no other interaction loaded, treat current game state as transition end.
        if(loadedIC_Count == 0) EventHandler.Call_OnTransitionEnd();
        loadedIC_Count ++;
    }
    void EndInteraction(IC_Basic interController){
        interController.ExitInteraction();
        loadedIC_Count --;
        loadedIC_Count = Mathf.Max(0, loadedIC_Count);
    //If no loaded interaction, treat current game state as transition.
        if(loadedIC_Count == 0) EventHandler.Call_OnTransitionBegin();
    }
    void CleanUpInteraction(IC_Basic interController){
        if(!interController.m_isDone) EndInteraction(interController);
        interController.CleanUpInteraction();
    }
    void PreloadInteraction(IC_Basic interController)=>interactionControllers[interactionIndex].PreloadInteraction();

#region Tool Function
    public IC_Basic GetMiniGame(int index)=>interactionControllers[index];
    public void StartAtInteraction(int startIndex){
        CleanUpAllInteractions();

        interactionIndex = startIndex;
        interactionControllers[startIndex].PreloadInteraction();
        interactionControllers[startIndex].EnterInteraction();
        loadedIC_Count ++;
    }
    public void CleanUpAllInteractions()
    {
        for (int i = 0; i < interactionControllers.Length; i++)
        {
            interactionControllers[i].CleanUpInteraction();
        }
    }
#endregion

#if UNITY_EDITOR
    public void Editor_ActivateInteractions(int index){
        interactionControllers[index].PreloadInteraction();
    }
#endif
}
