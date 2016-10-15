using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Gameplay.Player
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerInput : NetworkBehaviour
    {
        private PlayerMovement _movement;
        private PlayerSpellCaster _spellCaster;

        void Start ()
        {
            _movement = GetComponent<PlayerMovement>();
            _spellCaster = GetComponent<PlayerSpellCaster>();
        }
	
        void Update ()
        {
            // Only local player's input is proceeded
            if(!isLocalPlayer)
                return;

            if (Input.GetButtonDown("Jump"))
                _spellCaster.CmdCast(0);

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
}
