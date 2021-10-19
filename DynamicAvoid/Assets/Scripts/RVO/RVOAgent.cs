using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.RVO
{
	public class Agent
	{

		Vector2 smoothPos;

		public Vector2 Position
		{
			get;
			private set;
		}

		private FlowDir.Map _map;

		public Vector2 InterpolatedPosition
		{
			get { return smoothPos; }
		}

		public Vector2 DesiredVelocity { get { return desiredVelocity; } set { desiredVelocity = value; } }

		//Current values for double buffer calculation

		public float radius, height, maxSpeed, neighbourDist, agentTimeHorizon, obstacleTimeHorizon, weight;
		public bool locked = false;

		public int maxNeighbours = 10;
		public Vector2 position, desiredVelocity, prevSmoothPos;

		public float Radius { get { return radius; } set { radius = value; } }
		public Vector2 Velocity { get; set; }
		public bool DebugDraw { get; set; }

		public int MaxNeighbours { get; set; }

		/** Used internally for a linked list */
		internal Agent next;
		private int _id;
		public int id { get { return _id; } }

		private Vector2 velocity;
		internal Vector2 newVelocity;

		public List<Agent> neighbours = new List<Agent>();
		public List<float> neighbourDists = new List<float>();


		public Agent(FlowDir.Unit unit, FlowDir.Map map, Vector2 pos)
		{
			_id = id;
			_map = map;

			maxSpeed = unit.Group == 1 ? 20 : 100;
			if(unit.id == 1)
            {
				maxSpeed = 25;
			}

			agentTimeHorizon = 2;
			neighbourDist = 30;
			Radius = 5f;
			MaxNeighbours = 10;

			StartMove(pos);
		}

		public void StartMove(Vector2 pos)
		{
			position = pos;
			Position = position;
			prevSmoothPos = position;
			smoothPos = position;
		}

		// Update is called once per frame
		public void Update(float deltaTime)
		{
			var prevPos = position;
			var prevVel = velocity;

			velocity = newVelocity;

			prevSmoothPos = smoothPos;

			//Note the case P/p
			//position = Position;
			position = prevSmoothPos;
			//Debug.Log(desiredVelocity+"~~~~~~~~~~~~"+deltaTime + "#####" + velocity);
			position = position + velocity * deltaTime * maxSpeed;
			//if (Vector2.Angle(prevVel, newVelocity) > 90)
			//      {
			//	Debug.Log(prevVel + "~~~~~~~~~~~~~~" + newVelocity);
			//	Debug.Log(Vector2.Distance(position, prevPos) + "#####" + maxSpeed);
			//}
			Position = position;
		}

		public void Interpolate(float t)
		{
			smoothPos = prevSmoothPos + (Position - prevSmoothPos) * t;
		}

		public static System.Diagnostics.Stopwatch watch1 = new System.Diagnostics.Stopwatch();
		public static System.Diagnostics.Stopwatch watch2 = new System.Diagnostics.Stopwatch();

		public void CalculateNeighbours()
		{

			neighbours.Clear();
			neighbourDists.Clear();

			float rangeSq;

			//watch1.Start ();
			if (MaxNeighbours > 0)
			{
				rangeSq = neighbourDist * neighbourDist;

				//simulator.KDTree.GetAgentNeighbours (this, rangeSq);
				_map.Quadtree.Query(new Vector2(position.x, position.y), neighbourDist, this);

			}
			//watch1.Stop ();

			rangeSq = (obstacleTimeHorizon * maxSpeed + radius);
			rangeSq *= rangeSq;
			// Obstacles disabled at the moment
			//simulator.KDTree.GetObstacleNeighbours (this, rangeSq);

		}

		float Sqr(float x)
		{
			return x * x;
		}

		public float InsertAgentNeighbour(Agent agent, float rangeSq)
		{
			if (this == agent) return rangeSq;

			//2D Dist
			float distSqr = Vector2.SqrMagnitude(agent.position - position);
			if (distSqr < rangeSq)
			{
				if (neighbours.Count < maxNeighbours)
				{
					neighbours.Add(agent);
					neighbourDists.Add(distSqr);
				}

				int i = neighbours.Count - 1;
				if (distSqr < neighbourDists[i])
				{
					while (i != 0 && distSqr < neighbourDists[i - 1])
					{
						neighbours[i] = neighbours[i - 1];
						neighbourDists[i] = neighbourDists[i - 1];
						i--;
					}
					neighbours[i] = agent;
					neighbourDists[i] = distSqr;
				}

				if (neighbours.Count == maxNeighbours)
				{
					rangeSq = neighbourDists[neighbourDists.Count - 1];
				}
			}
			return rangeSq;
		}

		/*public void UpdateNeighbours () {
			neighbours.Clear ();
			float sqrDist = neighbourDistance*neighbourDistance;
			for ( int i = 0; i < simulator.agents.Count; i++ ) {
				float dist = (simulator.agents[i].position - position).sqrMagnitude;
				if ( dist <= sqrDist ) {
					neighbours.Add ( simulator.agents[i] );
				}
			}
		}*/


		public struct VO
		{

			public Vector2 origin, center;

			Vector2 line1, line2, tangentDir1, dir2;

			Vector2 cutoffLine, cutoffDir;

			float sqrCutoffDistance;

			bool leftSide;

			public bool colliding;

			float radius;

			float weightFactor;

			/** Creates a VO for avoiding another agent */
			public VO(Vector2 center, Vector2 offset, float radius, Vector2 sideChooser, float inverseDt, float weightFactor)
			{

				// Adjusted so that a parameter weightFactor of 1 will be the default ("natural") weight factor
				this.weightFactor = weightFactor * 0.5f;

				//this.radius = radius;
				Vector2 globalCenter;
				this.origin = offset;
				weightFactor = 0.5f;
				// Collision?
				if (center.magnitude < radius)
				{
					colliding = true;
					leftSide = false;

					line1 = center.normalized * (center.magnitude - radius);
					tangentDir1 = new Vector2(line1.y, -line1.x).normalized;
					line1 += offset;

					cutoffDir = Vector2.zero;
					cutoffLine = Vector2.zero;
					sqrCutoffDistance = 0;
					dir2 = Vector2.zero;
					line2 = Vector2.zero;
					this.center = Vector2.zero;
					this.radius = 0;
				}
				else
				{

					colliding = false;

					center *= inverseDt;
					radius *= inverseDt;
					globalCenter = center + offset;

					sqrCutoffDistance = center.magnitude - radius;

					this.center = center;
					cutoffLine = center.normalized * sqrCutoffDistance;
					cutoffDir = new Vector2(-cutoffLine.y, cutoffLine.x).normalized;
					cutoffLine += offset;

					sqrCutoffDistance *= sqrCutoffDistance;
					float alpha = Mathf.Atan2(-center.y, -center.x);

					float delta = Mathf.Abs(Mathf.Acos(radius / center.magnitude));

					this.radius = radius;

					// Bounding Lines

					leftSide = Polygon.Left(Vector2.zero, center, sideChooser);

					// Point on circle
					line1 = new Vector2(Mathf.Cos(alpha + delta), Mathf.Sin(alpha + delta)) * radius;
					// Vector tangent to circle which is the correct line tangent
					tangentDir1 = new Vector2(line1.y, -line1.x).normalized;

					// Point on circle
					line2 = new Vector2(Mathf.Cos(alpha - delta), Mathf.Sin(alpha - delta)) * radius;
					// Vector tangent to circle which is the correct line tangent
					dir2 = new Vector2(line2.y, -line2.x).normalized;

					line1 += globalCenter;
					line2 += globalCenter;
				}
			}

			/** Returns a negative number of if  p lies on the left side of a line which with one point in  a and has a tangent in the direction of  dir.
				* The number can be seen as the double signed area of the triangle {a, a+dir, p} multiplied by the length of  dir.
				* If length(dir)=1 this is also the distance from p to the line {a, a+dir}.
				*/
			public static float Det(Vector2 a, Vector2 dir, Vector2 p)
			{
				return (p.x - a.x) * (dir.y) - (dir.x) * (p.y - a.y);
				//(px*dirY - py*dirX) + (dirX*ay - ax*dirY)
			}

			public Vector2 Sample(Vector2 p, out float weight)
			{

				if (colliding)
				{
					// Calculate double signed area of the triangle consisting of the points
					// {line1, line1+dir1, p}
					float l1 = Det(line1, tangentDir1, p);

					// Serves as a check for which side of the line the point p is
					if (l1 >= 0)
					{
						/*float dot1 = Vector2.Dot ( p - line1, dir1 );

						Vector2 c1 = dot1 * dir1 + line1;
						return (c1-p);*/
						weight = l1 * weightFactor;
						return new Vector2(-tangentDir1.y, tangentDir1.x) * weight; // 10 is an arbitrary constant signifying incompressability
																					// (the higher the value, the more the agents will avoid penetration)
					}
					else
					{
						weight = 0;
						return new Vector2(0, 0);
					}
				}

				float det3 = Det(cutoffLine, cutoffDir, p);
				if (det3 <= 0)
				{
					weight = 0;
					return Vector2.zero;
				}
				else
				{
					float det1 = Det(line1, tangentDir1, p);
					float det2 = Det(line2, dir2, p);
					if (det1 >= 0 && det2 >= 0)
					{
						if (leftSide)
						{
							if (det3 < radius)
							{
								weight = det3 * weightFactor;
								return new Vector2(-cutoffDir.y, cutoffDir.x) * weight;

								/*Vector2 dir = (p - center);
								float magn = dir.magnitude;
								weight = radius-magn;
								dir *= (1.0f/magn)*weight;
								return dir;*/
							}

							weight = det1;
							return new Vector2(-tangentDir1.y, tangentDir1.x) * weight;
						}
						else
						{
							if (det3 < radius)
							{
								weight = det3 * weightFactor;
								return new Vector2(-cutoffDir.y, cutoffDir.x) * weight;
							}

							weight = det2 * weightFactor;
							return new Vector2(-dir2.y, dir2.x) * weight;
						}
					}
				}

				weight = 0;
				return new Vector2(0, 0);
			}


		}

		internal void CalculateVelocity()
		{
			//var vos = _map.vos;
			List<RVO.Agent.VO> vos = new List<RVO.Agent.VO>();

			float inverseAgentTimeHorizon = 1.0f / agentTimeHorizon;

			foreach (Agent other in neighbours)
			{
				if (other == this) continue;

				float totalRadius = radius + other.radius;
				Vector2 voBoundingOrigin = other.position - position;
				Vector2 relativeVelocity = velocity - other.Velocity;
				Vector2 voAverage = (velocity + other.Velocity) * 0.5f;

				vos.Add(
					new VO(voBoundingOrigin, voAverage, totalRadius, relativeVelocity, inverseAgentTimeHorizon, 1)
					);
				//vos[voCount] = new VO(voBoundingOrigin, voAverage, totalRadius, relativeVelocity, inverseAgentTimeHorizon, 1);
				//voCount++;
			}

			Vector2 result = Vector2.zero;
			float best = float.PositiveInfinity;
			float qualityCutoff = 0.05f;
			float cutoff = velocity.magnitude * qualityCutoff;
			result = Trace(vos, new Vector2(desiredVelocity.x, desiredVelocity.y), cutoff, out best);

			Vector2 p = Velocity;
			float score;
			Vector2 res = Trace(vos, p, cutoff, out score);

			if (score < best)
			{
				result = res;
			}

			newVelocity = result.normalized;
		}

		public static float DesiredVelocityWeight = 0.02f;
		public static float DesiredVelocityScale = 0.1f;
		//public static float DesiredSpeedScale = 0.0f;
		public static float GlobalIncompressibility = 10;



		/** Traces the vector field constructed out of the velocity obstacles.
			* Returns the position which gives the minimum score (approximately).
			*/
		//Trace(vos, voCount, new Vector2(desiredVelocity.x, desiredVelocity.z), cutoff, out best );
		Vector2 Trace(List<VO> vos, Vector2 p, float cutoff, out float score)
		{

			float stepScale = 1.5f;

			float bestScore = float.PositiveInfinity;
			Vector2 bestP = p;
			for (int s = 0; s < 10; s++)
			{
				float step = 1.0f - (s / 10f);
				step *= stepScale;

				Vector2 dir = Vector2.zero;
				float mx = 0;
				for (int i = 0; i < vos.Count; i++)
				{
					float w;
					Vector2 d = vos[i].Sample(p, out w);
					//if (_id == 1)
					//{
					//	Debug.Log("~~~~~~~~~~~~~~" + d);
					//}
					dir += d;

					if (w > mx) mx = w;
					//mx = System.Math.Max (mx, d.sqrMagnitude);
				}

				Vector2 bonusForDesiredVelocity = desiredVelocity - p;

				float weight = bonusForDesiredVelocity.magnitude * DesiredVelocityWeight;
				dir += bonusForDesiredVelocity * DesiredVelocityScale;
				mx = System.Math.Max(mx, weight);

				score = mx;

				if (score < bestScore)
				{
					bestScore = score;
				}

				bestP = p;
				if (score <= cutoff && s > 10) break;

				float sq = dir.sqrMagnitude;
				if (sq > 0) dir *= mx / Mathf.Sqrt(sq);

				dir *= step;
				p += dir;
			}

			if (_id == 1)
			{
				Debug.Log("#############" + bestP);
			}
			score = bestScore;
			return bestP;
		}
	}
}

