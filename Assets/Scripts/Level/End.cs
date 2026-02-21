using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    public void AE_Restart()
    {
        GameManager.Instance.SwitchingScene(gameSceneName);
    }
}
