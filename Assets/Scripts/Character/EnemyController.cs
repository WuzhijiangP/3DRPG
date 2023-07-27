using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//ʹ��ö�٣���¼ÿ��״̬��Enemy
public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }

//ʹ��RequireComponent��ǽű���
//�Ϳ�����ÿ���ҽӴ˽ű��Ķ������û�����NavMeshAgent�������Զ������NavMeshAgent���
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;

    Animator anim;

    CharacterState characterState;

    //����ö�ٱ���
    private EnemyStates enemyStates;

    [Header("Bisic Setting")]
    //�����ʼ�ƶ��ٶȣ�׷��ʱ����٣�
    float speed;
    //������Unity�༭����ѡ����˵����ͣ���վ׮���ͻ���Ѳ������
    public bool isGuard;
    //���˵Ĺ���Ŀ��
    GameObject attactTarget;
    //������˵Ŀ��ӷ�Χ
    public float sightRadius;
    //�������CD
    float lastAttactTime;

    [Header("Patrol State")]
    //Ѳ�߷�Χ
    public float patrolRange;
    //Ѳ�߷�Χ�����������
    Vector3 patrolPoint;
    //�����ʼλ��
    Vector3 savePosition;
    //Ѳ��ʱ��ͣ��ʱ��
    public float enemyWaitingTime;
    //ͣ��ʱ�䵹��ʱ
    float waitingCount;

    //���Ƶ��˶�����״̬
    bool isIdle, isChase, isFollow;

    private void Awake()
    {
        //��Awake�л�ȡ���
        agent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();

        characterState = GetComponent<CharacterState>();
    }

    private void Start()
    {
        savePosition = transform.position;

        speed = agent.speed;

        waitingCount = enemyWaitingTime;

        //��ʼ��״̬
        if (isGuard)
        {
            //վ׮״̬
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            //Ѳ��״̬
            enemyStates = EnemyStates.PATROL;
            //��ʼ���һ��Ѳ�ߵ㣬��ʼ�ƶ�
            GetRandomPoint();
        }
    }

    private void Update()
    {
        //ʵʱ�޸ĵ���״̬
        SwitchStates();

        SwitchAnimation();

        //����CD�����ʱ��
        if (lastAttactTime > 0)
        {
            lastAttactTime -= Time.deltaTime;
        }
    }

    //����ת����
    void SwitchAnimation()
    {
        //���ݲ�ͬ������ʵʱ���ж���ת��
        anim.SetBool("walk", isIdle);
        anim.SetBool("chase", isChase);
        anim.SetBool("follow", isFollow);
        anim.SetBool("critical", characterState.isCritical);
    }


    //����״̬ת����
    void SwitchStates()
    {
        
        if (FindedPlayer())
        {
            //������ң���״̬�޸ĳ�׷��ģʽ��
            enemyStates = EnemyStates.CHASE;
            Debug.Log("������ң���ʼ׷��");
        }

        //ģ������״̬�������ﵽĳ������ʱ��״̬����ת��
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                
                break;
            case EnemyStates.PATROL:
                //Ѳ��ʱ���ƶ��ٶȱ�Ϊԭ���ٶȵ�һ��
                //ʹ�ó˷����ıȳ�����
                agent.speed = speed * 0.5f;

                if (Vector3.Distance(patrolPoint, transform.position) > agent.stoppingDistance)
                {
                    //�ƶ����µĵ�
                    isIdle = true;
                    agent.destination = patrolPoint;
                }
                else
                {
                    //�ߵ���Ŀ�ĵأ�ֹͣ�ƶ�����Ѱ���µĵ�
                    isIdle = false;

                    if (waitingCount > 0)
                    {
                        //��ʼ����ʱ�ȴ�
                        waitingCount -= Time.deltaTime;
                    }
                    else
                    {
                        GetRandomPoint();
                        //����ʱ��
                        waitingCount = enemyWaitingTime;
                    }
                }

                break;
            case EnemyStates.CHASE:
                //׷��ʱ���ٶȱ�Ϊԭ�����ٶ�
                agent.speed = speed;

                //TODO:������������״̬
                isIdle = false;
                isChase = true;
                //TODO:�������ս�ˣ��ص�ԭ����״̬
                if (!FindedPlayer())
                {
                    isFollow = false;
                    isChase = false;
                    //��ս�����ڹ��ԣ�Enemy����ǰ��һС�ξ��룬��˴˴�ֱ�Ӹ�ֵ���꣬����ֱ����ԭ��ͣ����
                    agent.destination = transform.position;
                    if (waitingCount > 0)
                    {
                        //��ʼ����ʱ�ȴ�
                        waitingCount -= Time.deltaTime;
                    }
                    else if (isGuard)
                    {
                        //�ص�վ׮״̬
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        //�ص�Ѳ��״̬
                        enemyStates = EnemyStates.PATROL;
                        //��ȡ�µĵ�λ
                        GetRandomPoint();
                        //����ʱ��
                        waitingCount = enemyWaitingTime;
                    }

                }
                else
                {
                    //TODO:׷��Player
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attactTarget.transform.position;
                }

                //TODO:����ڹ�����Χ�ڣ��򹥻�
                if (TargetInAttactRange() || TargetInSkillRange())
                {
                    //�ڹ�����Χ�ڣ���ֹͣ���棬���й���
                    isFollow = false;
                    agent.isStopped = true;

                    //������ҡ���������Խ�����һ�ι���
                    if (lastAttactTime <= 0)
                    {
                        //������ҡ
                        lastAttactTime = characterState.attactData.coolDown;

                        //�ж���ι����Ƿ񱩻�
                        //Random.value�᷵��һ������0~1֮���С������ʵ�������жϰٷֱ���ֵ�Ƚ�
                        //��ȡ������ֵС�ڱ����ʣ��򱩻�
                        characterState.isCritical = Random.value < characterState.attactData.criticalChance;

                        //ִ�й���
                        Attact();

                    }
                }

                break;
            case EnemyStates.DEAD:
                
                break;
        }
    }

    //ִ�й�������
    void Attact()
    {
        //���򹥻�Ŀ��
        transform.LookAt(attactTarget.transform);

        if (TargetInAttactRange())
        {
            //���Ž�ս��������
            anim.SetTrigger("attact");
        }
        if (TargetInSkillRange())
        {
            //����Զ�̹�������
            anim.SetTrigger("skillattact");

        }
    }

    //�����ж�����Ƿ��ڽ�ս������Χ��
    bool TargetInAttactRange()
    {
        if (attactTarget != null)
        {
            
            return Vector3.Distance(transform.position, attactTarget.transform.position) <= characterState.attactData.attactRange;
        }
        else
        {
            return false;
        }
    }

    //�����ж�����Ƿ���Զ�̹�����Χ��
    bool TargetInSkillRange()
    {
        if (attactTarget != null)
        {
            //�Ƿ���ڽ�ս������Χ��������Զ�̹�����Χ�ڣ���ֹ��ս״̬��Զ��״̬�ص�
            return (Vector3.Distance(transform.position, attactTarget.transform.position) > characterState.attactData.attactRange)
                    && (Vector3.Distance(transform.position, attactTarget.transform.position) <= characterState.attactData.skillRange);
        }
        else
        {
            return false;
        }
    }

    //�ж��Ƿ������
    bool FindedPlayer()
    {
        //�ҵ���Χ�뾶�ڵ�������ײ��������
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        //ѭ������������ײ��
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                //����Ҷ�Ϊ����Ŀ��
                attactTarget = target.gameObject;
                return true;
            }
        }

        //�����ʧ���Ŀ��
        attactTarget = null;
        return false;
    }

    //����Ѳ�߷�Χ
    private void OnDrawGizmosSelected()
    {
        //��ʾѲ�߷�Χ��ɫ
        Gizmos.color = Color.blue;
        //ʹ��DrawWireSphere�������Ƴ�������һ������
        //ʹ��DrawSphere�������Ƴ�����һ��ʵ�ĵ�Բ�����¿�����Enemy
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }

    //��ȡѲ�������
    void GetRandomPoint()
    {
        //ƽ��ֻ�漰��X���Z�ᣬ��˻�ȡ��������������
        //���ֵ��Χ��Ѳ�߷�Χ��������ֵ
        float PatrolX = Random.Range(-patrolRange, patrolRange);
        float PatrolZ = Random.Range(-patrolRange, patrolRange);

        //�����µĵ�λ
        //Y�᲻ʹ��savePosition��y���꣬����ʹ�õ�ǰposition��y����
        //��Ϊ����ߵ����»���̨���ϣ�y����һֱ���ֲ���ᵼ�����գ�������Ҫʵʱ����
        Vector3 newPoint = new Vector3(savePosition.x + PatrolX, transform.position.y, savePosition.z + PatrolZ);

        //���Enemy�����ϰ����ϵ�����
        //�ж��µ�λ�Ƿ��ڲ�������������
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPoint, out hit, patrolRange, 1))
        {
            patrolPoint = newPoint;
        }
        else
        {
            //���򽫲���
            patrolPoint = transform.position;
        }
    }

}
