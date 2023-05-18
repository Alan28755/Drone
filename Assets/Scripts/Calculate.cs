using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculate : MonoBehaviour
{
    private Mesh _mesh;
    // Start is called before the first frame update
    void Start()
    {
        _mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            int versCount = _mesh.vertexCount;
            Color[] colors = new Color[versCount];
            for (int i = 0; i < versCount; i++)
            {
                colors[i] = new Color(1, 0, 0, 1);
            }

            _mesh.colors = colors;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            int versCount = _mesh.vertexCount;
            Color[] colors = new Color[versCount];
            for (int i = 0; i < versCount; i++)
            {
                colors[i] = new Color(0, 1, 0, 1);
            }

            _mesh.colors = colors;
        }

    }

}
