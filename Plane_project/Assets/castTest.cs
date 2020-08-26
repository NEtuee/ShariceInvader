using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class castTest : MonoBehaviour
{
    
    void OnDrawGizmos()
    {
        Vector3 a = new Vector3(-1.0f, -1.0f, 0.0f);
        Vector3 b = new Vector3(1.0f, -1.5f, 0.0f);
        Vector3 c = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f);

        Gizmos.color = Color.gray * 0.3f;
        Gizmos.DrawLine(c, a);
        Gizmos.DrawLine(c, b);

        Gizmos.color = Color.green * 0.5f;
        Gizmos.DrawLine(a, b);

        var dir = b - a;
		var save = dir.x;
		dir.x = dir.y;
		dir.y = -save;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(c, c + dir);

        Vector2 perpend = Vector2.zero;
		if(MathEx.Line_Line(a,b,c,c + dir,ref perpend))
		{
            Gizmos.DrawSphere(perpend,0.1f);
		}

        
    }
}
