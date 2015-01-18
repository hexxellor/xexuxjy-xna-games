﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

    public class ItemPanelController : MonoBehaviour
    {

        public GameObject itemSlotPrefab;
        dfControl lastControl;
        private ShopItem lastShopItem;


        dfRichTextLabel detailsPanelLabel;
        dfControl imagePanel;

        GameObject modelImageSprite;

        int previousSelection;
        int currentSelection;

        List<string> shopItemNames ;
        List<ShopItem> shopItems ;

        float scrollIncrement;


        public void ControlPress(ActionButton ab)
        {
            dfScrollPanel sp = GameObject.Find("ItemPanel").GetComponent<dfScrollPanel>();
            Vector2 scrollPos = sp.ScrollPosition;

            scrollPos.y += scrollIncrement;
            sp.ScrollPosition = scrollPos;

            if (ab == ActionButton.Move1Up)
            {
                scrollPos.y -= scrollIncrement;
                sp.ScrollPosition = scrollPos;
            }
            else if (ab == ActionButton.Move1Down)
            {
                scrollPos.y += scrollIncrement;
                sp.ScrollPosition = scrollPos;
            }

            previousSelection = currentSelection;

            currentSelection = (int)(shopItems.Count * (sp.VertScrollbar.Value / sp.VertScrollbar.MaxValue));

            ItemSelectionChanged(shopItems[previousSelection], shopItems[currentSelection]);

        }



        // Use this for initialization
        void Awake()
        {
            GameObject panel = GameObject.Find("ShopItemDescriptionLabel");
            detailsPanelLabel = panel.GetComponent<dfRichTextLabel>();

            modelImageSprite = GameObject.Find("ShopItemSpritePanel");

        }


        public void SetData(Shop currentShop)
        {
            shopItemNames = currentShop.GetItemList();
            shopItems = new List<ShopItem>();

            dfScrollPanel container = GameObject.Find("ItemPanel").GetComponent<dfScrollPanel>();

            dfPanel itemPanel = itemSlotPrefab.GetComponent<dfPanel>();

            if (container != null)
            {
                //container.
                foreach (string itemName in shopItemNames)
                {
                    shopItems.Add(new ShopItem(itemName));
                }

                container.Virtualize<ShopItem>(shopItems, itemPanel);

                currentSelection = 0;

                scrollIncrement = container.VertScrollbar.MaxValue / shopItems.Count;
            }

        }


        // Update is called once per frame
        void Update()
        {

            //// test
            //if(Input.GetMouseButton(0))
            //{
            //    dfControl controlUnderMouse = dfInputManager.ControlUnderMouse;
            //    if(controlUnderMouse != null)
            //    {
            //        GameObject go = controlUnderMouse.gameObject;
            //        ShopItemGUI shopItemGUI = go.GetComponent<ShopItemGUI>();
            //        if (shopItemGUI != null)
            //        {
            //            ShopItem currentShopItem = shopItemGUI.ShopItem;
            //            ItemSelectionChanged(lastShopItem, currentShopItem);
            //        }
            //        lastControl = controlUnderMouse;
                    
            //    }
            //}
        }

        public void ItemSelectionChanged(ShopItem previous, ShopItem current)
        {
            if (previous != null)
            {
                previous.Selected = false;
            }


            if (current != null)
            {
                current.Selected = true;
                detailsPanelLabel.Text = current.Item.Name;
                detailsPanelLabel.Text = string.Format("<h2 color=\"yellow\">{0}</h1><p>PWR: {1} CON:{2} INI: {3}</p><p><i>{4}</i></p>", current.Name, 10, 10, 10, current.Item.Description);

                GameObject goPrefab = (GameObject)(Resources.Load(GladiusGlobals.ModelsRoot + current.Item.ShortMeshName));
                if (goPrefab != null)
                {
                    modelImageSprite.GetComponent<ModelWindowHolder>().AttachedModelPrefabToWindow(goPrefab);
                }
                else
                {
                    Debug.LogWarning("Can't find : " + current.Item.ShortMeshName);
                }

                previous = current;
            }
        }


        public void PurchaseItem(ShopItemGUI itemGui)
        {
            //GladiusGlobals.GladiatorSchool.CurrentCharacter.ReplaceItem(itemGui.ShopItem.Item.Name);
        }

    }

