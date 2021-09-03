﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items
{
    public class AccessoryTests
    {
        [Fact]
        public void DressedIn_Null_DoNotThrow()
        {
            var sut = ItemTestData.CreateRing(1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.SkillAxe,5);
            sut.DressedIn(null);
        }
        [Fact]
        public void DressedIn_Player_AddSkillBonus()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer(skills: PlayerTestDataBuilder.GenerateSkills(10));
            var sut = ItemTestData.CreateRing(1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.SkillAxe, 5);
            //act
            sut.DressedIn(player);

            //assert
            player.GetSkillBonus(SkillType.Axe).Should().Be(5);
        }

        [Fact]
        public void UndressFrom_Null_DoNotThrow()
        {
            //arrange
            var sut = ItemTestData.CreateRing(1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.SkillAxe, 5);
            //act
            sut.UndressFrom(null);
        }
        [Fact]
        public void UndressFrom_Player_RemoveSkillBonus()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer(skills: PlayerTestDataBuilder.GenerateSkills(10));
            var sut = ItemTestData.CreateRing(1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.SkillAxe, 5);

            //act
            sut.DressedIn(player);
            //assert
            player.GetSkillBonus(SkillType.Axe).Should().Be(5);

            //act
            sut.UndressFrom(player);
            //assert
            player.GetSkillBonus(SkillType.Axe).Should().Be(0);
        }


        [Fact]
        public void Decrease_DefendedAttack_DecreaseCharges()
        {
            //arrange
            var defender = PlayerTestDataBuilder.BuildPlayer();
            var attacker = PlayerTestDataBuilder.BuildPlayer();

            var hmm = ItemTestData.CreateAttackRune(1, damageType: DamageType.Energy);

            var sut = ItemTestData.CreateRing(1, charges:50);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.AbsorbPercentEnergy, 10);
            
            sut.DressedIn(defender);

            //act
            attacker.Attack(defender, hmm);
            
            //assert
            sut.Charges.Should().Be(49);
        }

        [Fact]
        public void NoCharges_10Charges_ReturnsFalse()
        {
            //arrange
            var sut = ItemTestData.CreateRing(1, charges: 10);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.AbsorbPercentEnergy, 10);
            
            //assert
            sut.NoCharges.Should().BeFalse();
        }
        [Fact]
        public void NoCharges_0Charges_ReturnsTrue()
        {
            //arrange
            var sut = ItemTestData.CreateRing(1, charges: 0);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.AbsorbPercentEnergy, 10);

            //assert
            sut.NoCharges.Should().BeTrue();
        }

        [Fact]
        public void Protect_NoCharges_DoNotProtect()
        {
            //arrange
            var defender = PlayerTestDataBuilder.BuildPlayer();
            var attacker = PlayerTestDataBuilder.BuildPlayer();
            var oldHp = defender.HealthPoints;

            var totalDamage = 0;
            defender.OnInjured += (enemy, victim, damage) =>
            {
                totalDamage = damage.Damage;
            };

            var hmm = ItemTestData.CreateAttackRune(1, damageType: DamageType.Energy, min:100,max:100);

            var sut = ItemTestData.CreateRing(1, charges: 0);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.AbsorbPercentEnergy, 100);
            sut.DressedIn(defender);

            //act
            attacker.Attack(defender, hmm);
            
            //assert
            totalDamage.Should().NotBe(0);
            defender.HealthPoints.Should().BeLessThan(oldHp);
        }
        [Fact]
        public void Protect_1Charge_ProtectFromDamage()
        {
            //arrange
            var defender = PlayerTestDataBuilder.BuildPlayer();
            var attacker = PlayerTestDataBuilder.BuildPlayer();

            var oldHp = defender.HealthPoints;

           var totalDamage = 0;
            defender.OnInjured += (enemy, victim, damage) =>
            {
                totalDamage = damage.Damage;
            };

            var hmm = ItemTestData.CreateAttackRune(1, damageType: DamageType.Energy, min: 100, max: 100);

            var sut = ItemTestData.CreateRing(1, charges: 1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.AbsorbPercentEnergy, 100);
            sut.DressedIn(defender);

            //act
            attacker.Attack(defender, hmm);

            //assert
            totalDamage.Should().Be(0);
            defender.HealthPoints.Should().Be(oldHp);
        }
    }
}