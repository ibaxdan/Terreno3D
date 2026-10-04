using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camaraTransform;

    [Header("Ajustes de Movimiento")]
    public float velocidadMovimiento = 6.0f;
    public float velocidadRotacion = 120.0f;

    [Header("Ajustes de Vista (Flechas)")]
    public float velocidadCamara = 70.0f;
    public float limiteVerticalMin = -20.0f; // Límite mirando hacia arriba
    public float limiteVerticalMax = 60.0f;  // Límite mirando hacia abajo

    [Header("Salto y Gravedad")]
    public float fuerzaSalto = 5.0f;
    public float gravedad = -9.81f;

    [Header("Modo Vuelo")]
    public bool modoVuelo = false;
    public float velocidadVuelo = 8.0f;

    private CharacterController controller;
    private float velocidadVertical;
    private float rotacionVerticalCamara = 15.0f; // Ángulo inicial hacia abajo

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Si no se asignó la cámara manualmente, buscar la cámara hija
        if (camaraTransform == null && Camera.main != null)
        {
            camaraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // 1. Control de perspectiva vertical con Flecha Arriba / Flecha Abajo
        float pitchInput = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) pitchInput -= 1f;   // Levanta la cabeza
        if (Input.GetKey(KeyCode.DownArrow)) pitchInput += 1f; // Baja la cabeza

        if (camaraTransform != null)
        {
            rotacionVerticalCamara += pitchInput * velocidadCamara * Time.deltaTime;
            rotacionVerticalCamara = Mathf.Clamp(rotacionVerticalCamara, limiteVerticalMin, limiteVerticalMax);
            
            // Aplica la inclinación vertical a la cámara manteniendo sus rotaciones Y y Z
            camaraTransform.localEulerAngles = new Vector3(rotacionVerticalCamara, 0f, 0f);
        }

        // 2. Giro horizontal del personaje con A / D o Flechas Izquierda / Derecha
        float inputRotacion = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.LeftArrow)) inputRotacion = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) inputRotacion = 1f;

        transform.Rotate(0, inputRotacion * velocidadRotacion * Time.deltaTime, 0);

        // 3. Alternar modo vuelo con la tecla F
        if (Input.GetKeyDown(KeyCode.F))
        {
            modoVuelo = !modoVuelo;
            velocidadVertical = 0f;
        }

        // 4. Avance y Retroceso con W y S
        float inputAvance = Input.GetAxis("Vertical");
        Vector3 direccionAvance = transform.forward * inputAvance;

        // 5. Lógica de vuelo vs caminar
        if (modoVuelo)
        {
            float inputElevacion = 0f;
            if (Input.GetKey(KeyCode.Space)) inputElevacion = 1f;
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.C)) inputElevacion = -1f;

            Vector3 movimientoVuelo = (direccionAvance * velocidadVuelo) + (Vector3.up * inputElevacion * velocidadVuelo);
            controller.Move(movimientoVuelo * Time.deltaTime);
        }
        else
        {
            if (controller.isGrounded)
            {
                if (velocidadVertical < 0)
                {
                    velocidadVertical = -2f;
                }

                if (Input.GetButtonDown("Jump"))
                {
                    velocidadVertical = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);
                }
            }
            else
            {
                velocidadVertical += gravedad * Time.deltaTime;
            }

            Vector3 movimiento = direccionAvance * velocidadMovimiento;
            movimiento.y = velocidadVertical;
            controller.Move(movimiento * Time.deltaTime);
        }
    }
}