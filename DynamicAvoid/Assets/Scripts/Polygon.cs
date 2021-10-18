using UnityEngine;
using System;
using System.Collections.Generic;

public static class Polygon
{

	/** Signed area of a triangle in the XZ plane multiplied by 2.
	 * This will be negative for clockwise triangles and positive for counter-clockwise ones */
	public static long TriangleArea2(Int3 a, Int3 b, Int3 c)
	{
		return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z);

	}

	/** Signed area of a triangle in the XZ plane multiplied by 2.
	 * This will be negative for clockwise triangles and positive for counter-clockwise ones.
	 */
	public static float TriangleArea2(Vector3 a, Vector3 b, Vector3 c)
	{
		return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z);
	}

	/** Signed area of a triangle in the XZ plane multiplied by 2.
	 * This will be negative for clockwise triangles and positive for counter-clockwise ones.
	 * This method can handle larger numbers than TriangleArea2(Int3)
	 */
	[System.Obsolete("Use TriangleArea2 instead to avoid confusion regarding the factor 2")]
	public static long TriangleArea(Int3 a, Int3 b, Int3 c)
	{
		return TriangleArea2(a, b, c);
	}

	/** Signed area of a triangle in the XZ plane multiplied by 2.
	 * This will be negative for clockwise triangles and positive for counter-clockwise ones.
	 * Idential to TriangleArea2(Vector3), kept for compability.
	 */
	[System.Obsolete("Use TriangleArea2 instead to avoid confusion regarding the factor 2")]
	public static float TriangleArea(Vector3 a, Vector3 b, Vector3 c)
	{
		return TriangleArea2(a, b, c);
	}

	/** Returns if the triangle \a ABC contains the point \a p in XZ space */
	public static bool ContainsPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
	{
		return Polygon.IsClockwiseMargin(a, b, p) && Polygon.IsClockwiseMargin(b, c, p) && Polygon.IsClockwiseMargin(c, a, p);
	}

	/** Returns if the triangle \a ABC contains the point \a p */
	public static bool ContainsPoint(Int2 a, Int2 b, Int2 c, Int2 p)
	{
		return Polygon.IsClockwiseMargin(a, b, p) && Polygon.IsClockwiseMargin(b, c, p) && Polygon.IsClockwiseMargin(c, a, p);
	}

	/** Returns if the triangle \a ABC contains the point \a p */
	public static bool ContainsPoint(Int3 a, Int3 b, Int3 c, Int3 p)
	{
		return Polygon.IsClockwiseMargin(a, b, p) && Polygon.IsClockwiseMargin(b, c, p) && Polygon.IsClockwiseMargin(c, a, p);
	}

	/** Checks if \a p is inside the polygon.
	 * \author http://unifycommunity.com/wiki/index.php?title=PolyContainsPoint (Eric5h5)
	 */
	public static bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
	{
		int j = polyPoints.Length - 1;
		bool inside = false;

		for (int i = 0; i < polyPoints.Length; j = i++)
		{
			if (((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) &&
				 (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x))
				inside = !inside;
		}
		return inside;
	}

	/** Checks if \a p is inside the polygon (XZ space)
	 * \author http://unifycommunity.com/wiki/index.php?title=PolyContainsPoint (Eric5h5)
	 */
	public static bool ContainsPoint(Vector3[] polyPoints, Vector3 p)
	{
		int j = polyPoints.Length - 1;
		bool inside = false;

		for (int i = 0; i < polyPoints.Length; j = i++)
		{
			if (((polyPoints[i].z <= p.z && p.z < polyPoints[j].z) || (polyPoints[j].z <= p.z && p.z < polyPoints[i].z)) &&
				 (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.z - polyPoints[i].z) / (polyPoints[j].z - polyPoints[i].z) + polyPoints[i].x))
				inside = !inside;
		}
		return inside;
	}

	/** Returns if \a p lies on the left side of the line \a a - \a b. Uses XZ space.
	  * Does not return true if the points are colinear. */
	public static bool LeftNotColinear(Vector3 a, Vector3 b, Vector3 p)
	{
		return (b.x - a.x) * (p.z - a.z) - (p.x - a.x) * (b.z - a.z) < -float.Epsilon;
	}

	/** Returns if \a p lies on the left side of the line \a a - \a b. Uses XZ space. Also returns true if the points are colinear */
	public static bool Left(Vector3 a, Vector3 b, Vector3 p)
	{
		return (b.x - a.x) * (p.z - a.z) - (p.x - a.x) * (b.z - a.z) <= 0;
	}

	/** Returns if \a p lies on the left side of the line \a a - \a b. Also returns true if the points are colinear */
	public static bool Left(Vector2 a, Vector2 b, Vector2 p)
	{
		return (b.x - a.x) * (p.y - a.y) - (p.x - a.x) * (b.y - a.y) <= 0;
	}

	/** Returns if \a p lies on the left side of the line \a a - \a b. Uses XZ space. Also returns true if the points are colinear */
	public static bool Left(Int3 a, Int3 b, Int3 c)
	{
		return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) <= 0;
	}

	/** Returns if \a p lies on the left side of the line \a a - \a b. Uses XZ space. */
	public static bool LeftNotColinear(Int3 a, Int3 b, Int3 c)
	{
		return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) < 0;
	}

	/** Returns if \a p lies on the left side of the line \a a - \a b. Also returns true if the points are colinear */
	public static bool Left(Int2 a, Int2 b, Int2 c)
	{
		return (long)(b.x - a.x) * (long)(c.y - a.y) - (long)(c.x - a.x) * (long)(b.y - a.y) <= 0;
	}

	/** Returns if the points a in a clockwise order.
	 * Will return true even if the points are colinear or very slightly counter-clockwise
	 * (if the signed area of the triangle formed by the points has an area less than or equals to float.Epsilon) */
	public static bool IsClockwiseMargin(Vector3 a, Vector3 b, Vector3 c)
	{
		return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z) <= float.Epsilon;
	}

	/** Returns if the points a in a clockwise order */
	public static bool IsClockwise(Vector3 a, Vector3 b, Vector3 c)
	{
		return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z) < 0;
	}

	/** Returns if the points a in a clockwise order */
	public static bool IsClockwise(Int3 a, Int3 b, Int3 c)
	{
		return LeftNotColinear(a, b, c);
	}

	/** Returns true if the points a in a clockwise order or if they are colinear */
	public static bool IsClockwiseMargin(Int3 a, Int3 b, Int3 c)
	{
		return Left(a, b, c);
	}

	/** Returns true if the points a in a clockwise order or if they are colinear */
	public static bool IsClockwiseMargin(Int2 a, Int2 b, Int2 c)
	{
		return Left(a, b, c);
	}

	/** Returns if the points are colinear (lie on a straight line) */
	public static bool IsColinear(Int3 a, Int3 b, Int3 c)
	{
		return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) == 0;
	}

	/** Returns if the points are colinear (lie on a straight line) */
	public static bool IsColinearAlmost(Int3 a, Int3 b, Int3 c)
	{
		long v = (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z);
		return v > -1 && v < 1;
	}

	/** Returns if the points are colinear (lie on a straight line) */
	public static bool IsColinear(Vector3 a, Vector3 b, Vector3 c)
	{
		float v = (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z);
		//Epsilon not chosen with much though, just that float.Epsilon was a bit too small.
		return v <= 0.0000001f && v >= -0.0000001f;
	}

	/** Returns if the line segment \a a2 - \a b2 intersects the infinite line \a a - \a b. a-b is infinite, a2-b2 is not infinite */
	public static bool IntersectsUnclamped(Vector3 a, Vector3 b, Vector3 a2, Vector3 b2)
	{
		return Left(a, b, a2) != Left(a, b, b2);
	}

	/** Returns if the line segment \a a2 - \a b2 intersects the line segment \a a - \a b.
	 * If only the endpoints coincide, the result is undefined (may be true or false).
	 */
	public static bool Intersects(Int2 a, Int2 b, Int2 a2, Int2 b2)
	{
		return Left(a, b, a2) != Left(a, b, b2) && Left(a2, b2, a) != Left(a2, b2, b);
	}

	/** Returns if the line segment \a a2 - \a b2 intersects the line segment \a a - \a b.
	 * If only the endpoints coincide, the result is undefined (may be true or false).
	 *
	 * \note XZ space
	 */
	public static bool Intersects(Int3 a, Int3 b, Int3 a2, Int3 b2)
	{
		return Left(a, b, a2) != Left(a, b, b2) && Left(a2, b2, a) != Left(a2, b2, b);
	}

	/** Returns if the two line segments intersects. The lines are NOT treated as infinite (just for clarification)
	  * \see IntersectionPoint
	  */
	public static bool Intersects(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
	{

		Vector3 dir1 = end1 - start1;
		Vector3 dir2 = end2 - start2;

		float den = dir2.z * dir1.x - dir2.x * dir1.z;

		if (den == 0)
		{
			return false;
		}

		float nom = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);
		float nom2 = dir1.x * (start1.z - start2.z) - dir1.z * (start1.x - start2.x);
		float u = nom / den;
		float u2 = nom2 / den;

		if (u < 0F || u > 1F || u2 < 0F || u2 > 1F)
		{
			return false;
		}

		return true;
	}

	/** Intersection point between two infinite lines.
	 * Lines are treated as infinite. If the lines are parallel 'start1' will be returned. Intersections are calculated on the XZ plane.
	 */
	public static Vector3 IntersectionPointOptimized(Vector3 start1, Vector3 dir1, Vector3 start2, Vector3 dir2)
	{

		float den = dir2.z * dir1.x - dir2.x * dir1.z;

		if (den == 0)
		{
			return start1;
		}

		float nom = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);

		float u = nom / den;

		return start1 + dir1 * u;
	}

	/** Intersection point between two infinite lines.
	 * Lines are treated as infinite. If the lines are parallel 'start1' will be returned. Intersections are calculated on the XZ plane.
	 */
	public static Vector3 IntersectionPointOptimized(Vector3 start1, Vector3 dir1, Vector3 start2, Vector3 dir2, out bool intersects)
	{

		float den = dir2.z * dir1.x - dir2.x * dir1.z;

		if (den == 0)
		{
			intersects = false;
			return start1;
		}

		float nom = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);

		float u = nom / den;

		intersects = true;
		return start1 + dir1 * u;
	}

	/** Returns if the ray (start1, end1) intersects the segment (start2, end2).
	 * false is returned if the lines are parallel.
	 * Only the XZ coordinates are used.
	 * \todo Double check that this actually works
	 */
	public static bool IntersectionFactorRaySegment(Int3 start1, Int3 end1, Int3 start2, Int3 end2)
	{

		Int3 dir1 = end1 - start1;
		Int3 dir2 = end2 - start2;

		long den = dir2.z * dir1.x - dir2.x * dir1.z;

		if (den == 0)
		{
			return false;
		}

		long nom = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);
		long nom2 = dir1.x * (start1.z - start2.z) - dir1.z * (start1.x - start2.x);

		//factor1 < 0
		// If both have the same sign, then nom/den < 0 and thus the segment cuts the ray before the ray starts
		if (!(nom < 0 ^ den < 0))
		{
			return false;
		}

		//factor2 < 0
		if (!(nom2 < 0 ^ den < 0))
		{
			return false;
		}

		if ((den >= 0 && nom2 > den) || (den < 0 && nom2 <= den))
		{
			return false;
		}
		//factor1 = (float)nom/den;
		//factor2 = (float)nom2/den;
		return true;
	}

	/** Returns the intersection factors for line 1 and line 2. The intersection factors is a distance along the line \a start - \a end where the other line intersects it.\n
	 * \code intersectionPoint = start1 + factor1 * (end1-start1) \endcode
	 * \code intersectionPoint2 = start2 + factor2 * (end2-start2) \endcode
	 * Lines are treated as infinite.\n
	 * false is returned if the lines are parallel and true if they are not.
	 * Only the XZ coordinates are used.
	 */
	public static bool IntersectionFactor(Int3 start1, Int3 end1, Int3 start2, Int3 end2, out float factor1, out float factor2)
	{

		Int3 dir1 = end1 - start1;
		Int3 dir2 = end2 - start2;

		long den = dir2.z * dir1.x - dir2.x * dir1.z;

		if (den == 0)
		{
			factor1 = 0;
			factor2 = 0;
			return false;
		}

		long nom = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);
		long nom2 = dir1.x * (start1.z - start2.z) - dir1.z * (start1.x - start2.x);

		factor1 = (float)nom / den;
		factor2 = (float)nom2 / den;

		return true;
	}

	/** Returns the intersection factors for line 1 and line 2. The intersection factors is a distance along the line \a start - \a end where the other line intersects it.\n
	 * \code intersectionPoint = start1 + factor1 * (end1-start1) \endcode
	 * \code intersectionPoint2 = start2 + factor2 * (end2-start2) \endcode
	 * Lines are treated as infinite.\n
	 * false is returned if the lines are parallel and true if they are not.
	 * Only the XZ coordinates are used.
	 */
	public static bool IntersectionFactor(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out float factor1, out float factor2)
	{

		Vector3 dir1 = end1 - start1;
		Vector3 dir2 = end2 - start2;

		float den = dir2.z * dir1.x - dir2.x * dir1.z;

		if (den <= 0.00001f && den >= -0.00001f)
		{
			factor1 = 0;
			factor2 = 0;
			return false;
		}

		float nom = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);
		float nom2 = dir1.x * (start1.z - start2.z) - dir1.z * (start1.x - start2.x);

		float u = nom / den;
		float u2 = nom2 / den;

		factor1 = u;
		factor2 = u2;

		return true;
	}

	/** Returns the intersection factor for line 1 with ray 2.
	 * The intersection factors is a factor distance along the line \a start - \a end where the other line intersects it.\n
	 * \code intersectionPoint = start1 + factor * (end1-start1) \endcode
	 * Lines are treated as infinite.\n
	 *
	 * The second "line" is treated as a ray, meaning only matches on start2 or forwards towards end2 (and beyond) will be returned
	 * If the point lies on the wrong side of the ray start, Nan will be returned.
	 *
	 * NaN is returned if the lines are parallel. */
	public static float IntersectionFactorRay(Int3 start1, Int3 end1, Int3 start2, Int3 end2)
	{

		Int3 dir1 = end1 - start1;
		Int3 dir2 = end2 - start2;

		int den = dir2.z * dir1.x - dir2.x * dir1.z;

		if (den == 0)
		{
			return float.NaN;
		}

		int nom = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);
		int nom2 = dir1.x * (start1.z - start2.z) - dir1.z * (start1.x - start2.x);

		if ((float)nom2 / den < 0)
		{
			return float.NaN;
		}
		return (float)nom / den;
	}

	/** Returns the intersection factor for line 1 with line 2.
	 * The intersection factor is a distance along the line \a start1 - \a end1 where the line \a start2 - \a end2 intersects it.\n
	 * \code intersectionPoint = start1 + intersectionFactor * (end1-start1) \endcode.
	 * Lines are treated as infinite.\n
	 * -1 is returned if the lines are parallel (note that this is a valid return value if they are not parallel too) */
	public static float IntersectionFactor(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
	{

		Vector3 dir1 = end1 - start1;
		Vector3 dir2 = end2 - start2;

		float den = dir2.z * dir1.x - dir2.x * dir1.z;

		if (den == 0)
		{
			return -1;
		}

		float nom = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);

		float u = nom / den;

		return u;
	}

	/** Returns the intersection point between the two lines. Lines are treated as infinite. \a start1 is returned if the lines are parallel */
	public static Vector3 IntersectionPoint(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
	{
		bool s;
		return IntersectionPoint(start1, end1, start2, end2, out s);
	}

	/** Returns the intersection point between the two lines. Lines are treated as infinite. \a start1 is returned if the lines are parallel */
	public static Vector3 IntersectionPoint(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out bool intersects)
	{

		Vector3 dir1 = end1 - start1;
		Vector3 dir2 = end2 - start2;

		float den = dir2.z * dir1.x - dir2.x * dir1.z;

		if (den == 0)
		{
			intersects = false;
			return start1;
		}

		float nom = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);

		float u = nom / den;

		intersects = true;
		return start1 + dir1 * u;
	}

	/** Returns the intersection point between the two lines. Lines are treated as infinite. \a start1 is returned if the lines are parallel */
	public static Vector2 IntersectionPoint(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
	{
		bool s;
		return IntersectionPoint(start1, end1, start2, end2, out s);
	}

	/** Returns the intersection point between the two lines. Lines are treated as infinite. \a start1 is returned if the lines are parallel */
	public static Vector2 IntersectionPoint(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2, out bool intersects)
	{

		Vector2 dir1 = end1 - start1;
		Vector2 dir2 = end2 - start2;

		float den = dir2.y * dir1.x - dir2.x * dir1.y;

		if (den == 0)
		{
			intersects = false;
			return start1;
		}

		float nom = dir2.x * (start1.y - start2.y) - dir2.y * (start1.x - start2.x);

		float u = nom / den;

		intersects = true;
		return start1 + dir1 * u;
	}

	/** Returns the intersection point between the two line segments in XZ space.
	 * Lines are NOT treated as infinite. \a start1 is returned if the line segments do not intersect
	 * The point will be returned along the line [start1, end1] (this matters only for the y coordinate).
	 */
	public static Vector3 SegmentIntersectionPoint(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out bool intersects)
	{
		Vector3 dir1 = end1 - start1;
		Vector3 dir2 = end2 - start2;

		float den = dir2.z * dir1.x - dir2.x * dir1.z;

		if (den == 0)
		{
			intersects = false;
			return start1;
		}

		float nom = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);
		float nom2 = dir1.x * (start1.z - start2.z) - dir1.z * (start1.x - start2.x);
		float u = nom / den;
		float u2 = nom2 / den;

		if (u < 0F || u > 1F || u2 < 0F || u2 > 1F)
		{
			intersects = false;
			return start1;
		}

		intersects = true;
		return start1 + dir1 * u;
	}

	public static List<Vector3> hullCache = new List<Vector3>();

	/** Calculates convex hull in XZ space for the points.
	  * Implemented using the very simple Gift Wrapping Algorithm
	  * which has a complexity of O(nh) where \a n is the number of points and \a h is the number of points on the hull,
	  * so it is in the worst case quadratic.
	  */
	public static Vector3[] ConvexHull(Vector3[] points)
	{

		if (points.Length == 0) return new Vector3[0];

		lock (hullCache)
		{
			List<Vector3> hull = hullCache;
			hull.Clear();

#if ASTARDEBUG
				for (int i=0;i<points.Length;i++) {
					Debug.DrawLine (points[i],points[(i+1)%points.Length],Color.blue);
				}
#endif

			int pointOnHull = 0;
			for (int i = 1; i < points.Length; i++) if (points[i].x < points[pointOnHull].x) pointOnHull = i;

			int startpoint = pointOnHull;
			int counter = 0;

			do
			{
				hull.Add(points[pointOnHull]);
				int endpoint = 0;
				for (int i = 0; i < points.Length; i++) if (endpoint == pointOnHull || !Left(points[pointOnHull], points[endpoint], points[i])) endpoint = i;

				pointOnHull = endpoint;

				counter++;
				if (counter > 10000)
				{
					Debug.LogWarning("Infinite Loop in Convex Hull Calculation");
					break;
				}
			} while (pointOnHull != startpoint);

			return hull.ToArray();
		}
	}

	/** Does the line segment intersect the bounding box.
	 * The line is NOT treated as infinite.
	 * \author Slightly modified code from http://www.3dkingdoms.com/weekly/weekly.php?a=21
	 */
	public static bool LineIntersectsBounds(Bounds bounds, Vector3 a, Vector3 b)
	{
		// Put line in box space
		a -= bounds.center;
		b -= bounds.center;

		// Get line midpoint and extent
		var LMid = (a + b) * 0.5F;
		var L = (a - LMid);
		var LExt = new Vector3(Math.Abs(L.x), Math.Abs(L.y), Math.Abs(L.z));

		Vector3 extent = bounds.extents;

		// Use Separating Axis Test
		// Separation vector from box center to line center is LMid, since the line is in box space
		if (Math.Abs(LMid.x) > extent.x + LExt.x) return false;
		if (Math.Abs(LMid.y) > extent.y + LExt.y) return false;
		if (Math.Abs(LMid.z) > extent.z + LExt.z) return false;
		// Crossproducts of line and each axis
		if (Math.Abs(LMid.y * L.z - LMid.z * L.y) > (extent.y * LExt.z + extent.z * LExt.y)) return false;
		if (Math.Abs(LMid.x * L.z - LMid.z * L.x) > (extent.x * LExt.z + extent.z * LExt.x)) return false;
		if (Math.Abs(LMid.x * L.y - LMid.y * L.x) > (extent.x * LExt.y + extent.y * LExt.x)) return false;
		// No separating axis, the line intersects
		return true;
	}

	/** Subdivides \a path and returns the new array with interpolated values.
	 * The returned array is \a path subdivided \a subdivisions times, the resulting points are interpolated using Mathf.SmoothStep.\n
	 * If \a subdivisions is less or equal to 0 (zero), the original array will be returned */
	public static Vector3[] Subdivide(Vector3[] path, int subdivisions)
	{

		subdivisions = subdivisions < 0 ? 0 : subdivisions;

		if (subdivisions == 0)
		{
			return path;
		}

		var path2 = new Vector3[(path.Length - 1) * (int)Mathf.Pow(2, subdivisions) + 1];

		int c = 0;
		for (int p = 0; p < path.Length - 1; p++)
		{
			float step = 1.0F / Mathf.Pow(2, subdivisions);

			for (float i = 0; i < 1.0F; i += step)
			{
				path2[c] = Vector3.Lerp(path[p], path[p + 1], Mathf.SmoothStep(0, 1, i));
				c++;
			}
		}

		path2[c] = path[path.Length - 1];
		return path2;
	}

	/** Returns the closest point on the triangle. The \a triangle array must have a length of at least 3.
	 * \see ClosesPointOnTriangle(Vector3,Vector3,Vector3,Vector3);
	 */
	public static Vector3 ClosestPointOnTriangle(Vector3[] triangle, Vector3 point)
	{
		return ClosestPointOnTriangle(triangle[0], triangle[1], triangle[2], point);
	}

	/** Returns the closest point on the triangle.
	 *
	 * \author Got code from the internet, changed a bit to work with the Unity API
	 *
	 */
	public static Vector3 ClosestPointOnTriangle(Vector3 tr0, Vector3 tr1, Vector3 tr2, Vector3 point)
	{
		Vector3 diff = tr0 - point;
		Vector3 edge0 = tr1 - tr0;
		Vector3 edge1 = tr2 - tr0;
		float a00 = edge0.sqrMagnitude;
		float a01 = Vector3.Dot(edge0, edge1);
		float a11 = edge1.sqrMagnitude;
		float b0 = Vector3.Dot(diff, edge0);
		float b1 = Vector3.Dot(diff, edge1);
		//float c = diff.sqrMagnitude;
		float det = a00 * a11 - a01 * a01;

		float s = a01 * b1 - a11 * b0;
		float t = a01 * b0 - a00 * b1;
		//float sqrDistance;

		if (s + t <= det)
		{
			if (s < 0f)
			{
				if (t < 0f)  // region 4
				{
					if (b0 < 0f)
					{
						t = 0f;
						if (-b0 >= a00)
						{
							s = 1f;
							//sqrDistance = a00 + (2f) * b0 + c;
						}
						else
						{
							s = -b0 / a00;
							//sqrDistance = b0 * s + c;
						}
					}
					else
					{
						s = 0f;
						if (b1 >= 0f)
						{
							t = 0f;
							//sqrDistance = c;
						}
						else if (-b1 >= a11)
						{
							t = 1f;
							//sqrDistance = a11 + (2f) * b1 + c;
						}
						else
						{
							t = -b1 / a11;
							//sqrDistance = b1 * t + c;
						}
					}
				}
				else  // region 3
				{
					s = 0f;
					if (b1 >= 0f)
					{
						t = 0f;
						//sqrDistance = c;
					}
					else if (-b1 >= a11)
					{
						t = 1f;
						//sqrDistance = a11 + (2f) * b1 + c;
					}
					else
					{
						t = -b1 / a11;
						// sqrDistance = b1 * t + c;
					}
				}
			}
			else if (t < 0f)  // region 5
			{
				t = 0f;
				if (b0 >= 0f)
				{
					s = 0f;
					// sqrDistance = c;
				}
				else if (-b0 >= a00)
				{
					s = 1f;
					//sqrDistance = a00 + (2f) * b0 + c;
				}
				else
				{
					s = -b0 / a00;
					//  sqrDistance = b0 * s + c;
				}
			}
			else  // region 0
			{
				// minimum at interior point
				float invDet = 1f / det;
				s *= invDet;
				t *= invDet;
				// sqrDistance = s * (a00 * s + a01 * t + (2f) * b0) + t * (a01 * s + a11 * t + (2f) * b1) + c;
			}
		}
		else
		{
			float tmp0, tmp1, numer, denom;

			if (s < 0f)  // region 2
			{
				tmp0 = a01 + b0;
				tmp1 = a11 + b1;
				if (tmp1 > tmp0)
				{
					numer = tmp1 - tmp0;
					denom = a00 - (2f) * a01 + a11;
					if (numer >= denom)
					{
						s = 1f;
						t = 0f;
						// sqrDistance = a00 + (2f) * b0 + c;
					}
					else
					{
						s = numer / denom;
						t = 1f - s;
						// sqrDistance = s * (a00 * s + a01 * t + (2f) * b0) + t * (a01 * s + a11 * t + (2f) * b1) + c;
					}
				}
				else
				{
					s = 0f;
					if (tmp1 <= 0f)
					{
						t = 1f;
						// sqrDistance = a11 + (2f) * b1 + c;
					}
					else if (b1 >= 0f)
					{
						t = 0f;
						//  sqrDistance = c;
					}
					else
					{
						t = -b1 / a11;
						//  sqrDistance = b1 * t + c;
					}
				}
			}
			else if (t < 0f)  // region 6
			{
				tmp0 = a01 + b1;
				tmp1 = a00 + b0;
				if (tmp1 > tmp0)
				{
					numer = tmp1 - tmp0;
					denom = a00 - (2f) * a01 + a11;
					if (numer >= denom)
					{
						t = 1f;
						s = 0f;
						// sqrDistance = a11 + (2f) * b1 + c;
					}
					else
					{
						t = numer / denom;
						s = 1f - t;
						// sqrDistance = s * (a00 * s + a01 * t + (2f) * b0) + t * (a01 * s + a11 * t + (2f) * b1) + c;
					}
				}
				else
				{
					t = 0f;
					if (tmp1 <= 0f)
					{
						s = 1f;
						//sqrDistance = a00 + (2f) * b0 + c;
					}
					else if (b0 >= 0f)
					{
						s = 0f;
						// sqrDistance = c;
					}
					else
					{
						s = -b0 / a00;
						//  sqrDistance = b0 * s + c;
					}
				}
			}
			else  // region 1
			{
				numer = a11 + b1 - a01 - b0;
				if (numer <= 0f)
				{
					s = 0f;
					t = 1f;
					//  sqrDistance = a11 + (2f) * b1 + c;
				}
				else
				{
					denom = a00 - (2f) * a01 + a11;
					if (numer >= denom)
					{
						s = 1f;
						t = 0f;
						//  sqrDistance = a00 + (2f) * b0 + c;
					}
					else
					{
						s = numer / denom;
						t = 1f - s;
						// sqrDistance = s * (a00 * s + a01 * t + (2f) * b0) + t * (a01 * s + a11 * t + (2f) * b1) + c;
					}
				}
			}
		}

		// Account for numerical round-off error.
		//  if (sqrDistance < 0f)
		//  {
		//       sqrDistance = 0f;
		//    }

		return tr0 + s * edge0 + t * edge1;
	}

	/** Get the 3D minimum distance between 2 segments
	* Input:  two 3D line segments S1 and S2
	* \returns the shortest squared distance between S1 and S2
	*/
	public static float DistanceSegmentSegment3D(Vector3 s1, Vector3 e1, Vector3 s2, Vector3 e2)
	{
		Vector3 u = e1 - s1;
		Vector3 v = e2 - s2;
		Vector3 w = s1 - s2;
		float a = Vector3.Dot(u, u);         // always >= 0
		float b = Vector3.Dot(u, v);
		float c = Vector3.Dot(v, v);         // always >= 0
		float d = Vector3.Dot(u, w);
		float e = Vector3.Dot(v, w);
		float D = a * c - b * b;        // always >= 0
		float sc, sN, sD = D;       // sc = sN / sD, default sD = D >= 0
		float tc, tN, tD = D;       // tc = tN / tD, default tD = D >= 0

		// compute the line parameters of the two closest points
		if (D < 0.000001f)
		{ // the lines are almost parallel
			sN = 0.0f;         // force using point P0 on segment S1
			sD = 1.0f;         // to prevent possible division by 0.0 later
			tN = e;
			tD = c;
		}
		else
		{                 // get the closest points on the infinite lines
			sN = (b * e - c * d);
			tN = (a * e - b * d);
			if (sN < 0.0f)
			{        // sc < 0 => the s=0 edge is visible
				sN = 0.0f;
				tN = e;
				tD = c;
			}
			else if (sN > sD)
			{  // sc > 1  => the s=1 edge is visible
				sN = sD;
				tN = e + b;
				tD = c;
			}
		}

		if (tN < 0.0f)
		{            // tc < 0 => the t=0 edge is visible
			tN = 0.0f;
			// recompute sc for this edge
			if (-d < 0.0f)
				sN = 0.0f;
			else if (-d > a)
				sN = sD;
			else
			{
				sN = -d;
				sD = a;
			}
		}
		else if (tN > tD)
		{      // tc > 1  => the t=1 edge is visible
			tN = tD;
			// recompute sc for this edge
			if ((-d + b) < 0.0f)
				sN = 0;
			else if ((-d + b) > a)
				sN = sD;
			else
			{
				sN = (-d + b);
				sD = a;
			}
		}
		// finally do the division to get sc and tc
		sc = (Math.Abs(sN) < 0.000001f ? 0.0f : sN / sD);
		tc = (Math.Abs(tN) < 0.000001f ? 0.0f : tN / tD);

		// get the difference of the two closest points
		Vector3 dP = w + (sc * u) - (tc * v);  // =  S1(sc) - S2(tc)

		return dP.sqrMagnitude;   // return the closest distance
	}

}