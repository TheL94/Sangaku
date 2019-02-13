﻿using UnityEngine;
using System.Collections.Generic;
using System;

namespace Sangaku
{
    public class EnemyBulletCollisionBehaviour : BaseBehaviour
    {
        [SerializeField] List<MonoBehaviour> entitiesToIgnore;
        [SerializeField] bool usesTrigger = true;

        [SerializeField] UnityVoidEvent OnGenericHit;
        [SerializeField] UnityDamageReceiverEvent OnDamageReceiverHit;


        protected override void CustomSetup()
        {
            if (entitiesToIgnore == null)
            {
                Debug.LogError(name + "'s EnemyBulletCollisionBehaviour has a parameter set to null!");
                return;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!usesTrigger)
                return;

            foreach (MonoBehaviour mono in entitiesToIgnore)
            {
                if (other.GetComponent(mono.GetType()))
                    return;
            }

            DamageReceiverBehaviour _drb = other.GetComponent<DamageReceiverBehaviour>();
            if (_drb)
            {
                OnDamageReceiverHit.Invoke(_drb);
                print(other.name + "'s dmg receiver hit");
            }
            else
            {
                OnGenericHit.Invoke();
                print(other.name +" generic hit");
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (usesTrigger)
                return;

            foreach (MonoBehaviour mono in entitiesToIgnore)
            {
                if (collision.collider.GetComponent(mono.GetType()))
                    return;
            }

            DamageReceiverBehaviour _drb = collision.collider.GetComponent<DamageReceiverBehaviour>();
            if (_drb && !_drb.Entity.GetType().IsAssignableFrom(typeof(EnemyController)) && !_drb.Entity.GetType().IsAssignableFrom(typeof(OrbController)))
            {
                OnDamageReceiverHit.Invoke(_drb);
            }
            else
            {
                OnGenericHit.Invoke();
            }
        }
    }
}