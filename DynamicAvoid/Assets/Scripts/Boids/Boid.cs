using UnityEngine;
using System.Collections.Generic;


namespace Pathfinding.Boids
{
    public class Boid
    {

        static int MASS_MIN = 1;
        static int MASS_MAX = 5;
        static int AVOID_RADIUS = 1;
        static int APPROACH_RADIUS = 3;
        static int ALLIGN_RADIUS = 1;
        static int MAX_FORCE = 1;

        public void SetPos2(Vector2 pos)
        {
            loc = new Vector3(pos.x, 0, pos.y);
        }

        protected Vector3 loc;
        public Vector3 Loc { get { return loc; } set { loc = value; } }
        protected Vector3 vel;
        public Vector3 Vel { get { return vel; } set { vel = value; } }
        protected Vector3 acc;

        FlowDir.Unit unit;
        public FlowDir.Unit Unit { get { return unit; } }

        protected int mass;
        protected int maxForce = MAX_FORCE;
        protected float speed = 50f;

        public Boid(Vector3 loc, FlowDir.Unit u)
        {
            unit = u;
            this.loc = loc;
            vel = Vector3.zero;
            acc = Vector3.zero;
            mass = 1; // Random.Range(MASS_MIN, MASS_MAX);
        }

        public virtual void Update(float deltaTime)
        {
            vel += acc * deltaTime * speed;
            loc += vel * deltaTime * speed;
            acc = Vector3.zero;
            vel = limitVec(vel, speed);

            resetPosition();
        }

        public void flockForce(List<Boid> boids)
        {
            avoidForce(boids);
            approachForce(boids);
            alignForce(boids);
        }


        public virtual void avoidForce(List<Boid> boids)
        {
            //Applies a force to the boid that makes 
            appAlignForce(boids, AVOID_RADIUS, 2.5f);
        }


        public void applyForce(Vector3 force)
        {
            force /= mass;
            acc += force;
        }


        public virtual void approachForce(List<Boid> boids)
        {
            appAlignForce(boids, APPROACH_RADIUS);
        }

        public virtual void alignForce(List<Boid> boids)
        {
            //Keep track of how many boids are in sight.
            //To store vels of boids in sight.
            //Algorhithm analogous to approach- and avoidForce.
            appAlignForce(boids, ALLIGN_RADIUS, 1.0f, false);
        }

        public virtual void replaceForce(Vector3 obstacle, float radius)
        {
            //Force that drives boid away from obstacle.

            var futPos = loc + vel; //Calculate future position for more effective behavior.
            var dist = obstacle - futPos;
            float d = dist.magnitude;


            if (d <= radius)
            {
                var repelVec = loc - obstacle;
                repelVec.Normalize();
                if (d != 0)
                { //Don't divide by zero.
                    float scale = 1.0f / d; //The closer to the obstacle, the stronger the force.
                    repelVec.Normalize();
                    repelVec *= (maxForce * 7);
                    if (repelVec.magnitude < 0)
                    { //Don't let the boids turn around to avoid the obstacle.
                        repelVec.y = 0;
                    }
                }
                applyForce(repelVec);
            }
        }


        protected void appAlignForce(List<Boid> boids, int radius, float forceRate=1f, bool isLoc=true)
        {
            float count = 0;
            var sum = Vector3.zero;

            foreach (Boid other in boids)
            {

                int approachRadius = mass + radius;
                var dist = other.Loc - loc;
                float d = dist.magnitude;

                if (d != 0 && d < approachRadius)
                {
                    sum += isLoc ? other.Loc : other.Vel;
                    count++;
                }
            }
            if (count > 0)
            {
                sum /= count;
                var vec = isLoc ? (sum - loc) : sum;
                vec = limitVec(vec, maxForce*forceRate);
                applyForce(vec);
            }
        }


        protected Vector3 limitVec(Vector3 vec, float l)
        {
            if (vec.magnitude <= l)
            {
                return vec;
            }

            vec.Normalize();
            vec *= l;
            return vec;
        }


        protected void resetPosition()
        {
            //Make canvas doughnut-shaped.
            /*
            if (loc.x <= 0)
            {
                loc.x = width;
            }
            if (loc.x > width)
            {
                loc.x = 0;
            }
            if (loc.y <= 0)
            {
                loc.y = height;
            }
            if (loc.y > height)
            {
                loc.y = 0;
            }
            */
        }
    }
}
