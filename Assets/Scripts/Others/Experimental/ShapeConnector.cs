using UnityEngine;

public class ShapeConnector : MonoBehaviour
{
    [SerializeField] private bool circularShape;
    [SerializeField] private Clickable_Moveable moveable;
    [SerializeField] private ShapeConnectPoint[] m_connectors;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if(moveable.isControlling)
        {

        }
        else
        {
            
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        foreach(var connector in m_connectors)
        {
            Vector3 right = transform.InverseTransformVector(connector.connectCenter.right);
            Gizmos.DrawSphere(connector.connectCenter.localPosition, 0.2f);
            Gizmos.DrawLine(connector.connectCenter.localPosition - right.normalized*connector.connectLength*0.5f, 
                            connector.connectCenter.localPosition + right.normalized*connector.connectLength*0.5f);
        }
    }
}
