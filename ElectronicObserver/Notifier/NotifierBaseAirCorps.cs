using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Notifier
{
    /// <summary>
    /// 基地航空隊出撃通知を扱います。
    /// </summary>
    public class NotifierBaseAirCorps : NotifierBase
    {
        private readonly static TimeSpan RelocationSpan = TimeSpan.FromMinutes(12);

        /// <summary>
        /// 未補給時に通知する
        /// </summary>
        public bool NotifiesNotSupplied { get; set; }

        /// <summary>
        /// 疲労時に通知する
        /// </summary>
        public bool NotifiesTired { get; set; }

        /// <summary>
        /// 編成されていないときに通知する
        /// </summary>
        public bool NotifiesNotOrganized { get; set; }


        /// <summary>
        /// 待機のとき通知する
        /// </summary>
        public bool NotifiesStandby { get; set; }

        /// <summary>
        /// 退避の時通知する
        /// </summary>
        public bool NotifiesRetreat { get; set; }

        /// <summary>
        /// 休息の時通知する
        /// </summary>
        public bool NotifiesRest { get; set; }


        /// <summary>
        /// 通常海域で通知する
        /// </summary>
        public bool NotifiesNormalMap { get; set; }

        /// <summary>
        /// イベント海域で通知する
        /// </summary>
        public bool NotifiesEventMap { get; set; }


        /// <summary>
        /// 基地枠の配置転換完了時に通知する
        /// </summary>
        public bool NotifiesSquadronRelocation { get; set; }

        /// <summary>
        /// 装備の配置転換完了時に通知する
        /// </summary>
        public bool NotifiesEquipmentRelocation { get; set; }


        // supress when sortieing

        private bool _isAlreadyNotified = false;
        private bool _isInSortie = false;
        private HashSet<int> _notifiedEquipments = new HashSet<int>();



        public NotifierBaseAirCorps()
            : base()
        {
            this.Initalize();
        }

        public NotifierBaseAirCorps(Utility.Configuration.ConfigurationData.ConfigNotifierBaseAirCorps config)
            : base(config)
        {
            this.Initalize();

            this.NotifiesNotSupplied = config.NotifiesNotSupplied;
            this.NotifiesTired = config.NotifiesTired;
            this.NotifiesNotOrganized = config.NotifiesNotOrganized;

            this.NotifiesStandby = config.NotifiesStandby;
            this.NotifiesRetreat = config.NotifiesRetreat;
            this.NotifiesRest = config.NotifiesRest;

            this.NotifiesNormalMap = config.NotifiesNormalMap;
            this.NotifiesEventMap = config.NotifiesEventMap;

            this.NotifiesSquadronRelocation = config.NotifiesSquadronRelocation;
            this.NotifiesEquipmentRelocation = config.NotifiesEquipmentRelocation;
        }

        private void Initalize()
        {
            this.DialogData.Title = "기지 항공대 알림";

            var o = APIObserver.Instance;

            o["api_port/port"].ResponseReceived += this.Port;
            o["api_get_member/mapinfo"].ResponseReceived += this.BeforeSortie;
            o["api_get_member/sortie_conditions"].ResponseReceived += this.BeforeSortieEventMap;
            o["api_req_map/start"].RequestReceived += this.Sally;
        }

        private void Port(string apiname, dynamic data)
        {
            this._isAlreadyNotified = false;
            this._isInSortie = false;
            this._notifiedEquipments.Clear();
        }

        private void BeforeSortieEventMap(string apiname, dynamic data)
        {
            if (this._isAlreadyNotified)
                return;

            if (!this.NotifiesEventMap)
                return;

            var db = KCDatabase.Instance;
            this.CheckBaseAirCorps(db.BaseAirCorps.Values.Where(corps => db.MapArea[corps.MapAreaID].MapType == 1));
        }

        private void BeforeSortie(string apiname, dynamic data)
        {
            if (this._isAlreadyNotified)
                return;

            if (!this.NotifiesNormalMap)
                return;

            var db = KCDatabase.Instance;
            this.CheckBaseAirCorps(db.BaseAirCorps.Values.Where(corps => db.MapArea[corps.MapAreaID].MapType == 0));
        }


        private bool CheckBaseAirCorps(IEnumerable<BaseAirCorpsData> corpslist)
        {
            var db = KCDatabase.Instance;
            var sb = new StringBuilder();
            var messages = new LinkedList<string>();

            foreach (var corps in corpslist)
            {
                if (this.NotifiesNotSupplied && corps.Squadrons.Values.Any(sq => sq.State == 1 && sq.AircraftCurrent < sq.AircraftMax))
                    messages.AddLast("미보급");
                if (this.NotifiesTired && corps.Squadrons.Values.Any(sq => sq.State == 1 && sq.Condition > 1))
                    messages.AddLast("피로도");
                if (this.NotifiesNotOrganized)
                {
                    if (corps.Squadrons.Values.Any(sq => sq.State == 0))
                        messages.AddLast("미편성");
                    if (corps.Squadrons.Values.Any(sq => sq.State == 2))
                        messages.AddLast("배치전환중");
                }

                if (this.NotifiesStandby && corps.ActionKind == 0)
                    messages.AddLast("대기중");
                if (this.NotifiesRetreat && corps.ActionKind == 3)
                    messages.AddLast("대피중");
                if (this.NotifiesRest && corps.ActionKind == 4)
                    messages.AddLast("휴식중");

                if (messages.Any())
                {
                    if (sb.Length == 0)
                        sb.Append("기지 항공대의 출격 준비가 완료되지 않았습니다.：");
                    sb.Append($"#{corps.MapAreaID} {corps.Name} ({string.Join(", ", messages)})");
                }
                messages.Clear();
            }

            if (sb.Length > 0)
            {
                this.Notify(sb.ToString());
                this._isAlreadyNotified = true;
                return true;
            }
            return false;
        }

        private void Sally(string apiname, dynamic data)
        {
            this._isInSortie = true;
        }


        protected override void UpdateTimerTick()
        {
            var db = KCDatabase.Instance;

            if (!db.RelocatedEquipments.Any())
                return;

            if (this._isInSortie)
                return;

            if (this.NotifiesSquadronRelocation)
            {

                StringBuilder sb = null;
                foreach (var corps in db.BaseAirCorps.Values.Where(corps =>
                         (this.NotifiesNormalMap && db.MapArea[corps.MapAreaID].MapType == 0) ||
                         (this.NotifiesEventMap && db.MapArea[corps.MapAreaID].MapType == 1)))
                {
                    var targetSquadrons = corps.Squadrons.Values.Where(sq => sq.State == 2 && !this._notifiedEquipments.Contains(sq.EquipmentID) && (DateTime.Now - sq.RelocatedTime) >= RelocationSpan);

                    if (targetSquadrons.Any())
                    {
                        sb = sb?.Append(", ") ?? new StringBuilder();

                        sb.Append(string.Join(", ", targetSquadrons.Select(sq =>
                            $"#{corps.MapAreaID} {corps.Name} 第{sq.SquadronID}中隊 ({sq.EquipmentInstance.NameWithLevel})")));

                        foreach (var sq in targetSquadrons)
                            this._notifiedEquipments.Add(sq.EquipmentID);
                    }
                }

                if (sb != null)
                {
                    this.Notify(sb.ToString() + "의 배치전환이 완료되었습니다. 모항에 돌아가면 업데이트됩니다.");
                }
            }

            if (this.NotifiesEquipmentRelocation)
            {
                var targets = db.RelocatedEquipments.Where(kv => !this._notifiedEquipments.Contains(kv.Key) &&
                    (DateTime.Now - kv.Value.RelocatedTime) >= RelocationSpan);

                if (targets.Any())
                {
                    this.Notify($"{string.Join(", ", targets.Select(kv => kv.Value.EquipmentInstance.NameWithLevel))} 의 배치전환이 완료되었습니다. 모항에 돌아가면 업데이트됩니다.");

                    foreach (var t in targets)
                        this._notifiedEquipments.Add(t.Key);
                }
            }


        }

        public void Notify(string message)
        {
            this.DialogData.Title = "기지 항공대 알림";
            this.DialogData.Message = message;

            base.Notify();
        }

        public override void ApplyToConfiguration(Utility.Configuration.ConfigurationData.ConfigNotifierBase config)
        {
            base.ApplyToConfiguration(config);

            var c = config as Utility.Configuration.ConfigurationData.ConfigNotifierBaseAirCorps;

            if (c != null)
            {
                c.NotifiesNotSupplied = this.NotifiesNotSupplied;
                c.NotifiesTired = this.NotifiesTired;
                c.NotifiesNotOrganized = this.NotifiesNotOrganized;

                c.NotifiesStandby = this.NotifiesStandby;
                c.NotifiesRetreat = this.NotifiesRetreat;
                c.NotifiesRest = this.NotifiesRest;

                c.NotifiesNormalMap = this.NotifiesNormalMap;
                c.NotifiesEventMap = this.NotifiesEventMap;

                c.NotifiesSquadronRelocation = this.NotifiesSquadronRelocation;
                c.NotifiesEquipmentRelocation = this.NotifiesEquipmentRelocation;
            }
        }
    }
}