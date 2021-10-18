using Pathfinding.Boids;
using System.Collections.Generic;


namespace Pathfinding.FlowDir
{

    public class UnitManager
    {
        Dictionary<int, List<Boid>> boids = new Dictionary<int, List<Boid>>();

        public List<Boid> Boids = new List<Boid>();
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
            Boids.Clear();
            boids.Clear();
        }

    }

}
