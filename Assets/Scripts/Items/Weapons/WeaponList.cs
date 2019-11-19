using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// List of weapons
public class Weapon_AssaultRifle : BaseWeapon.BaseWeaponModifier
{
    public override void SetTarget(BaseWeapon target)
    {
        target.Name = "Assault Rifle";
        target.DamagePerShot = 50;
        target.ShootSpeed = 50;
        target.FireRate = 0.5f;
    }
}
public class Weapon_GOD : BaseWeapon.BaseWeaponModifier
{
    public override void SetTarget(BaseWeapon target)
    {
        target.Name = "GOD";
        target.DamagePerShot = 100000;
        target.ShootSpeed = 50;
        target.FireRate = 0.1f;
        target.BulletLifeTime = 2;
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
        target.Name = "Assault Rifle E";
        target.DamagePerShot = 10;
        target.ShootSpeed = 10;
        target.FireRate = 1.5f;
        // Wait 2s before ennemy start attacking
        target.DelayWeaponSwitch = 0.5f;
    }
}
public class WeaponB_AssaultRifle : BaseWeapon.BaseWeaponModifier
{
    public override void SetTarget(BaseWeapon target)
    {
        target.Name = "Assault Rifle B";
        target.DamagePerShot = 10;
        target.ShootSpeed = 25;
        target.FireRate = 1;
        // Wait 2s before ennemy start attacking
        target.DelayWeaponSwitch = 2;
    }
}

