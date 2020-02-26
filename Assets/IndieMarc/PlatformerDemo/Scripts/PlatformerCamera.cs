using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Platformer camera script
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// Company: Falling Flames Games
/// </summary>

namespace IndieMarc.Platformer
{

    public class PlatformerCamera : MonoBehaviour
    {
        [Header("Camera Target")]
        public GameObject target;
        public Vector3 target_offset;
        public float camera_speed = 5f;

        [Header("Level Limits")]
        public float level_bottom;
        public float level_left;
        public float level_right;

        private PlatformerCharacter target_character;
        private Camera cam;
        private Vector3 cur_pos;
        private GameObject lock_target = null;

        private static PlatformerCamera _instance;

        void Awake()
        {
            _instance = this;
            cam = GetComponent<Camera>();
        }

        void LateUpdate()
        {
            GameObject cam_target = target;

            if (lock_target != null)
                cam_target = lock_target;

            if (cam_target != null)
            {
                //Find target
                Vector3 target_pos = cam_target.transform.position + target_offset;

                //Set level limits
                float fh = GetFrustrumHeight() / 2f;
                float fw = GetFrustrumWidth() / 2f;
                target_pos.x = Mathf.Max(level_left + fw, target_pos.x);
                target_pos.x = Mathf.Min(level_right - fw, target_pos.x);
                target_pos.y = Mathf.Max(level_bottom + fh, target_pos.y);

                //Check if need to move
                Vector3 diff = target_pos - transform.position;
                if (diff.magnitude > 0.1f)
                {
                    //Move camera
                    transform.position = Vector3.SmoothDamp(transform.position, target_pos, ref cur_pos, 1f / camera_speed, Mathf.Infinity, Time.deltaTime);
                }
            }
        }

        public float GetFrustrumHeight()
        {
            if (cam.orthographic)
                return 2f * cam.orthographicSize;
            else
                return 2.0f * Mathf.Abs(transform.position.z) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        public float GetFrustrumWidth()
        {
            return GetFrustrumHeight() * cam.aspect;
        }

        public void LockCameraOn(GameObject ltarget)
        {
            lock_target = ltarget;
        }

        public void UnlockCamera()
        {
            lock_target = null;
        }

        public static PlatformerCamera Get()
        {
            return _instance;
        }

        public static Camera GetCamera()
        {
            if (_instance)
                return _instance.cam;
            return null;
        }
    }

}
