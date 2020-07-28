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
        promptText.text = "Welcome!";

        //They all start at max (i.e. not running)
        cTimer = sTimer = tTimer = textTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Fade(promptText, textTimer);
        Fade(circle, cTimer);
        Fade(square, sTimer);
        Fade(triangle, tTimer);
    }

    private void Fade(RawImage obj, float timer)
    {
        if (Time.time - timer <= shapeFadeTime)
        {
            float a = 1f - (Time.time - timer) / shapeFadeTime;
            obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, a);
        }
        else if (obj.color.a != 0)
        {
            Color c = obj.color;
            c.a = 0;
            obj.color = c;
        }
    }

    private void Fade(Text obj, float timer)
    {
        if (Time.time - timer <= textFadeTime)
        {
            float a = 1f - (Time.time - timer) / textFadeTime;
            obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, a);
        }
        else if (obj.color.a != 0)
        {
            Color c = obj.color;
            c.a = 0;
            obj.color = c;
        }
    }

    public void Interact(FishMachine.Interaction interaction)
    {
        switch (interaction)
        {
            //We do 'textTimer = xTimer = Time.time' because both timers start at the same time
            case FishMachine.Interaction.POINTING:
                textTimer = cTimer = Time.time;
                promptText.text = "Point";
                break;
            case FishMachine.Interaction.SPRINKLING:
                textTimer = sTimer = Time.time;
                promptText.text = "Sprinkle";
                break;
            case FishMachine.Interaction.WAVE:
                textTimer = tTimer = Time.time;
                promptText.text = "Wave";
                break;
        }
    }
}
