using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Splines;

public class HiddenTube : MonoBehaviour
{
    [SerializeField] private Transform entrance;
    [SerializeField] private Transform exit;
    [SerializeField] private SplineContainer path;

    [SerializeField] private Animation pathAnimation;

    private const string PATH_ANIME = "EVO_Path";
    private const string PATH_ANIME_REVERSE = "EVO_Path_Reverse";
    
    private bool isBallTravelling = false;

    public bool TryStartTravelling(BounceBall ball, bool reversePath)
    {
        if (!isBallTravelling)
        {
            StartCoroutine(coroutineTravel(ball, path, reversePath));
            return true;
        }
        return false;
    }

    IEnumerator coroutineTravel(BounceBall ball, SplineContainer path, bool reversePath)
    {
        isBallTravelling = true;
        
        Vector3 startPos = ball.transform.position;
        SplineUtility.GetNearestPoint(path.Spline, (float3)startPos, out float3 nearest, out float startRatio);
        float targetRatio = reversePath?0:1;
        ball.PhysicsSleep();
        ball.enabled = false;
        
        Vector3 startTangent = path.EvaluateTangent(1-targetRatio);
        Quaternion startTangentRotation = Quaternion.FromToRotation(startTangent, Vector3.up);

        pathAnimation.Play(reversePath ? PATH_ANIME_REVERSE : PATH_ANIME);
        yield return new WaitForLoop(.2f, (t) =>
        {
            float ratio = Mathf.Lerp(1-targetRatio, targetRatio, t*t);
            ball.transform.position = path.EvaluatePosition(ratio);
            Vector3 tangent = path.EvaluateTangent(ratio);
            ball.ShapeBallToVel(tangent*20);
        });
        ball.transform.position = path.EvaluatePosition(targetRatio);
        yield return null;
        ball.WakePhysics();
        
        Transform finalTrans = reversePath?entrance:exit;
        ball.Launch(finalTrans.up, 1);
        ball.enabled = true;
        yield return new WaitForSeconds(0.2f);
        isBallTravelling = false;
    }
}