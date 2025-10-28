using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShaderNoiseMotionReduce : MonoBehaviour
{
    [SerializeField] private float motionReduce = 0;
    private SpriteRenderer m_sprite;
    private const string NoiseUVOffsetName = "_NoiseUVOffset";
    void Start()
    {
        m_sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector2 offset = transform.position;
        offset *= -motionReduce;
        m_sprite.material.SetVector(NoiseUVOffsetName, offset);
    }
}