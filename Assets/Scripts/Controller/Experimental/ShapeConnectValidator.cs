using UnityEngine;

public static class ShapeConnectValidator
{
    public static bool ValidateBodyAt(ConnectBody body, ConnectBody pendingBody, Vector3 position, Quaternion rotation)
    {
        Collider source = body.m_geoCollider;
        GameObject probeObject = new GameObject("TempConnectBodyProbe");
        probeObject.hideFlags = HideFlags.DontSave;

        // Create a probe root at the requested pose.
        probeObject.transform.SetPositionAndRotation(position, rotation);
        probeObject.transform.localScale = body.transform.lossyScale;

        Transform probeColliderRoot = probeObject.transform;
        // Clone the source collider object with Instantiate and strip unrelated components.
        Collider probeCollider = GameObject.Instantiate(source);
        Transform clonedTransform = probeCollider.transform;
        clonedTransform.SetParent(probeColliderRoot, false);
        clonedTransform.localPosition = source.transform.localPosition;
        clonedTransform.localRotation = source.transform.localRotation;
        clonedTransform.localScale = source.transform.localScale;
        probeCollider.hideFlags = HideFlags.DontSave;
        probeCollider.gameObject.hideFlags = HideFlags.DontSave;

        if (probeCollider is MeshCollider probeMeshCollider)
            probeMeshCollider.convex = true;

        probeCollider.isTrigger = true;
        Physics.SyncTransforms();

        float searchRadius = probeCollider.bounds.extents.magnitude + 0.01f;
        Collider[] candidates = Physics.OverlapSphere(
            probeCollider.bounds.center,
            searchRadius,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < candidates.Length; i++)
        {
            Collider other = candidates[i];
            if (other == probeCollider)
                continue;

            var detectBody = other.GetComponentInParent<ConnectBody>();
            if(detectBody == null)
                continue;
            if (detectBody == body)
                continue;
            if(detectBody!=pendingBody && !detectBody.IsConnectedToBody(pendingBody))
                continue;
            if (Physics.ComputePenetration(
                probeCollider,
                probeCollider.transform.position,
                probeCollider.transform.rotation,
                other,
                other.transform.position,
                other.transform.rotation,
                out _,
                out _
            ))
            {
                GameObject.Destroy(probeObject);
                return false;
            }
        }

        GameObject.Destroy(probeObject);
        return true;
    }
}
