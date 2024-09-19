using System.Collections.Generic;
using UnityEngine;

// �S����Ă��鎞�̏����̊��N���X
public class CarryAction
{
    protected GameObject m_owner;
    protected GameObject m_parent;
    protected Rigidbody m_rigidbody;

    public virtual void OnStart(GameObject owner, GameObject parent)
    {
        m_owner = owner;
        m_parent = parent;

        m_rigidbody = m_owner.GetComponent<Rigidbody>();

        // �����I�ȋ������~�߂�
        m_rigidbody.isKinematic = true;

        // ���g��S�����I�u�W�F�N�g�̎q�ɂ���
        m_owner.transform.parent = m_parent.transform;
    }

    public virtual void OnUpdate() { }

    public virtual void OnEnd()
    {
        // �e�q�֌W����������
        m_owner.transform.parent = null;

        // �����I�ȋ������ĊJ
        m_rigidbody.isKinematic = false;

        m_rigidbody = null;
        m_owner = null;
        m_parent = null;
    }
}

// Rigidbody�݂̂̃I�u�W�F�N�g���S����Ă���Ƃ��̏���
public class CarryRigid : CarryAction
{
    const float ThrownPower = 7.0f;   // ������΂���鋭��

    BoxCollider m_parentCollider;

    public override void OnStart(GameObject owner, GameObject parent)
    {
        base.OnStart(owner, parent);

        m_parentCollider = m_parent.GetComponent<BoxCollider>();

        // ���W�Ɖ�]��ݒ肷��
        Vector3 pos = Vector3.zero;
        pos.y = m_parentCollider.center.y + (m_parentCollider.size.y / 2) + (m_owner.transform.localScale.y / 2);

        // �v���C���[�̓���Ɉړ�
        m_owner.transform.localPosition = pos;
        m_owner.transform.localRotation = Quaternion.identity;
    }

    public override void OnEnd()
    {
        // �\�ߕ����I�ȋ������ĊJ���Ă���
        m_rigidbody.isKinematic = false;

        // ������΂�
        Vector3 vec = m_parent.transform.forward * ThrownPower / m_rigidbody.mass +
            m_parent.transform.up * ThrownPower / m_rigidbody.mass;
        m_rigidbody.AddForce(vec, ForceMode.Impulse);

        base.OnEnd();
    }
}

// �v���C���[
public class CarryPlayer : CarryAction
{
    const float OffsetY = 0.1f; // �S��������Y���W�I�t�Z�b�g

    Player m_player;

    public override void OnStart(GameObject owner, GameObject parent)
    {
        base.OnStart(owner, parent);

        m_player = m_owner.GetComponent<Player>();
        BoxCollider m_ownerCollider = m_owner.GetComponent<BoxCollider>();
        Vector3 pos = Vector3.zero;

        // Box�Ȃ�v���C���[
        // BoxCollider�����Ă��Ȃ���΃R�����g�ɒS����Ă���
        if(m_parent.TryGetComponent<BoxCollider>(out BoxCollider boxCollider) == true)
        {
            // �S���ł���v���C���[�̓���ɐݒ�
            pos.y = (m_ownerCollider.size.y / 2) + (boxCollider.size.y / 2) + OffsetY;
        }
        else
        {
            // �R���C�_�[�̒��S�ɐݒ�
            pos = new Vector3(0, -m_ownerCollider.size.y, 0);
        }

        // ���W�Ɖ�]��K�p����
        m_owner.transform.localPosition = pos;
        m_owner.transform.localRotation = Quaternion.identity;

        // �ړ��̋@�\���~�߂�
        m_player.enabled = false;
    }

    public override void OnEnd()
    {
        // �����I�ȋ������ĊJ
        m_rigidbody.isKinematic = false;

        // �ړ��̋@�\���ĊJ���ē�����΂�
        m_player.enabled = true;
        m_player.OnThrown();

        base.OnEnd();
    }
}

// �v���y��
public class CarryPropeller : CarryAction
{
    BoxCollider m_parentCollider;
    PropellerRotate m_propellerRotate;
    Player m_player;
    float m_startPos;

    const float MaxHeight = 5.0f;       // �ō����x
    const float MaxRisingTime = 2.0f;   // �ō����x�ɓ��B����܂ł̎���
    float m_elapsedTime;

    public override void OnStart(GameObject owner, GameObject parent)
    {
        base.OnStart(owner, parent);

        m_parentCollider = m_parent.GetComponent<BoxCollider>();
        m_propellerRotate = m_owner.GetComponent<PropellerRotate>();
        m_player = m_parent.GetComponent<Player>();
        m_elapsedTime = 0;
        m_startPos = m_parent.transform.position.y;

        // ���W�Ɖ�]��ݒ肷��
        Vector3 pos = new Vector3(0, m_parentCollider.center.y + m_parentCollider.size.y / 2 + m_owner.transform.localScale.y / 2, 0);
        m_owner.transform.localPosition = pos;
        m_owner.transform.rotation = Quaternion.identity;

        // �v���C���[�̏d�͂𖳌�������
        m_player.IsGravity = false;

        // �v���y���̉�]���J�n����
        m_propellerRotate.OnStart();

        // �v���y���̎g�p���J�E���^�[�ɕ�
        StageCounter.OnUseItem((int)StageCounter.Item.Propeller);
    }

    public override void OnUpdate()
    {
        m_elapsedTime = m_elapsedTime < MaxRisingTime ? m_elapsedTime + Time.deltaTime : MaxRisingTime;

        // ���̍��x�܂ŏ㏸����
        Vector3 parentPos = m_parent.transform.position;
        float t = Mathf.Sqrt(1 - Mathf.Pow(m_elapsedTime / MaxRisingTime - 1, 2));
        parentPos.y = Mathf.Lerp(m_startPos, MaxHeight, t);
        m_parent.transform.position = parentPos;
    }

    public override void OnEnd()
    {
        // �v���C���[�̏d�͂�L��������
        m_player.IsGravity = true;

        // �v���y���̉�]���~����
        m_propellerRotate.OnStop();

        base.OnEnd();
    }
}

// �{��
public class CarryBomb : CarryAction
{
    const float ThrownPower = 5.0f; // ������΂���鋭��

    BoxCollider m_boxCollider;

    public override void OnStart(GameObject owner, GameObject parent)
    {
        base.OnStart(owner, parent);

        m_boxCollider = m_parent.GetComponent<BoxCollider>();

        // ���W�Ɖ�]��ݒ肷��
        Vector3 pos = Vector3.zero;
        pos.y = m_boxCollider.center.y + (m_boxCollider.size.y / 2) + (m_owner.transform.localScale.y / 2);

        m_owner.transform.localPosition = pos;
        m_owner.transform.localRotation = Quaternion.identity;
    }

    public override void OnEnd()
    {
        m_rigidbody.isKinematic = false;

        // ����
        m_owner.GetComponent<Bomb>().OnIgnition();

        // ������΂�
        Vector3 vec = m_parent.transform.forward * ThrownPower + m_parent.transform.up * ThrownPower;
        m_rigidbody.AddForce(vec, ForceMode.Impulse);

        // ���̃I�u�W�F�N�g�����ĂȂ��悤�ɂ���
        m_owner.GetComponent<CarryObject>().enabled = false;

        base.OnEnd();
    }
}

// ��
public class CarryWall : CarryAction
{
    BoxCollider m_parentCollider;
    List<Transform> m_deleteTransformList = new List<Transform>();

    public override void OnStart(GameObject owner, GameObject parent)
    {
        base.OnStart(owner, parent);

        m_parentCollider = m_parent.GetComponent<BoxCollider>();

        // ���W�Ɖ�]��ݒ肷��
        Vector3 pos = Vector3.zero;
        pos.y = m_parentCollider.center.y + (m_parentCollider.size.y / 2) + (m_owner.transform.localScale.y / 2);

        m_owner.transform.localPosition = pos;
        m_owner.transform.localRotation = Quaternion.identity;

        // ��
        StageCounter.OnUseItem((int)StageCounter.Item.Wall);
        
        // �ǃI�u�W�F�N�g�̂���Rigidbody�̂��Ă�����̂͐؂藣��
        GetChildren(owner);
        if(m_deleteTransformList != null)
        {
            foreach(Transform transform in m_deleteTransformList)
            {
                transform.parent = null;

                // �؂藣�����I�u�W�F�N�g���W�߂�
                StageSceneManager.instance.AddStage(transform.gameObject);
            }
            m_deleteTransformList = null;
        }
    }

    void GetChildren(GameObject gameObject)
    {
        // ���ׂĂ̎q�I�u�W�F�N�g��Rigidbody���N������
        Transform children = gameObject.transform.GetComponentInChildren<Transform>();
        if (children.childCount == 0) return;

        foreach (Transform transform in children)
        {
            if(transform.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = false;
                m_deleteTransformList.Add(transform);
            }

            // ���ׂĂ̎q�I�u�W�F�N�g���ċA�֐��ŒT��
            GetChildren(transform.gameObject);
        }
    }

    public override void OnEnd()
    {
        m_rigidbody.isKinematic = false;

        // ������΂�
        float power = 7;
        Vector3 vec = m_parent.transform.forward * power + m_parent.transform.up * power;
        m_rigidbody.AddForce(vec, ForceMode.Impulse);

        base.OnEnd();
    }
}