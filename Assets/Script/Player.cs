using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Player : MonoBehaviour
{
    public int Id;
    public string UserName;

    public Transform ShootOrigin;
    public float Health;
    public float MaxHealth = 100.0f;

    public int ItemAmount = 0;
    public int MaxItemAmount = 3;

    private float MoveSpeed = 5f / Constants.TICKS_PER_SEC;
    private bool[] Inputs;

    public void Initialize(int _Id, string _UserName)
    {
        Id = _Id;
        UserName = _UserName;
        Health = MaxHealth;

        Inputs = new bool[4];
    }

    public void FixedUpdate()
    {
        if(Health <= 0f)
        {
            return;
        }

        Vector2 _InputDirection = Vector2.zero;
        if (Inputs[0])
        {
            _InputDirection.y += 1;
        }
        if (Inputs[1])
        {
            _InputDirection.y -= 1;
        }
        if (Inputs[2])
        {
            _InputDirection.x -= 1;
        }
        if (Inputs[3])
        {
            _InputDirection.x += 1;
        }

        Move(_InputDirection);
    }

    private void Move(Vector2 _InputDirection)
    {
        Vector3 _MoveDirection = new Vector3(_InputDirection.x, _InputDirection.y, 1);
        transform.position += _MoveDirection * MoveSpeed;

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void SetInput(bool[] _Inputs, Quaternion _Rotation)
    {
        Inputs = _Inputs;
        transform.rotation = _Rotation;
    }

    public void Shoot(Vector3 _ViewDirection)
    {
        RaycastHit2D[] _Hit = Physics2D.RaycastAll(ShootOrigin.position, _ViewDirection, 25.0f);

        foreach (RaycastHit2D Object in _Hit)
        {
            if (Object.collider.CompareTag("Player") && Object.collider != this.GetComponent<Collider2D>())
            {
                Debug.Log($"Hit To {Object.collider.GetComponent<Player>().UserName}");
                Object.collider.GetComponent<Player>().TakeDamage(50.0f);

                break;
            }
        }
    }

    public void TakeDamage(float _Damage)
    {
        if(Health <= 0.0f)
        {
            return;
        }

        Health -= _Damage;
        if(Health <= 0.0f)
        {
            Health = 0.0f;
            transform.position = new Vector3(0f, 0f, 0f);

            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3.0f);

        Health = MaxHealth;
        ServerSend.PlayerRespawn(this);
        yield break;
    }

    public bool AttemptPickUpItem()
    {
        if(ItemAmount >= MaxItemAmount)
        {
            return false;
        }

        ItemAmount++;
        return true;
    }
}
