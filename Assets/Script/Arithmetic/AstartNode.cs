using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    
    /// <summary>
    /// 格子类型
    /// </summary>
    public enum E_Node_Type
    {
        //可以走
        Walk,
        //不能走
        Stop,
    }

    /// <summary>
    /// 格子属性
    /// </summary>
    public class AstartNode 
    {
        //格子对象的坐标
        public int x;
        public int y;
    
        //寻路消耗
        public float f;
        //离起点距离
        public float g;
        //离终点距离
        public float h;
        //父对象
        public AstartNode father;
    
        //格子的类型
        public E_Node_Type type;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">格子的横坐标</param>
        /// <param name="y">格子的纵坐标</param>
        /// <param name="type">格子的类型</param>
        public AstartNode(int x, int y, E_Node_Type type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
}
