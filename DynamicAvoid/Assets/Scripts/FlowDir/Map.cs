using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pathfinding.FlowDir
{
    public class Map : ObjBase
    {
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

        public void Init()
        {
            createUnits();
            createMap();
        }

        void createUnits()
        {
            for (var i = 0; i < 10; i++)
            {
                unitMgr.CreateUnit(this);
            }
        }


        void createMap()
        {
            allGrids = new MapGrid[Row, Column];

            for(int r = 0; r < Row; r++)
            {
                for (int c = 0; c < Column; c++)
                {
                    allGrids[r, c] = new MapGrid(r, c, this);
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
                    dir.y = -dir.y;
                    centerGrid.dir = dir;
                }
            }
        }
    }
}
