using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Sprite[] defaultSprites;
    public Sprite[] pausedSprites;  

    private bool isPaused = false;  
    private Image buttonImage;     

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = defaultSprites[0]; 
    }

    public void OnClick()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Time.timeScale = 0; 
            TimeManager.Instance.PauseByPlayer();
            buttonImage.sprite = pausedSprites[0]; 
        }
        else
        {
            // Time.timeScale = 1; 
            TimeManager.Instance.ResumeByPlayer();
            buttonImage.sprite = defaultSprites[0]; 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateSpriteOnHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateSpriteOnIdle();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateSpriteOnPress();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UpdateSpriteOnHover();
    }

    private void UpdateSpriteOnHover()
    {
        if (isPaused)
        {
            buttonImage.sprite = pausedSprites[1]; // 暂停状态悬停图标
        }
        else
        {
            buttonImage.sprite = defaultSprites[1]; // 悬停图标
        }
    }

    private void UpdateSpriteOnIdle()
    {
        if (isPaused)
        {
            buttonImage.sprite = pausedSprites[0]; // 暂停状态图标
        }
        else
        {
            buttonImage.sprite = defaultSprites[0]; // 默认图标
        }
    }

    private void UpdateSpriteOnPress()
    {
        if (isPaused)
        {
            buttonImage.sprite = pausedSprites[2]; // 暂停状态按下图标
        }
        else
        {
            buttonImage.sprite = defaultSprites[2]; // 按下图标
        }
    }
}
