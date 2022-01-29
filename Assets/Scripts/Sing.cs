using UnityEngine;

public class Sing : MonoBehaviour
{
    private float StartPos;

    private Transform cam;

    public float ParallaxEffect;
    void Start()
    {
        StartPos = transform.position.x;
        cam = Camera.main.transform;
    }

    void Update()
    {
        float Distance = cam.transform.position.x * ParallaxEffect;
        transform.position = new Vector3(StartPos + Distance, transform.position.y, transform.position.z);

    }
}
