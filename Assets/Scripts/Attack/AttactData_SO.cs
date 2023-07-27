using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new AttactData", menuName = "Character States/AttactData")]
public class AttactData_SO : ScriptableObject
{
    //攻击范围
    public float attactRange;
    //远程攻击范围
    public float skillRange;
    //技能CD
    public float coolDown;
    //最低伤害
    public int minDamage;
    //最高伤害
    public int maxDamage;
    //暴击率
    public float criticalChance;
    //暴击之后的伤害加成百分比
    public float criticalMultipler;
}
