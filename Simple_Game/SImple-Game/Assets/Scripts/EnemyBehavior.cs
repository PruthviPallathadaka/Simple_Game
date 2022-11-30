
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    private const float AttackSpeed = 5f;
    private const float MovementSpeed = 10f;
    private const float AttackDistance = 2f;
    private GameObject _player;
    private NavMeshAgent _navMeshAgent;
    private GameObject _target;
    private GameObject[] _cells;

    private float _attackTime = 0f;
    public const float MaxHealth = 1f;
    public float Health = MaxHealth;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _target = null;
        _cells = GameObject.FindGameObjectsWithTag("Cell");
    }

    // Update is called once per frame 
    private void Update()
    {
        //Vector3 targetPos = _player.transform.position;
        //Vector3 dir = (targetPos - this.transform.position).normalized;
        //transform.position += new Vector3(dir.x * MovementSpeed * Time.deltaTime, dir.y * MovementSpeed * Time.deltaTime, 0);

        if(_target == null || !_target.activeSelf)
        {
            var isThereEnemyLeft = SelectNewTarget();
            if(!isThereEnemyLeft)
            {
                GameOver();
            }
        }

        if(!_navMeshAgent.SetDestination(_target.transform.position))
        {
            Debug.Log("CANNOT GO TO POSITION");
        }


        if(Vector3.Distance(_target.transform.position, transform.position) <= AttackDistance)
        {
            _attackTime += Time.deltaTime;
            if(_attackTime >= AttackSpeed)
            {
                KillCell(_target);
            }
        }
        else
        {
            _attackTime = 0f;
        }

        UpdateHealth();
    }

    private void UpdateHealth()
    {
        if (Vector3.Distance(_player.transform.position, transform.position) <= AttackDistance)
        {
            Health -= Time.deltaTime;
        }
        else
        {
            Health += Time.deltaTime;
        }

        if(Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        else if(Health < 0f)
        {
            gameObject.SetActive(false);
            Debug.LogError("Virus: Oh noe, I am dead!");
        }

    }


    private void KillCell(GameObject target)
    {
        target.SetActive(false);
    }

    private bool SelectNewTarget()
    {
        var closestDistance = float.MaxValue;
        foreach(var cell in _cells)
        {
            if(cell.activeSelf)
            {
                var newDistance = Vector3.Distance(cell.transform.position, transform.position);
                if (newDistance <= closestDistance)
                {
                    closestDistance = newDistance;
                    _target = cell;
                    //multiple enemies can avoid targeting the same cell
                }
            }
        }

        return (_target != null && _target.activeSelf);
    }

    private void GameOver()
    {
        Debug.LogError("ALL CELLS ARE DESTROYED, GAME OVER, YOU LOST!");

    }
}
