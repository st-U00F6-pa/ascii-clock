using System.Text;
using System.Text.Json;

class Parameters
{
    public char dialSymbol { get; set; }
    public char secondHandSymbol { get; set; }
    public char minuteHandSymbol { get; set; }
    public char hourHandSymbol { get; set; }

    public double dialRadius { get; set; }
    public double secondHandLength { get; set; }
    public double minuteHandLength { get; set; }
    public double hourHandLength { get; set; }

    public bool smooth { get; set; }
    public bool renderLastLine { get; set; }

    public Parameters()
    {
        dialSymbol = '*';
        secondHandSymbol = 's';
        minuteHandSymbol = 'm';
        hourHandSymbol = 'H';

        dialRadius = 0.9;
        secondHandLength = 0.8;
        minuteHandLength = 0.7;
        hourHandLength = 0.5;

        smooth = true;
        renderLastLine = true;
    }
}
class Program
{
    static Parameters parameters = new Parameters();

    static int screenWidth;
    static int screenHeight;

    static char[,] screenBuffer = new char[0, 0];
    
    static StringBuilder screenBufferStringBuilder = new StringBuilder();

    static void DrawPixel(double x, double y, char c)
    {
        x = Math.Round(x);
        y = Math.Round(y);

        if (x >= 0 && x < screenWidth && y >= 0 && y < screenHeight)
        {
            screenBuffer[(int)x, (int)y] = c;
        }
    }
    static void DrawLine(double start_x, double start_y, double end_x, double end_y, char c)
    {
        double delta_x = end_x - start_x;
        double delta_y = end_y - start_y;

        int steps = (int)Math.Round(
            Math.Max(
                Math.Abs(delta_x), 
                Math.Abs(delta_y)
                )
            );

        double increment_x = delta_x / steps;
        double increment_y = delta_y / steps;

        double x = start_x;
        double y = start_y;

        for (int i = 0; i <= steps; i++)
        {

            DrawPixel(x, y, c);
            x += increment_x;
            y += increment_y;
        }
    }
    static void DrawCircle(double origin_x, double origin_y, double radius, char c)
    {
        double leftmost_x = origin_x - radius * Math.Sqrt(2);
        double rightmost_x = origin_x + radius * Math.Sqrt(2);

        double topmost_y = origin_y - radius * Math.Sqrt(0.5);
        double bottommost = origin_y + radius * Math.Sqrt(0.5);

        // For more info visit https://www.desmos.com/calculator/wrewzu4rcf

        for (double x = leftmost_x; x < rightmost_x; x++)
        {
            DrawPixel(
                x,
                origin_y + Math.Sqrt(radius * radius - Math.Pow(0.5 * x - 0.5 * origin_x, 2)),
                c
            );

            DrawPixel(
                x,
                origin_y - Math.Sqrt(radius * radius - Math.Pow(0.5 * x - 0.5 * origin_x, 2)),
                c
            );
        }

        for (double y = topmost_y; y < bottommost; y++)
        {
            DrawPixel(
                origin_x + 2 * Math.Sqrt(radius * radius - Math.Pow(y - origin_y, 2)),
                y,
                c
            );

            DrawPixel(
                origin_x - 2 * Math.Sqrt(radius * radius - Math.Pow(y - origin_y, 2)),
                y,
                c
            );
        }
    }
    static void Main(string[] args)
    {
        Console.CursorVisible = false;

        // Parameters loading

        if (!File.Exists("parameters.json"))
        {
            var parametersFile = File.CreateText("parameters.json");

            parametersFile.Write(JsonSerializer.Serialize(parameters, new JsonSerializerOptions { WriteIndented = true }));

            parametersFile.Close();
        }
        else
        {
            try
            {
                var parametersJSONString = File.ReadAllText("parameters.json");

                var deserializedParameters = JsonSerializer.Deserialize<Parameters>(parametersJSONString);

                if (deserializedParameters != null) { parameters = deserializedParameters; }
            }
            catch (Exception) { }
        }

        // Actual clock rendering

        while (true)
        {
            // Screen buffer setup

            screenWidth = Console.WindowWidth;
            screenHeight = Console.WindowHeight;

            screenBuffer = new char[screenWidth, screenHeight];

            for (int x = 0; x < screenWidth; x++)
            {
                for (int y = 0; y < screenHeight; y++)
                {

                    DrawPixel(x, y, ' ');
                }
            }

            // Clock rendering setup

            double origin_x = screenWidth / 2;
            double origin_y = screenHeight / 2;

            double max_radius = Math.Min(screenWidth / 2, screenHeight) / 2;

            DateTime now = DateTime.Now;

            // Dial rendering

            double dialRadiusPixels = parameters.dialRadius * max_radius;

            DrawCircle(origin_x, origin_y, dialRadiusPixels, parameters.dialSymbol);

            // Second hand rendering

            double handLengthPixels = parameters.secondHandLength * max_radius;

            double angle = (now.Second + (parameters.smooth ? (now.Millisecond / 1000.0) : 0)) / 60 * Math.PI * 2;
            
            double target_x = origin_x + handLengthPixels * Math.Sin(angle) * 2;
            double target_y = origin_y - handLengthPixels * Math.Cos(angle);
            
            DrawLine(origin_x, origin_y, target_x, target_y, parameters.secondHandSymbol);

            // Minute hand rendering

            handLengthPixels = parameters.minuteHandLength * max_radius;

            angle = (now.Minute + (parameters.smooth ? (now.Second / 60.0) : 0)) / 60 * Math.PI * 2;

            target_x = origin_x + handLengthPixels * Math.Sin(angle) * 2;
            target_y = origin_y - handLengthPixels * Math.Cos(angle);

            DrawLine(origin_x, origin_y, target_x, target_y, parameters.minuteHandSymbol);

            // Hour hand rendering

            handLengthPixels = parameters.hourHandLength * max_radius;

            angle = (now.Hour + (parameters.smooth ? (now.Minute / 60.0) : 0)) / 12 * Math.PI * 2;

            target_x = origin_x + handLengthPixels * Math.Sin(angle) * 2;
            target_y = origin_y - handLengthPixels * Math.Cos(angle);

            DrawLine(origin_x, origin_y, target_x, target_y, parameters.hourHandSymbol);

            // Buffer-to-terminal rendering

            screenBufferStringBuilder = new StringBuilder((screenWidth * (screenHeight + 1)));

            for (int y = 0; y < screenHeight - (parameters.renderLastLine ? 0 : 1); y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    screenBufferStringBuilder.Append(screenBuffer[x, y]);
                }
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(screenBufferStringBuilder.ToString());
        }
    }
}