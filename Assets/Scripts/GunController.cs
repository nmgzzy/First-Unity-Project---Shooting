using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    Gun equipedGun;
    public Gun startingGun;
    void Start() {
        if (startingGun != null) {
            EquipGun(startingGun);
        }

    }
    public void EquipGun(Gun gunToEquip) 
    {
        if (equipedGun != null) {
            Destroy(equipedGun.gameObject);
        }
        equipedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equipedGun.transform.parent = weaponHold;
        
    }
    public void Shoot()
    {
        if (equipedGun != null) {
            equipedGun.Shoot();
        }
    }
}
