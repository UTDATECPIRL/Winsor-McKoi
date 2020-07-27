using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Jobs;

public class TrainingGuiScript : MonoBehaviour
{

    private RawImage circle, square, triangle;

    private float cTimer, sTimer, tTimer, textTimer;

    private Text promptText;

    [Range(0f, 5f)]
    public float shapeFadeTime;

    [Range(0f, 5f)]
    public float textFadeTime;

    // Start is called before the first frame update
    void Start()
    {
        circle = GameObject.Find("Circle").GetComponent<RawImage>();
        square = GameObject.Find("Square").GetComponent<RawImage>();
        triangle = GameObject.Find("Triangle").GetComponent<RawImage>();

        promptText = GetComponentInChildren<Text>();

     //   circle.SetActive(false);
     //   square.SetActive(false);
     //   triangle.SetActive(false);
        promptText.text = "Welcome!";

        //They all start at max (i.e. not running)
        cTimer = sTimer = tTimer = textTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Material material;
        float now = Time.time;

        if (now - textTimer <= textFadeTime)
        {
            float a = 1f - (now - textTimer) / textFadeTime;
            promptText.color = new Color(promptText.color.r, promptText.color.g, promptText.color.b, a);
        }
        else if(promptText.color.a != 0)
        {
            Color c = promptText.color;
            c.a = 0;
            promptText.color = c;
        }

        if (now - cTimer <= shapeFadeTime)
        {
            float a = 1f - (now - cTimer) / shapeFadeTime;
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, a);
        }
        else if (circle.color.a != 0)
        {
            Color c = circle.color;
            c.a = 0;
            circle.color = c;
        }

        if (now - sTimer <= shapeFadeTime)
        {
            float a = 1f - (now - sTimer) / shapeFadeTime;
            square.color = new Color(square.color.r, square.color.g, square.color.b, a);
        }
        else if (square.color.a != 0)
        {
            Color c = square.color;
            c.a = 0;
            square.color = c;
        }

        if (now - tTimer <= shapeFadeTime)
        {
            float a = 1f - (now - tTimer) / shapeFadeTime;
            triangle.color = new Color(triangle.color.r, triangle.color.g, triangle.color.b, a);
        }
        else if (triangle.color.a != 0)
        {
            Color c = triangle.color;
            c.a = 0;
            triangle.color = c;
        }

    }

    public void Interact(FishMachine.Interaction interaction)
    {
        switch (interaction)
        {
            case FishMachine.Interaction.POINTING:
                //                circle.SetActive(!circle.activeInHierarchy);
                textTimer = cTimer = Time.time;
                promptText.text = "Point";
                break;
            case FishMachine.Interaction.SPRINKLING:
                //                square.SetActive(!square.activeInHierarchy);
                textTimer = sTimer = Time.time;
                promptText.text = "Sprinkle";
                break;
            case FishMachine.Interaction.WAVE:
                //                triangle.SetActive(!triangle.activeInHierarchy);
                textTimer = tTimer = Time.time;
                promptText.text = "Wave";
                break;
        }
    }
}
