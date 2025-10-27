using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    [Header("Prefab & Settings")]
    public GameObject spherePrefab;
    public int sphereCount = 10;
    public float radius = 5f;

    void Start()
    {
        if (spherePrefab == null)
        {
            Debug.LogError("⚠️ Chưa gán Sphere Prefab vào script!");
            return;
        }

        // Tạo 10 sphere xung quanh vòng tròn
        for (int i = 0; i < sphereCount; i++)
        {
            // Tính góc chia đều 360 độ
            float angle = i * Mathf.PI * 2f / sphereCount;

            // Tính vị trí trên vòng tròn
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 pos = new Vector3(x, 1, z);

            // Sinh ra Sphere
            GameObject sphere = Instantiate(spherePrefab, pos, Quaternion.identity, transform);

            // Đặt màu random
            Renderer rend = sphere.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = new Material(rend.material); // tạo bản sao để không thay đổi prefab
                rend.material.color = Random.ColorHSV(); // random màu
            }

            // Xoay mặt về tâm (nếu muốn)
            sphere.transform.LookAt(transform.position);
        }
    }
}