using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOperationBehaviour : MonoBehaviour {

    Vector3 m_Center;
    Vector3 m_LPreviousPoint;
    Vector3 m_LInitialPoint;
    Vector3 m_RPreviousPoint;
    Vector3 m_RInitialPoint;
    Vector3 m_MPreviousPoint;
    Vector3 m_MInitialPoint;

    float m_Inertia = 0.0f;
    Vector3 m_RotationInertia = Vector3.zero;

    public Vector3 CenterPos { set { m_Center = value; } }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MouseOperation();
    }

    void MouseOperation()
    {
        Vector3 currentPoint = Input.mousePosition;

        // ButtonDown
        if (Input.GetMouseButtonDown(0))
        {
            m_LPreviousPoint = currentPoint;
            m_LInitialPoint = currentPoint;
            m_RotationInertia = Vector3.zero;
            m_Inertia = 0.0f;
        }
        if (Input.GetMouseButtonDown(1))
        {
            m_RPreviousPoint = currentPoint;
            m_RInitialPoint = currentPoint;
        }
        if (Input.GetMouseButtonDown(2))
        {
            m_MPreviousPoint = currentPoint;
            m_MInitialPoint = currentPoint;
        }

        // ButtonUp
        if (Input.GetMouseButtonUp(0))
        {
            m_RotationInertia = currentPoint - m_LPreviousPoint;
            m_Inertia = 1.0f;
        }
        if (Input.GetMouseButtonUp(1))
        {
        }
        if (Input.GetMouseButtonUp(2))
        {
        }

        // Drag
        if (Input.GetMouseButton(0))
        {
            // Left drag
            {
                Rotation(m_LPreviousPoint, currentPoint);
            }
            m_LPreviousPoint = currentPoint;
        }
        if (Input.GetMouseButton(1))
        {
            // Right drag
            {
                Scale(m_RPreviousPoint, currentPoint);
            }
            m_RPreviousPoint = currentPoint;
        }
        if (Input.GetMouseButton(2))
        {
            // Middle drag
            {
                Translation(m_MPreviousPoint, currentPoint);
            }
            m_MPreviousPoint = currentPoint;
        }

        float mouseWheelScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWheelScroll != 0)
        {
            // Wheel
        }

        if( m_Inertia > 0.0f)
        {
            if( m_Inertia == 1.0f)
            {
                if( m_RotationInertia.magnitude < 2.0f)
                {
                    m_Inertia = 0.0f;
                }
            }
            m_RotationInertia = m_RotationInertia * m_Inertia;
            Rotation(Vector3.zero, m_RotationInertia);
            m_Inertia -= 0.02f;
        }
    }

    void Rotation(Vector3 previousPoint, Vector3 currentPoint)
    {
        Vector3 centerPt = m_Center;
        Vector3 translation2d = (currentPoint - previousPoint);

        Vector3 axis = this.transform.localToWorldMatrix * new Vector3(-translation2d.y, translation2d.x, 0.0f);
        float angle = 360.0f * translation2d.magnitude / Screen.height;
        this.transform.RotateAround(m_Center, axis, angle);
    }

    private void Translation(Vector3 previousPoint, Vector3 currentPoint)
    {
        Vector3 translation2d = currentPoint - previousPoint;

        Vector3 translation3d = Vector3.zero;
        if (Camera.main.orthographic)
        {
            translation3d.x = translation2d.x * Camera.main.orthographicSize * 2.0f / Screen.width * Camera.main.aspect;
            translation3d.y = translation2d.y * Camera.main.orthographicSize * 2.0f / Screen.height;
        }
        else
        {
            translation3d.x = Vector3.Distance(Vector3.zero, this.transform.position) * Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView * translation2d.x / Screen.width);
            translation3d.y = Vector3.Distance(Vector3.zero, this.transform.position) * Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView * translation2d.y / Screen.height);
        }

        this.transform.Translate(-translation3d);
    }

    private void Scale(Vector3 previousPoint, Vector3 currentPoint)
    {
        Vector3 translation2d = currentPoint - previousPoint;
        float dist = translation2d.y /5.0f / Screen.height;

        Vector3 delta = new Vector3(0.0f, 0.0f, dist);
        Vector3 translation = this.transform.localToWorldMatrix * delta;
        Vector3 pos = this.transform.position - translation;
        this.transform.position = pos;
    }
}
