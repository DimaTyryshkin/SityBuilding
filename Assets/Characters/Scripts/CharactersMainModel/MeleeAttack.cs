using System.Collections;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
    public class MeleeAttack : MonoBehaviour
    {
        [SerializeField]
        bool isPlayer;

        [SerializeField]
        bool debug;

        [SerializeField, IsntNull]
        GameObject debugMarker;

        void Start()
        {
            debugMarker.SetActive(false);
        }

        public void Attack(Vector3 position, Vector3 dirForward, float maxDistance, CharacterHp selfHp)
        {
            // var m1 = Instantiate(debugMarker, position, Quaternion.identity);
            // m1.name = "m1";
            // var m2 = Instantiate(debugMarker, position + dirForward, Quaternion.identity);
            // m2.name = "m2";

            RaycastHit[] result = Physics.SphereCastAll(position, 1, dirForward, maxDistance);


            // string log = result.ToStringMultiline("Attack", x =>
            // {
            //     var t = x.collider.gameObject.transform;
            //     if (t.parent)
            //         return t.parent.name + "+";
            //     else
            //         return t.name;
            // });
            // Debug.Log(log);

            CharacterHp target = null;
            Vector3 hitPoint = Vector3.zero;
            float minDistance = maxDistance * 2;

            foreach (RaycastHit hit in result)
            {
                var hp = hit.collider.GetComponentInParent<CharacterHp>();
                if (hp && hp.IsPlayer != isPlayer)
                {
                    // Debug.Log($"1 minDistance={minDistance} hit.distance={hit.distance}");
                    if (hit.distance < minDistance)
                    {
                        // Debug.Log($"1");
                        hitPoint = hit.point;
                        minDistance = hit.distance;
                        target = hp;
                    }
                }
            }

            if (target)
            {
                target.SetDamage();

                if (debug)
                {
                    StopAllCoroutines();
                    StartCoroutine(ShowMarker(target.transform.position));
                }
            }
        }

        IEnumerator ShowMarker(Vector3 point)
        {
            debugMarker.transform.position = point;

            debugMarker.SetActive(true);
            yield return new WaitForSeconds(2);
            debugMarker.SetActive(false);
        }
    }
}