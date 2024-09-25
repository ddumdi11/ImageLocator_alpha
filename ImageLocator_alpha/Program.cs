using ImageLocator_alpha;
using System.Drawing;
using System.IO;

Console.WriteLine("Hello, World!");

// Basisverzeichnis des ausgeführten Programms ermitteln
string baseDirectory = AppContext.BaseDirectory;


// Baue relative Pfade auf
string templateFilePath = Path.Combine(baseDirectory, "Resources", "Pics2Find", "eva_angebot_vertrag_ueberblick.png");
string resultFolderPath = Path.Combine(baseDirectory, "Resources", "Results");

// Pfadüberprüfung
if (!File.Exists(templateFilePath))
{
    Console.WriteLine("Template-Datei existiert nicht: " + templateFilePath);
    return;
}

if (!Directory.Exists(Path.Combine(baseDirectory, "Resources", "Results")))
{
    Console.WriteLine("Ergebnisverzeichnis existiert nicht.");
    Directory.CreateDirectory(Path.Combine(baseDirectory, "Resources", "Results"));
}


LocateElementByPictureInDesktop locateElementByPictureInDesktop = new LocateElementByPictureInDesktop();
Point? locatedPoint = locateElementByPictureInDesktop.SearchTemplateOnEachMonitor(templateFilePath, resultFolderPath);

if (locatedPoint.HasValue)
{
    Console.WriteLine("Gefundenes Template mit den Koordinaten:");
    Console.WriteLine("X = " + locatedPoint.Value.X);
    Console.WriteLine("Y = " + locatedPoint.Value.Y);
}

// Mouse-Click auf Template bei Lokalisierung
locateElementByPictureInDesktop.SearchAndClick(templateFilePath, resultFolderPath);
