using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpartaDungeonBattle
{
    public class Monster
    {
        public int Level { get; private set; }
        public string Name { get; private set; }
        public int Attack { get; private set; }
        private int _hp;
        public int HP
        {
            get { return _hp; }
            set
            {
                int previousHP = _hp;
                _hp = value;
                if (_hp < 0) _hp = 0;
                HealthChangedCallback?.Invoke(previousHP, _hp);
            }
        }
        public Action<int, int> HealthChangedCallback { get; set; }

        public bool isDead { get { return HP <= 0; } }

        /// <summary>
        /// 필요에 따라 편집
        /// </summary>
        public Monster(int level, string name, int attack, int hp)
        {
            Level = level;
            Name = name;
            Attack = attack;
            HP = hp;
            HealthChangedCallback = (previousHP, postHP) => 
            {
                Console.WriteLine($"Lv.{Level} {Name}");
                Console.WriteLine($"HP {previousHP} -> {(postHP > 0 ? postHP : "Dead\n")}");
            };
        }

        public bool NormalAttack(Character character)
        {
            //추후에 몬스터가 크리티컬 확률, 크뎀 배율, 최소최대 데미지범위 와 같은 status를 활용하게 될경우 아래 3 변수를 몬스터 이동시키면 구현가능
            Random random = new Random();
            float critRate = 0.15f;
            float critMultiplier = 1.6f;
            float damageRangeMultiplier = 0.1f;   // 일반 데미지 계산할때 몇퍼센트 오차가 발생하는지 보여주는 수치 0.1f 는 -10% ~ +10% 오차가 있음을 의미

            // 공격력 계산방법 => damage = atk 에서 atk 가중치를 반영해서 구함. 소수점의 경우에는 음수일때 Floor, 양수일때 Ceiling 으로 처리           
            int damage = random.Next((int)Math.Floor(Attack * (1 - damageRangeMultiplier)),
                                        (int)Math.Ceiling(Attack * (1 + damageRangeMultiplier))
                                     );
            // 크리티컬 데미지 여부 판단 후 크리티컬 배율 반영
            bool isCritAttack= false;
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
            return character.OnHit(damage, AttackType.NORMAL, isCritAttack);
        }
        public bool OnHit(int damage, AttackType type, bool isCritAttack)
        {
            Random random = new Random();
            float dodgeRate = 0.1f;
            switch (type)
            {
                case AttackType.NORMAL:
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
                case AttackType.SKILL:
                    // 항상 회피 실패
                    HP -= damage;
                    if (HP < 0) HP = 0;
                    return true;
                default:
                    // 예외처리 구현하려면 이곳수정
                    return true;
            }
        }
    }
}
