using UnityEngine;
using Cysharp.Threading.Tasks;
using Cinemachine;

public class CameraTransition : SingletonMonoBehaviour<CameraTransition>
{
    [SerializeField] GameObject m_cameraStandardPos;        // カメラの標準位置
    [SerializeField] GameObject m_cameraFadeInStartPos;     // カメラがフェードインする始動位置
    [SerializeField] GameObject m_cameraFadeOutEndPos;      // カメラがフェードアウトする停止位置
    [SerializeField] GameObject m_lookAt;                   // カメラの注視点
    [SerializeField] float m_fadeInTime;        // フェードインにかかる時間
    [SerializeField] float m_fadeOutTime;       // フェードアウトにかかる時間
    [SerializeField] float m_transitionDelay;   // トランジションが始まるまでの遅延時間

    CinemachineVirtualCamera m_cinemachineVCam;
    CinemachineInputProvider m_cinemachineInputProvider;    // マウスでカメラを回転させているスクリプト

    float m_transitionStartPosY;
    float m_transitionEndPosY;
    float m_elapsedTime;
    float m_transitionTime;
    bool m_transitionCompleted;

    private void Awake()
    {
        m_cinemachineVCam = GetComponent<CinemachineVirtualCamera>();
        m_cinemachineInputProvider = GetComponent<CinemachineInputProvider>();
        m_elapsedTime = 0;
        m_transitionCompleted = true;

        // カメラを回転できないようにする
        m_cinemachineInputProvider.enabled = false;
    }

    public async UniTask CameraTransitionFadeIn()
    {
        // カメラを回転できないようにする
        m_cinemachineInputProvider.enabled = false;

        // カメラのブレンドを停止する
        GetComponent<CinemachineVirtualCameraBase>().PreviousStateIsValid = false;

        // フェードインの初期位置にワープする
        m_cinemachineVCam.enabled = false;
        m_cinemachineVCam.transform.position = new Vector3(
            m_cinemachineVCam.transform.position.x,
            m_cameraFadeInStartPos.transform.position.y,
            m_cinemachineVCam.transform.position.z);
        m_lookAt.transform.position = m_cameraFadeInStartPos.transform.position;
        m_cinemachineVCam.enabled = true;

        // トランジションに使用する変数を初期化
        m_transitionStartPosY = m_cameraFadeInStartPos.transform.position.y;
        m_transitionEndPosY = m_cameraStandardPos.transform.position.y;
        m_elapsedTime = 0;
        m_transitionTime = m_fadeInTime;
        m_transitionCompleted = false;

        // トランジションが終わるまで待機
        await UniTask.WaitUntil(() => m_transitionCompleted == true);

        // カメラのブレンドを再開する
        GetComponent<CinemachineVirtualCameraBase>().PreviousStateIsValid = true;

        // カメラを回転できるようにする
        m_cinemachineInputProvider.enabled = true;
    }

    public async UniTask CameraTransitionFadeOut()
    {
        // ほめるコメントを流す
        CommentControl.instance.ComplimentComment();

        // カメラを回転できないようにする
        m_cinemachineInputProvider.enabled = false;

        // 一定時間停止する
        await UniTask.Delay(System.TimeSpan.FromSeconds(m_transitionDelay));

        // トランジションに使用する変数を初期化
        m_transitionStartPosY = m_cameraStandardPos.transform.position.y;
        m_transitionEndPosY = m_cameraFadeOutEndPos.transform.position.y;
        m_elapsedTime = 0;
        m_transitionTime = m_fadeOutTime;
        m_transitionCompleted = false;

        // トランジションが終わるまで待機
        await UniTask.WaitUntil(() => m_transitionCompleted == true);
    }

    // tが0～1の範囲に縛られないLerp
    static float Lerp(float begin, float end, float t)
    {
        // tが0～1の範囲なら標準のLerpで返す
        if(t >= 0 && t <= 1) return Mathf.Lerp(begin, end, t);

        // そうでないなら計算して返す
        return (end - begin) * t + begin;
    }

    // カメラの移動用イージング関数
    float EaseInOutBack(float from, float to, float x)
    {
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;

        float ease = x < 0.5
          ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
          : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;

        // 0～1の範囲を超える自作Lerpで値を返す
        return Lerp(from, to, ease);
    }

    // ゲーム終了時の演出
    public void End()
    {
        // カメラの回転を止める
        m_cinemachineInputProvider.enabled = false;
    }

    private void FixedUpdate()
    {
        // トランジションの必要がなければスキップ
        if (m_transitionCompleted == true) return;

        // トランジションに必要な時間が経過していればスキップ
        if (m_elapsedTime > m_transitionTime)
        {
            m_transitionCompleted = true;
            return;
        }

        m_elapsedTime += Time.deltaTime;

        // 経過時間からカメラの移動量を出す
        float cameraPosY = EaseInOutBack(m_transitionStartPosY, m_transitionEndPosY, m_elapsedTime / m_transitionTime);

        // カメラに適用
        m_lookAt.transform.position = new Vector3(m_lookAt.transform.position.x, cameraPosY, m_lookAt.transform.position.z);
    }
}
