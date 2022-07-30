using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun equipedGun;
    public Gun[] gunList;
    int gunIndex = 0;
    void Awake() {
        if (gunList.GetLength(0) > 0) {
            EquipGun(gunList[0]);
        }
    }

    public void EquipLastGun()
    {
        gunIndex = gunIndex - 1;
        if (gunIndex < 0) {
            gunIndex = gunList.GetLength(0) - 1;
        }
        EquipGun(gunList[gunIndex]); 
    }

    public void EquipNextGun()
    {
        gunIndex = (gunIndex + 1) % gunList.GetLength(0);
        EquipGun(gunList[gunIndex]); 
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
