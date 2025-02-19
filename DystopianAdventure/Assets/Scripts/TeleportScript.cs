using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportScript : MonoBehaviour
{
    public bool isActivated = false;
    private ParticleSystem ps;
    public Transform mainCamAxis;
    public float pickUpRange = 17;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Stop();

    }

    public bool turnOnTP() {
        if (isActivated == false && Input.GetKey(KeyCode.E)) {
            RaycastHit hit;
            if(Physics.Raycast(mainCamAxis.transform.position, mainCamAxis.transform.forward, out hit, pickUpRange))
            {
                if(hit.transform.gameObject.name == "TeleportPad")
                {
                    isActivated = true;
                    Debug.Log("Activated");
                    GameObject.Find("/Canvas/AliveUI/ImportantUI/GuideHint").SendMessage("MissionSix");
                    ps.Play();
                    return true;
                }
            }
        }
        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "FirstPersonPlayer" && isActivated == true)
        {
            SceneManager.LoadScene(2);
        }
    }
}
