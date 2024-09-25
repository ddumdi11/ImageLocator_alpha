using FlaUI.Core.Input;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ImageLocator_alpha
{
    public class LocateElementByPictureInDesktop
    {
        System.Drawing.Point? locatedPoint;
        double? maxMaxValOfMatches; // Verwende Nullable für klarere Intention

        /// <summary>
        /// Sucht ein Template auf allen Monitoren und gibt den Punkt zurück.
        /// </summary>
        /// <param name="templateFilePath">Pfad zum Template-Bild</param>
        /// <param name="resultFilePath">Pfad, um das Ergebnisbild zu speichern</param>
        /// <returns>Den Punkt, an dem das Template gefunden wurde</returns>
        public System.Drawing.Point? SearchTemplateOnEachMonitor(string templateFilePath, string resultFilePath)
        {
            // Reset locatedPoint und maxMaxValOfMatches bei Mehrfachaufruf mit der selben Instanz
            locatedPoint = null;
            maxMaxValOfMatches = null;

            // Template in Graustufen laden
            Mat template = Cv2.ImRead(templateFilePath, ImreadModes.Grayscale);

            // Ermittelt die Anzahl der Monitore und erstellt für jeden Monitor einen Screenshot
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RectX lprcMonitor, IntPtr dwData) =>
            {
                using (Mat screenshot = CaptureMonitor(lprcMonitor)) // Erstelle einen Screenshot für jeden Monitor
                {

#if DEBUG
                    // Speichern des Screenshots pro Monitor
                    string filePath = Path.Combine(resultFilePath, $"screenshot{hMonitor}.png");
                    screenshot.SaveImage(filePath);
#endif

                    // Konvertiere den Screenshot in Graustufen
                    Cv2.CvtColor(screenshot, screenshot, ColorConversionCodes.BGRA2GRAY);

#if DEBUG
                    // Drucke Typ pro Bild aus
                    Console.WriteLine("Template Type: " + template.Type());
                    Console.WriteLine("Screenshot Type: " + screenshot.Type());
#endif

                    using (Mat result = new Mat()) // Template Matching durchführen
                    {
                        Cv2.MatchTemplate(screenshot, template, result, TemplateMatchModes.CCoeffNormed);

                        // MinMaxLoc finden
                        Cv2.MinMaxLoc(result, out double minVal, out double maxVal, out OpenCvSharp.Point minLoc, out OpenCvSharp.Point maxLoc);

                        // Falls Match gefunden wird
                        if (maxVal > 0.6)
                        {
#if DEBUG
                            Console.WriteLine($"Match gefunden mit Wert {maxVal}");
#endif

                            if (maxMaxValOfMatches == null || maxVal > maxMaxValOfMatches)
                            {
                                maxMaxValOfMatches = maxVal;
                                locatedPoint = new System.Drawing.Point(maxLoc.X + template.Width / 2, maxLoc.Y + template.Height / 2);
                            }

                            // Rechteck um den gefundenen Bereich zeichnen
                            Rect match = new Rect(maxLoc, new OpenCvSharp.Size(template.Width, template.Height));
                            Cv2.Rectangle(screenshot, match, new Scalar(0, 0, 255), 3); // Rotes Rechteck

                            // Speichern des Ergebnisses
                            Cv2.ImWrite(resultFilePath.Replace(".png", $"_monitor_{hMonitor}.png"), screenshot);
                        }
                        else
                        {
#if DEBUG
                            Console.WriteLine("Kein Match gefunden.");
#endif
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);

            return locatedPoint;
        }

        public Mat CaptureMonitor(RectX monitorBounds)
        {
            // Hole die Bildschirmkoordinaten für den angegebenen Monitor
            Rectangle bounds = new Rectangle(monitorBounds.Left, monitorBounds.Top, monitorBounds.Right - monitorBounds.Left, monitorBounds.Bottom - monitorBounds.Top);

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(bounds.Location, System.Drawing.Point.Empty, bounds.Size);
                }

                // Konvertiere Bitmap zu Mat
                Mat mat = BitmapConverter.ToMat(bitmap);
                return mat;
            }
        }

        // P/Invoke: Definiert den RECT und die EnumDisplayMonitors-Funktion
        [StructLayout(LayoutKind.Sequential)]
        public struct RectX
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RectX lprcMonitor, IntPtr dwData);


        public void SearchAndClick(string templateFilePath, string resultFilePath)
        {
            System.Drawing.Point? clickPoint = SearchTemplateOnEachMonitor(templateFilePath, resultFilePath);
            if (clickPoint.HasValue)
            {
                System.Drawing.Point clickablePoint = clickPoint.Value;
                Mouse.Click(clickablePoint);
            }
        }
    }
}
