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

namespace Kreski_pod_3
{
   public class Segment
    {
        Vector2 a;                  // a określa początek segmentu, b jego koniec --> prosta AB 
        private float angle = 0;     // Kąt pod którym jest wyrysowywany segment
        float len;                  // długość segmentu 
        public Vector2 b = new Vector2(); 
        Segment parent = null;      // Poprzedni segment 
        Segment child = null;       // kolejny segment
        

                    /* GETery oraz SETery */
        public Segment Child { get => child; set => child = value; }
        public Segment Parent { get => parent; set => parent = value; }
        public Vector2 A { get => a; set => a = value; }
        public float Angle { get => angle; set => angle = value; }


        /* Konstruktory */

        public Segment(float x, float y, float len_)
        {
            A = new Vector2(x, y);
            len = len_;
            calculateB();
        }

        public Segment (Segment parent_,float len_)
        {
            Parent = parent_;
            A = new Vector2(Parent.b.X, Parent.b.Y);
            len = len_;
            calculateB();
        }

                /* Metody*/

        public  void follow(float tx,float ty) // Parametryczna wersja follow, na podstawie dostarczonych współrzędnych 
        {
            Vector2 target = new Vector2(tx, ty);
            Vector2 dir = Vector2.Subtract(target, A);
            Angle = (float)Math.Atan2(dir.Y, dir.X);
            checkAngle();                       // *
            dir = setMag(len, dir);
          
            dir = Vector2.Multiply(-1, dir);
            a = Vector2.Add(target, dir);

        }

         void checkAngle()  //  sprawdzenie kąta 
        {
            if (this.Angle >= 2.9F) // arbitralna wartość w radianach użyta w celach testowych 
            {
               this.Angle = 2.9F;
                calculateB();
            }
            if(this.Angle <=0)
            {
                this.Angle =0;
                calculateB();
            }
            //2,56
        }


        void setA(Vector2 pos)
        {
            A = new Vector2(pos.X,pos.Y);
        }

        public  void attachA() // Przyłącza jako punkt A, punkt B rodzica. Czyli Punkt A kolejnego(dziecka/child) segmentu jest punktem B obecnego (rodzica/parent). 
        {
            setA(Parent.b);
        }

        public    void follow()             //Określa zachowanie poszczególnych segmentów.               
        {                                  //Pobiera wspołrzędne początku kolejnego segmentu (child) i wysyła je do parametrycznej wersji metody
            float targetX = Child.A.X;
            float targetY = Child.A.Y;
            follow(targetX, targetY);

        }

        public void Line (Graphics pg) // zamiań na pen w głównej klasie 
        {
            Pen p = new Pen(Color.Red, 3);
            pg.DrawLine(p, A.X, A.Y, b.X, b.Y);
        }

        public   void calculateB()  // Wylicza współrzędnego puntu B (końca) segmentu na podstawie cosinusa (współ. X ) lub sinusa (współ. Y) przemnożonego przez dolecową długość 
        {
            
            float dx = (float) (Math.Cos(Angle) * len); 
            float dy = (float) (Math.Sin(Angle) * len);
            b = new Vector2(a.X + dx, a.Y + dy);
        }

        private void Mag(float i,ref Vector2 v) // Ustawia długość wektora. Nie pomylić z długością segmentu.
        {
            v=Vector2.Normalize(v);
            v = Vector2.Multiply(v, i);
        }
        private Vector2 setMag(float i,Vector2 v)
        {

            v = Vector2.Normalize(v);
            v = Vector2.Multiply(v, i);    // achtung 
            return v;
        }
    }
}
