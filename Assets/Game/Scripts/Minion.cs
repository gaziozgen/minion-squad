using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Minion : MonoBehaviour
{
    public int positonOnStuff = -1;

    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float movingSpeed = 2f;
    [SerializeField] private Transform holderPositon = null;
    [SerializeField] private float holdHigh = 0.25f;
    [SerializeField] private Transform mesh = null;
    [SerializeField] private float seeRange = 10f;

    private Police kidnapperPolice = null; // polis tarafýnda alýkonulmasý kesinleþtiyse, polis

    private bool hadResevation = false; // birpolidte vaye eþyada rezervasyonu varsa
    private bool isReservedByPolice = false; // bir polis tarafýndan rezerve edildiyse

    private bool needHelp = false; // eðer kaçýrýldýysa
    private bool dealingWithPolice = false; // eðer kaçýrýldý veya yardým ediyorsa

    private Transform target = null;
    private Transform finalTarget = null;
    private bool holding = false;
    private NavMeshAgent agent = null;
    private CapsuleCollider capsuleCollider = null;

    // for bug solving
    private bool cannotBeLand = false;

    void Awake()
    {
        finalTarget = FindObjectOfType<Truck>().enterPoint;
        capsuleCollider = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        target = null;
        agent.speed = speed;
    }

    private void OnObjectSpawn()
    {
        agent.enabled = false;
        positonOnStuff = -1;
        target = null;
        holding = false;
        capsuleCollider.enabled = true;
        hadResevation = false;
        kidnapperPolice = null;
        isReservedByPolice = false;
        needHelp = false;
        dealingWithPolice = false;
        cannotBeLand = false;
        ResetSpeed();
    }

    void Update()
    {
        if (agent.enabled) {
            if (target && target.gameObject.activeSelf)
            {
                agent.SetDestination(target.position);
            }
            else
            {
                FindTarget();
            }
        }
    }
    private void OnTriggerStay(Collider other) //---------------------------- to enter olabilir
    {
        if (other.CompareTag("PoliceCar"))
        {
            if (kidnapperPolice)
            {
                kidnapperPolice.target = other.transform;
            }
            capsuleCollider.enabled = false;
            cannotBeLand = true;
            other.GetComponent<PoliceCar>().TakeMinion();
            transform.LeanMove(other.transform.position, 0.4f);
            CancelMission(); // ------------------------------------------------ açmayý dene
            transform.LeanScale(Vector3.zero, 0.5f).setOnComplete(() =>
            {
                agent.enabled = false;
                gameObject.SetActive(false);
            });
            
        }
        else if (agent.enabled)
        {
            if (other.CompareTag("Truck"))
            {
                capsuleCollider.enabled = false;
                agent.enabled = false;
                other.GetComponent<Truck>().TakeMinion();
                CancelMission(); // ------------------------------------------------ açmayý dene
                gameObject.SetActive(false);
            }
            else if (other.CompareTag("Police"))
            {
                Police police = other.GetComponent<Police>();
                if (ReferenceEquals(other.gameObject, target.gameObject)) // minyon polisi yakalamýþ
                {
                    hadResevation = false;
                    print("bu minyon yakalanan minyona yardýma geldi");
                    police.target = finalTarget;
                    police.Carry();
                    police.OnSecondMinionCome(this);

                } 
                else if (ReferenceEquals(police.target.gameObject, gameObject) && !needHelp) // polis minyonu yakalamýþ
                {
                    if (holding)
                    {
                        Stuff stuff = target.parent.parent.GetComponent<Stuff>();
                        if (stuff)
                        {
                            holding = false;
                            target.parent.parent.GetComponent<Stuff>().Leave(gameObject);
                        }
                        else
                        {
                            print("minyon yakalanýrken stuff çoktan kamyona verilmiþ");
                        }
                        
                    }
                    else if (dealingWithPolice) {

                        if (hadResevation)
                        {
                            if (target.CompareTag("Police"))
                            {
                                target.GetComponent<Police>().CancelReservation();
                            }
                            else
                            {
                                print("bir rezervasyon var gözüküyor ancak bulunamadý");
                            }
                        }
                        else
                        {
                            print("polis baþka minyona yardým eden minyonu yakaladý");

                            Police oldPolice = target.parent.parent.GetComponent<Police>();
                            oldPolice.OnSecondMinionWent(this);
                            oldPolice.Drop();
                            oldPolice.SetSpeed(movingSpeed); // aslýnda doðru minyondan almadý ama olsun zaten taþýma hýzý minyonlarýn kendi içinde deðiþmiyor
                        }
                    }
                    else if (hadResevation)
                    {
                        if(target.CompareTag("Stuff"))
                        {
                            target.GetComponent<Stuff>().CancelReservation();
                        }
                        else
                        {
                            print("bir rezervasyon var gözüküyor ancak bulunamadý");
                        }
                    }
                    Carry();
                    police.SetSpeed(movingSpeed);
                    police.KidnapTheMinion(this);
                    dealingWithPolice = true;
                    needHelp = true;
                    kidnapperPolice = police;
                    CallHelp();
                }
            }
            else if (other.CompareTag("Stuff") && !holding && ReferenceEquals(other.gameObject, target.gameObject))
            {
                hadResevation = false;
                holding = true;
                target = other.GetComponent<Stuff>().TakePlace(GetComponent<Minion>());
            }
        }
    }

    public void FindTarget()
    {
        int layerMask = 1 << 7 | 1 << 6;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, seeRange, layerMask);

        float min = seeRange;
        Collider closestTarget = null;
        bool helpCall = false;
        foreach (var hitCollider in hitColliders)
        {
            float length = (transform.position - hitCollider.transform.position).magnitude;

            if (hitCollider.CompareTag("Stuff") && !helpCall && (length < min))
            {
                Stuff stuff = hitCollider.GetComponent<Stuff>();
                if (stuff.Available())
                {
                    min = length;
                    closestTarget = hitCollider;
                }
            }
            else if (hitCollider.CompareTag("Minion"))
            {
                Minion minion = hitCollider.GetComponent<Minion>();
                if (!ReferenceEquals(gameObject, minion.gameObject) && minion.IsNeedHelp())
                {
                    if (helpCall)
                    {
                        if (min > length)
                        {
                            print(hitCollider.gameObject.name + " yardým çaðýrýyor");
                            min = length;
                            closestTarget = hitCollider;
                        }
                    }
                    else
                    {
                        print(hitCollider.gameObject.name + " yardým çaðýrýyor");
                        helpCall = true;
                        min = length;
                        closestTarget = hitCollider;
                    }
                }
            }
        }
        if (closestTarget)
        {
            if (helpCall)
            {
                dealingWithPolice = true;
                target = closestTarget.GetComponent<Minion>().ReservationForHelp();
            }
            else
            {
                target = closestTarget.GetComponent<Stuff>().Reserve();
            }
            hadResevation = true;
        }
        else
        {
            target = finalTarget;
        }
    }

    private void CancelMission()
    {
        if (holding)
        {
            Stuff stuff = target.parent.parent.GetComponent<Stuff>();
            if (stuff)
            {
                target.parent.parent.GetComponent<Stuff>().Leave(gameObject);
                holding = false;
            }
        }
        if (hadResevation)
        {
            if (target.CompareTag("Stuff"))
            {
                target.GetComponent<Stuff>().CancelReservation();
            }
            else if (target.CompareTag("Police"))
            {
                target.GetComponent<Police>().CancelReservation();
            }
            else
            {
                print("bir rezervasyon var gözüküyor ancak bulunamadý " + gameObject.GetInstanceID().ToString());
            }
        }
    }

    public void CallHelp()
    {
        int layerMask = 1 << 6;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, seeRange, layerMask);

        float min = seeRange;
        Collider closestTarget = null;
        foreach (var hitCollider in hitColliders)
        {
            float length = (transform.position - hitCollider.transform.position).magnitude;
            if (!ReferenceEquals(gameObject, hitCollider.gameObject) && length < min)
            {
                Minion minion = hitCollider.GetComponent<Minion>();

                if (minion.AveliableToHelp())
                {
                    min = length;
                    closestTarget = hitCollider;
                }
            }
        }
        if (closestTarget)
        {
            Minion minion = closestTarget.GetComponent<Minion>();
            minion.CancelMission();
            minion.FindTarget();
        }
    }

    public void Land()
    {
        if (!cannotBeLand)
        {
            capsuleCollider.enabled = true;
            agent.enabled = true;
            FindTarget();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public bool AveliableToHelp()
    {
        print("AveliableToHelp");
        return agent.enabled && !dealingWithPolice;
    }

    public bool IsNeedHelp()
    {
        return needHelp && kidnapperPolice && kidnapperPolice.Available();
    }

    public Transform ReservationForHelp()
    {
        print("ReservationForHelp");
        dealingWithPolice = true;
        return kidnapperPolice.Reserve();
    }

    public bool AveliableForPoliceHunt()
    {
        return agent.enabled && !isReservedByPolice && !needHelp;
    }

    public void ReservedForPolice()
    {
        print("ReservedForPolice");
        isReservedByPolice = true;
    }

    public void Carry()
    {
        mesh.localPosition = new Vector3(0, holdHigh, 0);
        agent.speed = movingSpeed;
    }

    public void Drop()
    {
        mesh.localPosition = Vector3.zero;
        agent.speed = speed;
    }

    public Transform GetHoldPositon()
    {
        return holderPositon;
    }

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void ResetSpeed()
    {
        agent.speed = speed;
    }

    public void ResetReservations()
    {
        hadResevation = false;
    }
}
