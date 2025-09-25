using System;
using System.Collections.Generic;
using DG.Tweening;
using Saves;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MenuController : MonoBehaviour
    {
        [Header("Containers")]
        [SerializeField] private RectTransform mainMenuContainer;
        [SerializeField] private RectTransform startContainer;
        [SerializeField] private RectTransform recordsContainer;
    
        [Header("Start Menu")]
        [SerializeField] private Slider levelWidth;
        [SerializeField] private TMP_Text levelWidthText;
    
        [SerializeField] private Slider levelHeight;
        [SerializeField] private TMP_Text levelHeightText;
    
        [SerializeField] private Slider countOfExits;
        [SerializeField] private TMP_Text countOfExitsText;
    
        [Header("Board Menu")]
        [SerializeField] private RectTransform recordsContentContainer;
        [SerializeField] private TMP_Text noRecordsText;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private ResultRecordUIElement recordUIPrefab;

        private SubMenu _currentMenu = SubMenu.Main;

        private enum SubMenu
        {
            Main,
            Records,
            Start,
        }

        private Dictionary<SubMenu, RectTransform> _menuTransformDictionary = new Dictionary<SubMenu, RectTransform>();

        public void Awake()
        {
            _menuTransformDictionary.Add(SubMenu.Main, mainMenuContainer);
            _menuTransformDictionary.Add(SubMenu.Start, startContainer);
            _menuTransformDictionary.Add(SubMenu.Records, recordsContainer);
        }

        private void Start()
        {
            InitBoard();
        }

        #region [Transitions]
    
        public void ClickStart()
        {
            StartMenuResetValues();
            SelectMenu(SubMenu.Start);
        }

        public void ClickLoad()
        {
            SelectMenu(SubMenu.Records);
        }

        public void ClickQuit()
        {
            Application.Quit();
        }

        public void ClickBackToMainMenu()
        {
            SelectMenu(SubMenu.Main, true);
        }

        private void SelectMenu(SubMenu menu, bool reverse = false)
        {
            var offset = Screen.width * 1.5f;

            if (reverse)
            {
                offset *= -1;
            }
        
            if (menu == _currentMenu)
            {
                throw new Exception("trying to select current menu - aborting");
            }

            if (!_menuTransformDictionary.ContainsKey(_currentMenu))
            {
                throw new Exception("No current menu - aborting");
            }
        
            var prevTransform = _menuTransformDictionary[_currentMenu];
        
            prevTransform.DOLocalMoveX(-offset, 0.5f).OnComplete(
                () => prevTransform.gameObject.SetActive(false)
            );
        
            var selectedTransform = _menuTransformDictionary[menu];
            selectedTransform.localPosition = new Vector3(offset, 0, 0);
            selectedTransform.gameObject.SetActive(true);
        
            selectedTransform.DOLocalMoveX(0, 0.5f);
            _currentMenu = menu;
        }

        #endregion

        #region [Start new game]

        private void StartMenuResetValues()
        {
            levelHeight.value = 10;
            levelWidth.value = 10;
            countOfExits.value = 1;
        
            StartMenuUpdateDisplayValues();
        }

        public void StartMenuUpdateDisplayValues()
        {
            levelHeightText.text = $"level height: {levelHeight.value.ToString()}";
            levelWidthText.text = $"level width: {levelWidth.value.ToString()}";
            countOfExitsText.text = $"count of exits: {countOfExits.value.ToString()}";
        }

        public void ClickPlay()
        {
            GameController.Instance.PlayNewGame(
                (int) levelWidth.value, 
                (int) levelHeight.value, 
                (int) countOfExits.value
            );
        }
        #endregion
 
        #region [Board]

        private void InitBoard()
        {
            var data = SaveManager.GetResultRecords();
        
            data.Reverse();
        
            if (data.Count == 0)
            {
                noRecordsText.gameObject.SetActive(true);
                scrollRect.gameObject.SetActive(false);
                return;
            }

            foreach (var recordData in data)
            {
                var recordElement = Instantiate(recordUIPrefab, recordsContentContainer);
                recordElement.Init(recordData);
                recordElement.transform.SetSiblingIndex(0);
            }
        }

        #endregion
    }
}