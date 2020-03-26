using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticule : MonoBehaviour
{

    public Pointer m_pointer;
    public SpriteRenderer m_CircleRender;

    public Sprite m_OpenSprite;
    public Sprite m_ClosedSprite;

    private Camera m_camera = null;

    private void Awake()
    {
        m_pointer.OnPointerUpdate += UpdateSpirte;
        m_camera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.LookAt(m_camera.gameObject.transform);
    }
    private void OnDestroy()
    {
        m_pointer.OnPointerUpdate -= UpdateSpirte;

    }

    private void UpdateSpirte(Vector3 point, GameObject hitObject)
    {
        transform.position = point;
        if (hitObject)
        {
            m_CircleRender.sprite = m_ClosedSprite;
        }
        else
        {
            m_CircleRender.sprite = m_OpenSprite;

        }
    }
}
