using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shoot : PowerUp
{

    float fireCooldown = 0.5f;
    float fireStartTime;
    public bool coolingDown;
    public BulletBehavior bulletPrefab;
    public ChargedBullet chargedBulletPrefab;

    public List<GameObject> bulletPool = new List<GameObject>();

    public Action SetBulletType;
    public GameObject laserPrefab;

    int maxBulletNumber;
    int bulletsSpent;
    PlayerReferences playerReferences;

    public bool allowFire;

    float fireAllowedDuration = 15.0f;
    public override void Awake()
    {
        base.Awake();
        playerReferences = GetComponent<PlayerReferences>();
        ChoosePowerUp.laserChosen += this.SetPoweredUp;
        autoActivated = false;
        ourRequirement = Requirement.OnlyUseOffSwitch;

        Switch.SwitchEntered += this.SetOnSwitch;
        Switch.SwitchExited += this.SetOffSwitch;

    }

    public override void StartPowerUp(){
        base.StartPowerUp();
        StartCoroutine(CountDownUntilCantFire());
    }

    

    public IEnumerator CountDownUntilCantFire(){
        allowFire = true;
        yield return new WaitForSeconds(fireAllowedDuration);
        allowFire = false;
    }

    void SetBulletTypeNormal()
    {

    }

    void SetBulletTypeFlare()
    {

    }

    public void LaserFire()
    {
        laserPrefab.SetActive(true);
    }

    public void Fire()
    {
        BulletBehavior bullet = bulletPrefab.GetPooledInstance<BulletBehavior>();

        //GameObject obj = ObjectPool.current.GetPooledObject();

        if (!coolingDown && bullet != null)
        {
            fireStartTime = Time.time;

            bullet.transform.position = new Vector3(transform.position.x, transform.position.y, 14.89f);
            bullet.transform.rotation = transform.rotation;
            bullet.Fly(Input.mousePosition);
        }
        else
        {
            return;
        }

    }

    public void ChargeFire()
    {
        Debug.Log("ChargedFire");
        ChargedBullet bullet = chargedBulletPrefab.GetPooledInstance<ChargedBullet>();

        //GameObject obj = ObjectPool.current.GetPooledObject();

        if (!coolingDown && bullet != null)
        {
            fireStartTime = Time.time;

            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.Fly(Input.mousePosition);
        }
        else
        {
            Debug.Log("Nothing happened");
            return;
        }
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (Time.time < fireStartTime + fireCooldown)
        {
            coolingDown = true;
        }
        else
        {
            coolingDown = false;
        }

    }
}
