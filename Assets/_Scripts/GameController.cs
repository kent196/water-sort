using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private BottleController firstBottle, secondBottle;
    public static GameController instance;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        InputCheck();
    }

    private void InputCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2d, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<BottleController>())
                {
                    BottleController clickedBottle = hit.collider.GetComponent<BottleController>();

                    if (firstBottle == null)
                    {
                        if (clickedBottle.numberOfColorsInBottle == 0 || clickedBottle.numberOfTopColorLayers == 4)
                        {
                            // Do something if the first bottle has 0 colors
                            Debug.Log("First bottle has 0 colors");
                        }
                        else
                        {
                            firstBottle = clickedBottle;
                            if (firstBottle.clickAble)
                            {
                                firstBottle.transform.position = new Vector3(firstBottle.transform.position.x, firstBottle.transform.position.y + 0.5f, firstBottle.transform.position.z);
                            }
                        }
                    }
                    else
                    {
                        if (firstBottle == hit.collider.GetComponent<BottleController>())
                        {
                            if (firstBottle.clickAble)
                            {
                                firstBottle.transform.position = new Vector3(firstBottle.transform.position.x, firstBottle.transform.position.y - 0.5f, firstBottle.transform.position.z);
                            }
                            firstBottle = null;
                        }
                        else
                        {
                            secondBottle = hit.collider.GetComponent<BottleController>();
                            firstBottle.bottleRef = secondBottle;

                            firstBottle.UpdateTopColor();
                            secondBottle.UpdateTopColor();

                            if (secondBottle.FillBottleCheck(firstBottle.topColor))
                            {
                                firstBottle.StartTransferColor();
                                firstBottle = null;
                                secondBottle = null;
                            }
                            else
                            {
                                firstBottle.transform.position = new Vector3(firstBottle.transform.position.x, firstBottle.transform.position.y - 0.5f, firstBottle.transform.position.z);
                                firstBottle = null;
                                secondBottle = null;
                            }
                        }
                    }
                }
                else if (hit.collider.GetComponent<Button>())
                {
                    Debug.Log("Click");
                    //AudioManager.Instance.PlaySFX("Click");
                }
            }
        }
    }

    // This method should be called when the transfer is complete
    //public void OnTransferComplete()
    //{
    //    isTransferInProgress = false; // Reset the flag
    //    firstBottle = null;
    //    secondBottle = null;
    //}
}
