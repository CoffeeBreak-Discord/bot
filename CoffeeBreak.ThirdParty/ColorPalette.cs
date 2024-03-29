using System.Drawing;
using System.Globalization;

namespace CoffeeBreak.ThirdParty;
public class ColorPallete
{
    public class RGBPlate
    {
        public int R
        {
            get { return _plate.R; }
        }

        public int G
        {
            get { return _plate.G; }
        }

        public int B
        {
            get { return _plate.B; }
        }

        public string HexCode
        {
            get { return $"{_plate.R.ToString("X2")}{_plate.G.ToString("X2")}{_plate.B.ToString("X2")}"; }
        }

        public uint IntCode
        {
            get { return uint.Parse(this.HexCode, NumberStyles.HexNumber); }
        }

        private Color _plate;
        public Color ToColor() => _plate;

        public RGBPlate(int r, int g, int b)
        {
            _plate = Color.FromArgb(r, g, b);
        }
    }

    public RGBPlate[] Plate;
    public ColorPallete(RGBPlate[] plate)
    {
            this.Plate = plate;
    }

    public RGBPlate Randomize()
    {
        return this.Plate[new Random().Next(this.Plate.Count())];
    }
}