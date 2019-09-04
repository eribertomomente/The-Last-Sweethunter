using UnityEngine;

public class EnumPanel : MonoBehaviour
{
    public enum ActivePanel
    {
        SelectionPanel,
        MenuPanel,
        LobbyPanel
    }
    public ActivePanel myActivePanel;
}