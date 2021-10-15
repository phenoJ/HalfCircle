using System.Collections.Generic;
using Pathfinding.Boids;
using UnityEngine;


namespace Pathfinding.FlowDir
{

    public class UnitManager
    {
        public List<Boid> Boids = new List<Boid>();
        public List<Unit> Units = new List<Unit>();


        public void CreateUnit(Map map)
        {
            var u = new Unit(map, this);
            this.Units.Add(u);
            Boids.Add(u.boid);
        }

        public void Clear()
        {
            Units.Clear();
            Boids.Clear();
        }

    }

}
