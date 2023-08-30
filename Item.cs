﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpartaDungeonBattle
{
    public enum ItemType { Weapon = 0, Armor, Consumption }
    public enum ItemTarget { ToCharacter = 0, ToMonster }
​
	public abstract class Item
    {
        public ItemType ItemType { get; }
        public string ItemName { get; }
        public int Gold { get; }
        public string Description { get; }

        public Item(ItemType itemType, string itemName, int gold, string description)
        {
            ItemType = itemType;
            ItemName = itemName;
            Gold = gold;
            Description = description;
        }
    }

    public class Equipment : Item
    {​
		// Equipment 고유 필드
		public int Attack { get; }
        public int Defense { get; }
​
		public Equipment(ItemType itemType, string itemName, int gold, string description, int atk, int def) : base(itemType, itemName, gold, description)
        {
            Attack = atk;
            Defense = def;
        }
    }
    public class Consumption : Item
    {​
        public ItemTarget Target { get; }

        // Consumption 고유 메서드
        public void ItemFunc(Character? character, Monster? monster)
        {
            if (character != null) ItemToCharacter.Invoke(character);
            if (monster != null) ItemToMonster.Invoke(monster);
        }

        Action<Character> ItemToCharacter;​
		Action<Monster> ItemToMonster;
​
		public Consumption(ItemType itemType, string itemName, int gold, string description, ItemTarget target) : base(itemType, itemName, gold, description)
        {
            Target = target;
        }
    }
}
