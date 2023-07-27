using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    //Agent组件
    NavMeshAgent agent;

    //玩家数据文件
    CharacterState characterState;

    //动画组件
    Animator anim;

    //攻击敌人变量
    GameObject attactEnemy;
    //攻击后摇时间
    //public float lastAttactTime = 1;
    float attactTimeCount; 

    //一般在Awake中获取变量实例
    private void Awake()
    {
        //获取到人物的NavMeshAgent组件，用来修改的的destination参数
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterState = GetComponent<CharacterState>();
    }

    private void Start()
    {
        //event事件通过 "+=" 来注册方法
        MouseController.Instance.onMouseClick += MovetoTarget;

        MouseController.Instance.onClickEnemy += EventAttact;
        
        characterState.MaxBlood = 2;

        //初始化攻击后摇时间
        attactTimeCount = characterState.attactData.coolDown;
    }


    private void Update()
    {
        SwitchAniamtion();

        //攻击后摇倒计时
        if(attactTimeCount > 0)
            attactTimeCount -= Time.deltaTime;
    }

    void SwitchAniamtion()
    {
        //直接使用agent中的速度变量velocity来赋值
        //sqrMagnitude方法是将Vector3的velocity转换成float类型
        anim.SetFloat("speed", agent.velocity.sqrMagnitude);
    }

    //将此移动方法注册到鼠标控制器的事件中，每次事件执行都会调用这个方法
    void MovetoTarget(Vector3 target)
    {
        //可以取消攻击，否则一旦选中敌人则无法停止
        StopAllCoroutines();

        //防止玩家停止移动一次后不再能够移动
        agent.isStopped = false;

        //移动位置
        agent.destination = target;
    }

    //攻击敌人事件
    private void EventAttact(GameObject obj)
    {
        
        if (obj != null)
        {
            //用变量接住获取到的实体进行操作，增加代码健壮性
            attactEnemy = obj;

            //使用协程进行玩家移动到敌人面前的操作
            //如果直接在方法中使用While循环进行距离判断并且移动，玩家会一瞬间移动到敌人面前
            StartCoroutine(moveToTarget(attactEnemy));
        }
    }

    IEnumerator moveToTarget(GameObject attactTarget)
    {
        //防止玩家停止移动一次后不再能够移动
        agent.isStopped = false;

        //实时变更玩家朝向敌人
        transform.LookAt(attactTarget.transform);

        //移动到玩家攻击距离的位置
        //characterState.attactData.attactRange获取到玩家的攻击距离属性
        while (Vector3.Distance(transform.position, attactTarget.transform.position) > characterState.attactData.attactRange)
        {
            agent.destination = attactTarget.transform.position;
            yield return null;    //此处不需要等待时间，没有移动到目的地之前，就一直移动
        }

        //执行完循环说明移动到目的地，直接停止移动
        agent.isStopped = true;
        //攻击后摇不在CD中，播放攻击动画
        if (attactTimeCount <= 0)
        {
            characterState.isCritical = Random.value < characterState.attactData.criticalChance;
            anim.SetBool("critical", characterState.isCritical);
            anim.SetTrigger("attact");
            //重置攻击后摇时间
            attactTimeCount = characterState.attactData.coolDown;
        }
    }
}
