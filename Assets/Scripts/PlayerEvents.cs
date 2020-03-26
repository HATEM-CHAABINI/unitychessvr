using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEvents : MonoBehaviour
{
    #region Events
    public static UnityAction OnTouchPadUp = null;
    public static UnityAction OnTouchPadDown = null;
    public static UnityAction<OVRInput.Controller, GameObject> OnControllerSource = null; 

    #endregion

    #region Anchors
    public GameObject m_LeftAnchors;
    public GameObject m_RightAnchors;
    public GameObject m_HeadAnchors;
    #endregion

    #region Input
    private Dictionary<OVRInput.Controller, GameObject> m_ControllerSets = null;
    private OVRInput.Controller m_InputSource = OVRInput.Controller.None;
    private OVRInput.Controller m_Controller = OVRInput.Controller.None;
    private bool m_InputActive = true;
    #endregion


    private void Awake()
    {
        OVRManager.HMDMounted += PlayerFound;
        OVRManager.HMDUnmounted += PlayerLost;

        m_ControllerSets = CreateControllerSets();
    }

    private void OnDestroy()
    {
        OVRManager.HMDMounted -= PlayerFound;
        OVRManager.HMDUnmounted -= PlayerLost;

    }

    private void Update()
    {
        //CHECK FOR ACTIVE INPUT
        if (!m_InputActive)
            return;
        CheckForController();
        //CHECK IF A CONTROLLER EXISTS 
        CheckInputSource();
        //CHECK FOR INPUT SOURCE
        Input();
    }

    private void CheckForController()
    {
        OVRInput.Controller controllerCheck = m_Controller;

        //RIGHT REMOTE
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
            controllerCheck = OVRInput.Controller.RTrackedRemote;

        //LEFT Remote
        if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
            controllerCheck = OVRInput.Controller.LTrackedRemote;

        // IF NO CONTROLLERS, HEADSET
        if (!OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote) && !OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
            controllerCheck = OVRInput.Controller.Touchpad;

        //UPDATE
        m_Controller = UpdateSource(controllerCheck, m_Controller);
    

    }
    private void CheckInputSource()
    {
        //UPDATE
        m_InputSource = UpdateSource(OVRInput.GetActiveController(), m_InputSource);

    }
    private void Input()
    {
        //Touchpad down
        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
        {
            if (OnTouchPadDown != null)
                OnTouchPadDown();
        }
        //Touchpad up
        if (OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad))
        {
            if (OnTouchPadUp != null)
                OnTouchPadUp();
        }
    }

    private OVRInput.Controller UpdateSource(OVRInput.Controller check,OVRInput.Controller previous)
    {
        // IF VALUES ARE THE SAME , RETURN
            if (check == previous)
                return previous;

        // GET CONTROLLER OBJECT
            GameObject controllerObject = null;
            m_ControllerSets.TryGetValue(check, out controllerObject);

        // IF NO CONTROLLER , SET TO THE HEAD
            if (controllerObject == null)
                controllerObject = m_HeadAnchors;

        // SEND OUT EVENT
        if (OnControllerSource != null)
            OnControllerSource(check, controllerObject);
        // return new value
            return check; 
    }

    private void PlayerFound()
    {
        m_InputActive = true;
    }

    private void PlayerLost()
    {
        m_InputActive = false;
    }

    private Dictionary<OVRInput.Controller,GameObject> CreateControllerSets()
    {
        Dictionary<OVRInput.Controller, GameObject> newSets = new Dictionary<OVRInput.Controller, GameObject>()
        {
            { OVRInput.Controller.LTrackedRemote,m_LeftAnchors },
            { OVRInput.Controller.RTrackedRemote,m_RightAnchors },
            { OVRInput.Controller.Touchpad,m_HeadAnchors }
        };
        return newSets;
    }



}
