using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
    //为Data文件创建变量，操作其中字段
    public CharacterData_SO characterData;
    public AttactData_SO attactData;

    [HideInInspector]
    //判断是否暴击
    public bool isCritical;

    #region Read from CharacterData_SO（使用属性从Data文件中读取和操作字段）
    private int maxBlood;
    private int currentBlood;
    private int baseDefend;
    private int currentDefend;
    //为四个变量定义其属性，在属性中使用Data文件的数值
    public int MaxBlood
    {
        get
        {
            if (characterData != null)
            {
                //获取Data文件中的信息
                return characterData.maxBlood;
            }
            else
            {
                return 0;
            }
        }

        set
        {
            //给Data文件中的字段赋值
            //属性中的value，代表调用时外部传进来的值
            characterData.maxBlood = value;
        }
    }

    public int CurrentBlood
    {
        get
        {
            if (characterData != null)
            {
                //获取Data文件中的信息
                return characterData.currentBlood;
            }
            else
            {
                return 0;
            }
        }

        set
        {
            characterData.currentBlood = value;
        }
    }

    public int BaseDefend
    {
        get
        {
            if (characterData != null)
            {
                //获取Data文件中的信息
                return characterData.baseDefend;
            }
            else
            {
                return 0;
            }
        }

        set
        {
            characterData.baseDefend = value;
        }
    }

    public int CurrentDefend
    {
        get
        {
            if (characterData != null)
            {
                //获取Data文件中的信息
                return characterData.currentDefend;
            }
            else
            {
                return 0;
            }
        }

        set
        {
            characterData.currentDefend = value;
        }
    }
    #endregion

    #region Character Attact

    //计算受到攻击伤害
    //需要获取两个对象的攻击力防御力数值，来进行计算
    public void TakeDamage(CharacterState attacter, CharacterState defender)
    {
        //攻击者攻击力减去防御者防御力
        //使用Max函数控制收到伤害不能为负
        int damage = Mathf.Max( attacter.CurrentAttact() - defender.currentDefend, 0);

        //通过属性给封装字段赋值
        //防止伤害过高血量降至负值
        CurrentBlood = Mathf.Max(CurrentBlood - damage, 0);

        //TODO：血量条 UI
        //TODO: 经验值Update
    }

    int CurrentAttact()
    {
        //在对象数值中的 “最大攻击力” 和 “最小攻击力” 间取随机数
        float coreDamage = Random.Range(attactData.minDamage, attactData.maxDamage);

        if (isCritical)
        {
            //如果暴击了，将伤害乘以暴击伤害倍率
            coreDamage *= attactData.criticalMultipler;

            Debug.Log("暴击！" + coreDamage);
        }

        //四舍五入后转换成int
        return (int)(coreDamage + 0.5);
    }
    #endregion
}
