using UnityEngine;

namespace Game2.Building
{
    public class MetallMiningCampBuilding : BuildingBase
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Transform spawnPoint;
        [SerializeField] float f;

        float timeNextSpawn;

        public override void Delinking()
        {

        }

        private void FixedUpdate()
        {
            if (Time.time > timeNextSpawn)
            {
                timeNextSpawn = Time.time + 2;
                GameObject go = Instantiate(prefab, spawnPoint.position, Random.rotation);
                go.SetActive(true);
                go.GetComponent<Rigidbody>().AddForce((spawnPoint.forward + Random.onUnitSphere * 0.3f) * f, ForceMode.Acceleration);
                Physics.IgnoreCollision(go.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
            }
        }
    }
}