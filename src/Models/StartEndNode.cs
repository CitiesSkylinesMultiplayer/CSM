using UnityEngine;

namespace CSM.Models
{
    public struct StartEndNode
    {
        public Vector3 startnode;

        public Vector3 EndNode;

        public StartEndNode(Vector3 startnode, Vector3 endnode)
        {
            this.startnode = startnode;
            this.EndNode = endnode;
        }
    }
}