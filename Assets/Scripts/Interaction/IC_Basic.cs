using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("InteractionController")]
public abstract class IC_Basic : MonoBehaviour
{
    [SerializeField] protected GameObject interactionAssetsGroup;

    public bool m_isLoaded{get; private set;} = false;
    public bool m_isDone{get; private set;} = false;
    public bool m_isPlaying{get; private set;} = false;

//This might be an async function
    public void PreloadInteraction(){
        if(interactionAssetsGroup!=null) interactionAssetsGroup.SetActive(true);
        LoadAssets();
    }
    public void EnterInteraction(){
        m_isPlaying = true;
        OnInteractionStart();
    }
    public void ExitInteraction(){
        OnInteractionEnd();
        m_isDone = true;
        m_isPlaying = false;
    }
//This might be an async function
    public void CleanUpInteraction(){
        if(interactionAssetsGroup!=null) interactionAssetsGroup.SetActive(false);
        UnloadAssets();
    }
    protected virtual void LoadAssets(){m_isLoaded = true;}
    protected virtual void UnloadAssets(){m_isLoaded = false;}
    protected virtual void OnInteractionStart(){}
    protected virtual void OnInteractionEnd(){}
}
