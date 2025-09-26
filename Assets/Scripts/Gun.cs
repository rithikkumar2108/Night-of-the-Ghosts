using SingularityGroup.HotReload;
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("Read-Only Stats")]
    public float fireRate;
    public float Damage;
    [Range(0, 0.3f)]
    public float SpreadRange;
    [Space]
    public Camera fpsCam;
    public GameObject BulletObject;
    public Transform BulletSpawnPoint;
    public ParticleSystem MuzzleFlash;
    [Header("Parameters")]
    public float BulletForce;
    public float maxRange;

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

        if (!isReloading)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, Time.deltaTime * recoilSpeed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, initialLocalRot, Time.deltaTime * recoilSpeed);
        }
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
        Bullet bullet1 = bullet.GetComponent<Bullet>();
        bullet1.Damage = (Damage);
        MuzzleFlash.Play();
        bullet.transform.forward = (targetPoint - bullet.transform.position).normalized;
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * BulletForce, ForceMode.Impulse);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return StartCoroutine(ReloadAnimation());

        if (MagazineCount > 0)
        {
            CurrentAmmo = MagazineCapacity;
            MagazineCount--;
        }

        isReloading = false;
        Debug.Log("Reloaded!");
    }

    IEnumerator ReloadAnimation()
    {
        float duration = reloadTime; // how long the animation takes
        float elapsed = 0f;

        Vector3 forwardPos = initialLocalPos + Vector3.forward * 0.7f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Move forward/back smoothly
            transform.localPosition = Vector3.Lerp(initialLocalPos, forwardPos, Mathf.Sin(t * Mathf.PI));

            // Apply rotation incrementally (Y = cowboy twirl, Z = roll)
            transform.localRotation = initialLocalRot * Quaternion.Euler(t * 360f, 0, 0f);

            yield return null;
        }

        // Reset at the end
        transform.localPosition = initialLocalPos;
        transform.localRotation = initialLocalRot;
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
