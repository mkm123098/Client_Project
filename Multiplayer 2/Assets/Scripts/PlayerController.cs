using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform camTransform;

    private bool[] inputs;

    private void Start()
    {
        inputs = new bool[6];
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
            inputs[0] = true;

        if (Input.GetKey(KeyCode.S))
            inputs[1] = true;

        if (Input.GetKey(KeyCode.A))
            inputs[2] = true;

        if (Input.GetKey(KeyCode.D))
            inputs[3] = true;

        if (Input.GetKey(KeyCode.Space))
            inputs[4] = true;

        if (Input.GetKey(KeyCode.LeftShift))
            inputs[5] = true;
    }

    private void FixedUpdate()
    {
        SendInput();

        for (int i = 0; i < inputs.Length; i++)
            inputs[i] = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player1 = other.GetComponent<Player>();
        if (player1 != null)
        {
            Debug.Log("Collided with enemy player");
            SendCollision();
        }
        /*        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Model")
                {
                    Debug.Log ("hi");
                    SendCollision();
                }*/
    }

    #region Messages
    private void SendInput()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.input);
        message.AddBools(inputs, false);
        message.AddVector3(camTransform.forward);
        NetworkManager.Singleton.Client.Send(message);
    }

    private void SendCollision()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.playerCollision);
        message.AddString("Collision occured between player");
        NetworkManager.Singleton.Client.Send(message);
    }
    #endregion
}
