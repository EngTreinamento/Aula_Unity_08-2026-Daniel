using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    // Referência de qual arma estamos utilizando
    [SerializeField] WeaponData weaponData;

    //Referência para a nossa câmera
    [SerializeField] Camera cam;
    //Referência de onde vai sair o tiro
    [SerializeField] Transform muzzle;

    //Prefab do tiro que vai ser instanciado
    [SerializeField] GameObject bulletPrefab;

    //Referência de onde o player está olhando
    [SerializeField] Transform visual;

    //A referência de camadas para saber o que vamos igorar na hora de fazer a mira
    [SerializeField] LayerMask aimMask = ~0;
    //Controle de velocidade que o player gira para olhar para a mira
    [SerializeField] float aimTurnSpeed = 12f;
    //Booelana para controlar se eu quero que o player vire sem estar atirando
    [SerializeField] bool turnWhenShoot = true;

    //Controle de cadência de tiro
    float nextFireTime;
    //Variável para saber qual é o meu collider
    Collider myCollider;
    //Vetor 3 que guarda o último lugar visto
    Vector3 lastAimPoint;
    //Booleana para saber se estamos mirando
    bool isMirando;
    //Booleana para saber se estmaos controlando o Giro
    public bool controllingTurn;

    private void Awake()
    {
        cam = Camera.main;

        myCollider = GetComponent<Collider>();
    }

    void Update()
    {
        //Pegando o ponto de vista
        lastAimPoint = CalculatePointView();

        //Guardando a informação que eu estou mirando com o botão direito do mouse
        isMirando = Input.GetMouseButton(1);

        //Verificando se eu estou clicando no botão esquerdo do mouse
        bool isShooting = Input.GetMouseButton(0);
        bool wantToShoot;

        //Verificando a minha arma tem cadência, se tiver, fala que eu estou tentando atirar ou não
        if(weaponData.cadence > 0)
        {
            wantToShoot = isShooting;
        }
        else
        {
            wantToShoot = Input.GetMouseButtonDown(0);
        }

        controllingTurn = isMirando || (isShooting && turnWhenShoot);

        if (controllingTurn)
        {
            TurnToCameraDirection();
        }

        if (wantToShoot)
        {
            TryToShoot();
        }
    }

    private Vector3 CalculatePointView()
    {
        Ray camRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(camRay, out RaycastHit hit, weaponData.range, aimMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }
        else
        {
            return camRay.GetPoint(weaponData.range);
        }
    }

    private void TurnToCameraDirection()
    {
        Vector3 look = cam.transform.forward;

        look.y = 0;

        Quaternion target = Quaternion.LookRotation(look);

        visual.rotation = Quaternion.Slerp(visual.rotation, target, aimTurnSpeed * Time.deltaTime);
    }

    private void TryToShoot()
    {
        if(weaponData.cadence == 0)
        {
            Shoot();
            return;
        }

        if(Time.time < nextFireTime)
        {
            return;
        }

        nextFireTime = Time.time + (1 / weaponData.cadence);

        Shoot();
    }

    private void Shoot()
    {
        Vector3 origin = muzzle.position;
        Vector3 direction = (lastAimPoint - origin).normalized;

        if(Vector3.Dot(direction, cam.transform.forward) < 0)
        {
            direction = cam.transform.forward;
        }

        GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.LookRotation(direction));

        if(bullet.TryGetComponent(out Collider bulletCollider))
        {
            Physics.IgnoreCollision(bulletCollider, myCollider);
        }

        bullet.GetComponent<Bullet>().Setup(weaponData.damage);
    }
}
