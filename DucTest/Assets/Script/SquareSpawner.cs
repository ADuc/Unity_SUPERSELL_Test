using UnityEngine;

public class SquareSpawner : MonoBehaviour
{
    [Header("Prefab & Settings")]
    public GameObject cubePrefab;
    public int cubeCount = 20;   
    public float size = 10f;     

    void Start()
    {
        if (cubePrefab == null)
        {
            Debug.LogError("Need Cube Prefab!");
            return;
        }

        int perSide = Mathf.Max(2, cubeCount / 4);

        int totalSpawned = 0;

        for (int side = 0; side < 4; side++)
        {
            int pointCount = (side < 3) ? perSide - 1 : perSide;

            for (int i = 0; i < pointCount; i++)
            {
                float t = i / (float)(perSide - 1);
                Vector3 pos = Vector3.zero;

                switch (side)
                {
                    case 0: 
                        pos = new Vector3(Mathf.Lerp(-size / 2, size / 2, t), 1, -size / 2);
                        break;
                    case 1: 
                        pos = new Vector3(size / 2, 1, Mathf.Lerp(-size / 2, size / 2, t));
                        break;
                    case 2: 
                        pos = new Vector3(Mathf.Lerp(size / 2, -size / 2, t), 1, size / 2);
                        break;
                    case 3: 
                        pos = new Vector3(-size / 2, 1, Mathf.Lerp(size / 2, -size / 2, t));
                        break;
                }

                GameObject cube = Instantiate(cubePrefab, pos, Quaternion.identity, transform);
                Renderer rend = cube.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = new Material(rend.material);
                    rend.material.color = Random.ColorHSV();
                }

                totalSpawned++;
                if (totalSpawned >= cubeCount) return; 
            }
        }
    }
}