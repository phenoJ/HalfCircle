using System.Collections.Generic;
using Pathfinding.Boids;
using UnityEngine;



namespace Pathfinding.FlowDir
{
    public class Unit : ObjBase
    {
        private static int s_idGen = 1;
        private int _id;
        public int id { get { return _id; } }

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

        public Vector2 Pos2
        {
            get
            {
                return new Vector2(PosX, PosY);
            }
        }
        private RVO.Agent _agent;
        public RVO.Agent agent { get { return _agent; } }


        public Boid boid { get; set; }
        private Map _map;
        private UnitManager _mgr;
        public Unit(Map map, UnitManager mgr)
        {
            _id = s_idGen++;
            _map = map;
            boid = new Boid(
                    new Vector3(Random.Range(10, 50), 0, Random.Range(10, 50))
                );
            _mgr = mgr;
            _agent = new RVO.Agent(_id, map, Pos2);
        }

        public void Move(float deltaTime)
        {
            updateVec();
            //boid.flockForce(_mgr.Boids);
            //displace(deltaTime);
        }


        void updateVec()
        {
            var c = Mathf.Clamp(
                Mathf.FloorToInt(boid.Loc.x / Constans.GRID_WIDTH), 0, _map.Column - 1);
            var r = Mathf.Clamp(
                Mathf.FloorToInt(boid.Loc.z / Constans.GRID_HEIGHT), 0, _map.Row - 1);
            var d = _map.GetFlowDir(r, c);
            _agent.DesiredVelocity = _agent.Velocity == Vector2.zero ?
                d : d * _agent.Velocity;

            //boid.Vel = new Vector3(d.x, 0, d.y);
        }

        public void SynAgentPos()
        {
            boid.Loc = new Vector3(_agent.InterpolatedPosition.x, 0, _agent.InterpolatedPosition.y);
            //_img.rectTransform.anchoredPosition = _agent.InterpolatedPosition;
        }


        void displace(float deltaTime = 0.0f)
        {
            boid.Update(deltaTime);
        }
    }
}
