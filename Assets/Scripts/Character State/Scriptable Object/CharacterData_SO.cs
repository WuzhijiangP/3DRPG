using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data", menuName ="Character States/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("State Info")]
    //����Ѫ��
    public int maxBlood;
    //��ǰѪ��
    public int currentBlood;
    //����������
    public int baseDefend;
    //��ǰ������
    public int currentDefend;

}
