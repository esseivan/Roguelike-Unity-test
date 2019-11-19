using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    public const float SHOOT_SPEED_CONST_MOD = 100;

    public string Name = "Unknown";
    public int AmmoPerShot = 1;
    public int BulletPerShot = 1;
    public BulletType CompatibleTypes = (BulletType)0b11111111;
    public int DamagePerShot = 30;
    public int MaxAmmoPerMagazine = 30;
    public int MaxTotalAmmo = 100;
    public float ShootSpeed = 10;
    public float ReloadTime = 1;
    public float BulletLifeTime = 2;
    public float FireRate = 0.5f;
    public float DelayWeaponSwitch = 1;

    public BaseWeapon Scale(float factor)
    {
        return ScaleFrom(this, factor);
    }

    public BaseWeapon ScaleFrom(BaseWeapon source, float factor)
    {
        BaseWeapon scaled = this;
        ScaleFrom(ref scaled, source, factor);
        return this;
    }

    public BaseWeapon Clone()
    {
        return new BaseWeapon
        {
            Name = Name,
            AmmoPerShot = AmmoPerShot,
            BulletLifeTime = BulletLifeTime,
            CompatibleTypes = CompatibleTypes,
            DamagePerShot = DamagePerShot,
            MaxAmmoPerMagazine = MaxAmmoPerMagazine,
            MaxTotalAmmo = MaxTotalAmmo,
            ShootSpeed = ShootSpeed,
            BulletPerShot = BulletPerShot,
            ReloadTime = ReloadTime,
            FireRate = FireRate
        };
    }

    public static void ScaleFrom(ref BaseWeapon scaled, BaseWeapon source, float factor)
    {
        if (factor == 0)
            throw new System.ArgumentOutOfRangeException(nameof(factor) + " is : " + factor);

        float beginnerScaler = 0.25f, easyScaler = 0.5f, mediumScaler = 1, hardScaler = 2;
        float bulletSpeedScaler;
        float damagePerShotScaler;
        float fireRateScaler;
        int ammoPerShotScaler;

        // Don't edit : Name, CompatibleTypes, MaxAmmoPerMagazine, MaxTotalmmo
        //scaled.Name = source.Name;
        //scaled.CompatibleTypes = source.CompatibleTypes;
        //scaled.MaxAmmoPerMagazine = source.MaxAmmoPerMagazine;
        //scaled.MaxTotalAmmo = source.MaxTotalAmmo;

        // Scalers are :
        /* Beginner:    0.25
         * Easy :       0.5
         * Normal :     1
         * Hard :       2
         */

        if (factor <= beginnerScaler)
        {
            // Set ammo per shot to 1
            ammoPerShotScaler = 1;
            // Change bullet speed, damage, fire rate
            bulletSpeedScaler = damagePerShotScaler = fireRateScaler = factor;
        }
        else if (factor >= easyScaler)
        {
            // Divide ammo per shot by 2
            ammoPerShotScaler = source.AmmoPerShot / 2;
            // Change bullet speed, damage, fire rate
            bulletSpeedScaler = damagePerShotScaler = fireRateScaler = factor;
        }
        else if (factor <= mediumScaler)
        {
            // Keep all originals
            ammoPerShotScaler = source.AmmoPerShot;
            // Change bullet speed, damage, fire rate
            bulletSpeedScaler = damagePerShotScaler = fireRateScaler = factor;
        }
        else if (factor <= hardScaler)
        {
            // Multiply by 2 the AmmoPerShot
            ammoPerShotScaler = source.AmmoPerShot * 2;
            // Change bullet speed, damage, fire rate
            bulletSpeedScaler = damagePerShotScaler = fireRateScaler = factor;
        }
        else
        {
            // Multiply by factor the AmmoPerShot
            ammoPerShotScaler = Mathf.CeilToInt(source.AmmoPerShot * factor);
            // Don't change bullet speed, capped
            bulletSpeedScaler = 2;
            // Change damage, fire rate
            damagePerShotScaler = fireRateScaler = factor;
        }

        // AmmoPerShot
        //if (source.AmmoPerShot > ammoPerShotScaler)
        if (ammoPerShotScaler <= 0)
            ammoPerShotScaler = 1;

        scaled.AmmoPerShot = ammoPerShotScaler;

        // Bullet life time related to bullet speed
        scaled.ShootSpeed = source.ShootSpeed * bulletSpeedScaler;
        scaled.BulletLifeTime = source.BulletLifeTime / bulletSpeedScaler;

        // Damage per shot
        scaled.DamagePerShot = Mathf.RoundToInt(source.DamagePerShot * damagePerShotScaler);

        // Reload time and weapon fire rate
        scaled.ReloadTime = source.ReloadTime / fireRateScaler;
        scaled.FireRate = source.FireRate / fireRateScaler;
    }

    public bool IsAmmoCompatible(BulletType ammoType)
    {
        if ((ammoType & CompatibleTypes) != 0)
            return true;
        return false;
    }

    public override string ToString()
    {
        return
$@"DamagePerShot : {DamagePerShot}
ShootSpeed : {ShootSpeed}
ReloadTime : {ReloadTime}
BulletLifeTime : {BulletLifeTime}
DelayBetweenShots : {FireRate}";
    }

    public enum BulletType
    {
        Normal = 1,
        Piercing = 2,
        Fire = 4,
        Ice = 8,
    }

    public class BaseWeaponModifier
    {
        public BaseWeapon CreateTarget()
        {
            BaseWeapon output = new BaseWeapon();
            SetTarget(output);
            return output;
        }

        public virtual void SetTarget(BaseWeapon target)
        {
            target.Name += " Modified";
        }
    }
}