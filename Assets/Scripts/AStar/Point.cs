using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Map
{
    public long Width;
    public long Height;
    public Point[,] Points;
}

public class Point : MonoBehaviour
{
    public Vector2 Pos = Vector2.zero;
    public bool IsBarrier = false; 
    public Point Parent;
    public float GCost;
    public float HCost;
    public float FCost;

    private List<TMP_Text> m_CostText = new();

    private void Start()
    {
        GetComponentsInChildren<TMP_Text>(m_CostText);
        ResetPointState();
    }

    private void Update()
    {
        UpdateText();
    }

    public void ResetPointState()
    {
        GCost = 0;
        HCost = 0;
        FCost = 0;
        Parent = null;
        UpdateText();
    }

    public void UpdateText()
    {
        m_CostText.Find(data => { return data.name == "GCost"; }).text = "GCost" + GCost.ToString();
        m_CostText.Find(data => { return data.name == "HCost"; }).text = "HCost" + HCost.ToString();
        m_CostText.Find(data => { return data.name == "FCost"; }).text = "FCost" + FCost.ToString();
    }

    #region Override Operator

    public static bool operator <(Point p1, Point p2)
    {
        return p1.FCost == p2.FCost ? p1.HCost < p2.HCost : p1.FCost < p2.FCost;
    }
    public static bool operator >(Point p1, Point p2)
    {
        return p1.FCost == p2.FCost ? p1.HCost > p2.HCost : p1.FCost > p2.FCost;
    }
    #endregion
}
