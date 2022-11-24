using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour //https://youtu.be/4I0vonyqMi8?t=296 Este vid nos puede facilitar la existencia para organizar.
{
    public static GameManager instance; //Creo buena idea que cada manager sea un singleton. No vamos a tener 2 managers iguales en ningun momento.
    public GameState state;
    public static event Action<GameState> OnGameStateChange;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        UpdateGameState(GameState.MainMenu);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenu();
                break;
            case GameState.Pause:
                HandlePause();
                break;
            case GameState.Gaming:
                HandleGaming();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        //notificar con eventos a la script que le corresponda al cambiar los states
        OnGameStateChange?.Invoke(newState); //El "?.invoke" es para que no tire null error si nadie se subscribio al evento. Lo anoto xq no lo sabia¿
    }

    private void HandlePause()
    {

    }
    private void HandleMainMenu()
    {

    }
    private void HandleGaming()
    {

    }
    public enum GameState
    {
        //Menus previos irian aca
        MainMenu,
        Pause,
        Gaming
    }

}
