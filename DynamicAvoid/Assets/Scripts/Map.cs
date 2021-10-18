using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Constans
{
    public const int MAX_ROW_CNT = 50;
    public const int MAX_COL_CNT = 50;
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
    public  Vector2 dir { 
        get { return _dir; } 
        set { 
            _dir = value.normalized;
            if (_dir == Vector2.zero)
            {
                arrowRectTrans.gameObject.SetActive(false);
                return;
            }
            arrowRectTrans.gameObject.SetActive(true);

            if (_dir.y == 0 || _dir.x == 0)
            {
                if (_dir.y == 0)
                {
                    arrowRectTrans.eulerAngles = new Vector3(0, 0, _dir.x > 0 ? 0 : 180);
                }
                else if (_dir.x == 0)
                {
                    arrowRectTrans.eulerAngles = new Vector3(0, 0, _dir.y < 0 ? -90 : 90);
                }
            }
            else
            {
                float rotate = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
                arrowRectTrans.eulerAngles = new Vector3(0, 0, rotate);
            }
        } }

    public Image img { private set; get; }
    public Text text { private set; get; }

    public RectTransform arrowRectTrans { private set; get; }

    private bool _block = false;
    public bool IsBlock { get { return _block; } }

    public MapGrid(int r, int c, Image img, Text text, RectTransform arrowRectTrans, bool isBlock = false)
    {
        this.r = r;
        this.c = c;
        this.img = img;
        this.text = text;
        this.arrowRectTrans = arrowRectTrans;
        _idx = r * Constans.MAX_ROW_CNT + c;
        _posX = c * Constans.GRID_WIDTH * 0.5f;
        _posY = r * Constans.GRID_HEIGHT * 0.5f;
        _block = isBlock;
    }
}

public class Unit
{
    private Image _img;
    private Map _map;

    private Agent _agent;
    public Agent agent { get { return _agent; } }

    private int _id;
    public int id { get { return _id; } }

    private static int s_idGen = 1;
    public Unit(Map map, Transform parent)
    {
        var obj = new GameObject();
        obj.transform.SetParent(parent);
        var img = obj.AddComponent<Image>();
        img.sprite = map.unitSpr;
        img.rectTransform.sizeDelta = new Vector2(10, 10);
        img.rectTransform.anchorMin = Vector2.zero;
        img.rectTransform.anchorMax = Vector2.zero;
        img.rectTransform.pivot = Vector2.zero;
        img.rectTransform.localScale = Vector2.one;
        img.raycastTarget = false;
        img.color = Color.red;
        //img.rectTransform.anchoredPosition = new Vector2(Random.Range(10,15), Random.Range(10, 15));
        _id = s_idGen++;
        if (_id == 1)
        {
            img.color = Color.blue;
            img.rectTransform.anchoredPosition = new Vector2(10, 10);
        }
        else
        {
            //img.rectTransform.anchoredPosition = new Vector2(11, 11);
            img.rectTransform.anchoredPosition = new Vector2(Random.Range(10, 15), Random.Range(10, 15));
        }
        _img = img;
        _map = map;
        _agent = new Agent(_id, map, img.rectTransform.anchoredPosition);
    }

    public void Move(float deltaTime)
    {
        var pos = _img.rectTransform.anchoredPosition;
        var c = Mathf.Clamp(Mathf.FloorToInt(pos.x / Constans.GRID_WIDTH), 0, Constans.MAX_COL_CNT - 1); 
        var r = Mathf.Clamp(Mathf.FloorToInt(pos.y / Constans.GRID_HEIGHT), 0, Constans.MAX_COL_CNT - 1);
        _agent.DesiredVelocity = _agent.Velocity == Vector2.zero ? _map.GetFlowDir(r, c) : _map.GetFlowDir(r,c) * _agent.Velocity;
    }

    public void SynAgentPos()
    {
        _img.rectTransform.anchoredPosition = _agent.InterpolatedPosition;
    }
}

public class Map : MonoBehaviour
{
    private MapGrid[,] allGrids = new MapGrid[50, 50];
    private ArrayList allUnits = new ArrayList();
    
    List<Agent> agents = new List<Agent>();

    public Transform gridBgParent;
    public Transform textBgParent;
    public Transform arrowBgParent;
    public Transform unitContainer;
    public Font font;
    public Sprite unitSpr;
    public Agent.VO[] vos = new Agent.VO[20];

    private bool moving = true;
    RVOQuadtree quadtree = new RVOQuadtree();
    private float deltaTime;
    public float DeltaTime { get { return deltaTime; } }

    private float prevDeltaTime = 0;

    private float lastStep = -99999;
    private float lastStepInterpolationReference = -9999;
    public RVOQuadtree Quadtree { get { return quadtree; } }
    private float desiredDeltaTime = 0.05f;
    public float DesiredDeltaTime { get { return desiredDeltaTime; } set { desiredDeltaTime = System.Math.Max(value, 0.0f); } }
    
    private bool interpolation = true;
    public bool Interpolation { get { return interpolation; } set { interpolation = value; } }

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
        if (moving)
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
                for (int i = 0; i < agents.Count; i++)
                {
                    agents[i].Interpolate((Time.time - lastStepInterpolationReference) / DeltaTime);
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

                for (int i = 0; i < agents.Count; i++)
                {
                    agents[i].Update(deltaTime);
                }


                for (int i = 0; i < agents.Count; i++)
                {
                    agents[i].CalculateNeighbours();
                    agents[i].CalculateVelocity();
                }
            }

            if (Interpolation)
            {
                for (int i = 0; i < agents.Count; i++)
                {
                    agents[i].Interpolate((Time.time - lastStepInterpolationReference) / DeltaTime);
                    ((Unit)allUnits[i]).SynAgentPos();
                }
            }
        }
    }

    void BuildQuadtree()
    {
        quadtree.Clear();
        if (agents.Count > 0)
        {
            Rect bounds = Rect.MinMaxRect(agents[0].position.x, agents[0].position.y, agents[0].position.x, agents[0].position.y);
            for (int i = 1; i < agents.Count; i++)
            {
                Vector3 p = agents[i].position;
                bounds = Rect.MinMaxRect(Mathf.Min(bounds.xMin, p.x), Mathf.Min(bounds.yMin, p.z), Mathf.Max(bounds.xMax, p.x), Mathf.Max(bounds.yMax, p.z));
            }
            quadtree.SetBounds(bounds);

            for (int i = 0; i < agents.Count; i++)
            {
                quadtree.Insert(agents[i]);
            }

            //quadtree.DebugDraw ();
        }
    }

    private void FixedUpdate()
    {
        if (!moving)
            return;
        foreach(Unit unit in allUnits)
        {
            unit.Move(Time.fixedDeltaTime);
        }
    }

    void CreateUnits()
    {
        for(var i =0; i <50; i++)
        {
            var unit = new Unit(this, unitContainer);
            allUnits.Add(unit);
            agents.Add(unit.agent);
        }
    }
    void CreateMap()
    {
        float x = 0f;
        float y = 0f;

        Color[] colors = new Color[] {
            new Color(190f / 255f, 190f/ 255f, 190f/ 255f, 1),
            new Color(100f / 255f, 100f/ 255f, 100f/ 255f, 1),
            new Color(255f / 255f, 0f/ 255f, 0f/ 255f, 1)
        };

        HashSet<int> block = new HashSet<int>();
        for (int i = 0; i < 5; i++)
        {
            block.Add(20 + (i + 5) * Constans.MAX_COL_CNT);
        }

        for(int r = 0; r < Constans.MAX_ROW_CNT; r++)
        {
            for(int c = 0; c < Constans.MAX_COL_CNT; c++)
            {
                int i = r * Constans.MAX_COL_CNT + c;
                var obj = new GameObject();
                obj.transform.SetParent(gridBgParent);
                var isBlock = block.Contains(i);
                var img = obj.AddComponent<Image>();
                img.rectTransform.sizeDelta = new Vector2(Constans.GRID_WIDTH, Constans.GRID_HEIGHT);
                img.rectTransform.anchorMin = new Vector2(0, 0);
                img.rectTransform.anchorMax = new Vector2(0, 0);
                img.rectTransform.pivot = Vector2.zero;
                img.rectTransform.localScale = Vector2.one;
                img.raycastTarget = true;
                img.rectTransform.anchoredPosition = new Vector2(x, y);
                
                if (!isBlock)
                {
                    var trigger = obj.AddComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerClick;
                    entry.callback.AddListener((eventData) => {
                        //this.OnPointerClick(eventData) 
                        calculateHeatField(i / Constans.MAX_COL_CNT, i % Constans.MAX_COL_CNT);
                    });
                    trigger.triggers.Add(entry);
                }

                if (isBlock)
                {
                    img.color = colors[2];
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        img.color = colors[0];
                    }
                    else
                    {
                        img.color = colors[1];
                    }
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
                arrowText.text = "=>";
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

                allGrids[r, c] = new MapGrid(r,c, img, text, arrowText.rectTransform, isBlock);
                
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
        if (r < Constans.MAX_ROW_CNT - 1/* && !allGrids[r + 1, c].IsBlock*/)  //t
        {
            neighbors.Add(allGrids[r + 1, c]);
        }
        else
        {
            neighbors.Add(null);
        }

        if (r > 0 && !allGrids[r - 1, c].IsBlock) // b
        {
            neighbors.Add(allGrids[r - 1, c]);
        }
        else
        {
            neighbors.Add(null);
        }

        if (c > 0 && !allGrids[r, c - 1].IsBlock) // l
        {
            neighbors.Add(allGrids[r, c - 1]);
        }
        else
        {
            neighbors.Add(null);
        }

        if (c < Constans.MAX_COL_CNT - 1 && !allGrids[r, c + 1].IsBlock) //r
        {
            neighbors.Add(allGrids[r, c + 1]);
        }
        else
        {
            neighbors.Add(null);
        }

        if (serachDiagonal)
        {
            if (r < Constans.MAX_ROW_CNT - 1 && c < Constans.MAX_COL_CNT - 1 && !allGrids[r + 1, c + 1].IsBlock) // rt
            {
                neighbors.Add(allGrids[r + 1, c + 1]);
            }
            if (r > 0 && c < Constans.MAX_COL_CNT - 1 && !allGrids[r - 1, c + 1].IsBlock) // rb
            {
                neighbors.Add(allGrids[r - 1, c + 1]);
            }
            if (r > 0 && c > 0 && !allGrids[r - 1, c - 1].IsBlock) // lb
            {
                neighbors.Add(allGrids[r - 1, c - 1]);
            }
            if (r < Constans.MAX_ROW_CNT - 1 && c > 0 && !allGrids[r + 1, c - 1].IsBlock) //lt 
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
                allGrids[r, c].cost = (r == tr && c == tc) ? 0 : 9999;
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
                if (neighbor == null || neighbor.IsBlock)
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
                if ((tr == r && tc == c) || centerGrid.IsBlock)
                {
                    centerGrid.dir = Vector2.zero;
                    continue;
                }

                var neighbors = getNeighbors(r, c, true);
                var dir = Vector2.zero;

                MapGrid smallNeighbor = null;
                foreach (MapGrid neighbor in neighbors)
                {
                    if (neighbor == null)
                        continue;
                    if (smallNeighbor == null)
                    {
                        smallNeighbor = neighbor;
                        continue;
                    }
                    if (smallNeighbor.cost >= neighbor.cost)
                    {
                        if (neighbor.cost < smallNeighbor.cost)
                        {
                            smallNeighbor = neighbor;
                            continue;
                        }
                        var disSmall = Mathf.Abs(smallNeighbor.r - centerGrid.r) + Mathf.Abs(smallNeighbor.c - centerGrid.c);
                        var disNei = Mathf.Abs(neighbor.r - centerGrid.r) + Mathf.Abs(neighbor.c - centerGrid.c);
                        smallNeighbor = disSmall < disNei ? smallNeighbor : neighbor;
                    }
                }
                //MapGrid left = (MapGrid)neighbors[(int)Constans.DirType.Left];
                //MapGrid right = (MapGrid)neighbors[(int)Constans.DirType.Right];
                //MapGrid top = (MapGrid)neighbors[(int)Constans.DirType.Top];
                //MapGrid bottom = (MapGrid)neighbors[(int)Constans.DirType.Bottom];
                //dir.x = (left != null ? left.cost : centerGrid.cost) - (right != null ? right.cost : centerGrid.cost); 
                //dir.y = (top != null ? top.cost : centerGrid.cost) - (bottom != null ? bottom.cost : centerGrid.cost);
                //dir = dir.normalized;
                //dir.y = -dir.y;
                dir.x = smallNeighbor.c - centerGrid.c;
                dir.y = smallNeighbor.r - centerGrid.r;
                dir = dir.normalized;
                centerGrid.dir = dir;
                
            }
        }
    }
}
