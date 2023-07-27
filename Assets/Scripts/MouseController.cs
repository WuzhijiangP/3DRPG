using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

//������������Event�¼���д��
//����һ���࣬�̳�UnityEvent<һ�������¼�����������>
//�������л�����ʾ��Unity�༭���У��ø�Agent��destination������ֵ����������ĵط�Vector3���������������
//[Serializable]
//public class EventVector3 : UnityEvent<Vector3> { }

public class MouseController : MonoBehaviour
{
    //Ϊ�������ഴ������ģʽ
    public static MouseController Instance;

    //ʹ��Event�¼������������
    //public EventVector3 onMouseClick;
    public event Action<Vector3> onMouseClick;
    //����������¼�
    public event Action<GameObject> onClickEnemy;

    //���ڱ���������ص����Ϣ
    RaycastHit hitInfo;

    //���ͼ�����
    public Texture2D point, enterdoor, attack, target, arrow;

    private void Awake()
    {
        //����ģʽ��ÿ�γ�����ǰ�������ж�
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Update()
    {
        
        SetMouseTexture();

        MouseControl();
    }

    //�޸�ָ��ͼ��
    void SetMouseTexture()
    {
        //Camera.main.ScreenPointToRay() ����������ȡ�����������һ�����ߣ������ǵ����λ��
        //���� ��굱ǰλ��
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //�ж����������ײ�����壬��������������Ϣ������hitInfo
        if (Physics.Raycast(ray,out hitInfo))
        {
            //����ָ��ͼ��
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    //�����Դ��������ͼƬ�ķ���
                    //����1��������ͼƬ
                    //����2���������ê��ƫ���������ʱһ�������ָ�봦Ϊ�����򣬵����������׼��ͼ�꣬������Ӧ��Ϊ���ģ������Ҫ����ƫ����
                    //���磺ֱ��Ϊ32����׼��ͼƬ�����õ�Ϊ���ĵ㣬���ƫ����Ϊ(16, 16)
                    //����3�����ģʽ��һ�㲻�޸ģ�����ΪĬ��
                    Cursor.SetCursor(point, new Vector2(0,0), CursorMode.Auto);
                    break;
                case "Door":

                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(4, 4), CursorMode.Auto);
                    break;
            }


        }

    }

    //��������
    void MouseControl()
    {
        //0�����  1���Ҽ�  2���м�
        //�������������������߻�ȡ����ײ��ʱ�������ж�
        if (Input.GetMouseButtonDown(0) && hitInfo.collider)
        {
            //.CompareTag("") ���������ж�Tag�Ƿ����ĳ���ַ���
            //�˴��ж�Ϊ����굱ǰ����Ŀ���TagΪGround�������ǵ���¼�
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                //ִ�е���¼���ʹ��Invoke()�����¼�
                //�������ΪVector3
                //�˴���Ԫ���ʽ��˼Ϊ���Ƿ񴥷�������¼��������˾͵����¼��������������ĵ�����꣬���͸�Agent��destination����
                onMouseClick?.Invoke(hitInfo.point);

            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                //�������Ϊ���˵���Ϸ����
                onClickEnemy?.Invoke(hitInfo.collider.gameObject);
            }
        }
    }

}
