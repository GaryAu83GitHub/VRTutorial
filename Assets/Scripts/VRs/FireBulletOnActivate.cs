using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireBulletOnActivate : MonoBehaviour
{
    public GameObject BulletObject;
    public Transform MuzzleTransform;
    public float bulletSpeed = 20;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(Fire);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire(ActivateEventArgs arg)
    {
        GameObject bullet = Instantiate(BulletObject);
        bullet.transform.position = MuzzleTransform.position;
        bullet.GetComponent<Rigidbody>().velocity = MuzzleTransform.forward * bulletSpeed;
        Destroy(bullet, 5);
    }
}
