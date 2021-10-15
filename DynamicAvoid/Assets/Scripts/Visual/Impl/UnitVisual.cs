using UnityEngine;
using UnityEngine.UI;


namespace Pathfinding.Visual
{
    public class UnitVisual : VisualBase
    {

        public FlowDir.Unit Unit { get; set; }

        Image img;

        public override void Create()
        {
            base.Create();

            Obj = new GameObject();
            Obj.transform.SetParent(Parent);

            img = Obj.AddComponent<Image>();
            img.rectTransform.sizeDelta = new Vector2(5, 5);
            img.rectTransform.anchorMin = new Vector2(0, 0);
            img.rectTransform.anchorMax = new Vector2(0, 0);
            img.rectTransform.pivot = new Vector2(0, 0);
            img.rectTransform.localScale = Vector2.one;
            img.raycastTarget = false;

            updateVisualPos();
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            updateVisualPos();
        }


        void updateVisualPos()
        {
            img.rectTransform.anchoredPosition = new Vector2(Unit.PosX, Unit.PosY);
        }
    }
}
