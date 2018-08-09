using GTA;
using GTA.Native;

namespace StateZ.Extensions
{
	public static class VehicleExtended
	{
		public static VehicleClass GetModelClass(Model vehicleModel)
		{
			return Function.Call<VehicleClass>(GTA.Native.Hash.GET_VEHICLE_CLASS_FROM_NAME, new InputArgument[1]
            {
                vehicleModel.Hash
            });
		}
	}
}
