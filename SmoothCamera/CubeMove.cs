
using UnityEngine;

public class CubeMove : MonoBehaviour
{
    public float MoveSpeed;
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += Vector3.right * MoveSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position -= Vector3.right * MoveSpeed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += Vector3.forward * MoveSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= Vector3.forward * MoveSpeed;
        }
    }
}
