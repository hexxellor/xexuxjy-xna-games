using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SelectionGrid : MonoBehaviour
{
    public int GridWidth = 5;
    public int GridHeight = 2;

    public dfPanel GroupPanel;
    public dfButton[] Slots;
    public String[] UnassignedTextures;
    public string EmptySlot = "blocker.tga";
    public string UnavailableSlot = "blocker.tga";
    public string AvailableSlot = "emptyi.tga";
    public string RequiredSlot = "required.tga";

    public int NumAvailable;

    public Rectangle SlotDims = new Rectangle(0, 0, 40, 60);

    public int MaxSize
    { get { return GridWidth * GridHeight; } }

    public void Init(dfControl owner,String name,MouseEventHandler onClick)
    {
        GroupPanel = owner.AddControl<dfPanel>();
        GroupPanel.name = name;
        GroupPanel.RelativePosition = new Vector2();
        Slots = new dfButton[GridWidth * GridHeight];
        UnassignedTextures = new string[Slots.Length];

        Vector2 ownerSize = owner.Size;
        float slotWidth = ownerSize.x / GridWidth;
        float slotHeight = ownerSize.y / GridHeight;


        GroupPanel.Size = new Vector2(GridWidth * SlotDims.Width, GridHeight * SlotDims.Height);
        for (int i = 0; i < GridWidth; ++i)
        {
            for (int j = 0; j < GridHeight; j++)
            {
                int index = (j * GridWidth) + i;
                Slots[index] = GroupPanel.AddControl<dfButton>();
                Slots[index].RelativePosition = new Vector3(i * slotWidth, j * slotHeight, 0);
                Slots[index].Width = slotWidth;
                Slots[index].Height = slotHeight;
                Slots[index].Click += onClick;
                SlotData slotData = new SlotData();
                slotData.index = index;
                Slots[index].Tag = slotData;
                SetSlot(i, j, null);
            }
        }
    }

    public void SetStartDefault(List<CharacterData> characterList)
    {
        int count = 0;
        NumAvailable = characterList.Count;
        for (int i = 0; i < characterList.Count; ++i)
        {
            SlotData slotData = Slots[count].Tag as SlotData;
            slotData.Required = characterList[count];
            UnassignedTextures[count] = (characterList[count].RequiredMask != 0) ? RequiredSlot : AvailableSlot;
            count++;
        }
        for (int i = count; i < Slots.Length; ++i)
        {
            UnassignedTextures[count++] = UnavailableSlot;
        }
        for (int i = 0; i < Slots.Length; ++i)
        {
            Slots[i].BackgroundSprite = UnassignedTextures[i];
        }

    }

    //public void SetStartDefault(int numRequired, int numAvailable)
    //{
    //    NumRequired = numRequired;
    //    NumAvailable = numAvailable;

    //    int diff = AvailableSlots - Slots.Length;
    //    if (diff >= 0)
    //    {
    //        NumAvailable -= (diff + 1);
    //    }

    //    int count = 0;
    //    for (int i = 0; i < NumRequired; ++i)
    //    {
    //        //SetSlot(count++, RequiredSlot);
    //        UnassignedTextures[count++] = RequiredSlot;
    //    }
    //    for (int i = 0; i < NumAvailable; ++i)
    //    {
    //        //SetSlot(count++, AvailableSlot);
    //        UnassignedTextures[count++] = AvailableSlot;
    //    }
    //    for (int i = count; i < Slots.Length; ++i)
    //    {
    //        //SetSlot(count++, UnavailableSlot);
    //        UnassignedTextures[count++] = UnavailableSlot;
    //    }
    //    for (int i = 0; i < Slots.Length; ++i)
    //    {
    //        SetSlot(i, null);
    //    }

    //}



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Cleanup()
    {

    }

    public void SetSlot(int index, CharacterData characterData)
    {
        int x = index % GridWidth;
        int y = index / GridWidth;
        SetSlot(x, y, characterData);
    }

    public void SetSlot(int x, int y, CharacterData characterData)
    {
        DebugUtils.Assert(x >= 0 && x < GridWidth && y >= 0 && y < GridHeight);
        int index = (y * GridWidth) + x;
        SetSlot(Slots[index], characterData);
    }

    public void SetSlot(dfButton control, CharacterData characterData)
    {
        SlotData slotData = control.Tag as SlotData;
        slotData.Current = characterData;
        string texture = slotData.Current == null ?UnassignedTextures[slotData.index] :characterData.ThumbNailName;
        control.BackgroundSprite = texture;
    }

    public int NextAvailableSlot()
    {
        int result = -1;
        for (int i = 0; i < Slots.Length; ++i)
        {
            //if (Slots[i].BackgroundSprite == EmptySlot)
            if(IsSlotEmpty(i))
            {
                result = i;
                break;
            }
        }

        return result;
    }

    public bool IsSlotEmpty(int index)
    {
        SlotData slotData = Slots[index].Tag as SlotData;
        return slotData.Current == null;
    }

    public int AvailableSlots
    { get { return NumAvailable; } }

    public int FilledSlots
    {
        get
        {
            int count = 0;
            for (int i = 0; i < AvailableSlots; ++i)
            {
                if (!IsSlotEmpty(i))
                {
                    count++;
                }
            }
            return count;
        }
    }

    public int EmptySlots
    {
        get
        {
            return AvailableSlots - FilledSlots;
        }
    }

    public void FillParty(List<CharacterData> party)
    {
        party.Clear();
        for (int i = 0; i < Slots.Length; ++i)
        {
            SlotData slotData = Slots[i].Tag as SlotData;
            if (slotData.Current != null)
            {
                party.Add(slotData.Current);
            }
        }
    }

}

public class SlotData
{
    public CharacterData Required;
    public CharacterData Current;
    public int index = 0;
}



