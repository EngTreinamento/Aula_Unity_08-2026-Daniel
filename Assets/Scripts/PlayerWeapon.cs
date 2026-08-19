using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Configurações de Tiro")]
    [Tooltip("Referência de qual arma estamos utilizando")]
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

    //Tiramos a nextFireTime para usar uma booleana para saber se podemos atirar
    bool canShoot = true;
    //Variável para saber qual é o meu collider
    Collider myCollider;
    //Vetor 3 que guarda o último lugar visto
    Vector3 lastAimPoint;
    //Booleana para saber se estamos mirando
    bool isMirando;
    //Booleana para saber se estmaos controlando o Giro
    public bool controllingTurn;

    [Header("Configurações de Munição")]
    //Referência para saber quantos pentes nós temos, e inicialmente terá 3 pentes
    [SerializeField] int qtdPente = 3;
    int qtdInBag;
    //Referência de texto onde mostramos quanto tem de munição (30/90)
    [SerializeField] TextMeshProUGUI ammoText;
    //Referência de slider para mostrar a munição sendo utilizada
    [SerializeField] Slider ammoSlider;
    //Quanto tem de munição atual
    int actualAmmo;
    //Trava de ação enquanto recarrega
    bool reloading;

    private void Awake()
    {
        cam = Camera.main;

        myCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        //Quando inicia o jogo, eu tenho um pente carregado, por isso chamo o ammoCapacity
        actualAmmo = weaponData.ammoCapacity;
        qtdInBag = weaponData.ammoCapacity * qtdPente;
        //Depois atualizdo a UI com as informações
        UpdateAmmoInfo();
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

        if(Input.GetKeyDown(KeyCode.R) && reloading == false)
        {
            StartCoroutine(Reload());
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
        // Se a arma não tiver cadência, ela vai atirar e já vai parar o método utilizando o return;
        if(weaponData.cadence == 0)
        {
            Shoot();
            return;
        }

        // Se a cadência for maior que 0, e eu posso atirar, aí sim eu vou atirar
        if (weaponData.cadence > 0)
        {
            if (canShoot)
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        // Verificação de segurança, caso eu não possa atirar, seja por cadência ou pente vazio, eu saio do método
        if (canShoot == false || actualAmmo <= 0 || reloading == true)
        {
            //Caso tenha acabado de zerar o pente, ele chama a Corrotina de recarregamento
            if(actualAmmo <= 0 && reloading == false)
            {
                StartCoroutine(Reload());
            }
            return;
        }

        // Pegando informação de onde vai surgir o tiro, e qual a direção
        Vector3 origin = muzzle.position;
        Vector3 direction = (lastAimPoint - origin).normalized;

        //Medida de segurança para o tiro não ter risco de ir para trás do personagem
        if(Vector3.Dot(direction, cam.transform.forward) < 0)
        {
            direction = cam.transform.forward;
        }

        //Instanciando o tiro
        GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.LookRotation(direction));

        //Ignorando as colisões se possível
        if(bullet.TryGetComponent(out Collider bulletCollider))
        {
            Physics.IgnoreCollision(bulletCollider, myCollider);
        }

        // Passando para o tiro qual é o dano que ele vai causar
        bullet.GetComponent<Bullet>().Setup(weaponData.damage);

        //Quando eu coloco -- ou ++ ele reduz ou aumenta 1 na variável
        actualAmmo--;
        UpdateAmmoInfo();

        // Chamo a corrotina de intervalo de cadência
        StartCoroutine(CadenceInterval());
    }


    private IEnumerator CadenceInterval()
    {
        // Quando inicia a corrotina, a gente deixa a booleana como falsa,
        // esperamos o tempo de cadência, e depois deixamos true
        canShoot = false;

        //Importante: coloque o f quando é float, aconteceu dele interpretar que foi feita uma
        //divisão entre números inteiros, e ficava dando 0 de tempo
        yield return new WaitForSeconds(1f / weaponData.cadence);
        canShoot = true;
    }

    //Corrotina de recarga
    private IEnumerator Reload()
    {
        //Primeiro verifica se tem pente para ser recarregado
        if(qtdInBag > 0)
        {
            //Liga a trava de recarga para não recarregar várias vezes
            reloading = true;
            yield return new WaitForSeconds(1f);
            //Depois do intervalo, reduz um pente, alimenta a munição atual, desliga a trava e atualiza a UI
            int qtdRequired = weaponData.ammoCapacity - actualAmmo;

            int qtdAAdd = Mathf.Min(qtdRequired, qtdInBag);
            qtdInBag -= qtdAAdd;
            actualAmmo += qtdAAdd;

            reloading = false;
            UpdateAmmoInfo();
        }
    }

    //Método de atualizar a UI com as informações de munição
    private void UpdateAmmoInfo()
    {
        //Mostra no texto quanto tem atual e quanto tem de munição para ser recarregada
        ammoText.text = actualAmmo + "/" + qtdInBag;
        //Configuramos o slider para ser dinâmico de acordo com a arma utilizada
        //Atualizamos qual é o valor máximo considerando a capacidade do pente, e quanto ele tem atualmente
        ammoSlider.maxValue = weaponData.ammoCapacity;
        ammoSlider.value = actualAmmo;
    }

    public void AddPente(int pente)
    {
        qtdInBag += (pente * weaponData.ammoCapacity);
        if(actualAmmo <= 0 && reloading == false)
        {
            StartCoroutine(Reload());
        }
        UpdateAmmoInfo();
    }
}
