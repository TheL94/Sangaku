﻿using UnityEngine;

namespace Sangaku
{
    public class DamageReceiverBehaviour : BaseBehaviour
    {
        #region Events
        public UnityIntEvent OnHealthChanged;
        [SerializeField] UnityVoidEvent OnHealthDepleated; 
        #endregion

        protected override void CustomSetup()
        {
            _currentHealth = maxHealth;
            print(name + "health setup done!");
        }

        [SerializeField] int maxHealth;
        int _currentHealth;
        int CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                if (_currentHealth != value)
                {
                    _currentHealth = value;
                    OnHealthChanged.Invoke(_currentHealth);
                }
            }
        }

        /// <summary>
        /// Funzione che aggiunge o sottrae salute
        /// </summary>
        /// <param name="_value">la salute da aggiungere o sottrarre</param>
        public void SetHealth(int _value)
        {
            if (!IsSetupped)
                return;
            int tempHealth = CurrentHealth;
            tempHealth += _value;
            if (tempHealth <= 0)
            {
                tempHealth = 0;
                OnHealthDepleated.Invoke();
                print(name + " fucking died.");
            }
            if (tempHealth > maxHealth)
                tempHealth = maxHealth;
            CurrentHealth = tempHealth;
        }

        public float GetHealth()
        {
            return maxHealth;
        }
    } 
}