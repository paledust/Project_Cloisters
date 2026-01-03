using UnityEngine;
using UnityEngine.InputSystem;

public class NarrativeRandomForceField : MonoBehaviour
{
    [SerializeField] private Vector2 phase;
    [SerializeField] private float noiseScroll;
    [SerializeField] private float noiseScale;
    [SerializeField] private float noiseAmp;
    [SerializeField, ShowOnly] private Vector2 offset;
    void OnEnable()
    {
        offset = Vector2.zero;
    }
    void LateUpdate()
    {
        offset += Vector2.one * Time.deltaTime * noiseScroll;
    }
    public void ApplyForce(Rigidbody rigid, float multi)
    {
        float noise = Mathf.PerlinNoise(rigid.position.x * noiseScale + offset.x + phase.x, rigid.position.y * noiseScale + offset.y + phase.y);
        float angle = (noise * 2 - 1) * Mathf.PI * 2;
        Vector3 force = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * noiseAmp * multi;
        rigid.AddForce(force, ForceMode.VelocityChange);
    }
    // void OnGUI()
    // {
    //     Vector3 mouseScr = Mouse.current.position.ReadValue();
    //     mouseScr.z = 35;
    //     Vector2 pos = Camera.main.ScreenToWorldPoint(mouseScr);
    //     float value = Mathf.PerlinNoise(pos.x * noiseScale + offset.x + phase.x, pos.y * noiseScale + offset.y + phase.y);
    //     float angle = (value * 2 - 1) * Mathf.PI * 2;
    //     angle = angle * Mathf.Rad2Deg;
    //     GUI.Label(new Rect(10, 10, 300, 20), "Perlin Noise Value at Mouse: " + angle.ToString("F3"));
    // }
}
