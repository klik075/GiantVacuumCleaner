using UnityEngine;

public class TapController : IndividualSingleton<TapController>
{
    TapButton tabButton;
    public void SelectedButton(TapButton button)
    {
        if (tabButton != null)//±âÁ¸ ¹öÆ° ÇØÁ¦
        {
            tabButton.Unselect();//²û
        }

        tabButton = button; //ÇÒ´ç
        tabButton.Select(); //¼±ÅÃ
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        SelectedButton(transform.GetChild(2).GetComponent<TapButton>());//Ã¹ ÅÇÄ­À» °¡Á®¿È
    }
}
