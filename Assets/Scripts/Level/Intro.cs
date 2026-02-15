using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private float delayBeforeLoading = 3f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadGameSceneAfterDelay());        
    }
    IEnumerator LoadGameSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoading);
        GameManager.Instance.SwitchingScene(gameSceneName);
    }
}
