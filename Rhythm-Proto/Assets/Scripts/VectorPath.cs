/* === Copyright Luc Kadletz 2019 === */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class VectorPath
{

	public class Segment 
	{
		public Vector3 Start;
		public Vector3 End;
		public float Length 
		{
			get
			{
				return Vector3.Distance(Start, End);
			}
		}
	}

	public Vector3[] Points;

	public Vector3 Begining
	{
		get
		{
			return Points.First();
		}
	}

	public Vector3 End
	{
		get
		{
			return Points.Last();
		}
	}

    public VectorPath(IList<Vector3> points)
    {
		Points = points.ToArray();
    }

	public Vector3 GetPointAlongPath(float t)
	{
		if(t <= 0.0f) return Begining;
		if(t >= 1.0f) return End;
		
		float seg_t;
		Segment seg = GetSegment(t, out seg_t);

		if(seg != null)
		{
			// barrycentric coords yo
			return seg.Start + seg_t * (seg.End - seg.Start);
		}

		return Begining;
	}

	public float Distance()
	{
		float dist = 0.0f;
		for(int i = 1; i < Points.Length; ++i)
		{
			dist += Vector3.Distance(Points[i-1], Points[i]);
		}
		return dist;
	}

	public Segment GetSegment(float t, out float seg_t)
	{
		float partial = 0.0f;
		seg_t = 0.0f;
		float total = Distance();

		if(t < 0.0f || t > 1.0f)
		{
			return null;
		}

		// Walk the path until we cross t
		for(int i = 1; i < Points.Length; ++i)
		{
			Segment seg = new Segment
			{
				Start = Points[i-1],
				End = Points[i],
			};

			Debug.DrawRay(seg.Start, seg.End, Color.cyan, 0.01f);

			// If we were on this line, we'd be this far along 
			seg_t = (t - partial);
			// Move partial along a percentage of total dist
			partial += seg.Length / total;
			if(partial >= t)
			{
				return seg;
			}
		}
		
		return null;
	}
}
