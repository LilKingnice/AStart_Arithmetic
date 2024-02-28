using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Generate : MonoBehaviour
{
    [Header("基础属性")]
    //左上角第一个
    public int beginX=-3;
    public int beginY=-3;

    public int offsetX=2;
    public int offsetY=2;

    public int mapW=5;
    public int mapH=5;
    [Range(0,60)]
    public float difficulty;
    private List<AstartNode> list;

    [Header("修改属性")]
    public Material RedMaterial;
    public Material YellowMaterial;
    public Material GreenMaterial;
    public Material DefualMaterial;

    
    private Vector2 beginPos = Vector2.right * -1;
    
    
    private Dictionary<string,GameObject> cubes=new Dictionary<string,GameObject>();
    private void Awake()
    {
        AstartManager.Instance.InitMapInfo(mapW, mapH,difficulty);
    }

    void Start()
    {
        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

                obj.transform.position =new Vector3(beginX + i * offsetX, beginY + j * offsetY, 0);
                obj.name = i + "_" + j;
                
                //存储cube到容器中
                cubes.Add(obj.name, obj);
                
                //得到所有格子 判断是否可走
                AstartNode node = AstartManager.Instance.nodes[i, j];
                if (node.type==E_Node_Type.Stop)
                {
                    obj.GetComponent<MeshRenderer>().material = RedMaterial;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //发生碰撞的信息
            RaycastHit info;
            //获取从屏幕发出的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //实现点击选取起点
            if (Physics.Raycast(ray,out info,1000))
            {
                if (beginPos==Vector2.right*-1)
                {
                    //清理上次缓存
                    ClearLine2();
                    
                    string[] strs = info.collider.gameObject.name.Split('_');

                    beginPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));

                    info.collider.gameObject.GetComponent<MeshRenderer>().material = YellowMaterial;
                }
                else
                {
                    string[] strs = info.collider.gameObject.name.Split('_');
                    Vector2 endPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));
                    
                    
                    //自动寻路
                    list = AstartManager.Instance.FindPath(beginPos, endPos);
                    
                    //cubes[(int)beginPos.x + "_" + (int)beginPos.y].GetComponent<MeshRenderer>().material = DefualMaterial;
                    
                    if (list != null) 
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            cubes[list[i].x+"_"+list[i].y].GetComponent<MeshRenderer>().material = GreenMaterial;
                        }
                    }
                    ClearLine1();
                }
                
            }
        }
    }

    private void ClearLine1()
    {
        beginPos = Vector2.right * -1;
    }

    void ClearLine2()
    {
        if (list != null) 
        {
            for (int i = 0; i < list.Count; i++)
            {
                cubes[list[i].x+"_"+list[i].y].GetComponent<MeshRenderer>().material = DefualMaterial;
            }
        }
    }
}
