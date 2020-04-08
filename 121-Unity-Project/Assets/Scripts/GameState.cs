using UnityEngine;
using System.Collections;


// adapted from http://answers.unity.com/answers/464040/view.html
public class GameState {

    public enum States {
        Running, Died, Pause, Won
    }

    public static States state = States.Pause;

    public static void ChangeState(States newState) {
        state = newState;
    }

    public static bool IsState(States stateToCheck) {
        return state == stateToCheck;
    }

    public static bool IsAcceptingMovement {
        get {
            return IsState(States.Running);
        }
    }

    public static bool IsPaused {
        get {
            return IsState(States.Pause);
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
