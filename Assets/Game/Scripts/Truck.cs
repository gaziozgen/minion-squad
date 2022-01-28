using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FateGames;

public class Truck : MonoBehaviour
{
    public Transform enterPoint = null;
    [SerializeField] Transform generalMesh = null;
    [SerializeField] TMPro.TextMeshProUGUI text = null;
    [SerializeField] private float speed = 1;
    [SerializeField] private Transform minionThrowPosition = null;
    [SerializeField] private NavMeshSurface surface = null;
    [SerializeField] private int totalMinions = 50;
    [SerializeField] private Transform policeCarTarget = null;

    private int currentMinions;
    private Transform _transform = null;
    private GameManager gameManager = null;

    // player
    [SerializeField] private float cooldown = 0.2f;
    private float currentCooldown;

    void Awake()
    {
        currentMinions = totalMinions;
        surface.BuildNavMesh();
        _transform = transform;
        currentCooldown = cooldown;
        gameManager = GameManager.Instance;
        UpdateText();
    }

    void Update()
    {
        if (gameManager.State == GameState.IN_GAME)
        {
            _transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.z + speed * Time.deltaTime);
        }

        currentCooldown -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            if (currentCooldown <= 0)
            {
                currentCooldown = cooldown;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                int layerMask = 1 << 6 | 1 << 7 | 1 << 8 | 1 << 9;
                layerMask = ~layerMask;
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    // eðer mape týklandýysa kontrolü gelecek
                    ThrowMinion(hit.point);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Final final = collision.GetComponent<Final>();
        if (final)
        {
            gameManager.LevelManager.FinishLevel(true);
        }
    }

    private void ThrowMinion(Vector3 targetPos)
    {
        if (currentMinions > 0)
        {
            currentMinions -= 1;
            UpdateText();

            Transform minion = ObjectPooler.Instance.SpawnFromPool("Minion", minionThrowPosition.position, Quaternion.identity).transform;
            float time = Mathf.Sqrt((targetPos - minionThrowPosition.position).magnitude) / 3;

            // rotation
            Vector3 dir = targetPos - minionThrowPosition.position;
            dir.y = 0f;
            minion.rotation = Quaternion.Euler(90, Vector3.SignedAngle(Vector3.forward, dir, Vector3.up), 0);
            minion.LeanRotate(Vector3.zero, time);

            // position
            ProjectileMotion.SimulateProjectileMotion(minion, targetPos, time, () => {
                minion.GetComponent<Minion>().Land();
            });
        }

    }

    private void UpdateText()
    {
        text.text = currentMinions + "/" + totalMinions;
    }

    public void Bounce(float size)
    {
        LeanTween.cancel(generalMesh.gameObject);
        generalMesh.LeanScale(new Vector3(size, size, 1), 0.1f).setOnComplete(() =>
        {
            generalMesh.LeanScale(Vector3.one, 0.1f);
        });
    }

    public void TakeMinion()
    {
        currentMinions += 1;
        UpdateText();
        Bounce(1.1f);
    }

    public Transform GetPoliceCarTarget()
    {
        return policeCarTarget;
    }

}
