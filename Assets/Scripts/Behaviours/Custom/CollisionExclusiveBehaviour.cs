﻿using System.Collections.Generic;
using UnityEngine;

namespace Sangaku
{
    /// <summary>
    /// Behaviour che permette di interfacciarsi con le collisoni di unity in modo generico, gestendo una lista escusiva di oggetti da controllare
    /// </summary>
    public class CollisionExclusiveBehaviour : BaseBehaviour
    {
        [Header("Collision Events", order = 3)]
        [SerializeField] UnityCollisionEvent OnCollisionEnterEvent;
        [SerializeField] UnityCollisionEvent OnCollisionStayEvent;
        [SerializeField] UnityCollisionEvent OnCollisionExitEvent;
        [Header("Collision Parameters", order = 4)]
        [SerializeField] List<MonoBehaviour> entitiesToIgnoreForCollision;

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!CheckEntitiesToIgnoreForCollider(entitiesToIgnoreForCollision, collision))
            {
                OnCollisionEnterEvent.Invoke(collision);
                CollisionEnter(collision); 
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!CheckEntitiesToIgnoreForCollider(entitiesToIgnoreForCollision, collision))
            {
                OnCollisionStayEvent.Invoke(collision);
                CollisionStay(collision); 
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!CheckEntitiesToIgnoreForCollider(entitiesToIgnoreForCollision, collision))
            {
                OnCollisionExitEvent.Invoke(collision);
                CollisionExit(collision); 
            }
        }

        protected virtual void CollisionEnter(Collision _collision) { }
        protected virtual void CollisionStay(Collision _collision) { }
        protected virtual void CollisionExit(Collision _collision) { }

        /// <summary>
        /// Funzione che controlla se ho colliso con un'entità da ignorare
        /// </summary>
        /// <param name="_entitiesToIgnore"></param>
        /// <param name="_collision"></param>
        /// <returns></returns>
        protected bool CheckEntitiesToIgnoreForCollider(List<MonoBehaviour> _entitiesToIgnore, Collision _collision)
        {
            if (_entitiesToIgnore == null)
                return false;

            for (int i = 0; i < _entitiesToIgnore.Count; i++)
            {
                if (_entitiesToIgnore[i] != null && _collision.collider.GetComponent(_entitiesToIgnore[i].GetType()))
                    return true;
            }
            return false;
        }
    } 
}