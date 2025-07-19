using System;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    public enum MapMode { SETBARRIER, SETSTART, SETEND }

    public MapMode Mode = MapMode.SETBARRIER;
    public long MapWidth, MapHeight;
    public Point MapPrefab;

    private Map m_Map;
    private List<Point> m_MapTiles;
    private Point m_StarPoint;
    private Point m_EndPoint;
    private List<Point> m_PrePath;
    private bool m_IsChange = false;


    // Start is called before the first frame update
    void Start()
    {
        InitMap(MapWidth, MapHeight);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Star"  + m_StarPoint?.Pos);
        Debug.Log("End" + m_EndPoint?.Pos);
        
        UpdateMapFactor();
        if(m_IsChange){
            UpdateMap();
            m_IsChange = false;
        }
    }

    private void UpdateMap()
    {
        // 清除所有点的Cost
        ResetPoints();

        // 生成新路径
        AStar aStar = new AStar();
        aStar.InitMap(m_Map);
        List<Point> path = aStar.AStarAlgorithm(m_StarPoint, m_EndPoint);
        Debug.Log(path);
        if (path == null) return;

        ClearPath();
        m_PrePath = path;

        for (int i = 1; i < path.Count - 1; ++i) {
            SetColor(path[i], Color.green);
        }
        SetColor(m_StarPoint, Color.blue);
        SetColor(m_EndPoint, Color.red);
    }

    private void ResetPoints()
    {
        foreach(Point point in m_MapTiles){
            point.ResetPointState();
            if (!point.IsBarrier)
                SetColor(point, Color.white);
        }
    }

    /// <summary>
    /// 产生新路径后清空地图瓦片的颜色和地图上点的数据
    /// </summary>
    private void ClearPath()
    {
        if(m_PrePath != null){
            Debug.Log("Clear Path");
            foreach(Point point in m_PrePath){
                point.ResetPointState();
                SetColor(point, Color.white);
            }
        }
    }

    private void UpdateMapFactor()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(ray.origin, ray.direction);
        if (hitInfo.collider == null || hitInfo.collider.tag != "Map") return;

        Point tile = hitInfo.collider.gameObject.GetComponent<Point>();
        Vector3 pos = tile.transform.position;
        bool isBarrier = m_Map.Points[(int)pos.x, (int)pos.y].IsBarrier;
        switch(Mode){
            case MapMode.SETBARRIER:{
                if(isBarrier){
                    SetColor(tile, Color.white);
                    m_Map.Points[(int)pos.x,(int)pos.y].IsBarrier = false;
                }
                else{
                    SetColor(tile, Color.black);
                    m_Map.Points[(int)pos.x, (int)pos.y].IsBarrier = true;
                }
                break;
            }
            case MapMode.SETSTART:{
                if (isBarrier) break;
                m_IsChange = true;
                SetColor(m_StarPoint, Color.white);
                m_StarPoint = tile;
                SetColor(m_StarPoint, Color.blue);
                break;
            }
            case MapMode.SETEND:{
                if(isBarrier) break;
                m_IsChange = true;
                SetColor(m_EndPoint, Color.white);
                m_EndPoint = tile;
                SetColor(m_EndPoint, Color.red);
                break;
            }
        }
    }

    private void InitMap(long width, long height)
    {
        m_Map = new Map();
        m_MapTiles = new List<Point>();
        m_StarPoint = null;
        m_EndPoint = null;

        m_Map.Width = width;
        m_Map.Height = height;
        m_Map.Points = new Point[m_Map.Width, m_Map.Height];
        for (long i = 0; i < width; ++i) {
            for (long j = 0; j < height; ++j) {
                Point tile = Instantiate(MapPrefab);
                tile.transform.position = new Vector3(i, j, 0);
                tile.transform.SetParent(transform);
                tile.Pos = new Vector3(i, j, 0);
                tile.tag = "Map";
                m_Map.Points[i, j] = tile;
                m_MapTiles.Add(tile);
            }
        }
    }

    static public void SetColor(Point point, Color color)
    {
        if (point == null) return;
        point.gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    static public bool CmpV3AndV2(Vector3 vector3, Vector2 vector2)
    {
        return vector3.x == vector2.x && vector3.y == vector2.y;
    }

}
