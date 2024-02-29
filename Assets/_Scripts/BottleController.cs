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
    [SerializeField] private ParticleSystem smoke;
    [Range(0, 4)]
    public int numberOfColorsInBottle = 4;
    public int numberOfTopColorLayers = 1;
    [SerializeField] private bool justThisBottle;
    [SerializeField] private int numberOfTransferColor;
    [SerializeField] private Vector3 originalPos, startPos, endPos;
    [SerializeField] private LineRenderer lineRenderer;

    private bool playedSound = false;
    private bool pourSound = false;

    public static BottleController Instance { get; private set; }
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
        UpdateColorsInBottle();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateTopColor();
        CheckBottleFillLevel();
    }

    public void StartTransferColor()
    {

        ChooseRotationPointAndDirection();
        numberOfTransferColor = Mathf.Min(numberOfTopColorLayers, 4 - bottleRef.numberOfColorsInBottle);
        for (int i = 0; i < numberOfTransferColor; i++)
        {
            bottleRef.bottleColors[bottleRef.numberOfColorsInBottle + i] = topColor;
            Debug.Log("transfer color " + topColor);
            Debug.Log("receive color " + bottleRef.bottleColors[bottleRef.numberOfColorsInBottle + i]);
            bottleRef.UpdateColorsInBottle();
        }

        CalculateRotationIndex(4 - bottleRef.numberOfColorsInBottle);

        transform.GetComponent<SpriteRenderer>().sortingOrder += 5;
        bottleMask.sortingOrder += 5;
        bottleRef.numberOfColorsInBottle += numberOfTransferColor;
        StartCoroutine("MoveBottle");
    }

    public void UpdateColorsInBottle()
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
                    Color pourColor = new Color(topColor.r, topColor.g, topColor.b, 1);
                    float streamHeight = 5f;

                    lineRenderer.startColor = pourColor;
                    lineRenderer.endColor = pourColor;

                    lineRenderer.SetPosition(0, chosenSide.position);
                    lineRenderer.SetPosition(1, chosenSide.position - Vector3.up * streamHeight);

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

        numberOfColorsInBottle -= numberOfTransferColor;
        //bottleRef.numberOfColorsInBottle += numberOfTransferColor;

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
                            numberOfTopColorLayers = 4;
                            Invoke("BottleComplete", 1.5f);
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

    private void BottleComplete()
    {
        float colorOffset = 1.5f;
        Debug.Log(numberOfTopColorLayers);
        Color smokeColor = new Color(topColor.r * colorOffset, topColor.g * colorOffset, topColor.b * colorOffset, 0.004f);
        Vector3 vfxSpawnPos = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
        Vector3 confettiPos = new Vector3(transform.position.x, transform.position.y + 1.7f, transform.position.z);
        if (!playedSound)
        {
            AudioManager.Instance.PlaySFX("ContainerFinish"); // Play the sound here
            Instantiate(confetti, confettiPos, Quaternion.identity);
            smoke.startColor = smokeColor;
            Instantiate(sparkling, vfxSpawnPos, Quaternion.Euler(-90f, 0, 0));
            Instantiate(smoke, vfxSpawnPos, Quaternion.Euler(-90f, 0, 0));
            playedSound = true;
        }

    }

}
