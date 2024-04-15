﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace OpenGL
{
    public partial class Form1 : Form
    {
        int BasicProgramID;
        int BasicVertexShader;
        int BasicFragmentShader;
        Vector3 CubeColor;
        float CubeTransparency;
        float CubeSpecularForce;
        int CubeSpecular;
        Vector3 CameraPosition;
        Vector3 CubeCoord2;
        int rayTracingDepth;

        public Form1()
        {
            InitializeComponent();
        }

        void loadShader(String filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (System.IO.StreamReader sr = new System.IO.StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        void SetUniformVec3(string name, OpenTK.Vector3 value)
        {
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, name), value);
        }
        void SetUniformFloat(string name, float value)
        {
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, name), value);
        }
        void SetUniformInt(string name, int value)
        {
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, name), value);
        }

        void InitShaders()
        {
            BasicProgramID = GL.CreateProgram();
            loadShader("..\\..\\Shaders\\raytracing.vert", ShaderType.VertexShader, BasicProgramID, out BasicVertexShader);
            loadShader("..\\..\\Shaders\\raytracing.frag", ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);
            GL.LinkProgram(BasicProgramID);
            int status = 0;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);
            Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));
        }

        private static bool Init()
        {
            GL.Enable(EnableCap.ColorMaterial);
            GL.ShadeModel(ShadingModel.Smooth);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            return true;
        }


        private void Draw()
        {
            GL.ClearColor(Color.AliceBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(BasicProgramID);
            SetUniformVec3("cube_color", CubeColor);
            SetUniformVec3("camera_position", CameraPosition);
            SetUniformFloat("cube_transparency", CubeTransparency);
            SetUniformInt("cube_specular", CubeSpecular);
            SetUniformFloat("cube_specular_force", CubeSpecularForce);
            SetUniformInt("rayTracingDepth", rayTracingDepth);
            // Quad
            GL.Color3(Color.White);
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0, 1);
            GL.Vertex2(-1, -1);

            GL.TexCoord2(1, 1);
            GL.Vertex2(1, -1);

            GL.TexCoord2(1, 0);
            GL.Vertex2(1, 1);

            GL.TexCoord2(0, 0);
            GL.Vertex2(-1, 1);

            GL.End();
            openGlControl.SwapBuffers();
            GL.UseProgram(0);
        }

        private void openGlControl_Paint(object sender, PaintEventArgs e)
        {
            Draw();
        }

        private void openGlControl_Load(object sender, EventArgs e)
        {
            Init();
            InitShaders();
            CubeSpecular = 3;
            CubeSpecularForce = 0.5f;
            CubeTransparency = 1.0f;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            CubeColor.X = trackBar1.Value / 255.0f;
            openGlControl.Invalidate();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            CameraPosition.X = trackBar4.Value;
            openGlControl.Invalidate();
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            CameraPosition.Y = trackBar5.Value;
            openGlControl.Invalidate();
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            CameraPosition.Z = trackBar6.Value;
            openGlControl.Invalidate();
        }

        private void trackBar2_Scroll_1(object sender, EventArgs e)
        {
            CubeColor.Y = trackBar2.Value / 255.0f;
            openGlControl.Invalidate();
        }

        private void trackBar3_Scroll_1(object sender, EventArgs e)
        {
            CubeColor.Z = trackBar3.Value / 255.0f;
            openGlControl.Invalidate();
        }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            CubeTransparency = trackBar7.Value / 255.0f;

            float minForce = 1.0f;
            float maxForce = 2.0f;
            float valueRange = maxForce - minForce;

            float trackBarValue = trackBar7.Value;

            CubeTransparency = minForce + (valueRange * trackBarValue / trackBar7.Maximum);

            openGlControl.Invalidate();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                CubeSpecular = 1;
                openGlControl.Invalidate();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                CubeSpecular = 2;
                openGlControl.Invalidate();
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                CubeSpecular = 3;
                openGlControl.Invalidate();
            }
        }

        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            int minForce = 0;
            int maxForce = 10;
            int valueRange = maxForce - minForce;

            int trackBarValue = trackBar8.Value;

            rayTracingDepth = minForce + (valueRange * trackBarValue / trackBar8.Maximum);
            openGlControl.Invalidate();
        }

        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            float minForce = 0.0f;
            float maxForce = 1.0f;
            float valueRange = maxForce - minForce;

            float trackBarValue = trackBar9.Value;

            float cubeSpecularForce = minForce + (valueRange * trackBarValue / trackBar9.Maximum);

            CubeSpecularForce = cubeSpecularForce;
            openGlControl.Invalidate();
        }
    }
}
