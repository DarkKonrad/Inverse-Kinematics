using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Collections;
using System.Numerics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Kreski_pod_3
{
    [DataContractAttribute()]
    public class Segment
    {
        [DataMember()]
        Vector2 a;                  // a określa początek segmentu, b jego koniec --> prosta AB 
        [DataMember()]
        private float angle = 0;     // Kąt pod którym jest wyrysowywany segment od początku układu współrzędnych 
        [DataMember()]
        float len;                  // długość segmentu 
        [DataMember()]
        private Vector2 b = new Vector2();
        [DataMember()]
        Segment parent = null;      // Poprzedni segment 
        [DataMember()]
        Segment child = null;       // kolejny segment
        float last = 0;
        float ang;

        Graphics pg;
        /* GETery oraz SETery */

        public Segment Child { get => child; set => child = value; }
        public Segment Parent { get => parent; set => parent = value; }
        public Vector2 A { get => a; set => a = value; }
        public float Angle { get => angle; set => angle = value; }
        public Vector2 B { get => b; set => b = value; }


        /* Konstruktory */

        public Segment(float x, float y, float len_, Graphics p)
        {
            A = new Vector2(x, y);
            len = len_;
            pg = p;
            calculateB(pg);
        }

        public Segment(Segment parent_, float len_, Graphics p)
        {
            Parent = parent_;
            A = new Vector2(Parent.b.X, Parent.b.Y);
            len = len_;
            pg = p;
            calculateB(pg);
        }
        Segment() { a = new Vector2(); b = new Vector2(); child = null; parent = null; angle = 0; len = 0; }


        /* Metody*/

        public void follow(float tx, float ty) // Parametryczna wersja follow, na podstawie dostarczonych współrzędnych 
        {

            Vector2 target = new Vector2(tx, ty);
            Vector2 dir = Vector2.Subtract(target, A);


            Angle = (float)Math.Atan2(dir.Y, dir.X);
            IsAngleOk();
            //  iAtan();
            dir = setMag(len, dir);

            dir = Vector2.Multiply(-1, dir);
            a = Vector2.Add(target, dir);


        }

        void iAtan()
        {
            if (child != null)
            {
                this.ang = (float)Math.Atan2(b.Y - a.Y, b.X - a.X) - (float)Math.Atan2(child.b.Y - child.a.Y, child.b.X - child.a.X);
                Debug.WriteLine(Math.Abs(RadianToDegree(ang)) + "\n");

            }
        }

        void IsAngleOk()  //  sprawdzenie kąta 
        {

            if (child != null)
            {
                pg.TranslateTransform(a.X, a.Y);                                                                   
                Vector2 ap = Vector2.Subtract(a, b);
                Vector2 bp = Vector2.Subtract(child.A, child.B);
               
                ang = angleBetween(ap, bp);
                pg.TranslateTransform(-a.X, -a.Y);

                Debug.WriteLine(ang);

                //float apLen = ap.Length();
                //float bpLen = bp.Length();
                //float preangle = ap.X * bp.X + ap.Y * bp.Y;
                // ang = (float)Math.Acos(preangle / (apLen * bpLen)); // w radianach 

                //Debug.WriteLine((RadianToDegree(ang)));
                //if (RadianToDegree(ang) >= 160F)
                //{
                //    Debug.WriteLine("warunek spełniono");
                //    // Angle = (float)DegreeToRadian(160);


                //}




                last = angle;
                //     Debug.WriteLine((RadianToDegree(ang) - 180F) * (-1) + "\n");



            }






        }


        void setA(Vector2 pos)
        {
            A = new Vector2(pos.X, pos.Y);
        }

        public void attachA() // Przyłącza jako punkt A, punkt B rodzica. Czyli Punkt A kolejnego(dziecka/child) segmentu jest punktem B obecnego (rodzica/parent). 
        {
            setA(Parent.b);
        }

        public void follow()             //Określa zachowanie poszczególnych segmentów.               
        {                                  //Pobiera wspołrzędne początku kolejnego segmentu (child) i wysyła je do parametrycznej wersji metody
            float targetX = Child.A.X;
            float targetY = Child.A.Y;
            follow(targetX, targetY);

        }

        public void Line(Graphics pg) // zamiań na pen w głównej klasie 
        {


            Pen p = new Pen(Color.Red, 3);
            pg.DrawLine(p, A.X, A.Y, b.X, b.Y);

        }

        public void calculateB(Graphics pg)  // Wylicza współrzędnego puntu B (końca) segmentu na podstawie cosinusa (współ. X ) lub sinusa (współ. Y) przemnożonego przez dolecową długość 
        {

            //    Angle = (float)DegreeToRadian(cos);

            if (Math.Abs(ang) >= 160F)
            {
                pg.TranslateTransform(a.X, a.Y);
                
                //float dx = (float)(Math.Cos(ang + DegreeToRadian(-160)) * len);
                //float dy = (float)(Math.Sin(ang + DegreeToRadian(-160)) * len);
                float dx = (float)(Math.Cos(DegreeToRadian(160)) * len);
                float dy = (float)(Math.Sin(DegreeToRadian(160)) * len);
                pg.TranslateTransform(-a.X, -a.Y);
                b = new Vector2(a.X + dx, a.Y + dy);
                
            }

            else
            {
                pg.TranslateTransform(a.X, a.Y);
                float dx = (float)(Math.Cos(Angle) * len);
                float dy = (float)(Math.Sin(Angle) * len);
                b = new Vector2(a.X + dx, a.Y + dy);
                pg.TranslateTransform(-a.X, -a.Y);

            }


                //while ((float)Math.Abs(RadianToDegree(ang)) >= 160F)
                //   {

                //       float xd = (float)Math.Cos(DegreeToRadian(20) * len);
                //       float yd = (float)Math.Sin(DegreeToRadian(20) * len);
                //       b = new Vector2(a.X + xd, a.Y + yd);
                //       Application.DoEvents();
                //   }
                //   if ((float)Math.Abs(RadianToDegree(ang)) < 160F)
                //  {

                // }




                //else
                //{
                //    float dx = (float)(Math.Cos(ang) * len);
                //    float dy = (float)(Math.Sin(ang) * len);
                //    b = new Vector2(a.X + dx, a.Y + dy);

                //}




                //  Debug.WriteLine("\n"+"ANGLE--> "+RadianToDegree(angle));


                //  checkAngle();







            }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }



        private void Mag(float i, ref Vector2 v) // Ustawia długość wektora. Nie pomylić z długością segmentu.
        {
            v = Vector2.Normalize(v);
            v = Vector2.Multiply(v, i);
        }
        private Vector2 setMag(float i, Vector2 v)
        {

            v = Vector2.Normalize(v);
            v = Vector2.Multiply(v, i);    // achtung 
            return v;
        }

        private float angleBetween(Vector2 a, Vector2 b)
        {
            System.Windows.Vector vector1 = new System.Windows.Vector(a.X, a.Y);
            System.Windows.Vector vector2 = new System.Windows.Vector(b.X, b.Y);
            float angleBetween;

            angleBetween = (float)System.Windows.Vector.AngleBetween(vector1, vector2);

            return angleBetween;

        }

    }
}
