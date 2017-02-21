using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MonsterLove.StateMachine;

public class GameStateMachine : MonoBehaviour {

    public ActionStateMachine actionStateMachineComponent;
    [Tooltip("Object that contains the canvas of the main screen.")]
    public GameObject startScreenCanvas;
    [Tooltip("Object that contains the canvas of the level start animation.")]
    public GameObject levelStartAnimationCanvas;
    [Tooltip("Object that contains the canvas of the gameplay UI.")]
    public GameObject gameplayUiCanvas;
    [Tooltip("Castle object in the level.")]
    public GameObject castle;
    [Tooltip("State in which the game will start.")]
    public GameStates startingState = GameStates.StartScreen;
    [Tooltip("Fade out time for the start screen graphics.")]
    public float startScreenAlphaFadeOutTime = 0.15f;
    [Tooltip("Fade in time for the gameplay sound and music.")]
    public float startScreenMusicFadeOutTime = 1f;
    [Tooltip("Fade in/out time for the level start animation graphics.")]
    public float levelStartAlphaFadeTime = 0.15f;
    [Tooltip("Fade in time for the gameplay sound and music.")]
    public float gameplaySoundFadeInTime = 2f;
    [Tooltip("Fade in time for the gameplay ui.")]
    public float gameplayUiFadeInTime = 0.15f;
    [Tooltip("Position of the camera when showing the start screen.")]
    public Transform cameraStartScreenPosition;
    [Tooltip("Time that the player walks automatically before giving control to the player.")]
    public float playerWalkingTime = 0.8f;
    public AudioSource startScreenMusic;
    public AudioSource startScreenStartSfx;
    public SoundManager soundManager;
    public PlayerInput playerInput;
    public CameraController cameraController;
    public CharacterMovement characterMovement;

    private StateMachine<ActionStates> actionStateMachine;
    private StateMachine<GameStates> gameStateMachine;
    private float globalMusicVolume;
    private float globalSfxVolume;
    /// <summary>
    /// Moment in which the last state change happened
    /// </summary>
    private float lastStateChangeTimestamp;
    /// <summary>
    /// Moment in which the fade out of the start screen music started
    /// </summary>
    private float fadeOutMusicTimestamp;
    /// <summary>
    /// Initial volume of the start music
    /// </summary>
    private float startMusicVolume;
    /// <summary>
    /// If the start screen music is being fade out
    /// </summary>
    private bool fadingOutStartMusic;

    // Use this for initialization
    void Start () {
        globalMusicVolume = soundManager.globalMusicVolume;
        globalSfxVolume = soundManager.globalSfxVolume;
        startMusicVolume = startScreenMusic.volume;
        actionStateMachine = actionStateMachineComponent.StateMachine;
        gameStateMachine = StateMachine<GameStates>.Initialize(this, startingState);
    }

    void Update()
    {
        if (fadingOutStartMusic)
        {
            float elapsedTime = Time.time - fadeOutMusicTimestamp;
            float volume = Mathf.Lerp(startMusicVolume, 0, elapsedTime / fadeOutMusicTimestamp);
            startScreenMusic.volume = volume;
            if (volume == 0)
            {
                startScreenMusic.Stop();
                fadingOutStartMusic = false;
            }
        }
    }

    void StartScreen_Enter()
    {
        // Set Camera position
        cameraController.enabled = false;
        cameraController.transform.position = cameraStartScreenPosition.position;
        // Set canvas alpha
        SetCanvasAlpha(startScreenCanvas, 1);
        // Mute game music and sfx
        soundManager.globalMusicVolume = 0;
        soundManager.globalSfxVolume = 0;
        // Disable character state
        actionStateMachine.ChangeState(ActionStates.Disabled);
        // Play Music
        startScreenMusic.Play();
        lastStateChangeTimestamp = Time.time;
    }

    void StartScreen_Update()
    {
        if (playerInput.startGame)
        {
            gameStateMachine.ChangeState(GameStates.StartScreenToAnimationTransition);
        }
    }

    void StartScreenToAnimationTransition_Enter()
    {
        // Set Camera position
        cameraController.enabled = true;
        cameraController.transform.position = cameraStartScreenPosition.position;
        // Set canvas alpha
        SetCanvasAlpha(startScreenCanvas, 1);
        // Mute game music and sfx
        soundManager.globalMusicVolume = 0;
        soundManager.globalSfxVolume = 0;
        // Disable character state
        actionStateMachine.ChangeState(ActionStates.Disabled);
        // Stop Music
        startScreenStartSfx.Play();
        fadingOutStartMusic = true;
        lastStateChangeTimestamp = Time.time;
        fadeOutMusicTimestamp = lastStateChangeTimestamp;
    }

    void StartScreenToAnimationTransition_Update()
    {
        float elapsedTime = Time.time - lastStateChangeTimestamp;
        float alpha = Mathf.Lerp(1, 0, elapsedTime / startScreenAlphaFadeOutTime);
        SetCanvasAlpha(startScreenCanvas, alpha);
        if (alpha == 0)
        {
            gameStateMachine.ChangeState(GameStates.LevelStartAnimation);
        }
    }

    void LevelStartAnimation_Enter()
    {
        // Mute game music and sfx
        soundManager.globalMusicVolume = 0;
        soundManager.globalSfxVolume = 0;
        // Enable level start animation rendering and fade in the alpha
        SetCanvasAlpha(levelStartAnimationCanvas, 0);
        EnableChildrenAnimators(levelStartAnimationCanvas, true);
        // Disable character state
        actionStateMachine.ChangeState(ActionStates.Disabled);
        lastStateChangeTimestamp = Time.time;
    }

    void LevelStartAnimation_Update()
    {
        float elapsedTime = Time.time - lastStateChangeTimestamp;
        // Slowly fade game music and sound in
        soundManager.globalMusicVolume = Mathf.Lerp(0, globalMusicVolume, elapsedTime / gameplaySoundFadeInTime);
        soundManager.globalSfxVolume = Mathf.Lerp(0, globalSfxVolume, elapsedTime / gameplaySoundFadeInTime);
        float alpha = Mathf.Lerp(0, 1, elapsedTime / levelStartAlphaFadeTime);
        SetCanvasAlpha(levelStartAnimationCanvas, alpha);
    }

    void CastleGatesOpening_Enter()
    {
        EnableChildrenAnimators(castle, true);
        actionStateMachine.ChangeState(ActionStates.Disabled);
        lastStateChangeTimestamp = Time.time;
    }

    void CastleGatesOpening_Update()
    {
        float elapsedTime = Time.time - lastStateChangeTimestamp;
        float alpha = Mathf.Lerp(1, 0, elapsedTime / levelStartAlphaFadeTime);
        SetCanvasAlpha(levelStartAnimationCanvas, alpha);
    }

    void PlayerWalkingOut_Enter()
    {
        actionStateMachine.ChangeState(ActionStates.Disabled);
        lastStateChangeTimestamp = Time.time;
    }

    void PlayerWalkingOut_FixedUpdate()
    {
        characterMovement.UpdateInput(1, false);
        characterMovement.Move();
        float elapsedTime = Time.time - lastStateChangeTimestamp;
        if (elapsedTime >= playerWalkingTime)
        {
            gameStateMachine.ChangeState(GameStates.PlayingLevel);
        }
    }

    void PlayingLevel_Enter()
    {
        actionStateMachine.ChangeState(ActionStates.Idle);
        lastStateChangeTimestamp = Time.time;
    }

    void PlayingLevel_Update()
    {
        float elapsedTime = Time.time - lastStateChangeTimestamp;
        float alpha = Mathf.Lerp(0, 1, elapsedTime / gameplayUiFadeInTime);
        SetCanvasAlpha(gameplayUiCanvas, alpha);
    }

    public void OnLevelStartAnimationEnded()
    {
        if (gameStateMachine.State == GameStates.LevelStartAnimation)
        {
            gameStateMachine.ChangeState(GameStates.CastleGatesOpening);
        }
    }

    public void OnOpeningEventEnded()
    {
        if (gameStateMachine.State == GameStates.CastleGatesOpening)
        {
            gameStateMachine.ChangeState(GameStates.PlayerWalkingOut);
        }
    }

    void SetCanvasAlpha(GameObject canvas, float alpha)
    {
        canvas.GetComponent<CanvasGroup>().alpha = alpha;
    }

    void EnableChildrenAnimators(GameObject gameObject, bool enabled)
    {
        Animator[] animators = gameObject.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            animator.enabled = enabled;
        }
    }
}
