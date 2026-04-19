using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWaterSimulation : MonoBehaviour {
    public CustomRenderTexture texture;
    public int iterationPerFrame = 5;
    public float ForceThreshold = 0.1f;
    private int ForceID = Shader.PropertyToID("_Force");
    void Start()
    {
        texture.Initialize();
    }

    void FixedUpdate()
    {
        texture.material.SetFloat(ForceID, ForceThreshold);
        texture.Update(iterationPerFrame);
    }
}
