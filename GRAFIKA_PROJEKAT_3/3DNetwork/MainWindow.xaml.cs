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
using System.Windows.Media.Media3D;
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
        private GeometryModel3D hitgeo;
        public Dictionary<long, Tuple<string, object>> dictionaryNodes { get; set; }
        public Dictionary<long, LineEntity> dictionaryLines { get; set; }
        public List<GeometryModel3D> geometryModels { get; set; }
        public List<GeometryModel3D> lineModels { get; set; }
        public long[,] Matrix { get; set; }
        public ToolTip tooltip = new ToolTip();
        public MainWindow()
        {
            InitializeComponent();
            dictionaryNodes = new Dictionary<long, Tuple<string, object>>();
            dictionaryLines = new Dictionary<long, LineEntity>();
            geometryModels = new List<GeometryModel3D>();
            lineModels = new List<GeometryModel3D>();
            Matrix = new long[200, 200];
           
            LoadXml();
            FindMinMax(out minX, out maxX, out minY, out maxY);
            DrawNodes();
            
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
        private void DrawNodes()
        {
            foreach (var el in dictionaryNodes.Values)
            {

                if (el.Item1.Equals("substation"))
                {
                    SubstationEntity subEntity = (SubstationEntity)el.Item2;
                    double x = 0;
                    double z = 0;
                    GeometryModel3D myGeometryModel = MakeCube(x, z);
                    
                    MyModel3DGroup.Children.Add(myGeometryModel);
                    
                    geometryModels.Add(myGeometryModel);

                    //ToolTip toolTip = new ToolTip();
                    //toolTip.Content = "Substation\nID: " + subEntity.Id + "  Name: " + subEntity.Name;
                    //toolTip.Background = Brushes.Black;
                    //toolTip.Foreground = Brushes.White;
                    //toolTip.Padding = new Thickness(10);

                }
                //if (el.Item1.Equals("switch"))
                //{
                //    SwitchEntity switchEntity = (SwitchEntity)el.Item2;
                //    Rectangle myRectangle = new Rectangle();
                //    myRectangle.Width = 3;
                //    myRectangle.Height = 3;
                //    myRectangle.StrokeThickness = 1;
                //    myRectangle.Stroke = Brushes.Red;
                //    myRectangle.Fill = Brushes.Red;

                //    ToolTip toolTip = new ToolTip();
                //    toolTip.Content = "Switch\nID: " + switchEntity.Id + "  Name: " + switchEntity.Name + "\nStatus: " + switchEntity.Status;
                //    toolTip.Background = Brushes.Black;
                //    toolTip.Foreground = Brushes.White;
                //    toolTip.Padding = new Thickness(10);

                //    myRectangle.ToolTip = toolTip;
                //    myRectangle.Name = "_" + switchEntity.Id.ToString();


                //    MyModel3DGroup.Children.Add(myRectangle);

                //    myRectangle.MouseLeftButtonDown += MouseLeftClickNode;

                //}
                //if (el.Item1.Equals("node"))
                //{
                //    NodeEntity nodeEntity = (NodeEntity)el.Item2;
                //    Rectangle myRectangle = new Rectangle();

                //    myRectangle.Width = 3;
                //    myRectangle.Height = 3;
                //    myRectangle.StrokeThickness = 1;
                //    myRectangle.Stroke = Brushes.Green;
                //    myRectangle.Fill = Brushes.Green;

                //    ToolTip toolTip = new ToolTip();
                //    toolTip.Content = "Node\nID: " + nodeEntity.Id + "  Name: " + nodeEntity.Name;
                //    toolTip.Background = Brushes.Black;
                //    toolTip.Foreground = Brushes.White;
                //    toolTip.Padding = new Thickness(10);

                //    myRectangle.ToolTip = toolTip;
                //    myRectangle.Name = "_" + nodeEntity.Id.ToString();

                //    MyModel3DGroup.Children.Add(myRectangle);

                //    myRectangle.MouseLeftButtonDown += MouseLeftClickNode;

                //}

            }
        }
        public GeometryModel3D MakeCube(double x, double z)
        {
            GeometryModel3D myGeometryModel = new GeometryModel3D();
            MeshGeometry3D myMeshGeometry3D = new MeshGeometry3D();

            Point3DCollection myPositionCollection = new Point3DCollection();
            myPositionCollection.Add(new Point3D(0, 0, 0));
            myPositionCollection.Add(new Point3D(0.01, 0, 0));
            myPositionCollection.Add(new Point3D(0, 0.01, 0));
            myPositionCollection.Add(new Point3D(0.01, 0.01, 0));
            myPositionCollection.Add(new Point3D(0, 0, 0.01));
            myPositionCollection.Add(new Point3D(0.01, 0, 0.01));
            myPositionCollection.Add(new Point3D(0, 0.01, 0.01));
            myPositionCollection.Add(new Point3D(0.01, 0.01, 0.01));

            myMeshGeometry3D.Positions = myPositionCollection;

            Int32Collection myTriangleIndicesCollection = new Int32Collection();
            Int32[] indices = { 2, 3, 1, 3, 1, 0, 7, 1, 3, 7, 5, 1, 6, 5, 7, 6, 4, 5, 6, 2, 0, 2, 0, 4, 2, 7, 3, 2, 6, 7, 0, 1, 5, 0, 5, 4 };
            foreach (var i in indices)
            {
                myTriangleIndicesCollection.Add(i);
            }
            myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;
            DiffuseMaterial myMaterial;
            int numberOfCon = GetNumberOfConnections();
            if (numberOfCon >= 0 && numberOfCon < 3)
            {
                myMaterial = new DiffuseMaterial() { Brush = Brushes.PaleVioletRed };
            }
            else if(numberOfCon>=3 && numberOfCon<=5)
            {
                myMaterial = new DiffuseMaterial() { Brush = Brushes.Red };
            }
            else
            {
                myMaterial = new DiffuseMaterial() { Brush = Brushes.DarkRed };
            }
            
            myGeometryModel.Material = myMaterial;
            myGeometryModel.Geometry = myMeshGeometry3D;


            return myGeometryModel;
        }

        private void MyViewport3D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point mouseposition = e.GetPosition(MyViewport3D);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

            PointHitTestParameters pointparams =
                     new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams =
                     new RayHitTestParameters(testpoint3D, testdirection);

            //test for a result in the Viewport3D     
            hitgeo = null;
            VisualTreeHelper.HitTest(MyViewport3D, null, HTResult, pointparams);
        }
        private HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {

            RayHitTestResult rayResult = rawresult as RayHitTestResult;
           
            if (rayResult != null)
            {

                DiffuseMaterial newColor =
                     new DiffuseMaterial(new SolidColorBrush(
                     System.Windows.Media.Colors.Blue));

                bool gasit = false;
                for (int i = 0; i < lineModels.Count; i++)
                {
                    if (lineModels[i] == rayResult.ModelHit)
                    {
                        hitgeo = (GeometryModel3D)rayResult.ModelHit;
                        gasit = true;
                        hitgeo.Material = newColor;
                        
                    }
                    else
                    {
                        lineModels[i].Material = newColor;
                    }
                }
                if (!gasit)
                {
                    hitgeo = null;
                }
            }

            return HitTestResultBehavior.Stop;
        }
        private HitTestResultBehavior HTResult2(System.Windows.Media.HitTestResult rawresult)
        {

            RayHitTestResult rayResult = rawresult as RayHitTestResult;
            
            tooltip.IsOpen = false;
            if (rayResult != null)
            {

                bool gasit = false;
                for (int i = 0; i < geometryModels.Count; i++)
                {
                    if (geometryModels[i] == rayResult.ModelHit)
                    {
                        tooltip.IsOpen = true;
                        tooltip.Content = "bla";
                        tooltip.Background = Brushes.Black;
                        ToolTipService.SetShowDuration(tooltip, 1000);
                       
                    }
                    else
                    {
                        tooltip.IsOpen = false;
                    }
                }
                if (!gasit)
                {
                    hitgeo = null;
                }
            }

            return HitTestResultBehavior.Stop;
        }

        private void MyViewport3D_MouseEnter(object sender, MouseEventArgs e)
        {
            System.Windows.Point mouseposition = e.GetPosition(MyViewport3D);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

            PointHitTestParameters pointparams =
                     new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams =
                     new RayHitTestParameters(testpoint3D, testdirection);

            //test for a result in the Viewport3D     
            hitgeo = null;
            VisualTreeHelper.HitTest(MyViewport3D, null, HTResult2, pointparams);
        }

        private void MyViewport3D_MouseLeave(object sender, MouseEventArgs e)
        {
            tooltip.IsOpen = false;
        }

        public int GetNumberOfConnections()
        {
            return 0;
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

        public bool IsInRange(double latitude, double longitude)
        {
            if(latitude > 45.2325 && latitude < 45.277031 &&  longitude > 19.793909 && longitude < 19.894459)
            {
                return true;
            }
            return false;
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
