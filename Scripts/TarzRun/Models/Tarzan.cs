
using System.Collections;
using System.Collections.Generic;
using TarzRun.Controllers;
using TarzRun.Managers;
using UnityEngine;

namespace TarzRun
{
    public class Tarzan : MonoBehaviour
    {
        public GameObject ropeTarget;
        public GameObject ivy;
        public GameObject leftArm;
        public GameObject[] rocks;
        public GameObject handle;
        public float attackModeTime;
        public float leftAttackPlayerTime;
        private Animator _anim;
        private int _animStep = 0;
        private int _randomX;
        private LineRenderer _line;
        private float h = 3;
        private float _gravity = -18;
        private float counterToMoveOtherPosition;
        private float moveToOtherPosition = 0.035f;
        private float distance;
        private float _counterInAttackMode;
        
        private int currPosition;
        private Vector3[] positions;
        private float speed;
        private GameObject _player;
        private GameObject _currentRock;
        private bool _isDeath;
        private bool _moveFinished = false;
        public int _totalSwing;
        // Start is called before the first frame update
        void Start()
        {
            _anim = GetComponent<Animator>();
            _line = GetComponent<LineRenderer>();
            _player = GameObject.FindGameObjectWithTag("Player");
            positions = new Vector3[_line.GetComponent<LineRenderer>().positionCount];
            //StartGame();
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void StartGame()
        {
            StartCoroutine(RunToTarget(new Vector3(4, 4, 7.6f), 1));
        }
        #region Movement
        IEnumerator RunToTarget(Vector3 target, float time)
        {
            _anim.SetBool("isRunning", true);
            _anim.SetBool("isJumping", false);
            _anim.SetBool("isClimbing", false);
            _anim.SetBool("isIdle", false);
            ivy.gameObject.SetActive(false);
            transform.LeanMove(target, time);
            yield return new WaitForSeconds(time);
            ivy.gameObject.SetActive(true);
            if (_animStep == 0)
            {
                _animStep++;
                StartCoroutine(JumpToTarget(new Vector3(4, 4, 11.5f), .8f));
            }
            else if (_animStep == 2)
            {
                _animStep++;
                FindObjectOfType<CameraController>().LookPoint();
                _player.GetComponentInChildren<PlayerController>().StartGame();
                DrawPath(new Vector3(-3.5f, 4f, 21.83f));

                _anim.SetBool("isClimbing", true);
                _anim.SetBool("isJumping", false);
                _anim.SetBool("isRunning", false);
                _anim.SetBool("isIdle", false);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,
                        -45, transform.eulerAngles.z);
                currPosition = 0;
                if (!_isDeath)
                StartCoroutine(MoveToPath());

            }
            else if (_animStep % 2 == 1)
            {
                LeftSide();
            }
            else if (_animStep % 2 == 0)
            {
                RightSide();
            }
        }
        private void LeftSide()
        {
            _totalSwing++;
            _animStep++;
            if (_totalSwing >3)
            {
                if (!LevelManager.Instance.isChasing)
                {
                    LevelManager.Instance.isChasing = true;
                    StartCoroutine(StartCounting());
                    StartCoroutine(UIManager.Instance.OpenSwerve());
                }
                DrawPath(new Vector3(8, 4, transform.position.z + 5));
                _randomX = Random.Range(-170, 200);
                if(_counterInAttackMode< leftAttackPlayerTime)
                _currentRock = Instantiate(rocks[Random.Range(0, 6)], transform.position, transform.rotation);
            }
            else if(_totalSwing==-1)
            {
                moveToOtherPosition /= 1.5f;
                _moveFinished = true;
                StartCoroutine(MoveToFinishPoint(new Vector3(0, 0.11f, transform.position.z + 2)));
            }
            else
            {
                DrawPath(new Vector3(4, 4, transform.position.z + 7));

            }
            _anim.SetBool("isClimbing", true);
            _anim.SetBool("isJumping", false);
            _anim.SetBool("isRunning", false);
            _anim.SetBool("isIdle", false);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x,
                45, transform.eulerAngles.z);
            currPosition = 0;

            if (!_isDeath && !_moveFinished)
                StartCoroutine(MoveToPath());


        }
        private void RightSide()
        {
            _totalSwing++;
            _animStep++;
            if (_totalSwing > 3)
            {
                if (!LevelManager.Instance.isChasing)
                {
                    LevelManager.Instance.isChasing = true;
                    StartCoroutine(UIManager.Instance.OpenSwerve());
                    StartCoroutine(StartCounting());
                }
                DrawPath(new Vector3(-8, 4, transform.position.z + 5));
                _randomX = Random.Range(-170, 200);
                if(_counterInAttackMode< leftAttackPlayerTime)
                _currentRock = Instantiate(rocks[Random.Range(0, 6)], transform.position, transform.rotation);
            }
            else
            {

                DrawPath(new Vector3(-4, 4, transform.position.z + 5));
            }
            _anim.SetBool("isClimbing", true);
            _anim.SetBool("isJumping", false);
            _anim.SetBool("isRunning", false);
            _anim.SetBool("isIdle", false);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x,
                -45, transform.eulerAngles.z);
            currPosition = 0;
            if (!_isDeath)
                StartCoroutine(MoveToPath());

        }
        IEnumerator JumpToTarget(Vector3 target, float time)
        {
            _anim.SetBool("isJumping", true);
            _anim.SetBool("isRunning", false);
            _anim.SetBool("isClimbing", false);
            _anim.SetBool("isIdle", false);
            transform.LeanMove(target, time);
            yield return new WaitForSeconds(time);
            if (_animStep == 1)
            {
                _animStep++;
                StartCoroutine(RunToTarget(new Vector3(4, 4, 14.6f), .8f));
            }
        }
        IEnumerator MoveToPath()
        {
            if (!_isDeath)
            {
                counterToMoveOtherPosition += Time.deltaTime;

                if (counterToMoveOtherPosition >= moveToOtherPosition)
                {
                    counterToMoveOtherPosition = 0;
                    currPosition++;
                    if (_animStep % 2 == 1)
                    {
                        if (LevelManager.Instance.isChasing && transform.position.x > _randomX / 100 && _counterInAttackMode< leftAttackPlayerTime)
                        {
                            _currentRock.transform.position = leftArm.transform.position;
                        }
                        transform.eulerAngles += new Vector3(0, 3, 0);
                    }
                    else
                    {
                        if (LevelManager.Instance.isChasing && transform.position.x < _randomX / 100 && _counterInAttackMode< leftAttackPlayerTime)
                        {
                            _currentRock.transform.position = leftArm.transform.position;
                        }
                        transform.eulerAngles += new Vector3(0, -3, 0);
                    }
                    if (currPosition < 31)
                        distance = Vector3.Distance(transform.position, positions[currPosition]);
                }
                if (currPosition <= 29)
                {
                    if (distance > 3)
                        speed = 30;
                    else
                        speed = distance * 8f;

                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(
                        positions[currPosition].x,
                        positions[currPosition].y,
                        positions[currPosition].z), speed * Time.deltaTime);
                }
                yield return new WaitForEndOfFrame();
                if (currPosition < 31)
                    StartCoroutine(MoveToPath());
                else if(!_moveFinished)
                {
                    transform.eulerAngles = Vector3.zero;
                    StartCoroutine(RunToTarget(new Vector3(transform.position.x, transform.position.y, transform.position.z + 12.5f), 2f));
                }
                else
                {
                    transform.eulerAngles = Vector3.zero;
                }
            }
        }
        IEnumerator StartCounting()
        {
            _counterInAttackMode += 1;
            yield return new WaitForSeconds(1);
            print(_counterInAttackMode);
            if (_counterInAttackMode <= attackModeTime)
                StartCoroutine(StartCounting());
            else
            {
                LevelManager.Instance.isChasing = false;
                _totalSwing = -5;
                StartCoroutine(_player.GetComponentInChildren<PlayerController>().EnterAttackMode());
            }
        }
        IEnumerator MoveToFinishPoint(Vector3 finishPoint)
        {
            transform.LeanMove(finishPoint, 1.5f);
            yield return new WaitForSeconds(1.5f);
            LevelManager.Instance.isGameFinished = true;
            LevelManager.Instance.isFailed = true;
            StartCoroutine(UIManager.Instance.CloseGamePlayCanvas());
            _player.GetComponentInChildren<PlayerController>().NormalizeEverything();
            
            ivy.gameObject.SetActive(false);

            transform.eulerAngles = Vector3.zero;
            _anim.SetBool("isClimbing", false);
            _anim.SetBool("isRunning", true);

            transform.LeanMoveZ(finishPoint.z+2, .5f);
            yield return new WaitForSeconds(.5f);
            _player.GetComponentInChildren<PlayerController>().DefeatAnimation();
            StartCoroutine(UIManager.Instance.OpenLoseCanvas());
            _anim.SetBool("isHanging",true);
            _anim.SetBool("isRunning", false);
            yield return new WaitForSeconds(1);
            handle.GetComponent<Animator>().enabled = true;
            StartCoroutine(FollowHandle());
            yield return new WaitForSeconds(5);
            _anim.SetBool("isHanging", false);
            _anim.SetBool("isRunning", true);
            transform.LeanMoveZ(transform.position.z + 80, 20);
        }
        IEnumerator FollowHandle()
        {

            transform.position = Vector3.Lerp(transform.position,
                new Vector3(transform.position.x, handle.transform.localPosition.y - 3.5f, handle.transform.position.z), 20 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            if(transform.position.z<269)
            StartCoroutine(FollowHandle());
        }
        
       
        #endregion
        #region LineRenderer
        void DrawPath(Vector3 target)
        {
            LaunchData launchData = CalculateLaunchData(target);
            Vector3 previousDrawPoint = transform.position;
            int resolution = 30;
            for (int i = 1; i <= resolution; i++)
            {
                float simulationTime = i / (float)resolution * launchData.timeToTarget;
                Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * _gravity * simulationTime * simulationTime / 2f;
                Vector3 drawPoint = transform.position + displacement;
                Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
                previousDrawPoint = drawPoint;
                Vector3 pos = _line.GetPosition(i - 1);
                _line.SetPosition(i - 1, new Vector3(drawPoint.x, 4 - Mathf.Abs(drawPoint.y - 4), drawPoint.z));
            }
            ropeTarget.transform.position = new Vector3(_line.GetPosition(15).x, _line.GetPosition(15).y + 20, _line.GetPosition(15).z);
            _line.SetPosition(0, transform.position);
            _line.SetPosition(30, _line.GetPosition(29));
            _line.GetComponent<LineRenderer>().GetPositions(positions);
        }
        struct LaunchData
        {
            public readonly Vector3 initialVelocity;
            public readonly float timeToTarget;

            public LaunchData(Vector3 initialVelocity, float timeToTarget)
            {
                this.initialVelocity = initialVelocity;
                this.timeToTarget = timeToTarget;
            }

        }
        LaunchData CalculateLaunchData(Vector3 target)
        {
            float displacementY = target.y - transform.position.y;
            Vector3 displacementXZ = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
            float time = Mathf.Sqrt(-2 * h / _gravity) + Mathf.Sqrt(2 * (displacementY - h) / _gravity);
            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * _gravity * h);
            Vector3 velocityXZ = displacementXZ / time;

            return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(_gravity), time);
        }
        #endregion
        public void Death()
        {
            _anim.SetBool("isDeath", true);
            _isDeath = true;
            if (LeanTween.isTweening(gameObject))
                LeanTween.cancelAll();
            GetComponent<Rigidbody>().useGravity = true;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ground") && _anim.GetBool("isDeath") && !_anim.GetBool("isHitGround"))
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                GetComponent<Rigidbody>().useGravity = false;
                _anim.SetBool("isHitGround", true);
            }
        }
    }
}