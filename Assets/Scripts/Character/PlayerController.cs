using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    //Agent���
    NavMeshAgent agent;

    //��������ļ�
    CharacterState characterState;

    //�������
    Animator anim;

    //�������˱���
    GameObject attactEnemy;
    //������ҡʱ��
    //public float lastAttactTime = 1;
    float attactTimeCount; 

    //һ����Awake�л�ȡ����ʵ��
    private void Awake()
    {
        //��ȡ�������NavMeshAgent����������޸ĵĵ�destination����
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterState = GetComponent<CharacterState>();
    }

    private void Start()
    {
        //event�¼�ͨ�� "+=" ��ע�᷽��
        MouseController.Instance.onMouseClick += MovetoTarget;

        MouseController.Instance.onClickEnemy += EventAttact;
        
        characterState.MaxBlood = 2;

        //��ʼ��������ҡʱ��
        attactTimeCount = characterState.attactData.coolDown;
    }


    private void Update()
    {
        SwitchAniamtion();

        //������ҡ����ʱ
        if(attactTimeCount > 0)
            attactTimeCount -= Time.deltaTime;
    }

    void SwitchAniamtion()
    {
        //ֱ��ʹ��agent�е��ٶȱ���velocity����ֵ
        //sqrMagnitude�����ǽ�Vector3��velocityת����float����
        anim.SetFloat("speed", agent.velocity.sqrMagnitude);
    }

    //�����ƶ�����ע�ᵽ�����������¼��У�ÿ���¼�ִ�ж�������������
    void MovetoTarget(Vector3 target)
    {
        //����ȡ������������һ��ѡ�е������޷�ֹͣ
        StopAllCoroutines();

        //��ֹ���ֹͣ�ƶ�һ�κ����ܹ��ƶ�
        agent.isStopped = false;

        //�ƶ�λ��
        agent.destination = target;
    }

    //���������¼�
    private void EventAttact(GameObject obj)
    {
        
        if (obj != null)
        {
            //�ñ�����ס��ȡ����ʵ����в��������Ӵ��뽡׳��
            attactEnemy = obj;

            //ʹ��Э�̽�������ƶ���������ǰ�Ĳ���
            //���ֱ���ڷ�����ʹ��Whileѭ�����о����жϲ����ƶ�����һ�һ˲���ƶ���������ǰ
            StartCoroutine(moveToTarget(attactEnemy));
        }
    }

    IEnumerator moveToTarget(GameObject attactTarget)
    {
        //��ֹ���ֹͣ�ƶ�һ�κ����ܹ��ƶ�
        agent.isStopped = false;

        //ʵʱ�����ҳ������
        transform.LookAt(attactTarget.transform);

        //�ƶ�����ҹ��������λ��
        //characterState.attactData.attactRange��ȡ����ҵĹ�����������
        while (Vector3.Distance(transform.position, attactTarget.transform.position) > characterState.attactData.attactRange)
        {
            agent.destination = attactTarget.transform.position;
            yield return null;    //�˴�����Ҫ�ȴ�ʱ�䣬û���ƶ���Ŀ�ĵ�֮ǰ����һֱ�ƶ�
        }

        //ִ����ѭ��˵���ƶ���Ŀ�ĵأ�ֱ��ֹͣ�ƶ�
        agent.isStopped = true;
        //������ҡ����CD�У����Ź�������
        if (attactTimeCount <= 0)
        {
            characterState.isCritical = Random.value < characterState.attactData.criticalChance;
            anim.SetBool("critical", characterState.isCritical);
            anim.SetTrigger("attact");
            //���ù�����ҡʱ��
            attactTimeCount = characterState.attactData.coolDown;
        }
    }
}
