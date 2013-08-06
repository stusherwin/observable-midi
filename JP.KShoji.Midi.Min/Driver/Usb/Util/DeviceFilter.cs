/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System.Collections.Generic;
using System.IO;
using Android.Content;
using Android.Hardware.Usb;
using Android.Util;
using JP.KShoji.Driver.Midi;
using JP.KShoji.Driver.Midi.Util;
using Org.Xmlpull.V1;
using Sharpen;

namespace JP.KShoji.Driver.Usb.Util
{
	/// <summary>Utility methods from com.android.server.usb.UsbSettingsManager.DeviceFilter
	/// 	</summary>
	/// <seealso>http://ics-custom-services.googlecode.com/git-history/1899c9df4b68885df4f351fa9feee603a08ee8ec/java/com/android/server/usb/UsbSettingsManager.java
	/// 	</seealso>
	/// <author>K.Shoji</author>
	public sealed class DeviceFilter
	{
		private readonly int usbVendorId;

		private readonly int usbProductId;

		private readonly int usbClass;

		private readonly int usbSubclass;

		private readonly int usbProtocol;

		/// <summary>constructor</summary>
		/// <param name="vendorId"></param>
		/// <param name="productId"></param>
		/// <param name="clasz"></param>
		/// <param name="subclass"></param>
		/// <param name="protocol"></param>
		public DeviceFilter(int vendorId, int productId, int clasz, int subclass, int protocol
			)
		{
			// USB Vendor ID (or -1 for unspecified)
			// USB Product ID (or -1 for unspecified)
			// USB device or interface class (or -1 for unspecified)
			// USB device subclass (or -1 for unspecified)
			// USB device protocol (or -1 for unspecified)
			usbVendorId = vendorId;
			usbProductId = productId;
			usbClass = clasz;
			usbSubclass = subclass;
			usbProtocol = protocol;
		}

		/// <summary>Load DeviceFilter from resources(res/xml/device_filter.xml).</summary>
		/// <remarks>Load DeviceFilter from resources(res/xml/device_filter.xml).</remarks>
		/// <param name="context"></param>
		/// <returns></returns>
//		public static IList<JP.KShoji.Driver.Usb.Util.DeviceFilter> GetDeviceFilters(Context
//			 context)
//		{
//			// create device filter
//			XmlPullParser parser = context.Resources.GetXml(R.Xml.device_filter);
//			IList<JP.KShoji.Driver.Usb.Util.DeviceFilter> deviceFilters = new AList<JP.KShoji.Driver.Usb.Util.DeviceFilter
//				>();
//			try
//			{
//				int hasNext = XmlPullParser.START_DOCUMENT;
//				while (hasNext != XmlPullParser.END_DOCUMENT)
//				{
//					hasNext = parser.Next();
//					JP.KShoji.Driver.Usb.Util.DeviceFilter deviceFilter = ParseXml(parser);
//					if (deviceFilter != null)
//					{
//						deviceFilters.AddItem(deviceFilter);
//					}
//				}
//			}
//			catch (XmlPullParserException e)
//			{
//                throw new System.Exception("XmlPullParserException", e);
//			}
//			catch (IOException e)
//			{
//                throw new System.Exception("IOException", e);
//			}
//			return Sharpen.Collections.UnmodifiableList(deviceFilters);
//		}

		/// <summary>
		/// convert
		/// <see cref="Org.Xmlpull.V1.XmlPullParser">Org.Xmlpull.V1.XmlPullParser</see>
		/// into
		/// <see cref="DeviceFilter">DeviceFilter</see>
		/// </summary>
		/// <param name="parser"></param>
		/// <returns>
		/// parsed
		/// <see cref="DeviceFilter">DeviceFilter</see>
		/// </returns>
		/*pubc static JP.KShoji.Driver.Usb.Util.DeviceFilter ParseXml(XmlPullParser parser
			)
		{
			int vendorId = -1;
			int productId = -1;
			int deviceClass = -1;
			int deviceSubclass = -1;
			int deviceProtocol = -1;
			int count = parser.AttributeCount;
			for (int i = 0; i < count; i++)
			{
				string name = parser.GetAttributeName(i);
				// All attribute values are ints
				int value = System.Convert.ToInt32(parser.GetAttributeValue(i));
				if ("vendor-id".Equals(name))
				{
					vendorId = value;
				}
				else
				{
					if ("product-id".Equals(name))
					{
						productId = value;
					}
					else
					{
						if ("class".Equals(name))
						{
							deviceClass = value;
						}
						else
						{
							if ("subclass".Equals(name))
							{
								deviceSubclass = value;
							}
							else
							{
								if ("protocol".Equals(name))
								{
									deviceProtocol = value;
								}
							}
						}
					}
				}
			}
			// all blank(may be not proper tags)
			if (vendorId == -1 && productId == -1 && deviceClass == -1 && deviceSubclass == -
				1 && deviceProtocol == -1)
			{
				return null;
			}
			return new JP.KShoji.Driver.Usb.Util.DeviceFilter(vendorId, productId, deviceClass
				, deviceSubclass, deviceProtocol);
		}*/

		//<summary>check equals</summary>
		/// <param name="clasz"></param>
		/// <param name="subclass"></param>
		/// <param name="protocol"></param>
		/// <returns></returns>
		private bool Matches(int clasz, int subclass, int protocol)
		{
			return ((usbClass == -1 || clasz == usbClass) && (usbSubclass == -1 || subclass ==
				 usbSubclass) && (usbProtocol == -1 || protocol == usbProtocol));
		}

		/// <summary>check equals</summary>
		/// <param name="device"></param>
		/// <returns></returns>
		public bool Matches(UsbDevice device)
		{
            return true;
			/*if (usbVendorId != -1 && device.VendorId != usbVendorId)
			{
				return false;
			}
			if (usbProductId != -1 && device.ProductId != usbProductId)
			{
				return false;
			}
			// check device class/subclass/protocol
            if (Matches((int)device.DeviceClass, (int)device.DeviceSubclass, (int)device.DeviceProtocol))
			{
				return true;
			}
			// if device doesn't match, check the interfaces
			int count = device.InterfaceCount;
			for (int i = 0; i < count; i++)
			{
				UsbInterface intf = device.GetInterface(i);
                if (Matches((int)intf.InterfaceClass, (int)intf.InterfaceSubclass, (int)intf.InterfaceProtocol))
				{
					return true;
				}
			}
			return false;*/
		}
	}
}
