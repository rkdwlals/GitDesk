using UnityEngine;
using Cinemachine;
using System.Collections;

/// <summary>
/// 모니터 클릭 시 해당 카메라(월드뷰)가 활성화된 상태에서만 줌 인 및 로딩→잠금 시퀀스를 실행합니다.
/// 줌인된 상태에서는 ZoomOut만 동작하도록 처리했습니다.
/// </summary>
public class MonitorZoomController : MonoBehaviour
{
    [Header("Virtual Cameras")]
    public CinemachineVirtualCamera worldCam;
    public CinemachineVirtualCamera monitorCam;
    public float zoomPriority = 20f;
    public float normalPriority = 10f;

    [Header("UI Screens")]
    [Tooltip("클릭 후 1초 뒤 자동으로 켤 로딩화면")]
    public GameObject loadingScreenUI;
    [Tooltip("로딩 끝나고 켤 잠금화면")]
    public GameObject lockScreenUI;
    [Tooltip("로딩화면 노출까지 딜레이 (줌인 애니메이션 포함)")]
    public float loadingScreenDelay = 1f;
    [Tooltip("로딩화면 노출 지속시간")]
    public float loadingDuration = 3f;

    // 시퀀스 실행 플래그
    bool isSequenceRunning = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isSequenceRunning)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 월드 카메라가 활성화된(줌아웃) 상태에서만 ZoomIn 실행
                if (hit.collider.CompareTag("Monitor") && worldCam.Priority > monitorCam.Priority)
                {
                    ZoomIn();
                }
                // 모니터 줌인 상태에서 화면 바깥 클릭 시 ZoomOut
                else if (hit.collider == null && monitorCam.Priority > worldCam.Priority)
                {
                    ZoomOut();
                }
            }
        }
    }

    /// <summary>
    /// 모니터 카메라 우선순위 변경으로 줌인, 로딩 시퀀스 시작
    /// </summary>
    void ZoomIn()
    {
        monitorCam.Priority = (int)zoomPriority;
        worldCam.Priority = (int)normalPriority;

        StartCoroutine(ShowLoadingAndLockSequence());
    }

    /// <summary>
    /// 월드 카메라로 복귀
    /// </summary>
    void ZoomOut()
    {
        monitorCam.Priority = 0;
        worldCam.Priority = (int)normalPriority;
    }

    /// <summary>
    /// 로딩 화면 노출 및 잠금 화면 전환 시퀀스
    /// </summary>
    IEnumerator ShowLoadingAndLockSequence()
    {
        isSequenceRunning = true;

        // 딜레이 (줌인 애니메이션 + 추가 딜레이)
        yield return new WaitForSeconds(loadingScreenDelay);

        // 로딩 화면 활성화
        if (loadingScreenUI != null)
            loadingScreenUI.SetActive(true);

        // 스피너 회전 및 대기
        yield return new WaitForSeconds(loadingDuration);

        // 로딩 화면 비활성화, 잠금 화면 활성화
        if (loadingScreenUI != null)
            loadingScreenUI.SetActive(false);
        if (lockScreenUI != null)
            lockScreenUI.SetActive(true);

        isSequenceRunning = false;
    }
}
