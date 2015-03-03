using UnityEngine;
using System.Collections;

public class EndEarlyState : GameState
{
    public CashBasherManager manager;

    public GameObject endGameDialogue, endEarlyScreen;

    public float targetX = -70;

    private Vector3 targetPos, velocityRef;

    private bool showLeaveOption = false;

    public override void Prepare()
    {
        targetPos = new Vector3(targetX, endGameDialogue.transform.position.y);
        endEarlyScreen.SetActive(true);

        StartCoroutine(Preshow());
    }

    IEnumerator Preshow()
    {
        yield return new WaitForSeconds(0.5f);

        StarCounter counter = FindObjectOfType<StarCounter>();

        counter.TransferStars();

        yield return new WaitForSeconds(counter.GetStarGiveDuration());

        showLeaveOption = true;
    }

    public override void UpdateState()
    {
        endGameDialogue.transform.position = Vector3.SmoothDamp(endGameDialogue.transform.position, targetPos, ref velocityRef, 0.3f);
    }

    public override void OnStateGUI()
    {
        if(showLeaveOption)
        {
            if (GUI.Button(new Rect(Screen.width * (0.75f), Screen.height * (0.05f), Screen.width * (0.22f), Screen.height * (0.2f)),
                           "Continue"))
            {
                Network.Disconnect();
                Application.LoadLevel("MiniGameMenu");
            }
        }
    }
}
