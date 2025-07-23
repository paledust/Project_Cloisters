using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_BurstBall : Basic_Clickable
{
    [SerializeField] private ParticleSystem p_burst;
    [SerializeField] private ParticleSystem p_star;
    [SerializeField] private float burstRadius = 1.5f;
    [SerializeField] private float bounceSpeed = 2f;
    [SerializeField] private string ballTag = "BounceBall";
    [Header("Time")]
    [SerializeField] private float stopTime = 0.25f;
    [SerializeField] private float stopDuration = 0.1f;

    private bool isCooling = false;

    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        if (isCooling) return;
        Vector3 pos = hitPos;
        pos.z = transform.position.z;

        var collider = Physics.OverlapSphere(pos, burstRadius, 1 << Service.DefaultLayer);
        foreach (var go in collider)
        {
            if (go.tag == ballTag)
            {
                Vector2 diff = go.transform.position - pos;
                float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                if (angle < 0)
                    angle += 360;
                angle = Mathf.Round(angle / 60) * 60;
                go.GetComponent<BounceBall>().Bounce(Quaternion.Euler(0, 0, angle) * Vector2.right, bounceSpeed, AttributeModifyType.Add);
                p_star.transform.position = go.transform.position;
                p_star.Play();
                StartCoroutine(coroutineCool());
                break;

            }
        }

        p_burst.transform.position = pos;
        p_burst.Play();
    }
    IEnumerator coroutineCool()
    {
        isCooling = true;
        Time.timeScale = stopTime;
        yield return new WaitForSecondsRealtime(stopDuration);
        Time.timeScale = 1f;
        isCooling = false;
    }
}
