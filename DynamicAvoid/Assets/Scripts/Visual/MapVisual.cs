using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


namespace Pathfinding.Visual
{
    public class MapVisual : MonoBehaviour
    {
        public Transform gridBgParent;
        public Transform textBgParent;
        public Transform arrowBgParent;
        public Transform unitContainer;
        public Font font;

        public Sprite unitSprite;
        FlowDir.Map map;
        private bool moving = false;
        GridVisual[,] gridsVisual = null;
        List<UnitVisual> unitsVisual = new List<UnitVisual>();
        public Text TargetText;


        private bool interpolation = true;
        public bool Interpolation { get { return interpolation; } set { interpolation = value; } }
        private float deltaTime;
        public float DeltaTime { get { return deltaTime; } }
        private float desiredDeltaTime = 0.05f;
        public float DesiredDeltaTime
        {
            get { return desiredDeltaTime; }
            set { desiredDeltaTime = System.Math.Max(value, 0.0f); }
        }


        private void Start()
        {
            map = new FlowDir.Map(40, 30);
            map.Init();

            createVisual();
        }


        void Update()
        {
            if (Input.GetKeyDown("h"))
            {
                textBgParent.gameObject.SetActive(!textBgParent.gameObject.activeSelf);
                foreach (var g in gridsVisual)
                {
                    g.switchText(textBgParent.gameObject.activeSelf);
                }
                
            }
            if (Input.GetKeyDown("d"))
            {
                arrowBgParent.gameObject.SetActive(!arrowBgParent.gameObject.activeSelf);
                foreach (var g in gridsVisual)
                {
                    g.switchArrow(arrowBgParent.gameObject.activeSelf);
                }
            }

            if (Input.GetKeyDown("s"))
            {
                moving = !moving;
                if (moving)
                {
                    map.Interpolation = interpolation;
                    map.DesiredDeltaTime = DesiredDeltaTime;
                    map.DeltaTime = DeltaTime;
                }
            }

            if (moving)
            {
                map.Update();
            }
        }


        private void FixedUpdate()
        {
            if (!moving)
                return;
            foreach (var unit in map.UnitMgr.Units)
            {
                unit.Move(Time.fixedDeltaTime);
            }

            fixedUpdateVisual();
        }


        void createVisual()
        {
            gridsVisual = new GridVisual[map.Row, map.Column];

            var allGrids = map.Grids;
            for (int r = 0; r < map.Row; r++)
            {
                for (int c = 0; c < map.Column; c++)
                {
                    var g = allGrids[r, c];
                    var v = new GridVisual();
                    v.Parent = gridBgParent;
                    v.Grid = g;
                    v.font = font;
                    v.mapVisual = this;
                    v.Create();
                    gridsVisual[r, c] = v;
                }
            }

            foreach(var u in map.UnitMgr.Units)
            {
                var v = new UnitVisual();
                v.Parent = unitContainer;
                v.Unit = u;
                
                v.Create();
                if(unitSprite != null)
                {
                    v.SetSprite(unitSprite);
                }
                unitsVisual.Add(v);
            }
        }


        private void fixedUpdateVisual()
        {
            for (int r = 0; r < map.Row; r++)
            {
                for (int c = 0; c < map.Column; c++)
                {
                    gridsVisual[r, c].FixedUpdate();
                }
            }

            foreach (var v in unitsVisual)
            {
                v.FixedUpdate();
            }

            TargetText.text = textMapTarget;
        }


        string textMapTarget
        {
            get
            {
                return string.Format("{0}, {1}", map.Target.x, map.Target.y);
            }
        }


        public void RefreshTarget()
        {
            for (int r = 0; r < map.Row; r++)
            {
                for (int c = 0; c < map.Column; c++)
                {
                    gridsVisual[r, c].UpdateDebug();
                }
            }
        }


    }

}
