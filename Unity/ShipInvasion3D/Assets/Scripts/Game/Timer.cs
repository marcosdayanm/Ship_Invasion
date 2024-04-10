using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeLimit; // Tiempo límite en segundos, ajustable desde el Inspector
    private bool isTimerActive = false; 
    private Coroutine timerCoroutine;

    // Delegado y evento para actualizar el tiempo restante cada segundo
    public delegate void TimeUpdateAction(float timeRemaining);
    public event TimeUpdateAction OnTimeUpdate;

    // Propiedad para revisar si el temporizador está activo
    public bool IsTimerActive
    {
        get { return isTimerActive; }
    }

    public void StartTimer(float time = 10f)
    {
        this.timeLimit = time;
        if (!isTimerActive) // Evita reiniciar el temporizador si ya está activo
        {
            timerCoroutine = StartCoroutine(Countdown());
        }
    }

    private IEnumerator Countdown()
    {
        isTimerActive = true;
        float remainingTime = timeLimit;
        while(remainingTime > 0)
        {
            OnTimeUpdate?.Invoke(remainingTime);
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        // Cuando el tiempo se acaba, notificar que el temporizador ya no está activo y el tiempo restante es 0
        OnTimeUpdate?.Invoke(0);
        isTimerActive = false;
    }

    // Método opcional para detener el temporizador manualmente si es necesario
    public void StopTimer()
    {
        StopCoroutine(timerCoroutine);
        isTimerActive = false;
        OnTimeUpdate?.Invoke(0); // Opcional: notificar que el temporizador se detuvo antes de tiempo
    }
}
