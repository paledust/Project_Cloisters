using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("InteractionController_Manager")]
public class IC_Manager : MonoBehaviour
{
    [SerializeField] private IC_Basic[] interactionControllers;
    private int interactionIndex = 0;
    private int loadedIC_Count = 0;

    void Awake(){
        EventHandler.E_OnNextInteraction += NextInteractionHandler;
        EventHandler.E_OnEndInteraction  += EndInteractionHandler;
    }
    void OnDestroy(){
        EventHandler.E_OnNextInteraction += NextInteractionHandler;
        EventHandler.E_OnEndInteraction  += EndInteractionHandler;        
    }
    void NextInteractionHandler(){
        interactionIndex ++;
        if(interactionIndex >= interactionControllers.Length){
            interactionIndex --;
            Debug.LogWarning("No Interaction");
        //To Do: End Game.
            return;
        }
        interactionControllers[interactionIndex].EnterMiniGame();

        if(loadedIC_Count == 0) EventHandler.Call_OnTransitionEnd();
        loadedIC_Count ++;
    }
    void EndInteractionHandler(IC_Basic interController){
        interController.ExitMiniGame();
        loadedIC_Count --;
        loadedIC_Count = Mathf.Max(0, loadedIC_Count);
        if(loadedIC_Count == 0) EventHandler.Call_OnTransitionBegin();
    }
    public IC_Basic GetMiniGame(int index)=>interactionControllers[index];
    public void StartInteraction(int startIndex){
        interactionIndex = startIndex;
        interactionControllers[startIndex].EnterMiniGame();
        loadedIC_Count ++;
    }
}
