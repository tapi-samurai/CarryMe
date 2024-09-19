using UnityEngine;
using Cysharp.Threading.Tasks;
using Cinemachine;

public class CameraTransition : SingletonMonoBehaviour<CameraTransition>
{
    [SerializeField] GameObject m_cameraStandardPos;        // �J�����̕W���ʒu
    [SerializeField] GameObject m_cameraFadeInStartPos;     // �J�������t�F�[�h�C������n���ʒu
    [SerializeField] GameObject m_cameraFadeOutEndPos;      // �J�������t�F�[�h�A�E�g�����~�ʒu
    [SerializeField] GameObject m_lookAt;                   // �J�����̒����_
    [SerializeField] float m_fadeInTime;        // �t�F�[�h�C���ɂ����鎞��
    [SerializeField] float m_fadeOutTime;       // �t�F�[�h�A�E�g�ɂ����鎞��
    [SerializeField] float m_transitionDelay;   // �g�����W�V�������n�܂�܂ł̒x������

    CinemachineVirtualCamera m_cinemachineVCam;
    CinemachineInputProvider m_cinemachineInputProvider;    // �}�E�X�ŃJ��������]�����Ă���X�N���v�g

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

        // �J��������]�ł��Ȃ��悤�ɂ���
        m_cinemachineInputProvider.enabled = false;
    }

    public async UniTask CameraTransitionFadeIn()
    {
        // �J��������]�ł��Ȃ��悤�ɂ���
        m_cinemachineInputProvider.enabled = false;

        // �J�����̃u�����h���~����
        GetComponent<CinemachineVirtualCameraBase>().PreviousStateIsValid = false;

        // �t�F�[�h�C���̏����ʒu�Ƀ��[�v����
        m_cinemachineVCam.enabled = false;
        m_cinemachineVCam.transform.position = new Vector3(
            m_cinemachineVCam.transform.position.x,
            m_cameraFadeInStartPos.transform.position.y,
            m_cinemachineVCam.transform.position.z);
        m_lookAt.transform.position = m_cameraFadeInStartPos.transform.position;
        m_cinemachineVCam.enabled = true;

        // �g�����W�V�����Ɏg�p����ϐ���������
        m_transitionStartPosY = m_cameraFadeInStartPos.transform.position.y;
        m_transitionEndPosY = m_cameraStandardPos.transform.position.y;
        m_elapsedTime = 0;
        m_transitionTime = m_fadeInTime;
        m_transitionCompleted = false;

        // �g�����W�V�������I���܂őҋ@
        await UniTask.WaitUntil(() => m_transitionCompleted == true);

        // �J�����̃u�����h���ĊJ����
        GetComponent<CinemachineVirtualCameraBase>().PreviousStateIsValid = true;

        // �J��������]�ł���悤�ɂ���
        m_cinemachineInputProvider.enabled = true;
    }

    public async UniTask CameraTransitionFadeOut()
    {
        // �ق߂�R�����g�𗬂�
        CommentControl.instance.ComplimentComment();

        // �J��������]�ł��Ȃ��悤�ɂ���
        m_cinemachineInputProvider.enabled = false;

        // ��莞�Ԓ�~����
        await UniTask.Delay(System.TimeSpan.FromSeconds(m_transitionDelay));

        // �g�����W�V�����Ɏg�p����ϐ���������
        m_transitionStartPosY = m_cameraStandardPos.transform.position.y;
        m_transitionEndPosY = m_cameraFadeOutEndPos.transform.position.y;
        m_elapsedTime = 0;
        m_transitionTime = m_fadeOutTime;
        m_transitionCompleted = false;

        // �g�����W�V�������I���܂őҋ@
        await UniTask.WaitUntil(() => m_transitionCompleted == true);
    }

    // t��0�`1�͈̔͂ɔ����Ȃ�Lerp
    static float Lerp(float begin, float end, float t)
    {
        // t��0�`1�͈̔͂Ȃ�W����Lerp�ŕԂ�
        if(t >= 0 && t <= 1) return Mathf.Lerp(begin, end, t);

        // �����łȂ��Ȃ�v�Z���ĕԂ�
        return (end - begin) * t + begin;
    }

    // �J�����̈ړ��p�C�[�W���O�֐�
    float EaseInOutBack(float from, float to, float x)
    {
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;

        float ease = x < 0.5
          ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
          : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;

        // 0�`1�͈̔͂𒴂��鎩��Lerp�Œl��Ԃ�
        return Lerp(from, to, ease);
    }

    // �Q�[���I�����̉��o
    public void End()
    {
        // �J�����̉�]���~�߂�
        m_cinemachineInputProvider.enabled = false;
    }

    private void FixedUpdate()
    {
        // �g�����W�V�����̕K�v���Ȃ���΃X�L�b�v
        if (m_transitionCompleted == true) return;

        // �g�����W�V�����ɕK�v�Ȏ��Ԃ��o�߂��Ă���΃X�L�b�v
        if (m_elapsedTime > m_transitionTime)
        {
            m_transitionCompleted = true;
            return;
        }

        m_elapsedTime += Time.deltaTime;

        // �o�ߎ��Ԃ���J�����̈ړ��ʂ��o��
        float cameraPosY = EaseInOutBack(m_transitionStartPosY, m_transitionEndPosY, m_elapsedTime / m_transitionTime);

        // �J�����ɓK�p
        m_lookAt.transform.position = new Vector3(m_lookAt.transform.position.x, cameraPosY, m_lookAt.transform.position.z);
    }
}
