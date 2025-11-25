using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // allows >65k vertices
        combinedMesh.CombineMeshes(combine);

        // Create GameObject to hold combined mesh
        GameObject combinedObj = new GameObject("CombinedMesh");
        combinedObj.transform.position = transform.position;
        combinedObj.transform.rotation = transform.rotation;
        combinedObj.transform.localScale = transform.localScale;

        // Add Mesh components
        MeshFilter mf = combinedObj.AddComponent<MeshFilter>();
        MeshRenderer mr = combinedObj.AddComponent<MeshRenderer>();
        MeshCollider mc = combinedObj.AddComponent<MeshCollider>();

        mf.sharedMesh = combinedMesh;
        mc.sharedMesh = combinedMesh;

        // Give it the same material as the parent
        mr.material = GetComponentInChildren<MeshRenderer>().sharedMaterial;
    }
}
