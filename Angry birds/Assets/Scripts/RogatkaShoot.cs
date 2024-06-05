using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class RogatkaShoot : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;

    [Header("Transform Renderers")]
    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition; 
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;
    [SerializeField] private Transform _elasticTransform;

    [Header("Rogatka Stats")]
    [SerializeField] private float _maxDistance = 3.5f;
    [SerializeField] private float _shotForce = 5f;
    [SerializeField] private float _timeBetweenBirdRespawns = 2f;
    [SerializeField] private float _elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve _elasticCurve;
    [SerializeField] private float _maxAnimationTime = 1f;

    [Header("Rogatka Scripts")]
    [SerializeField] private RogatkaArea _rogatkaArea;
    [SerializeField] private CameraManager _cameraManager;

    [Header("Bird")]
    [SerializeField] private AngryBird _angryBirdPrefab;
    [SerializeField] private float _angryBirdPositionOffset = 2f;

    /* [Header("Sounds")]
    [SerializeField] private AudioClip _elasticPulledClip;
    [SerializeField] private AudioClip[] _elasticReleasedClips;
    */
    private Vector2 _slingShotLinesPosition;
    private Vector2 _direction;
    private Vector2 _directionNormalized;

    private bool _clickedWithInArea;
    private bool _birdOnRogatka;

    private AngryBird _spawnedAngryBird;

   // private AudioSource _audioSource;

    private void Awake()
    {
        // _audioSource = GetComponent<AudioSource>();

        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        spawnAngryBird();
    }
    private void Update()
    {
        if (InputManager.WasLeftMouseButtonPressed && _rogatkaArea.isMouseInRogatkaArea())
        {
            _clickedWithInArea = true;

            if (_birdOnRogatka)
            {
                //SoundManager.instance.PlayClip(_elasticPulledClip, _audioSource);
                _cameraManager.SwitchToFollowCam(_spawnedAngryBird.transform);
            }
            
           
        }
        if (InputManager.IsLeftMouseButtonPressed && _clickedWithInArea && _birdOnRogatka) 
        {
            DrawSlingShot();

            PositionAndRotateAngryBird();
        }
        if (InputManager.WasLeftMouseButtonReleased && _birdOnRogatka && _clickedWithInArea)
        {
            if (GameManager.Instance.HasEnoughShots())
            {
                _clickedWithInArea = false;

                _birdOnRogatka = false;

                _spawnedAngryBird.LaunchBird(_direction, _shotForce);

               //  SoundManager.instance.PlayRandomClip(_elasticReleasedClips, _audioSource);

                GameManager.Instance.UseShot();

                AnimateRogatka();

                //SetLines(_centerPosition.position);

                if(GameManager.Instance.HasEnoughShots())
                {

                    StartCoroutine(SpawnAngryBirdAfterTime());
                
                }
            }
        }
        
    }
    #region Rogatka Methods
    private void DrawSlingShot() 
    {
        
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDistance);

        SetLines(_slingShotLinesPosition);

        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }
    private void SetLines(Vector2 position) 
    {

        if (!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }

        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1,_leftStartPosition.position);

        _rightLineRenderer.SetPosition(0,position);
        _rightLineRenderer.SetPosition(1,_rightStartPosition.position);
    }
    #endregion

    #region Angry Bird

    private void spawnAngryBird()
    {
        _elasticTransform.DOComplete();
        SetLines(_idlePosition.position);

        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_idlePosition.position + dir * _angryBirdPositionOffset;

        _spawnedAngryBird = Instantiate(_angryBirdPrefab, _idlePosition.position, Quaternion.identity);
        _spawnedAngryBird.transform.right = dir;

        _birdOnRogatka = true;
    }
    private void PositionAndRotateAngryBird()
    {
        _spawnedAngryBird.transform.position = _slingShotLinesPosition +_directionNormalized * _angryBirdPositionOffset;
        _spawnedAngryBird.transform.right = _directionNormalized;
    }

    private IEnumerator SpawnAngryBirdAfterTime() 
    {
        yield return new WaitForSeconds(_timeBetweenBirdRespawns); //генератор который ждет 2 секунды а потом выполнится код снизу 

        //some code
        spawnAngryBird();
        _cameraManager.SwitchToIdleCam();
    }

    #endregion

    #region Animate  Rogatka

    private void AnimateRogatka()
    {
        _elasticTransform.position = _leftLineRenderer.GetPosition(0);

        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);

        float time = dist / _elasticDivider;

        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateRogatkaLines(_elasticTransform, time));
    }

    private IEnumerator AnimateRogatkaLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time && elapsedTime < _maxAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            SetLines(trans.position);

            yield return null;

        }
    }

    #endregion
}
