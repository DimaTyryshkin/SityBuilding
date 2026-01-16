using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
    //public class PlayerBuildingInput : PlayerInput
    //{
    //    public void SetBuiuldingPrefab(Building.BrushBuilder buildingPrefab)
    //    {
    //    }
    //}

    public class PlayerGunInput : PlayerInput
    {
        [SerializeField, IsntNull] Gun gun;

        protected override void UpdteInternal()
        {
            if (Input.GetKeyDown(KeyCode.R))
                gun.ReloadInput();

            //if (Input.GetMouseButton(0))
            //    gun.ShotInput();
        }
    }
}