using UnityEngine;

// 배경이 카메라를 천천히 따라오게 해서 멀리 있는 느낌을 줌 (패럴랙스)
// factor 0 = 일반 오브젝트처럼 고정,  1 = 카메라에 딱 붙어서 같이 움직임
public class BackgroundParallax : MonoBehaviour
{
    public float factorX = 0.5f;
    public float factorY = 0.3f;

    Transform cam;
    Vector3 startPos;

    private void Start(){
        startPos = transform.position;
        if(Camera.main != null) cam = Camera.main.transform;
    }

    private void LateUpdate(){
        if(cam == null) return;
        transform.position = startPos + new Vector3(cam.position.x * factorX, cam.position.y * factorY, 0f);
    }
}
