using UnityEngine;
using UnityEngine.UI;
using TMPro; // If you use TextMeshPro

public class EvolutionUI : MonoBehaviour
{
    [Header("Button 1 References")]
    [SerializeField] private Button button1;
    [SerializeField] private TMP_Text button1Name;
    [SerializeField] private TMP_Text button1Description;
    [SerializeField] private Image button1Image;

    [Header("Button 2 References")]
    [SerializeField] private Button button2;
    [SerializeField] private TMP_Text button2Name;
    [SerializeField] private TMP_Text button2Description;
    [SerializeField] private Image button2Image;

    private EvolutionOption option1;
    private EvolutionOption option2;

    private EvolutionManager evolutionManager;

    private void OnEnable()
    {
        evolutionManager = GameObject.FindFirstObjectByType<EvolutionManager>();

        EvolutionOption[] options = evolutionManager.GetRandomChoices();
        option1 = options[0];
        option2 = options[1];

        button1Name.text = option1.optionName;
        button1Description.text = option1.description;
        button1Image.sprite = option1.icon;

        button2Name.text = option2.optionName;
        button2Description.text = option2.description;
        button2Image.sprite = option2.icon;

   
        button1.onClick.AddListener(() => Choose(option1, option2));
        button2.onClick.AddListener(() => Choose(option2, option1));
    }

    private void Choose(EvolutionOption chosen, EvolutionOption notChosen)
    {
        Debug.Log($"[EVOLUTION] Player chose: {chosen.optionName} | Enemies gain: {notChosen.optionName}");

        evolutionManager.ApplyChoice(chosen, notChosen);
        // Hide UI after choosing
        gameObject.SetActive(false);
    }

}
