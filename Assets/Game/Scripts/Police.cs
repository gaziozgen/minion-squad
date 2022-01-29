using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Police : MonoBehaviour
{

    public Transform target = null;

    [SerializeField] private List<Transform> points = null;
    [SerializeField] private float seeRange = 10;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float movingSpeed = 2;
    [SerializeField] private float holdHigh = 0.25f;
    [SerializeField] private int money = 5;
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] Transform mesh = null;

    private Transform finalTarget = null;
    private List<Minion> minions = null;
    private bool isReserved = false;
    private NavMeshAgent agent = null;

    void Awake()
    {
        minions = new List<Minion>();
        finalTarget = FindObjectOfType<Truck>().enterPoint;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    private void OnObjectSpawn()
    {
        target = null;
        minions = new List<Minion>();
        isReserved = false;
        ResetSpeed();
    }

    void Update()
    {
        if (target && target.gameObject.activeSelf)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            LookForTarget();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Truck"))
        {
            print("earn " + money + "$");
            for (int i = 0; i < minions.Count; i++)
            {
                minions[i].SetTarget(other.transform);
            }
            other.GetComponent<Truck>().Bounce(bounceScale);
            transform.LeanScale(Vector3.zero, 0.5f).setOnComplete(() =>
            {
                transform.localScale = Vector3.one;
                gameObject.SetActive(false);
            });
        }
        else if (other.CompareTag("PoliceCar") && target.CompareTag("PoliceCar"))
        {
            finalTarget.GetComponent<PoliceCar>().TakePolice();
            transform.LeanScale(Vector3.zero, 0.5f).setOnComplete(() =>
            {
                transform.localScale = Vector3.one;
                gameObject.SetActive(false);
            });
        }
    }

    public bool Available()
    {
        return !isReserved;
    }

    public Transform Reserve()
    {
        if (!isReserved)
        {
            isReserved = true;
            return transform;
        }
        else
        {
            print("there is no space for reservation");
            return null;
        }
    }

    public void CancelReservation()
    {
        isReserved = false;
    }

    private void LookForTarget()
    {
        int layerMask = 1 << 6;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, seeRange, layerMask);

        Transform newTarget = null;

        foreach (var hitCollider in hitColliders)
        {
            Minion minion = hitCollider.GetComponent<Minion>();
            if (minion.AveliableForPoliceHunt())
            {
                minion.ReservedForPolice();
                newTarget = hitCollider.transform;
                break;
            }
        }

        if(newTarget)
        {
            target = newTarget;
        }
        else
        {
            target = finalTarget;
        }
    }

    public void KidnapTheMinion(Minion minion)
    {
        print("KidnapTheMinion");
        if (minions.Count == 0)
        {
            minions.Add(minion);
            minion.SetTarget(finalTarget);
            target = minion.GetHoldPositon();
        }
        else
        {
            print("bunun yaþanmamasý gerekiyodu -----------------------");
        }
    }

    public void OnSecondMinionCome(Minion minion)
    {
        print("GetSecondPlace");
        if (minions.Count == 1)
        {
            minions.Add(minion);
            minions[0].SetTarget(points[0]);
            minions[0].Drop();
            minions[0].SetSpeed(movingSpeed + 0.5f);
            minion.SetTarget(points[1]);
            minion.SetSpeed(movingSpeed + 0.5f);
        }
        else
        {
            print("dolu kardeþim doluuuuuuuuuu ------------------");
        }
    }

    public void OnSecondMinionWent(Minion secondMinion)
    {
        isReserved = false;
        target = minions[0].GetHoldPositon();
        minions[0].SetTarget(finalTarget);
        minions[0].Carry();
        minions[0].CallHelp();
        if (!minions.Remove(secondMinion))
        {
            print("böyle bir minyon yokmuþ --------------");
        }

    }


    public void SetFinalTarget(Transform policeCar)
    {
        finalTarget = policeCar;
    }

    public void Carry()
    {   mesh.localRotation = Quaternion.Euler(-90, 0, 0);
        mesh.localPosition = new Vector3(0, holdHigh, 0.3f);
        agent.speed = movingSpeed;
    }

    public void Drop()
    {
        mesh.localPosition = Vector3.zero;
        mesh.localRotation = Quaternion.Euler(0, 0, 0);
        agent.speed = speed;
    }

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void ResetSpeed()
    {
        agent.speed = speed;
    }
}
