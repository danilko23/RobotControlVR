using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes;
using RosMessageTypes.Geometry;
using TMPro;
/// <summary>
///
/// </summary>
public class RosPublisherExample : MonoBehaviour
{
    public ROSConnection ros;
    //public string topicName = "pos_rot";
    public string topicName = "unity_to_ros";

    // The game object
    //public GameObject controller;


    // Publish the controller's position and rotation every N seconds
    public float publishMessageFrequency = 0.5f;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;

    Vector3 InitialControllerPosition;
    Vector3 CurrentControllerPosition;

    Quaternion InitialControllerRotation;
    Quaternion CurrentControllerRotation;

    bool firstPress = true;
    bool publish = true;

    LineRenderer lr;
    public Material tMat;

    // Connection Status
    public TextMeshPro textMeshPro;

    PoseMsg controllerPos;

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseMsg>(topicName);
        InitialControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        InitialControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // Draw line
        GameObject myLine = new GameObject();
        myLine.transform.position = transform.position;
        myLine.AddComponent<LineRenderer>();
        lr = myLine.GetComponent<LineRenderer>();
        lr.material = tMat;
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.SetPosition(0, InitialControllerPosition);
    }

    private void Update()
    {
        OVRInput.Update();
        OVRInput.FixedUpdate();
        
        // check connectio status
        if(ros.HasConnectionError)
        {
            textMeshPro.text = "Not Connected";
            textMeshPro.color = Color.red;
        }
        else
        {
            textMeshPro.text = "Connected";
            textMeshPro.color = Color.blue;

        }
        
        // if controller trigger stays pressed
        if(OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            if(firstPress)
            {
                InitialControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                InitialControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
                lr.SetPosition(0, InitialControllerPosition);
                firstPress = false;
            }
        }

        CurrentControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        CurrentControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            controllerPos = new PoseMsg(
                new PointMsg
                (
                CurrentControllerPosition.z - InitialControllerPosition.z,
                -(CurrentControllerPosition.x - InitialControllerPosition.x),
                CurrentControllerPosition.y - InitialControllerPosition.y
                ),
                new QuaternionMsg
                (
                    //CurrentControllerRotation.z - InitialControllerRotation.z,
                    //-(CurrentControllerRotation.x - InitialControllerRotation.x),
                    //CurrentControllerRotation.y - InitialControllerRotation.y,
                    //CurrentControllerRotation.w - InitialControllerRotation.w
                    CurrentControllerRotation.z,
                    -CurrentControllerRotation.x,
                    CurrentControllerRotation.y,
                    CurrentControllerRotation.w
                ));

            // Finally send the message to server_endpoint.py running in ROS
            if(OVRInput.Get(OVRInput.Button.Two))
            {
                ros.Publish(topicName, controllerPos);
            }
            timeElapsed = 0;
        }

        lr.SetPosition(1, CurrentControllerPosition);
            
        if(OVRInput.Get(OVRInput.Button.One) && publish)
        {
            ros.Publish(topicName, controllerPos);
            publish = false;
        }

        if(!OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            firstPress = true;
        }

        if(!OVRInput.Get(OVRInput.Button.One))
        {
            publish = true;
        }

        //Debug
        //ros.Publish(topicName, controllerPos);
    }
}


        //        controller.transform.position.x,
        //        controller.transform.position.y,
        //        controller.transform.position.z,
        //        controller.transform.rotation.x,
        //        controller.transform.rotation.y,
        //        controller.transform.rotation.z,
        //        controller.transform.rotation.w