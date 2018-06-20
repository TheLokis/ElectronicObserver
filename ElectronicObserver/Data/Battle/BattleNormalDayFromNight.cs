using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

	/// <summary>
	/// 通常/連合艦隊 vs 通常艦隊　夜昼戦
	/// </summary>
	public class BattleNormalDayFromNight : BattleDayFromNight
	{

		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);

			NightInitial = new PhaseNightInitial(this, "야전개시", false);
			FriendlySupport = new PhaseFriendlySupport(this, "우군함대원호");
			NightSupport = new PhaseSupport(this, "야전지원공격", true);
			NightBattle = new PhaseNightBattle(this, "제1차야전", 1);
			NightBattle2 = new PhaseNightBattle(this, "제2차야전", 2);


			if (NextToDay)
			{
                /*
				JetBaseAirAttack = new PhaseJetBaseAirAttack(this, "噴式基地航空隊攻撃");
				JetAirBattle = new PhaseJetAirBattle(this, "噴式航空戦");
				BaseAirAttack = new PhaseBaseAirAttack(this, "基地航空隊攻撃");
				Support = new PhaseSupport(this, "支援攻撃");
				AirBattle = new PhaseAirBattle(this, "航空戦");
				OpeningASW = new PhaseOpeningASW(this, "先制対潜");
				OpeningTorpedo = new PhaseTorpedo(this, "先制雷撃", 0);
				Shelling1 = new PhaseShelling(this, "第一次砲撃戦", 1, "1");
				Shelling2 = new PhaseShelling(this, "第二次砲撃戦", 2, "2");
				Torpedo = new PhaseTorpedo(this, "雷撃戦", 3);*/

                JetBaseAirAttack = new PhaseJetBaseAirAttack(this, "기지항공대 분식 강습");
                JetAirBattle = new PhaseJetAirBattle(this, "분식 항공전");
                BaseAirAttack = new PhaseBaseAirAttack(this, "기지 항공대 공격");
                AirBattle = new PhaseAirBattle(this, "1차 항공전");
                Support = new PhaseSupport(this, "지원 공격");
                OpeningASW = new PhaseOpeningASW(this, "선제대잠");
                OpeningTorpedo = new PhaseTorpedo(this, "선제뇌격", 0);
                Shelling1 = new PhaseShelling(this, "제1차포격전", 1, "1");
                Shelling2 = new PhaseShelling(this, "제2차포격전", 2, "2");
                Torpedo = new PhaseTorpedo(this, "뇌격전", 3);

            }

			foreach (var phase in GetPhases())
				phase.EmulateBattle(_resultHPs, _attackDamages);
		}

		public override string APIName => "api_req_sortie/night_to_day";

		public override string BattleName => "대일반함대 야주전";



		public override IEnumerable<PhaseBase> GetPhases()
		{
			yield return Initial;
			yield return Searching;
			yield return NightInitial;
			yield return FriendlySupport;
			yield return NightSupport;
			yield return NightBattle;
			yield return NightBattle2;

			if (NextToDay)
			{
				yield return JetBaseAirAttack;
				yield return JetAirBattle;
				yield return BaseAirAttack;
				yield return Support;
				yield return AirBattle;
				yield return OpeningASW;
				yield return OpeningTorpedo;
				yield return Shelling1;
				yield return Shelling2;
				yield return Torpedo;
			}
		}
	}
}
