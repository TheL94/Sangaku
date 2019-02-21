﻿using UnityEngine;
using UnityEngine.Events;

namespace Sangaku
{
    /// <summary>
    /// Behaviour che gestisce l'input del player
    /// </summary>
    public class PlayerInputBehaviour : BaseBehaviour
    {
        #region Events
        /// <summary>
        /// Evento lanciato alla pressione del bottone di dash
        /// </summary>
        [SerializeField] UnityVector3Event OnDashPressed;
        /// <summary>
        /// Evento lanciato alla pressione del bottone di shot
        /// </summary>
        [SerializeField] UnityEvent OnShotPressed;
        /// <summary>
        /// Evento lanciato al cambio di direzione dell'asse di input
        /// </summary>
        [SerializeField] UnityVector3Event OnDirectionUpdate;
        #endregion

        /// <summary>
        /// Enumerativo che indica il tipo di movimento che si vuole
        /// </summary>
        public enum DirectionType { Global, Camera };
        public DirectionType InputDirection;

        /// <summary>
        /// Direzione in cui viene mosso l'asse di input della direzione
        /// </summary>
        Vector3 _moveDirection;
        /// <summary>
        /// Propery che lancia un evento al cambio di direzione dell'input
        /// </summary>
        Vector3 MoveDirection
        {
            get { return _moveDirection; }
            set
            {
                if (_moveDirection != value)
                {
                    _moveDirection = value;
                    OnDirectionUpdate.Invoke(_moveDirection); 
                }
            }
        }
        /// <summary>
        /// Riferimento alla camera
        /// </summary>
        Camera cam;

        bool canShoot;
        bool canMove;
        bool canDash;

        #region API
        public void ToggleShootInput(bool _value)
        {
            canShoot = _value;
        }
        public void ToggleMovementInput(bool _value)
        {
            canMove = _value;
            MoveDirection = Vector3.zero;
        }
        public void ToggleDashInput(bool _value)
        {
            canDash = _value;
        }
        #endregion

        /// <summary>
        /// Custom setup del Behaviour
        /// </summary>
        protected override void CustomSetup()
        {
            cam = Camera.main;

            canShoot = true;
            canMove = true;
            canDash = true;
        }

        /// <summary>
        /// Update
        /// </summary>
        void Update()
        {
            if (IsSetupped)
            {
                if (CheckControllerConnection())
                    ReadControllerInput();
                else
                    ReadKeyboardInput();
            }
        }

        #region Controller Inputs
        [Header("Controller")]
        /// <summary>
        /// Tasto che corrisponde allo shot per il controlller
        /// </summary>
        [SerializeField] string controllerShotInput = "ShotController";
        /// <summary>
        /// Tasto che corrisponde al dash per il controlller
        /// </summary>
        [SerializeField] string controllerDashInput = "DashController";
        /// <summary>
        ///  Asse che corrisponde al movimento orizzontale per il controller
        /// </summary>
        [SerializeField] string controllerHorizontalInput = "HorizontalLeftController";
        /// <summary>
        /// Asse che corrisponde al movimento verticale per il controller
        /// </summary>
        [SerializeField] string controllerVerticalInput = "VerticalLeftController";

        /// <summary>
        /// Variabile che salva il valore al frame precedente dell'asse di shot
        /// </summary>
        int cShotInputPrevValue;
        /// <summary>
        /// Funzione che si occupa di leggere l'input del controller
        /// </summary>
        void ReadControllerInput()
        {
            //Move Input
            if (canMove)
                CalculateMoveDirection(Input.GetAxis(controllerHorizontalInput), Input.GetAxis(controllerVerticalInput));

            //Shoot Input
            if (canShoot)
            {
                int shotAxis = (int)Input.GetAxis(controllerShotInput);
                if (shotAxis == 1 && cShotInputPrevValue == 0)
                {
                    OnShotPressed.Invoke();
                    cShotInputPrevValue = 1;
                }
                else if (shotAxis == 0 && cShotInputPrevValue == 1)
                    cShotInputPrevValue = 0;            
            }

            //DashInput
            if (canDash && Input.GetButtonDown(controllerDashInput))
                OnDashPressed.Invoke(MoveDirection);
        }
        #endregion

        #region Keyboard Inputs
        [Header("Keyboard")]
        /// <summary>
        /// Tasto che corrisponde allo shot per la tastiera
        /// </summary>
        [SerializeField] string keyboardShotInput = "ShotKeyboard";
        /// <summary>
        /// Tasto che corrisponde al dash per la tastiera
        /// </summary>
        [SerializeField] string keyboardDashInput = "DashKeyboard";
        /// <summary>
        /// Asse che corrisponde al movimento orizzontale per la tastiera
        /// </summary>
        [SerializeField] string keyboardHorizontalInput = "HorizontalKeyboard";
        /// <summary>
        /// Asse che corrisponde al movimento verticale per la tastiera
        /// </summary>
        [SerializeField] string keyboardVerticalInput = "VerticalKeyboard";

        /// <summary>
        /// Funzione che si occupa di leggere l'input della tastiera
        /// </summary>
        void ReadKeyboardInput()
        {
            //Move Input
            if (canMove)
                CalculateMoveDirection(Input.GetAxisRaw(keyboardHorizontalInput), Input.GetAxisRaw(keyboardVerticalInput));

            //Shoot Input
            if (canShoot && Input.GetButtonDown(keyboardShotInput))
                OnShotPressed.Invoke();

            //DashInput
            if (canDash && Input.GetButtonDown(keyboardDashInput))
                OnDashPressed.Invoke(MoveDirection);
        }
        #endregion

        /// <summary>
        /// Funzione che controlla se è connesso un controller
        /// </summary>
        /// <returns>Ritorna true se è connesso, false altrimenti</returns>
        bool CheckControllerConnection()
        {
            string[] controllerNames = Input.GetJoystickNames();

            if (controllerNames.Length == 0)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < controllerNames.Length; i++)
                {
                    if (!string.IsNullOrEmpty(controllerNames[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Variabile temporanea per il calcolo del movimento in base alla camera
        /// </summary>
        Vector3 cameraBasedDirection;
        /// <summary>
        /// Funzione che calcola il vettore di movimento dati due assi di input
        /// </summary>
        /// <param name="_horizontalInput"></param>
        /// <param name="_verticalInput"></param>
        void CalculateMoveDirection(float _horizontalInput, float _verticalInput)
        {
            if (InputDirection == DirectionType.Global)
                MoveDirection = new Vector3(_horizontalInput, 0f, _verticalInput).normalized;
            else if (InputDirection == DirectionType.Camera && cam != null)
            {
                MoveDirection = new Vector3(_horizontalInput, 0f, _verticalInput).normalized;
                cameraBasedDirection = cam.transform.TransformDirection(MoveDirection);
                MoveDirection = new Vector3(cameraBasedDirection.x, MoveDirection.y, cameraBasedDirection.z).normalized;
            }
        }
    }
}
