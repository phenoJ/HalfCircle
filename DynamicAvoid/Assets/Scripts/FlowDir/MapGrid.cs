using UnityEngine;


namespace Pathfinding.FlowDir
{

    public class MapGrid : ObjBase
    {
        public Map Map{get; private set;}

        public int r;
        public int c;
        public int X { get { return r; } }
        public int Y { get { return c; } }


        private float _posX;
        private float _posY;
        public float posX { get { return _posX; } }
        public float posY { get { return _posY; } }

        private int _idx = 0;
        public int idx { get { return _idx; } }

        private float _cost = 0;
        public float cost { get { return _cost; } set { _cost = value; } }

        private Vector2 _dir = Vector2.zero;
        public Vector2 dir { get { return _dir; } set { _dir = value; } }


        public MapGrid(int r, int c, Map map)
        {
            Map = map;
            this.r = r;
            this.c = c;
            _idx = r * map.Row + c;
            _posX = c * Constans.GRID_WIDTH;
            _posY = r * Constans.GRID_HEIGHT;
        }
    }

}
