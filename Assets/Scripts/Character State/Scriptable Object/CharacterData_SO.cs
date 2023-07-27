using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data", menuName ="Character States/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("State Info")]
    //基础血量
    public int maxBlood;
    //当前血量
    public int currentBlood;
    //基础防御力
    public int baseDefend;
    //当前防御力
    public int currentDefend;

}
