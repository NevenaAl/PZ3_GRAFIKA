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
        private int canvasX;
        private int canvasY;
        private int matrixRow;
        private int matrixColumn;

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
        public int CanvasX
        {
            get
            {
                return canvasX;
            }

            set
            {
                canvasX = value;
            }
        }
        public int CanvasY
        {
            get
            {
                return canvasY;
            }

            set
            {
                canvasY = value;
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
    }
}
