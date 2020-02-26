using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player controls for platformer demo
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// Company: Falling Flames Games
/// </summary>

namespace IndieMarc.Platformer
{

    public class PlatformerControls : MonoBehaviour
    {

        public int player_id;
        public KeyCode left_key;
        public KeyCode right_key;
        public KeyCode up_key;
        public KeyCode down_key;
        public KeyCode jump_key;
        public KeyCode action_key;
        public KeyCode pause_key;
        public KeyCode exit_key;

        public KeyCode caught_key;
        public KeyCode victory_key;


        private Vector2 move = Vector2.zero;
        private bool jump_press = false;
        private bool jump_hold = false;
        private bool action_press = false;
        private bool action_hold = false;
        private bool pause_press = false;
        private bool exit_press = false;
        private bool caught_press = false;
        private bool victory_press = false;

        private static Dictionary<int, PlatformerControls> controls = new Dictionary<int, PlatformerControls>();

        void Awake()
        {
            controls[player_id] = this;
        }

        void OnDestroy()
        {
            controls.Remove(player_id);
        }

        void Update()
        {

            move = Vector2.zero;
            jump_hold = false;
            jump_press = false;
            action_hold = false;
            action_press = false;
            pause_press = false;
            exit_press = false;
            caught_press = false;
            victory_press = false;

            if (Input.GetKey(left_key))
                move += -Vector2.right;
            if (Input.GetKey(right_key))
                move += Vector2.right;
            if (Input.GetKey(up_key))
                move += Vector2.up;
            if (Input.GetKey(down_key))
                move += -Vector2.up;
            if (Input.GetKey(jump_key))
                jump_hold = true;
            if (Input.GetKeyDown(jump_key))
                jump_press = true;
            if (Input.GetKey(action_key))
                action_hold = true;
            if (Input.GetKeyDown(action_key))
                action_press = true;
            if (Input.GetKeyDown(pause_key))
                pause_press = true;
            if (Input.GetKeyDown(exit_key))
                exit_press = true;
            if (Input.GetKeyDown(caught_key))
                caught_press = true;
            if (Input.GetKeyDown(victory_key))
                victory_press = true;

            float move_length = Mathf.Min(move.magnitude, 1f);
            move = move.normalized * move_length;
        }


        //------ These functions should be called from the Update function, not FixedUpdate
        public Vector2 GetMove()
        {
            return move;
        }

        public bool GetJumpDown()
        {
            return jump_press;
        }

        public bool GetJumpHold()
        {
            return jump_hold;
        }

        public bool GetActionDown()
        {
            return action_press;
        }

        public bool GetPausePressed()
        {
            return pause_press;
        }

        public bool GetExitPressed()
        {
            return exit_press;
        }

        public bool GetCaughtPressed()
        {
            return caught_press;
        }

        public bool GetVictoryPressed()
        {
            return victory_press;
        }

        public bool GetActionHold()
        {
            return action_hold;
        }

        //-----------

        public static PlatformerControls Get(int player_id)
        {
            foreach (PlatformerControls control in GetAll())
            {
                if (control.player_id == player_id)
                {
                    return control;
                }
            }
            return null;
        }

        public static PlatformerControls[] GetAll()
        {
            PlatformerControls[] list = new PlatformerControls[controls.Count];
            controls.Values.CopyTo(list, 0);
            return list;
        }

    }

}
