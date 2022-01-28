using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;


public class PoliceCar : MonoBehaviour
{
    [SerializeField] private float cooldownToRestart = 5f;
    [SerializeField] private int totalPolice = 1;
    [SerializeField] TMPro.TextMeshProUGUI policeCount = null;
    [SerializeField] TMPro.TextMeshProUGUI minionCount = null;
    [SerializeField] Transform generalMesh = null;

    private int currentPolices;
    private int currentMinions = 0;
    private Transform target = null;

    private bool start = false;
    private bool ready = false;
    private float currentCooldownToRestart;

    void Awake() 
    {
        currentCooldownToRestart = cooldownToRestart;
        currentPolices = totalPolice;
        Truck truck = FindObjectOfType<Truck>();
        target = truck.GetPoliceCarTarget();
        UpdatePoliceText();
        UpdateMinionText();
    }

    private void Come()
    {
        transform.position = target.position - new Vector3(0, 0, 10);
        currentPolices = totalPolice;
        ready = true;
        UpdatePoliceText();
    }

    void Update()
    {
        if (start)
        {
            if (currentPolices > 0)
            {
                float dif = (target.position - transform.position).z;
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Time.deltaTime * dif * 3 / 4);

                if (currentPolices > 0)
                {
                    int layerMask = 1 << 6;

                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 8, layerMask);

                    foreach (var hitCollider in hitColliders)
                    {
                        Minion minion = hitCollider.GetComponent<Minion>();
                        if (minion.AveliableForPoliceHunt())
                        {
                            currentPolices -= 1;
                            UpdatePoliceText();
                            minion.ReservedForPolice();
                            Police police = ObjectPooler.Instance.SpawnFromPool("Police", transform.position, Quaternion.identity).GetComponent<Police>();
                            police.SetFinalTarget(transform);
                            police.target = hitCollider.transform;
                            break;
                        }
                    }
                }
            }
            else
            {
                currentCooldownToRestart -= Time.deltaTime;
                if (currentCooldownToRestart < 0)
                {
                    currentCooldownToRestart = cooldownToRestart;
                    Come();
                }
            }
        }
    }

    public void TakePolice()
    {
        Bounce();
        currentPolices += 1;
        UpdatePoliceText();
        currentCooldownToRestart = cooldownToRestart;
    }

    public void TakeMinion()
    {
        Bounce();
        currentMinions += 1;
        UpdateMinionText();
    }

    private void UpdatePoliceText()
    {
        policeCount.text = currentPolices.ToString();
    }

    private void UpdateMinionText()
    {
        minionCount.text = currentMinions.ToString();
    }

    public void Bounce()
    {
        LeanTween.cancel(generalMesh.gameObject);
        generalMesh.LeanScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f).setOnComplete(() =>
        {
            generalMesh.LeanScale(Vector3.one, 0.1f);
        });
    }

    public void StartPlay()
    {
        LeanTween.delayedCall(2f, () =>
        {
            start = true;
            Come();
        });
    }
}
