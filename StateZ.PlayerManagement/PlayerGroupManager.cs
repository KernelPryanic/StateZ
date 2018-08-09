using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using StateZ.Extensions;
using StateZ.Static;
using StateZ.Wrappers;

namespace StateZ.PlayerManagement
{
    public class PlayerGroupManager : Script
    {
        private readonly UIMenu _pedMenu;

        private Ped _selectedPed;

        private PedCollection _peds;

        private readonly Dictionary<Ped, PedTask> _pedTasks = new Dictionary<Ped, PedTask>();

        public Ped PlayerPed => Database.PlayerPed;

        public Vector3 PlayerPosition => Database.PlayerPosition;

        public static PlayerGroupManager Instance
        {
            get;
            private set;
        }

        public PlayerGroupManager()
        {
            Instance = this;
            _pedMenu = new UIMenu("Guard", "SELECT AN OPTION");
            MenuConrtoller.MenuPool.Add(_pedMenu);
            _pedMenu.OnMenuClose += delegate
            {
                _selectedPed = null;
            };
            UIMenuListItem tasksItem = new UIMenuListItem("Tasks", Enum.GetNames(typeof(PedTask)).Cast<object>().ToList(), 0, "Give peds a specific task to perform.");
            tasksItem.Activated += delegate
            {
                if (!(_selectedPed == null))
                {
                    PedTask index = (PedTask)tasksItem.Index;
                    SetTask(_selectedPed, index);
                }
            };
            UIMenuItem applyAllItem = new UIMenuItem("Apply To Nearby", "Apply the selected task to nearby peds within 50 meters.");
            applyAllItem.Activated += delegate
            {
                PedTask task2 = (PedTask)tasksItem.Index;
                List<Ped> list2 = (from ped in (IEnumerable<Ped>)PlayerPed.CurrentPedGroup
                                   where ped.Position.VDist(PlayerPosition) < 50f
                                   select ped).ToList();
                list2.ForEach(delegate (Ped ped)
                {
                    SetTask(ped, task2);
                });
            };
            UIMenuItem giveWeaponItem = new UIMenuItem("Give Weapon", "Give this ped your current weapon.");
            giveWeaponItem.Activated += delegate
            {
                if (!(_selectedPed == null))
                {
                    TradeWeapons(PlayerPed, _selectedPed);
                }
            };
            UIMenuItem takeWeaponItem = new UIMenuItem("Take Weapon", "Take the ped's current weapon.");
            takeWeaponItem.Activated += delegate
            {
                if (!(_selectedPed == null))
                {
                    TradeWeapons(_selectedPed, PlayerPed);
                }
            };
            UIMenuListItem globalTasks = new UIMenuListItem("Guard Tasks", Enum.GetNames(typeof(PedTask)).Cast<object>().ToList(), 0, "Give all gurads a specific task to perform.");
            globalTasks.Activated += delegate
            {
                PedTask task = (PedTask)globalTasks.Index;
                List<Ped> list = ((IEnumerable<Ped>)PlayerPed.CurrentPedGroup).ToList();
                list.ForEach(delegate (Ped ped)
                {
                    SetTask(ped, task);
                });
            };
            _pedMenu.AddItem(tasksItem);
            _pedMenu.AddItem(applyAllItem);
            _pedMenu.AddItem(giveWeaponItem);
            _pedMenu.AddItem(takeWeaponItem);
            ModController.Instance.MainMenu.AddItem(globalTasks);
            this.Tick += (EventHandler)OnTick;
            this.Aborted += (EventHandler)OnAborted;
        }

        private unsafe void SetTask(Ped ped, PedTask task)
        {
            if (task != (PedTask)(-1) && !ped.IsPlayer)
            {
                if (!_pedTasks.ContainsKey(ped))
                {
                    _pedTasks.Add(ped, task);
                }
                else
                {
                    _pedTasks[ped] = task;
                }
                ped.Task.ClearAll();
                switch (task)
                {
                    case PedTask.Chill:
                        Function.Call(GTA.Native.Hash.TASK_WANDER_IN_AREA, (InputArgument[])new InputArgument[6]
                        {
                        ped.Handle,
                        ped.Position.X,
                        ped.Position.Y,
                        ped.Position.Z,
                        100f,
                        -1
                        });
                        break;
                    case PedTask.Combat:
                        ped.Task.FightAgainstHatedTargets(100f);
                        break;
                    case PedTask.Guard:
                        ped.Task.GuardCurrentPosition();
                        break;
                    case PedTask.StandStill:
                        ped.Task.StandStill(-1);
                        break;
                    case PedTask.Leave:
                        {
                            ped.LeaveGroup();
                            Blip currentBlip = ped.CurrentBlip;
                            if (currentBlip.Handle != 0)
                            {
                                currentBlip.Remove();
                            }
                            ped.MarkAsNoLongerNeeded();
                            EntityEventWrapper.Dispose(ped);
                            break;
                        }
                    case PedTask.VehicleFollow:
                        {
                            Vehicle vehicle = World.GetClosestVehicle(ped.Position, 100f);
                            if (vehicle == null)
                            {
                                UI.Notify("There's no vehicle near this ped.", true);
                                return;
                            }
                            Function.Call(GTA.Native.Hash._TASK_VEHICLE_FOLLOW, (InputArgument[])new InputArgument[6]
                            {
                        ped.Handle,
                        vehicle.Handle,
                        PlayerPed.Handle,
                        1074528293,
                        262144,
                        15
                            });
                            break;
                        }
                }
                ped.BlockPermanentEvents = task == PedTask.Follow;
            }
        }

        private void OnAborted(object sender, EventArgs eventArgs)
        {
            PedGroup group = PlayerPed.CurrentPedGroup;
            List<Ped> peds = (from ped in (IEnumerable<Ped>)@group
                              where !ped.IsPlayer
                              select ped).ToList();
            group.Dispose();
            while (peds.Count > 0)
            {
                Ped ped2 = peds[0];
                ped2.Delete();
                peds.RemoveAt(0);
            }
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
            if (!PlayerPed.IsInVehicle() && !MenuConrtoller.MenuPool.IsAnyMenuOpen() && PlayerPed.CurrentPedGroup.MemberCount > 0)
            {
                Ped[] peds = World.GetNearbyPeds(PlayerPed, 1.5f);
                Ped ped = World.GetClosest<Ped>(PlayerPosition, peds);
                if (!(ped == null) && !ped.IsInVehicle() && !(ped.CurrentPedGroup != PlayerPed.CurrentPedGroup))
                {
                    Game.DisableControlThisFrame(2, GTA.Control.Context);
                    UiExtended.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to configure this ped.", false);
                    if (Game.IsDisabledControlJustPressed(2, GTA.Control.Context))
                    {
                        _selectedPed = ped;
                        _pedMenu.Visible = !_pedMenu.Visible;
                    }
                }
            }
        }

        public void Deserialize()
        {
            if (_peds == null)
            {
                PedCollection des = Serializer.Deserialize<PedCollection>("./scripts/Guards.dat");
                if (des == null)
                {
                    des = new PedCollection();
                }
                _peds = des;
                _peds.ListChanged += delegate
                {
                    Serializer.Serialize("./scripts/Guards.dat", _peds);
                };
                _peds.ToList().ForEach(delegate (PedData data)
                {
                    Ped ped = World.CreatePed(data.Hash, data.Position);
                    if (!(ped == null))
                    {
                        ped.Rotation = data.Rotation;
                        ped.Recruit(PlayerPed);
                        data.Weapons.ForEach(delegate (Weapon w)
                        {
                            ped.Weapons.Give(w.Hash, w.Ammo, true, true);
                        });
                        data.Handle = ped.Handle;
                        SetTask(ped, data.Task);
                    }
                });
            }
        }

        public void SavePeds()
        {
            if (_peds == null)
            {
                Deserialize();
            }
            List<Ped> group = PlayerPed.CurrentPedGroup.ToList(false);
            if (group.Count <= 0)
            {
                UI.Notify("You have no bodyguards.");
            }
            else
            {
                List<PedData> pedDatas = _peds.ToList();
                List<PedData> peds = group.ConvertAll(delegate (Ped ped)
                {
                    PedData data2 = pedDatas.Find((PedData pedData) => pedData.Handle == ped.Handle);
                    return UpdatePedData(ped, data2);
                }).ToList();
                peds.ForEach(delegate (PedData data)
                {
                    if (!_peds.Contains(data))
                    {
                        _peds.Add(data);
                    }
                });
                Serializer.Serialize("./scripts/Guards.dat", _peds);
                UI.Notify("~b~Guards~s~ saved!");
            }
        }

        private PedData UpdatePedData(Ped ped, PedData data)
        {
            PedTask task = _pedTasks.ContainsKey(ped) ? _pedTasks[ped] : ((PedTask)(-1));
            IEnumerable<WeaponHash> hashes = from hash in (WeaponHash[])Enum.GetValues(typeof(WeaponHash))
                                             where ped.Weapons.HasWeapon(hash)
                                             select hash;
            WeaponComponent[] componentHashes = (WeaponComponent[])Enum.GetValues(typeof(WeaponComponent));
            List<Weapon> weapons = hashes.ToList().ConvertAll(delegate (WeaponHash hash)
            {
                GTA.Weapon weapon = ped.Weapons[hash];
                WeaponComponent[] components = (from h in componentHashes
                                                where weapon.IsComponentActive(h)
                                                select h).ToArray();
                return new Weapon(weapon.Ammo, weapon.Hash, components);
            }).ToList();
            switch (data == null)
            {
                case true:
                    {
                        int handle = ped.Handle;
                        Model model = ped.Model;
                        data = new PedData(handle, model.Hash, ped.Rotation, ped.Position, task, weapons);
                        break;
                    }
                case false:
                    data.Position = ped.Position;
                    data.Rotation = ped.Rotation;
                    data.Task = task;
                    data.Weapons = weapons;
                    break;
            }
            return data;
        }

        private static void TradeWeapons(Ped trader, Ped reviever)
        {
            if (trader.Weapons.Current != trader.Weapons[GTA.Native.WeaponHash.Unarmed])
            {
                GTA.Weapon weapon = trader.Weapons.Current;
                if (!reviever.Weapons.HasWeapon(weapon.Hash))
                {
                    if (!reviever.IsPlayer)
                    {
                        reviever.Weapons.Drop();
                    }
                    GTA.Weapon newWeapon = reviever.Weapons.Give(weapon.Hash, 0, true, true);
                    newWeapon.Ammo = weapon.Ammo;
                    newWeapon.InfiniteAmmo = false;
                    trader.Weapons.Remove(weapon);
                }
            }
        }
    }
}
