﻿using ElectronicObserver.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{

	public static class Constants
	{

		#region 艦船・装備

		/// <summary>
		/// 艦船の速力を表す文字列を取得します。
		/// </summary>
		public static string GetSpeed(int value)
		{
			switch (value)
			{
				case 0:
					return "육상";
				case 5:
					return "저속";
				case 10:
					return "고속";
				case 15:
					return "고속+";
				case 20:
					return "최속";
				default:
					return "불명";
			}
		}

		/// <summary>
		/// 射程を表す文字列を取得します。
		/// </summary>
		public static string GetRange(int value)
		{
			switch (value)
			{
				case 0:
					return "없음";
				case 1:
					return "단";
				case 2:
					return "중";
				case 3:
					return "장";
				case 4:
					return "최장";
				case 5:
					return "최장+";
				default:
					return "不明";
			}
		}

		/// <summary>
		/// 艦船のレアリティを表す文字列を取得します。
		/// </summary>
		public static string GetShipRarity(int value)
		{
			switch (value)
			{
				case 0:
					return "赤";
				case 1:
					return "藍";
				case 2:
					return "青";
				case 3:
					return "水";
				case 4:
					return "銀";
				case 5:
					return "金";
				case 6:
					return "虹";
				case 7:
					return "輝虹";
				case 8:
					return "桜虹";
				default:
					return "不明";
			}
		}

		/// <summary>
		/// 装備のレアリティを表す文字列を取得します。
		/// </summary>
		public static string GetEquipmentRarity(int value)
		{
			switch (value)
			{
				case 0:
					return "커먼";
				case 1:
					return "레어";
				case 2:
					return "홀로";
				case 3:
					return "S홀로";
				case 4:
					return "SS홀로";
				case 5:
					return "SS홀로'";
				case 6:
					return "SS홀로+";
				default:
					return "不明";
			}
		}

		/// <summary>
		/// 装備のレアリティの画像インデックスを取得します。
		/// </summary>
		public static int GetEquipmentRarityID(int value)
		{
			switch (value)
			{
				case 0:
					return 1;
				case 1:
					return 3;
				case 2:
					return 4;
				case 3:
					return 5;
				case 4:
					return 6;
				case 5:
					return 7;
				case 6:
					return 8;
				default:
					return 0;
			}
		}


		/// <summary>
		/// 艦船のボイス設定フラグを表す文字列を取得します。
		/// </summary>
		public static string GetVoiceFlag(int value)
		{

			switch (value)
			{
				case 0:
					return "-";
				case 1:
					return "방치";
				case 2:
					return "시보";
				case 3:
					return "방치+시보";
				case 4:
					return "특수방치";
				case 5:
					return "방치+특수방치";
				case 6:
					return "시보+특수방치";
				case 7:
					return "방치+시보+특수방치";
				default:
					return "불명";
			}
		}


		/// <summary>
		/// 艦船の損傷度合いを表す文字列を取得します。
		/// </summary>
		/// <param name="hprate">現在HP/最大HPで表される割合。</param>
		/// <param name="isPractice">演習かどうか。</param>
		/// <param name="isLandBase">陸上基地かどうか。</param>
		/// <param name="isEscaped">退避中かどうか。</param>
		/// <returns></returns>
		public static string GetDamageState(double hprate, bool isPractice = false, bool isLandBase = false, bool isEscaped = false)
		{

            if (isEscaped)
                return "대피";
            else if (hprate <= 0.0)
                return isPractice ? "이탈" : (!isLandBase ? "굉침" : "파괴");
            else if (hprate <= 0.25)
                return !isLandBase ? "대파" : "손괴";
            else if (hprate <= 0.5)
                return !isLandBase ? "중파" : "손해";
            else if (hprate <= 0.75)
                return !isLandBase ? "소파" : "혼란";
            else if (hprate < 1.0)
                return "정상";
            else
                return "없음";

        }


		/// <summary>
		/// 基地航空隊の行動指示を表す文字列を取得します。
		/// </summary>
		public static string GetBaseAirCorpsActionKind(int value)
		{
			switch (value)
			{
				case 0:
					return "대기";
				case 1:
					return "출격";
				case 2:
					return "방공";
				case 3:
					return "대피";
				case 4:
					return "휴식";
				default:
					return "불명";
			}
		}


		/// <summary>
		/// 艦種略号を取得します。
		/// </summary>
		public static string GetShipClassClassification(ShipTypes shiptype)
		{
			switch (shiptype)
			{
				case ShipTypes.Escort:
					return "DE";
				case ShipTypes.Destroyer:
					return "DD";
				case ShipTypes.LightCruiser:
					return "CL";
				case ShipTypes.TorpedoCruiser:
					return "CLT";
				case ShipTypes.HeavyCruiser:
					return "CA";
				case ShipTypes.AviationCruiser:
					return "CAV";
				case ShipTypes.LightAircraftCarrier:
					return "CVL";
				case ShipTypes.Battlecruiser:
					return "BC";    // ? FBB, CC?
				case ShipTypes.Battleship:
					return "BB";
				case ShipTypes.AviationBattleship:
					return "BBV";
				case ShipTypes.AircraftCarrier:
					return "CV";
				case ShipTypes.SuperDreadnoughts:
					return "BB";
				case ShipTypes.Submarine:
					return "SS";
				case ShipTypes.SubmarineAircraftCarrier:
					return "SSV";
				case ShipTypes.Transport:
					return "AP";    // ? AO?
				case ShipTypes.SeaplaneTender:
					return "AV";
				case ShipTypes.AmphibiousAssaultShip:
					return "LHA";
				case ShipTypes.ArmoredAircraftCarrier:
					return "CVB";
				case ShipTypes.RepairShip:
					return "AR";
				case ShipTypes.SubmarineTender:
					return "AS";
				case ShipTypes.TrainingCruiser:
					return "CT";
				case ShipTypes.FleetOiler:
					return "AO";
				default:
					return "IX";
			}
		}


		/// <summary>
		/// 艦型を表す文字列を取得します。
		/// </summary>
		public static string GetShipClass(int id)
		{
			switch (id)
			{
				case 1: return "아야나미급";
				case 2: return "이세급";
				case 3: return "카가급";
				case 4: return "쿠마급";
				case 5: return "아카츠키급";
				case 6: return "콩고급";
				case 7: return "후루타카급";
				case 8: return "타카오급";
				case 9: return "모가미급";
				case 10: return "하츠하루급";
				case 11: return "쇼호급";
				case 12: return "후부키급";
				case 13: return "아오바급";
				case 14: return "아카기급";
				case 15: return "치토세급";
				case 16: return "센다이급";
				case 17: return "소류급";
				case 18: return "아사시오급";
				case 19: return "나가토급";
				case 20: return "나가라급";
				case 21: return "텐류급";
				case 22: return "시마카제급";
				case 23: return "시라츠유급";
				case 24: return "히요급";
				case 25: return "히류급";
				case 26: return "후소급";
				case 27: return "호쇼급";
				case 28: return "무츠키급";
				case 29: return "묘코급";
				case 30: return "카게로급";
				case 31: return "토네급";
				case 32: return "류조급";
				case 33: return "쇼카쿠급";
				case 34: return "유바리급";
				case 35: return "해대6형";
				case 36: return "순잠을형개2";
				case 37: return "야마토급";
				case 38: return "유구모급";
				case 39: return "순잠을형";
				case 40: return "순잠3형";
				case 41: return "아가노급";
				case 42: return "「霧」";
				case 43: return "다이호급";
				case 44: return "센토쿠급(이400급잠수함)";
				case 45: return "특종선병형";
				case 46: return "삼식잠항수송정";
				case 47: return "Bismarck급";
				case 48: return "Z1급";
				case 49: return "공작함";
				case 50: return "타이게이급";
				case 51: return "류호급";
				case 52: return "오요도급";
				case 53: return "운류급";
				case 54: return "아키즈키급";
				case 55: return "Admiral Hipper급";
				case 56: return "카토리급";
				case 57: return "유보트 IX C형";
				case 58: return "V.Veneto급";
				case 59: return "아키츠시마급";
				case 60: return "改카자하야급";
				case 61: return "Maestrale급";
				case 62: return "미즈호급";
				case 63: return "Graf Zeppelin급";
				case 64: return "Zara급";
				case 65: return "Iowa급";
				case 66: return "카미카제급";
				case 67: return "Queen Elizabeth급";
				case 68: return "Aquila급";
				case 69: return "Lexington급";
				case 70: return "C.Teste급";
				case 71: return "순잠갑형개2";
				case 72: return "카모이급";
				case 73: return "Гангут급";
				case 74: return "시무슈급";
				case 75: return "카스가마루급";
				case 76: return "타이요급";
				case 77: return "에토로후급";
				case 78: return "Ark Royal급";
				case 79: return "Richelieu급";
				case 80: return "Guglielmo Marconi급";
				case 81: return "Ташкент급";
				case 82: return "J급";
				case 83: return "Casablanca급";
				case 84: return "Essex급";
				case 85: return "히부리급";
				case 86: return "로호잠수함";
				case 87: return "John C.Butler급";
				default: return "불명";
			}
		}

		#endregion


		#region 出撃

		/// <summary>
		/// マップ上のセルでのイベントを表す文字列を取得します。
		/// </summary>
		public static string GetMapEventID(int value)
		{

			switch (value)
			{

				case 0:
					return "초기위치";
				case 1:
					return "이벤트없음";
				case 2:
					return "자원";
				case 3:
					return "소용돌이";
				case 4:
					return "통상전";
				case 5:
					return "보스전";
				case 6:
					return "기분탓이었다";
				case 7:
					return "항공전";
				case 8:
					return "선단호위성공";
				case 9:
					return "상륙지점";
				default:
					return "불명";
			}
		}

		/// <summary>
		/// マップ上のセルでのイベント種別を表す文字列を取得します。
		/// </summary>
		public static string GetMapEventKind(int value)
		{

			switch (value)
			{
				case 0:
					return "비전투";
				case 1:
					return "주야전";
				case 2:
					return "야전";
				case 3:
					return "주야전";       // 対通常?
				case 4:
					return "항공전";
				case 5:
					return "적연합";
				case 6:
					return "공습전";
				case 7:
					return "주야전";       // 対連合
				default:
					return "불명";
			}
		}


		/// <summary>
		/// 海域難易度を表す文字列を取得します。
		/// </summary>
		public static string GetDifficulty(int value)
		{

			switch (value)
			{
				case -1:
					return "없음";
				case 0:
					return "미설정";
				case 1:
					return "정";
				case 2:
					return "병";
				case 3:
					return "을";
				case 4:
					return "갑";
				default:
					return "불명";
			}
		}

		/// <summary>
		/// 海域難易度を表す数値を取得します。
		/// </summary>
		public static int GetDifficulty(string value)
		{

			switch (value)
			{
				case "未選択":
					return 0;
				case "丁":
					return 1;
				case "丙":
					return 2;
				case "乙":
					return 3;
				case "甲":
					return 4;
				default:
					return -1;
			}

		}

		/// <summary>
		/// 空襲被害の状態を表す文字列を取得します。
		/// </summary>
		public static string GetAirRaidDamage(int value)
		{
			switch (value)
			{
				case 1:
					return "자원 피해";
				case 2:
					return "자원・항공대 피해";
				case 3:
					return "항공대 피해";
				case 4:
					return "피해없음";
				default:
					return "발생하지않음";
			}
		}

		/// <summary>
		/// 空襲被害の状態を表す文字列を取得します。(短縮版)
		/// </summary>
		public static string GetAirRaidDamageShort(int value)
		{
			switch (value)
			{
				case 1:
					return "자원 피해";
				case 2:
					return "자원・항공대 피해";
				case 3:
					return "항공대 피해";
				case 4:
					return "피해없음";
				default:
					return "-";
			}
		}


		#endregion


		#region 戦闘

		/// <summary>
		/// 陣形を表す文字列を取得します。
		/// </summary>
		public static string GetFormation(int id)
		{
			switch (id)
			{
				case 1:
					return "단종진";
				case 2:
					return "복종진";
				case 3:
					return "윤형진";
				case 4:
					return "제형진";
				case 5:
					return "단횡진";
				case 6:
					return "경계진";
				case 11:
					return "제1경계항행서열";
				case 12:
					return "제2경계항행서열";
				case 13:
					return "제3경계항행서열";
				case 14:
					return "제4경계항행서열";
				default:
					return "不明";
			}
		}

		/// <summary>
		/// 陣形を表す数値を取得します。
		/// </summary>
		public static int GetFormation(string value)
		{
			switch (value)
			{
				case "単縦陣":
					return 1;
				case "複縦陣":
					return 2;
				case "輪形陣":
					return 3;
				case "梯形陣":
					return 4;
				case "単横陣":
					return 5;
				case "警戒陣":
					return 6;
				case "第一警戒航行序列":
					return 11;
				case "第二警戒航行序列":
					return 12;
				case "第三警戒航行序列":
					return 13;
				case "第四警戒航行序列":
					return 14;
				default:
					return -1;
			}
		}

		/// <summary>
		/// 陣形を表す文字列(短縮版)を取得します。
		/// </summary>
		public static string GetFormationShort(int id)
		{
			switch (id)
			{
				case 1:
					return "단종진";
				case 2:
					return "복종진";
				case 3:
					return "윤형진";
				case 4:
					return "제형진";
				case 5:
					return "단횡진";
				case 6:
					return "경계진";
				case 11:
					return "제1경계";
				case 12:
					return "제2경계";
				case 13:
					return "제3경계";
				case 14:
					return "제4경계";
				default:
					return "불명";
			}
		}

		/// <summary>
		/// 交戦形態を表す文字列を取得します。
		/// </summary>
		public static string GetEngagementForm(int id)
		{
			switch (id)
			{
				case 1:
					return "동항전";
				case 2:
					return "반항전";
				case 3:
					return "T유리";
				case 4:
					return "T불리";
				default:
					return "불명";
			}
		}

		/// <summary>
		/// 索敵結果を表す文字列を取得します。
		/// </summary>
		public static string GetSearchingResult(int id)
		{
			switch (id)
			{
				case 1:
					return "성공";
				case 2:
					return "성공(미귀환기)";
				case 3:
					return "미귀환";
				case 4:
					return "실패";
				case 5:
					return "성공(색적기없음)";
				case 6:
					return "실패(색적기없음)";
				default:
					return "불명";
			}
		}

		/// <summary>
		/// 索敵結果を表す文字列(短縮版)を取得します。
		/// </summary>
		public static string GetSearchingResultShort(int id)
		{
			switch (id)
			{
				case 1:
					return "성공";
				case 2:
					return "성공△";
				case 3:
					return "미귀환";
				case 4:
					return "실패";
				case 5:
					return "성공";
				case 6:
					return "실패";
				default:
					return "불명";
			}
		}

		/// <summary>
		/// 制空戦の結果を表す文字列を取得します。
		/// </summary>
		public static string GetAirSuperiority(int id)
		{
			switch (id)
			{
				case 0:
					return "제공균등";
				case 1:
					return "제공권확보";
				case 2:
					return "제공권우세";
				case 3:
					return "제공권열세";
				case 4:
					return "제공권상실";
				default:
					return "불명";
			}
		}



		/// <summary>
		/// 昼戦攻撃種別を表す文字列を取得します。
		/// </summary>
		public static string GetDayAttackKind(DayAttackKind id)
		{
			switch (id)
			{
				case DayAttackKind.NormalAttack:
					return "일반공격";
				case DayAttackKind.Laser:
					return "레이저공격";
				case DayAttackKind.DoubleShelling:
					return "연격";
				case DayAttackKind.CutinMainSub:
					return "컷인(주포/부포)";
				case DayAttackKind.CutinMainRadar:
					return "컷인(주포/전탐)";
				case DayAttackKind.CutinMainAP:
					return "컷인(주포/철갑)";
				case DayAttackKind.CutinMainMain:
					return "컷인(주포/주포)";
				case DayAttackKind.CutinAirAttack:
					return "대공컷인";
				case DayAttackKind.Shelling:
					return "포격";
				case DayAttackKind.AirAttack:
					return "공습";
				case DayAttackKind.DepthCharge:
					return "폭뢰공격";
				case DayAttackKind.Torpedo:
					return "뇌격";
				case DayAttackKind.Rocket:
					return "로켓사격";
				case DayAttackKind.LandingDaihatsu:
					return "기지공격(대발동정)";
				case DayAttackKind.LandingTokuDaihatsu:
					return "기지공격(특대발동정)";
				case DayAttackKind.LandingDaihatsuTank:
					return "기지공격(대발전차)";
				case DayAttackKind.LandingAmphibious:
					return "기지공격(내화정)";
				case DayAttackKind.LandingTokuDaihatsuTank:
					return "기지공격(특대발전차)";
				default:
					return "불명";
			}
		}


		/// <summary>
		/// 夜戦攻撃種別を表す文字列を取得します。
		/// </summary>
		public static string GetNightAttackKind(NightAttackKind id)
		{
			switch (id)
			{
				case NightAttackKind.NormalAttack:
					return "일반공격";
				case NightAttackKind.DoubleShelling:
					return "연격";
				case NightAttackKind.CutinMainTorpedo:
					return "컷인(주포/어뢰)";
				case NightAttackKind.CutinTorpedoTorpedo:
					return "컷인(어뢰x2)";
				case NightAttackKind.CutinMainSub:
					return "컷인(주포x2/부포)";
				case NightAttackKind.CutinMainMain:
					return "컷인(주포x3)";
				case NightAttackKind.CutinAirAttack:
					return "대공컷인";
				case NightAttackKind.CutinTorpedoRadar:
					return "구축컷인(주포/어뢰/전탐)";
				case NightAttackKind.CutinTorpedoPicket:
					return "구축컷인(어뢰/견시원/전탐)";
				case NightAttackKind.Shelling:
					return "포격";
				case NightAttackKind.AirAttack:
					return "공습";
				case NightAttackKind.DepthCharge:
					return "폭뢰공격";
				case NightAttackKind.Torpedo:
					return "뇌격";
				case NightAttackKind.Rocket:
					return "로켓사격";
                case NightAttackKind.LandingDaihatsu:
                    return "기지공격(대발동정)";
                case NightAttackKind.LandingTokuDaihatsu:
                    return "기지공격(특대발동정)";
                case NightAttackKind.LandingDaihatsuTank:
                    return "기지공격(대발전차)";
                case NightAttackKind.LandingAmphibious:
                    return "기지공격(내화정)";
                case NightAttackKind.LandingTokuDaihatsuTank:
                    return "기지공격(특대발전차)";
                default:
                    return "불명";
            }
		}


		/// <summary>
		/// 対空カットイン種別を表す文字列を取得します。
		/// </summary>
		public static string GetAACutinKind(int id)
		{
			switch (id)
			{
				case 0:
					return "없음";
				case 1:
					return "고각포x2/전탐<秋月>";
				case 2:
					return "고각포/전탐<秋月>";
				case 3:
					return "고각포x2<秋月>";
				case 4:
					return "대구경주포/삼식탄/고사장치/전탐";
				case 5:
					return "고각포+고사장치x2/전탐";
				case 6:
					return "대구경주포/삼식탄/고사장치";
				case 7:
					return "고각포/고사장치/전탐";
				case 8:
					return "고각포+고사장치/전탐";
				case 9:
					return "고각포/고사장치";
				case 10:
					return "고각포/집중배치/전탐<마야>";
				case 11:
					return "고각포/집중배치<마야>";
				case 12:
					return "집중배치/기총/전탐";
				case 14:
					return "고각포/기총/전탐<이스즈>";
				case 15:
					return "고각포/기총<이스즈>";
				case 16:
					return "고각포/기총/전탐<카스미>";
				case 17:
					return "고각포/기총<카스미>";
				case 18:
					return "집중배치<사츠키>";
				case 19:
					return "고각포(고사장치X)/집중배치<키누>";
				case 20:
					return "집중배치<키누>";
				case 21:
					return "고각포/전탐<유라>";
				case 22:
					return "집중배치<후미즈키>";
				case 23:
					return "기총(집배X)<UIT-25>";
				case 24:
					return "고각포/기총(집배X)<타츠타>";
				case 25:
					return "분진포개2/전탐/삼식탄<이세>";
				case 26:
					return "고각포+증설기총/전탐<무사시>";
				case 28:
					return "분진포개2/전탐<이세>";
				case 29:
					return "고각포/전탐<하마카제>";
				default:
					return "불명";
			}
		}


		/// <summary>
		/// 勝利ランクを表すIDを取得します。
		/// </summary>
		public static int GetWinRank(string rank)
		{
			switch (rank.ToUpper())
			{
				case "E":
					return 1;
				case "D":
					return 2;
				case "C":
					return 3;
				case "B":
					return 4;
				case "A":
					return 5;
				case "S":
					return 6;
				case "SS":
					return 7;
				default:
					return 0;
			}
		}

		/// <summary>
		/// 勝利ランクを表す文字列を取得します。
		/// </summary>
		public static string GetWinRank(int rank)
		{
			switch (rank)
			{
				case 1:
					return "E";
				case 2:
					return "D";
				case 3:
					return "C";
				case 4:
					return "B";
				case 5:
					return "A";
				case 6:
					return "S";
				case 7:
					return "SS";
				default:
					return "不明";
			}
		}

		#endregion


		#region その他

		/// <summary>
		/// 資源の名前を取得します。
		/// </summary>
		/// <param name="materialID">資源のID。</param>
		/// <returns>資源の名前。</returns>
		public static string GetMaterialName(int materialID)
		{

			switch (materialID)
			{
				case 1:
					return "연료";
				case 2:
					return "탄약";
				case 3:
					return "강재";
				case 4:
					return "보크사이트";
				case 5:
					return "고속건조재";
				case 6:
					return "고속수복재";
				case 7:
					return "개발자재";
				case 8:
					return "개수자재";
				default:
					return "불명";
			}
		}


		/// <summary>
		/// 階級を表す文字列を取得します。
		/// </summary>
		public static string GetAdmiralRank(int id)
		{
			switch (id)
			{
				case 1:
					return "원수";
				case 2:
					return "대장";
				case 3:
					return "중장";
				case 4:
					return "소장";
				case 5:
					return "대령";
				case 6:
					return "중령";
				case 7:
					return "신입중령";
				case 8:
					return "소령";
				case 9:
					return "중견소령";
				case 10:
					return "신입소령";
				default:
					return "제독";
			}
		}


		/// <summary>
		/// 任務の発生タイプを表す文字列を取得します。
		/// </summary>
		public static string GetQuestType(int id)
		{
			switch (id)
			{
				case 1:     //デイリー
					return "일간";
				case 2:     //ウィークリー
					return "주간";
				case 3:     //マンスリー
					return "월간";
				case 4:     //単発
					return "일회";
				case 5:     //その他(輸送5/空母3)
					return "기타";
				default:
					return "?";
			}

		}


		/// <summary>
		/// 任務のカテゴリを表す文字列を取得します。
		/// </summary>
		public static string GetQuestCategory(int id)
		{
			switch (id)
			{
				case 1:
					return "편성";
				case 2:
					return "출격";
				case 3:
					return "연습";
				case 4:
					return "원정";
				case 5:
					return "보급";        //入渠も含むが、文字数の関係
				case 6:
					return "공창";
				case 7:
					return "개수";
				case 8:
					return "출격";
				case 9:
					return "기타";
				default:
					return "不明";
			}
		}


		/// <summary>
		/// 遠征の結果を表す文字列を取得します。
		/// </summary>
		public static string GetExpeditionResult(int value)
		{
			switch (value)
			{
				case 0:
					return "실패";
				case 1:
					return "성공";
				case 2:
					return "대성공";
				default:
					return "불명";
			}
		}


		/// <summary>
		/// 連合艦隊の編成名を表す文字列を取得します。
		/// </summary>
		public static string GetCombinedFleet(int value)
		{
			switch (value)
			{
				case 0:
					return "일반함대";
				case 1:
					return "기동함대";
				case 2:
					return "수상부대";
				case 3:
					return "수송부대";
				default:
					return "불명";
			}
		}

		#endregion

	}

}
