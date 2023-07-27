using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new AttactData", menuName = "Character States/AttactData")]
public class AttactData_SO : ScriptableObject
{
    //������Χ
    public float attactRange;
    //Զ�̹�����Χ
    public float skillRange;
    //����CD
    public float coolDown;
    //����˺�
    public int minDamage;
    //����˺�
    public int maxDamage;
    //������
    public float criticalChance;
    //����֮����˺��ӳɰٷֱ�
    public float criticalMultipler;
}
