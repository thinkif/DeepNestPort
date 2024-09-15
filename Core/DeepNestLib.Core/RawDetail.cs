using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace DeepNestLib
{
    public class RawDetail
    {
        #region Jeffrey

        /// <summary>
        /// 是否允许旋转
        /// </summary>
        public bool allowRotate { get; set; } = true;

        /// <summary>
        /// 是否包含搭接量, 如果包含搭接量, 则必须排在画面最左侧
        /// </summary>
        public bool isIncludeOverlap { get; set; }

        #endregion

        public List<LocalContour> Outers = new List<LocalContour>();
        public List<LocalContour> Holes = new List<LocalContour>();
        public object Tag;
        public RectangleF BoundingBox()
        {
            GraphicsPath gp = new GraphicsPath();
            foreach (var item in Outers)
            {
                gp.AddPolygon(item.Points.ToArray());
            }
            return gp.GetBounds();
        }

        public string Name { get; set; }

        public NFP ToNfp()
        {

            NFP po = null;
            List<NFP> nfps = new List<NFP>();
            foreach (var item in Outers)
            {
                var nn = new NFP();
                nfps.Add(nn);
                foreach (var pitem in item.Points)
                {
                    nn.AddPoint(new SvgPoint(pitem.X, pitem.Y));
                }
                foreach (var ch in item.Childrens)
                {
                    nn = new NFP();
                    nfps.Add(nn);
                    foreach (var pitem in ch.Points)
                    {
                        nn.AddPoint(new SvgPoint(pitem.X, pitem.Y));
                    }
                }
            }

            if (nfps.Any())
            {
                var tt = nfps.OrderByDescending(z => z.Area).First();
                po = tt;
                po.Name = Name;

                foreach (var r in nfps)
                {
                    if (r == tt) 
                        continue;

                    if (po.children == null)
                    {
                        po.children = new List<NFP>();
                    }
                    po.children.Add(r);
                }



            }

            po.allowRotate = this.allowRotate;
            po.isIncludeOverlap = this.isIncludeOverlap;

            return po;
        }

        public void Scale(double v)
        {
            foreach (var item in Outers)
            {
                item.Scale(v);
            }
            foreach (var item in Holes)
            {
                item.Scale(v);
            }
        }
    }
}