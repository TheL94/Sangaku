﻿using UnityEngine;
using UnityEngine.Events;

namespace Deirin.Utility
{
    public class Timer : MonoBehaviour
    {
        public float time;
        public bool startCountinOnAwake = false;
        public bool repeat = true;

        public UnityEvent OnTimerEnd;

        bool countTime = false;
        float timer;

        #region MonoBehaviour methods

        private void OnEnable()
        {
            if(repeat)
                OnTimerEnd.AddListener(ResetTimer);
        }

        private void OnDisable()
        {
            if(repeat)
                OnTimerEnd.RemoveListener(ResetTimer);
        }

        private void Awake()
        {
            if (startCountinOnAwake)
                StartTimer(time);
        }

        void Update()
        {
            if (countTime)
                timer += Time.deltaTime;
            if (timer >= time)
                OnTimerEnd.Invoke();
        }

        #endregion

        #region API

        public void StartTimer(float _time)
        {
            time = _time;
            countTime = true;
        }

        public void StartTimer()
        {
            timer = 0;
            countTime = true;
        }

        public void StopTimer()
        {
            timer = 0;
            countTime = false;
        }

        public void PauseTimer()
        {
            countTime = false;
        }

        public void ResetTimer()
        {
            timer = 0;
            countTime = true;
        }

        #endregion

    }

}