using System.Collections;
using UnityEngine;

public class SmiteController : MonoBehaviour, Iweapon
{
    [SerializeField] SmiteData smiteData;   // El ScripteableObject con la info del tipo de Smite
    [SerializeField] TrailData trailData;   // La info de la representación de los misiles mágicos de este tipo de Smite

    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioSource audioSource;

    bool CanShoot = true;   // Esta variable nos servirá para controlar cuando podemos disparar o no


    public void Shoot(EnemyController target)
    {
        if (!CanShoot) return; // Si no podemos disparar terminamos la ejecución de esta llamada al método

        audioSource.PlayOneShot(shootSound);

        /////////////////////////////////////////////////////////////////////////////////
        /// Podemos dibujar una serie de Debug.Lines para visualizar el área de daño 
        ///------------------------------------------------------------------------------
        // Por cada x angulos dibujamos una linea que vaya desde el centro del target hacia fuera
        // Que sean tan largas como el radio del ataque y duren al menos medio segundo para poder apreciar el área
        ///////////////////////////////////////////////////////////////////////////////

        Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, smiteData.AreaRadius, LayerMask.GetMask("Enemy"));

        foreach (var hitCollider in hitColliders)
        {
            EnemyController enemy = hitCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.GetDamaged(smiteData.Damage);
            }
        }

        StartCoroutine(trailData.PlayTrail(target.transform.position + Vector3.up * 20, target.transform.position));

        //////////////////
        /// A RELLENAR ///
        ///////////////////////////////////////////////////////////////////////////////
        // - Casteamos una OverlapSphete en la posicion del target y de radio AreaRadious que solo afecte a los objetos de la layer Enemy
        // - Las coliosiones que hayamos detectado los guardaremos en una array de Colliders
        // - Por cada collider detectado
        //      - Cogeremos el EnemyController de la colision y le haremos la canitdad de daño que indique Damage
        // - Por ultimo iniciamos la corrutina que dibuja y mueve los "proyectiles" 
        //   dandole el una posicion encima del target como inicio y la posicion del target como final de la trayectoria 
        //////////////////////////////////////////////////////////////////////////////

        // Cada vez que disparemos tendremos que recargar
        Reload();
    }

    public void Reload()
    {
        /// Llamaremos a este método cuando queramos recargar
        // Solo iniciamos una coorrutina, pero podriamos rellenar este método 
        // con cualquier cosas que queremos que realize el player antes o durante la accion de recarga
        // (animaciones, ataques especiales, etc)

        StartCoroutine(Reloading());
    }

    public float GetRange() { return smiteData.Range; }
    public void SwitchWeapon() { Reload(); }

    IEnumerator Reloading()
    {
        CanShoot = false;
        yield return new WaitForSeconds(smiteData.ReloadingTime);
        CanShoot = true;
    }
}
