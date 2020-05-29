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
        #region Fields
        public double noviX, noviY;
        public double minX = 19.793909;
        public double maxX = 19.894459;
        public double minY = 45.2325;
        public double maxY = 45.277031;
        private GeometryModel3D hitgeo;
        public Dictionary<long, Tuple<string, PowerEntity>> dictionaryNodes { get; set; }
        public Dictionary<long, LineEntity> dictionaryLines { get; set; }
        public Dictionary<long,GeometryModel3D> geometryModels { get; set; }
        public Dictionary<long,GeometryModel3D> lineModels { get; set; }
        public string[,] NodesMatrix { get; set; }
        public string[,] LinesMatrix { get; set; }
        public ToolTip tooltip = new ToolTip();
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            dictionaryNodes = new Dictionary<long, Tuple<string, PowerEntity>>();
            dictionaryLines = new Dictionary<long, LineEntity>();
            geometryModels = new Dictionary<long, GeometryModel3D>();
            lineModels = new Dictionary<long, GeometryModel3D>();
            NodesMatrix = new string[200, 200];
            LinesMatrix = new string[200, 200];
           
            LoadXml();
            FillMatrix();
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
                if (IsInRange(subEnt.Y, subEnt.X))
                {
                    dictionaryNodes.Add(subEnt.Id, new Tuple<string, PowerEntity>("substation", subEnt));

                }

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

                if (IsInRange(nodeEnt.Y, nodeEnt.X))
                {
                    dictionaryNodes.Add(nodeEnt.Id, new Tuple<string, PowerEntity>("node", nodeEnt));
                }

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

                if (IsInRange(switchEnt.Y, switchEnt.X))
                {
                    dictionaryNodes.Add(switchEnt.Id, new Tuple<string, PowerEntity>("switch", switchEnt));
                }


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
                lineEnt.Vertices = new List<Point>();
                foreach (XmlNode pointNode in node.ChildNodes[9].ChildNodes) // 9 posto je Vertices 9. node u jednom line objektu
                {
                    Point p = new Point();

                    p.X = double.Parse(pointNode.SelectSingleNode("X").InnerText);
                    p.Y = double.Parse(pointNode.SelectSingleNode("Y").InnerText);

                    ToLatLon(p.X, p.Y, 34, out noviX, out noviY);

                    lineEnt.Vertices.Add(new Point(noviX, noviY));
                }

                if (DictionaryContainsNode(lineEnt.FirstEnd) && DictionaryContainsNode(lineEnt.SecondEnd))
                {
                    if (!LineExists(lineEnt.FirstEnd, lineEnt.SecondEnd))
                    {
                        dictionaryLines.Add(lineEnt.Id, lineEnt);

                        dictionaryNodes[lineEnt.FirstEnd].Item2.Connections++;
                        dictionaryNodes[lineEnt.SecondEnd].Item2.Connections++;
                    }

                }

            }

        }

        #region 3D
        private void DrawLines()
        {
            foreach(var el in dictionaryLines.Values)
            {
                GeometryModel3D myGeometryModel = new GeometryModel3D();
                lineModels.Add(el.Id, myGeometryModel);
            }
        }
        private void DrawNodes()
        {
            foreach (var el in dictionaryNodes.Values)
            {

                if (el.Item1.Equals("substation"))
                {
                    SubstationEntity subEntity = (SubstationEntity)el.Item2;
                    
                    GeometryModel3D myGeometryModel = MakeCube(subEntity);
                    
                    MyModel3DGroup.Children.Add(myGeometryModel);
                    
                    geometryModels.Add(subEntity.Id,myGeometryModel);

                }
                if (el.Item1.Equals("switch"))
                {
                    SwitchEntity switchEntity = (SwitchEntity)el.Item2;
                   
                    GeometryModel3D myGeometryModel = MakeCube(switchEntity);

                    MyModel3DGroup.Children.Add(myGeometryModel);

                    geometryModels.Add(switchEntity.Id, myGeometryModel);

                }
                if (el.Item1.Equals("node"))
                {
                    NodeEntity nodeEntity = (NodeEntity)el.Item2;
                   
                    GeometryModel3D myGeometryModel = MakeCube(nodeEntity);

                    MyModel3DGroup.Children.Add(myGeometryModel);

                    geometryModels.Add(nodeEntity.Id, myGeometryModel);

                }

            }
        }
        public GeometryModel3D MakeCube(PowerEntity entity)
        {
            double x = entity.MapX;
            double y = entity.MapY;
            double z = entity.MapZ;

            GeometryModel3D myGeometryModel = new GeometryModel3D();
            MeshGeometry3D myMeshGeometry3D = new MeshGeometry3D();

            Point3DCollection myPositionCollection = new Point3DCollection();
           
            myPositionCollection.Add(new Point3D(0 + x, 0 + y, 0 + z));
            myPositionCollection.Add(new Point3D(0.01 + x, 0 + y, 0 + z));
            myPositionCollection.Add(new Point3D(0 + x, 0.01 + y, 0 + z));
            myPositionCollection.Add(new Point3D(0.01 + x, 0.01 + y, 0 + z));
            myPositionCollection.Add(new Point3D(0 + x, 0 + y, 0.01 + z));
            myPositionCollection.Add(new Point3D(0.01 + x, 0 + y, 0.01 + z));
            myPositionCollection.Add(new Point3D(0 + x, 0.01 + y, 0.01 + z));
            myPositionCollection.Add(new Point3D(0.01 + x, 0.01 + y, 0.01 + z));

            myMeshGeometry3D.Positions = myPositionCollection;

            Int32Collection myTriangleIndicesCollection = new Int32Collection();
            Int32[] indices = { 2, 3, 1, 3, 1, 0, 7, 1, 3, 7, 5, 1, 6, 5, 7, 6, 4, 5, 6, 2, 0, 2, 0, 4, 2, 7, 3, 2, 6, 7, 0, 1, 5, 0, 5, 4 };
            foreach (var i in indices)
            {
                myTriangleIndicesCollection.Add(i);
            }
            myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;
            DiffuseMaterial myMaterial;
            if (entity.Connections >= 0 && entity.Connections < 3)
            {
                myMaterial = new DiffuseMaterial() { Brush = (Brush)(new BrushConverter().ConvertFrom("#FF994343"))};
            }
            else if(entity.Connections >= 3 && entity.Connections <= 5)
            {
                myMaterial = new DiffuseMaterial() { Brush = (Brush)(new BrushConverter().ConvertFrom("#FF931B1B")) };
            }
            else
            {
                myMaterial = new DiffuseMaterial() { Brush = (Brush)(new BrushConverter().ConvertFrom("#FF4F0505")) };
            }

            myGeometryModel.Material = myMaterial;
            myGeometryModel.Geometry = myMeshGeometry3D;


            return myGeometryModel;
        }
        #endregion

        #region MouseEvents
        private HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {

            RayHitTestResult rayResult = rawresult as RayHitTestResult;
           
            if (rayResult != null)
            {

                DiffuseMaterial newColor =
                     new DiffuseMaterial(new SolidColorBrush(
                     System.Windows.Media.Colors.Green));
            
                bool gasit = false;
                foreach (long key in lineModels.Keys)
                {
                    if (lineModels[key] == (GeometryModel3D)rayResult.ModelHit)
                    {
                        LineEntity lineEntity = dictionaryLines[key];

                        GeometryModel3D node1 = geometryModels[lineEntity.FirstEnd];
                        GeometryModel3D node2 = geometryModels[lineEntity.SecondEnd];

                        node1.Material = newColor;
                        node2.Material = newColor;

                        hitgeo = (GeometryModel3D)rayResult.ModelHit;
                        gasit = true;
                       // hitgeo.Material = newColor;
                        
                    }
                    else
                    {
                       // lineModels[key].Material = newColor;
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

            //tooltip.IsOpen = false;
           
            if (rayResult != null)
            {

                bool gasit = false;
                foreach (long key in geometryModels.Keys)
                {
                    if (geometryModels[key] == (GeometryModel3D)rayResult.ModelHit)
                    {
                         Tuple<string, PowerEntity> powerEntity = dictionaryNodes[key];

                         if (powerEntity.Item1 == "switch")
                            tooltip.Content = "Switch\nID: " + powerEntity.Item2.Id + "  Name: " + powerEntity.Item2.Name + "\nStatus: " + ((SwitchEntity)powerEntity.Item2).Status;
                         else if (powerEntity.Item1 == "node")
                            tooltip.Content = "Node\nID: " + powerEntity.Item2.Id + "  Name: " + powerEntity.Item2.Name;
                         else if (powerEntity.Item1 == "substation")
                            tooltip.Content = "Substation\nID: " + powerEntity.Item2.Id + "  Name: " + powerEntity.Item2.Name;
                        
                         tooltip.IsOpen = true;
                         tooltip.Background = Brushes.Black;
                         tooltip.Foreground = Brushes.White;
                         tooltip.Padding = new Thickness(10);
                        ToolTipService.SetShowDuration(tooltip, 1000);
                        gasit = true;
                    }
                }
                if (!gasit)
                {
                    tooltip.IsOpen = false;
                    hitgeo = null;
                }
            }

            return HitTestResultBehavior.Stop;
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

            hitgeo = null;
            VisualTreeHelper.HitTest(MyViewport3D, null, HTResult, pointparams);
        }
        private void MyViewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point mouseposition = e.GetPosition(MyViewport3D);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

            PointHitTestParameters pointparams =
                     new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams =
                     new RayHitTestParameters(testpoint3D, testdirection);

            hitgeo = null;
            VisualTreeHelper.HitTest(MyViewport3D, null, HTResult2, pointparams);
        }
        private void MyViewport3D_MouseLeave(object sender, MouseEventArgs e)
        {
            tooltip.IsOpen = false;
       
        }
        #endregion

        #region Matrix
       
        public void FillMatrix()
        {
            foreach (var el in dictionaryNodes.Values)
            {
                PowerEntity powEntity;
                if (el.Item1.Equals("substation"))
                {
                    powEntity = (SubstationEntity)el.Item2;

                }
                else if (el.Item1.Equals("switch"))
                {
                    powEntity = (SwitchEntity)el.Item2;

                }
                else
                {
                    powEntity = (NodeEntity)el.Item2;
                }

                int x = Scale(powEntity.X, minX, maxX);
                int z = Scale(powEntity.Y, minY, maxY);
                int row = 199 - z;
                int column = x;
                double yDifference = 0;

                if (NodesMatrix[row, column] != null)
                {
                    string s = NodesMatrix[row, column];
                    string[] array = s.Split('_');
                    int nodesNum = array.Count() -1;
                    yDifference += nodesNum* 0.01;
                }
                NodesMatrix[row, column] += "_"+ powEntity.Id.ToString();
                powEntity.MatrixRow = row;
                powEntity.MatrixColumn = column;

                powEntity.MapX = x * 0.01;
                powEntity.MapZ = 1.99 - z * 0.01;
                powEntity.MapY = yDifference;

            }
        }
        #endregion

        #region Helper
        private bool LineExists(long firstNodeId, long secondNodeId)
        {
            foreach (var line in dictionaryLines.Values)
            {
                if ((line.FirstEnd == firstNodeId && line.SecondEnd == secondNodeId) || (line.SecondEnd == firstNodeId && line.FirstEnd == secondNodeId))
                {
                    return true;
                }
            }
            return false;
        }
        private bool DictionaryContainsNode(long nodeId)
        {
            return dictionaryNodes.ContainsKey(nodeId);

        }
        private int Scale(double value, double min, double max)
        {
            return (int)((double)(value - min) / (max - min) * 199);
        }
        public bool IsInRange(double latitude, double longitude)
        {
            if (latitude > minY && latitude < maxY && longitude > minX && longitude < maxX)
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
        #endregion
    }
}
