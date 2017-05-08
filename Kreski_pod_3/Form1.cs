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
using System.Numerics;
namespace Kreski_pod_3
{
    public partial class Form1 : Form
    {
        Graphics pg;
        Segment end;
        Segment start;
        Vector2 anchor;
       
        int vbef = 0;
        List<TrackBar> TrList = new List<TrackBar>();
        List<Segment> Lsegment = new List<Segment>();
        public Form1()
        {
            
            InitializeComponent();
            InitalizeArm();
            Num1.Value = 3;
            Num1.Minimum = 2;
            Num1.Maximum = 6;
        }

        private void InitalizeArm()
        {
            start = new Segment(300, 200, 100);
            Segment current = start;
           
            Lsegment.Add(start);
            for (int i = 0; i < Num1.Value; i++)
            {
                Segment next = new Segment(current, 100);
                Lsegment.Add(next);
                current.Child = next;
                current = next;
// ************************************************************************************************************************
                TrList.Add(new TrackBar());
              
                TrList[i].Location = new System.Drawing.Point(trackBar2.Location.X, trackBar2.Location.Y + 100*i);
                TrList[i].Size = trackBar2.Size;
                TrList[i].Parent = this.ParentForm;
                TrList[i].Visible = true;
           //   TrList[i].ValueChanged += TrackBar_ValueChanged;//new System.EventHandler(TrackBar_ValueChanged);
                TrList[i].Tag = i;
                TrList[i].Maximum = trackBar2.Maximum;
                this.Controls.Add(TrList[i]);
    // *******************************************************************************************************************
            }
            end = current;
            anchor = new Vector2(this.panel2.ClientRectangle.Width / 3, this.panel2.ClientRectangle.Height / 3);

        }

        private void TrackBar_ValueChanged(object sender, EventArgs e)
        {
            //TO DO: kod obsługujący ręczną obsługę suwaka  

            var trackBar = (TrackBar)sender;
           // if (trackBar.Value > vbef)
        //    {
                Lsegment[(int)trackBar.Tag].Angle += (float)(Math.PI / 180);
            Lsegment[(int)trackBar.Tag].calculateB();
           
            //   end.angle += (float)(Math.PI / 180); // dodawanie 1 stopnia
            panel2.Invalidate();
                panel2.Update();
       //     }
     
        }

     private void setTrackBars()
        {
        for(int i =0;i< TrList.Count;i++)
            {
                TrList[i].Value = (int)(Lsegment[i].Angle * 180/Math.PI);
               
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            pg = e.Graphics;
            base.OnPaint(e);

            end.calculateB();
            try
            {
                Segment next = end.Parent;

                while (next != null)
                {
                    next.follow();
                    next.calculateB();
                    next = next.Parent;
                }

                start.A = new Vector2(anchor.X, anchor.Y);
                start.calculateB();
                next = start.Child;

                while (next != null)
                {
                    next.attachA();
                    next.calculateB();
                    next = next.Child;
                }

                end.Line(pg);

                next = end.Parent;

                while (next != null)
                {
                    next.Line(pg);
                    next = next.Parent;

                }
                
                setTrackBars(); //*
                
            }   


         
            catch { }
        }

   



        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (trackBar2.Value > vbef)
            {

                end.Angle += (float)(Math.PI / 180); // dodawanie 1 stopnia w radianach
                panel2.Invalidate();
                panel2.Update();
            }
            else if (trackBar2.Value < vbef)
            {

                end.Angle -= (float)(Math.PI / 180);
                panel2.Invalidate();
                panel2.Update();

            }
            
            vbef = trackBar2.Value; 

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
           
            InitalizeArm();
            panel2.Invalidate();
        }

   
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            end.follow((float)e.X, (float)e.Y); // podążanie za myszką 
            panel2.Invalidate();
            trackBar2.Value = (int)(end.Angle * (180 / Math.PI));
            setTrackBars();
        }
    }
}
