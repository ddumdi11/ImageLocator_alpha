using ImageLocator_alpha;
using System.Drawing;

Console.WriteLine("Hello, World!");


string templateFilePath = "{Pfad zur Template-Datei}"; // Image-Datei im png-Format
string resultFilePath = "{Pfad zur Resultat-Datei}"; // Image-Datei im png-Format



LocateElementByPictureInDesktop locateElementByPictureInDesktop = new LocateElementByPictureInDesktop();
Point LocatedPoint = locateElementByPictureInDesktop.SearchTemplateOnEachMonitor(templateFilePath, resultFilePath);
Console.WriteLine("X = " + LocatedPoint.X);
Console.WriteLine("Y = " + LocatedPoint.Y);

