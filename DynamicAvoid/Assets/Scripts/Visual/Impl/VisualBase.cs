using UnityEngine;
using UnityEngine.UI;

namespace Pathfinding.Visual
{
    public class VisualBase
    {
        public Transform Parent { get; set; }
        public GameObject Obj { get; protected set; }

        public virtual void FixedUpdate()
        {

        }

        public virtual void Create()
        {

        }

    }
}
