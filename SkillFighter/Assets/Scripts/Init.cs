using UnityEngine;
using UnityEngine.InputSystem;


public class Init : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        var player1 = PlayerInput.Instantiate(playerPrefab, controlScheme: "Keyboard", pairWithDevice: Keyboard.current);
        var player2 = PlayerInput.Instantiate(playerPrefab, controlScheme: "Keyboard2", pairWithDevice: Keyboard.current);
    }
}