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
        private static Color[] s_colors = new Color[] {
            new Color(190f / 255f, 190f/ 255f, 190f/ 255f, 1),
            new Color(100f / 255f, 100f/ 255f, 100f/ 255f, 1),
            new Color(255f / 255f, 0f/ 255f, 0f/ 255f, 1)
        };

        private void Start()
        {
            map = new FlowDir.Map(40, 50);
            map.Init();

            map.Interpolation = interpolation;
            map.DesiredDeltaTime = DesiredDeltaTime;
            map.DeltaTime = DeltaTime;
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

            map.Update();
        }


        private void FixedUpdate()
        {
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

            
        }

        public void UpdateTargetText()
        {
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

            UpdateTargetText();
        }


    }

}
