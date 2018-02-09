using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shoot : MonoBehaviour
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
    void Awake()
    {
        //TODO: Make different icons for laser/Ride/shoot stuff
        playerReferences = GetComponent<PlayerReferences>();
        ChoosePowerUp.laserChosen += this.AllowFire;
    }

    void AllowFire()
    {
        if (playerReferences.locationHandler.currentSwitch == null)
        {
            //this only applies when the playre is on a switch
            allowFire = true;
        }

    }

    public IEnumerator CountDownUntilCantFire(){
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
    void Update()
    {

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
