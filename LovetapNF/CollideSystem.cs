using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LovetapNF
{
    public class CollideSystem
    {
        public static List<WatchedCollider> colliders = new List<WatchedCollider>();
        static ColliderData[] colliderSnapshot;

        public static ColliderData getColliderByName(string name)
        {
            if (colliderSnapshot == null)
                return null;
            for (int i=0; i < colliderSnapshot.Length; i++)
            {
                if (colliderSnapshot[i].name == name)
                    return colliderSnapshot[i];
            }
            return null;
        }
        public static void updateColliders()
        {
            colliderSnapshot = ColliderCon.getColliders();
            foreach (var col in colliders)
            {
                var col1 = getColliderByName(col.first);
                var col2 = getColliderByName(col.second);
                
                if (col1 == null | col2 == null)
                    continue;
                col.tripped = false;
                col.rfirst = col1;
                col.rsecond = col2;
                if (Vector3.Distance(col1.position,col2.position) < (col1.radius + col2.radius))
                {
                    col.tripped = true;
                }
            }
        }
    }
    public class WatchedCollider
    {
        public string first;
        public string second;
        public ColliderData rfirst;
        public ColliderData rsecond;
        public bool tripped;
        public float dist;
        
        public WatchedCollider(ColliderData data, ColliderData data2)
        {
            first = data.name;
            second = data2.name;
            rfirst = data;
            rsecond = data2;
        }
    }
}
