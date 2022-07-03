using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LadderController : MonoBehaviour
{
    [SerializeField] private GameObject playerController;
    [SerializeField] private GameObject playerSpriteObj;
    [SerializeField] private SpriteRenderer playerSpriteRender;
    [SerializeField] private SpriteRenderer[] wandSpriteRender;
    [SerializeField] private LevelGeneration levelGenerator;
    [SerializeField] private Image fadingImage;

    [SerializeField] private float frameTime = 1 / 30f;
    [SerializeField] private float faddingRate = 0.05f;
    private float rateTowardsHoleCenter = 0.1f;
    private bool leaveCurrentRoom = false;
    private bool fallingAnimation = false;
    private bool loadingIntoNewRoom = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerController = collision.gameObject;
            playerController.GetComponent<PlayerController>().canMove = false;
            playerController.GetComponent<Rigidbody2D>().velocity = new Vector3(0f, 0f, 0f);
            playerController.transform.position = Vector3.MoveTowards(playerController.transform.position, this.transform.position + new Vector3(0.5f, 0.5f, 0), rateTowardsHoleCenter);

            wandSpriteRender = playerController.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < wandSpriteRender.Length; i++)
            {
                if(wandSpriteRender[i].gameObject.tag == "Weapon")
                {
                    wandSpriteRender[i].color = new Color(1f, 1f, 1f, 0);
                }
            }
            

            fallingAnimation = true;
            StartCoroutine(fallingAnimationController(frameTime));
        }
    }

    IEnumerator fallingAnimationController(float frameTime)
    {
        while(fallingAnimation)
        {
            yield return new WaitForSeconds(frameTime);

            playerSpriteObj.transform.localScale -= new Vector3(0.015f, 0.015f, 0f);
            playerSpriteRender.color = new Color(1f, 1f, 1f, playerSpriteRender.color.a - faddingRate);
            fadingImage.color = new Color(0f, 0f, 0f, fadingImage.color.a + faddingRate);
            playerController.transform.position = Vector3.MoveTowards(playerController.transform.position, this.transform.position + new Vector3(0.5f, 0.5f, 0), rateTowardsHoleCenter);

            if (playerSpriteObj.transform.localScale.x <= 0.4f)
            {
                fallingAnimation = false;
                levelGenerator.ClearMap();
                levelGenerator.Start();

                playerSpriteObj.transform.localScale = new Vector3(1f, 1f, 1f);
                playerSpriteRender.color = new Color(1f, 1f, 1f, 1f);
                playerController.GetComponent<PlayerController>().canMove = true;

                for (int i = 0; i < wandSpriteRender.Length; i++)
                {
                    wandSpriteRender[i].color = new Color(1f, 1f, 1f, 1f);
                }

                loadingIntoNewRoom = true;
            }
        }

        while(loadingIntoNewRoom)
        {
            yield return new WaitForSeconds(0.5f * frameTime);

            fadingImage.color = new Color(0f, 0f, 0f, fadingImage.color.a - faddingRate);

            if (fadingImage.color.a <= 0f)
            {
                loadingIntoNewRoom = false;
            }
        }
    }

    private void FixedUpdate()
    {
        
    }
}
