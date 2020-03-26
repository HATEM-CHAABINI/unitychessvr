/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class LaserPointer : OVRCursor
{

    /// Used to position the reticle at the current position.
    [Tooltip("Used to position the reticle at the current position.")]
    public GvrControllerReticleVisual reticle;

    /// The end point of the visual will not necessarily be along the forward direction of the laser.
    /// This is particularly true in both Camera and Hybrid Raycast Modes. In that case, both the
    /// laser and the controller are rotated to face the end point. This reference is used to control
    /// the rotation of the controller.
    [Tooltip("Used to rotate the controller to face the current position.")]
    public Transform controller;

    /// Color of the laser pointer including alpha transparency.
    [Tooltip("Start color of the laser pointer including alpha transparency.")]
    public Color laserColor = new Color(1.0f, 1.0f, 1.0f, 0.25f);

    /// Color of the laser pointer including alpha transparency.
    [Tooltip("End color of the laser pointer including alpha transparency.")]
    public Color laserColorEnd = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    /// Maximum distance of the laser (meters).
    [Tooltip("Maximum distance of the laser (meters).")]
    [Range(0.0f, 20.0f)]
    public float maxLaserDistance = 1.0f;

    /// The rate that the current position moves towards the target position.
    [Tooltip("The rate that the current position moves towards the target position.")]
    public float lerpSpeed = 20.0f;

    /// If the targetPosition is greater than this threshold, then
    /// the position changes immediately instead of lerping.
    [Tooltip("If the target position is greater than this threshold, then the position changes " +
      "immediately instead of lerping.")]
    public float lerpThreshold = 1.0f;

    /// This is primarily used for Hybrid Raycast mode (details in _GvrBasePointer_) to prevent
    /// mismatches between the laser and the reticle when the "camera" component of the ray is used.
    [Tooltip("Determines if the laser will shrink when it isn't facing in the forward direction " +
      "of the transform.")]
    public bool shrinkLaser = true;

    /// Amount to shrink the laser when it is fully shrunk.
    [Range(0.0f, 1.0f)]
    [Tooltip("Amount to shrink the laser when it is fully shrunk.")]
    public float shrunkScale = 0.2f;

    /// Begin shrinking the laser when the angle between transform.forward and the reticle
    /// is greater than this value.
    [Range(0.0f, 15.0f)]
    [Tooltip("Begin shrinking the laser when the angle between transform.forward and the reticle " +
      "is greater than this value.")]
    public float beginShrinkAngleDegrees = 0.0f;

    /// Finish shrinking the laser when the angle between transform.forward and the reticle is
    /// greater than this value.
    [Range(0.0f, 15.0f)]
    [Tooltip("Finish shrinking the laser when the angle between transform.forward and the reticle " +
      "is greater than this value.")]
    public float endShrinkAngleDegrees = 2.0f;
    public float overrideCameraRayIntersectionDistance;
    public enum LaserBeamBehavior
    {
        On,        // laser beam always on
        Off,        // laser beam always off
        OnWhenHitTarget,  // laser beam only activates when hit valid target
    }

    public GameObject cursorVisual;
    public float maxLength = 10.0f;

    private LaserBeamBehavior _laserBeamBehavior;

    public LaserBeamBehavior laserBeamBehavior
    {
        set {
            _laserBeamBehavior = value;
            if(laserBeamBehavior == LaserBeamBehavior.Off || laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
            {
                lineRenderer.enabled = false;
            }
            else
            {
                lineRenderer.enabled = true;
            }
        }
        get
        {
            return _laserBeamBehavior;
        }
    }
    private Vector3 _startPoint;
    private Vector3 _forward;
    private Vector3 _endPoint;
    private bool _hitTarget;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        if (cursorVisual) cursorVisual.SetActive(false);
    }

    public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
    {
        _startPoint = start;
        _endPoint = dest;
        _hitTarget = true;
    }

    public override void SetCursorRay(Transform t)
    {
        _startPoint = t.position;
        _forward = t.forward;
        _hitTarget = false;
    }

    private void LateUpdate()
    {
        lineRenderer.SetPosition(0, _startPoint);
        if (_hitTarget)
        {
            lineRenderer.SetPosition(1, _endPoint);
            UpdateLaserBeam(_startPoint, _endPoint);
            if (cursorVisual)
            {
                cursorVisual.transform.position = _endPoint;
                cursorVisual.SetActive(true);
            }
        }
        else
        {
            UpdateLaserBeam(_startPoint, _startPoint + maxLength * _forward);
            lineRenderer.SetPosition(1, _startPoint + maxLength * _forward);
            if (cursorVisual) cursorVisual.SetActive(false);
        }
    }

    // make laser beam a behavior with a prop that enables or disables
    private void UpdateLaserBeam(Vector3 start, Vector3 end)
    {
        if(laserBeamBehavior == LaserBeamBehavior.Off)
        {
            return;
        }
        else if(laserBeamBehavior == LaserBeamBehavior.On)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }
        else if(laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
        {
            if(_hitTarget)
            {
                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.SetPosition(0, start);
                    lineRenderer.SetPosition(1, end);
                }
            }
            else
            {
                if(lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                }
            }
        }
    }

    void OnDisable()
    {
        if(cursorVisual) cursorVisual.SetActive(false);
    }
}
