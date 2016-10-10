using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInput : NetworkBehaviour
{
    private PlayerMovement _movement;
    private PlayerPunch _punch;
    private PlayerSpellCaster _spellCaster;

    void Start ()
    {
        _movement = GetComponent<PlayerMovement>();
        _punch = GetComponent<PlayerPunch>();
        _spellCaster = GetComponent<PlayerSpellCaster>();
    }
	
	void Update ()
    {
        // Only local player's input is proceeded
	    if(!isLocalPlayer)
            return;

	    if (Input.GetButtonDown("Jump"))
            _punch.CmdPunch();

        if (Input.GetButtonDown("Fire1"))
            _spellCaster.CmdCast(1);

        if (Input.GetButtonDown("Fire2"))
            _spellCaster.CmdCast(2);

        if (Input.GetButtonDown("Fire3"))
            _spellCaster.CmdCast(3);
    }

    void FixedUpdate()
    {
        // Only local player's input is proceeded
        if (!isLocalPlayer)
            return;

        _movement.InputMovement(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
