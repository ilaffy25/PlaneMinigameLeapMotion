using UnityEngine;

public class WinCheckpoint : MonoBehaviour
{
    [Header("Referencias")]
    public GameManager gameManager;  // Arrastras tu GameManager aquí
    public GameObject restartButton; // El botón que quieres mostrar

    [Header("Mensaje de victoria")]
    public string winMessage = "¡Ganaste!";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Mostrar mensaje
            if (gameManager != null)
            {
                gameManager.TriggerGameOver(winMessage);
            }

            // Activar el botón
            if (restartButton != null)
            {
                restartButton.SetActive(true);
            }

            // Desactivar este checkpoint para que no se repita
            gameObject.SetActive(false);
        }
    }
}
