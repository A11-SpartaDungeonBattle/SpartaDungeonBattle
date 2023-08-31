using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpartaDungeonBattle;

namespace SpartaDungeonBattle
{
    [Serializable]
    public class Skill
    {
        public string SkillName { get; private set; }
        public int Cost { get; private set; }
        public string SkillDescription { get; private set; }

        public bool IsNeedTarget { get; private set; }
        Action<List<Monster>, Character>? SkillToMonsters;
        Action<Monster, Character>? SkillToMonster;

        /// <summary>
        /// 필요에 따라 편집
        /// </summary>
        public Skill(SkillName skillName) 
        { 
            switch(skillName) 
            {
                // 파이어볼
                case SpartaDungeonBattle.SkillName.FIREBALL:
                    SkillName = "파이어볼";
                    Cost = 15;
                    SkillDescription = "단일의 적에게 캐릭터의 MaxMP의 절반의 데미지를 가한다";
                    IsNeedTarget = true;
                    SkillToMonster += (Monster monster,Character character) => 
                        {monster.HP -= character.MaxMP; if (monster.HP <= 0) monster.HP = 0; };
                    break;

                case SpartaDungeonBattle.SkillName.MULTISHOT:
                    SkillName = "다중사격";
                    Cost = 15;
                    SkillDescription = "모든 적에게 캐릭터의 공격력의 데미지를 가한다";
                    IsNeedTarget = false;
                    SkillToMonsters += (List<Monster> monsters, Character character) =>
                        {   
                            foreach(var monster in monsters)
                            { 
                               monster.HP -= character.Attack; 
                                if (monster.HP <= 0) monster.HP = 0;
                            }
                        };
                    break;

                case SpartaDungeonBattle.SkillName.POWERSTRIKE:
                    SkillName = "강한타격";
                    Cost = 10;
                    SkillDescription = "단일의 적에게 캐릭터의 공격력의 2배의 데미지를 가한다";
                    IsNeedTarget = true;
                    SkillToMonster += (Monster monster, Character character) =>
                        {   monster.HP -= character.Attack * 2; 
                            if (monster.HP <= 0) monster.HP = 0; 
                        };
                    break;

                default:
                    // 예외 처리
                    break;
            }
        }


    }

    public enum SkillName
    {
        FIREBALL = 0,
        MULTISHOT,
        POWERSTRIKE
    }
}
