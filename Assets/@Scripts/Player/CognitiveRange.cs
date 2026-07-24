using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CognitiveRange : MonoBehaviour
{
    private Player player;
    private void Awake()
    {
        player = gameObject.GetComponentInParent<Player>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "object")
        {
            Thing thing = other.GetComponent<Thing>();
            if (thing == null)
                return;
            int lvDif = player.CurrentData.Lv - thing.CurrentData.Lv;
            if (lvDif >= 0)
            {
                Outline outline = other.GetComponent<Outline>();
                if(outline != null)
                    outline.enabled = true;    
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "object") 
        {
            Outline outline = other.GetComponent<Outline>();
            if (outline != null && outline.enabled == true)
                outline.enabled = false;
        }
    }
}
