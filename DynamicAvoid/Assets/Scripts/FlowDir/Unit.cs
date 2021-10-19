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

            var posX = 0;
            var posY = 0;

            if(group != 2)
            {
                posX = Random.Range(10, 50) + (group - 1) * 200;
                posY = Random.Range(10, 50) + (group - 1) * 200;
            }
            else
            {
                posX = 25 * (id % 10 ) + (group - 1) * 200;
                posY = 25 * (int)(id / 10) + (group - 1) * 200;
            }
            

            boid = new Boid(
                    new Vector3(posX, 0, posY), this
                );
            _mgr = mgr;
            _agent = new RVO.Agent(this, map, Pos2);
        }

        /// <summary>
        /// 当前计算的运动速度
        /// </summary>
        Vector2 curVelocity;

        void applyBoid(float deltaTime)
        {
            var posOld = Pos2;

            boid.Vel = new Vector3(curVelocity.x, 0, curVelocity.y);
            
            boid.flockForce(_mgr.GetBoidsByGroup(Group));
            boid.Update(deltaTime);
            var posNew = Pos2;
            boid.SetPos2(posOld);

            curVelocity = (posNew - posOld) / deltaTime;
        }

        public void Move(float deltaTime)
        {

            if (targetOffset.magnitude <= 3)
            {
                return;
            }

            curVelocity = Vector2.zero;

            var c = Mathf.Clamp(
                Mathf.FloorToInt(boid.Loc.x / Constans.GRID_WIDTH), 0, _map.Column - 1);
            var r = Mathf.Clamp(
                Mathf.FloorToInt(boid.Loc.z / Constans.GRID_HEIGHT), 0, _map.Row - 1);
            var d = _map.GetFlowDir(r, c);
            curVelocity = _agent.Velocity == Vector2.zero ?
                d : d * _agent.Velocity;

            updateTargetVelocity();

            if(targetOffset.magnitude > 30)
            {
                applyBoid(deltaTime);
            }

            _agent.DesiredVelocity = curVelocity;
        }

        public void SynAgentPos()
        {
            if (targetOffset.magnitude <= 3)
            {
                //boid.Loc = new Vector3(_agent.InterpolatedPosition.x, 0, _agent.InterpolatedPosition.y);
                return;
            }

            boid.Loc = new Vector3(_agent.InterpolatedPosition.x, 0, _agent.InterpolatedPosition.y);

        }


        Vector2 _offsetCenter = Vector2.zero;

        public void UpdateCenterPos(Vector2 center)
        {
            _offsetCenter = Pos2 - center;
        }


        Vector2 _targetPos = Vector2.zero;
        float _startDistance = 0f;
        public void UpdateTargetPos(Vector2 center)
        {
            //TODO: 考虑碰撞偏移
            _targetPos = center + _offsetCenter;
            _startDistance = targetOffset.magnitude;

            _agent.StartMove(Pos2);
        }


        void updateTargetVelocity()
        {
            if(_targetPos == Vector2.zero)
            {
                return;
            }

            var d = targetOffset.magnitude;
            if(_startDistance > 0)
            {
                if(d < _startDistance)
                {
                    var r = d / _startDistance;
                    var n = -targetOffset.normalized;
                    curVelocity = (n * (1 - r) + curVelocity.normalized * r) * curVelocity.magnitude;
                }
                
            }
            else
            {
                curVelocity = Vector2.zero;
            }
        }


        Vector2 targetOffset
        {
            get
            {
                return Pos2 - _targetPos;
            }
        }
    }
}
