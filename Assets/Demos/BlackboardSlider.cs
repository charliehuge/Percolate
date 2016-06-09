using UnityEngine;

public class BlackboardSlider : MonoBehaviour
{
    [SerializeField] private string _blackboardParamName;

    public void OnSliderChanged(float sliderValue)
    {
        BlackboardUI.Instance.UpdateBlackboard(_blackboardParamName, sliderValue);
    }
}
