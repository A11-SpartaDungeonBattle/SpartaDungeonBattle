using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpartaDungeonBattle
{
    [Serializable]
    public class Character
    {
        public static Character CurrentCharacter { get; private set; }
        public int Level { get; private set; }
        public string Name { get; set; }
        public ClassType Class { get; private set; }
        public List<Equipment> ItemOnEquipped { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        private int _hp;
        public int HP
        {
            get { return _hp; }
            set
            {
                int previousHP = _hp;
                _hp = value;
                if (_hp < 0) _hp = 0;
                else if (_hp > MaxHP) _hp = MaxHP;
                HPChangedCallback?.Invoke(previousHP, _hp);
            }
        }
        public Action<int, int> HPChangedCallback { get; set; }
        public bool isDead { get { return HP <= 0; } }
        public int MaxHP { get; private set; }
        private int _mp;
        public int MP
        {
            get { return _mp; }
            set
            {
                int previousMP = _mp;
                _mp = value;
                if (_mp < 0) _mp = 0;
                else if (_hp <= MaxMP) _mp = MaxMP;
                MPChangedCallback?.Invoke(previousMP, _mp);
            }
        }
        public int MaxMP { get; private set; }
        public Action<int, int> MPChangedCallback { get; set; }
        public int Gold { get; private set; }
        public int Exp { get; private set; }
        public Inventory Inventory { get; private set; }
        //public List<Skill> Skills { get; private set; }
        public Skill MainSkill { get; private set; }
        /// <summary>
        /// 필요에 따라 편집
        /// </summary>
        public Character()
        {
            CurrentCharacter = this;
            Level = 1;
            Name = "";
            Class = ClassType.WARRIOR;
            Gold = 0;
            Exp = 0;
            Inventory = new Inventory();
            ItemOnEquipped = new List<Equipment>();
            HPChangedCallback = (previousHP, postHP) =>
            {
                Console.WriteLine($"Lv.{Level} {Name}");
                Console.WriteLine($"HP {previousHP} -> {(postHP > 0 ? postHP : "Dead\n")}");
            };
            MPChangedCallback = (previousMP, postMP) =>
            {
                Console.WriteLine($"Lv.{Level} {Name}");
                Console.WriteLine($"HP {previousMP} -> {(postMP > 0 ? postMP : "Dead\n")}");
            };
        }

        public void UseSkill(Monster monster)
        {
//            MainSkill.SkillToMonster?.Invoke(monster, this);
        }
        public void UseSkill(List<Monster> monsters)
        {
//            MainSkill.SkillToMonsters?.Invoke(monsters, this);
        }

        public void UseItem(Consumption consumption) { }

        public void EquipItem(Equipment equipment) 
        {
            //장착 중인 아이템이라면 해제, 그만큼 능력치 감소
            if (ItemOnEquipped.Contains(equipment))
            {
                ItemOnEquipped.Remove(equipment);

                this.Attack -= equipment.Attack;
                this.Defense -= equipment.Defense;
            }
            //미장착 아이템이라면 장착, 그만큼 능력치 증가
            else
            {
                ItemOnEquipped.Add(equipment);

                this.Attack += equipment.Attack;
                this.Defense += equipment.Defense;
            }
        }

        /// <summary>
        /// 경험치 증가 및 레벨업 메소드.  
        /// 레벨업 시 공격력 1, 방어력 1 상승
        /// </summary>
        /// <param name="exp">죽인 적 레벨 1 당 1 exp</param>
        public void IncreaseExp(int exp) 
        {
            int[] max_exp = new int[4] {10, 35, 60, 100};

            this.Exp += exp;

            //5레벨부터는 레벨업 없이 경험치만 쌓임.
            if(this.Level < 5)
            {
                //현재 레벨의 경험치 상한에 도달했다면 레벨업
                if (this.Exp >= max_exp[this.Level - 1]) 
                {
                    this.Exp -= max_exp[this.Level - 1];
                    this.Level++;

                    this.Attack += 1;
                    this.Defense += 1;
                    this.MaxHP += 10;
                    this.MaxMP += 10;
                    this.HP = MaxHP;
                    this.MaxMP = MaxMP;
                }
            }
        }

        /// <summary>
        /// 입력받은 클래스에 따른 시작 ATK, DEF, HP, MP 설정
        /// </summary>
        public void SetClass(ClassType classType)
        {
            Class = classType;
            switch (classType)
            {
                case ClassType.WARRIOR:
                    {
                        Attack = 10;
                        Defense = 10;
                        MaxHP = 100;
                        HP = MaxHP;
                        MaxMP = 50;
                        MP = MaxMP;
                        MainSkill = new Skill(SkillName.POWERSTRIKE);
                        break;
                    }
                case ClassType.ARCHER:
                    {
                        Attack = 10;
                        Defense = 6;
                        MaxHP = 80;
                        HP = MaxHP;
                        MaxMP = 60;
                        MP = MaxMP;
                        MainSkill = new Skill(SkillName.MULTISHOT);
                        break;
                    }
                case ClassType.MAGICIAN:
                    {
                        Attack = 6;
                        Defense = 4;
                        MaxHP = 60;
                        HP = MaxHP;
                        MaxMP = 90;
                        MP = MaxMP;
                        MainSkill = new Skill(SkillName.FIREBALL);
                        break;
                    }
                //case ClassType.CLERIC:
                //    {
                //        Attack = 8;
                //        Defense = 8;
                //        MaxHP = 80;
                //        HP = MaxHP;
                //        MaxMP = 70;
                //        MP = MaxMP;
                //        break;
                //    }
            }
        }
        public bool NormalAttack(Monster monster)
        {
            Random random = new Random();

            //추후에 캐릭터가 크리티컬 확률, 크뎀 배율, 최소최대 데미지범위 와 같은 status를 활용하게 될경우 아래 3 변수를 캐릭터로 이동시키면 구현가능
            float critRate = 0.15f;
            float critMultiplier = 1.6f;
            float damageRangeMultiplier = 0.1f;   // 일반 데미지 계산할때 몇퍼센트 오차가 발생하는지 보여주는 수치 0.1f 는 -10% ~ +10% 오차가 있음을 의미

            // 공격력 계산방법 => damage = atk 에서 atk 가중치를 반영해서 구함. 소수점의 경우에는 음수일때 Floor, 양수일때 Ceiling 으로 처리           
            int damage = random.Next(   (int)Math.Floor     (Attack * (1 - damageRangeMultiplier)),
                                        (int)Math.Ceiling   (Attack * (1 + damageRangeMultiplier))
                                     );
            // 크리티컬 데미지 여부 판단 후 크리티컬 배율 반영
            bool isCritAttack=false;
            if (random.Next(1, 101) <= critRate * 100)
            {
                damage = (int)(damage * critMultiplier);
                isCritAttack = true;
            }
            else
            {
                isCritAttack = false;
            }

            Console.WriteLine($"{Name} 의 공격!");
            return monster.OnHit(damage, AttackType.NORMAL, isCritAttack);
        }
        public bool OnHit(int damage, AttackType type, bool isCritAttack)
        {
            Random random = new Random();
            float dodgeRate = 0.1f;
            switch (type)
            {
                case AttackType.NORMAL :
                    if (random.Next(1, 101) <= dodgeRate * 100)
                    {
                        // 회피 성공
                        Console.WriteLine("하지만 공격은 빗나갔습니다!\n");
                        return false;
                    }
                    else
                    {
                        // 회피 실패
                        Console.WriteLine($"{Name} 을(를) 맞췄습니다.   [데미지 : {damage}]{(isCritAttack ? " - 치명타 공격!!" : "")}\n");
                        HP -= damage;
                        if (HP < 0) HP = 0;
                        return true;
                    }
                case AttackType.SKILL :
                    // 항상 회피 실패
                    HP -= damage;
                    if(HP < 0) HP=0;
                    return true;
                default:
                    // 예외처리 구현하려면 이곳수정
                    return true;
            }
        }



        public string ClassToString()
        {
            switch (Class) {
                case ClassType.WARRIOR:
                    return "전사";
                case ClassType.ARCHER:
                    return "궁수";
                case ClassType.MAGICIAN:
                    return "마법사";
                //case ClassType.CLERIC:
                //    return "클레릭";
                default:
                    return "";
            }
        }

        public bool SetNickname(string nickname)
        {
            if (nickname == "") return false;
            Name = nickname;
            return true;
        }

        /// <summary>
        /// 골드 증감 메소드
        /// </summary>
        /// <param name="gold">획득/잃은 골드</param>
        public void EarnGold(int gold)
        {
            this.Gold += gold;
        }
    }

    public enum ClassType
    {
        WARRIOR = 1,
        ARCHER,
        MAGICIAN,
        //CLERIC
    }

    public enum AttackType
    {
        NORMAL = 0,
        SKILL,
    }
}

