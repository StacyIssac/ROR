using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBulletController : MonoBehaviour
{
    [SerializeField, Tooltip("移动与旋转")]
    public float MaximumRotationSpeed = 120.0f;
    public float AcceleratedVeocity = 12.8f;
    public float MaximumVelocity = 30.0f;
    public float MaximumLifeTime = 4f;
    public float AccelerationPeriod = 0.5f;

    [HideInInspector]
    public Transform Target = null;        // Ŀ��
    [HideInInspector]
    public float CurrentVelocity = 0.0f;   // ��ǰ�ٶ�

    public int attackValue;

    private float lifeTime = 0.0f;            // ������

    float Velocity;
    float RotationSpeed;

    [Header("伤害")]
    public GameObject popupDamage;
    public GameObject explosion;
    public float critChance;

    private void Start()
    {
        RotationSpeed = Random.Range(MaximumRotationSpeed, MaximumRotationSpeed - 20);
        Velocity = Random.Range(MaximumVelocity, MaximumVelocity - 10);
        AccelerationPeriod = Random.Range(AccelerationPeriod, AccelerationPeriod + 0.4f);
    }

    // ��ը
    private void Explode()
    {
        // �����ɾ���������壬��ʱ�������Ѿ�ɢȥ������ɾ��������
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void Update()
    {


        float deltaTime = Time.deltaTime;
        lifeTime += deltaTime;

        // ��������������ڣ���ֱ�ӱ�ը��
        if (lifeTime > MaximumLifeTime)
        {
            Explode();
            return;
        }

        if (Target == null)
        {
            transform.position += transform.forward * CurrentVelocity * Time.deltaTime;
        }
        else
        {
            // ���㳯��Ŀ��ķ���ƫ������������������ڣ������Ŀ��
            Vector3 offset = ((lifeTime < AccelerationPeriod) && (Target != null)) ? Vector3.up : (Target.position - transform.position).normalized;

            // ���㵱ǰ������Ŀ�귽��ĽǶȲ�
            float angle = Vector3.Angle(transform.forward, offset);

            // ���������ת�ٶȣ�����ת��Ŀ�깲����Ҫ��ʱ��
            float needTime = angle / (RotationSpeed * (CurrentVelocity / Velocity));

            // ����ǶȺ�С����ֱ�Ӷ�׼Ŀ��
            if (Vector3.Distance(Target.position, transform.position) < 30)
            {
                transform.forward = offset;
            }
            else
            {
                // ��ǰ֡���ʱ�������Ҫ��ʱ�䣬��ȡ����Ӧ����ת�ı�����
                transform.forward = Vector3.Slerp(transform.forward, offset, deltaTime / needTime).normalized;
            }

            // �����ǰ�ٶ�С������ٶȣ�����м���
            if (CurrentVelocity < MaximumVelocity)
                CurrentVelocity += deltaTime * AcceleratedVeocity;

            // ���Լ���ǰ��λ��
            transform.position += transform.forward * CurrentVelocity * deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.gameObject.GetComponent<EnemyStatus>().IsAttack(attackValue, critChance);
            collision.gameObject.gameObject.GetComponent<EnemyStatus>().hasAttack = true;
            CreateDamageVal(collision.gameObject.transform.position, attackValue);
        }
        else
        {
            Debug.Log('?');
        }

        Explode();
    }

    void CreateDamageVal(Vector3 pos, int value)
    {
        GameObject mObject = (GameObject)Instantiate(popupDamage, pos, Quaternion.identity);
        mObject.GetComponent<AttackValue>().Value = value;
        mObject.GetComponent<AttackValue>().mTarget = pos;
    }
}
