using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathEx : MonoBehaviour {

	public static void BresenhamLine(Vector2Int start, Vector2Int end, ref List<Vector2Int> result) 
	{
    	bool steep = MathEx.abs(end.y - start.y) > MathEx.abs(end.x - start.x);
    	if (steep)
		{
			int save = start.x;
			start.x = start.y;
			start.y = save;

			save = end.x;
			end.x = end.y;
			end.y = save;
    	}
    	if (start.x > end.x)
		{
			int save = start.x;
			start.x = end.x;
			end.x = save;

			save = start.y;
			start.y = end.y;
			end.y = save;
    	}

    	int deltax = end.x - start.x;
    	int deltay = MathEx.abs(end.y - start.y);
    	int error = 0;
    	int ystep;
    	int y = start.y;
    	if (start.y < end.y)
			ystep = 1;
		else
			ystep = -1;
    	for (int x = start.x; x <= end.x; x++) 
		{
    	    if (steep)
				result.Add(new Vector2Int(y, x));
    	    else
				result.Add(new Vector2Int(x, y));
    	    error += deltay;
    	    if (2 * error >= deltax)
			{
    	        y += ystep;
    	        error -= deltax;
    	    }
    	}
	}

	public static float DistanceFromPointToLine(Vector2 point, Vector2 line0, Vector2 line1)
    {
        Vector2 l1 = line0;
        Vector2 l2 = line1;

    	return abs((l2.x - l1.x)*(l1.y - point.y) - (l1.x - point.x)*(l2.y - l1.y))/
                Mathf.Sqrt(Mathf.Pow(l2.x - l1.x, 2) + Mathf.Pow(l2.y - l1.y, 2));
    }

	public static void Swap<T>(ref T one, ref T two)
	{
		T save = one;
		one = two;
		two = save;
	}
	public static int abs(int value) {return value < 0 ? - value : value;}
	public static float abs(float value) {return value < 0 ? -value : value;}
	public static float normalize(float value) {return value < 0 ? -1 : (value == 0 ? 0 : 1);}
	public static float limitMinus(float value, float factor) {return value - factor < 0 ? 0 : value - factor;}
	public static float nearZero(float value) {return abs(value) < 0.0001f ? 0 : value;}
	public static float distance(float x1, float x2) {return abs(x1 - x2);}
	public static float vectorScale(Vector3 v) {return (abs(v.x) + abs(v.y));}
	//public static float directionToAngle(Vector2 dir) {return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;}
	public static float directionToAngle(Vector2 dir) 
	{
		float val = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		return clamp360Degree(val);
	}
	public static Vector3 angleToDirection(float angle) {return new Vector3(Mathf.Cos(angle),Mathf.Sin(angle));}
	public static float clamp360Degree(float eulerAngle)
    {
        //  float val = eulerAngle - Mathf.CeilToInt(eulerAngle / 360f) * 360f;
		//  val = val < 0 ? val + 360f : val;
		float val = eulerAngle + ((float)((int)-eulerAngle / 360) * 360f);
		val = val < 0 ? val + 360f : val;
		return val;
    }
	public static float FlipLeftAngle(float angle) //clamp angle only
	{
		float a = 180f - angle;
		a = 180 + a;
		return a;
	}
	public static void nearZero(ref Vector3 value) {value.x = nearZero(value.x); value.y = nearZero(value.y);}
	public static bool halfCompare(float one,float two) {return abs((one - two)) <= 0.0001f;}
	public static int Vector3Compare(Vector3 one, Vector3 two)
	{
		float x = abs(one.x) + abs(one.y);
		float y = abs(two.x) + abs(two.y);

		return x == y ? 0 : (x > y ? 1 : 2);
	}
	public static bool Vector3ValueEqual(Vector3 one, Vector3 two)
	{
		return one.x == two.x ? (one.y == two.y ? (one.z == two.z) : false) : false;
	}
}
