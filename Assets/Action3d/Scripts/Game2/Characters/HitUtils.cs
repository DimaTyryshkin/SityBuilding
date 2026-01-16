using GamePackages.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public struct HitInfo
    {
        public Vector3 point;
        public Vector3 normal;
        public Collider collider;

        public IDamageable GetDamageable()
        {
            if (!collider)
                return null;

            return collider.GetComponentInParent<IDamageable>();
        }
    }

    public static class HitUtils
    {
        static RaycastHit[] raycastHit;

        public static HitInfo RayCast(Vector3 point, Vector3 dir, LayerMask layerMask, float distance = Bullet.MaxDistance)
        {
#if UNITY_EDITOR
            Assert.IsTrue(Mathf.Approximately(dir.magnitude, 1f));
#endif

            if (raycastHit == null)
                raycastHit = new RaycastHit[100];

            HitInfo hit = new HitInfo();
            int count = Physics.RaycastNonAlloc(point, dir, raycastHit, distance, layerMask);
            int index = raycastHit.MinIndex(r => r.distance, count);

            if (index == -1)
            {
                hit.normal = Vector3.up;
                hit.point = point + dir * distance;
            }
            else
            {
                hit.point = raycastHit[index].point;
                hit.normal = raycastHit[index].normal;
                hit.collider = raycastHit[index].collider;

                if (hit.collider is BoxCollider b)
                    hit.point = b.ClosestPoint(hit.point);
                else if (hit.collider is CapsuleCollider c)
                    hit.point = c.ClosestPoint(hit.point);
                else if (hit.collider is SphereCollider s)
                    hit.point = s.ClosestPoint(hit.point);
                else if (hit.collider is MeshCollider m)
                {
                    if (m.convex)
                        hit.point = m.ClosestPoint(hit.point);
                }
            }

            return hit;
        }
    }
}