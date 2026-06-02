using System;
using Cinemachine;
using UnityEngine;

[AddComponentMenu("InteractionController")]
public abstract class IC_Basic : MonoBehaviour
{
    [SerializeField] protected GameObject interactionAssetsGroup;
[Header("Interaction")]
    [SerializeField] protected Vector2 interact_rect;
    [SerializeField] protected float interact_depth;

[Header("Music")]
    [SerializeField] protected AmbienceHandler ambHandler;
    [SerializeField] protected BGMHandler bgmHandler;
    [SerializeField] protected string ambKey;
    [SerializeField] protected string musKey;

    private bool m_hasMusicStarted = false;

    public bool m_isLoaded{get; private set;} = false;
    public bool m_isDone{get; private set;} = false;
    public bool m_isPlaying{get; private set;} = false;

    public virtual void TL_FadeInSound(float crossFadeTime) => FocusMusic(crossFadeTime);
    public void Editor_LoadInteraction()
    {
        if(interactionAssetsGroup!=null) interactionAssetsGroup.SetActive(true);
    }
    public void Editor_CleanUpInteraction()
    {
        if(interactionAssetsGroup!=null) interactionAssetsGroup.SetActive(false);
    }
//This might be an async function
    public void PreloadInteraction(){
        if(interactionAssetsGroup!=null) interactionAssetsGroup.SetActive(true);
        LoadAssets();
    }
    public void EnterInteraction(){
        m_isPlaying = true;
        OnInteractionEnter();
        FocusMusic(1f);
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
    void FocusMusic(float transition)
    {
        if(m_hasMusicStarted) 
            return;
            
        m_hasMusicStarted = true;
        if(!string.IsNullOrEmpty(ambKey))
        {
            if(ambKey == "{stop}")
                ambHandler.FadeOutAmbience(1);
            else
                ambHandler.PlayAmbience(ambKey, 1, transition);
        }
        if(!string.IsNullOrEmpty(musKey))
        {
            if(musKey == "{stop}")
                bgmHandler.FadeOutMusic(1);
            else
                bgmHandler.PlayMusic(musKey, 1, transition);
        }
    }
//When loading resources
    protected virtual void LoadAssets() { m_isLoaded = true; }
//When unloading resources
    protected virtual void UnloadAssets() { m_isLoaded = false; }
//When interaction is started
    protected virtual void OnInteractionEnter() { }
//When interaction is done
    protected virtual void OnInteractionEnd() { }
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0,1,0,0.5f);
        Vector3 pos = Camera.main.transform.position;
        pos += Camera.main.transform.forward*interact_depth;
        Gizmos.DrawWireCube(pos, new Vector3(interact_rect.x, interact_rect.y, 0.01f));
    }
}
