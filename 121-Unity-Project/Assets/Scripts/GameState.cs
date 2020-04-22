using UnityEngine;
using System.Collections;
using Mirror;

// adapted from http://answers.unity.com/answers/464040/view.html
// and https://gamedev.stackexchange.com/a/116010
public class GameState : NetworkBehaviour {


    private static GameState _instance;

    public static GameState Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    // modes of gameplay: either still playing, lost, or won
    public enum States {
        Playing, Lost, Won
    }

    private States state = States.Playing;
    [SyncVar] private bool gameIsOver = false;

    [SyncVar] public int TotalNumPlayers = -1;
    [SyncVar] public int NumPlayersRemaining = -1;

    // pause is handled independently of game state so you can pause regardless
    // of the current game state
    private bool isPaused = false;

    // TODO use counter to keep track of how many screens deep into the pause
    // screen the player currently is? That would let us tell when the escape
    // button should hide the current screen vs. hide the whole game

    private void ChangeState(States newState) {
        state = newState;
    }

    private bool IsState(States stateToCheck) {
        return state == stateToCheck;
    }

    public void Play() {
        ChangeState(States.Playing);
    }

    public void Pause() {
        isPaused = true;
    }

    public void Unpause() {
        isPaused = false;
    }

    public void Lose() {
        ChangeState(States.Lost);
    }

    public void Win() {
        ChangeState(States.Won);
    }

    public void EndGame() {
        gameIsOver = true;
    }

    public bool GameHasEnded {
        get {
            return gameIsOver;
        }
    }

    // return whether or not local player is currently in the match (hasn't lost or won)
    public bool IsPlaying {
        get {
            return IsState(States.Playing);
        }
    }

    // return whether or not local player's game is paused
    public bool IsPaused {
        get {
            return isPaused;
        }
    }

    // return whether or not local player has lost
    public bool HasLost {
        get {
            return IsState(States.Lost);
        }
    }

    // return whether or not local player has lost
    public bool HasWon {
        get {
            return IsState(States.Won);
        }
    }

    // return whether or not a UI menu is currently open
    // (so the keyboard/mouse shouldn't accept player input)
    public bool UIIsOpen {
        get {
            return isPaused;
        }
    }

    // return whether or not the player movement/action controls should apply
    // to the game
    // (as opposed to being disabled, for instance when the UI is open)
    public bool PlayerControlsActive {
        get {
            return !gameIsOver && !isPaused && IsPlaying;
        }
    }

    // return whether or not the camera controls should apply to the game
    // (as opposed to being disabled, for instance when the UI is open)
    public bool CameraControlsActive {
        get {
            return !gameIsOver && !isPaused && IsPlaying;
        }
    }

    // // set the initial amount of players server-side
    // [ClientRpc] public static void RpcChangeNumPlayersRemaining(int newNumPlayersRemaining) {
    //     numPlayersRemaining = newNumPlayersRemaining;
    // }
    //
    // // update the amount of players left client-side
    // [Command] public static void CmdDecrementNumPlayersRemaining() {
    //     --numPlayersRemaining;
    // }
    //
    // public static int NumPlayersRemaining {
    //     get {
    //         return numPlayersRemaining;
    //     }
    // }
}
