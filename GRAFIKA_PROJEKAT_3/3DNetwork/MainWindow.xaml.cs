using _3DNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace _3DNetwork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double noviX, noviY;
        public double minX, maxX, minY, maxY;
        public Dictionary<long, Tuple<string, object>> dictionaryNodes { get; set; }
        public Dictionary<long, LineEntity> dictionaryLines { get; set; }
        public long[,] Matrix { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            dictionaryNodes = new Dictionary<long, Tuple<string, object>>();
            dictionaryLines = new Dictionary<long, LineEntity>();
            Matrix = new long[300, 200];
           
            LoadXml();
            FindMinMax(out minX, out maxX, out minY, out maxY);
            
        }
        private void LoadXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            XmlNodeList nodeList;

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            foreach (XmlNode node in nodeList)
            {
                SubstationEntity subEnt = new SubstationEntity();
                subEnt.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                subEnt.Name = node.SelectSingleNode("Name").InnerText;
                subEnt.X = double.Parse(node.SelectSingleNode("X").InnerText);
                subEnt.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                ToLatLon(subEnt.X, subEnt.Y, 34, out noviY, out noviX);
                subEnt.X = noviX;
                subEnt.Y = noviY;
                dictionaryNodes.Add(subEnt.Id, new Tuple<string, object>("substation", subEnt));

            }


            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            foreach (XmlNode node in nodeList)
            {
                NodeEntity nodeEnt = new NodeEntity();
                nodeEnt.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                nodeEnt.Name = node.SelectSingleNode("Name").InnerText;
                nodeEnt.X = double.Parse(node.SelectSingleNode("X").InnerText);
                nodeEnt.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                ToLatLon(nodeEnt.X, nodeEnt.Y, 34, out noviY, out noviX);
                nodeEnt.X = noviX;
                nodeEnt.Y = noviY;
                dictionaryNodes.Add(nodeEnt.Id, new Tuple<string, object>("node", nodeEnt));

            }


            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode node in nodeList)
            {
                SwitchEntity switchEnt = new SwitchEntity();
                switchEnt.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                switchEnt.Name = node.SelectSingleNode("Name").InnerText;
                switchEnt.X = double.Parse(node.SelectSingleNode("X").InnerText);
                switchEnt.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                switchEnt.Status = node.SelectSingleNode("Status").InnerText;

                ToLatLon(switchEnt.X, switchEnt.Y, 34, out noviY, out noviX);
                switchEnt.X = noviX;
                switchEnt.Y = noviY;
                dictionaryNodes.Add(switchEnt.Id, new Tuple<string, object>("switch", switchEnt));


            }

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            foreach (XmlNode node in nodeList)
            {
                LineEntity lineEnt = new LineEntity();
                lineEnt.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                lineEnt.Name = node.SelectSingleNode("Name").InnerText;
                if (node.SelectSingleNode("IsUnderground").InnerText.Equals("true"))
                {
                    lineEnt.IsUnderground = true;
                }
                else
                {
                    lineEnt.IsUnderground = false;
                }
                lineEnt.R = float.Parse(node.SelectSingleNode("R").InnerText);
                lineEnt.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                lineEnt.LineType = node.SelectSingleNode("LineType").InnerText;
                lineEnt.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);
                lineEnt.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
                lineEnt.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);

                dictionaryLines.Add(lineEnt.Id, lineEnt);

            }

        }
        public void FindMinMax(out double minXlocal, out double maxXlocal, out double minYlocal, out double maxYlocal)
        {
            maxXlocal = 0;
            maxYlocal = 0;
            minXlocal = ((PowerEntity)dictionaryNodes.First().Value.Item2).X;
            minYlocal = ((PowerEntity)dictionaryNodes.First().Value.Item2).Y;
            foreach (var el in dictionaryNodes.Values)
            {
                if (((PowerEntity)el.Item2).Y > maxYlocal)
                {
                    maxYlocal = ((PowerEntity)el.Item2).Y;
                }
                if (((PowerEntity)el.Item2).X > maxXlocal)
                {
                    maxXlocal = ((PowerEntity)el.Item2).X;
                }
                if (((PowerEntity)el.Item2).Y < minYlocal)
                {
                    minYlocal = ((PowerEntity)el.Item2).Y;
                }
                if (((PowerEntity)el.Item2).X < minXlocal)
                {
                    minXlocal = ((PowerEntity)el.Item2).X;
                }

            }
        }
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }
    }
}
