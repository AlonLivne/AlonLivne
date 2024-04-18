using UnityEngine;

public class HSPanelButton : MonoBehaviour
{
    private static HSPanelButton _selectedButton;
    [SerializeField] private Canvas _controlledCanvas;
    [SerializeField] private Animator _aniamtor;
    
    public void OnClick()
    {
        if (_selectedButton == this)
        {
            return;
        }

        SelectButton(this);
    }

    private static void SelectButton(HSPanelButton hsPanelButton)
    {
        _selectedButton?.Deselect();
        _selectedButton = hsPanelButton;
        _selectedButton.Select();
    }

    private void Deselect()
    {
        _controlledCanvas?.gameObject?.SetActive(false);
        _aniamtor.SetTrigger("deselect");
    }

    private void Select()
    {
        _controlledCanvas?.gameObject?.SetActive(true);
        _aniamtor.SetTrigger("select");
    }
}
