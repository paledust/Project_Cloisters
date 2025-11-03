using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCharge : MonoBehaviour
{
    [SerializeField] private GameObject glowEffect;
    public void GlowOn()
    {
        glowEffect.SetActive(true);
    }
}
