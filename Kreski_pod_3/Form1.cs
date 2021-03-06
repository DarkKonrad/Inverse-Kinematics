﻿using System;
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
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Windows.Media;

namespace Kreski_pod_3
{
    public partial class Form1 : Form
    {
        Graphics pg;
        Segment end;
        Segment start,next,current;
        Vector2 anchor;
        bool loading=false;
        DataContractSerializer serial;
        DataContractSerializerSettings settings = new DataContractSerializerSettings();

        int vbef = 0;
        List<TrackBar> TrList = new List<TrackBar>();
        List<Segment> Lsegment = new List<Segment>();
        public Form1()
        {


            InitializeComponent();
            pg = panel2.CreateGraphics();
            InitalizeArm();
            Num1.Value = 3;
            Num1.Minimum = 0;
            Num1.Maximum = 6;
       
        }

        private void InitalizeArm()
        {
            Lsegment.Clear();
            Lsegment.TrimExcess();
          
          
            start = new Segment(300, 200, 100,pg);
             current = start;
           
            Lsegment.Add(start);
            for (int i = 0; i < Num1.Value; i++)
            {
                 next = new Segment(current, 100,pg);
                Lsegment.Add(next);
                current.Child = next;
                current = next;

// ************************************************************************************************************************
           //     TrList.Add(new TrackBar());             
           //     TrList[i].Location = new System.Drawing.Point(trackBar2.Location.X, trackBar2.Location.Y + 100*i);
           //     TrList[i].Size = trackBar2.Size;
           //     TrList[i].Parent = this.ParentForm;
           //     TrList[i].Visible = true;
           //     TrList[i].ValueChanged += TrackBar_ValueChanged;//new System.EventHandler(TrackBar_ValueChanged);
           //     TrList[i].Tag = i;
           //     TrList[i].Maximum = trackBar2.Maximum;
           //     this.Controls.Add(TrList[i]);

    // *******************************************************************************************************************
            }
            end = current;
            anchor = new Vector2(this.panel2.ClientRectangle.Width / 3, this.panel2.ClientRectangle.Height / 3);
            Lsegment.TrimExcess();
           
        }
     
        private void TrackBar_ValueChanged(object sender, EventArgs e)
        {
        //    //TO DO: kod obsługujący ręczną obsługę suwaka  

        //    var trackBar = (TrackBar)sender;
        //   // if (trackBar.Value > vbef)
        ////    {
        //        Lsegment[(int)trackBar.Tag].Angle += (float)(Math.PI / 180);
        //    Lsegment[(int)trackBar.Tag].calculateB();
           
        //    //   end.angle += (float)(Math.PI / 180); // dodawanie 1 stopnia
        //    panel2.Invalidate();
        //        panel2.Update();
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
         
            base.OnPaint(e);
         
            end.calculateB(pg);
            try
            {
                Segment next = end.Parent;

                while (next != null)
                {
 
                    next.follow();
                    next.calculateB(pg);
                    next = next.Parent;
                }

                start.A = new Vector2(anchor.X, anchor.Y);
                start.calculateB(pg);
                next = start.Child;

                while (next != null)
                {
                    next.attachA();
                    next.calculateB(pg);
                    next = next.Child;
                }

                end.Line(pg);

                next = end.Parent;

                while (next != null)
                {
                    next.Line(pg);
                    next = next.Parent;

                }
               
                float an = end.Angle;
                an = (float)(an * 180 / Math.PI);
                label1.Text = an.ToString();
    
                
                
                /* setTrackBars();*/ //*
               

            }   

           
         
            catch { }

        }

   



        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (trackBar2.Value > vbef) // trackbar moę to jest to
            {

              //  end.Angle += 0.1F* (float)(180F/Math.PI ); // dodawanie 1 stopnia w radianach
                end.Angle += (float)(Math.PI * 1 / 180);
                panel2.Invalidate();
                panel2.Update();
            }
            else if (trackBar2.Value < vbef)
            {

                // end.Angle -=0.1F* (float)(180F /Math.PI);
                end.Angle -= (float)(Math.PI * 1 / 180);
                panel2.Invalidate();
                panel2.Update();

            }
            
            vbef = trackBar2.Value; 

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (loading == false)
            {
                Lsegment.Clear();
                InitalizeArm();
                panel2.Invalidate();
            }
            else loading = false;
        }

   
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
       //     pg.TranslateTransform(e.X, e.Y);
            end.follow((float)e.X, (float)e.Y); // podążanie za myszką 
       //     pg.TranslateTransform(-e.X, -e.Y);
            panel2.Invalidate();
           // trackBar2.Value = (int)(end.Angle * (180 / Math.PI)); //*
            setTrackBars();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            settings.PreserveObjectReferences = true;
            
            if (File.Exists("InvKinematics.xml"))
            {
                File.Delete("InvKinematics.xml");
            }

            FileStream writer = new FileStream("InvKinematics.xml",FileMode.Create,FileAccess.Write);
            Lsegment.TrimExcess();
            serial = new DataContractSerializer(typeof(List<Segment>), settings);
            serial.WriteObject(writer, this.Lsegment);

            writer.Close();
            writer.Dispose();
           
        }

        private void label1_Click(object sender, EventArgs e)
        {
         
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
         
            settings.PreserveObjectReferences = true;
            
            FileStream Reader = new FileStream("InvKinematics.xml", FileMode.Open, FileAccess.Read);
           
           serial = new DataContractSerializer(typeof(List<Segment>), settings);

            List<Segment> temp = (List<Segment>)serial.ReadObject(Reader);

            Reader.Dispose();

            LoadFromXml(ref temp);
          
            panel2.Invalidate();
           
            Reader.Close();
            Reader.Dispose();
        }

        private void LoadFromXml(ref List<Segment> list)
        {

            try
            {
                 Lsegment = new List<Segment>();
              
                start = list[0];
                current = start;
                 Lsegment.Add(start);
             
                for (int i = 1; i < list.Count; i++) // uwaga na count 
                {
                    Debug.WriteLine("temp: " + list.Count);
                    Debug.WriteLine("lSegment: " + Lsegment.Count);
                    next = list[i];
                    Lsegment.Add(next);
                    current.Child = next;
                    current = next;
                }
               
                end = current;
          
                loading = true;

                Num1.Value = Lsegment.Count-1;

            }
           catch(Exception ex)
            {
                MessageBox.Show("Exception: " + ex.ToString());
            }
        }

    }
}
