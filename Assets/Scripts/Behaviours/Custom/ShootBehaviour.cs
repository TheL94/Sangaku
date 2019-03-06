﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Deirin.AI;

namespace Sangaku
{
    /// <summary>
    /// Behaviour che gestisc lo sparo
    /// </summary>
    public class ShootBehaviour : BaseBehaviour
    {
        //references
        [SerializeField] BaseEntity projectilePrefab;
        [SerializeField] Transform shootPoint;

        //behaviours
        [SerializeField] bool usesObjectPooler = true;
        [SerializeField] bool hasTargets = true;
        [SerializeField] bool searchForTargetsOnAwake = true;
        [SerializeField] bool startsShootingOnAwake = false;

        //parameters
        [SerializeField] float secondsBetweenShots = 1f;
        [Range(0.1f, 360f)]
        [SerializeField] float turnRateAnglesPerSecond = 90;
        [SerializeField] string projectilePoolTag;

        [SerializeField] UnityFloatEvent OnShoot;

        float timer = 0;
        List<Transform> targets = new List<Transform>();
        Transform currentTarget;
        float shorterTargetDistance;
        bool canShoot = false;

        protected override void CustomSetup()
        {
            if (!projectilePrefab)
            {
                Debug.LogError(name + " has no projectile referenced!");
                return;
            }
            if (!shootPoint)
            {
                Debug.LogError(name + " has no projectile spawn point referenced!");
                return;
            }
            if (searchForTargetsOnAwake)
            {
                FindTargets();
            }
            if (startsShootingOnAwake)
                canShoot = true;
        }

        private void Update()
        {
            if (targets.Count > 0 && targets[0] == null)
            {
                Debug.LogWarning(name + " has lost targets!");
                return;
            }

            CheckTargets();

            if (canShoot)
            {
                timer += Time.deltaTime;
                if (timer >= secondsBetweenShots)
                    Shoot();
            }
        }

        Vector3 directionToTarget;
        float targetAngle;
        void CheckTargets()
        {
            if (hasTargets)
            {
                if (targets.Count == 0 && isRotating)
                {
                    StopCoroutine(RotateToFaceTarget());
                    return;
                }

                if (targets.Count > 0)
                {
                    shorterTargetDistance = Vector3.Distance(transform.position, targets[0].position);
                    currentTarget = targets[0];
                    if (targets.Count > 1)
                        for (int i = 1; i < targets.Count; i++)
                        {
                            float d = Vector3.Distance(transform.position, targets[i].position);
                            if (d < shorterTargetDistance)
                            {
                                shorterTargetDistance = d;
                                currentTarget = targets[i];
                            }
                        }
                    directionToTarget = (currentTarget.position - transform.position).normalized;
                    targetAngle = 90 - Mathf.Atan2(directionToTarget.z, directionToTarget.x) * Mathf.Rad2Deg;

                    if (!isRotating)
                        StartCoroutine(RotateToFaceTarget());
                }
            }
        }

        float angle;
        bool isRotating = false;
        IEnumerator RotateToFaceTarget()
        {
            isRotating = true;
            while (Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle) > 0.05f || Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle) < -0.05f)
            {
                angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * turnRateAnglesPerSecond);
                transform.eulerAngles = Vector3.up * angle;
                yield return null;
            }
            isRotating = false;
        }

        #region API

        /// <summary>
        /// Funzione che instanzia un proiettile
        /// </summary>
        public virtual void Shoot()
        {
            if (usesObjectPooler)
            {
                ObjectPooler.Instance.GetPoolableFromPool(projectilePoolTag, shootPoint.position, shootPoint.rotation);
            }
            else
            {
                BaseEntity instBullet = Instantiate(projectilePrefab.gameObject, shootPoint.position, shootPoint.rotation).GetComponent<BaseEntity>();
                instBullet.SetUpEntity();
            }

            timer = 0;
            OnShoot.Invoke(secondsBetweenShots);
        }

        public void SetTarget(Transform _target)
        {
            targets.Clear();
            targets.Add(_target);
            canShoot = true;
        }

        public void SetTargets(Transform[] _targets)
        {
            targets.Clear();
            targets = _targets.ToList();
            canShoot = true;
        }

        public void SetTargets(List<Transform> _targets)
        {
            targets.Clear();
            targets = _targets;
            canShoot = true;
        }

        public void ResetTargets()
        {
            targets.Clear();
            currentTarget = null;
            canShoot = false;
        }

        public void FindTargets()
        {
            List<ShootTarget> sts = new List<ShootTarget>();
            sts = FindObjectsOfType<ShootTarget>().ToList();
            foreach (ShootTarget st in sts)
            {
                targets.Add(st.transform);
            }
        }

        #endregion

    }
}