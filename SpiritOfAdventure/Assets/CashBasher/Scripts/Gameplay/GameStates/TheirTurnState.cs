using UnityEngine;
using System.Collections;

public class TheirTurnState : GameState 
{
    CashBasherManager manager;
    CameraMan cameraMan;

    public TheirTurnState(CashBasherManager m, CameraMan cm)
    {
        manager = m;
        cameraMan = cm;
    }

    public void Prepare()
    {
        //if (Network.isServer)
        //{
        //    cameraMan.FollowPosition(new Vector3(-4.5f, 1f, 0f));
        //}
        //else
        //{
        //    cameraMan.FollowPosition(new Vector3(4.5f, 1f, 0f));
        //}

        manager.StartCoroutine(Preshow());
    }

    IEnumerator Preshow()
    {
        if (manager.HasEffects(Network.isClient))
        {
            if (Network.isServer)
            {
                cameraMan.FollowPosition(new Vector3(10f, 0f, 0f));
            }
            else
            {
                cameraMan.FollowPosition(new Vector3(-10f, 0f, 0f));
            }

            cameraMan.ZoomTo(5f);

            yield return new WaitForSeconds(3.0f);
        }

        if (Network.isServer)
        {
            cameraMan.FollowPosition(new Vector3(4.5f, 1f, 0f));
        }
        else
        {
            cameraMan.FollowPosition(new Vector3(-4.5f, 1f, 0f));
        }

        cameraMan.ZoomTo(7f);
    }

    public void Update()
    {

    }

    public void OnGUI()
    {

    }

    public void End()
    {

    }
}
