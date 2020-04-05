using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class piece : MonoBehaviour {
    public GameObject FloatTextPrefab;

    public int side; //0 for white, 1 for black
    public string Nom;
    public string habitat;
    public string carvi;
    float height;
    float dest;
    bool lift;
    bool fall;
    bool moving;
    public float speed;
    Vector3 target;
    public board B;
    float delta = 0.01f;
    public AudioSource sound;

    // Use this for initialization
    void Start () {
        lift = false;
        fall = false;
        moving = false;
        height = gameObject.transform.position.y;
        speed = 5;
	}
	
	// Update is called once per frame
	void Update () {
        if (lift)
            if (gameObject.transform.position.y < dest)
            {
                sound.Play();
                
                gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
                if (FloatTextPrefab) { 
                ShowFloatingText();
                }
            }
            else
            {
                lift = false;
            }
        if (fall)
            if (gameObject.transform.position.y > dest)
            {
                gameObject.transform.Translate(Vector3.down * speed * Time.deltaTime);
            }
            else
            {
                fall = false;
                gameObject.transform.position.Set(gameObject.transform.position.x, 0, gameObject.transform.position.z);

                //sound.Play();
            }
        if (moving)
        {
            if (Mathf.Abs(gameObject.transform.position.z-target.z)>delta || Mathf.Abs(gameObject.transform.position.x - target.x) > delta)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            }
            else
            {
                moving = false;
                B.remove_piece();
                godown();
                B.next_grid.temp_piece = gameObject;
            }
        }
	}

    public void goup()
    {
        dest = height + 3;
        lift = true;
    }

    public void godown()
    {
        dest = height;
        fall = true;
    }

    void ShowFloatingText()
    {
       var go = Instantiate(FloatTextPrefab, new Vector3(2.43f, 3.47f, 3.09f), Quaternion.Euler(0f, 35.428f, 0f));
        go.transform.GetChild(1).GetComponent<TextMesh>().text = Nom + "\n" + habitat + "\n" + carvi;
        // TextP.GetComponent<TextMesh>().text = "aaaa";
        //  go.GetComponent<TextMesh>().text = Nom + "\n" + habitat + "\n" + carvi ;
    }

    public void moveto(Vector3 t)
    {
        //sound.Play();

        target = t;
        target.y = gameObject.transform.position.y;
        moving = true;
    }
}
