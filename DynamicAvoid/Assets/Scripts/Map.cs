using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pathfinding.Boids;

public class Constans
{
    public const int MAX_ROW_CNT = 40;
    public const int MAX_COL_CNT = 40;
    public const int GRID_WIDTH = 20;
    public const int GRID_HEIGHT = 20;

    public enum DirType
    {
        Top,
        Bottom,
        Left,
        Right
    }
}

public class MapGrid
{
    public int r;
    public int c;

    private float _posX;
    private float _posY;
    public float posX { get { return _posX; } }
    public float posY { get { return _posY; } }

    private int _idx = 0;
    public int idx { get { return _idx; } }

    private float _cost = 0;
    public float cost { get { return _cost; } set { _cost = value; text.text = string.Format("{0}", value); } }

    private Vector2 _dir = Vector2.zero;
    public  Vector2 dir { get { return _dir; } set { _dir = value; } }

    public Image img { private set; get; }
    public Text text { private set; get; }

    public RectTransform arrowRectTrans { private set; get; }

    public MapGrid(int r, int c, Image img, Text text, RectTransform arrowRectTrans)
    {
        this.r = r;
        this.c = c;
        this.img = img;
        this.text = text;
        this.arrowRectTrans = arrowRectTrans;
        _idx = r * Constans.MAX_ROW_CNT + c;
        _posX = c * Constans.GRID_WIDTH * 0.5f;
        _posY = r * Constans.GRID_HEIGHT * 0.5f;
    }
}


public class UnitManager
{
    public List<Boid> Boids = new List<Boid>();
    public List<Unit> Units = new List<Unit>();


    public void CreateUnit(Map map, Transform unitContainer)
    {
        var u = new Unit(map, unitContainer, this);
        this.Units.Add(u);
        Boids.Add(u.boid);
    }

    public void Clear()
    {
        Units.Clear();
        Boids.Clear();
    }

}

public class Unit
{
    public Boid boid { get; set; }

    private Image _img;
    private Map _map;
    private UnitManager _mgr;
    public Unit(Map map, Transform parent, UnitManager mgr)
    {

        var obj = new GameObject();
        obj.transform.SetParent(parent);
        var img = obj.AddComponent<Image>();
        img.rectTransform.sizeDelta = new Vector2(5, 5);
        img.rectTransform.anchorMin = new Vector2(0, 0);
        img.rectTransform.anchorMax = new Vector2(0, 0);
        img.rectTransform.pivot = new Vector2(0, 0);
        img.rectTransform.localScale = Vector2.one;
        img.raycastTarget = false;
        boid = new Boid(
                new Vector3(Random.Range(10, 50), 0, Random.Range(10, 50))
            );
        _img = img;
        _map = map;
        _mgr = mgr;
        displace();
    }

    public void Move(float deltaTime)
    {
        updateVec();
        boid.flockForce(_mgr.Boids);
        displace(deltaTime);
    }


    void updateVec()
    {
        var c = Mathf.Clamp(Mathf.FloorToInt(boid.Loc.x / Constans.GRID_WIDTH), 0, Constans.MAX_COL_CNT - 1);
        var r = Mathf.Clamp(Mathf.FloorToInt(boid.Loc.z / Constans.GRID_HEIGHT), 0, Constans.MAX_COL_CNT - 1);
        var d = _map.GetFlowDir(r, c);
        boid.Vel = new Vector3(d.x, 0, d.y);
    }


    void displace(float deltaTime=0.0f)
    {
        boid.Update(deltaTime);
        _img.rectTransform.anchoredPosition = new Vector2(boid.Loc.x, boid.Loc.z);
    }
}

public class Map : MonoBehaviour
{
    public Vector2 Target { get; set; }

    private MapGrid[,] allGrids = new MapGrid[50, 50];
    private UnitManager unitMgr = new UnitManager();

    public Transform gridBgParent;
    public Transform textBgParent;
    public Transform arrowBgParent;
    public Transform unitContainer;
    public Font font;

    private bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        CreateMap();
        CreateUnits();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            textBgParent.gameObject.SetActive(!textBgParent.gameObject.activeSelf);
        }
        if (Input.GetKeyDown("d"))
        {
            arrowBgParent.gameObject.SetActive(!arrowBgParent.gameObject.activeSelf);
        }

        if (Input.GetKeyDown("s"))
        {
            moving = !moving;
        }
    }

    private void FixedUpdate()
    {
        if (!moving)
            return;
        foreach(Unit unit in unitMgr.Units)
        {
            unit.Move(Time.fixedDeltaTime);
        }
    }

    void CreateUnits()
    {
        for(var i =0; i < 10; i++)
        {
            unitMgr.CreateUnit(this, unitContainer);
        }
    }
    void CreateMap()
    {
        float x = 0f;
        float y = 0f;

        Color[] colors = new Color[] {
            new Color(190f / 255f, 190f/ 255f, 190f/ 255f, 1),
            new Color(100f / 255f, 100f/ 255f, 100f/ 255f, 1)
        };
        for(int r = 0; r < Constans.MAX_ROW_CNT; r++)
        {
            for(int c = 0; c < Constans.MAX_COL_CNT; c++)
            {
                int i = r * Constans.MAX_COL_CNT + c;
                var obj = new GameObject();
                obj.transform.SetParent(gridBgParent);
                var img = obj.AddComponent<Image>();
                img.rectTransform.sizeDelta = new Vector2(Constans.GRID_WIDTH, Constans.GRID_HEIGHT);
                img.rectTransform.anchorMin = new Vector2(0, 0);
                img.rectTransform.anchorMax = new Vector2(0, 0);
                img.rectTransform.pivot = Vector2.zero;
                img.rectTransform.localScale = Vector2.one;
                img.raycastTarget = true;
                img.rectTransform.anchoredPosition = new Vector2(x, y);
                var trigger = obj.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                int curX = r;
                int curY = c;

                entry.callback.AddListener((eventData) => {
                    //this.OnPointerClick(eventData) 
                    calculateHeatField(i / Constans.MAX_COL_CNT, i % Constans.MAX_COL_CNT);
                    Target = new Vector2(curX, curY);
                });
                trigger.triggers.Add(entry);

                if (i % 2 == 0)
                {
                    img.color = colors[0];
                }
                else
                {
                    img.color = colors[1];
                }

                var textObj = new GameObject();
                textObj.transform.SetParent(textBgParent);
                var text = textObj.AddComponent<Text>();
                text.text = "0";
                text.raycastTarget = false;
                text.rectTransform.sizeDelta = new Vector2(Constans.GRID_WIDTH, Constans.GRID_HEIGHT);
                text.rectTransform.anchorMin = new Vector2(0, 0);
                text.rectTransform.anchorMax = new Vector2(0, 0);
                text.rectTransform.pivot = Vector2.zero;
                text.rectTransform.localScale = Vector2.one;
                text.raycastTarget = false;
                text.rectTransform.anchoredPosition = new Vector2(x, y);
                text.font = font;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;


                var arrowObj = new GameObject();
                arrowObj.transform.SetParent(arrowBgParent);
                var arrowText = arrowObj.AddComponent<Text>();
                arrowText.text = "===>";
                arrowText.raycastTarget = false;
                arrowText.rectTransform.sizeDelta = new Vector2(Constans.GRID_WIDTH, Constans.GRID_HEIGHT);
                arrowText.rectTransform.anchorMin = new Vector2(0, 0);
                arrowText.rectTransform.anchorMax = new Vector2(0, 0);
                arrowText.rectTransform.localScale = Vector2.one;
                arrowText.raycastTarget = false;
                arrowText.rectTransform.anchoredPosition = new Vector2(x + Constans.GRID_WIDTH * 0.5f, y + Constans.GRID_HEIGHT * 0.5f);
                arrowText.font = font;
                arrowText.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;

                allGrids[r, c] = new MapGrid(r,c, img, text, arrowText.rectTransform);
                
                x += Constans.GRID_WIDTH;
            }
            var tmp = colors[0];
            colors[0] = colors[1];
            colors[1] = tmp;
            x = 0;
            y += Constans.GRID_HEIGHT;
        }
    }
    
    public Vector2 GetFlowDir(int r, int c)
    {
        if (r < 0 || c < 0 || r >= Constans.MAX_ROW_CNT || c >= Constans.MAX_COL_CNT)
            return Vector2.zero;
        return allGrids[r, c].dir;
    }
    ArrayList getNeighbors(int r, int c, bool serachDiagonal = false)
    {
        ArrayList neighbors = new ArrayList();
        if (r < Constans.MAX_ROW_CNT - 1)  //t
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

        if (c < Constans.MAX_COL_CNT - 1) //r
        {
            neighbors.Add(allGrids[r, c + 1]);
        }
        else
        {
            neighbors.Add(null);
        }

        if (serachDiagonal)
        {
            if (r < Constans.MAX_ROW_CNT - 1 && c < Constans.MAX_COL_CNT - 1) // rt
            {
                neighbors.Add(allGrids[r + 1, c + 1]);
            }
            if (r > 0 && c < Constans.MAX_COL_CNT - 1) // rb
            {
                neighbors.Add(allGrids[r - 1, c + 1]);
            }
            if (r > 0 && c > 0) // lb
            {
                neighbors.Add(allGrids[r - 1, c - 1]);
            }
            if (r < Constans.MAX_ROW_CNT - 1 && c > 0) //lt 
            {
                neighbors.Add(allGrids[r + 1, c - 1]);
            }
        }
        return neighbors;
    }

    void calculateHeatField(int tr, int tc)
    {
        Debug.LogFormat("{0},{1}", tr, tc);
        int idx = tr * Constans.MAX_COL_CNT + tc;

        for (int r = 0; r < Constans.MAX_ROW_CNT; r++)
        {
            for (int c = 0; c < Constans.MAX_COL_CNT; c++)
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

            int currentC = centerIdx % Constans.MAX_COL_CNT;
            int currentR = centerIdx / Constans.MAX_COL_CNT;

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
        for (int r = 0; r < Constans.MAX_ROW_CNT; r++)
        {
            for (int c = 0; c < Constans.MAX_COL_CNT; c++)
            {
                MapGrid centerGrid = allGrids[r, c];
                
                centerGrid.arrowRectTrans.gameObject.SetActive((tr != r || tc != c));
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
                if (dir.y == 0 || dir.x == 0)
                {
                    if (dir.y == 0)
                    {
                        centerGrid.arrowRectTrans.eulerAngles = new Vector3(0, 0, dir.x > 0 ? 0 : 180);
                    }
                    else if(dir.x == 0)
                    {
                        centerGrid.arrowRectTrans.eulerAngles = new Vector3(0, 0, dir.y < 0 ? -90 : 90);
                    }
                }
                else
                {
                    float rotate = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    centerGrid.arrowRectTrans.eulerAngles = new Vector3(0, 0, rotate);
                }
            }
        }
    }
}
