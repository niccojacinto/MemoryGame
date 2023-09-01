using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int pairNumber;

    private Animator anim;
    private Renderer rend;

    [SerializeField]
    GameObject frontFace;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rend = frontFace.GetComponent<Renderer>();
    }

    private void Start()
    {
        FlipToBack();
    }

    public void FlipToFront()
    {
        anim.SetBool("Selected", true);
    }

    public void FlipToBack()
    {
        anim.SetBool("Selected", false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void SetFace(Material mat)
    {
        rend.material = mat;
    }

    private void OnMouseDown()
    {
        // Debug.Log(pairNumber);
        MemoryGame.Instance.SelectCard(this);
    }
}
