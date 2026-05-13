namespace CocomoModel
{
    public enum CocomoModelType
    {
        Organic = 0,
        SemiDetached = 1,
        Embedded = 2
    }
    
    public struct CocomoModelCoefficients
    {
        public double a { get; }
        public double b { get; }
        public double c { get; }
        public double d { get; }

        public CocomoModelCoefficients(double a, double b, double c, double d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }
    }
    
    public class CocomoModelBasic
    {
        // Таблиця коефіцієнтів базової моделі: {a, b, c, d}
        // 0 - Organic (Розповсюджений),
        // 1 - SemiDetached (Напівнезалежний),
        // 2 - Embedded (Вбудований)
        private static readonly Dictionary<CocomoModelType, CocomoModelCoefficients> ModelTypeTable = 
            new Dictionary<CocomoModelType, CocomoModelCoefficients>()
            {
                { CocomoModelType.Organic, new CocomoModelCoefficients(2.4, 1.05, 2.5, 0.38) },
                { CocomoModelType.SemiDetached, new CocomoModelCoefficients(3.0, 1.12, 2.5, 0.35) },
                { CocomoModelType.Embedded, new CocomoModelCoefficients(3.6, 1.20, 2.5, 0.32) }
            };

        // Трудомісткість ( PM = a * SIZE^b )
        public static double GetEfforts(double codeSize, CocomoModelType modelType)
        {
            var coefficient = ModelTypeTable[modelType];
            return coefficient.a * Math.Pow(codeSize, coefficient.b);
        }

        // Час розробки ( TM = c * PM^d )
        public static double GetTimeToDevelop(double codeSize, CocomoModelType modelType)
        {
            var coefficient = ModelTypeTable[modelType];
            var efforts = GetEfforts(codeSize, modelType);
            return coefficient.c * Math.Pow(efforts, coefficient.d);
        }

        // Середня чисельність персоналу ( SS = PM / TM )
        public static double GetPersonsToDevelop(double codeSize, CocomoModelType modelType)
        {
            var efforts = GetEfforts(codeSize, modelType);
            var time = GetTimeToDevelop(codeSize, modelType);
            
            return time > 0 ? efforts / time : 0;
        }
        
        // Середня чисельність персоналу ( P = SIZE / PM )
        public static double GetProductivity(double codeSize, CocomoModelType modelType)
        {
            var efforts = GetEfforts(codeSize, modelType);
            
            return codeSize > 0 ? codeSize / efforts : 0;
        }
    }
}
