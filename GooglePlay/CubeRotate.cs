
using UnityEngine;

public class CubeRotate : MonoBehaviour
{
    public static bool Rotate;
    public static bool Init;
    
    private float _time = 0;
    private void Update()
    {
        if (!Init) return;
        _time += Time.deltaTime * 100;
        transform.localRotation = Quaternion.Euler(0, _time, 0);

        if(!Rotate) return;
        transform.localRotation = Quaternion.Euler(_time, _time, 0);
    }

    public void Rolling()
    {
        Rotate = !Rotate;
    }
}