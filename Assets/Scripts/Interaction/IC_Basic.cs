using UnityEngine;

[AddComponentMenu("InteractionController")]
public abstract class IC_Basic : MonoBehaviour
{
    [SerializeField] protected GameObject interactionAssetsGroup;
[Header("Interaction")]
    [SerializeField] protected Vector2 interact_rect;
    [SerializeField] protected float interact_depth;

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
        OnInteractionEnter();
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
    protected virtual void OnInteractionEnter(){}
    protected virtual void OnInteractionEnd(){}
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0,1,0,0.5f);
        Vector3 pos = Camera.main.transform.position;
        pos += Camera.main.transform.forward*interact_depth;
        Gizmos.DrawWireCube(pos, new Vector3(interact_rect.x, interact_rect.y, 0.01f));
    }
}
