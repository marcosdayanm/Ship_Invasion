using System.Collections;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeLimit; // Tiempo límite en segundos, ajustable desde el Inspector
    public bool isTimerActive = false; 
    private Coroutine timerCoroutine;

    [SerializeField] TMP_Text matchTimerText;
    [SerializeField] TMP_Text playTimerText;
    
    public float matchTime;

    SceneConnection sceneConnection = null;


    // Delegado y evento para actualizar el tiempo restante cada segundo
    public delegate void TimeUpdateAction(float timeRemaining);
    public event TimeUpdateAction OnTimeUpdate;



    public void Start()
    {
        matchTime = timeLimit;
        sceneConnection = GameObject.FindWithTag("SceneConnection").GetComponent<SceneConnection>();
    }


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
                // Actualizar el texto del temporizador en pantalla
                matchTimerText.text = FormatTime(remainingTime);
                
                OnTimeUpdate?.Invoke(remainingTime);
                yield return new WaitForSeconds(1f);
                remainingTime--;
            }

            matchTimerText.text = "00:00"; // Actualizar a 00:00 cuando el tiempo se acabe
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

        // Método para formatear el tiempo en minutos y segundos
        private string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
}