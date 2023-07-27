using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//使用枚举，记录每种状态的Enemy
public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }

//使用RequireComponent标记脚本类
//就可以让每个挂接此脚本的对象如果没有添加NavMeshAgent，都会自动添加上NavMeshAgent组件
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;

    Animator anim;

    CharacterState characterState;

    //创建枚举变量
    private EnemyStates enemyStates;

    [Header("Bisic Setting")]
    //保存初始移动速度（追击时会加速）
    float speed;
    //用于在Unity编辑器中选择敌人的类型，是站桩类型还是巡逻类型
    public bool isGuard;
    //敌人的攻击目标
    GameObject attactTarget;
    //定义敌人的可视范围
    public float sightRadius;
    //攻击间隔CD
    float lastAttactTime;

    [Header("Patrol State")]
    //巡逻范围
    public float patrolRange;
    //巡逻范围内生成随机点
    Vector3 patrolPoint;
    //保存初始位置
    Vector3 savePosition;
    //巡逻时的停顿时间
    public float enemyWaitingTime;
    //停顿时间倒计时
    float waitingCount;

    //控制敌人动画的状态
    bool isIdle, isChase, isFollow;

    private void Awake()
    {
        //在Awake中获取组件
        agent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();

        characterState = GetComponent<CharacterState>();
    }

    private void Start()
    {
        savePosition = transform.position;

        speed = agent.speed;

        waitingCount = enemyWaitingTime;

        //初始化状态
        if (isGuard)
        {
            //站桩状态
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            //巡逻状态
            enemyStates = EnemyStates.PATROL;
            //初始获得一个巡逻点，开始移动
            GetRandomPoint();
        }
    }

    private void Update()
    {
        //实时修改敌人状态
        SwitchStates();

        SwitchAnimation();

        //攻击CD间隔计时器
        if (lastAttactTime > 0)
        {
            lastAttactTime -= Time.deltaTime;
        }
    }

    //动画转换器
    void SwitchAnimation()
    {
        //根据不同参数，实时进行动画转换
        anim.SetBool("walk", isIdle);
        anim.SetBool("chase", isChase);
        anim.SetBool("follow", isFollow);
        anim.SetBool("critical", characterState.isCritical);
    }


    //敌人状态转换机
    void SwitchStates()
    {
        
        if (FindedPlayer())
        {
            //发现玩家，将状态修改成追击模式；
            enemyStates = EnemyStates.CHASE;
            Debug.Log("发现玩家，开始追击");
        }

        //模拟有限状态机，当达到某个条件时，状态进行转换
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                
                break;
            case EnemyStates.PATROL:
                //巡逻时，移动速度变为原来速度的一半
                //使用乘法消耗比除法低
                agent.speed = speed * 0.5f;

                if (Vector3.Distance(patrolPoint, transform.position) > agent.stoppingDistance)
                {
                    //移动到新的点
                    isIdle = true;
                    agent.destination = patrolPoint;
                }
                else
                {
                    //走到了目的地，停止移动，再寻找新的点
                    isIdle = false;

                    if (waitingCount > 0)
                    {
                        //开始倒计时等待
                        waitingCount -= Time.deltaTime;
                    }
                    else
                    {
                        GetRandomPoint();
                        //重置时间
                        waitingCount = enemyWaitingTime;
                    }
                }

                break;
            case EnemyStates.CHASE:
                //追击时，速度变为原来的速度
                agent.speed = speed;

                //TODO:调整动画播放状态
                isIdle = false;
                isChase = true;
                //TODO:如果拉脱战了，回到原本的状态
                if (!FindedPlayer())
                {
                    isFollow = false;
                    isChase = false;
                    //脱战后，由于惯性，Enemy会往前滑一小段距离，因此此处直接赋值坐标，让其直接在原地停下来
                    agent.destination = transform.position;
                    if (waitingCount > 0)
                    {
                        //开始倒计时等待
                        waitingCount -= Time.deltaTime;
                    }
                    else if (isGuard)
                    {
                        //回到站桩状态
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        //回到巡逻状态
                        enemyStates = EnemyStates.PATROL;
                        //获取新的点位
                        GetRandomPoint();
                        //重置时间
                        waitingCount = enemyWaitingTime;
                    }

                }
                else
                {
                    //TODO:追击Player
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attactTarget.transform.position;
                }

                //TODO:如果在攻击范围内，则攻击
                if (TargetInAttactRange() || TargetInSkillRange())
                {
                    //在攻击范围内，将停止跟随，进行攻击
                    isFollow = false;
                    agent.isStopped = true;

                    //攻击后摇结束，可以进行下一次攻击
                    if (lastAttactTime <= 0)
                    {
                        //攻击后摇
                        lastAttactTime = characterState.attactData.coolDown;

                        //判断这次攻击是否暴击
                        //Random.value会返回一个介于0~1之间的小数，很实用用于判断百分比数值比较
                        //获取到的数值小于暴击率，则暴击
                        characterState.isCritical = Random.value < characterState.attactData.criticalChance;

                        //执行攻击
                        Attact();

                    }
                }

                break;
            case EnemyStates.DEAD:
                
                break;
        }
    }

    //执行攻击方法
    void Attact()
    {
        //面向攻击目标
        transform.LookAt(attactTarget.transform);

        if (TargetInAttactRange())
        {
            //播放近战攻击动画
            anim.SetTrigger("attact");
        }
        if (TargetInSkillRange())
        {
            //播放远程攻击动画
            anim.SetTrigger("skillattact");

        }
    }

    //返回判断玩家是否在近战攻击范围内
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

    //返回判断玩家是否在远程攻击范围内
    bool TargetInSkillRange()
    {
        if (attactTarget != null)
        {
            //是否大于近战攻击范围，但是在远程攻击范围内，防止近战状态和远程状态重叠
            return (Vector3.Distance(transform.position, attactTarget.transform.position) > characterState.attactData.attactRange)
                    && (Vector3.Distance(transform.position, attactTarget.transform.position) <= characterState.attactData.skillRange);
        }
        else
        {
            return false;
        }
    }

    //判断是否发现玩家
    bool FindedPlayer()
    {
        //找到周围半径内的所有碰撞到的物体
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        //循环查找所有碰撞体
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                //将玩家定为攻击目标
                attactTarget = target.gameObject;
                return true;
            }
        }

        //如果丢失玩家目标
        attactTarget = null;
        return false;
    }

    //绘制巡逻范围
    private void OnDrawGizmosSelected()
    {
        //显示巡逻范围颜色
        Gizmos.color = Color.blue;
        //使用DrawWireSphere方法绘制出来的是一个轮廓
        //使用DrawSphere方法绘制出来是一个实心的圆，导致看不到Enemy
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }

    //获取巡逻随机点
    void GetRandomPoint()
    {
        //平面只涉及到X轴和Z轴，因此获取这两个轴的随机点
        //随机值范围：巡逻范围的正，负值
        float PatrolX = Random.Range(-patrolRange, patrolRange);
        float PatrolZ = Random.Range(-patrolRange, patrolRange);

        //设置新的点位
        //Y轴不使用savePosition的y坐标，而是使用当前position的y坐标
        //因为如果走到悬崖或者台阶上，y坐标一直保持不表会导致悬空，所以需要实时更新
        Vector3 newPoint = new Vector3(savePosition.x + PatrolX, transform.position.y, savePosition.z + PatrolZ);

        //解决Enemy卡在障碍物上的问题
        //判断新点位是否在不可行走区域中
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPoint, out hit, patrolRange, 1))
        {
            patrolPoint = newPoint;
        }
        else
        {
            //否则将不动
            patrolPoint = transform.position;
        }
    }

}
