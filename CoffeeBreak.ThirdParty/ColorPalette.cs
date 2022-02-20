namespace CoffeeBreak.ThirdParty;

public class ColorPallete
{
    public class RGBPlate
    {
        public readonly int R;
        public readonly int G;
        public readonly int B;

        public RGBPlate(int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }
    }

    private RGBPlate[] _plate;
    public ColorPallete(RGBPlate[] plate)
    {
            _plate = plate;
    }

    public RGBPlate Randomize()
    {
        return _plate[new Random().Next(_plate.Count())];
    }
}