﻿using UnityEngine;

namespace Sangaku
{
    [RequireComponent(typeof(ManaBehaviour))]
    public class PlayerShootBehaviour : BaseBehaviour
    {
        #region Serialized Fields
        [SerializeField] protected BaseEntity projectilePrefab;
        [SerializeField] protected Transform shootPoint;
        [Tooltip("How many seconds between each shot.")]
        [SerializeField] protected float secondsBetweenShots;
        [Tooltip("Costo di un proiettile in mana")]
        [SerializeField] float cost;
        #endregion

        #region Events
        [SerializeField] protected UnityFloatEvent OnOrbShoot;
        #endregion

        ManaBehaviour mana;
        PlayerOrbInteractionBehaviour orbInteraction;

        protected override void CustomSetup()
        {
            mana = GetComponent<ManaBehaviour>();
            orbInteraction = GetComponentInChildren<PlayerOrbInteractionBehaviour>();
        }

        /// <summary>
        /// Shoot logic.
        /// </summary>
        public void Shoot()
        {
            if (orbInteraction.caughtOrb)
            {
                ShootCaughtOrb();
            }
            else
            {
                ShootOrb();
            }
        }

        /// <summary>
        /// Instantiates a new Orb.
        /// </summary>
        void ShootOrb()
        {
            if (mana.SetMana(-cost))
            {
                //----- ObjPooler wating room
                OrbController instantiatedOrb = Instantiate(projectilePrefab.gameObject, shootPoint.position, shootPoint.rotation).GetComponent<OrbController>();
                instantiatedOrb.PlayerReference = Entity as PlayerController;
                instantiatedOrb.SetUpEntity();
                OnOrbShoot.Invoke(secondsBetweenShots);
            }
        }

        /// <summary>
        /// Frees the caught orb.
        /// </summary>
        public void ShootCaughtOrb()
        {
            orbInteraction.FreeOrb();
            OnOrbShoot.Invoke(secondsBetweenShots);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(shootPoint.position, 0.3f);
        }
    }
}