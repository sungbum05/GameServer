using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> Projectiles = new Dictionary<int, Projectile>();
    private static int NextProjectileId = 1;

    public int Id;
    public Rigidbody2D Rigidbody;
    public int ThrowByPlayer;
    public Vector3 InitialForce;
    public float ExplosionRadius = 1.5f;
    public float ExplosionDamage = 75f;

    private void Start()
    {
        Debug.Log("Projectile Start");

        //Id = NextProjectileId;
        //NextProjectileId++;
        //Projectiles.Add(Id, this);

        //ServerSend.SpawnProjectile(this, ThrowByPlayer);
        //Debug.Log("Projectile Spawn Func");

        //Rigidbody.AddForce(InitialForce);
        //StartCoroutine(ExplodeAfterTime());
    }

    public void Initialize(Vector3 _InitialMovementDirection, float _InitialForceStrength, int _ThrowByPlayer)
    {
        Debug.Log("Projectile Initialize");

        InitialForce = _InitialMovementDirection * _InitialForceStrength;
        ThrowByPlayer = _ThrowByPlayer;

        //원래 Start 내용
        Debug.Log("Projectile Start");
        Id = NextProjectileId;
        NextProjectileId++;
        Projectiles.Add(Id, this);

        ServerSend.SpawnProjectile(this, ThrowByPlayer);
        Debug.Log("Projectile Spawn Func");

        Rigidbody.AddForce(InitialForce);
        StartCoroutine(ExplodeAfterTime());
    }

    private void FixedUpdate()
    {
        ServerSend.ProjectilePosition(this);
    }

    private void OnTriggerEnter2D(Collider2D Other)
    {
        if(Server.Clients[ThrowByPlayer].MyPlayer.GetComponent<Collider2D>() != Other.GetComponent<Collider2D>())
            Explode();
    }

    private void Explode()
    {
        ServerSend.ProjectileExplode(this);

        Collider2D[] _Colliders = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);
        foreach(Collider2D _Collider in _Colliders)
        {
            if(_Collider.CompareTag("Player") && _Collider != this.GetComponent<Collider2D>())
            {
                _Collider.GetComponent<Player>().TakeDamage(ExplosionDamage);
            }
        }

        Projectiles.Remove(Id);
        Destroy(gameObject);
    }

    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(10.0f);

        Explode();
    }
}
