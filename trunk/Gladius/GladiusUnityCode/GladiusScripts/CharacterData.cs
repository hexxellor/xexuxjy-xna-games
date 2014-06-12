using System.Xml;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

namespace Gladius
{

    public class CharacterData
    {

        //<Character Name="Urlan">
        //   <Attributes Accuracy ="40" Defense= "60" Power= "60" Consitution= "80" Experience="1000" Level ="1"/>
        //   <Skills>

        //   </Skills>
        //   <Equipment Head ="" Arm1="" Arm2="" Body="" Special=""/>
        // </Character>        

        public void InitValues()
        {
            m_attributeDictionary[GameObjectAttributeType.Accuracy] = new BoundedAttribute(10);
            m_attributeDictionary[GameObjectAttributeType.Power] = new BoundedAttribute(10);
            m_attributeDictionary[GameObjectAttributeType.Defense] = new BoundedAttribute(10);
            m_attributeDictionary[GameObjectAttributeType.Constitution] = new BoundedAttribute(10);
            m_attributeDictionary[GameObjectAttributeType.CharacterSkillPoints] = new BoundedAttribute(10);
            m_attributeDictionary[GameObjectAttributeType.Experience] = new BoundedAttribute(10);

            // go through all the other attributes and add them if empty.
            foreach (GameObjectAttributeType attr in EnumUtil.GetValues<GameObjectAttributeType>())
            {
                BoundedAttribute ba = null;
                if (!m_attributeDictionary.TryGetValue(attr, out ba))
                {
                    m_attributeDictionary[attr] = new BoundedAttribute(0);
                }
            }


        }

        public void Load(XmlElement xmlElement)
        {
            SetupCharacterData(xmlElement);
        }

        public void Save(StreamWriter streamWriter)
        {
            StringBuilder data = new StringBuilder();
            data.AppendFormat("<Character Name=\"{0}\" Class=\"{1}\" Level=\"{2}\">", Name, ActorClass.ToString(), Level);
            data.Append("<Attributes ");
            foreach (GameObjectAttributeType attrType in m_attributeDictionary.Keys)
            {
                data.AppendFormat("{0}=\"{1}\" ", attrType, m_attributeDictionary[attrType].CurrentValue);
            }
            data.Append(" />");

            data.AppendFormat("<Equipment Head=\"{0}\" Arm1=\"{1}\" Arm2=\"{2}\" Body=\"{3}\" Special=\"{4}\" />\n",
            m_items[(int)ItemLocation.Helmet], m_items[(int)ItemLocation.Shield], m_items[(int)ItemLocation.Weapon],
            m_items[(int)ItemLocation.Armor], m_items[(int)ItemLocation.Accessory]);

            data.Append("</Character>");
            streamWriter.WriteLine(data);
        }

        public int XPToNextLevel
        {
            get
            {
                if (Level < GladiusGlobals.MaxLevel - 2)
                {
                    int nextlevel = ActorGenerator.ActorXPLevels[ActorClass][Level + 1];
                    return nextlevel - m_attributeDictionary[GameObjectAttributeType.Experience].CurrentValue;
                }
                return -1;
            }
        }


        public String Name
        {
            get;
            set;
        }

        public String ThumbNailName
        {
            get;
            set;
        }


        public int Level
        {
            get;
            set;
        }

        public int Experience
        {
            get;
            set;
        }

        public int JobPoints
        {
            get;
            set;
        }

        public ActorClass ActorClass
        {
            get;
            set;
        }

        public Point? StartPosition
        { get; set; }


        public Item GetItemAtLocation(ItemLocation location)
        {
            return m_items[(int)location];
        }

        //            	CON, PWR, ACC, DEF, INT, MOVE
        public int CON
        { get; set; }

        public int PWR
        { get; set; }

        public int ACC
        { get; set; }

        public int DEF
        { get; set; }

        public int INT
        { get; set; }

        public int MOVE
        { get; set; }

        /*
         *   <Character Name="" Accuracy ="" Defense= "" Power= "" Consitution= "" Experience="" Level ="">
        <Skills>

        </Skills>
        <Equipment Head ="" Arm1="" Arm2="" Body="" Special=""/>
        </Character>
        */
        public void SetupCharacterData(XmlElement element)
        {
            Name = element.Attributes["Name"].Value;
            ThumbNailName = element.Attributes["ThumbnailName"].Value;

            ActorClass = (ActorClass)Enum.Parse(typeof(ActorClass), element.Attributes["Class"].Value);
            Level = int.Parse(element.Attributes["Level"].Value);
            m_male = (element.Attributes["Sex"].Value == "M");

            XmlElement attributes = (XmlElement)element.SelectSingleNode("Attributes");
            foreach (XmlAttribute attr in attributes.Attributes)
            {
                try
                {
                    GameObjectAttributeType attrType = (GameObjectAttributeType)Enum.Parse(typeof(GameObjectAttributeType), attr.Name);
                    int val = int.Parse(attr.Value);
                    m_attributeDictionary[attrType] = new BoundedAttribute(val);
                }
                catch (System.Exception ex)
                {

                }

            }

        }

        public void AddItem(String itemKey)
        {
            Item item;
            if (GladiusGlobals.ItemManager.TryGetValue(itemKey, out item))
            {
                m_items[(int)item.Location] = item;
                UpdateStats();
            }
        }

        public void RemoveItem(String itemKey)
        {
            Item item;
            if (GladiusGlobals.ItemManager.TryGetValue(itemKey, out item))
            {
                m_items[(int)item.Location] = null;
                UpdateStats();
            }
        }

        private void UpdateStats()
        {
        }

        public Dictionary<GameObjectAttributeType, BoundedAttribute> Attributes
        {
            get
            {
                return m_attributeDictionary;
            }
        }

        
        public int XP
        {
            get 
            {
                return m_attributeDictionary[GameObjectAttributeType.Experience].CurrentValue;
            }
            set 
            {
                m_attributeDictionary[GameObjectAttributeType.Experience].CurrentValue = value;
            }



        }

        public String GetInfoString()
        {
            if (m_infoString == null)
            {
                StringBuilder sb = new StringBuilder();
                //sb.AppendFormat("*** {0} ***\n", Name);
                //sb.Append("\nClass : " + ActorClass);
                //sb.Append("\nLevel : " + Level);
                //sb.Append("\nPower : " + m_attributeDictionary[GameObjectAttributeType.Power].CurrentValue);
                //sb.Append("\nAccuracy : " + m_attributeDictionary[GameObjectAttributeType.Accuracy].CurrentValue);
                //sb.Append("\nDefense : " + m_attributeDictionary[GameObjectAttributeType.Defense].CurrentValue);
                //sb.Append("\nConstitution : " + m_attributeDictionary[GameObjectAttributeType.Constitution].CurrentValue);
                //sb.Append("\nXP : " + m_attributeDictionary[GameObjectAttributeType.Experience].CurrentValue);

                m_infoString = sb.ToString();
            }
            return m_infoString;
        }


        public void AddSkill(String skillName)
        {
            m_skillList.Add(skillName);
        }

        public void AddItemByNameAndLoc(String itemName,ItemLocation loc)
        {
            if (!String.IsNullOrEmpty(itemName))
            {
                m_itemNames[(int)loc] = itemName;
            }
        }

        public String GetItemNameAtLoc(ItemLocation loc)
        {
            return m_itemNames[(int)loc];
        }

        public bool Selected
        {
            get;
            set;
        }
        private bool m_male;
        private Item[] m_items = new Item[(int)ItemLocation.NumItems];
        private String[] m_itemNames = new String[(int)ItemLocation.NumItems];
        private Dictionary<GameObjectAttributeType, BoundedAttribute> m_attributeDictionary = new Dictionary<GameObjectAttributeType, BoundedAttribute>();
        private String m_infoString;
        public List<String> m_skillList = new List<String>();



    }
}