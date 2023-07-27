using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
    //ΪData�ļ��������������������ֶ�
    public CharacterData_SO characterData;
    public AttactData_SO attactData;

    [HideInInspector]
    //�ж��Ƿ񱩻�
    public bool isCritical;

    #region Read from CharacterData_SO��ʹ�����Դ�Data�ļ��ж�ȡ�Ͳ����ֶΣ�
    private int maxBlood;
    private int currentBlood;
    private int baseDefend;
    private int currentDefend;
    //Ϊ�ĸ��������������ԣ���������ʹ��Data�ļ�����ֵ
    public int MaxBlood
    {
        get
        {
            if (characterData != null)
            {
                //��ȡData�ļ��е���Ϣ
                return characterData.maxBlood;
            }
            else
            {
                return 0;
            }
        }

        set
        {
            //��Data�ļ��е��ֶθ�ֵ
            //�����е�value���������ʱ�ⲿ��������ֵ
            characterData.maxBlood = value;
        }
    }

    public int CurrentBlood
    {
        get
        {
            if (characterData != null)
            {
                //��ȡData�ļ��е���Ϣ
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
                //��ȡData�ļ��е���Ϣ
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
                //��ȡData�ļ��е���Ϣ
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

    //�����ܵ������˺�
    //��Ҫ��ȡ��������Ĺ�������������ֵ�������м���
    public void TakeDamage(CharacterState attacter, CharacterState defender)
    {
        //�����߹�������ȥ�����߷�����
        //ʹ��Max���������յ��˺�����Ϊ��
        int damage = Mathf.Max( attacter.CurrentAttact() - defender.currentDefend, 0);

        //ͨ�����Ը���װ�ֶθ�ֵ
        //��ֹ�˺�����Ѫ��������ֵ
        CurrentBlood = Mathf.Max(CurrentBlood - damage, 0);

        //TODO��Ѫ���� UI
        //TODO: ����ֵUpdate
    }

    int CurrentAttact()
    {
        //�ڶ�����ֵ�е� ����󹥻����� �� ����С�������� ��ȡ�����
        float coreDamage = Random.Range(attactData.minDamage, attactData.maxDamage);

        if (isCritical)
        {
            //��������ˣ����˺����Ա����˺�����
            coreDamage *= attactData.criticalMultipler;

            Debug.Log("������" + coreDamage);
        }

        //���������ת����int
        return (int)(coreDamage + 0.5);
    }
    #endregion
}
