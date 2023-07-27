using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

//创建带参数的Event事件的写法
//创建一个类，继承UnityEvent<一个或多个事件参数的类型>
//将其序列化，显示在Unity编辑器中，用给Agent的destination参数赋值，将鼠标点击的地方Vector3类型坐标变量传出
//[Serializable]
//public class EventVector3 : UnityEvent<Vector3> { }

public class MouseController : MonoBehaviour
{
    //为鼠标控制类创建单例模式
    public static MouseController Instance;

    //使用Event事件，创建鼠标点击
    //public EventVector3 onMouseClick;
    public event Action<Vector3> onMouseClick;
    //鼠标点击敌人事件
    public event Action<GameObject> onClickEnemy;

    //用于保存鼠标点击地点的信息
    RaycastHit hitInfo;

    //鼠标图标变量
    public Texture2D point, enterdoor, attack, target, arrow;

    private void Awake()
    {
        //单例模式，每次程序唤醒前都进行判断
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

    //修改指针图标
    void SetMouseTexture()
    {
        //Camera.main.ScreenPointToRay() 方法用来获取从摄像机发射一条射线，到我们点击的位置
        //参数 鼠标当前位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //判断如果射线碰撞到物体，将碰到的物体信息传出给hitInfo
        if (Physics.Raycast(ray,out hitInfo))
        {
            //更换指针图标
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    //引擎自带更换鼠标图片的方法
                    //参数1：更换的图片
                    //参数2：鼠标作用锚点偏移量，点击时一般是鼠标指针处为作用域，但是如果是瞄准镜图标，作用域应该为中心，因此需要设置偏移量
                    //例如：直径为32的瞄准镜图片，作用点为中心点，因此偏移量为(16, 16)
                    //参数3：鼠标模式，一般不修改，设置为默认
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

    //鼠标控制器
    void MouseControl()
    {
        //0：左键  1：右键  2：中键
        //按下鼠标左键，并且射线获取到碰撞体时，进行判断
        if (Input.GetMouseButtonDown(0) && hitInfo.collider)
        {
            //.CompareTag("") 方法用于判断Tag是否等于某个字符串
            //此处判断为：鼠标当前放置目标的Tag为Ground，而不是点击事件
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                //执行点击事件，使用Invoke()调用事件
                //传入参数为Vector3
                //此处多元表达式意思为：是否触发鼠标点击事件，触发了就调用事件，将射线碰到的点的坐标，传送给Agent的destination变量
                onMouseClick?.Invoke(hitInfo.point);

            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                //传入参数为敌人的游戏对象
                onClickEnemy?.Invoke(hitInfo.collider.gameObject);
            }
        }
    }

}
