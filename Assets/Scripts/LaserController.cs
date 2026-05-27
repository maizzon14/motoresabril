using System.Collections;
using UnityEngine;

public class LaserController : MonoBehaviour, Iweapon
{
    [SerializeField] LaserData laserData;  // El Scriptable Object con la info de del láser
    [SerializeField] LineData lineData;    // La info de la representación del rayo de este tipo de laser

    [SerializeField] Transform Barrel;     // Posición desde la que dispararemos el laser
    [SerializeField] GameObject User;      // El GameObject que usa el arma, en nuestro caso el Player

    /// La info basica del arma
    int Damage;
    float Radius;
    float ReloadTime;
    float Range;
    float FireRating;

    int MaxAmo;                            // Municion máxima
    int AmoCost;                           // Coste de disparar un laser
    int CurrentAmo;                        // Municion actual


    bool CanShoot = true;                  // Esta variable nos servirá para controlar cuando se puede disparar o no

    Vector3 direction;                     // Direción en la que disparamos
    Vector3 origin;                        // Posición desde la que disparamos

    Transform[] laserPoints;               // Vector de puntos que determina la forma y longitud del laser

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Asignamos a nuestras variables la info del ScripteableObject
        Damage = laserData.Damage;
        Radius = laserData.Radius;
        Range = laserData.Range;

        FireRating = laserData.FireRating;
        ReloadTime = laserData.ReloadTime;

        MaxAmo = laserData.MaxAmo;
        AmoCost = laserData.AmoCost;

        // Empezamos con la municion al máximo
        CurrentAmo = MaxAmo;

        // Este laser va a tener solo dos puntos: inicio y final. Los creamos y se los asignamos al array de puntos
        laserPoints = new Transform[2]
        {
           new GameObject("LaserOrigin").transform,
           new GameObject("LaserEnd").transform
        };
    }

    void Update()
    {

        LayerMask notEnemies = ~LayerMask.GetMask("Enemy");

        direction = User.transform.forward.normalized;
        origin = Barrel.position;
        float limit = Range;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, Range, notEnemies))
        {
            limit = hit.distance;
        }

        laserPoints[0].position = origin;
        laserPoints[1].position = origin + direction * limit;

        //////////////////
        /// A RELLENAR ///
        //////////////////////////////////////////////////////////////////
        /// Explicacion de que vamos a hacer:
        /// -------------------------------------------------------------
        /// No queremos que el láser atravisese muros o paredes, pero que no tenga problemas con los enemigos
        /// Para ello tenemos que detectar con antelacion cuando el laser va a colisionar contra un objeto que no queremos atravesar
        /// y acutualizar el punto final del laser en nuestra array con el punto en el que hemos golpeado a ese objeto.
        /// El cálculo lo vamos a hacer con un RayCast que detectará el primer objeto no atravesable que encuentre.
        /// Las LayerMask nos ayudarán con esto: 
        ///     - Podeis crear una especifica para los objetos no atravesables.
        ///     - Podeis INVERTIR LA MASK del enemy para detectar cualquier cosa que no sea un Enemy (buscad en la docucomo hacerlo)
        /// 
        /// Este cálculo podríamos hacerlo perfectamente en la función Shoot, antes de realizar el ShpereCastAll.
        /// Pero hacerlo en el Update nos da un par de ventajas:
        ///     - Actualizar continuamente las posiciones de inicio y final del láser nos permite utilizar herramienteas de debugging
        ///       para visualizar el alcanze del láser en todo momento
        ///     - Si queremos hacer un láser con cierto movimiento, la opción más simple es actualizar las posiciones en el Update
        /// 
        //////////////////////////////////////////////////////////////////
        // Para poder actualizar los puntos del laser tendremos que:
        // --------------------------------------------------------------
        // - Actualizar la 'direction' con la dirección forward (normalizada) actual del 'User' 
        // - Actualizar la posicion 'origin' con la posición actual del 'Barrel'
        // - Vamos a crear una variable 'limit' donde guardaremos la distancia a la que llega el láser y le asignaremo el 'Range' como distacia base
        // - Creamos una variable RaycastHit para guardar la información del impacto del RayCast que haremos a continuacion
        // - Lanzamos un Raycast y, si golpea algo que de una Layer que no podemos atravesar
        //      - Actualizamos la 'limit' con la distancia que hay hasta el punto de impacto
        // - Actualizamos la posicion del laserPoints[0] con 'origin'
        // - Actualizamos la posicion del laserPoints[1] con la 'origin' + 'direction' * 'limit'
        //////////////////////////////////////////////////////////////////
    }

    public void Shoot(EnemyController target)
    {

        if (!CanShoot) return;

        float distance = Vector3.Distance(laserPoints[0].position, laserPoints[1].position);
        RaycastHit[] hits = Physics.SphereCastAll(origin, Radius, direction, distance, LayerMask.GetMask("Enemy"));
            foreach (RaycastHit hit in hits)
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.GetDamaged(Damage);
                }
            }

        float duration = FireRating / 3;

        StartCoroutine(lineData.DrawLineRenderer(laserPoints, duration));
        CurrentAmo -= AmoCost;
        if (CurrentAmo < AmoCost)
        {
            StartCoroutine(Reloading());
        }
        else
        {
            StartCoroutine(WaitFireRate());
        }

        //////////////////
        /// A RELLENAR ///
        //////////////////////////////////////////////////////////////////
        /// Explicacion de que vamos a hacer:
        /// -------------------------------------------------------------
        /// Si podemos disparar, lanzaremos un SphereCastAll que golpe a todos los enemgido (solo enemigos) que encuentre en su camino
        /// desde la posicion Inicial del Laser hasta la posicion final que habremos cualculado en el Update.
        /// A cada enemigo que nos encontremos le haremos daño
        //////////////////////////////////////////////////////////////////
        // Como lo haremos
        // --------------------------------------------------------------
        // - Comprobar si podemos disparar y si no, terminar la ejecucion del metodo
        // - Creamos una variable 'distance' que guarde la distancia que hay entre el laserPoints[0] y el laserPoints[1]
        // - Creamos una Array de RayCastHit donde guardar los impactos del SphereCast que haremos a continuacion
        // - Lanzamos un ShpereCastAll, que empiece en 'origin', de radio 'radius', con direccion 'direction', distacia 'distance' y que solo afecte a la Layer Enemy. 
        //   Guardaremos los impactos detectados en la Array que hemos creado antes (Echadle un ojo a la DOCUMENTACION)
        // - Por cada impacto guardado en la lista
        //      - Sacamos el EnemyController del gameobject al que hemos golpeado
        //      - Le hacemos la cantidad de daño marcada por 'damage'
        // - Creamos una variable 'duration' donde guardaremos una fraccion del tiempo entre disparos (Por ejemplo:'FireRating / 3')
        // - Inciamos la coorutina del lineData para que dibuje un láser con los laserPoints y que dure lo que hemos decidido en 'duration'
        // - Reducimos 'CurrentAmo' lo indicado por 'AmoCost'
        // - Si 'CurrentAmo' es menor que 'AmoCost'
        //      - Recargamos el arma
        // - Sino 
        //      - Iniciamos la corrutina para esperar el tiempo entre disparos
        //////////////////////////////////////////////////////////////////
    }

    public float GetRange() { return Range; }
    public void Reload() { StartCoroutine(Reloading()); }
    public void SwitchWeapon() { Reload(); }


    IEnumerator Reloading()
    {
        CanShoot = false;
        yield return new WaitForSeconds(ReloadTime);
        CurrentAmo = MaxAmo;
        CanShoot = true;
    }

    IEnumerator WaitFireRate()
    {
        CanShoot = false;
        yield return new WaitForSeconds(FireRating);
        CanShoot = true;
    }

    void OnDrawGizmos()
    {
        /// Dibujaremos unas esferas las posiciones que tendría el laser a cada momento, con un tamaño igual al 'Radius'

        // OnDrawGizmo se ejecuta incluso cuando no le damos a "PLAY".
        // Como la lista de puntos la generamos en el Awake vamos a gacer una pequeña comprobacion para evitar que nos de errores cuando no estemos jugando
        if (laserPoints == null || laserPoints.Length == 0) return;

        Gizmos.color = Color.lightBlue;

        // Por cada posicion en la Array de puntos
        for (int i = 0; i < laserPoints.Length; i++)
        {
            // Dibujamos una esfera de tamaño 'Radious' en el lugar que indica el Tranform en la poscion i
            Gizmos.DrawWireSphere(laserPoints[i].position, Radius);
            // Si no estamos en la primera posicion del array, dibujaremos una linea entre los puntos de la posicion anterior (i-1) y la actual (i)
            if (i > 0) Gizmos.DrawLine(laserPoints[i - 1].position, laserPoints[i].position);
        }
    }
}
