using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MonsterLove.StateMachine;

public class GameStateMachine : MonoBehaviour {

    public ActionStateMachine actionStateMachineComponent;
    [Tooltip("Object that contains the canvas of the level start animation.")]
    public GameObject levelStartAnimationCanvas;
    [Tooltip("Castle object in the level.")]
    public GameObject castle;
    [Tooltip("State in which the game will start.")]
    public GameStates startingState = GameStates.LevelStartAnimation;
    [Tooltip("Fade in/out time for the level start animation graphics.")]
    public float levelStartAlphaFadeTime = 0.15f;
    [Tooltip("Fade in time for the gameplay sound and music.")]
    public float gameplaySoundFadeInTime = 2f;
    public SoundManager soundManager;

    private StateMachine<ActionStates> actionStateMachine;
    private StateMachine<GameStates> gameStateMachine;
    private float globalMusicVolume;
    private float globalSfxVolume;
    /// <summary>
    /// Moment in which the level start animation started
    /// </summary>
    private float levelStartAnimationTimestamp;

    // Use this for initialization
    void Start () {
        globalMusicVolume = soundManager.globalMusicVolume;
        globalSfxVolume = soundManager.globalSfxVolume;
        actionStateMachine = actionStateMachineComponent.StateMachine;
        gameStateMachine = StateMachine<GameStates>.Initialize(this, startingState);
    }

    void LevelStartAnimation_Enter()
    {
        // Mute game music and sfx
        soundManager.globalMusicVolume = 0;
        soundManager.globalSfxVolume = 0;
        // Enable level start animation rendering and fade in the alpha
        levelStartAnimationCanvas.SetActive(true);
        SetCanvasAlpha(0, 0);
        SetCanvasAlpha(1, levelStartAlphaFadeTime);
        EnableChildrenAnimators(levelStartAnimationCanvas, true);
        // Disable character state
        actionStateMachine.ChangeState(ActionStates.Disabled);
        levelStartAnimationTimestamp = Time.time;
    }

    void LevelStartAnimation_Update()
    {
        // Slowly fade game music and sound in
        soundManager.globalMusicVolume = Mathf.Lerp(0, globalMusicVolume, (Time.time - levelStartAnimationTimestamp) / gameplaySoundFadeInTime);
        soundManager.globalSfxVolume = Mathf.Lerp(0, globalSfxVolume, (Time.time - levelStartAnimationTimestamp) / gameplaySoundFadeInTime);
    }

    void LevelStartAnimation_Exit()
    {
        SetCanvasAlpha(0, levelStartAlphaFadeTime);
    }

    void OpeningEvent_Enter()
    {
        EnableChildrenAnimators(castle, true);
        actionStateMachine.ChangeState(ActionStates.Disabled);
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

    void SetCanvasAlpha(float alpha, float time)
    {
        Graphic[] graphics = levelStartAnimationCanvas.GetComponentsInChildren<Graphic>();
        foreach (Graphic graphic in graphics)
        {
            graphic.CrossFadeAlpha(alpha, time, false);
        }
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
