    using UnityEngine;
    using UnityEngine.UI; // Required for Text
    using Unity.Robotics.ROSTCPConnector;
    using TMPro;

    public class ScoreDisplay : MonoBehaviour {

        public TMP_Text ipText; // Create a reference to the UI Text element
        public ROSConnection ROS;

        void Start() {
            // Initially set the text to the score
            ipText.SetText(ROS.RosIPAddress);
        }

        void Update()
        {
            if (ROS.HasConnectionError)
            {
                ipText.SetText(ROS.RosIPAddress + " Error");
            }
            else
            {
                ipText.SetText(ROS.RosIPAddress);
            }
        }
    }
