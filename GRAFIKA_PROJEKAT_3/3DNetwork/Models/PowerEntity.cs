using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DNetwork.Models
{
    public class PowerEntity
    {
        private long id;
        private string name;
        private double x;
        private double y;
        private double mapX;
        private double mapY;
        private double mapZ;
        private int matrixRow;
        private int matrixColumn;
        private int connections;

        public PowerEntity()
        {

        }

        public long Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }
        public double MapX
        {
            get
            {
                return mapX;
            }

            set
            {
                mapX = value;
            }
        }
        public double MapY
        {
            get
            {
                return mapY;
            }

            set
            {
                mapY = value;
            }
        }
        public double MapZ
        {
            get
            {
                return mapZ;
            }

            set
            {
                mapZ = value;
            }
        }
        public int MatrixRow
        {
            get
            {
                return matrixRow;
            }

            set
            {
                matrixRow = value;
            }
        }
        public int MatrixColumn
        {
            get
            {
                return matrixColumn;
            }

            set
            {
               matrixColumn = value;
            }
        }
        public int Connections
        {
            get
            {
                return connections;
            }

            set
            {
                connections = value;
            }
        }
    }
}
