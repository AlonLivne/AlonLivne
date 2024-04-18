using UnityEngine;
using UnityEngine.UI;

public class ProgressionView : MonoBehaviour
{
    [SerializeField] private Slider _progressionSlider;
    [SerializeField] private Text _progressionText;
    [SerializeField] private Transform _prizePlaceHolder;
    [SerializeField] private bool _isLocked;
    
    public void Initialize(int currentValue, int maxValue, GameObject image = null)
    {
        VisualSync(currentValue, maxValue);
        if (image != null)
        {
            SetImage(image);
        }
    }

    public void VisualSync(int currentValue, int maxValue)
    {
        SetText(currentValue, maxValue);
        SetSlider((float)currentValue/maxValue);
    }
    
    private void SetSlider(float currentValue)
    {
        _progressionSlider.value = currentValue;
    }

    private void SetImage(GameObject image)
    {
        image.transform.SetParent(_prizePlaceHolder);
    }

    private void SetText(int currentValue, int maxValue)
    {
        _progressionText.text = ($"{currentValue}/{maxValue}");
    }
}
