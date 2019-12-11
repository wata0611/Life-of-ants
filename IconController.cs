using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconController : MonoBehaviour
{
    [SerializeField] float xPos;
    [SerializeField] float yPos;
    [SerializeField] GameObject gameObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(gameObject.transform.position.x + xPos,
            gameObject.transform.position.y + yPos,
            gameObject.transform.position.z);
        if (transform.parent == null)
            gameObject.SetActive(false);
    }
}
