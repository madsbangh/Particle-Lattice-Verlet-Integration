using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ClothMenu : MonoBehaviour
{
    [SerializeField]
    private VerletCloth cloth; 

    [Header("Menu")]
    [SerializeField]
    private Button menuBtn;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private Button resetBtn; 

    [Header("Menu Functions")]
    [SerializeField]
    private Slider clothSizeX;
    [SerializeField]
    private Slider clothSizeY;
    [SerializeField]
    private Slider restLengthSlider;
    [SerializeField]
    private Toggle applyGravityToggle; 



    // Use this for initialization
    void Start()
    {
        menuBtn.onClick.AddListener(() => MenuButton());
        resetBtn.onClick.AddListener(() => ResetButton());
        restLengthSlider.onValueChanged.AddListener(UpdateRestLength);
        applyGravityToggle.onValueChanged.AddListener(UpdateGravity);
        clothSizeX.onValueChanged.AddListener(UpdateClothSizeX);
        clothSizeY.onValueChanged.AddListener(UpdateClothSizeY);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MenuButton()
    {
        menuPanel.SetActive(!menuPanel.activeInHierarchy);
    }

    public void ResetButton()
    {
        cloth.ResetSimulation(); 
    }

    public void UpdateClothSizeX(float value)
    {
        cloth.SetSizeX((int)value);
        cloth.UpdateCloth();
    }

    public void UpdateClothSizeY(float value)
    {
        cloth.SetSizeY((int)value);
        cloth.UpdateCloth();
    }

    public void UpdateRestLength(float value)
    {
        cloth.SetRestLength(value);
    }

    public void UpdateGravity(bool value)
    {
        cloth.SetGravity(value); 
    }
}
