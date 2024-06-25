using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainAlphaSlider_Element : UiElement
{
    public Slider slider;
    public Button viewButton;
    public Sprite visableSprite;
    public Sprite invisableSprite;
    public Text title;
    public Text percentShow;

    public Button showChildButton;
    public GameObject childSliderFather;
    List<ChildAlphaSlider_Element> bindChildSliders;
    public void Ini(List<ChildAlphaSlider_Element> allChildSliders, string titleValue)
    {
        title.text = titleValue;
        gameObject.name = titleValue;
        bindChildSliders = allChildSliders;
        slider.value = 1;
        viewButton.image.sprite = visableSprite;
        foreach (ChildAlphaSlider_Element sliderTemp in bindChildSliders)
        {
            sliderTemp.SetAlphaValue(1);
        }
        percentShow.text = "100%";

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

        childSliderFather.SetActive(false);
        showChildButton.transform.eulerAngles = new Vector3(0, 0, 0);

        showChildButton.onClick.AddListener(() =>
        {
            if (childSliderFather.activeSelf == true)
            {
                ChangeChildListStatus(false);
            }
            else
            {
                ChangeChildListStatus(true);
            }

        });



    }

    public void ChangeChildListStatus(bool show)
    {
        childSliderFather.SetActive(show);
        if (show)
        {
            showChildButton.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
            showChildButton.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(childSliderFather.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
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
            foreach (ChildAlphaSlider_Element sliderTemp in bindChildSliders)
            {
                sliderTemp.SetAlphaValue(value);
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
                foreach (ChildAlphaSlider_Element sliderTemp in bindChildSliders)
                {
                    sliderTemp.SetAlphaValue(0);
                }
                viewButton.image.sprite = invisableSprite;
                slider.value = 0;
                percentShow.text = "0%";
            }
            else
            {
                foreach (ChildAlphaSlider_Element sliderTemp in bindChildSliders)
                {
                    sliderTemp.SetAlphaValue(1);
                }
                viewButton.image.sprite = visableSprite;
                slider.value = 1;
                percentShow.text = "100%";
            }
        }
    }




}
