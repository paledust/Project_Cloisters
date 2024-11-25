using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("InteractionController")]
public abstract class IC_Basic : MonoBehaviour
{
    [SerializeField] protected GameObject interactionAssetsGroup;
    public bool m_isDone{get; private set;} = false;
    public bool m_isPlaying{get; private set;} = false;
    
    public void EnterMiniGame(){
        m_isPlaying = true;
        if(interactionAssetsGroup!=null) interactionAssetsGroup.SetActive(true);
        Initialize();
    }
    public void ExitMiniGame(){
        CleanUp();
        m_isDone = true;
        m_isPlaying = false;
    }
    protected virtual void Initialize(){}
    protected virtual void CleanUp(){}
}
