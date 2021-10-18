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

        int _group = 1;
        public int Group { get { return _group; } }

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
        public Unit(Map map, UnitManager mgr, int group)
        {
            _group = group;
            _id = s_idGen++;
            _map = map;

            var posX = Random.Range(10, 50) + (group - 1) * 200;
            var posY = Random.Range(10, 50) + (group - 1) * 200;

            boid = new Boid(
                    new Vector3(posX, 0, posY)
                );
            _mgr = mgr;
            _agent = new RVO.Agent(this, map, Pos2);
        }

        public void Move(float deltaTime)
        {

            var c = Mathf.Clamp(
                Mathf.FloorToInt(boid.Loc.x / Constans.GRID_WIDTH), 0, _map.Column - 1);
            var r = Mathf.Clamp(
                Mathf.FloorToInt(boid.Loc.z / Constans.GRID_HEIGHT), 0, _map.Row - 1);
            var d = _map.GetFlowDir(r, c);

            var velocity = _agent.Velocity == Vector2.zero ?
                d : d * _agent.Velocity;

            boid.Vel = new Vector3(velocity.x, 0, velocity.y);
            var posOld = Pos2;

            boid.flockForce(_mgr.GetBoidsByGroup(Group));
            boid.Update(deltaTime);
            
            var posNew = Pos2;
            velocity = (posNew - posOld) / deltaTime;

            _agent.DesiredVelocity = velocity;
        }



        public void SynAgentPos()
        {
            boid.Loc = new Vector3(_agent.InterpolatedPosition.x, 0, _agent.InterpolatedPosition.y);
        }
    }
}
