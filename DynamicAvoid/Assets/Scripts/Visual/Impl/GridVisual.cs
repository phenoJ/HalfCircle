using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Pathfinding.Visual
{ 
    public class GridVisual : VisualBase
    {
        public MapVisual mapVisual { get; set; }
        public Font font { get; set; }

        static Color[] colors = new Color[] {
            new Color(190f / 255f, 190f/ 255f, 190f/ 255f, 1),
            new Color(100f / 255f, 100f/ 255f, 100f/ 255f, 1)
        };

        public FlowDir.MapGrid Grid { get; set; }

        Image img = null;

        public override void Create()
        {
            base.Create();

            Obj = new GameObject();
            Obj.transform.SetParent(Parent);
            Obj.name = Name;
            
            img = Obj.AddComponent<Image>();
            img.rectTransform.sizeDelta = new Vector2(Constans.GRID_WIDTH, Constans.GRID_HEIGHT);
            img.rectTransform.anchorMin = new Vector2(0, 0);
            img.rectTransform.anchorMax = new Vector2(0, 0);
            img.rectTransform.pivot = Vector2.zero;
            img.rectTransform.localScale = Vector2.one;
            img.raycastTarget = true;
            img.rectTransform.anchoredPosition = new Vector2(Grid.posX, Grid.posY);

            var trigger = Obj.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;

            entry.callback.AddListener((eventData) => {
                //this.OnPointerClick(eventData) 
                Grid.Map.CalculateHeatField(Grid.X, Grid.Y);
                Grid.Map.Target = new Vector2(Grid.X, Grid.Y);
                mapVisual.RefreshTarget();
            });
            trigger.triggers.Add(entry);


            int i = Grid.X + Grid.Y;
            if (Grid.IsBlock)
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
            
            createText();
            createArrow();
        }


        public void TextActive(bool active)
        {
            textObj.SetActive(active);
        }


        public void ArrowActive(bool active)
        {
            arrowTextObj.SetActive(active);
        }

        GameObject textObj;
        void createText()
        {
            textObj = new GameObject();
            textObj.transform.SetParent(Obj.transform);
            textObj.name = "text";
            var text = textObj.AddComponent<Text>();
            text.text = "0";
            text.raycastTarget = false;
            text.rectTransform.sizeDelta = new Vector2(Constans.GRID_WIDTH, Constans.GRID_HEIGHT);
            text.rectTransform.anchorMin = new Vector2(0, 0);
            text.rectTransform.anchorMax = new Vector2(0, 0);
            text.rectTransform.pivot = Vector2.zero;
            text.rectTransform.localScale = Vector2.one;
            text.raycastTarget = false;
            text.font = font;
            text.alignment = TextAnchor.MiddleCenter;
            text.rectTransform.anchoredPosition = new Vector2(0, 0);
            text.resizeTextForBestFit = true;

            textObj.SetActive(false);
        }


        void udpateText()
        {
            if (textObj.activeSelf)
            {
                textObj.GetComponent<Text>().text = string.Format("{0}", Grid.cost);
            }
        }

        GameObject arrowTextObj;
        Text arrowText;
        void createArrow()
        {
            arrowTextObj = new GameObject();
            arrowTextObj.transform.SetParent(Obj.transform);
            arrowTextObj.name = "arrow";
            arrowText = arrowTextObj.AddComponent<Text>();
            arrowText.text = "=>";
            arrowText.raycastTarget = false;
            arrowText.rectTransform.sizeDelta = new Vector2(Constans.GRID_WIDTH, Constans.GRID_HEIGHT);
            arrowText.rectTransform.anchorMin = new Vector2(0, 0);
            arrowText.rectTransform.anchorMax = new Vector2(0, 0);
            arrowText.rectTransform.localScale = Vector2.one;
            arrowText.raycastTarget = false;
            arrowText.rectTransform.anchoredPosition = new Vector2(Constans.GRID_WIDTH * 0.5f, Constans.GRID_HEIGHT * 0.5f);
            arrowText.font = font;
            arrowText.alignment = TextAnchor.MiddleCenter;
            arrowText.resizeTextForBestFit = true;

            arrowTextObj.SetActive(false);
        }


        void udpateArrow()
        {
            if (arrowTextObj.activeSelf)
            {
                var dir = Grid.dir;
                float rotate = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                arrowText.rectTransform.eulerAngles = new Vector3(0, 0, rotate);
            }
        }


        string Name
        {
            get
            {
                return string.Format("G_{0}_{1}", Grid.X, Grid.Y);
            }
        }


        public void switchArrow(bool enable)
        {
            arrowTextObj.SetActive(enable);
        }

        public void switchText(bool enable)
        {
            textObj.SetActive(enable);
        }

        public void UpdateDebug()
        {
            udpateArrow();
            udpateText();
        }
    }
}
