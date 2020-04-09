using UnityEngine;
using System.Collections;


// adapted from http://answers.unity.com/answers/464040/view.html
public class GameState {

    // modes of gameplay: either still playing, lost, or won
    public enum States {
        Playing, Lost, Won
    }

    private static States state = States.Playing;

    // pause is handled independently of game state so you can pause regardless
    // of the current game state
    private static bool isPaused = false;

    // TODO use counter to keep track of how many screens deep into the pause
    // screen the player currently is? That would let us tell when the escape
    // button should hide the current screen vs. hide the whole game

    private static void ChangeState(States newState) {
        state = newState;
    }

    private static bool IsState(States stateToCheck) {
        return state == stateToCheck;
    }

    public static void Play() {
        ChangeState(States.Playing);
    }

    public static void Pause() {
        isPaused = true;
    }

    public static void Unpause() {
        isPaused = false;
    }

    public static void Lose() {
        ChangeState(States.Lost);
    }

    // return whether or not local player is currently in the match (hasn't lost or won)
    public static bool IsPlaying {
        get {
            return IsState(States.Playing);
        }
    }

    // return whether or not local player's game is paused
    public static bool IsPaused {
        get {
            return isPaused;
        }
    }

    // return whether or not local player has lost
    public static bool HasLost {
        get {
            return IsState(States.Lost);
        }
    }

    // return whether or not a UI menu is currently open
    // (so the keyboard/mouse shouldn't accept player input)
    public static bool UIIsOpen {
        get {
            return isPaused;
        }
    }

    // return whether or not the player movement/action controls should apply
    // to the game
    // (as opposed to being disabled, for instance when the UI is open)
    public static bool PlayerControlsActive {
        get {
            return !isPaused && IsPlaying;
        }
    }

    // return whether or not the camera controls should apply to the game
    // (as opposed to being disabled, for instance when the UI is open)
    public static bool CameraControlsActive {
        get {
            return !isPaused && IsPlaying;
        }
    }
}
