using System;
using System.Collections;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    public bool isRaised;
    [SerializeField] private Color[] bottleColors;
    [SerializeField] private SpriteRenderer bottleMask;
    [SerializeField] private float[] fillAmounts;
    //[SerializeField] private float lastAngleValue;
    [SerializeField] private float directionMultiplier;
    [SerializeField] private float[] rotationValue;
    [SerializeField] private int rotationIndex = 0;
    [SerializeField] private Transform leftSide;
    [SerializeField] private Transform rightSide;
    [SerializeField] private Transform chosenSide;
    [SerializeField] private GameObject confetti;
    [SerializeField] private ParticleSystem sparkling;
    [Range(0, 4)]
    public int numberOfColorsInBottle = 4;
    public int numberOfTopColorLayers = 1;
    [SerializeField] private bool justThisBottle;
    [SerializeField] private int numberOfTransferColor;
    [SerializeField] private Vector3 originalPos, startPos, endPos;
    [SerializeField] private LineRenderer lineRenderer;

    private bool playedSound = false;
    private bool pourSound = false;
    public event EventHandler onBottleComplete;
    public Color topColor;
    public BottleController bottleRef;
    public AnimationCurve ScaleMultiplier;
    public AnimationCurve FillAmountCurve;
    public AnimationCurve RotationSpeedMultiplier;

    private const string COLOR1 = "_Color01";
    private const string COLOR2 = "_Color02";
    private const string COLOR3 = "_Color03";
    private const string COLOR4 = "_Color04";
    private const string MULTIPLYER = "_Multiplier";
    private const string FILL_AMOUNT = "_FillAmount";
    void Start()
    {

        isRaised = false;
        originalPos = transform.position;
        bottleMask.material.SetFloat(FILL_AMOUNT, fillAmounts[numberOfColorsInBottle]);
        UpdateTopColor();
        UpdateBottleColor();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateTopColor();
        CheckBottleFillLevel();
        UpdateBottleColor();
    }

    public void StartTransferColor()
    {

        ChooseRotationPointAndDirection();
        numberOfTransferColor = Mathf.Min(numberOfTopColorLayers, 4 - bottleRef.numberOfColorsInBottle);
        Debug.Log(numberOfTransferColor);
        for (int i = 0; i < numberOfTransferColor; i++)
        {
            bottleRef.bottleColors[bottleRef.numberOfColorsInBottle + i] = topColor;
        }
        bottleRef.UpdateBottleColor();
        Debug.Log("2nd bottle total colors " + bottleRef.numberOfColorsInBottle);
        CalculateRotationIndex(4 - bottleRef.numberOfColorsInBottle);

        transform.GetComponent<SpriteRenderer>().sortingOrder += 5;
        bottleMask.sortingOrder += 5;

        StartCoroutine("MoveBottle");
    }

    public void UpdateBottleColor()
    {
        bottleMask.material.SetColor(COLOR1, bottleColors[0]);
        bottleMask.material.SetColor(COLOR2, bottleColors[1]);
        bottleMask.material.SetColor(COLOR3, bottleColors[2]);
        bottleMask.material.SetColor(COLOR4, bottleColors[3]);
    }
    public float rotateTimer = 1f;
    IEnumerator RotateBottle()
    {

        float t = 0;
        float lerpValue;
        float angleValue;
        float lastAngleValue = 0;
        float lastFillValue = bottleMask.material.GetFloat(FILL_AMOUNT);
        while (t < rotateTimer)
        {

            lerpValue = t / rotateTimer;
            angleValue = Mathf.Lerp(0f, directionMultiplier * rotationValue[rotationIndex], lerpValue);

            //transform.eulerAngles = new Vector3(0, 0, angleValue);
            transform.RotateAround(chosenSide.position, Vector3.forward, lastAngleValue - angleValue);
            bottleMask.material.SetFloat(MULTIPLYER, ScaleMultiplier.Evaluate(angleValue));

            if (fillAmounts[numberOfColorsInBottle] > FillAmountCurve.Evaluate(angleValue))
            {
                if (lineRenderer.enabled == false)
                {
                    lineRenderer.startColor = new Color(topColor.r, topColor.g, topColor.b, 1);
                    lineRenderer.endColor = new Color(topColor.r, topColor.g, topColor.b, 1);

                    lineRenderer.SetPosition(0, chosenSide.position);
                    lineRenderer.SetPosition(1, chosenSide.position - Vector3.up * 5f);

                    lineRenderer.enabled = true;
                    AudioManager.Instance.PlaySFX("Pour");
                }
                bottleMask.material.SetFloat(FILL_AMOUNT, FillAmountCurve.Evaluate(angleValue));
                bottleRef.FillUp(FillAmountCurve.Evaluate(lastAngleValue) - FillAmountCurve.Evaluate(angleValue));
                //Debug.Log(angleValue + " " + ScaleMultiplier.Evaluate(angleValue));
            }

            t += Time.deltaTime * RotationSpeedMultiplier.Evaluate(angleValue);
            lastAngleValue = angleValue;
            yield return new WaitForEndOfFrame();

        }

        //transform.eulerAngles = new Vector3(0, 0, angleValue);
        angleValue = directionMultiplier * rotationValue[rotationIndex];
        bottleMask.material.SetFloat(MULTIPLYER, ScaleMultiplier.Evaluate(angleValue));
        bottleMask.material.SetFloat(FILL_AMOUNT, FillAmountCurve.Evaluate(angleValue));
        if (bottleRef.numberOfColorsInBottle < 4)
        {
            numberOfColorsInBottle -= numberOfTransferColor;
            bottleRef.numberOfColorsInBottle += numberOfTransferColor;
        }
        else
        {
            bottleRef.numberOfColorsInBottle = 4;
            bottleMask.material.SetFloat(FILL_AMOUNT, lastFillValue);
        }
        lineRenderer.enabled = false;
        AudioManager.Instance.PauseSFX("Pour");
        StartCoroutine("FlipBottleBack");

    }

    IEnumerator FlipBottleBack()
    {

        float t = 0;
        float lerpValue;
        float angleValue;
        float lastAngleValue = directionMultiplier * rotationValue[rotationIndex];

        while (t < rotateTimer)
        {
            lerpValue = t / rotateTimer;
            angleValue = Mathf.Lerp(directionMultiplier * rotationValue[rotationIndex], 0, lerpValue);

            //transform.eulerAngles = new Vector3(0, 0, angleValue);
            transform.RotateAround(chosenSide.position, Vector3.forward, lastAngleValue - angleValue);

            bottleMask.material.SetFloat(MULTIPLYER, ScaleMultiplier.Evaluate(angleValue));
            lastAngleValue = angleValue;
            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();

        }
        angleValue = 0f;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMask.material.SetFloat(MULTIPLYER, ScaleMultiplier.Evaluate(angleValue));
        UpdateTopColor();
        StartCoroutine("MoveBottleBack");
    }

    IEnumerator MoveBottle()
    {
        startPos = transform.position;
        if (chosenSide == leftSide)
        {
            endPos = bottleRef.rightSide.position;
        }
        else
        {
            endPos = bottleRef.leftSide.position;
        }

        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }

        transform.position = endPos;
        StartCoroutine("RotateBottle");
    }

    IEnumerator MoveBottleBack()
    {

        startPos = transform.position;
        endPos = originalPos;

        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }

        transform.position = endPos;
        MenuController.instance.CheckWinningCondition();
        //GameController.instance.OnTransferComplete();
        transform.GetComponent<SpriteRenderer>().sortingOrder -= 5;
        bottleMask.sortingOrder -= 5;
    }

    public void UpdateTopColor()
    {
        if (numberOfColorsInBottle != 0)
        {
            numberOfTopColorLayers = 1;
            topColor = bottleColors[numberOfColorsInBottle - 1];
            if (numberOfColorsInBottle == 4)
            {
                if (bottleColors[3].Equals(bottleColors[2]))
                {
                    numberOfTopColorLayers = 2;
                    if (bottleColors[2].Equals(bottleColors[1]))
                    {
                        numberOfTopColorLayers = 3;
                        if (bottleColors[1].Equals(bottleColors[0]))
                        {
                            float colorOffset = 0.9f;
                            numberOfTopColorLayers = 4;
                            if (!playedSound)
                            {
                                AudioManager.Instance.PlaySFX("ContainerFinish"); // Play the sound here
                                Instantiate(confetti, new Vector3(transform.position.x, transform.position.y + 1.7f, transform.position.z), Quaternion.identity);
                                sparkling.startColor = new Color(topColor.r * colorOffset, topColor.g * colorOffset, topColor.b * colorOffset, 250);
                                Instantiate(sparkling, new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z), Quaternion.Euler(-90f, 0, 0));
                                playedSound = true;
                            }
                        }
                    }
                }
            }
            else if (numberOfColorsInBottle == 3)
            {
                numberOfTopColorLayers = 1;
                if (bottleColors[2].Equals(bottleColors[1]))
                {
                    numberOfTopColorLayers = 2;
                    if (bottleColors[1].Equals(bottleColors[0]))
                    {
                        numberOfTopColorLayers = 3;
                    }
                }
            }
            else if (numberOfColorsInBottle == 2)
            {
                numberOfTopColorLayers = 1;
                if (bottleColors[1].Equals(bottleColors[0]))
                {
                    numberOfTopColorLayers = 2;
                }
            }
        }
        else
        {
            numberOfTopColorLayers = 0;
        }
    }

    public bool FillBottleCheck(Color checkColor)
    {
        if (numberOfColorsInBottle == 0)
        {
            return true;
        }
        else
        {
            if (numberOfColorsInBottle == 4)
            {
                return false;
            }
            else
            {
                if (topColor.Equals(checkColor))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    private void CalculateRotationIndex(int numberOfSpacesInSecondBottle)
    {
        rotationIndex = 3 - (numberOfColorsInBottle - Mathf.Min(numberOfSpacesInSecondBottle, numberOfTopColorLayers));
    }

    private void FillUp(float fillAmountToAdd)
    {
        bottleMask.material.SetFloat(FILL_AMOUNT, bottleMask.material.GetFloat(FILL_AMOUNT) + fillAmountToAdd);

    }

    private void CheckBottleFillLevel()
    {
        if (bottleMask.material.GetFloat(FILL_AMOUNT) > fillAmounts[fillAmounts.Length - 1])
        {
            bottleMask.material.SetFloat(FILL_AMOUNT, fillAmounts[fillAmounts.Length - 1]);

        }
    }

    private void ChooseRotationPointAndDirection()
    {
        if (transform.position.x > bottleRef.transform.position.x)
        {
            chosenSide = leftSide;
            directionMultiplier = -1f;
            Debug.Log("Left side chosen");
        }
        else
        {
            chosenSide = rightSide;
            directionMultiplier = 1f;
            Debug.Log("Right side chosen");
        }
    }


}
