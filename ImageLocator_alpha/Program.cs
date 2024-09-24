using ImageLocator_alpha;
using System.Drawing;

Console.WriteLine("Hello, World!");


string templateFilePath = "C:\\Users\\diede\\source\\repos\\ImageLocator_prealpha\\ImageLocator_prealpha\\Resources\\Pics2Find\\vlc_player.png";
string resultFilePath = "C:\\Users\\diede\\source\\repos\\ImageLocator_prealpha\\ImageLocator_prealpha\\Resources\\Results\\result_desktop.png";



LocateElementByPictureInDesktop locateElementByPictureInDesktop = new LocateElementByPictureInDesktop();
Point LocatedPoint = locateElementByPictureInDesktop.SearchTemplateOnEachMonitor(templateFilePath, resultFilePath);
Console.WriteLine("X = " + LocatedPoint.X);
Console.WriteLine("Y = " + LocatedPoint.Y);

