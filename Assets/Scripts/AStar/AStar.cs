using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class AStar
{
    private List<Point> m_OpenList = new List<Point>();
    private List<Point> m_CloseList = new List<Point>();
    private Map m_Map;

    /// <summary>
    /// A*算法主体
    /// </summary>
    /// <param name="start"></param> 起点
    /// <param name="end"></param>  终点
    /// <returns></returns> 从起点到终点的路径
    public List<Point> AStarAlgorithm(Point start, Point end)
    {
        if (start == null || end == null) return null;
        m_OpenList.Add(start);   // 将起点加入集合Open
        while (m_OpenList.Count > 0) {
            // 选取优先级最高的点
            Point point = FindHighestPriority();

            if (point == end) {
                return TracePath(point);
            }
            else {
                m_OpenList.Remove(point);
                m_CloseList.Add(point);
                TraverseNearPoint(point, start, end);
            }
        }

        return null;
    }

    /// <summary>
    /// 查找集合Open中优先级最高的点
    /// </summary>
    /// <returns></returns> 优先级最高的点
    private Point FindHighestPriority()
    {
        // 排序集合Open
        m_OpenList.Sort((a, b) => {
            return a.FCost == b.FCost ? (int)(a.HCost - a.HCost) : (int)(a.FCost - b.FCost);
        });
        // 返回优先级最高的点，即FCost最小
        return m_OpenList.First();
    }

    /// <summary>
    /// 遍历四周的点
    /// </summary>
    private void TraverseNearPoint(Point point, Point start, Point end)
    {
        for (int i = (int)point.Pos.x - 1; i <= (int)point.Pos.x + 1; i += 1) {
            for (int j = (int)point.Pos.y - 1; j <= (int)point.Pos.y + 1; j += 1) {
                // 超出地图界限
                if (i < 0 || i >= m_Map.Width || j < 0 || j >= m_Map.Height) continue;

                // 该点为障碍或是再集合Close中
                Point currentPoint = m_Map.Points[i, j];
                
                if (currentPoint.IsBarrier || m_CloseList.Find(data => { return data == currentPoint; }) != null) {
                    continue;
                }

                float GCost = point.GCost + Vector2.Distance(currentPoint.Pos, point.Pos);
                // 该点不再集合Open中
                if (m_OpenList.Find(data => { return data == currentPoint; }) == null) {
                    currentPoint.Parent = point;
                    CalculatePriority(currentPoint, end);
                    m_OpenList.Add(currentPoint);
                }
                else if(GCost < currentPoint.GCost){    // 路径更优
                    currentPoint.Parent = point;
                    CalculatePriority(currentPoint, end);
                }
                TestAStar.SetColor(currentPoint, Color.cyan);
            }
        }
    }

    /// <summary>
    /// 计算权重
    /// </summary>
    /// <param name="currentPoint"></param> 计算的点
    /// <param name="start"></param>    起点
    /// <param name="end"></param>  终点
    private void CalculatePriority(Point currentPoint, Point end)
    {
        // 点到起点的距离
        currentPoint.GCost = currentPoint.Parent.GCost + Vector2.Distance(currentPoint.Pos, currentPoint.Parent.Pos);
        // 点到终点的距离
        Heuristic(currentPoint, end);
        // 综合优先级
        currentPoint.FCost = currentPoint.GCost + currentPoint.HCost;
        currentPoint.UpdateText();
    }

    private void Heuristic(Point point, Point end)
    {
        // 欧几里得距离
        point.HCost = Vector2.Distance(point.Pos, end.Pos);
        // 曼哈顿距离
        //point.HCost = Mathf.Abs(point.Pos.x - end.Pos.x) +  Mathf.Abs(point.Pos.y - end.Pos.y);
    }

    /// <summary>
    /// 从终点回溯到起点
    /// </summary>
    /// <returns></returns> 返回起点到终点的路径
    private List<Point> TracePath(Point end)
    {
        if (end == null) return null;

        Point point = end;
        List<Point> path = new();
        for (path.Add(point); point.Parent != null; point = point.Parent, path.Add(point)) ;
        path.Reverse();

        return path.Count == 0 ? null : path;
    }

    public void InitMap(Map map)
    {
        m_Map = map;
    }
}
