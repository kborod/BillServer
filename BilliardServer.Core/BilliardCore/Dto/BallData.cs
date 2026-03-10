namespace Kborod.BilliardCore
{
    public class BallData : IBallData
    {
        public int Number { get; set; }
        public bool IsRemoved { get; set; }

        public long Xraw { get; set; }

        public long Yraw { get; set; }

        public void SetPosition(Fixed64 x, Fixed64 y)
        {
            Xraw = x.Raw; Yraw = y.Raw;
        }

        public override string ToString()
        {
            //return $"Num:{Number}_IsRemoved:{IsRemoved}_Xraw:{Xraw} ({new Fixed64(Xraw).ToFloat()})_Yraw:{Yraw} ({new Fixed64(Yraw).ToFloat()})";
            return $"Num:{Number}_IsRemoved:{IsRemoved}_Xraw:{Xraw} _Yraw:{Yraw} ";
        }
    }
}
