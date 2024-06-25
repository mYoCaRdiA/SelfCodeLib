using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChildAlphaSlider_Element : UiElement
{
    List<Material> bindMats;
    public Slider slider;
    public Button viewButton;
    public Sprite visableSprite;
    public Sprite invisableSprite;
    public Text title;
    public Text percentShow;



    public void Ini(List<Material> allMats, string titleValue)
    {
        title.text = titleValue;
        gameObject.name = titleValue;
        bindMats = allMats;
        SetAlphaValue(1);
        viewButton.onClick.AddListener(() =>
        {
            if (viewButton.image.sprite == visableSprite)
            {
                SetAlphaValue(0);
            }
            else
            {
                SetAlphaValue(1);
            }
        });

        slider.onValueChanged.AddListener(SetAlphaValue);
        
    }



    public void SetAlphaValue(float value)
    {
        if (value == 0)
        {
            SetVisable(false);
        }
        else if (value == 1)
        {
            SetVisable(true);
        }
        else
        {
            foreach (var item in bindMats)
            {
                item.color = new Color(item.color.r, item.color.g, item.color.b, value);
            }
            viewButton.image.sprite = visableSprite;
            slider.value = value;
            float targetValue = value * 100;
            percentShow.text = targetValue.ToString("0.0") + "%";
        }

        void SetVisable(bool couldView)
        {
            if (!couldView)
            {
                foreach (var item in bindMats)
                {
                    item.color = new Color(item.color.r, item.color.g, item.color.b, 0);
                }
                viewButton.image.sprite = invisableSprite;
                slider.value = 0;
                percentShow.text = "0%";
            }
            else
            {
                foreach (var item in bindMats)
                {
                    item.color = new Color(item.color.r, item.color.g, item.color.b, 1);
                }
                viewButton.image.sprite = visableSprite;
                slider.value = 1;
                percentShow.text = "100%";
            }
        }
    }




}
