using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype.NetworkLobby
{

    public class CharacterSelection : MonoBehaviour
    {
        private GameObject[] characterList;

        private LobbyMainMenu menu;

        private int index;

        private void Start()
        {
            index = PlayerPrefs.GetInt("CharacterSelected");
            //Debug.Log(index);

            characterList = new GameObject[transform.childCount];

            //Si riempie l'array con i modelli
            for (int i = 0; i < transform.childCount; i++)
                characterList[i] = transform.GetChild(i).gameObject;

            //Si disattivano tutti inizialmente
            foreach (GameObject go in characterList)
                go.SetActive(false);

            //Si attiva solo il modello selezionato
            if (characterList[index])
                characterList[index].SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }

        public void ToggleLeft()
        {
            //Si nasconde il modello corrente
            characterList[index].SetActive(false);

            index--;

            if (index < 0)
            {
                index = characterList.Length - 1;
            }

            //Si mostra il "nuovo" modello corrente 
            characterList[index].SetActive(true);
        }

        public void ToggleRight()
        {
            //Si nasconde il modello corrente
            characterList[index].SetActive(false);

            index++;

            if (index == characterList.Length)
            {
                index = 0;
            }

            //Si mostra il "nuovo" modello corrente 
            characterList[index].SetActive(true);
        }

        public void SelectButton()
        {

            PlayerPrefs.SetInt("CharacterSelected", index);
            //MenuSelection.Instance.EnablePanel(MenuSelection.Instance.Panels[1]);
            //SceneManager.LoadScene("MainMenu");
        }

        public void SendSelectionMessage()
        {
         
            //Send character selected to server
            GameObject[] lobbyPlayers = GameObject.FindGameObjectsWithTag("PlayerInfo");

            foreach (GameObject player in lobbyPlayers)
            {

                LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer>();

                if (lobbyPlayer.isLocalPlayer)
                    lobbyPlayer.SelectCharacter(index);
            }

           
      
        }
    }
}
