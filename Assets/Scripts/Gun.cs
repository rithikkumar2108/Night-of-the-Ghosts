using SingularityGroup.HotReload;
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public Camera fpsCam;
    public GameObject BulletObject;
    public Transform BulletSpawnPoint;

    [Header("Parameters")]
    public float fireRate;
    public float BulletForce;
    public float maxRange;
    [Range(0, 0.2f)]
    public float SpreadRange;

    public int MagazineCount;      // Spare magazines
    public int MagazineCapacity;   // Max bullets per mag
    public int CurrentAmmo;        // Bullets in current mag

    public float reloadTime = 2f;  // Reload duration in seconds
    public float recoilDistance = 0.1f; // How far the gun moves back
    public float recoilSpeed = 8f;      // How fast gun snaps back

    private float nextTimeToShoot = 0f;
    private bool isReloading = false;
    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;

    void Start()
    {
        initialLocalPos = transform.localPosition; // local to camera
        initialLocalRot = transform.localRotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && MagazineCount > 0 && CurrentAmmo < MagazineCapacity)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextTimeToShoot && !isReloading)
        {
            if (CurrentAmmo <= 0)
            {
                if (MagazineCount > 0)
                {
                    StartCoroutine(Reload());
                }
                return;
            }

            nextTimeToShoot = Time.time + 1 / fireRate;
            Shoot();
            CurrentAmmo--;

            StartCoroutine(RecoilAnimation());
        }

        // Smoothly return to original local position/rotation
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, Time.deltaTime * recoilSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, initialLocalRot, Time.deltaTime * recoilSpeed);
    }

    void Shoot()
    {
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, maxRange))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        targetPoint.x += Random.Range(-SpreadRange, SpreadRange);
        targetPoint.y += Random.Range(-SpreadRange, SpreadRange);

        GameObject bullet = Instantiate(BulletObject, BulletSpawnPoint.position, Quaternion.identity);
        bullet.transform.forward = targetPoint - bullet.transform.position;
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * BulletForce * Time.deltaTime, ForceMode.Impulse);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);

        if (MagazineCount > 0)
        {
            CurrentAmmo = MagazineCapacity;
            MagazineCount--;
        }

        isReloading = false;
        Debug.Log("Reloaded!");
    }

    IEnumerator RecoilAnimation()
    {
        Vector3 recoilPos = initialLocalPos - Vector3.forward * recoilDistance;

        float t = 0f;

        // Move back and rotate
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(initialLocalPos, recoilPos, t);
            yield return null;
        }

        t = 0f;
        // Return to initial
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(recoilPos, initialLocalPos, t);
            yield return null;
        }
    }
}
