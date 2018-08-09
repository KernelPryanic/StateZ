using GTA;
using GTA.Math;
using GTA.Native;
using System;
using StateZ.Extensions;
using StateZ.Scripts;
using StateZ.Static;

namespace StateZ.DataClasses
{
	public class ItemPreview
	{
		private Vector3 _currentOffset;

		private Prop _currentPreview;

		private Prop _resultProp;

		private bool _preview;

		private bool _isDoor;

		private string _currnetPropHash;

		public bool PreviewComplete
		{
			get;
			private set;
		}

		public ItemPreview()
		{
			ScriptEventHandler.Instance.RegisterScript(OnTick);
			ScriptEventHandler.Instance.Aborted += delegate
			{
				Abort();
			};
		}

		public void OnTick(object sender, EventArgs eventArgs)
		{
			if (_preview)
			{
				CreateItemPreview();
			}
		}

		public Prop GetResult()
		{
			return _resultProp;
		}

		public void StartPreview(string propHash, Vector3 offset, bool isDoor)
		{
			if (!_preview)
			{
				_preview = true;
				_currnetPropHash = propHash;
				_isDoor = isDoor;
			}
		}

		private void CreateItemPreview()
		{
			if (_currentPreview == null)
			{
				PreviewComplete = false;
				_currentOffset = Vector3.Zero;
				Prop newProp = World.CreateProp(_currnetPropHash, default(Vector3), default(Vector3), false, false);
				if (newProp == null)
				{
					UI.Notify($"Failed to load prop, even after request.\nProp Name: {_currnetPropHash}");
					_resultProp = null;
					_preview = false;
					PreviewComplete = true;
				}
				else
				{
					newProp.HasCollision = false;
					_currentPreview = newProp;
					_currentPreview.Alpha = 150;
					Database.PlayerPed.Weapons.Select(GTA.Native.WeaponHash.Unarmed, true);
					_resultProp = null;
				}
			}
			else
			{
				UiExtended.DisplayHelpTextThisFrame("Press ~INPUT_AIM~ to cancel.\nPress ~INPUT_ATTACK~ to place the item.", true);
				Game.DisableControlThisFrame(2, GTA.Control.Aim);
				Game.DisableControlThisFrame(2, GTA.Control.Attack);
				Game.DisableControlThisFrame(2, GTA.Control.Attack2);
				Game.DisableControlThisFrame(2, GTA.Control.ParachuteBrakeLeft);
				Game.DisableControlThisFrame(2, GTA.Control.ParachuteBrakeRight);
				Game.DisableControlThisFrame(2, GTA.Control.Cover);
				Game.DisableControlThisFrame(2, GTA.Control.Phone);
				Game.DisableControlThisFrame(2, GTA.Control.PhoneUp);
				Game.DisableControlThisFrame(2, GTA.Control.PhoneDown);
				Game.DisableControlThisFrame(2, GTA.Control.Sprint);
				GameExtended.DisableWeaponWheel();
				if (Game.IsDisabledControlPressed(2, GTA.Control.Aim))
				{
					_currentPreview.Delete();
					_currentPreview = (_resultProp = null);
					_preview = false;
					PreviewComplete = true;
					ScriptEventHandler.Instance.UnregisterScript(OnTick);
				}
				else
				{
					Vector3 camPos = GameplayCamera.Position;
					Vector3 camDir = GameplayCamera.Direction;
					RaycastResult cast = World.Raycast(camPos, camPos + camDir * 15f, -1, new IntersectOptions(), Database.PlayerPed);
					Vector3 coords = cast.HitCoords;
					if (coords != Vector3.Zero && coords.DistanceTo(Database.PlayerPosition) > 1.5f)
					{
						DrawScaleForms();
						float speed = Game.IsControlPressed(2, GTA.Control.Sprint) ? 1.5f : 1f;
						if (Game.IsControlPressed(2, GTA.Control.ParachuteBrakeLeft))
						{
							Vector3 rotation2 = _currentPreview.Rotation;
							rotation2.Z += Game.LastFrameTime * 50f * speed;
							_currentPreview.Rotation = rotation2;
						}
						else if (Game.IsControlPressed(2, GTA.Control.ParachuteBrakeRight))
						{
							Vector3 rotation = _currentPreview.Rotation;
							rotation.Z -= Game.LastFrameTime * 50f * speed;
							_currentPreview.Rotation = rotation;
						}
						if (Game.IsControlPressed(2, GTA.Control.PhoneUp))
						{
							_currentOffset.Z += Game.LastFrameTime * speed;
						}
						else if (Game.IsControlPressed(2, GTA.Control.PhoneDown))
						{
							_currentOffset.Z -= Game.LastFrameTime * speed;
						}
						_currentPreview.Position = coords + _currentOffset;
						_currentPreview.IsVisible = true;
						if (Game.IsDisabledControlJustPressed(2, GTA.Control.Attack))
						{
							_currentPreview.ResetAlpha();
							_resultProp = _currentPreview;
							_resultProp.HasCollision = true;
							_resultProp.FreezePosition = !_isDoor;
							_preview = false;
							_currentPreview = null;
							_currnetPropHash = string.Empty;
							PreviewComplete = true;
							ScriptEventHandler.Instance.UnregisterScript(OnTick);
						}
					}
					else
					{
						_currentPreview.IsVisible = false;
					}
				}
			}
		}

		private static void DrawScaleForms()
		{
			Scaleform scaleform = new Scaleform("instructional_buttons");
			scaleform.CallFunction("CLEAR_ALL", new object[0]);
			scaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", new object[1]
			{
				0
			});
			scaleform.CallFunction("CREATE_CONTAINER", new object[0]);
			scaleform.CallFunction("SET_DATA_SLOT", new object[3]
			{
				0,
				Function.Call<string>(GTA.Native.Hash._GET_CONTROL_ACTION_NAME, (InputArgument[])new InputArgument[3]
				{
					2,
					152,
					0
				}),
				string.Empty
			});
			scaleform.CallFunction("SET_DATA_SLOT", new object[3]
			{
				1,
				Function.Call<string>(GTA.Native.Hash._GET_CONTROL_ACTION_NAME, (InputArgument[])new InputArgument[3]
				{
					2,
					153,
					0
				}),
				"Rotate"
			});
			scaleform.CallFunction("SET_DATA_SLOT", new object[3]
			{
				2,
				Function.Call<string>(GTA.Native.Hash._GET_CONTROL_ACTION_NAME, (InputArgument[])new InputArgument[3]
				{
					2,
					172,
					0
				}),
				string.Empty
			});
			scaleform.CallFunction("SET_DATA_SLOT", new object[3]
			{
				3,
				Function.Call<string>(GTA.Native.Hash._GET_CONTROL_ACTION_NAME, (InputArgument[])new InputArgument[3]
				{
					2,
					173,
					0
				}),
				"Lift/Lower"
			});
			scaleform.CallFunction("SET_DATA_SLOT", new object[3]
			{
				4,
				Function.Call<string>(GTA.Native.Hash._GET_CONTROL_ACTION_NAME, (InputArgument[])new InputArgument[3]
				{
					2,
					21,
					0
				}),
				"Accelerate"
			});
			scaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", new object[1]
			{
				-1
			});
			scaleform.Render2D();
		}

		public void Abort()
		{
			Prop currentPreview = _currentPreview;
			if (currentPreview != null)
			{
				currentPreview.Delete();
			}
		}
	}
}
