using GFG;
using IndieMarc.Platformer;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

/// <summary>
/// Platformer character movement
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// Company: Falling Flames Games
/// </summary>

namespace GFG
{

    public class Guard : MonoBehaviour
    {

        public int player_id;

        [Header("Movement")]
        public float move_accel = 1f;
        public float move_deccel = 1f;
        public float move_max = 1f;

        [Header("Jump")]
        public bool can_jump = true;
        public float jump_strength = 1f;
        public float jump_time_min = 1f;
        public float jump_time_max = 1f;
        public float jump_gravity = 1f;
        public float jump_fall_gravity = 1f;
        public float jump_move_percent = 0.75f;
        public LayerMask ground_layer;
        public float ground_raycast_dist = 0.1f;

        [Header("Crouch")]
        public bool can_crouch = true;
        public float crouch_coll_percent = 0.5f;

        [Header("Death")]
        public float level_bottom;

        [Header("Parts")]
        public GameObject hold_hand;

        [HideInInspector] public UnityAction onDeath;
        
        private Rigidbody2D rigid;
        private Animator animator;
        private CapsuleCollider2D capsule_coll;
        private ContactFilter2D contact_filter;
        private Vector2 coll_start_h;
        private Vector2 coll_start_off;
        private Vector3 start_scale;

        private Vector2 move;
        private Vector2 move_input;
        private bool jump_press;
        private bool jump_hold;
        private bool disable_controls = false;

        private bool is_grounded = false;
        private bool is_ceiled = false;
        private bool is_fronted = false;
        private bool is_crouch = false;
        private bool is_jumping = false;
        private bool is_hiding = false;
        private float jump_timer = 0f;
        private Vector3 last_ground_pos;

        private float take_item_timer = 0f;

        private bool can_see_player;
        private bool inside_vision;

        PlatformerCharacter player;

        GameManager gameManager;

        public bool DirectionLeft;
        public float Range = 5;
        float currentRange;

        private static Dictionary<int, Guard> character_list = new Dictionary<int, Guard>();

        void Awake()
        {
            //character_list[player_id] = this;
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            capsule_coll = GetComponent<CapsuleCollider2D>();
            coll_start_h = capsule_coll.size;
            coll_start_off = capsule_coll.offset;
            start_scale = transform.localScale;

            contact_filter = new ContactFilter2D();
            contact_filter.layerMask = ground_layer;
            contact_filter.useLayerMask = true;
            contact_filter.useTriggers = false;

            last_ground_pos = transform.position;

            move_input.x = DirectionLeft ? -1 : 1;
        }

        [Inject]
        public void Construct(GameManager _gameManager)
        {
            gameManager = _gameManager;
        }

        void OnDestroy()
        {
            //character_list.Remove(player_id);
        }

        void Start()
        {

        }

        //Handle physics
        void FixedUpdate()
        {
            //Movement velocity
            float desiredSpeed = Mathf.Abs(move_input.x) > 0.1f ? move_input.x * move_max : 0f;
            float acceleration = Mathf.Abs(move_input.x) > 0.1f ? move_accel : move_deccel;
            acceleration = !is_grounded ? jump_move_percent * acceleration : acceleration;
            move.x = Mathf.MoveTowards(move.x, desiredSpeed, acceleration * Time.fixedDeltaTime);

            //Side facing
            if (Mathf.Abs(move.x) > 0.01f)
            {
                float side = (move.x < 0f) ? -1f : 1f;
                transform.localScale = new Vector3(start_scale.x * side, start_scale.y, start_scale.z);
            }

            UpdateJump();
            UpdateCrouch();

            float moveDistance = move_input.x * move_max * Time.fixedDeltaTime;
            currentRange += moveDistance;

            if (Mathf.Abs(currentRange) >= Range)
            {
                DirectionLeft = !DirectionLeft;
                move_input.x = DirectionLeft ? -1 : 1;
                currentRange = 0;
            }

            Vector3 pos = transform.position;
            pos.x += moveDistance;
            transform.position = pos;

            transform.localScale = new Vector3(start_scale.x * move_input.x, start_scale.y, start_scale.z);
            //Move
            //rigid.velocity = move;
        }

        //Handle render and controls
        void Update()
        {
            //move_input = Vector2.zero;
            jump_press = false;
            jump_hold = false;

            bool was_visible = can_see_player;
            if(inside_vision)
            {
                if (player != null)
                {
                    can_see_player = !player.IsHiding();
                }
            }
            else
            {
                can_see_player = false;
            }

            /*if(can_see_player)
            {
                if(player != null)
                {
                    can_see_player = !player.IsHiding();
                }
            }*/

            if (was_visible != can_see_player)
            {
                gameManager.PlayerSpotted(can_see_player);
            }

            /*
            //Controls
            if (!disable_controls)
            {
                PlatformerControls controls = PlatformerControls.Get(player_id);
                move_input = controls.GetMove();
                jump_press = controls.GetJumpDown();
                jump_hold = controls.GetJumpHold();

                if(controls.GetPausePressed())
                {

                }

                if(controls.GetExitPressed())
                {
                    gameManager.ShowTitleScreen();
                }

                if (jump_press)
                    TryJump();

                //Items
                take_item_timer += Time.deltaTime;
                if (carry_item && controls.GetActionDown())
                    carry_item.UseItem();
            }
            */

            //Reset when fall
            if (transform.position.y < level_bottom - GetCollSize().y)
            {
                Teleport(GetLastGround());
            }

            //Anims
            animator.SetBool("Jump", IsJumping());
            animator.SetBool("InAir", !IsGrounded());
            animator.SetBool("Hide", IsHiding());
            animator.SetBool("Crouch", IsCrouching());
            animator.SetFloat("Speed", move.magnitude);
            //animator.SetBool("Hold", GetHoldingItem() != null);
        }

        private void UpdateJump()
        {
            //Jump
            is_grounded = DetectGrounded(Vector2.down);
            is_ceiled = DetectGrounded(Vector2.up);
            is_fronted = DetectGrounded(Vector2.right * GetSide());
            jump_timer += Time.fixedDeltaTime;

            //Jump end timer
            if (is_jumping && !jump_hold && jump_timer > jump_time_min)
                is_jumping = false;
            if (is_jumping && jump_timer > jump_time_max)
                is_jumping = false;

            //Jump hit ceil
            if (is_ceiled)
            {
                is_jumping = false;
                move.y = Mathf.Min(move.y, 0f);
            }

            if (is_fronted)
            {
                move.x = 0f;
            }

            //Add jump velocity
            if (!is_grounded)
            {
                //Falling
                float gravity = !is_jumping ? jump_fall_gravity : jump_gravity; //Gravity increased when going down
                move.y = Mathf.MoveTowards(move.y, -move_max * 2f, gravity * Time.fixedDeltaTime);
            }
            else if (!is_jumping)
            {
                //Grounded
                move.y = 0f;
            }

            //Save last landed position
            if (is_grounded)
                last_ground_pos = transform.position;
        }

        private void UpdateCrouch()
        {

        }

        private void TryJump()
        {
            if (can_jump && is_grounded && !is_crouch && !is_hiding)
            {
                move.y = jump_strength;
                jump_timer = 0f;
                is_jumping = true;
                animator.SetTrigger("Action");
            }
        }

        private bool DetectGrounded(Vector2 dir)
        {
            bool grounded = false;
            Vector2[] raycastPositions = new Vector2[3];

            Vector2 raycast_start = rigid.position;
            float radius = GetCollSize().x * 0.5f;
            bool vertical = (Mathf.Abs(dir.y) > Mathf.Abs(dir.x));

            if (capsule_coll != null && vertical)
            {
                //Adapt raycast to collider
                Vector2 raycast_offset = capsule_coll.offset + dir.normalized * Mathf.Abs(capsule_coll.size.y * 0.5f - capsule_coll.size.x * 0.5f);
				raycast_start = rigid.position + raycast_offset * transform.localScale.y;
            }

            Vector2 side1 = vertical ? Vector2.left * radius / 2f : Vector2.up * radius;
            Vector2 side2 = vertical ? Vector2.right * radius / 2f : Vector2.down * radius;

            float ray_size = radius + ground_raycast_dist;
            raycastPositions[0] = raycast_start + side1;
            raycastPositions[1] = raycast_start;
            raycastPositions[2] = raycast_start + side2;

            RaycastHit2D[] hitBuffer = new RaycastHit2D[5];
            for (int i = 0; i < raycastPositions.Length; i++)
            {
                Physics2D.Raycast(raycastPositions[i], dir.normalized, contact_filter, hitBuffer, ray_size);
                Debug.DrawRay(raycastPositions[i], dir.normalized * ray_size);
                for (int j = 0; j < hitBuffer.Length; j++)
                {
                    if (hitBuffer[j].collider != null)
                    {
                        grounded = true;
                    }
                }
            }
            return grounded;
        }

        public void Kill()
        {
            //To Do
            //Not done because right now there is nothing beyond the demo level.
            //Could make it lose a life, or reload the level
        }

        public void Teleport(Vector3 pos)
        {
            transform.position = pos;
            move = Vector2.zero;
            is_jumping = false;
        }

        public Vector3 GetMove()
        {
            return move;
        }

        public float GetSide()
        {
            return Mathf.Sign(transform.localScale.x);
        }

        public Vector3 GetLastGround()
        {
            return last_ground_pos;
        }

        public bool IsJumping()
        {
            return is_jumping;
        }

        public bool IsGrounded()
        {
            return is_grounded;
        }

        public bool IsCrouching()
        {
            return is_crouch;
        }

        public bool IsHiding()
        {
            return is_hiding;
        }

        public Vector2 GetCollSize()
        {
            if (capsule_coll != null)
                return new Vector2(Mathf.Abs(transform.localScale.x) * capsule_coll.size.x, Mathf.Abs(transform.localScale.y) * capsule_coll.size.y);
            return new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        }

        public Vector3 GetHandPos()
        {
            return hold_hand.transform.position;
        }

        public bool IsAlive()
        {
            return true; //TO DO
        }

        public void DisableControls() { disable_controls = true; }
        public void EnableControls() { disable_controls = false; }

        void OnTriggerEnter2D(Collider2D coll)
        {
            PlatformerCharacter ch = coll.gameObject.GetComponent<PlatformerCharacter>();
            if (ch != null)
            {
                player = ch;
                //can_see_player = !ch.IsHiding();
                inside_vision = true;
            }
            //can_hide |= coll.gameObject.GetComponent<HideTrigger>();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            PlatformerCharacter ch = collision.gameObject.GetComponent<PlatformerCharacter>();
            if (ch != null)
            {
                //can_see_player = false;

                if(!ch.IsHiding())
                {
                    gameManager.PlayerSpotted(false);
                }

                inside_vision = false;
            }
        }

        /*public static void LockGameplay()
        {
            foreach (PlatformerCharacter character in GetAll())
                character.DisableControls();
        }

        public static void UnlockGameplay()
        {
            foreach (PlatformerCharacter character in GetAll())
                character.EnableControls();
        }

        public static PlatformerCharacter GetNearest(Vector3 pos, float range=999f, bool alive_only=true)
        {
            PlatformerCharacter nearest = null;
            float min_dist = range;
            foreach (PlatformerCharacter character in character_list.Values)
            {
                if (!alive_only || character.IsAlive())
                {
                    float dist = (pos - character.transform.position).magnitude;
                    if (dist < min_dist)
                    {
                        min_dist = dist;
                        nearest = character;
                    }
                }
            }
            return nearest;
        }
        
        public static PlatformerCharacter Get(int player_id)
        {
            foreach (PlatformerCharacter character in character_list.Values)
            {
                if (character.player_id == player_id)
                {
                    return character;
                }
            }
            return null;
        }
        
        public static PlatformerCharacter[] GetAll()
        {
            PlatformerCharacter[] list = new PlatformerCharacter[character_list.Count];
            character_list.Values.CopyTo(list, 0);
            return list;
        }*/
    }

}
