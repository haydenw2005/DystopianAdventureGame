﻿// all credits to Claire Blackshaw
// https://www.youtube.com/watch?v=5B6ALcOX4b8

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class HoverCarControl : MonoBehaviour
{
  Rigidbody m_body;
  float m_deadZone = 0.1f;

  public float m_hoverForce = 9.0f;
  public float m_hoverHeight = 2.0f;
  public GameObject[] m_hoverPoints;

  public float m_forwardAcl = 100.0f;
  public float m_backwardAcl = 25.0f;
  float m_currThrust = 0.0f;

  public float m_turnStrength = 10f;
  float m_currTurn = 0.0f;

  public GameObject m_leftAirBrake;
  public GameObject m_rightAirBrake;
  public Camera firstPersonCamera;

  public bool isActivated = false;

  int m_layerMask;


  public Transform mainCamAxis;
  public float pickUpRange = 5;
  public float moveForce = 250;
  public Transform playerPosition;
  public Transform bikePosition;
  public GameObject mainChar;

  public bool inCar;
  private GameObject heldobj;
  public HoverFollowCam HoverFollowCam;
  Camera m_MainCamera;

  public GameObject notImportantUI;


  void Start()
  {
    m_body = GetComponent<Rigidbody>();

    m_layerMask = 1 << LayerMask.NameToLayer("Characters");
    m_layerMask = ~m_layerMask;
    m_MainCamera = Camera.main;
    m_MainCamera.enabled = true;
    HoverFollowCam.enabled = false;
    inCar = HoverFollowCam.enabled;
  }

  void OnDrawGizmos()
  {

    //  Hover Force
    RaycastHit hit;
    for (int i = 0; i < m_hoverPoints.Length; i++)
    {
      var hoverPoint = m_hoverPoints [i];
      if (Physics.Raycast(hoverPoint.transform.position,
                          -Vector3.up, out hit,
                          m_hoverHeight,
                          m_layerMask))
      {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(hoverPoint.transform.position, hit.point);
        Gizmos.DrawSphere(hit.point, 0.5f);
      } else
      {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(hoverPoint.transform.position,
                       hoverPoint.transform.position - Vector3.up * m_hoverHeight);
      }
    }
  }

  void Update()
  {

    // Main Thrust
    m_currThrust = 0.0f;
    float aclAxis = Input.GetAxis("Vertical");
    if (aclAxis > m_deadZone)
      m_currThrust = aclAxis * m_forwardAcl;
    else if (aclAxis < -m_deadZone)
      m_currThrust = aclAxis * m_backwardAcl;

    // Turning
    m_currTurn = 0.0f;
    float turnAxis = Input.GetAxis("Horizontal");
    if (Mathf.Abs(turnAxis) > m_deadZone)
      m_currTurn = turnAxis;
  


    if (Input.GetKeyDown(KeyCode.E) && isActivated == true)
    {
      switchSeats();
    }

    if(this.transform.position.x < -101f && this.transform.position.x > -997f && this.transform.position.z < 1998f && this.transform.position.z > 998.7f)
    {
        RenderSettings.fog = true;
    }
    else
    {
        RenderSettings.fog = false;
    }

  }
  
  public bool switchSeats() {
    if (heldobj == null && inCar == false) {
      RaycastHit hit;
      if(Physics.Raycast(mainCamAxis.transform.position, mainCamAxis.transform.forward, out hit, pickUpRange))
      {
        if(hit.transform.gameObject.name == "HoverBike")
        {
          if (isActivated == false) {
            isActivated = true;
            GameObject.Find("/Canvas/AliveUI/ImportantUI/GuideHint").SendMessage("MissionFour");
            return true;
          }
          getInCar();
        }
      }
    }
    else {
      getOutCar();
    }
    return false;
  }

  

  void FixedUpdate()
  {

    //  Hover Force
    RaycastHit hit;
    for (int i = 0; i < m_hoverPoints.Length; i++)
    {
      var hoverPoint = m_hoverPoints [i];
      if (Physics.Raycast(hoverPoint.transform.position,
                          -Vector3.up, out hit,
                          m_hoverHeight,
                          m_layerMask))
        m_body.AddForceAtPosition(Vector3.up
          * m_hoverForce
          * (1.0f - (hit.distance / m_hoverHeight)),
                                  hoverPoint.transform.position);
      else
      {
        if (transform.position.y > hoverPoint.transform.position.y)
          m_body.AddForceAtPosition(
            hoverPoint.transform.up * m_hoverForce,
            hoverPoint.transform.position);
        else
          m_body.AddForceAtPosition(
            hoverPoint.transform.up * -m_hoverForce,
            hoverPoint.transform.position);
      }
    }

    // Forward
    if (Mathf.Abs(m_currThrust) > 0  && firstPersonCamera.enabled == false)
      m_body.AddForce(transform.forward * m_currThrust);

    // Turn
    if (m_currTurn > 0  && firstPersonCamera.enabled == false)
    {
      m_body.AddRelativeTorque(Vector3.up * m_currTurn * m_turnStrength);
    } else if (m_currTurn < 0  && firstPersonCamera.enabled == false)
    {
      m_body.AddRelativeTorque(Vector3.up * m_currTurn * m_turnStrength);
    }
  }

  void getInCar()
  {
    if (inCar == false)
    {
        //Enable the second Camera
        HoverFollowCam.enabled = true;
        //The Main first Camera is disabled
        m_MainCamera.enabled = false;
        mainChar.SetActive(false);
        // turn off ui
        notImportantUI.SetActive(false);
    }
    inCar = true;
  }

  void getOutCar()
  {
    if (inCar == true)
    {
        //disable the second Camera
        HoverFollowCam.enabled = false;
        //The Main first Camera is enabled
        m_MainCamera.enabled = true;
        mainChar.SetActive(true);
        playerPosition.position = bikePosition.position + new Vector3(0.0f, 0.0f, -4.0f);
        HoverFollowCam.transform.position = new Vector3(589.6f, 53.4f, 601.5f);
        // turn on ui
        notImportantUI.SetActive(true);
    }
    inCar = false;
  }

  public bool getStatus()
  {
    return inCar;
  }
}
