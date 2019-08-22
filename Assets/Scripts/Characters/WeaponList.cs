using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Weapons
public class Weapon_AssaultRifle : BaseWeapon.BaseWeaponModifier
{
    public override void SetTarget(BaseWeapon target)
    {
        target.Name = "Assault Rifle";
        target.DamagePerShot = 40;
        target.ShootSpeed = 50;
        target.FireRate = 0.5f;
    }
}

public class Weapon_ShotGun : BaseWeapon.BaseWeaponModifier
{
    public override void SetTarget(BaseWeapon target)
    {
        target.Name = "Shotgun";
        target.DamagePerShot = 10;
        target.AmmoPerShot = 3;
        target.BulletPerShot = 3;
        target.ShootSpeed = 50;
        target.FireRate = 0.5f;
    }
}
public class WeaponE_AssaultRifle : BaseWeapon.BaseWeaponModifier
{
    public override void SetTarget(BaseWeapon target)
    {
        target.Name = "Assault Rifle";
        target.DamagePerShot = 10;
        target.ShootSpeed = 10;
        target.FireRate = 2;
        // Wait 2s before ennemy start attacking
        target.DelayWeaponSwitch = 2;
    }
}
