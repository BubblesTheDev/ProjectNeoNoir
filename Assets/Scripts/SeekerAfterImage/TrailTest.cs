using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTest : MonoBehaviour
{
    public float activetime = .5f;
    public float meshRefreshRate = 0.20f;
    public float meshDestroyDelay = 0.25f;
    public Transform positionToSpawn;
    public Material mat;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(activateTrail(activetime));

    }


    IEnumerator activateTrail(float timeActive)
    {

        while (timeActive > 0)
        {

            timeActive--;

            if (skinnedMeshRenderers == null)

                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {

                GameObject gOb = new GameObject();
                gOb.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gOb.AddComponent<MeshRenderer>();
                MeshFilter mf = gOb.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);
                mf.mesh = mesh;
                mr.material = mat;

                Destroy(gOb, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }


    }
}