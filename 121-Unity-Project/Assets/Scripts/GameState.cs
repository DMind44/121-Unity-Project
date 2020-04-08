using UnityEngine;
using System.Collections;


// adapted from http://answers.unity.com/answers/464040/view.html
public class GameState {

    public enum States {
        Playing, Paused, Dead, Won
    }

    public static States state = States.Playing;

    public static void ChangeState(States newState) {
        state = newState;
    }

    public static bool IsState(States stateToCheck) {
        return state == stateToCheck;
    }

    public static void Play() {
        ChangeState(States.Playing);
    }

    public static void Pause() {
        ChangeState(States.Paused);
    }

    public static void Die() {
        ChangeState(States.Dead);
    }

    public static bool IsPlaying {
        get {
            return IsState(States.Playing);
        }
    }

    public static bool IsPaused {
        get {
            return IsState(States.Paused);
        }
    }

    public static bool IsDead {
        get {
            return IsState(States.Dead);
        }
    }



    // // You can still do this but will need GameState.Running = true;
    // // ChangeState is more atomic...
    // public static bool Running {
    //     get {
    //         return IsState(States.Pause);
    //     }
    //     set {
    //         if(value)
    //             ChangeState(States.Running);
    //     }
    // }
    // ...
}
