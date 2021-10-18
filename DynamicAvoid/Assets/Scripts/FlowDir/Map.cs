using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pathfinding.FlowDir
{
    public class Map : ObjBase
    {
        public RVO.Agent.VO[] vos = new RVO.Agent.VO[20];
        private float lastStep = -99999;
        private float lastStepInterpolationReference = -9999;
        private float prevDeltaTime = 0;
        private float deltaTime;
        public float DeltaTime { get { return deltaTime; } set { deltaTime = value; } }
        RVO.RVOQuadtree quadtree = new RVO.RVOQuadtree();
        public RVO.RVOQuadtree Quadtree { get { return quadtree; } }
        private float desiredDeltaTime = 0.05f;
        public float DesiredDeltaTime { get { return desiredDeltaTime; } set { desiredDeltaTime = System.Math.Max(value, 0.0f); } }

        private bool interpolation = true;
        public bool Interpolation { get { return interpolation; } set { interpolation = value; } }


        public void UpdateTarget(Vector2 tar)
        {
            Target = tar;
            unitMgr.SetBoidsTarget(new Vector2(Constans.GRID_WIDTH * (Target.y), Constans.GRID_HEIGHT * (Target.x)));
        }

        public Vector2 Target { get; set; }
        public int Row { get; private set; }
        public int Column { get; private set; }

        public MapGrid[,] Grids
        {
            get
            {
                return allGrids;
            }
        }

        private MapGrid[,] allGrids = null;
        private UnitManager unitMgr = new UnitManager();
        public UnitManager UnitMgr
        {
            get
            {
                return unitMgr;
            }
        }


        public Map(int r, int c)
        {
            Row = r;
            Column = c;
        }


        public void Update()
        {
            //Initialize last step
            if (lastStep < 0)
            {
                lastStep = Time.time;
                deltaTime = DesiredDeltaTime;
                prevDeltaTime = deltaTime;
                lastStepInterpolationReference = lastStep;
            }

            if (Time.time - lastStep >= DesiredDeltaTime)
            {
                foreach(var u in unitMgr.Units)
                {
                    u.agent.Interpolate((Time.time - lastStepInterpolationReference) / DeltaTime);
                }

                lastStepInterpolationReference = Time.time;

                prevDeltaTime = DeltaTime;
                deltaTime = Time.time - lastStep;
                lastStep = Time.time;

                // Implements averaging of delta times
                // Disabled for now because it seems to have caused more issues than it solved
                // Might re-enable later
                /*frameTimeBufferIndex++;
				frameTimeBufferIndex %= frameTimeBuffer.Length;
				frameTimeBuffer[frameTimeBufferIndex] = deltaTime;
				
				float sum = 0;
				float mn = float.PositiveInfinity;
				float mx = float.NegativeInfinity;
				for (int i=0;i<frameTimeBuffer.Length;i++) {
					sum += frameTimeBuffer[i];
					mn = Mathf.Min (mn, frameTimeBuffer[i]);
					mx = Mathf.Max (mx, frameTimeBuffer[i]);
				}
				sum -= mn;
				sum -= mx;
				sum /= (frameTimeBuffer.Length-2);
				sum = frame
				deltaTime = sum;*/

                //Calculate smooth delta time
                //Disabled because it seemed to cause more problems than it solved
                //deltaTime = (Time.time - frameTimeBuffer[(frameTimeBufferIndex-1+frameTimeBuffer.Length)%frameTimeBuffer.Length]) / frameTimeBuffer.Length;

                //Prevent a zero delta time
                deltaTime = System.Math.Max(deltaTime, 1.0f / 2000f);

                // Time reference for the interpolation
                // If delta time would not be subtracted, the character would have a zero velocity
                // during all frames when the velocity was recalculated


                BuildQuadtree();

                foreach (var u in unitMgr.Units)
                {
                    u.agent.Update(deltaTime);
                }

                foreach (var u in unitMgr.Units)
                {
                    u.agent.CalculateNeighbours();
                    u.agent.CalculateVelocity();
                }
            }

            if (Interpolation)
            {

                foreach (var u in unitMgr.Units)
                {
                    u.agent.Interpolate((Time.time - lastStepInterpolationReference) / DeltaTime);
                    u.SynAgentPos();
                }
            }
        }

        public void Init()
        {
            createUnits();
            createMap();
        }


        void BuildQuadtree()
        {
            quadtree.Clear();
            if (unitMgr.Units.Count > 0)
            {
                var agent0 = unitMgr.Units[0].agent;
                Rect bounds = Rect.MinMaxRect(agent0.position.x, agent0.position.y, agent0.position.x, agent0.position.y);
                for (int i = 1; i < unitMgr.Units.Count; i++)
                {
                    Vector3 p = unitMgr.Units[i].agent.position;
                    bounds = Rect.MinMaxRect(Mathf.Min(bounds.xMin, p.x), Mathf.Min(bounds.yMin, p.z), Mathf.Max(bounds.xMax, p.x), Mathf.Max(bounds.yMax, p.z));
                }
                quadtree.SetBounds(bounds);

                for (int i = 1; i < unitMgr.Units.Count; i++)
                {
                    quadtree.Insert(unitMgr.Units[i].agent);
                }

                //quadtree.DebugDraw ();
            }
        }


        void createUnits()
        {
            for (var i = 0; i < 10; i++)
            {
                unitMgr.CreateUnit(this, i < 5 ? 1 : 2);
            }

            unitMgr.UpdateBoidsGroupCenter();
        }


        void createMap()
        {
            allGrids = new MapGrid[Row, Column];

            HashSet<int> block = new HashSet<int>();
            for (int i = 0; i < 5; i++)
            {
                block.Add(20 + (i + 5) * Constans.MAX_COL_CNT);
            }

            for (int r = 0; r < Row; r++)
            {
                for (int c = 0; c < Column; c++)
                {
                    int i = r * Constans.MAX_COL_CNT + c;
                    allGrids[r, c] = new MapGrid(r, c, this, block.Contains(i));
                }
            }
        }

        public Vector2 GetFlowDir(int r, int c)
        {
            if (r < 0 || c < 0 || r >= Row || c >= Column)
                return Vector2.zero;
            return allGrids[r, c].dir;
        }
        ArrayList getNeighbors(int r, int c, bool serachDiagonal = false)
        {
            ArrayList neighbors = new ArrayList();
            if (r < Row - 1)  //t
            {
                neighbors.Add(allGrids[r + 1, c]);
            }
            else
            {
                neighbors.Add(null);
            }

            if (r > 0) // b
            {
                neighbors.Add(allGrids[r - 1, c]);
            }
            else
            {
                neighbors.Add(null);
            }

            if (c > 0) // l
            {
                neighbors.Add(allGrids[r, c - 1]);
            }
            else
            {
                neighbors.Add(null);
            }

            if (c < Column - 1) //r
            {
                neighbors.Add(allGrids[r, c + 1]);
            }
            else
            {
                neighbors.Add(null);
            }

            if (serachDiagonal)
            {
                if (r < Row- 1 && c < Column - 1) // rt
                {
                    neighbors.Add(allGrids[r + 1, c + 1]);
                }
                if (r > 0 && c < Column - 1) // rb
                {
                    neighbors.Add(allGrids[r - 1, c + 1]);
                }
                if (r > 0 && c > 0) // lb
                {
                    neighbors.Add(allGrids[r - 1, c - 1]);
                }
                if (r < Row - 1 && c > 0) //lt 
                {
                    neighbors.Add(allGrids[r + 1, c - 1]);
                }
            }
            return neighbors;
        }

        public void CalculateHeatField(int tr, int tc)
        {
            int idx = tr * Column + tc;

            for (int r = 0; r < Row; r++)
            {
                for (int c = 0; c < Column; c++)
                {
                    allGrids[r, c].cost = (r == tr && c == tc) ? 0 : int.MaxValue;
                }
            }

            List<int> openList = new List<int>();
            openList.Add(idx);

            while (openList.Count > 0)
            {
                int centerIdx = openList[0];
                openList.RemoveAt(0);

                int currentC = centerIdx % Column;
                int currentR = centerIdx / Column;

                var centerGrid = allGrids[currentR, currentC];
                var neighbors = getNeighbors(currentR, currentC);

                foreach (MapGrid neighbor in neighbors)
                {
                    if (neighbor == null)
                    {
                        continue;
                    }
                    float cost = centerGrid.cost + 1;

                    if (cost < neighbor.cost)
                    {

                        if (!openList.Contains(neighbor.idx))
                        {
                            openList.Add(neighbor.idx);
                        }

                        neighbor.cost = cost;
                    }
                }
            }
            drawHeatField(tr, tc);
        }

        void drawHeatField(int tr, int tc)
        {

            for (int r = 0; r < Row; r++)
            {
                for (int c = 0; c < Column; c++)
                {
                    MapGrid centerGrid = allGrids[r, c];
                    if (tr == r && tc == c)
                        continue;

                    var neighbors = getNeighbors(r, c);
                    var dir = Vector2.zero;
                    MapGrid left = (MapGrid)neighbors[(int)Constans.DirType.Left];
                    MapGrid right = (MapGrid)neighbors[(int)Constans.DirType.Right];
                    MapGrid top = (MapGrid)neighbors[(int)Constans.DirType.Top];
                    MapGrid bottom = (MapGrid)neighbors[(int)Constans.DirType.Bottom];
                    dir.x = (left != null ? left.cost : centerGrid.cost) - (right != null ? right.cost : centerGrid.cost);
                    dir.y = (top != null ? top.cost : centerGrid.cost) - (bottom != null ? bottom.cost : centerGrid.cost);
                    dir = dir.normalized;
                    /*
                    var dx = dir.x;
                    var dy = dir.y;
                    dir.x = - dy;
                    dir.y = dx;
                    */
                    dir.y = -dir.y;
                    //dir.x = -dir.x;
                    centerGrid.dir = dir;
                }
            }
        }
    }
}
