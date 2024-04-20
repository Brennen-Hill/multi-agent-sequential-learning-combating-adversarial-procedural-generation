using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * This class is used as a manager that sends out events and calls the other
 * classes' OnBoardTick listeners
 */
public class GameTicker : MonoBehaviour
{
    // singleton (useful so that any class can find the tick manager and
    // register a listener to the tick)
    public static GameTicker instance;

    // constant gives us the rate at which ticks are fired out
    [SerializeField]
    private float SECONDS_PER_AUTOTICK = 0.7f;

    // We will be able to pause the ticks if this variable is false
    [SerializeField]
    private bool isTicking = true;

    // This event will be fire every time the autotick happens
    [SerializeField]
    public UnityEvent BoardTick;

    // is called before Start()
    // initialize the singleton and the unity event
    void Awake() {
        // singleton
        if(instance != null) {
            Debug.Log("ERROR! Multiple instances of singleton exist! (GameTicker.cs)");
        }
        else {
            instance = this;
        }

        // event
        if(BoardTick == null) {
            BoardTick = new UnityEvent();
        }
    }

    // is called before the first frame update.
    void Start() {
        StartCoroutine(TickRoutine());
    }

    // This coroutine will be used to fire the event every SECONDS_PER_AUTOTICK
    // seconds
    IEnumerator TickRoutine() {
        yield return null;

        while(true) {
            yield return new WaitForSeconds(SECONDS_PER_AUTOTICK);
            if(isTicking) {
                BoardTick.Invoke();
            }
        }
    }
}
