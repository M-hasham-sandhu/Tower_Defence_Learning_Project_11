using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerBuilderUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown towerTypeDropdown;
    [SerializeField] private Button buildButton;
    [SerializeField] private TowerPlacementController placementController;
    [SerializeField] private TowerBuilder towerBuilder;

    private List<TowerType> towerTypes = new List<TowerType>();

    private void Start()
    {
        // Populate dropdown with available tower types from TowerBuilder
        towerTypeDropdown.ClearOptions();
        towerTypes.Clear();

        foreach (var towerData in towerBuilder.allTowers)
        {
            towerTypes.Add(towerData.towerType);
        }

        List<string> options = new List<string>();
        foreach (var type in towerTypes)
        {
            options.Add(type.ToString());
        }
        towerTypeDropdown.AddOptions(options);

        buildButton.onClick.AddListener(OnBuildButtonClicked);
    }

    private void OnBuildButtonClicked()
    {
        int selectedIndex = towerTypeDropdown.value;
        if (selectedIndex >= 0 && selectedIndex < towerTypes.Count)
        {
            TowerType selectedType = towerTypes[selectedIndex];
            placementController.StartPlacingTower(selectedType);
        }
    }
}

