using System;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("InteractionController_Manager")]
public class IC_Manager : MonoBehaviour
{
    [SerializeField] private IC_Basic[] interactionControllers;
[Header("Game End")]
    [SerializeField] private AmbienceHandler ambienceHandler;
    [SerializeField] private string endSceneName;
    [SerializeField] private float endDelay = 3f;
[Header("Debug Option")]
    [SerializeField] private int StartIndex = 0;
    [SerializeField] private bool startAtDebugIndex = false;
    [SerializeField] private InputActionMap debugActions;

    private int interactionIndex = 0;
    private int loadedIC_Count = 0;

    void Awake(){
        EventHandler.E_OnInteractionReachable   += PreloadInteraction; 
        EventHandler.E_OnNextInteraction        += NextInteraction;
        EventHandler.E_OnEndInteraction         += EndInteraction;
        EventHandler.E_OnInteractionUnreachable += CleanUpInteraction;
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        debugActions["progress"].performed += Debug_Progress;
        debugActions["regress"].performed += Debug_Regress;
        debugActions["restart"].performed += Debug_RestartLevel;
        debugActions["reset"].performed += Debug_Reset;
        debugActions.Enable();
    #endif
    }
    void OnDestroy(){
        EventHandler.E_OnInteractionReachable   -= PreloadInteraction; 
        EventHandler.E_OnNextInteraction        -= NextInteraction;
        EventHandler.E_OnEndInteraction         -= EndInteraction;
        EventHandler.E_OnInteractionUnreachable -= CleanUpInteraction; 
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        debugActions["progress"].performed -= Debug_Progress;
        debugActions["regress"].performed -= Debug_Regress;
        debugActions["restart"].performed -= Debug_RestartLevel;
        debugActions["reset"].performed -= Debug_Reset;
        debugActions.Disable();
    #endif
    }
    void Start()
    {
        ambienceHandler.Init(AudioManager.Instance);
    #if UNITY_EDITOR
        if(startAtDebugIndex)
        {
            StartAtInteraction(StartIndex);
            return;
        }
    #endif
        StartAtInteraction(LevelProgressionManager.Instance.LevelProgress);
    }

#if UNITY_EDITOR
    [ContextMenu("Set Up Scene To Interactions")]
    public void Editor_SetUpInteractions(){
        if(EditorApplication.isPlaying)
            return;
        CleanUpAllInteractions();
        Editor_ActivateInteractions(StartIndex);
    }
#endif
    void NextInteraction(){
        interactionIndex ++;
        if(interactionIndex >= interactionControllers.Length){
            interactionIndex --;
            Debug.LogWarning("No Interaction");
            if(interactionIndex >= interactionControllers.Length-1){
                StartCoroutine(CommonCoroutine.delayAction(()=>GameManager.Instance.SwitchingScene(endSceneName), endDelay));
            }
            LevelProgressionManager.Instance.SetProgress(interactionIndex);
            return;
        }
    //Make sure to load interaction before enter
        if(!interactionControllers[interactionIndex].m_isLoaded) 
        {
            PreloadInteraction(interactionControllers[interactionIndex]);
        }
        interactionControllers[interactionIndex].EnterInteraction();

    //If no other interaction loaded, treat current game state as transition end.
        if(loadedIC_Count == 0) EventHandler.Call_OnTransitionEnd();
        loadedIC_Count ++;

        LevelProgressionManager.Instance.SetProgress(interactionIndex);
    }
    void EndInteraction(IC_Basic interController){
        interController.ExitInteraction();
        loadedIC_Count --;
        loadedIC_Count = Mathf.Max(0, loadedIC_Count);
    //If no loaded interaction, treat current game state as transition.
        if(loadedIC_Count == 0) EventHandler.Call_OnTransitionBegin();
    }
    void CleanUpInteraction(IC_Basic interController){
        Debug.Log("Cleanup Interaction: "+interController.name);
        if(!interController.m_isDone) EndInteraction(interController);
        interController.CleanUpInteraction();
    }
    void PreloadInteraction(IC_Basic interController)=>interactionControllers[interactionIndex].PreloadInteraction();

#region Tool Function
    public IC_Basic GetMiniGame(int index)=>interactionControllers[index];
    public void StartAtInteraction(int startIndex){
        CleanUpAllInteractions();

        interactionIndex = startIndex;
        LevelProgressionManager.Instance.SetProgress(interactionIndex);
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

    void Debug_Progress(InputAction.CallbackContext context)
    {
        if(GameManager.Instance.IsSwitchingScene)
        {
            Debug.LogWarning("Scene is switching, cannot proceed to next interaction.");
            return;
        }
        int nextIndex = interactionIndex + 1;
        nextIndex = Mathf.Clamp(nextIndex, 0, interactionControllers.Length-1);
        LevelProgressionManager.Instance.SetProgress(nextIndex);
        EndInteraction(interactionControllers[interactionIndex]);
        CleanUp();
        GameManager.Instance.RestartLevel();
    }
    void Debug_Regress(InputAction.CallbackContext context)
    {
        if(GameManager.Instance.IsSwitchingScene)
        {
            Debug.LogWarning("Scene is switching, cannot regress to previous interaction.");
            return;
        }
        int nextIndex = interactionIndex - 1;
        nextIndex = Mathf.Clamp(nextIndex, 0, interactionControllers.Length-1);
        LevelProgressionManager.Instance.SetProgress(nextIndex);
        EndInteraction(interactionControllers[interactionIndex]);
        CleanUp();
        GameManager.Instance.RestartLevel();
    }
    void Debug_RestartLevel(InputAction.CallbackContext callback){
        if(callback.ReadValueAsButton())
        {
            if(GameManager.Instance.IsSwitchingScene)
            {
                Debug.LogWarning("Scene is switching, cannot restart level.");
                return;
            }
            CleanUp();
            GameManager.Instance.RestartLevel();
        }
    }
    void Debug_Reset(InputAction.CallbackContext callback){
        if(callback.ReadValueAsButton())
        {
            if(GameManager.Instance.IsSwitchingScene)
            {
                Debug.LogWarning("Scene is switching, cannot reset.");
                return;
            }
            CleanUp();
            GameManager.Instance.SwitchingScene("Intro");
        }
    }
    void CleanUp()
    {
        EventHandler.Call_OnFlushInput();
        foreach (var interController in interactionControllers)
        {
            if(interController.m_isPlaying)
            {
                interController.ExitInteraction();
            }
            if(interController.m_isLoaded)
            {
                interController.CleanUpInteraction();
            }
        }
        ambienceHandler.CleanUp();
        PhysicDragManager.Instance.CleanUp();
    }
#if UNITY_EDITOR
    public void Editor_CleanUpInteractions()
    {
        for (int i = 0; i < interactionControllers.Length; i++)
        {
            interactionControllers[i].Editor_CleanUpInteraction();
        }
    }
    public void Editor_ActivateInteractions(int index)
    {
        interactionControllers[index].Editor_LoadInteraction();
    }
#endif
}
