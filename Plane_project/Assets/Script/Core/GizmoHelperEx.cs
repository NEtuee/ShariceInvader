using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoHelperEx : SingletonMono<GizmoHelperEx>
{
    public class GizmoItem
    {
        public List<Vector2> points = new List<Vector2>();
        public Color color;
    }

    List<GizmoItem> drawList = new List<GizmoItem>();

    Queue<GizmoItem> queue = new Queue<GizmoItem>();

    public void Awake()
    {
        instance = this;
    }

    GizmoItem GetItem() 
    {
        if(queue.Count == 0)
            return new GizmoItem();
        else
            return queue.Dequeue();
    }

    public void DrawLine(Vector2 one, Vector2 two, Color color)
    {
        var item = GetItem();
        item.points.Clear();

        item.points.Add(one);
        item.points.Add(two);
        item.color = color;

        drawList.Add(item);
    }

    // public void DrawBounds(BoundBoxEx box, Vector2 pos, Color color)
	// {
    //     box.UpdateBound(pos);
    //     var lt = new Vector2(box.left,box.top);
    //     var rt = new Vector2(box.right,box.top);
    //     var rb = new Vector2(box.right,box.bottom);
    //     var lb = new Vector2(box.left,box.bottom);

    //     var item = GetItem();
    //     item.points.Clear();
        
    //     item.points.Add(lt);
    //     item.points.Add(rt);
    //     item.points.Add(rb);
    //     item.points.Add(lb);
    //     item.points.Add(lt);
    //     item.color = color;

    //     drawList.Add(item);
	// }

    public void OnDrawGizmos()
    {
        foreach(var item in drawList)
        {
            Gizmos.color = item.color;
            for(int i = 0; i < item.points.Count - 1; ++i)
            {
                Gizmos.DrawLine(item.points[i],item.points[i + 1]);
            }
            
            item.points.Clear();
            queue.Enqueue(item);
        }

        drawList.Clear();
    }
}
