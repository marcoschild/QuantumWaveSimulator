using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.Translate(new Vector3(horizontal, 0, vertical));
    }
}
