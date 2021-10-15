using System.Collections.Generic;
using Pathfinding.Boids;
using UnityEngine;



namespace Pathfinding.FlowDir
{
    public class Unit : ObjBase
    {
        public float PosX
        {
            get
            {
                return boid.Loc.x;
            }
        }
        public float PosY
        {
            get
            {
                return boid.Loc.z;
            }
        }


        public Boid boid { get; set; }
        private Map _map;
        private UnitManager _mgr;
        public Unit(Map map, UnitManager mgr)
        {
            _map = map;
            boid = new Boid(
                    new Vector3(Random.Range(10, 50), 0, Random.Range(10, 50))
                );
            _mgr = mgr;
        }

        public void Move(float deltaTime)
        {
            updateVec();
            boid.flockForce(_mgr.Boids);
            displace(deltaTime);
        }


        void updateVec()
        {
            var c = Mathf.Clamp(
                Mathf.FloorToInt(boid.Loc.x / Constans.GRID_WIDTH), 0, _map.Column - 1);
            var r = Mathf.Clamp(
                Mathf.FloorToInt(boid.Loc.z / Constans.GRID_HEIGHT), 0, _map.Row - 1);
            var d = _map.GetFlowDir(r, c);
            boid.Vel = new Vector3(d.x, 0, d.y);
        }


        void displace(float deltaTime = 0.0f)
        {
            boid.Update(deltaTime);
        }
    }
}
