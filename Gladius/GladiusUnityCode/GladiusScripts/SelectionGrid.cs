using UnityEngine;
using System.Collections;
using System;

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

    public int NumRequired;
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

                SetSlot(i, j, EmptySlot);
            }
        }
    }

    public void SetStartDefault(int numRequired,int numAvailable)
    {
        NumRequired = numRequired;
        NumAvailable = numAvailable;

        int diff = AvailableSlots - Slots.Length;
        if (diff >= 0)
        {
            NumAvailable -= (diff + 1);
        }

        int count = 0;
        for (int i = 0; i < NumRequired; ++i)
        {
            //SetSlot(count++, RequiredSlot);
            UnassignedTextures[count++] = RequiredSlot;
        }
        for (int i = 0; i < NumAvailable; ++i)
        {
            //SetSlot(count++, AvailableSlot);
            UnassignedTextures[count++] = AvailableSlot;
        }
        for (int i = count; i < Slots.Length; ++i)
        {
            //SetSlot(count++, UnavailableSlot);
            UnassignedTextures[count++] = UnavailableSlot;
        }
        for (int i = 0; i < Slots.Length; ++i)
        {
            SetSlot(i, null);
        }

    }



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

    public void SetSlot(int index, String texture, object tag = null)
    {
        int x = index % GridWidth;
        int y = index / GridWidth;
        SetSlot(x, y, texture,tag);
    }

    public void SetSlot(int x, int y, string texture,object tag=null)
    {
        DebugUtils.Assert(x >= 0 && x < GridWidth && y >= 0 && y < GridHeight);
        int index = (y * GridWidth) + x;
        SetSlot(Slots[index], texture, tag);
    }

    public void SetSlot(dfButton control, String texture, object tag = null)
    {
        
        int slotId = -1;
        for(int i=0;i<Slots.Length;i++)
        {
            if(Slots[i] == control)
            {
                slotId = i;
                break;
            }
        }
        
        if (texture == null)
        {
            texture = UnassignedTextures[slotId];
            tag = null;
        }
        control.BackgroundSprite = texture;
        control.Tag = tag;
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
        return Slots[index].Tag == null;
    }

    public int AvailableSlots
    { get { return NumRequired + NumAvailable; } }

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

}



