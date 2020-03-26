namespace Oculus.Platform.Samples.VrHoops
{
    using UnityEngine;
    using UnityEngine.UI;
    

    // Helper class to attach to the main camera that raycasts where the


    public class VREyeRaycaster : MonoBehaviour
    {

        [SerializeField] private LineRenderer m_LineRenderer = null; // For supporting Laser Pointer
        public bool ShowLineRenderer = true;                         // Laser pointer visibility
        [SerializeField] private Transform m_TrackingSpace = null;   // Tracking space (for line renderer)
        [SerializeField] private UnityEngine.EventSystems.EventSystem m_eventSystem = null;

        private Button m_currentButton;

        void Update()
        {
            RaycastHit hit;
            Button button = null;

            Vector3 worldStartPoint = Vector3.zero;
            Vector3 worldEndPoint = Vector3.zero;
            if (m_LineRenderer != null)
            {
                m_LineRenderer.enabled = ControllerIsConnected && ShowLineRenderer;
            }

            if (ControllerIsConnected && m_TrackingSpace != null)
            {
                Matrix4x4 localToWorld = m_TrackingSpace.localToWorldMatrix;
                Quaternion orientation = OVRInput.GetLocalControllerRotation(Controller);

                Vector3 localStartPoint = OVRInput.GetLocalControllerPosition(Controller);
                Vector3 localEndPoint = localStartPoint + ((orientation * Vector3.forward) * 500.0f);

                worldStartPoint = localToWorld.MultiplyPoint(localStartPoint);
                worldEndPoint = localToWorld.MultiplyPoint(localEndPoint);

                // Create new ray
                Ray ray = new Ray(worldStartPoint, worldEndPoint - worldStartPoint);


            }
            // do a forward raycast to see if we hit a Button
            if (Physics.Raycast(transform.position, transform.forward, out hit, 50f))
            {
                button = hit.collider.GetComponent<Button>();
            }

            if (button != null)
            {
                if (m_currentButton != button)
                {
                    m_currentButton = button;
                    m_currentButton.Select();
                }
            }
            else if (m_currentButton != null)
            {
                m_currentButton = null;
                if (m_eventSystem != null)
                {
                    m_eventSystem.SetSelectedGameObject(null);
                }
            }

            if (ControllerIsConnected && m_LineRenderer != null)
            {
                m_LineRenderer.SetPosition(0, worldStartPoint);
                m_LineRenderer.SetPosition(1, worldEndPoint);
            }

        }

        public bool ControllerIsConnected
        {
            get
            {
                OVRInput.Controller controller = OVRInput.GetConnectedControllers() & (OVRInput.Controller.LTrackedRemote | OVRInput.Controller.RTrackedRemote);
                return controller == OVRInput.Controller.LTrackedRemote || controller == OVRInput.Controller.RTrackedRemote;
            }
        }
        public OVRInput.Controller Controller
        {
            get
            {
                OVRInput.Controller controller = OVRInput.GetconnectedControllers();
                if ((controller & OVRInput.Controller.LTrackedRemote) == OVRInput.Controller.LTrackedRemote)
                {
                    return OVRInput.Controller.LTrackedRemote;
                }
                else if ((controller & OVRInput.Controller.RTrackedRemote) == OVRInput.Controller.RTrackedRemote)
                {
                    return OVRInput.Controller.RTrackedRemote;
                }
                return OVRInput.GetActiveController();
            }
        }
    }
}
