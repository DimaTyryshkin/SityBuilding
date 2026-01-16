using GamePackages.Audio;
using GamePackages.Core;
using UnityEngine;

namespace Game
{
    public class Bullet : MonoBehaviour
    {
        public const float MaxDistance = 200;
        [SerializeField] float speed = 150;
        [SerializeField] float gravityFactor = 10;
        [SerializeField] GameObject hitViewPrefab;
        [SerializeField] TrailRenderer trailRenderer;
        [SerializeField] LayerMask layerMask;

        [Header("Rebound")]
        [SerializeField] SoundsSet reboundSoud;
        [SerializeField] float reboundAngle = 10;
        [SerializeField] float reboundDirFluctuation = 0.1f;
        [SerializeField] RangeMinMax reboundSpeedFactor = new RangeMinMax(0.3f, 0.7f);


        float stopTime;
        bool skipFrame;
        Vector3 velocity;

        public void SetDir(Vector3 dir)
        {
            velocity = dir.normalized * speed;
            skipFrame = false;
            stopTime = Time.time + MaxDistance / speed;
        }

        void Update()
        {
            if (!skipFrame)
            {
                skipFrame = true;
                //  return;
            }

            float actualSpeed = velocity.magnitude;
            float distance = Time.deltaTime * actualSpeed;
            HitInfo hit = HitUtils.RayCast(transform.position, velocity.normalized, layerMask, distance);

            transform.position = Vector3.MoveTowards(transform.position, hit.point, distance);

            if (hit.collider)
            {
                float angle = Vector3.Angle(hit.normal, velocity);
                float delta = angle - 90;
                float p = (reboundAngle - delta) / reboundAngle;

                if (p > Random.Range(0f, 1f))
                {
                    velocity = Vector3.Reflect(velocity, hit.normal);
                    velocity += Random.onUnitSphere * actualSpeed * reboundDirFluctuation;

                    if (actualSpeed > speed * 0.5f)
                    {
                        SoundSetPlayer player = AppSounds.GetSoundPlayer(reboundSoud);
                        player.SetWorldPosition(hit.point);
                        player.Play();
                    }

                    velocity *= reboundSpeedFactor.Random();
                }
                else
                {
                    bool canDamage = actualSpeed / speed > 0.5f;
                    if (canDamage)
                    {
                        GameObject hitView = Instantiate(hitViewPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                        hitView.transform.SetParent(hit.collider.transform, true);
                        hitView.SetActive(true);

                        var damageable = hit.GetDamageable();
                        if (damageable != null)
                        {
                            damageable.ApplyDamage(new Damage()
                            {
                                damage = 1,
                                damageDir = velocity.normalized,
                                worldPoint = hit.point
                            });
                        }
                    }

                    enabled = false;
                    Destroy(gameObject, 5);
                }
            }
            else
            {
                velocity += Vector3.down * Time.deltaTime * gravityFactor;

                if (Time.time > stopTime)
                {
                    Destroy(gameObject, 5);
                    enabled = false;
                }
            }
        }
    }
}