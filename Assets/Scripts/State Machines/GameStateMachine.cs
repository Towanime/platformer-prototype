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
    [Tooltip("Fade in/out time for the level start animation graphics.")]
    public float startScreenAlphaFadeOutTime = 0.15f;
    [Tooltip("Fade in/out time for the level start animation graphics.")]
    public float levelStartAlphaFadeTime = 0.15f;
    [Tooltip("Fade in time for the gameplay sound and music.")]
    public float gameplaySoundFadeInTime = 2f;
    public SoundManager soundManager;
    public PlayerInput playerInput;

    private StateMachine<ActionStates> actionStateMachine;
    private StateMachine<GameStates> gameStateMachine;
    private float globalMusicVolume;
    private float globalSfxVolume;
    /// <summary>
    /// Moment in which the level start animation started
    /// </summary>
    private float lastStateChangeTimestamp;
    private float elapsedTime;

    // Use this for initialization
    void Start () {
        globalMusicVolume = soundManager.globalMusicVolume;
        globalSfxVolume = soundManager.globalSfxVolume;
        actionStateMachine = actionStateMachineComponent.StateMachine;
        gameStateMachine = StateMachine<GameStates>.Initialize(this, startingState);
    }

    void StartScreen_Enter()
    {
        SetCanvasAlpha(startScreenCanvas, 1);
        // Mute game music and sfx
        soundManager.globalMusicVolume = 0;
        soundManager.globalSfxVolume = 0;
        // Disable character state
        actionStateMachine.ChangeState(ActionStates.Disabled);
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
        SetCanvasAlpha(startScreenCanvas, 1);
        // Mute game music and sfx
        soundManager.globalMusicVolume = 0;
        soundManager.globalSfxVolume = 0;
        // Disable character state
        actionStateMachine.ChangeState(ActionStates.Disabled);
        lastStateChangeTimestamp = Time.time;
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

    void OpeningEvent_Enter()
    {
        EnableChildrenAnimators(castle, true);
        actionStateMachine.ChangeState(ActionStates.Disabled);
    }

    void OpeningEvent_Update()
    {
        float elapsedTime = Time.time - lastStateChangeTimestamp;
        float alpha = Mathf.Lerp(1, 0, elapsedTime / levelStartAlphaFadeTime);
        SetCanvasAlpha(levelStartAnimationCanvas, alpha);
    }

    void PlayingLevel_Enter()
    {
        actionStateMachine.ChangeState(ActionStates.Idle);
    }

    public void OnLevelStartAnimationEnded()
    {
        if (gameStateMachine.State == GameStates.LevelStartAnimation)
        {
            gameStateMachine.ChangeState(GameStates.OpeningEvent);
        }
    }

    public void OnOpeningEventEnded()
    {
        if (gameStateMachine.State == GameStates.OpeningEvent)
        {
            gameStateMachine.ChangeState(GameStates.PlayingLevel);
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
