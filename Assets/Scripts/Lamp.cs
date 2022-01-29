using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField] private int lamp;
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger("Lamp", lamp);
    }

}
