using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class Item : MonoBehaviour {

    public enum Type
    {
        Gear, Consumable, Material, Key
    }

    public enum Gear
    {
        Weapon, Shield, Head, Chest, Gloves, Pants, Boots, Choker, Earrings, Bracelet, Ring
    }

    public enum Rarity
    {
        Common, Uncommon, Rare, Very_Rare, Legendary
    }

    //This equates to 50% common, 30% uncommon, 15% rare, 5% very rare, 0% legendary
    public static float[] RarityChance = { 1.00f, 0.50f, 0.20f, 0.05f, 0.00f };

    //Crafting rarity chance. Players can only craft common (80%) or uncommon (20%) by default
    public static float[] RarityChanceCrafting = { 1.00f, 0.20f, 0.00f, 0.00f, 0.00f };

    //Upgrade caps per tier. Tier 1 can only go up to +3, tier 2 to +5, etc. For now cap is +5
    public static int[] UpgradeCapByTier = { 1, 3, 5 };

    //Item IDs for upgrade items by tier. Tier 0 is meant to be ignored
    // N/A, Flint
    public static int[] UpgradeIngredientID = { 0, 67 };

    //Item IDs for reroll enchantment items (just rolls randomly). Tier 0 should be ignored
    // N/A, melanite, leucocite, xanthocite, iosite
    public static int[] RerollEnchantmentID = { 0, 68 };

    //Dictionary that matches the item's ID number to the corresponding resource's location
    //If an item is added, the path has to be added here as well
    public static Dictionary<int, string> Path = new Dictionary<int, string>()
    {
        {  1, "Items/Pickup" },
        {  2, "Items/Consumables/Potions/Health Potion" },
        {  3, "Items/Materials/Green Plant" },
        {  4, "Items/Weapons/Swords/Bronze Sword" },
        {  5, "Items/Weapons/Maces/Bronze Mace" },
        {  6, "Items/Weapons/Staves/Oak Staff" },
        {  7, "Items/Weapons/Wands/Oak Wand" },
        {  8, "Items/Weapons/Bows/Oak Bow" },
        {  9, "Items/Armor/Chests/Bronze Cuirass" },
        { 10, "Items/Armor/Chests/Cotton Robe" },
        { 11, "Items/Armor/Heads/Cotton Hat" },
        { 12, "Items/Armor/Heads/Bronze Helm" },
        { 13, "Items/Armor/Heads/Leather Helm" },
        { 14, "Items/Armor/Chests/Leather Jacket" },
        { 15, "Items/Shields/Bronze Shield" },
        { 16, "Items/Materials/Oak Log" },
        { 17, "Items/Materials/Bronze Ore" },
        { 18, "Items/Materials/Cotton Fiber" },
        { 19, "Items/Materials/Leather Scrap" },
        { 20, "Items/Materials/Runestone Shard" },
        { 21, "Items/Materials/Thread" },
        { 22, "Items/Materials/Dirty Water" },
        { 23, "Items/Materials/Red Plant" },
        { 24, "Items/Materials/Blue Plant" },
        { 25, "Items/Materials/Yellow Plant" },
        { 26, "Items/Armor/Pants/Cotton Pants" },
        { 27, "Items/Armor/Pants/Leather Pants" },
        { 28, "Items/Armor/Pants/Bronze Greaves" },
        { 29, "Items/Armor/Boots/Cotton Boots" },
        { 30, "Items/Armor/Boots/Leather Boots" },
        { 31, "Items/Armor/Boots/Bronze Boots" },
        { 32, "Items/Armor/Gloves/Cotton Gloves" },
        { 33, "Items/Armor/Gloves/Leather Gloves" },
        { 34, "Items/Armor/Gloves/Bronze Gauntlets" },
        { 35, "Items/Materials/Grapes" },
        { 36, "Items/Keys/South Forest Key" },
        { 37, "Items/Keys/North Forest Key" },
        { 38, "Items/Keys/Forest Chest Key" },
        { 39, "Items/Keys/Church Key" },
        { 40, "Items/Consumables/Potions/Cooldown Potion" },
        { 41, "Items/Armor/Pants/Silk Pants" },
        { 42, "Items/Armor/Pants/Fine Leather Pants" },
        { 43, "Items/Armor/Pants/Copper Greaves" },
        { 44, "Items/Armor/Boots/Silk Boots" },
        { 45, "Items/Armor/Boots/Fine Leather Boots" },
        { 46, "Items/Armor/Boots/Copper Boots" },
        { 47, "Items/Armor/Gloves/Silk Gloves" },
        { 48, "Items/Armor/Gloves/Fine Leather Gloves" },
        { 49, "Items/Armor/Gloves/Copper Gauntlets" },
        { 50, "Items/Weapons/Swords/Copper Sword" },
        { 51, "Items/Weapons/Maces/Copper Mace" },
        { 52, "Items/Weapons/Staves/Maple Staff" },
        { 53, "Items/Weapons/Wands/Maple Wand" },
        { 54, "Items/Weapons/Bows/Maple Bow" },
        { 55, "Items/Armor/Chests/Copper Cuirass" },
        { 56, "Items/Armor/Chests/Silk Robe" },
        { 57, "Items/Armor/Heads/Silk Hat" },
        { 58, "Items/Armor/Heads/Copper Helm" },
        { 59, "Items/Armor/Heads/Fine Leather Helm" },
        { 60, "Items/Armor/Chests/Fine Leather Jacket" },
        { 61, "Items/Shields/Copper Shield" },
        { 62, "Items/Materials/Maple Log" },
        { 63, "Items/Materials/Copper Ore" },
        { 64, "Items/Materials/Silk Fiber" },
        { 65, "Items/Materials/Fine Leather Scrap" },
        { 66, "Items/Materials/Silk Thread" },
        { 67, "Items/Materials/Flint" },
        { 68, "Items/Materials/Melanite" },
        { 69, "Items/Keys/Forest 2 Key" }, /*
        { xx, "Items/xxxx" },
        { xx, "Items/xxxx" }*/
    };

    //Basic info
    public int ID;              //Unique ID number
    public int MaxStack;        //Maximum stack size
    public int Count;           //Number of items in stack
    public string Name;         //Name
    public Type ItemType;       //Type
    public Gear GearType;       //Type of gear, only used if Type == Type.Gear
    public bool NotDisposable;  //If true, item cannot be removed by normal means. Useful for quest items.
    public bool Tradable;       //If true, item can be traded, even if marked as not disposable. Useful for recipes.
    public Sprite Sprite;       //Sprite
    public Rarity ItemRarity;   //Item's rarity
    public int Quality;         //Item's quality
    public int Tier;            //Item tier
    public string Creator;      //Name of the creator
    public int Slot = -100;     //Slot number within UI. Currently 0-24 are the player inventory, -1 and -2 are for upgrading, -3 is enchanting

    //Gear stats
    public int BasePhysicalAttack;      //These should be self-explanatory
    public int BasePhysicalDefense;     //Set these in the prefab, but don't access them in the code
    public int BaseMagicalAttack;       //Use the below read-only attributes for that
    public int BaseMagicalDefense;
    public int BaseSpeed;

    //Read-only attributes that calculate and return the actual stat value
    public int PhysicalAttack   { get { return CalculateStat(BasePhysicalAttack);   } }
    public int PhysicalDefense  { get { return CalculateStat(BasePhysicalDefense);  } }
    public int MagicalAttack    { get { return CalculateStat(BaseMagicalAttack);    } }
    public int MagicalDefense   { get { return CalculateStat(BaseMagicalDefense);   } }
    public int Speed            { get { return CalculateStat(BaseSpeed);            } }

    //Enchantments
    public List<Enchantment> Enchantments = new List<Enchantment>();

    //Set various stats when loaded by database
    public void UpdateValuesFromDatabase(int count, Rarity itemRarity, int quality, List<Enchantment> enchantments, string creator)
    {
        Count = count;
        ItemRarity = itemRarity;
        Quality = quality;
        Enchantments = enchantments;
        Creator = creator;
    }

    //Rolls a rarity. This applies to all non-crafted items
    public void RollRarity()
    {
        float random = Random.Range(0, 1);
        int rarity = 0;
        for (int i = 0; i < RarityChance.Length; i++)
        {
            if(random < RarityChance[i])
            {
                rarity = i;
            }
        }
        ItemRarity = (Rarity)rarity;
    }

    //Same as above, but for crafting. Crafted items have a different rarity table
    public void RollRarityCrafted()
    {
        float random = Random.Range(0, 1);
        int rarity = 0;
        for (int i = 0; i < RarityChanceCrafting.Length; i++)
        {
            if (random < RarityChanceCrafting[i])
            {
                rarity = i;
            }
        }
        ItemRarity = (Rarity)rarity;
    }

    //Upgrades item quality
    public bool UpgradeQuality()
    {
        //Only works on gear
        if (ItemType != Type.Gear) return false;
        //Do not increase past upgrade cap
        if (Quality >= UpgradeCapByTier[Tier]) return false;

        //Increase quality
        Quality++;
        //If Quality is now odd, add enchantment (1, 3, 5, etc)
        if(Quality % 2 == 1)
        {
            Enchantments.Add(null);
            RerollEnchantment(Enchantments.Count - 1);
        }
        //Success
        return true;
    }

    //Set first enchantment(s), called after an item is first created (spawned or crafted)
    public void RollEnchantments()
    {
        //Tier 0 items do not receive enchantments
        if (Tier == 0) return;

        //Number of slots goes up one per item tier, and goes up one every other quality level (at 1, 3, 5, etc)
        int numberOfEnchantments = Tier + (Quality / 2);
        for(int i = 0; i < numberOfEnchantments; i++)
        {
            Enchantments.Add(null);
            RerollEnchantment(i);
        }
    }

    //Reroll single enchantment
    public bool RerollEnchantment(int slot)
    {
        //Only gear can have enchantments
        if (ItemType != Type.Gear) return false;
        //Get correct enchantment list
        List<int> enchantmentList = null;
        //Offensive item
        if (GearType == Gear.Weapon || GearType == Gear.Shield)
        {
            enchantmentList = Enchantment.RandomOffensiveEnchantments;
        }
        //Defensive item
        else if (GearType == Gear.Boots || GearType == Gear.Chest || GearType == Gear.Gloves || GearType == Gear.Head || GearType == Gear.Pants)
        {
            enchantmentList = Enchantment.RandomDefensiveEnchantments;
        }
        //Jewelry, etc
        else
        {
            enchantmentList = Enchantment.RandomEnchantments;
        }
        //Get current enchantment
        int currentIndex = (Enchantments[slot] == null ? -1 : enchantmentList.IndexOf(Enchantments[slot].ID));
        int rollCount = 0;
        //Get next unused enchantment index
        do
        {
            currentIndex = Random.Range(0, enchantmentList.Count);
            rollCount++;
            //currentIndex++;
            //currentIndex %= Enchantment.RandomOffensiveEnchantments.Count;
        }
        while (EnchantmentInList(enchantmentList[currentIndex]) && rollCount < 100) ;
        //If all enchantments are in use (should never happen but just in case) return false
        if (rollCount == 100) return false;
        //Replace the slot's enchantment with this new one
        if (Enchantments[slot] != null)
        {
            Destroy(Enchantments[slot].gameObject);
        }
        Enchantments[slot] = Instantiate(Resources.Load(Enchantment.Path[enchantmentList[currentIndex]]) as GameObject).GetComponent<Enchantment>();
        Enchantments[slot].transform.SetParent(transform);
        Enchantments[slot].gameObject.SetActive(false);
        //Success
        return true;
    }

    //Gets basic tooltip info
    public string GetTooltip()
    {
        return GetTooltip(false);
    }

    //Obscure last enchantment
    public string GetTooltip(bool hideLastEnchantment)
    { 
        StringBuilder sb = new StringBuilder();
        bool anyAdded = false;
        
        //Gear stats
        if (ItemType == Type.Gear)
        {
            if (BasePhysicalAttack > 0)
            {
                sb.AppendFormat("{0,-18} {1,4}", "Physical Attack", PhysicalAttack);
                anyAdded = true;
            }
            if (BasePhysicalDefense > 0)
            {
                if (anyAdded) sb.Append("\n");
                sb.AppendFormat("{0,-18} {1,4}", "Physical Defense", PhysicalDefense);
                anyAdded = true;
            }
            if (BaseMagicalAttack > 0)
            {
                if (anyAdded) sb.Append("\n");
                sb.AppendFormat("{0,-18} {1,4}", "Magical Attack", MagicalAttack);
                anyAdded = true;
            }
            if (BaseMagicalDefense > 0)
            {
                if (anyAdded) sb.Append("\n");
                sb.AppendFormat("{0,-18} {1,4}", "Magical Defense", MagicalDefense);
                anyAdded = true;
            }
            if (BaseSpeed > 0)
            {
                if (anyAdded) sb.Append("\n");
                sb.AppendFormat("{0,-18} {1,4}", "Speed", Speed);
                anyAdded = true;
            }
            //Rarity is only for gear
            if (anyAdded) sb.Append("\n");
            sb.AppendFormat("{0,-10} {1,12}", "Rarity", ItemRarity.ToString().Replace('_', ' '));
            anyAdded = true;
        }
        //Item tier, if applicable
        if(Tier > 0)
        {
            if (anyAdded) sb.Append("\n");
            sb.AppendFormat("{0,-18} {1,4}", "Tier", Tier);
            anyAdded = true;
        }
        //If there are any enchantments, list them
        if (Enchantments.Count > 0)
        {
            if (anyAdded) sb.Append("\n");
            sb.Append("     Enchantments");
            //Hide last enchantment
            int limit = (hideLastEnchantment ? Enchantments.Count - 1 : Enchantments.Count);
            for (int i = 0; i < limit; i++)
            { 
                sb.Append("\n");
                sb.Append(Enchantments[i].GetDescription());
            }
            if(hideLastEnchantment)
            {
                sb.Append("\n");
                sb.Append("+ New Enchantment");
            }
            anyAdded = true;
        }
        //Creator, if applicable
        if(!string.IsNullOrEmpty(Creator))
        {
            if (anyAdded) sb.Append("\n");
            sb.AppendFormat("{0,-10} {1,12}", "Creator", Creator);
        }

        return sb.ToString();
    }

    //Checks if enchantment is already in list (no duplicates are allowed)
    private bool EnchantmentInList(int id)
    {
        foreach(Enchantment e in Enchantments)
        {
            if (e == null) continue;
            if (e.ID == id) return true;
        }
        return false;
    }
    
    //Calculates actual stat value
    private int CalculateStat(int baseStat)
    {
        //Zero stats are never modified by rarity or quality
        if (baseStat == 0) return 0;
        //Otherwise, they are increased by one for each level of rarity and one for each level of quality
        return baseStat + (int)ItemRarity + Quality;
    }
}
