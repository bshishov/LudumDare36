using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerState))]
public class PlayerSpellCaster : NetworkBehaviour
{
    public SpellData Spell1;
    public SpellData Spell2;
    public SpellData Spell3;
    
    public Vector3 Force = new Vector3(0, 0.1f, 1f);
    public Vector3 Offset = new Vector3();
    public float StartRadius = 0.3f;

    private PlayerState _state;

    private float _time1;
    private float _time2;
    private float _time3;

    void Start()
    {
        _state = GetComponent<PlayerState>();
    }

    [Command]
    public void CmdCast(int spellId)
    {
        if(!isServer)
            return;

        if(_state.CanCast)
            RpcCast(spellId);
    }

    [ClientRpc]
    void RpcCast(int spellId)
    {
        if(!_state.CanCast)
            return;

        if (spellId == 1 && Time.time > _time1)
        {
            _time1 = Time.time + Spell1.CoolDown;
            Cast(Spell1);
        }

        if (spellId == 2 && Time.time > _time2)
        {
            _time2 = Time.time + Spell2.CoolDown;
            Cast(Spell2);
        }

        if (spellId == 3 && Time.time > _time3)
        {
            _time3 = Time.time + Spell3.CoolDown;
            Cast(Spell3);
        }
    }

    void Cast(SpellData spell)
    {
        if (spell == null || spell.Prefab == null)
        {
            Debug.LogWarning("Spell prefab is empty");
            return;
        }

        var direction = transform.TransformDirection(Vector3.forward);
        var spellInstance = GameObject.Instantiate(spell.Prefab, transform.position + direction * StartRadius + Offset, Quaternion.identity) as GameObject;

        if (spell.IsProjectile)
        {
            var rigidBody = spellInstance.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                rigidBody.AddForce(transform.TransformVector(Force), ForceMode.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Offset, StartRadius);
    }
}
