using Pathfinding.Boids;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.FlowDir
{

    public class UnitManager
    {
        Dictionary<int, List<Boid>> boids = new Dictionary<int, List<Boid>>();
        public List<Unit> Units = new List<Unit>();

        public List<Boid> GetBoidsByGroup(int group)
        {
            if (!boids.ContainsKey(group))
            {
                return new List<Boid>();
            }

            return boids[group];
        }

        public void CreateUnit(Map map, int group=1)
        {
            if(!boids.ContainsKey(group))
            {
                boids[group] = new List<Boid>();
            }

            var u = new Unit(map, this, group);
            this.Units.Add(u);
            boids[group].Add(u.boid);
        }

        public void Clear()
        {
            Units.Clear();
            boids.Clear();
        }


        public void SetBoidsTarget(Vector2 pos)
        {
            foreach (var g in boids.Keys)
            {
                foreach (var b in boids[g])
                {
                    b.Unit.UpdateTargetPos(pos);
                }
            }
        }

        public void UpdateBoidsGroupCenter()
        {
            foreach(var g in boids.Keys)
            {
                var pos = GetGroupCenter(g);

                foreach(var b in boids[g])
                {
                    b.Unit.UpdateCenterPos(new Vector2(pos.x, pos.z));
                }
            }
        }


        public Vector3 GetGroupCenter(int group)
        {
            var pos = Vector3.zero;
            pos = Vector3.zero;
            var bList = boids[group];
            foreach (var b in bList)
            {
                pos += b.Loc;
            }

            if (bList.Count > 0)
            {
                pos /= bList.Count;
            }

            return pos;
        }
    }

}
