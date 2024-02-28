using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    /// <summary>
    /// A星寻路管理器
    /// </summary>
    public class AstartManager
    {
        private static AstartManager instance;
    
        public static AstartManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AstartManager();
                return instance;
            }
        }

        private int mapW;
        private int mapH;
        
        //地图相关所有的格子对象容器
        public AstartNode[,] nodes;

        //开启列表
        private List<AstartNode> openList=new List<AstartNode>();
        //关闭列表
        private List<AstartNode> closedList=new List<AstartNode>();

        public void InitMapInfo(int w,int h,float dif)
        {
            this.mapW = w;
            this.mapH = h;

            nodes = new AstartNode[w, h];
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    AstartNode node=new AstartNode(i,j,Random.Range(0,100)<dif?E_Node_Type.Stop:E_Node_Type.Walk);
                    nodes[i, j] = node;
                }
            }
        }

        public List<AstartNode> FindPath(Vector2 startPos,Vector2 endPos)
        {
            //实际项目中传入的点往往是坐标系中的位置，坐标可能会存在小数的情况，需要额外进行“归属”格子的判断
            //这里举例只考虑为整数的情况
            //1.判断是否在范围内
            if (startPos.x<0||startPos.x>=mapW||
                startPos.y<0||startPos.y>=mapH||
                endPos.x<0||endPos.x>=mapW||
                endPos.y<0||endPos.y>=mapH)
            {
                Debug.Log("开始或者结束点在地图格子范围外");
                return null;
            }
            //关闭列表
            AstartNode start = nodes[(int)startPos.x, (int)startPos.y];
            //开启列表
            AstartNode end = nodes[(int)endPos.x, (int)endPos.y];
            
            //2.判断是否合法
            if (start.type == E_Node_Type.Stop || end.type == E_Node_Type.Stop)
            {
                Debug.Log("开始或结束点为障碍");
                return null;
            }

            //清空每一次的数据
            closedList.Clear();
            openList.Clear();
            
            //将开始点放入关闭列表中
            start.father = null;//father为空作为结束条件
            start.f = 0;
            start.g = 0;
            start.h = 0;
            closedList.Add(start);
            while (true)
            {
                
                //左上
                FindNearlyNodeToOpenList(start.x - 1, start.y - 1, 1.4f, start, end);
                //正上
                FindNearlyNodeToOpenList(start.x, start.y - 1, 1f, start, end);
                //右上
                FindNearlyNodeToOpenList(start.x + 1, start.y - 1, 1.4f, start, end);
                //正右
                FindNearlyNodeToOpenList(start.x + 1, start.y, 1f, start, end);
                //右下
                FindNearlyNodeToOpenList(start.x + 1, start.y + 1, 1.4f, start, end);
                //正下
                FindNearlyNodeToOpenList(start.x, start.y + 1, 1f, start, end);
                //左下
                FindNearlyNodeToOpenList(start.x - 1, start.y + 1, 1.4f, start, end);
                //正左
                FindNearlyNodeToOpenList(start.x - 1, start.y, 1f, start, end);


                //死路判断
                if (openList.Count == 0)
                {
                    Debug.Log("开始或者结束是死路");
                    return null;
                }


                //排序
                openList.Sort(SortOpenList);

                //放入关闭列表
                closedList.Add(openList[0]);
                //将选择的点有作为新的开始点，继续搜寻下一个点
                start = openList[0];
                //从开启列表中移除
                openList.RemoveAt(0);


                if (start == end)
                {
                    //回溯路径
                    List<AstartNode> path = new List<AstartNode>();
                    path.Add(end);
                    while (end.father != null)
                    {
                        path.Add(end.father);
                        end = end.father;
                    }
                    //列表调换顺序API
                    path.Reverse();
                    return path;
                }
            }
        }

        /// <summary>
        /// 排序函数主体
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int SortOpenList(AstartNode a,AstartNode b)
        {
            if (a.f>b.f)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        private void FindNearlyNodeToOpenList(int x,int y,float g,AstartNode father,AstartNode end)
        {
            //边界判断
            if (x < 0||x>=mapW||y<0||y>=mapH) 
            {
                return;
            }

            AstartNode node = nodes[x, y];

            if (node == null || node.type == E_Node_Type.Stop || 
                closedList.Contains(node) || openList.Contains(node))
            {
                return;
            }
            
            //计算路程消耗值f；f=g+h;
            
            node.father = father;
            
            node.g = father.g+g;
            //曼哈顿街区算法
            node.h=Mathf.Abs(end.x-node.x)+Mathf.Abs(end.y-node.y);
            
            node.f = node.g + node.h;
            
            openList.Add(node);
        }
    }
}
