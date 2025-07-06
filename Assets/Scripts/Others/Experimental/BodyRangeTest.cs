using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRangeTest : MonoBehaviour
{
    [System.Serializable]
    private enum TestState
    {
        Outside,
        Intersect,
        Inside,
    }
    [SerializeField, ShowOnly] private TestState currentState;
    [SerializeField] private ConnectBody m_body;
    [SerializeField] private Transform[] points;
    private float testTimer = 0;
    private RangeDetection rangeDetector;

    private const float TEST_PER_SEC = 10;

    void Awake()
    {
        currentState = TestState.Outside;
        m_body.ChangeActivateState(false);
    }
    void OnEnable() => testTimer = 0;
    void Update()
    {
        if (currentState != TestState.Outside)
        {
            testTimer += Time.deltaTime * TEST_PER_SEC;
            if (testTimer >= 1)
            {
                testTimer = 0;
                bool flag = true;
                foreach (var point in points)
                {
                    if (!rangeDetector.CheckPoint(point.position))
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    if (currentState != TestState.Inside)
                    {
                        currentState = TestState.Inside;
                        m_body.ChangeActivateState(true);
                    }
                }
                else
                {
                    if (currentState != TestState.Intersect)
                    {
                        currentState = TestState.Intersect;
                        m_body.ChangeActivateState(false);
                    }
                }
            }
        }

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        var range = other.GetComponent<RangeDetection>();
        if (range != null)
        {
            currentState = TestState.Intersect;
            testTimer = 1;
            rangeDetector = range;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        var range = other.GetComponent<RangeDetection>();
        if (range != null)
        {
            currentState = TestState.Outside;
            testTimer = 1;
            rangeDetector = null;
        }
    }
}
