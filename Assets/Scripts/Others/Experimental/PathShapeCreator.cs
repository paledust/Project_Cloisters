using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class PathShapeCreator : PathSceneTool
{
[Header ("River settings")]
    public float riverWidth = .4f;
    public bool flattenSurface;
[Header ("Material settings")]
    [SerializeField] private Material roadMaterial;
    [SerializeField] private float textureTiling = 1;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    protected override void PathUpdated () {
        if (pathCreator != null) {
            AssignMeshComponents ();
            AssignMaterials ();
            CreateMesh ();
        }
    }
    void CreateMesh () {
        Vector3[] verts = new Vector3[path.NumPoints * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];
        int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
        int[] roadTriangles = new int[numTris * 3];
        int vertIndex = 0;
        int triIndex = 0;
    // Vertices for the top of the road are layed out:
    // 0  1
    // 2  3
        int[] triangleMap = { 0, 2, 1, 1, 2, 3 };
        bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);
        for (int i = 0; i < path.NumPoints; i++) {
            Vector3 localUp = usePathNormals ? Vector3.Cross (path.GetTangent (i), path.GetNormal (i)) : path.up;
            Vector3 localRight = usePathNormals ? path.GetNormal (i) : Vector3.Cross (localUp, path.GetTangent (i));
            // Find position to left and right of current path vertex
            Vector3 vertSideA = path.GetPoint (i) - localRight * riverWidth;
            Vector3 vertSideB = path.GetPoint (i) + localRight * riverWidth;
            // Add top of road vertices
            verts[vertIndex + 0] = vertSideA - transform.position;
            verts[vertIndex + 1] = vertSideB - transform.position;
            // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
            uvs[vertIndex + 0] = new Vector2 (path.times[i], 0);
            uvs[vertIndex + 1] = new Vector2 (path.times[i], 1);
            // Top of road normals
            normals[vertIndex + 0] = localUp;
            normals[vertIndex + 1] = localUp;
            // Set triangle indices
            if (i < path.NumPoints - 1 || path.isClosedLoop) {
                for (int j = 0; j < triangleMap.Length; j++) {
                    roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                }
            }
            vertIndex += 2;
            triIndex += 6;
        }
        mesh.Clear ();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.subMeshCount = 3;
        mesh.SetTriangles (roadTriangles, 0);
        mesh.RecalculateBounds ();
    }
    // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
    void AssignMeshComponents () {
        // Ensure mesh renderer and filter components are assigned
        if (!gameObject.GetComponent<MeshFilter> ()) {
            gameObject.AddComponent<MeshFilter> ();
        }
        if (!GetComponent<MeshRenderer> ()) {
            gameObject.AddComponent<MeshRenderer> ();
        }
        meshRenderer = GetComponent<MeshRenderer> ();
        meshFilter = GetComponent<MeshFilter> ();
        if (mesh == null) {
            mesh = new Mesh ();
        }
        meshFilter.sharedMesh = mesh;
    }
    void AssignMaterials () {
        if (roadMaterial != null) {
            meshRenderer.sharedMaterial = roadMaterial;
            meshRenderer.sharedMaterial.mainTextureScale = new Vector3 (1, textureTiling);
        }
    }
}
