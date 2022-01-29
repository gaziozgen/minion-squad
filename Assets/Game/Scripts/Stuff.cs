using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stuff : MonoBehaviour
{
    public List<Transform> points = null;
    [SerializeField] private float movingSpeed = 2;
    [SerializeField] private float holdHigh = 0.25f;
    [SerializeField] private int money = 5;
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] Transform mesh = null;

    private bool[] isValidPoint;
    private bool ready = false;
    private List<Minion> holders = null;
    private List<Minion> reservedMinions = null;
    private int currentMinionNumber = 0;
    private int reservedPlace = 0;
    private Transform finalTarget = null;
    private NavMeshAgent agent = null;

    // reset
    private float resetTime = 10f;
    private float resetCooldown;

    void Awake()
    {
        resetCooldown = resetTime;
        isValidPoint = new bool[points.Count];
        for (int i = 0; i < isValidPoint.Length; i++)
        {
            isValidPoint[i] = true;
        }
        holders = new List<Minion>();
        reservedMinions = new List<Minion>();
        finalTarget = FindObjectOfType<Truck>().enterPoint;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movingSpeed;
    }

    void Update()
    {
        if (ready)
        {
            agent.SetDestination(finalTarget.position);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Truck"))
        {
            print("earn " + money + "$");

            for (int i = 0; i < holders.Count; i++)
            {
                holders[i].SetTarget(finalTarget);
            }
            other.GetComponent<Truck>().Bounce(bounceScale);
            transform.LeanScale(Vector3.zero, 0.5f).setOnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }

    public bool Available(Minion minion)
    {
        DeepCheck();
        if (reservedPlace < points.Count && !reservedMinions.Contains(minion))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DeepCheck()
    {
        if (reservedPlace == points.Count)
        {
            foreach (Minion minion in reservedMinions.ToArray())
            {
                if (!minion || !holders.Contains(minion) || !minion.GetTarget() || !minion.GetTarget().CompareTag("Stuff"))
                {
                    print("sorunlu bir minyon bulundu---------------------------------------------------hedefi: " + minion.GetTarget());
                    reservedPlace -= 1;
                    reservedMinions.Remove(minion);
                }
            }
        }
    }

    public Transform Reserve(Minion minion)
    {
        if (reservedPlace < points.Count)
        {
            reservedPlace += 1;
            reservedMinions.Add(minion);
            print(gameObject.name + " " + reservedPlace + " / " + points.Count);
            return transform;
        }
        else
        {
            print("there is no space for reservation");
            return null;
        }
    }

    public void CancelReservation(Minion minion)
    {
        reservedPlace -= 1;
        reservedMinions.Remove(minion);
    }

    public Transform TakePlace(Minion minion)
    {
        if (currentMinionNumber < points.Count)
        {
            int i;
            bool find = false;
            for (i = 0; i < isValidPoint.Length; i++)
            {
                if (isValidPoint[i])
                {
                    find = true;
                    isValidPoint[i] = false;
                    break;
                }
            }
            if (find)
            {
                minion.positonOnStuff = i;
                holders.Add(minion);
                currentMinionNumber += 1;

                if (currentMinionNumber == points.Count)
                {
                    ready = true;
                    Carry();
                }
                minion.SetSpeed(movingSpeed + 1f);
                return points[i];
            }
            else
            {
                print("yer var gözüküyodu ama bütün noktalar doluymuþ");
                return null;
            }
        }
        else
        {
            print("there is no space for take place");
            return null;
        }
    }

    public void Leave(GameObject gameObject)
    {
        foreach (Minion holder in holders.ToArray())
        {
            if (ReferenceEquals(gameObject, holder.gameObject))
            {
                print("a minion leaved from" + gameObject.name);
                isValidPoint[holder.positonOnStuff] = true;
                holder.positonOnStuff = -1;
                holders.Remove(holder);
                reservedPlace -= 1;
                currentMinionNumber -= 1;
                holder.ResetSpeed();
                agent.SetDestination(transform.position);
                if (ready)
                {
                    ready = false;
                    Drop();
                }
            }
        }
    }

    public void Carry()
    {
        mesh.localPosition = new Vector3(0, holdHigh, 0);
    }

    public void Drop()
    {
        mesh.localPosition = Vector3.zero;
    }
}
