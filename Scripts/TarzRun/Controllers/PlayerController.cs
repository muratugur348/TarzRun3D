using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TarzRun.Managers;
using RootMotion.FinalIK;
using TarzRun.UI;

namespace TarzRun.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        // Components
        private Animator _anim;
        private Rigidbody _rb;
        private LineRenderer _line;

        [Header("Variables")]
        public float moveForwardSpeed;
        public float moveHorizontalSpeed;
        public float slingShootMoveSpeed;

        [Header("Prefabs")]
        public GameObject bullet;
        public GameObject smokeExplosion;

        [Header("Child Objects")]
        public GameObject sling;
        public GameObject rightHandTarget;
        public GameObject slingTargetPoint;
        public GameObject targetFirstPoint;
        public GameObject targetFinishPoint;
        public GameObject slingHandleLeft;
        public GameObject slingHandleRight;
        public GameObject rightHandThumb;
        public GameObject slingHandleMovePositions;

        // Privates
        private bool _mouseDown = false;
        private bool _isMoveFinished = true;
        private bool _isHolding = false;
        private bool _isHandMovingBack = false;
        private bool _isOnTheGround = false;
        private bool _chasingOnce = false;
        private bool _isHandStarted = false;

        private Vector2 _secondPressPos;
        private Vector2 _currentSwipe;
        private Vector2 _firstPressPos;
        
        private RaycastHit objectHit;
        
        private int _slingHandleMoveOrder=3;
        
        private float _counterForMoveHand=0;
        private float _oldCurrX, _oldCurrY;
        private float _counterInMouseDown;
        
        private GameObject _slingTarget;
        private GameObject _currBullet;
        
        private CapsuleCollider _capsuleCollider;
        // Start is called before the first frame update
        void Start()
        {
            DOTween.Init();
            _anim = GetComponent<Animator>();
            _rb = GetComponentInParent<Rigidbody>();
            _line = transform.parent.GetComponent<LineRenderer>();
            _slingTarget = GameObject.FindGameObjectWithTag("SlingTarget");
            _capsuleCollider = GetComponentInParent<CapsuleCollider>();
            
        }

        private void Update()
        {
            if (!LevelManager.Instance.isGameFinished && _anim.GetBool("gameStarted") && Time.timeScale==1)
            {
                GetMouseInput();
                if (!LevelManager.Instance.isChasing)
                {
                    SlingShotAnimation();
                    MoveAim();
                }
                else
                JumpMovement();
            }
        }

        private void FixedUpdate()
        {
            if (LevelManager.Instance.isChasing && !LevelManager.Instance.isGameFinished && Time.timeScale==1) 
            {
                MoveHorizontal();
                ChangeToThirdPerson();
            }
        }
        public void StartGame()
        {
            _anim.SetBool("gameStarted", true);
            StartMove();

        }
        private void GetMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseDown = true;
                _firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                if (!LevelManager.Instance.isChasing)
                {
                    if(!_isHandMovingBack)
                    _currBullet= Instantiate(bullet, slingHandleLeft.transform.position, transform.rotation);
                    if (_isMoveFinished)
                    {
                        _isHandStarted = true;
                        _counterForMoveHand = 0;
                        StartCoroutine(MoveHandAnimation());
                    }
                    if (_isHandStarted)
                    {
                        LevelManager.Instance.isHolding = true;
                        _isHolding = true;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _mouseDown = false;
                _currentSwipe.x = 0;
                _isHolding = false;
                LevelManager.Instance.isHolding = false;
            }
        }
        public void NormalizeEverything()
        {
            _line.enabled = false;
            Destroy(_currBullet);
            GetComponentInParent<FullBodyBipedIK>().solver.IKPositionWeight = 0;
            GetComponentInParent<LimbIK>().solver.IKPositionWeight = 0;
            _currentSwipe = Vector2.zero;
            _secondPressPos = Vector2.zero;
        }
        #region movement
        private void StartMove()
        {
            LevelManager.Instance.isGamePlayable=true;
            StartCoroutine(MoveForward());
        }
        IEnumerator MoveForward()
        {
            if (!LevelManager.Instance.isGameFinished)
            {
                    _rb.velocity = new Vector3(_rb.velocity.x,_rb.velocity.y, 1* moveForwardSpeed) ;
                yield return new WaitForFixedUpdate();
                StartCoroutine(MoveForward());
            }
            else
                _rb.velocity = Vector3.zero;
        }
        private void MoveHorizontal()
        {
            if (LevelManager.Instance.isGamePlayable && _anim.GetBool("gameStarted") == true)
            {
                
                if (_mouseDown)
                {
                    //save ended touch 2d point
                    _secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    //create vector from the two points
                    _currentSwipe = new Vector2(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);
                    //normalize the 2d vector
                    _currentSwipe /= Screen.width;
                    if (_currentSwipe.y > 0.20f && _isOnTheGround && !_anim.GetBool("isRolling"))
                    {
                        _rb.AddForce(Vector3.up * 17000);
                        StartCoroutine(CloseAnimatorRunning());
                        _anim.SetBool("isJumping", true);
                        _anim.SetBool("runRight", false);
                        _anim.SetBool("runLeft", false);
                        _anim.SetBool("isRunning", false);
                        _mouseDown = false;
                    }
                    else if(_currentSwipe.y<-0.2f && _isOnTheGround && !_anim.GetBool("isJumping") && !_anim.GetBool("isRolling"))
                    {
                        StartCoroutine(CloseAnimatorRolling());
                        _anim.SetBool("isRolling", true);
                        _anim.SetBool("isJumping", false);
                        _anim.SetBool("runRight", false);
                        _anim.SetBool("runLeft", false);
                        _anim.SetBool("isRunning", false);
                        _mouseDown = false;
                    }

                    else if (_currentSwipe.x > 0.05f)
                    {
                        Vector3 rgt = transform.TransformDirection(Vector3.right);
                        if (Physics.Raycast(transform.position, rgt, out objectHit, 10))
                        {
                            if (objectHit.transform.CompareTag("Wall") && objectHit.distance > .5f)
                            {
                                _rb.velocity = new Vector3(_currentSwipe.x*moveHorizontalSpeed, _rb.velocity.y, moveForwardSpeed);
                            }
                            else if(objectHit.transform.CompareTag("Wall") && objectHit.distance<=.5f)
                            {
                                _rb.velocity = new Vector3(0, _rb.velocity.y, moveForwardSpeed);
                            }
                            else if (!objectHit.transform.CompareTag("Wall"))
                            {
                                _rb.velocity = new Vector3(_currentSwipe.x * moveHorizontalSpeed, _rb.velocity.y, moveForwardSpeed);
                                //_rb.AddForce(Vector3.right * moveHorizontalSpeed * 0.3f / 1, ForceMode.Force);

                            }
                        }
                        if (_anim.GetBool("runRight") == false)
                        {
                            _anim.SetBool("runRight", true);
                            _anim.SetBool("runLeft", false);
                            _anim.SetBool("isRunning", false);
                            //_anim.SetBool("isJumping", false);
                        }
                    }
                    else if (_currentSwipe.x < -0.05f)
                    {
                        Vector3 lft = transform.TransformDirection(-Vector3.right);
                        if (Physics.Raycast(transform.position, lft, out objectHit, 10))
                        {
                            if (objectHit.transform.CompareTag("Wall") && objectHit.distance > .5f)
                            {
                                _rb.velocity = new Vector3(_currentSwipe.x*moveHorizontalSpeed, _rb.velocity.y, moveForwardSpeed);
                                //_rb.AddForce(-Vector3.right * moveHorizontalSpeed * Mathf.Abs(_currentSwipe.x) / 1, ForceMode.Impulse);
                            }
                            else if (objectHit.transform.CompareTag("Wall") && objectHit.distance <= .5f)
                            {
                                _rb.velocity = new Vector3(0, _rb.velocity.y, moveForwardSpeed);
                            }
                            else if (!objectHit.transform.CompareTag("Wall"))
                            {
                                _rb.velocity = new Vector3(_currentSwipe.x * moveHorizontalSpeed, _rb.velocity.y, moveForwardSpeed);
                                //_rb.AddForce(-Vector3.right * moveHorizontalSpeed * 0.3f / 1, ForceMode.Impulse);

                            }

                        }
                        if (_anim.GetBool("runLeft") == false)
                        {
                            _anim.SetBool("runLeft", true);
                            _anim.SetBool("runRight", false);
                            _anim.SetBool("isRunning", false);
                            //_anim.SetBool("isJumping", false);
                        }
                    }
                    else
                    {
                        if (_anim.GetBool("isRunning") == false)
                        {
                            _anim.SetBool("isRunning", true);
                            _anim.SetBool("runLeft", false);
                            _anim.SetBool("runRight", false);
                           // _anim.SetBool("isJumping", false);
                        }
                    }

                }
                else
                {
                    if (_anim.GetBool("isRunning") == false)
                    {
                        _anim.SetBool("isRunning", true);
                        _anim.SetBool("runLeft", false);
                        _anim.SetBool("runRight", false);
                    }
                }
            }
        }
        private void MoveAccordingToBorder()
        {
            Vector3 rgt = transform.TransformDirection(Vector3.right);
            if (Physics.Raycast(transform.position, rgt, out objectHit, 10))
            {
                if (objectHit.transform.CompareTag("Wall") && objectHit.distance < .5f)
                {
                    _rb.velocity = new Vector3(0, _rb.velocity.y, moveForwardSpeed);
                }
            }
            Vector3 lft = transform.TransformDirection(-Vector3.right);
            if (Physics.Raycast(transform.position, lft, out objectHit, 10))
            {
                if (objectHit.transform.CompareTag("Wall") && objectHit.distance < .5f)
                {
                    _rb.velocity = new Vector3(0, _rb.velocity.y, moveForwardSpeed);
                }
            }
        }
        private void JumpMovement()
        {
            if (transform.position.y < 0.03f)
            {
                MoveAccordingToBorder();
                if (!_isOnTheGround)
                    _isOnTheGround = true;
                else if (!_mouseDown && _anim.GetBool("isJumping") == true)
                {
                    _anim.SetBool("isRunning", true);
                    _anim.SetBool("runLeft", false);
                    _anim.SetBool("runRight", false);
                    _anim.SetBool("isJumping", false);
                }
            }
            else if (transform.position.y >= 0.03f)
            {
                MoveAccordingToBorder();
                if (!_anim.GetBool("isJumping"))
                {
                    _anim.SetBool("isJumping", true);

                }
                _isOnTheGround = false;

            }
        }
        private void ChangeToThirdPerson()
        {
            if (!_chasingOnce)
            {
                NormalizeEverything();
                _anim.SetLayerWeight(1, 1);
                _chasingOnce = true;
            }
        }
        #endregion
        #region fire
        IEnumerator MoveHandAnimation()
        {
            _isMoveFinished = false;
            _counterForMoveHand += Time.deltaTime;
            rightHandTarget.transform.position = Vector3.MoveTowards(rightHandTarget.transform.position, 
                slingTargetPoint.transform.position, Time.deltaTime /3);
            if (_counterForMoveHand < 0.5f)
            {
                yield return new WaitForFixedUpdate();
                if(!_isHandStarted)
                _isHandStarted = true;
                StartCoroutine(MoveHandAnimation());
            }
            else
            {
                if (!_line.enabled && _isHolding)
                {
                    _line.enabled = true;
                }
                if (_isHandStarted)
                _isHandStarted = false;
                _isHandMovingBack = true;
                rightHandTarget.LeanMoveLocal(targetFirstPoint.transform.localPosition, .5f);
                yield return new WaitForSeconds(1f);
                _isMoveFinished = true;
                _isHandMovingBack = false;
            }
        }
        private void SlingShotAnimation()
        {
            
            if (_isHolding && !_isHandStarted)
            {
                slingHandleRight.transform.position = rightHandThumb.transform.position;
                slingHandleLeft.transform.position = rightHandThumb.transform.position;
                _slingHandleMoveOrder = 0;
            }
            else
            {
                if (_slingHandleMoveOrder == 0)
                {


                    slingHandleRight.transform.localPosition = Vector3.MoveTowards(slingHandleRight.transform.localPosition,
                    slingHandleMovePositions.transform.GetChild(0).transform.localPosition, Time.deltaTime * 10);
                    slingHandleLeft.transform.localPosition = Vector3.MoveTowards(slingHandleLeft.transform.localPosition,
                    slingHandleMovePositions.transform.GetChild(0).transform.localPosition, Time.deltaTime * 10);
                    if (slingHandleRight.transform.localPosition == slingHandleMovePositions.transform.GetChild(0).transform.localPosition)
                        _slingHandleMoveOrder++;
                }
                else if (_slingHandleMoveOrder == 1)
                {

                    slingHandleRight.transform.localPosition = Vector3.MoveTowards(slingHandleRight.transform.localPosition,
                    slingHandleMovePositions.transform.GetChild(1).transform.localPosition, Time.deltaTime * 10);
                    slingHandleLeft.transform.localPosition = Vector3.MoveTowards(slingHandleLeft.transform.localPosition,
                    slingHandleMovePositions.transform.GetChild(1).transform.localPosition, Time.deltaTime * 10);
                    if (slingHandleRight.transform.localPosition == slingHandleMovePositions.transform.GetChild(1).transform.localPosition)
                        _slingHandleMoveOrder++;
                }
                else if (_slingHandleMoveOrder == 2)
                {

                    slingHandleRight.transform.localPosition = Vector3.MoveTowards(slingHandleRight.transform.localPosition,
                    slingHandleMovePositions.transform.GetChild(2).transform.localPosition, Time.deltaTime * 10);
                    slingHandleLeft.transform.localPosition = Vector3.MoveTowards(slingHandleLeft.transform.localPosition,
                    slingHandleMovePositions.transform.GetChild(2).transform.localPosition, Time.deltaTime * 10);
                    if (slingHandleRight.transform.localPosition == slingHandleMovePositions.transform.GetChild(2).transform.localPosition)
                        _slingHandleMoveOrder++;
                }
                else if (_slingHandleMoveOrder == 3)
                {
                    slingHandleRight.transform.localPosition = Vector3.MoveTowards(slingHandleRight.transform.localPosition,
                    slingHandleMovePositions.transform.GetChild(3).transform.localPosition, Time.deltaTime * 5);
                    slingHandleLeft.transform.localPosition = Vector3.MoveTowards(slingHandleLeft.transform.localPosition,
                    slingHandleMovePositions.transform.GetChild(3).transform.localPosition, Time.deltaTime * 5);
                }
            }
        }
        private void MoveAim()
        {
            if (_isHolding)
            {
                
                if (_mouseDown)
                {
                    DrawPath();
                    _counterInMouseDown += Time.deltaTime;
                    //save ended touch 2d point
                    _secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    //create vector from the two points
                    _currentSwipe = new Vector2(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);
                    //normalize the 2d vector
                    _currentSwipe /= Screen.width;
                    if (_counterInMouseDown > 0.25f)
                    {
                        if (_secondPressPos.x != _oldCurrX)
                        {
                            _firstPressPos = _secondPressPos;
                            _oldCurrX = _firstPressPos.x;
                            _oldCurrY = _firstPressPos.y;

                        }
                        if (_secondPressPos.y != _oldCurrY)
                        {
                            _firstPressPos = _secondPressPos;
                            _oldCurrY = _firstPressPos.y;
                            _oldCurrX = _firstPressPos.x;


                        }
                        _counterInMouseDown = 0;
                    }

                    if ((rightHandTarget.transform.localPosition.x < -0.1f && _currentSwipe.x > 0) ||
                        (rightHandTarget.transform.localPosition.x > 0.1f && _currentSwipe.x < 0)||
                        (rightHandTarget.transform.localPosition.x > -0.1f && rightHandTarget.transform.localPosition.x < 0.1f))
                    {
                        rightHandTarget.transform.localPosition += new Vector3(_currentSwipe.x, 0, 0)* slingShootMoveSpeed;
                    }
                    if ((rightHandTarget.transform.localPosition.y < 1.3f && _currentSwipe.y > 0) ||
                        (rightHandTarget.transform.localPosition.y > 1.4f && _currentSwipe.y < 0) ||
                        (rightHandTarget.transform.localPosition.y > 1.3f && rightHandTarget.transform.localPosition.y < 1.4f))
                    {
                        rightHandTarget.transform.localPosition += new Vector3(0, _currentSwipe.y, 0)* slingShootMoveSpeed;
                    }

                }
            }
            else
            {
                /*if (_line.enabled)
                {
                    _line.enabled = false;
                }*/
                rightHandTarget.transform.position = Vector3.MoveTowards(rightHandTarget.transform.position,
                targetFinishPoint.transform.position, Time.deltaTime / 3);
            }
        }

        
        
        #endregion
       IEnumerator CloseAnimatorRunning()
        {
            yield return new WaitForSeconds(1.1f);
            _anim.SetBool("isJumping", false);
        }
        IEnumerator CloseAnimatorRolling()
        {
            _capsuleCollider.center = new Vector3(_capsuleCollider.center.x, 0.3781418f, _capsuleCollider.center.z);
            _capsuleCollider.height = 0.8f;
            yield return new WaitForSeconds(1.4f);
            _capsuleCollider.center = new Vector3(_capsuleCollider.center.x, 0.7914226f, _capsuleCollider.center.z);
            _capsuleCollider.height = 1.63f;
            _anim.SetBool("isRolling", false);
            _anim.SetBool("isRunning", true);
        }
        void DrawPath()
        {
            _line.SetPosition(0, slingHandleLeft.transform.position);
            _line.SetPosition(1, _slingTarget.transform.position);
            
        }
        public void DeathAnimation()
        {
            _anim.SetBool("isDeath", true);
        }
        public void DeathSound()
        {
            GetComponent<AudioSource>().Play();
        }
        public void WinAnimation()
        {
            NormalizeEverything();
            transform.LookAt(GameObject.FindGameObjectWithTag("Tarzan").transform);
            _anim.SetBool("isWin", true);
        }
        public void DefeatAnimation()
        {
            _anim.SetBool("isDefeated", true);
        }

        public IEnumerator EnterAttackMode()
        {
            if (!_anim.GetBool("isDeath"))
            {
                transform.parent.DOMove(new Vector3(-0.1f, 0, transform.position.z+3), 1).SetEase(Ease.Linear);
                yield return new WaitForSeconds(1);
                _anim.SetBool("isRunning", false);
                _anim.SetBool("runLeft", false);
                _anim.SetBool("runRight", false);
                _anim.SetBool("isJumping", false);
                _anim.SetBool("isRolling", false);
                _anim.SetBool("isBackAttackMode", true);
                _anim.SetLayerWeight(1, 0);
                GetComponentInParent<FullBodyBipedIK>().solver.IKPositionWeight = 1;
                GetComponentInParent<LimbIK>().solver.IKPositionWeight = 1;
            }
        }
    }
}