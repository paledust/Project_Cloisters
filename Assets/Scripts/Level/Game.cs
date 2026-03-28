using UnityEngine;

public class Game : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventHandler.Call_OnTransitionEnd();
    }
}
