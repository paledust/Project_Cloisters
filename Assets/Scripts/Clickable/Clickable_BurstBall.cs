using System;
using System.Collections;
using UnityEngine;

[Obsolete]
public class Clickable_BurstBall : Basic_Clickable
{
    [SerializeField] private ParticleSystem p_burst;
    [SerializeField] private ParticleSystem p_star;
    [SerializeField] private float burstRadius = 1.5f;
    [SerializeField] private float bounceSpeed = 2f;
    [SerializeField] private string ballTag = "BounceBall";

    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
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
                angle = Mathf.Round(angle / 30) * 30;
                go.GetComponent<BounceBall>().Bounce(Quaternion.Euler(0, 0, angle) * Vector2.right, bounceSpeed, 1, AttributeModifyType.Add);
                p_star.transform.position = go.transform.position;
                p_star.Play();
                break;

            }
        }

        p_burst.transform.position = pos;
        p_burst.Play();
    }
}