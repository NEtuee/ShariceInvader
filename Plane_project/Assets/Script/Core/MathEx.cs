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

	public static bool Line_Line( Vector2 p1,Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection )
	{
		float x = p1.x > p2.x ? p2.x : p1.x;
		if(x > p3.x && x > p4.x)
			return false;

	    float Ax,Bx,Cx,Ay,By,Cy,d,e,f,num/*,offset*/;
	    float x1lo,x1hi,y1lo,y1hi;

	    Ax = p2.x-p1.x;
	    Bx = p3.x-p4.x;

	    if(Ax<0) 
		{
	        x1lo=p2.x; x1hi=p1.x;
	    }
		else 
		{
	        x1hi=p2.x; x1lo=p1.x;
	    }

	    if(Bx>0) 
		{
	        if(x1hi < p4.x || p3.x < x1lo) return false;
	    } 
		else 
		{
	        if(x1hi < p3.x || p4.x < x1lo) return false;
	    }
	
	    Ay = p2.y-p1.y;
	    By = p3.y-p4.y;

	    // Y bound box test//

	    if(Ay<0) 
		{
	        y1lo=p2.y; y1hi=p1.y;
	    } 
		else 
		{
	        y1hi=p2.y; y1lo=p1.y;
	    }

	    if(By>0) 
		{
	        if(y1hi < p4.y || p3.y < y1lo) return false;
	    } 
		else 
		{
	        if(y1hi < p3.y || p4.y < y1lo) return false;
	    }

	    Cx = p1.x-p3.x;
	    Cy = p1.y-p3.y;
	
	    d = By*Cx - Bx*Cy;
	    f = Ay*Bx - Ax*By;

	    if(f>0) 
		{
	        if(d<0 || d>f) return false;
	    } 
		else 
		{
	        if(d>0 || d<f) return false;
	    }

	    e = Ax*Cy - Ay*Cx;

	    if(f>0) 
		{                          
	    	if(e<0 || e>f) return false;
	    } 
		else 
		{
	    	if(e>0 || e<f) return false;
	    }

	    if(f==0) return false;

	    num = d*Ax;
	  	intersection.x = p1.x + num / f;

	    num = d*Ay;
		intersection.y = p1.y + num / f;

	    return true;
	}

	public static Vector3 PerpendicularPoint(Vector3 start, Vector3 end, Vector3 point)
	{
		return Vector3.Project(point - start, end - start) + start;
	}

	public static float DistanceFromPointToLine(Vector2 point, Vector2 line0, Vector2 line1)
    {
        Vector2 l1 = line0;
        Vector2 l2 = line1;

    	return abs((l2.x - l1.x)*(l1.y - point.y) - (l1.x - point.x)*(l2.y - l1.y))/
                Mathf.Sqrt(Mathf.Pow(l2.x - l1.x, 2) + Mathf.Pow(l2.y - l1.y, 2));
    }

	public static Vector2 GetPointOnBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
	{
    	float u = 1f - t;
    	float t2 = t * t;
    	float u2 = u * u;
    	float u3 = u2 * u;
    	float t3 = t2 * t;
 
    	Vector2 result =
    	    (u3) * p0 +
    	    (3f * u2 * t) * p1 +
    	    (3f * u * t2) * p2 +
    	    (t3) * p3;
 
    	return result;
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
	public static Vector2 easeOutCubicVector2(Vector2 start, Vector2 end ,float time)
	{
		return new Vector2(easeOutCubic(start.x,end.x,time),easeOutCubic(start.y,end.y,time));
	}
	public static float easeOutCubic(float start, float end, float value)
	{
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}
	public static float easeInCubic(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value + start;
	}
	public static bool LineIntersection(out Vector2 intersection, Vector2 linePoint1, Vector2 lineVec1, Vector2 linePoint2, Vector2 lineVec2)
	{
	        Vector3 lineVec3 = linePoint2 - linePoint1;
	        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
	        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);
	
	        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
	
	        //is coplanar, and not parrallel
	        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
	        {
	            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
	            intersection = linePoint1 + (lineVec1 * s);
	            return true;
	        }
	        else
	        {
	            intersection = Vector3.zero;
	            return false;
	        }
	}
	public static Vector3 Lemniscate_Gerono(float factor, float time)
	{
		return new Vector3(Mathf.Cos(time), Mathf.Sin(2*time) * .5f) * factor;
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

	public static Vector3 RandomVector3(float xmin, float xmax, float ymin, float ymax, float zmin, float zmax)
	{
		return new Vector3(Random.Range(xmin,xmax),Random.Range(ymin,ymax),Random.Range(zmin,zmax));
	}

	public static Vector3 RandomVector3(float xmin, float xmax, float ymin, float ymax)
	{
		return new Vector3(Random.Range(xmin,xmax),Random.Range(ymin,ymax),0f);
	}

	public static Vector3 RandomCircle(float radius)
	{
		return new Vector3(Random.Range(-radius,radius),Random.Range(-radius,radius),0f);
	}

	public static Vector3 RandomVector3(float min, float max)
	{
		return new Vector3(Random.Range(min,max),Random.Range(min,max),0f);
	}

	public static int RandomInt(int start, int end)
	{
		return Random.Range(start,end + 1);
	}
}
