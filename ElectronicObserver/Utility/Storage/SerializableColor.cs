using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;

namespace ElectronicObserver.Utility.Storage
{

	/// <summary>
	/// シリアル化可能な <see cref="System.Drawing.Color"/> を扱います。
	/// </summary>
	[DataContract(Name = "SerializableColor")]
	[DebuggerDisplay("{ColorData}")]
	public class SerializableColor : IEquatable<SerializableColor>
	{

		[IgnoreDataMember]
		public Color ColorData { get; set; }


		public SerializableColor()
		{
            this.ColorData = Color.Black;
		}

		public SerializableColor(Color color)
		{
            this.ColorData = color;
		}

		public SerializableColor(string attribute)
		{
            this.SerializedColor = attribute;
		}

		public SerializableColor(uint colorCode)
		{
            this.ColorData = UIntToColor(colorCode);
		}


		[DataMember]
		public string SerializedColor
		{
			get { return ColorToString(this.ColorData); }
			set { this.ColorData = StringToColor(value); }
		}

		[IgnoreDataMember]
		public uint ColorCode
		{
			get { return ColorToUint(this.ColorData); }
			set { this.ColorData = UIntToColor(value); }
		}


		public static implicit operator Color(SerializableColor color)
		{
			if (color == null) return UIntToColor(0);
			return color.ColorData;
		}

		public static implicit operator SerializableColor(Color color)
		{
			return new SerializableColor(color);
		}


		public static Color StringToColor(string value)
		{
			if (value == null || value == string.Empty || !uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out uint i))
				return UIntToColor(0);

			return UIntToColor(i);
		}

		public static Color UIntToColor(uint color)
		{
			return Color.FromArgb(
				(int)((color >> 24) & 0xFF),
				(int)((color >> 16) & 0xFF),
				(int)((color >> 8) & 0xFF),
				(int)((color >> 0) & 0xFF));
		}

		public static string ColorToString(Color color)
		{
			return ColorToUint(color).ToString("X8");
		}

		public static uint ColorToUint(Color color)
		{
			return
				((uint)(color.A) << 24) |
				((uint)(color.R) << 16) |
				((uint)(color.G) << 8) |
				((uint)(color.B) << 0);
		}

		public bool Equals(SerializableColor other) => this.SerializedColor == other?.SerializedColor;
	}
}
