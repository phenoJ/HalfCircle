using UnityEngine;
using System.Collections.Generic;


namespace Pathfinding.Boids
{
    public class Predator : Boid
    {
        public Predator(Vector3 loc):base(loc)
        {

        }


        public override void approachForce(List<Boid> boids)
        { 
            //Same as for boid, but with bigger approachRadius.
            appAlignForce(boids, 260);
        }
    }
}
