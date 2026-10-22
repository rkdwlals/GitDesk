using UnityEngine;

public class SpinnerRotate : MonoBehaviour
{
    [Tooltip("회전 속도 (도/초)")]
    public float rotationSpeed = 180f;

    void Update()
    {
        // 로컬 Z축을 축으로, RectTransform 중심을 기준으로 시계 방향 회전
        transform.Rotate(
            0f,
            0f,
            -rotationSpeed * Time.deltaTime,  // 음수면 시계 방향
            Space.Self                         // 로컬 축 기준
        );
    }
}
