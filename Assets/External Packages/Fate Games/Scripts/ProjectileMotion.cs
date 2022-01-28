using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public static class ProjectileMotion
    {
        public delegate void Callback();
        public static Motion CreateProjectileMotion(this Transform projectile, Vector3 targetPosition, float time)
        {
            return CreateProjectileMotion(projectile, targetPosition, time, null);
        }

        public static Motion CreateProjectileMotion(this Transform projectile, Vector3 targetPosition, float time, Callback onComplete)
        {
            return CreateMotion(projectile, targetPosition, time, onComplete);
        }

        public static Motion CreateMotion(Transform projectile, Vector3 targetPosition, float time)
        {
            return CreateMotion(projectile, targetPosition, time, null);
        }

        public static Motion CreateMotion(Transform projectile, Vector3 targetPosition, float time, Callback onComplete)
        {
            return new Motion(projectile, targetPosition, time, onComplete);
        }

        public static void SimulateProjectileMotion(this Transform projectile, Vector3 targetPosition, float time)
        {
            SimulateProjectileMotion(projectile, targetPosition, time, null);
        }

        public static void SimulateProjectileMotion(this Transform projectile, Vector3 targetPosition, float time, Callback onComplete)
        {
            Motion motion = CreateMotion(projectile, targetPosition, time, onComplete);
            Simulate(motion);
        }

        public static void Simulate(Transform projectile, Vector3 targetPosition, float time)
        {
            Simulate(projectile, targetPosition, time, null);
        }
        public static void Simulate(Transform projectile, Vector3 targetPosition, float time, Callback onComplete)
        {
            Motion motion = CreateMotion(projectile, targetPosition, time, onComplete);
            Simulate(motion);
        }

        public static void Simulate(Motion motion)
        {
            float timePassed = 0;
            Vector3 velocity = motion.Force;
            LeanTween.value(motion.Projectile.gameObject, (float value) =>
            {
                float deltaTime = timePassed;
                timePassed = value;
                deltaTime = timePassed - deltaTime;
                velocity += Physics.gravity * deltaTime;
                motion.Projectile.position = motion.Projectile.position + velocity * deltaTime;
            }, 0, motion.Time, motion.Time).setOnComplete(() =>
            {
                motion.Projectile.position = motion.TargetPosition;
                if (motion.OnComplete != null)
                    motion.OnComplete();
            });
        }

        public class Motion
        {
            private Vector3 force = Vector3.zero;
            private float time = 0;
            private Transform projectile = null;
            private Vector3 targetPosition = Vector3.zero;
            private Vector3 startPosition = Vector3.zero;
            private Callback onComplete = null;

            public Vector3 Force
            {
                get
                {
                    return force;
                }
            }
            public float Time
            {
                get
                {
                    return time;
                }
            }

            public Transform Projectile
            {
                get
                {
                    return projectile;
                }
            }

            public Vector3 TargetPosition
            {
                get
                {
                    return targetPosition;
                }
            }
            public Vector3 StartPosition
            {
                get
                {
                    return startPosition;
                }
            }

            public Callback OnComplete
            {
                get
                {
                    return onComplete;
                }
            }
            public Motion(Transform projectile, Vector3 targetPosition, float time, Callback onComplete)
            {
                this.targetPosition = targetPosition;
                this.time = time;
                this.projectile = projectile;
                this.onComplete = onComplete;
                Vector3 dif = targetPosition - projectile.position;
                Vector3 force = new Vector3(dif.x, 0, dif.z) / time;
                force.y = dif.y / time - Physics.gravity.y * time / 2;
                this.force = force;
                this.startPosition = projectile.position;
            }
        }
    }
}

