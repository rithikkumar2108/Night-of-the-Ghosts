using UnityEngine;
using UnityEditor;

public class MeshColliderReplacer : EditorWindow
{
    [MenuItem("Tools/Replace MeshCollider with BoxColliders")]
    private static void ReplaceMeshCollider()
    {
        GameObject selected = Selection.activeGameObject;

        if (!selected)
        {
            Debug.LogWarning("No GameObject selected!");
            return;
        }

        MeshCollider meshCollider = selected.GetComponent<MeshCollider>();
        if (!meshCollider)
        {
            Debug.LogWarning("Selected GameObject has no MeshCollider!");
            return;
        }

        // Remove the MeshCollider
        DestroyImmediate(meshCollider);

        // Check for child meshes
        MeshFilter[] meshFilters = selected.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length > 0)
        {
            foreach (var mf in meshFilters)
            {
                BoxCollider bc = mf.gameObject.AddComponent<BoxCollider>();
                bc.center = mf.sharedMesh.bounds.center;
                bc.size = mf.sharedMesh.bounds.size;
            }
        }
        else
        {
            // Fallback: single collider from renderer bounds
            Renderer rend = selected.GetComponent<Renderer>();
            if (rend)
            {
                BoxCollider bc = selected.AddComponent<BoxCollider>();
                bc.center = rend.bounds.center - selected.transform.position;
                bc.size = rend.bounds.size;
            }
        }

        Debug.Log("Replaced MeshCollider with primitive colliders!");
    }
}
