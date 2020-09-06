/*
 * Copyright (c) 2020 LG Electronics Inc.
 *
 * SPDX-License-Identifier: MIT
 */

using UnityEngine;
using System.Threading;
using Stopwatch = System.Diagnostics.Stopwatch;

public class LaserPlugin : DevicePlugin
{
	private SensorDevices.Lidar lidar = null;

	private string hashServiceKey = string.Empty;
	private string hashKey = string.Empty;

	protected override void OnAwake()
	{
		partName = DeviceHelper.GetPartName(gameObject);

		lidar = gameObject.GetComponent<SensorDevices.Lidar>();
		lidar.SetPluginParameter(parameters);
	}

	protected override void OnStart()
	{
		RegisterServiceDevice("Info");
		RegisterTxDevice();

		AddThread(Response);
		AddThread(Sender);
	}

	private void Sender()
	{
		var sw = new Stopwatch();
		while (IsRunningThread)
		{
			if (lidar != null)
			{
				var datastreamToSend = lidar.PopData();
				sw.Restart();
				Publish(datastreamToSend);
				sw.Stop();
				lidar.SetTransportedTime((float)sw.Elapsed.TotalSeconds);
			}
		}
	}

	private void Response()
	{
		while (IsRunningThread)
		{
			var receivedBuffer = ReceiveRequest();

			var requestMessage = ParsingInfoRequest(receivedBuffer, ref msForInfoResponse);

			if (requestMessage != null)
			{
				var device = lidar as Device;

				switch (requestMessage.Name)
				{
					case "request_transform":
						var devicePose = device.GetPose();

						SetTransformInfoResponse(ref msForInfoResponse, devicePose);
						break;

					default:
						break;
				}

				SendResponse(msForInfoResponse);
			}

			ThreadWait();
		}
	}
}