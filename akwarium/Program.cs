using System;
using Tao.FreeGlut;
using OpenGL;
using System.Drawing;
using System.Media;

namespace akwarium
{

    class Program
    {
        private static int szerokosc = 1280, wysokosc = 720;
        private static ShaderProgram program;
        private static VBO<Vector3> akwar;
        private static VBO<int> akwarElements;
        private static VBO<Vector2> akwarUV;
        private static VBO<Vector3> normals;
        private static Texture glassTexture;
        private static Texture kolorK1, kolorK2, kolorK3, kolorK4;
        private static float left_site, right_site, top_site, bottom_site, back_site, front_site;
        public static double r1 =0.3d, r2 = 0.1d, r3 =0.2d , r4 =0.3d ;
        private static float time;
        private static float time_skal1 =1f;
        private static float time_skal2 = 1f;
        private static float time_skal3 = 1f;
        private static float time_skal4 = 1f;
        private static System.Diagnostics.Stopwatch watch;
        
        
        private static SoundPlayer music;
        private static SoundPlayer click;

        
        private static float angleX , angleY , angleZ ;

        private static float kula1X  = -0.5f, kula1Y=0.5f, kula1Z = -0.0f;//warunki początwkowe kuli 1;
        private static float kula2X = 0.1f, kula2Y = 0.1f, kula2Z = 0.0f;// warinki początkowe kuli 2;
        private static float kula3X = 0.9f, kula3Y = -0.7f, kula3Z = 0.0f;// warinki początkowe kuli 3;
        private static float kula4X = 0.5f, kula4Y = -0.5f, kula4Z = -0.0f;// warinki początkowe kuli 4;

        private static bool timeOut1 = false, timeOut2 = false, timeOut3= false, timeOut4=  false;

        private static bool czyPrawa1 = true, czyLewa1 = true, czyDol1 = true, czyGora1 = true, czyTyl1 = true, czyPrzod1 = true;
        private static bool czyPrawa2 = true, czyLewa2 = true, czyDol2 = true, czyGora2 = true, czyTyl2 = true, czyPrzod2 = true;
        private static bool czyPrawa3 = true, czyLewa3 = true, czyDol3 = true, czyGora3 = true, czyTyl3 = true, czyPrzod3 = true;
        private static bool czyPrawa4 = true, czyLewa4 = true, czyDol4 = true, czyGora4 = true, czyTyl4 = true, czyPrzod4 = true;

        static void Main(string[] args)
        {

                Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(szerokosc, wysokosc);
            Glut.glutCreateWindow("Akwarium");
            
            Glut.glutIdleFunc(Rend_okna);
            Glut.glutDisplayFunc(W_oknie);
            Glut.glutCloseFunc(OnClose);
            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutKeyboardUpFunc(OnKeyboardUp);
            
            Gl.Disable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.Blend); //ustawianie przezroczystosci
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            program = new ShaderProgram(VertexShader, FragmentShader);

            music = new SoundPlayer("G:\\Projekty\\PA\\akwarium\\music.wav");
            click = new SoundPlayer("G:\\Projekty\\PA\\akwarium\\click.wav");

            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)szerokosc / wysokosc, 0.1f, 1000f)); // jest tu zapisane jak daleko jest ogląd 
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up)); // patrzenie
            program["light_direction"].SetValue(new Vector3(1,1,1));


            //loadowanie textur
            glassTexture = new Texture("G:\\Projekty\\PA\\akwarium\\glass.jpg");
            kolorK1 = new Texture("G:\\Projekty\\PA\\akwarium\\piora.jpg");
            kolorK2 = new Texture("G:\\Projekty\\PA\\akwarium\\cos.jpg");
            kolorK3 = new Texture("G:\\Projekty\\PA\\akwarium\\klab.jpg");
            kolorK4 = new Texture("G:\\Projekty\\PA\\akwarium\\paski.jpg");


            akwarium_position(2,1,1); /// ustawianie akwarium

            normals = new VBO<Vector3>(new Vector3[] {

                //front normals
                new Vector3(0,0,1), new Vector3(0,0,1), new Vector3(0,0,1),
                 new Vector3(0,0,1), new Vector3(0,0,1), new Vector3(0,0,1),

                 //back normals
                new Vector3(0,0,-1), new Vector3(0,0,-1), new Vector3(0,0,-1),
                 new Vector3(0,0,-1), new Vector3(0,0,-1), new Vector3(0,0,-1),

                 //top normals
                new Vector3(0,1,0), new Vector3(0,1,0), new Vector3(0,1,0),
                 new Vector3(0,1,0), new Vector3(0,1,0), new Vector3(0,1,0),

                 //bottom normals
                new Vector3(0,-1,0), new Vector3(0,-1,0), new Vector3(0,-1,0),
                 new Vector3(0,-1,0), new Vector3(0,-1,0), new Vector3(0,-1,0),

                  //left normals
                new Vector3(-1,0,0), new Vector3(-1,0,0), new Vector3(-1,0,0),
                 new Vector3(-1,0,0), new Vector3(-1,0,0), new Vector3(-1,0,0),

                 //right normals
                new Vector3(1,0,0), new Vector3(1,0,0), new Vector3(1,0,0),
                 new Vector3(1,0,0), new Vector3(1,0,0), new Vector3(1,0,0)
            });

            akwarUV = new VBO<Vector2>(new Vector2[] {

                //front
                 new Vector2(0,0), new Vector2(1,0),  new Vector2(1,1),
                new Vector2(1,1), new Vector2(0,1),  new Vector2(0,0),
                //back
                new Vector2(0,0), new Vector2(1,0),  new Vector2(1,1),
                new Vector2(1,1), new Vector2(0,1),  new Vector2(0,0),
                //top
                new Vector2(0,0), new Vector2(1,0),  new Vector2(1,1),
                new Vector2(1,1), new Vector2(0,1),  new Vector2(0,0),
                //bottom
                 new Vector2(0,0), new Vector2(1,0),  new Vector2(1,1),
                new Vector2(1,1), new Vector2(0,1),  new Vector2(0,0),
               //left
                 new Vector2(0,0), new Vector2(1,0),  new Vector2(1,1),
                new Vector2(1,1), new Vector2(0,1),  new Vector2(0,0),
                //right
                 new Vector2(0,0), new Vector2(1,0),  new Vector2(1,1),
                new Vector2(1,1), new Vector2(0,1),  new Vector2(0,0),
               
             

            });
            
            akwarElements = new VBO<int>(new int [] {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35}, BufferTarget.ElementArrayBuffer);
            music_play();

            watch = System.Diagnostics.Stopwatch.StartNew();
            Glut.glutMainLoop();
        }
        private static void music_play()
        {
            music.PlayLooping();
        }

        private static void OnClose()
        {
            Console.Clear();
            normals.Dispose();
            akwar.Dispose();
            akwarUV.Dispose();
            akwarElements.Dispose();
            glassTexture.Dispose();
            program.DisposeChildren = true;
            program.Dispose();
        }

        private static void W_oknie()
        {
            /*
            Console.Clear();
            Console.WriteLine("podaj wielkosci kul:");
            Console.WriteLine("kulka 1:");
      */
            
        }
        private static void Rend_okna()
        {
            watch.Stop();
            float deltaTime = (float)watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
            time = deltaTime;
            watch.Restart();
            
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0+angleX,0+angleY,10+angleZ), Vector3.Zero, Vector3.Up));
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.Viewport(0, 0, szerokosc, wysokosc);
            Gl.UseProgram(program);
            Gl.ClearColor(0, 0, 0, 1);

            
            akwarium();
            kula1();
            kula2();
            kula3();
            kula4();
            
            
            Glut.glutSwapBuffers();
        }
        private static void akwarium_position(float akwarX, float akwarY, float akwarZ)
        {
            akwar = new VBO<Vector3>(new Vector3[] { 
                // front
                new Vector3(-akwarX,-akwarY,akwarZ),  new Vector3(akwarX,-akwarY,akwarZ),  new Vector3(akwarX,akwarY,akwarZ),
                new Vector3(akwarX,akwarY,akwarZ),  new Vector3(-akwarX,akwarY,akwarZ),  new Vector3(-akwarX,-akwarY,akwarZ),  
                //back
                new Vector3(-akwarX,-akwarY,-akwarZ),  new Vector3(akwarX,-akwarY,-akwarZ),  new Vector3(akwarX,akwarY,-akwarZ),
                new Vector3(akwarX,akwarY,-akwarZ),  new Vector3(-akwarX,akwarY,-akwarZ),  new Vector3(-akwarX,-akwarY,-akwarZ), 
                //top
                new Vector3(-akwarX,akwarY,akwarZ),  new Vector3(akwarX,akwarY,akwarZ),  new Vector3(akwarX,akwarY,-akwarZ),
                new Vector3(akwarX,akwarY,-akwarZ),  new Vector3(-akwarX,akwarY,-akwarZ),  new Vector3(-akwarX,akwarY,akwarZ),
                //bottom
                new Vector3(-akwarX,-akwarY,akwarZ),  new Vector3(akwarX,-akwarY,akwarZ),  new Vector3(akwarX,-akwarY,-akwarZ),
                new Vector3(akwarX,-akwarY,-akwarZ),  new Vector3(-akwarX,-akwarY,-akwarZ),  new Vector3(-akwarX,-akwarY,akwarZ),
                //left
                new Vector3(-akwarX,-akwarY,-akwarZ),  new Vector3(-akwarX,-akwarY,akwarZ),  new Vector3(-akwarX,akwarY,akwarZ),
                new Vector3(-akwarX,akwarY,akwarZ),  new Vector3(-akwarX,akwarY,-akwarZ),  new Vector3(-akwarX,-akwarY,-akwarZ),
                 //right
                new Vector3(akwarX,-akwarY,-akwarZ),  new Vector3(akwarX,-akwarY,akwarZ),  new Vector3(akwarX,akwarY,akwarZ),
                new Vector3(akwarX,akwarY,akwarZ),  new Vector3(akwarX,akwarY,-akwarZ),  new Vector3(akwarX,-akwarY,-akwarZ),
            });
            left_site = -akwarX;
            right_site = akwarX;
            front_site = akwarZ;
            back_site = -akwarZ;
            top_site = akwarY;
            bottom_site = -akwarY;

        }

        private static void akwarium()
        {
           
            
            Gl.BindTexture(glassTexture);
            program["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(0, 0, 0))); //tu ustawuamy rotacje przedmiotu 
            Gl.BindBufferToShaderAttribute(akwar, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(normals, program, "vertexNormal");
            Gl.BindBufferToShaderAttribute(akwarUV, program, "vertexUV");
            Gl.BindBuffer(akwarElements);
            //rysowanie akwarium;
            Gl.DrawElements(BeginMode.Triangles, akwarElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

        }
  
    
        private static void kula1()
        {
            
            // prawo lewo
            if (czyLewa1 == true && Math.Round(kula1X, 1) <= right_site - r1)
            {

                czyPrawa1 = false;
                kula1X += time * time_skal1;
                if (Math.Round(kula1X, 1) >= right_site - r1)
                {
                    czyPrawa1 = true;
                    czyLewa1 = false;
                    
               
                }
                    // warunek dla kuli 2
                else if (Math.Round(kula1X, 1) == Math.Round(kula2X, 1) - r2 && ((Math.Round(kula1Y,1)  > Math.Round(kula2Y, 1) - r2) && (Math.Round(kula1Y, 1) < Math.Round(kula2Y, 1) + r2) &&((Math.Round(kula1Z,1)> Math.Round(kula2Z,1) -r2) && Math.Round(kula1Z,1) < Math.Round(kula2Z,1)+r2)))
                {
                    czyPrawa1 = true;
                    czyLewa1 = false;

                   
                }
                // warunek dla kuli 3
                else if (Math.Round(kula1X, 1) == Math.Round(kula3X, 1) - r3 && ((Math.Round(kula1Y, 1) > Math.Round(kula3Y, 1) - r3) && (Math.Round(kula1Y, 1) < Math.Round(kula3Y, 1) + r3) && ((Math.Round(kula1Z, 1) > Math.Round(kula3Z, 1) - r3) && Math.Round(kula1Z, 1) < Math.Round(kula3Z, 1) + r3)))
                {
                    czyPrawa1 = true;
                    czyLewa1 = false;

                  
                }
                // warunek kuli 4
                else if (Math.Round(kula1X, 1) == Math.Round(kula4X, 1) - r4 && ((Math.Round(kula1Y, 1) > Math.Round(kula4Y, 1) - r4) && (Math.Round(kula1Y, 1) < Math.Round(kula4Y, 1) + r4) && ((Math.Round(kula1Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula1Z, 1) < Math.Round(kula4Z, 1) + r4)))
                {
                    czyPrawa1 = true;
                    czyLewa1 = false;

                  
                }
            }
            if (czyPrawa1 == true && Math.Round(kula1X, 1) >= left_site + r1)
            {

                czyLewa1 = false;
                kula1X -= time * time_skal1;
                if (Math.Round(kula1X, 1) <= left_site + r1)
                {
                    czyPrawa1 = false;
                    czyLewa1 = true;
                    
                }
                    //warunek dla kuli 2 
                else if (Math.Round(kula1X, 1) == Math.Round(kula2X, 1) +  r2 &&(Math.Round(kula1Y,1) > Math.Round(kula2Y, 1) - r2 && Math.Round(kula1Y, 1) < Math.Round(kula2Y, 1) + r2) &&((Math.Round(kula1Z,1)> Math.Round(kula2Z,1) -r2) && Math.Round(kula1Z,1) < Math.Round(kula2Z,1)+r2))
                {
                    czyPrawa1 = false;
                    czyLewa1 = true;

                    
                }
                //warunek kuli 3 
                else if (Math.Round(kula1X, 1) == Math.Round(kula3X, 1) + r3 && (Math.Round(kula1Y, 1) > Math.Round(kula3Y, 1) - r3 && Math.Round(kula1Y, 1) < Math.Round(kula3Y, 1) + r3) && ((Math.Round(kula1Z, 1) > Math.Round(kula3Z, 1) - r3) && Math.Round(kula1Z, 1) < Math.Round(kula3Z, 1) + r3))
                {
                    czyPrawa1 = false;
                    czyLewa1 = true;

                }
                // warunek kuli 4
                else if (Math.Round(kula1X, 1) == Math.Round(kula4X, 1) + r4 && (Math.Round(kula1Y, 1) > Math.Round(kula4Y, 1) - r4 && Math.Round(kula1Y, 1) < Math.Round(kula4Y, 1) + r4) && ((Math.Round(kula1Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula1Z, 1) < Math.Round(kula4Z, 1) + r4))
                {
                    czyPrawa1 = false;
                    czyLewa1 = true;
                
                }
            }
            
            //góra dół
            if (czyGora1 == true && Math.Round(kula1Y, 1) <= top_site - r1)
            {

               czyDol1 = false;
               kula1Y += time * time_skal1;
                if (Math.Round(kula1Y, 1) >= top_site - r1)
                {
                    czyDol1 = true;
                    czyGora1 = false;
                  
                }
                    //warunek dla kuli 2
                else if (Math.Round(kula1Y, 1) == Math.Round(kula2Y, 1) -  r2 && (Math.Round(kula1X,1) > Math.Round(kula2X, 1) - r2 && (Math.Round(kula1X, 1) < Math.Round(kula2X, 1) + r2)&&(Math.Round(kula1Z,1)> Math.Round(kula2Z,1) -r2) && Math.Round(kula1Z,1) < Math.Round(kula2Z,1)+r2))
                {
                    czyDol1 = true;
                    czyGora1 = false;

                    
                }
                //warunek kuli 3 
                else if (Math.Round(kula1Y, 1) == Math.Round(kula3Y, 1) - r3 && (Math.Round(kula1X, 1) > Math.Round(kula3X, 1) - r3 && (Math.Round(kula1X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula1Z, 1) > Math.Round(kula3Z, 1) - r3) && Math.Round(kula1Z, 1) < Math.Round(kula3Z, 1) + r3))
                {
                    czyDol1 = true;
                    czyGora1 = false;

                }
                // warunek kuli 4
                else if (Math.Round(kula1Y, 1) == Math.Round(kula4Y, 1) - r4 && (Math.Round(kula1X, 1) > Math.Round(kula4X, 1) - r4 && (Math.Round(kula1X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula1Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula1Z, 1) < Math.Round(kula4Z, 1) + r4))
                {
                    czyDol1 = true;
                    czyGora1 = false;
  
                   
                }
            }
            if (czyDol1 == true && Math.Round(kula1Y, 1) >= bottom_site + r1)
            {

                czyGora1 = false;
                kula1Y -= time;
                if (Math.Round(kula1Y, 1) <= bottom_site + r1)
                {
                    czyDol1 = false;
                    czyGora1 = true;
                    
                }
                    // warunek dla kuli 2
                else if (Math.Round(kula1Y, 1) == Math.Round(kula2Y, 1) + r2 &&(Math.Round(kula1X,1) > Math.Round(kula2X, 1) - r2 && Math.Round(kula1X, 1) < Math.Round(kula2X, 1) + r2)&&(Math.Round(kula1Z,1)> Math.Round(kula2Z,1) -r2) && Math.Round(kula1Z,1) < Math.Round(kula2Z,1)+r2)
                {
                    czyDol1 = false;
                    czyGora1 = true;

                  
                }
                //warunek kuli 3 
                else if (Math.Round(kula1Y, 1) == Math.Round(kula3Y, 1) + r3 && (Math.Round(kula1X, 1) > Math.Round(kula3X, 1) - r3 && Math.Round(kula1X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula1Z, 1) > Math.Round(kula3Z, 1) - r3)&& Math.Round(kula1Z, 1) < Math.Round(kula3Z, 1) + r3)
                {
                    czyDol1 = false;
                    czyGora1 = true;

                }
                // warunek kuli 4
                else if (Math.Round(kula1Y, 1) == Math.Round(kula4Y, 1) + r4 && (Math.Round(kula1X, 1) > Math.Round(kula4X, 1) - r4 && Math.Round(kula1X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula1Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula1Z, 1) < Math.Round(kula4Z, 1) + r4)
                {
                    czyDol1 = false;
                    czyGora1 = true;

                }
            }
            //przód tył
            if (czyPrzod1 == true && Math.Round(kula1Z, 1) <= front_site - r1)
            {

                czyTyl1 = false;
                kula1Z += time * time_skal1;
                if (Math.Round(kula1Z, 1) >= front_site - r1)
                {
                    czyTyl1 = true;
                    czyPrzod1 = false;
                }
                    // warunek dla kuli 2
                else if (Math.Round(kula1Z, 1) == Math.Round(kula2Z, 1) - r2 && (Math.Round(kula1X,1) > Math.Round(kula2X,1) -r2) && (Math.Round(kula1X,1)< Math.Round(kula2X,1)+r2) &&(Math.Round(kula1Y,1)> Math.Round(kula2Y,1) -r2 &&Math.Round(kula1Y,1)< Math.Round(kula2Y,1) +r2))
                {
                    czyTyl1 = true;
                    czyPrzod1 = false;
  
                }
                //warunek kuli 3 
                else if (Math.Round(kula1Z, 1) == Math.Round(kula3Z, 1) - r3 && (Math.Round(kula1X, 1) > Math.Round(kula3X, 1) - r3) && (Math.Round(kula1X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula1Y, 1) > Math.Round(kula3Y, 1) - r3 && Math.Round(kula1Y, 1) < Math.Round(kula3Y, 1) + r3))
                {
                    czyTyl1 = true;
                    czyPrzod1 = false;

                }
                // warunek kuli 4
                else if (Math.Round(kula1Z, 1) == Math.Round(kula4Z, 1) - r4 && (Math.Round(kula1X, 1) > Math.Round(kula4X, 1) - r4) && (Math.Round(kula1X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula1Y, 1) > Math.Round(kula4Y, 1) - r4 && Math.Round(kula1Y, 1) < Math.Round(kula4Y, 1) + r4))
                {
                    czyTyl1 = true;
                    czyPrzod1 = false;

                }
            }
            if (czyTyl1 == true && Math.Round(kula1Z, 1) >= back_site + r1)
            {

                czyPrzod1 = false;
                kula1Z -= time * time_skal1;
                if (Math.Round(kula1Z, 1) <= back_site + r1)
                {
                    czyTyl1 = false;
                    czyPrzod1 = true;
                  
                }
                    // warunek dla kuli 2
                else if (Math.Round(kula1Z, 1) == Math.Round(kula2Z, 1) + r2 && (Math.Round(kula1X, 1) > Math.Round(kula2X, 1) - r2) && (Math.Round(kula1X, 1) < Math.Round(kula2X, 1) + r2) && (Math.Round(kula1Y, 1) > Math.Round(kula2Y, 1) - r2 && Math.Round(kula1Y, 1) < Math.Round(kula2Y, 1) + r2))
                {
                    czyTyl1 = false;
                    czyPrzod1 = true;

                }
                //warunek kuli 3 
                else if (Math.Round(kula1Z, 1) == Math.Round(kula3Z, 1) + r3 && (Math.Round(kula1X, 1) > Math.Round(kula3X, 1) - r3) && (Math.Round(kula1X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula1Y, 1) > Math.Round(kula3Y, 1) - r3 && Math.Round(kula1Y, 1) < Math.Round(kula3Y, 1) + r3))
                {
                    czyTyl1 = false;
                    czyPrzod1 = true;

                }
                // warunek kuli 4
                else if (Math.Round(kula1Z, 1) == Math.Round(kula4Z, 1) + r4 && (Math.Round(kula1X, 1) > Math.Round(kula4X, 1) - r4) && (Math.Round(kula1X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula1Y, 1) > Math.Round(kula4Y, 1) - r4 && Math.Round(kula1Y, 1) < Math.Round(kula4Y, 1) + r4))
                {
                    czyTyl1 = false;
                    czyPrzod1 = true;

                }
            }

            Vector3 K1 =  new Vector3(0+kula1X,0+kula1Y,0+kula1Z);
            Gl.BindTexture(kolorK1);
           program["model_matrix"].SetValue(Matrix4.CreateTranslation(K1));
           Glut.glutSolidSphere(r1, 20, 20);


           
        }
        private static void kula2()
            {
               //prawo lewo
                if (czyLewa2 == true && Math.Round(kula2X, 1) <= right_site - r2)
                {
                    czyPrawa2 = false;
                    kula2X += time * time_skal2;
                    if (Math.Round(kula2X, 1) >= right_site - r2)
                    {
                        czyPrawa2 = true;
                        czyLewa2 = false;
                    }
                    // warunek 1 kuli
                    else if(Math.Round(kula2X, 1) == Math.Round(kula1X, 1) - r1 && ((Math.Round(kula2Y,1)  > Math.Round(kula1Y, 1) - r1) && (Math.Round(kula2Y, 1) < Math.Round(kula1Y, 1) + r1) &&((Math.Round(kula2Z,1)> Math.Round(kula1Z,1) -r1) && Math.Round(kula2Z,1) < Math.Round(kula1Z,1)+r1)))
                    {
                        czyPrawa2 = true;
                        czyLewa2 = false;
                    }
                    //warunek kuli 3 
                    else if (Math.Round(kula2X, 1) == Math.Round(kula3X, 1) - r4 && ((Math.Round(kula2Y, 1) > Math.Round(kula3Y, 1) - r3) && (Math.Round(kula2Y, 1) < Math.Round(kula3Y, 1) + r3) && ((Math.Round(kula2Z, 1) > Math.Round(kula3Z, 1) - r3) && Math.Round(kula2Z, 1) < Math.Round(kula3Z, 1) + r3)))
                    {
                        czyPrawa2 = true;
                        czyLewa2 = false;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula2X, 1) == Math.Round(kula4X, 1) - r4 && ((Math.Round(kula2Y, 1) > Math.Round(kula4Y, 1) - r4)&& (Math.Round(kula2Y, 1) < Math.Round(kula4Y, 1) + r4) && ((Math.Round(kula2Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula2Z, 1) < Math.Round(kula4Z, 1) + r4)))
                    {
                        czyPrawa2 = true;
                        czyLewa2 = false;
                    }
                }
                if (czyPrawa2 == true && Math.Round(kula2X, 1) >= left_site + r2)
                {
                    czyLewa2 = false;
                    kula2X -= time * time_skal2;
                    if (Math.Round(kula2X, 1) <= left_site + r2)
                    {
                        czyPrawa2 = false;
                        czyLewa2 = true;
                    }
                     //warunek 1 kuli
                    else if (Math.Round(kula2X, 1) == Math.Round(kula1X, 1) + r1 && (Math.Round(kula2Y, 1) > Math.Round(kula1Y, 1) - r1 && Math.Round(kula2Y, 1) < Math.Round(kula1Y, 1) + r1) && ((Math.Round(kula2Z, 1) > Math.Round(kula1Z, 1) - r1) && Math.Round(kula2Z, 1) < Math.Round(kula1Z, 1) + r1))
                    {
                        czyPrawa2 = false;
                        czyLewa2 = true;
                    }
                    //warunek kuli 3 
                    else if (Math.Round(kula2X, 1) == Math.Round(kula3X, 1) + r3 && (Math.Round(kula2Y, 1) > Math.Round(kula3Y, 1) - r3 && Math.Round(kula2Y, 1) < Math.Round(kula3Y, 1) + r3)&& ((Math.Round(kula2Z, 1) > Math.Round(kula3Z, 1) - r3) && Math.Round(kula2Z, 1) < Math.Round(kula3Z, 1) + r3))
                    {
                        czyPrawa2 = false;
                        czyLewa2 = true;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula2X, 1) == Math.Round(kula4X, 1) + r4 && (Math.Round(kula2Y, 1) > Math.Round(kula4Y, 1) - r4 && Math.Round(kula2Y, 1) < Math.Round(kula4Y, 1) + r4) && ((Math.Round(kula2Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula2Z, 1) < Math.Round(kula4Z, 1) + r4))
                    {
                        czyPrawa2 = false;
                        czyLewa2 = true;
                    }
                }

               //góra dół
                if (czyGora2 == true && Math.Round(kula2Y, 1) <= top_site - r2)
                {
                    czyDol2 = false;
                    kula2Y += time * time_skal2;
                    if (Math.Round(kula2Y, 1) >= top_site - r2)
                    {
                        czyDol2 = true;
                        czyGora2 = false;
                    }
                    //warunek 1 kuli
                    else if (Math.Round(kula2Y, 1) == Math.Round(kula1Y, 1) - r1 && (Math.Round(kula2X, 1) > Math.Round(kula1X, 1) - r1 && (Math.Round(kula2X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula2Z, 1) > Math.Round(kula1Z, 1) - r1) && Math.Round(kula2Z, 1) < Math.Round(kula1Z, 1) + r1))
                    {
                        czyDol2 = true;
                        czyGora2 = false;
                    }
                    //warunek kuli 3 
                    else if (Math.Round(kula2Y, 1) == Math.Round(kula3Y, 1) - r3 && (Math.Round(kula2X, 1) > Math.Round(kula3X, 1) - r3 && (Math.Round(kula2X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula2Z, 1) > Math.Round(kula3Z, 1) - r3) && Math.Round(kula2Z, 1) < Math.Round(kula3Z, 1) + r3))
                    {
                        czyDol2 = true;
                        czyGora2 = false;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula2Y, 1) == Math.Round(kula4Y, 1) - r4 && (Math.Round(kula2X, 1) > Math.Round(kula4X, 1) - r4 && (Math.Round(kula2X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula2Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula2Z, 1) < Math.Round(kula4Z, 1) + r4))
                    {
                        czyDol2 = true;
                        czyGora2 = false;
                    }
                }
                if (czyDol2 == true && Math.Round(kula2Y, 1) >= bottom_site + r2)
                {
                    czyGora2 = false;
                    kula2Y -= time * time_skal2;
                    if (Math.Round(kula2Y, 1) <= bottom_site + r2)
                    {
                        czyDol2 = false;
                        czyGora2 = true;
                    }
                    // warunek 1 kuli
                    else if (Math.Round(kula2Y, 1) == Math.Round(kula1Y, 1) + r1 && (Math.Round(kula2X, 1) > Math.Round(kula1X, 1) - r1 && Math.Round(kula2X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula2Z, 1) > Math.Round(kula1Z, 1) - r1) && Math.Round(kula2Z, 1) < Math.Round(kula1Z, 1) + r1)
                    {
                        czyDol2 = false;
                        czyGora2 = true;
                    }
                    //warunek kuli 3 
                    else if (Math.Round(kula2Y, 1) == Math.Round(kula3Y, 1) + r3 && (Math.Round(kula2X, 1) > Math.Round(kula3X, 1) - r3 && Math.Round(kula2X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula2Z, 1) > Math.Round(kula3Z, 1) - r3) && Math.Round(kula2Z, 1) < Math.Round(kula3Z, 1) + r3)
                    {
                        czyDol2 = false;
                        czyGora2 = true;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula2Y, 1) == Math.Round(kula4Y, 1) + r4 && (Math.Round(kula2X, 1) > Math.Round(kula4X, 1) - r4 && Math.Round(kula2X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula2Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula2Z, 1) < Math.Round(kula4Z, 1) + r4)
                    {
                        czyDol2 = false;
                        czyGora2 = true;
                    }
                }
                //przód tył
                if (czyPrzod2 == true && Math.Round(kula2Z, 1) <= front_site - r2)
                {
                    czyTyl2 = false;
                    kula2Z += time * time_skal2;
                    if (Math.Round(kula2Z, 1) >= front_site - r2)
                    {
                        czyTyl2 = true;
                        czyPrzod2 = false;
                    }
                    //warunek 1 kuli
                    else if (Math.Round(kula2Z, 1) == Math.Round(kula1Z, 1) - r1 && (Math.Round(kula2X, 1) > Math.Round(kula1X, 1) - r1) && (Math.Round(kula2X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula2Y, 1) > Math.Round(kula1Y, 1) - r1 && Math.Round(kula2Y, 1) < Math.Round(kula1Y, 1) + r1))
                    {
                        czyTyl2 = true;
                        czyPrzod2 = false;
                    }
                    //warunek kuli 3 
                    else if (Math.Round(kula2Z, 1) == Math.Round(kula3Z, 1) - r3 && (Math.Round(kula2X, 1) > Math.Round(kula3X, 1) - r3) && (Math.Round(kula2X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula2Y, 1) > Math.Round(kula3Y, 1) - r3 && Math.Round(kula2Y, 1) < Math.Round(kula3Y, 1) + r3))
                    {
                        czyTyl2 = true;
                        czyPrzod2 = false;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula2Z, 1) == Math.Round(kula4Z, 1) - r4 && (Math.Round(kula2X, 1) > Math.Round(kula4X, 1) - r4) && (Math.Round(kula2X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula2Y, 1) > Math.Round(kula4Y, 1) - r4 && Math.Round(kula2Y, 1) < Math.Round(kula4Y, 1) + r4))
                    {
                        czyTyl2 = true;
                        czyPrzod2 = false;
                    }
                }
                if (czyTyl2 == true && Math.Round(kula2Z, 1) >= back_site + r2)
                {
                    czyPrzod2 = false;
                    kula2Z -= time * time_skal2;
                    if (Math.Round(kula2Z, 1) <= back_site + r2)
                    {
                        czyTyl2 = false;
                        czyPrzod2 = true;
                    }
                    // warunek 1 kuli
                    else if (Math.Round(kula2Z, 1) == Math.Round(kula1Z, 1) + r1 && (Math.Round(kula2X, 1) > Math.Round(kula1X, 1) - r1) && (Math.Round(kula2X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula2Y, 1) > Math.Round(kula1Y, 1) - r1 && Math.Round(kula2Y, 1) < Math.Round(kula1Y, 1) + r1))
                    {
                        czyTyl2 = false;
                        czyPrzod2 = true;
                    }
                    //warunek kuli 3 
                    else if (Math.Round(kula2Z, 1) == Math.Round(kula3Z, 1) + r3 && (Math.Round(kula2X, 1) > Math.Round(kula3X, 1) - r3) && (Math.Round(kula2X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula2Y, 1) > Math.Round(kula3Y, 1) - r3 && Math.Round(kula2Y, 1) < Math.Round(kula3Y, 1) + r3))
                    {
                        czyTyl2 = false;
                        czyPrzod2 = true;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula2Z, 1) == Math.Round(kula4Z, 1) + r4 && (Math.Round(kula2X, 1) > Math.Round(kula4X, 1) - r4) && (Math.Round(kula2X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula2Y, 1) > Math.Round(kula4Y, 1) - r4 && Math.Round(kula2Y, 1) < Math.Round(kula4Y, 1) + r4))
                    {
                        czyTyl2 = false;
                        czyPrzod2 = true;
                    }
                }
                Vector3 K2 = new Vector3(0+kula2X,0+kula2Y,0+kula2Z);
                Gl.BindTexture(kolorK2);
                program["model_matrix"].SetValue(Matrix4.CreateTranslation(K2));
                Glut.glutSolidSphere(r2, 20, 20);    
        

            }
        private static void kula3()
        {
            //prawa lewa
                if (czyLewa3 == true && Math.Round(kula3X, 1) <= right_site - r3)
                {
                    czyPrawa3 = false;
                    kula3X += time * time_skal3;
                    if (Math.Round(kula3X, 1) >= right_site - r3)
                    {
                        czyPrawa3 = true;
                        czyLewa3 = false;
                    }
                    // warunek dla kuli 1
                    else if (Math.Round(kula3X, 1) == Math.Round(kula1X, 1) - r1 && ((Math.Round(kula3Y, 1) > Math.Round(kula1Y, 1) - r1) && (Math.Round(kula3Y, 1) < Math.Round(kula1Y, 1) + r1) && ((Math.Round(kula3Z, 1) > Math.Round(kula1Z, 1) - r1) && Math.Round(kula3Z, 1) < Math.Round(kula1Z, 1) + r1)))
                    {
                        czyPrawa3 = true;
                        czyLewa3 = false;
                    }
                    // warunek dla kuli 2
                    else if (Math.Round(kula3X, 1) == Math.Round(kula2X, 1) - r2 && ((Math.Round(kula3Y, 1) > Math.Round(kula2Y, 1) - r2) && (Math.Round(kula3Y, 1) < Math.Round(kula2Y, 1) + r2) && ((Math.Round(kula3Z, 1) > Math.Round(kula2Z, 1) - r2) && Math.Round(kula3Z, 1) < Math.Round(kula2Z, 1) + r2)))
                    {
                        czyPrawa3 = true;
                        czyLewa3 = false;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula3X, 1) == Math.Round(kula4X, 1) - r4 && ((Math.Round(kula3Y, 1) > Math.Round(kula4Y, 1) - r4) && (Math.Round(kula3Y, 1) < Math.Round(kula4Y, 1) + r4) && ((Math.Round(kula3Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula3Z, 1) < Math.Round(kula4Z, 1) + r4)))
                    {
                        czyPrawa3 = true;
                        czyLewa3 = false;
                    }

                }
                if (czyPrawa3 == true && Math.Round(kula3X, 1) >= left_site + r3)
                {
                    czyLewa3 = false;
                    kula3X -= time * time_skal3;
                    if (Math.Round(kula3X, 1) <= left_site + r3)
                    {
                        czyPrawa3 = false;
                        czyLewa3 = true;
                    }
                    // warunek dla kuli 1
                    else if (Math.Round(kula3X, 1) == Math.Round(kula1X, 1) + r1 && ((Math.Round(kula3Y, 1) > Math.Round(kula1Y, 1) - r1) && (Math.Round(kula3Y, 1) < Math.Round(kula1Y, 1) + r1) && ((Math.Round(kula3Z, 1) > Math.Round(kula1Z, 1) - r1) && Math.Round(kula3Z, 1) < Math.Round(kula1Z, 1) + r1)))
                    {
                        czyPrawa3 = false;
                        czyLewa3 = true;
                    }
                    // warunek dla kuli 2
                    else if (Math.Round(kula3X, 1) == Math.Round(kula2X, 1) + r2 && ((Math.Round(kula3Y, 1) > Math.Round(kula2Y, 1) - r2) && (Math.Round(kula3Y, 1) < Math.Round(kula2Y, 1) + r2) && ((Math.Round(kula3Z, 1) > Math.Round(kula2Z, 1) - r2) && Math.Round(kula3Z, 1) < Math.Round(kula2Z, 1) + r2)))
                    {
                        czyPrawa3 = false;
                        czyLewa3 = true;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula3X, 1) == Math.Round(kula4X, 1) + r4 && ((Math.Round(kula3Y, 1) > Math.Round(kula4Y, 1) - r4) && (Math.Round(kula3Y, 1) < Math.Round(kula4Y, 1) + r4) && ((Math.Round(kula3Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula3Z, 1) < Math.Round(kula4Z, 1) + r4)))
                    {
                        czyPrawa3 = false;
                        czyLewa3 = true;
                    }

                }

                //góra dół
                if (czyGora3 == true && Math.Round(kula3Y, 1) <= top_site - r3)
                {
                    czyDol3 = false;
                    kula3Y += time * time_skal3;
                    if (Math.Round(kula3Y, 1) >= top_site - r3)
                    {
                        czyDol3 = true;
                        czyGora3 = false;
                    }
                    //warunek dla kuli 1
                    else if (Math.Round(kula3Y, 1) == Math.Round(kula1Y, 1) - r1 && (Math.Round(kula3X, 1) > Math.Round(kula1X, 1) - r1 && (Math.Round(kula3X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula3Z, 1) > Math.Round(kula1Z, 1) - r1) && Math.Round(kula3Z, 1) < Math.Round(kula1Z, 1) + r1))
                    {
                        czyDol3 = true;
                        czyGora3= false;
                    }
                    //warunek kuli 2
                    else if (Math.Round(kula3Y, 1) == Math.Round(kula2Y, 1) - r2 && (Math.Round(kula3X, 1) > Math.Round(kula2X, 1) - r2 && (Math.Round(kula3X, 1) < Math.Round(kula2X, 1) + r2) && (Math.Round(kula3Z, 1) > Math.Round(kula2Z, 1) - r2) && Math.Round(kula3Z, 1) < Math.Round(kula2Z, 1) + r2))
                    {
                        czyDol3 = true;
                        czyGora3 = false;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula3Y, 1) == Math.Round(kula4Y, 1) - r4 && (Math.Round(kula3X, 1) > Math.Round(kula4X, 1) - r4 && (Math.Round(kula3X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula3Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula3Z, 1) < Math.Round(kula4Z, 1) + r4))
                    {
                        czyDol3 = true;
                        czyGora3 = false;
                    }

                }
                if (czyDol3 == true && Math.Round(kula3Y, 1) >= bottom_site + r3)
                {
                    czyGora3 = false;
                    kula3Y -= time * time_skal3;
                    if (Math.Round(kula3Y, 1) <= bottom_site + r3)
                    {
                        czyDol3 = false;
                        czyGora3 = true;
                    }
                    //warunek dla kuli 1
                    else if (Math.Round(kula3Y, 1) == Math.Round(kula1Y, 1) + r1 && (Math.Round(kula3X, 1) > Math.Round(kula1X, 1) - r1 && (Math.Round(kula3X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula3Z, 1) > Math.Round(kula1Z, 1) - r1) && Math.Round(kula3Z, 1) < Math.Round(kula1Z, 1) + r1))
                    {
                        czyDol3 = false;
                        czyGora3 = true;
                    }
                    //warunek kuli 2
                    else if (Math.Round(kula3Y, 1) == Math.Round(kula2Y, 1) + r2 && (Math.Round(kula3X, 1) > Math.Round(kula2X, 1) - r2 && (Math.Round(kula3X, 1) < Math.Round(kula2X, 1) + r2) && (Math.Round(kula3Z, 1) > Math.Round(kula2Z, 1) - r2) && Math.Round(kula3Z, 1) < Math.Round(kula2Z, 1) + r2))
                    {
                        czyDol3 = false;
                        czyGora3 = true;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula3Y, 1) == Math.Round(kula4Y, 1) + r4 && (Math.Round(kula3X, 1) > Math.Round(kula4X, 1) - r4 && (Math.Round(kula3X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula3Z, 1) > Math.Round(kula4Z, 1) - r4) && Math.Round(kula3Z, 1) < Math.Round(kula4Z, 1) + r4))
                    {
                        czyDol3 = false;
                        czyGora3 = true;
                    }

                }
                //przód tył
                if (czyPrzod3 == true && Math.Round(kula3Z, 1) <= front_site - r3)
                {
                    czyTyl3 = false;
                    kula3Z += time * time_skal3;
                    if (Math.Round(kula3Z, 1) >= front_site - r3)
                    {
                        czyTyl3 = true;
                        czyPrzod3 = false;
                    }
                    // warunek dla kuli 1
                    else if (Math.Round(kula3Z, 1) == Math.Round(kula1Z, 1) - r1 && (Math.Round(kula3X, 1) > Math.Round(kula1X, 1) - r1) && (Math.Round(kula3X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula3Y, 1) > Math.Round(kula1Y, 1) - r1 && Math.Round(kula3Y, 1) < Math.Round(kula1Y, 1) + r1))
                    {
                        czyTyl3 = true;
                        czyPrzod3 = false;
                    }
                    //warunek kuli 2 
                    else if (Math.Round(kula3Z, 1) == Math.Round(kula2Z, 1) - r2 && (Math.Round(kula3X, 1) > Math.Round(kula2X, 1) - r2) && (Math.Round(kula3X, 1) < Math.Round(kula2X, 1) + r2) && (Math.Round(kula3Y, 1) > Math.Round(kula2Y, 1) - r2 && Math.Round(kula3Y, 1) < Math.Round(kula2Y, 1) + r2))
                    {
                        czyTyl3 = true;
                        czyPrzod3 = false;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula3Z, 1) == Math.Round(kula4Z, 1) - r4 && (Math.Round(kula3X, 1) > Math.Round(kula4X, 1) - r4) && (Math.Round(kula3X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula3Y, 1) > Math.Round(kula4Y, 1) - r4 && Math.Round(kula3Y, 1) < Math.Round(kula4Y, 1) + r4))
                    {
                        czyTyl3 = true;
                        czyPrzod3 = false;
                    }
                }
                if (czyTyl3 == true && Math.Round(kula3Z, 1) >= back_site + r3)
                {
                    czyPrzod3 = false;
                    kula3Z -= time * time_skal3;
                    if (Math.Round(kula3Z, 1) <= back_site + r3)
                    {
                        czyTyl3 = false;
                        czyPrzod3 = true;
                    }
                    // warunek dla kuli 1
                    else if (Math.Round(kula3Z, 1) == Math.Round(kula1Z, 1) + r1 && (Math.Round(kula3X, 1) > Math.Round(kula1X, 1) - r1) && (Math.Round(kula3X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula3Y, 1) > Math.Round(kula1Y, 1) - r1 && Math.Round(kula3Y, 1) < Math.Round(kula1Y, 1) + r1))
                    {
                        czyTyl3 = false;
                        czyPrzod3 = true;
                    }
                    //warunek kuli 2 
                    else if (Math.Round(kula3Z, 1) == Math.Round(kula2Z, 1) + r2 && (Math.Round(kula3X, 1) > Math.Round(kula2X, 1) - r2) && (Math.Round(kula3X, 1) < Math.Round(kula2X, 1) + r2) && (Math.Round(kula3Y, 1) > Math.Round(kula2Y, 1) - r2 && Math.Round(kula3Y, 1) < Math.Round(kula2Y, 1) + r2))
                    {
                        czyTyl3 = false;
                        czyPrzod3 = true;
                    }
                    // warunek kuli 4
                    else if (Math.Round(kula3Z, 1) == Math.Round(kula4Z, 1) + r4 && (Math.Round(kula3X, 1) > Math.Round(kula4X, 1) - r4) && (Math.Round(kula3X, 1) < Math.Round(kula4X, 1) + r4) && (Math.Round(kula3Y, 1) > Math.Round(kula4Y, 1) - r4 && Math.Round(kula3Y, 1) < Math.Round(kula4Y, 1) + r4))
                    {
                        czyTyl3 = false;
                        czyPrzod3 = true;
                    }

                }
            Vector3 K3 = new Vector3(0 + kula3X, 0 + kula3Y, 0 + kula3Z);
            Gl.BindTexture(kolorK3);
            program["model_matrix"].SetValue(Matrix4.CreateTranslation(K3));
            Glut.glutSolidSphere(r3, 20, 20);


        }
        private static void kula4()
        {

                if (czyLewa4 == true && Math.Round(kula4X, 1) <= right_site - r4)
                {
                    czyPrawa4 = false;
                    kula4X += time * time_skal4;
                    if (Math.Round(kula4X, 1) >= right_site - r4)
                    {
                        czyPrawa4 = true;
                        czyLewa4 = false;
                    }
                    // warunek dla kuli 1
                    else if (Math.Round(kula4X, 1) == Math.Round(kula1X, 1) - r1 && ((Math.Round(kula4Y, 1) > Math.Round(kula1Y, 1) - r1) && (Math.Round(kula4Y, 1) < Math.Round(kula1Y, 1) + r1) && ((Math.Round(kula4Z, 1) > Math.Round(kula1Z, 1) - r1) && Math.Round(kula4Z, 1) < Math.Round(kula1Z, 1) + r1)))
                    {
                        czyPrawa4 = true;
                        czyLewa4 = false;
                    }
                    // warunek dla kuli 2
                    else if (Math.Round(kula4X, 1) == Math.Round(kula2X, 1) - r2 && ((Math.Round(kula4Y, 1) > Math.Round(kula2Y, 1) - r2) && (Math.Round(kula4Y, 1) < Math.Round(kula2Y, 1) + r2) && ((Math.Round(kula4Z, 1) > Math.Round(kula2Z, 1) - r2) && Math.Round(kula4Z, 1) < Math.Round(kula2Z, 1) + r2)))
                    {
                        czyPrawa4 = true;
                        czyLewa4 = false;
                    }
                    // warunek kuli 3
                    else if (Math.Round(kula4X, 1) == Math.Round(kula3X, 1) - r3 && ((Math.Round(kula4Y, 1) > Math.Round(kula3Y, 1) - r3) && (Math.Round(kula4Y, 1) < Math.Round(kula3Y, 1) + r3) && ((Math.Round(kula4Z, 1) > Math.Round(kula3Z, 1) - r3) && Math.Round(kula4Z, 1) < Math.Round(kula3Z, 1) + r3)))
                    {
                        czyPrawa4 = true;
                        czyLewa4 = false;
                    }
                }
                if (czyPrawa4 == true && Math.Round(kula4X, 1) >= left_site + r4)
                {
                    czyLewa4 = false;
                    kula4X -= time * time_skal4;
                    if (Math.Round(kula4X, 1) <= left_site + r4)
                    {
                        czyPrawa4 = false;
                        czyLewa4 = true;
                    }
                    // warunek dla kuli 1
                    else if (Math.Round(kula4X, 1) == Math.Round(kula1X, 1) + r1 && ((Math.Round(kula4Y, 1) > Math.Round(kula1Y, 1) - r1) && (Math.Round(kula4Y, 1) < Math.Round(kula1Y, 1) + r1) && ((Math.Round(kula4Z, 1) > Math.Round(kula1Z, 1) - r1) && Math.Round(kula4Z, 1) < Math.Round(kula1Z, 1) + r1)))
                    {
                        czyPrawa4 = false;
                        czyLewa4 = true;
                    }
                    // warunek dla kuli 2
                    else if (Math.Round(kula4X, 1) == Math.Round(kula2X, 1) + r2 && ((Math.Round(kula4Y, 1) > Math.Round(kula2Y, 1) - r2) && (Math.Round(kula4Y, 1) < Math.Round(kula2Y, 1) + r2) && ((Math.Round(kula4Z, 1) > Math.Round(kula2Z, 1) - r2) && Math.Round(kula4Z, 1) < Math.Round(kula2Z, 1) + r2)))
                    {
                        czyPrawa4 = false;
                        czyLewa4 = true;
                    }
                    // warunek kuli 3
                    else if (Math.Round(kula4X, 1) == Math.Round(kula3X, 1) + r3 && ((Math.Round(kula4Y, 1) > Math.Round(kula3Y, 1) - r3) && (Math.Round(kula4Y, 1) < Math.Round(kula3Y, 1) + r3) && ((Math.Round(kula4Z, 1) > Math.Round(kula3Z, 1) - r3) && Math.Round(kula4Z, 1) < Math.Round(kula3Z, 1) + r3)))
                    {
                        czyPrawa4 = false;
                        czyLewa4 = true;
                    }

                }

                //góra dół
                if (czyGora4 == true && Math.Round(kula4Y, 1) <= top_site - r4)
                {
                    czyDol4 = false;
                    kula4Y += time * time_skal4;
                    if (Math.Round(kula4Y, 1) >= top_site - r4)
                    {
                        czyDol4 = true;
                        czyGora4 = false;
                    }
                    //warunek dla kuli 1
                    else if (Math.Round(kula4Y, 1) == Math.Round(kula1Y, 1) - r1 && (Math.Round(kula4X, 1) > Math.Round(kula1X, 1) - r1 && (Math.Round(kula4X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula4Z, 1) > Math.Round(kula1Z, 1) - r1) && Math.Round(kula4Z, 1) < Math.Round(kula1Z, 1) + r1))
                    {
                        czyDol4 = true;
                        czyGora4 = false;
                    }
                    //warunek kuli 2 
                    else if (Math.Round(kula4Y, 1) == Math.Round(kula2Y, 1) - r2 && (Math.Round(kula4X, 1) > Math.Round(kula2X, 1) - r2 && (Math.Round(kula4X, 1) < Math.Round(kula2X, 1) + r2) && (Math.Round(kula4Z, 1) > Math.Round(kula2Z, 1) - r2) && Math.Round(kula4Z, 1) < Math.Round(kula2Z, 1) + r2))
                    {
                        czyDol4 = true;
                        czyGora4 = false;
                    }
                    // warunek kuli 3
                    else if (Math.Round(kula4Y, 1) == Math.Round(kula3Y, 1) - r3 && (Math.Round(kula4X, 1) > Math.Round(kula3X, 1) - r3 && (Math.Round(kula4X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula4Z, 1) > Math.Round(kula3Z, 1) - r3) && Math.Round(kula4Z, 1) < Math.Round(kula3Z, 1) + r3))
                    {
                        czyDol4 = true;
                        czyGora4 = false;
                    }
                }
                if (czyDol4 == true && Math.Round(kula4Y, 1) >= bottom_site + r4)
                {
                    czyGora4 = false;
                    kula4Y -= time * time_skal4;
                    if (Math.Round(kula4Y, 1) <= bottom_site + r4)
                    {
                        czyDol4 = false;
                        czyGora4 = true;
                    }
                    //warunek dla kuli 1
                    else if (Math.Round(kula4Y, 1) == Math.Round(kula1Y, 1) + r1 && (Math.Round(kula4X, 1) > Math.Round(kula1X, 1) - r1 && (Math.Round(kula4X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula4Z, 1) > Math.Round(kula1Z, 1) - r1) && Math.Round(kula4Z, 1) < Math.Round(kula1Z, 1) + r1))
                    {
                          czyDol4 = false;
                        czyGora4 = true;
                    }
                    //warunek kuli 2 
                    else if (Math.Round(kula4Y, 1) == Math.Round(kula2Y, 1) + r2 && (Math.Round(kula4X, 1) > Math.Round(kula2X, 1) - r2 && (Math.Round(kula4X, 1) < Math.Round(kula2X, 1) + r2) && (Math.Round(kula4Z, 1) > Math.Round(kula2Z, 1) - r2) && Math.Round(kula4Z, 1) < Math.Round(kula2Z, 1) + r2))
                    {
                          czyDol4 = false;
                          czyGora4 = true;

                    }
                    // warunek kuli 3
                    else if (Math.Round(kula4Y, 1) == Math.Round(kula3Y, 1) + r3 && (Math.Round(kula4X, 1) > Math.Round(kula3X, 1) - r3 && (Math.Round(kula4X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula4Z, 1) > Math.Round(kula3Z, 1) - r3) && Math.Round(kula4Z, 1) < Math.Round(kula3Z, 1) + r3))
                    {
                         czyDol4 = false;
                         czyGora4 = true;
                    }

                }
                //przód tył
                if (czyPrzod4 == true && Math.Round(kula4Z, 1) <= front_site - r4)
                {
                    czyTyl4 = false;
                    kula4Z += time * time_skal4;
                    if (Math.Round(kula4Z, 1) >= front_site - r4)
                    {
                        czyTyl4 = true;
                        czyPrzod4 = false;
                    }
                    // warunek dla kuli 1
                    else if (Math.Round(kula4Z, 1) == Math.Round(kula1Z, 1) - r1 && (Math.Round(kula4X, 1) > Math.Round(kula1X, 1) - r1) && (Math.Round(kula4X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula4Y, 1) > Math.Round(kula1Y, 1) - r1 && Math.Round(kula4Y, 1) < Math.Round(kula1Y, 1) + r1))
                    {
                        czyTyl4 = true;
                        czyPrzod4 = false;
                    }
                    //warunek kuli 2 
                    else if (Math.Round(kula4Z, 1) == Math.Round(kula2Z, 1) - r2 && (Math.Round(kula4X, 1) > Math.Round(kula2X, 1) - r2) && (Math.Round(kula4X, 1) < Math.Round(kula2X, 1) + r2) && (Math.Round(kula4Y, 1) > Math.Round(kula2Y, 1) - r2 && Math.Round(kula4Y, 1) < Math.Round(kula2Y, 1) + r2))
                    {
                        czyTyl4 = true;
                        czyPrzod4 = false;
                    }
                    // warunek kuli 3
                    else if (Math.Round(kula4Z, 1) == Math.Round(kula3Z, 1) - r3 && (Math.Round(kula4X, 1) > Math.Round(kula3X, 1) - r3) && (Math.Round(kula4X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula4Y, 1) > Math.Round(kula3Y, 1) - r3 && Math.Round(kula4Y, 1) < Math.Round(kula3Y, 1) + r3))
                    {
                        czyTyl4 = true;
                        czyPrzod4 = false;
                    }
                }
                if (czyTyl4 == true && Math.Round(kula4Z, 1) >= back_site + r4)
                {
                    czyPrzod4 = false;
                    kula4Z -= time * time_skal4;
                    if (Math.Round(kula4Z, 1) <= back_site + r4)
                    {
                        czyTyl4 = false;
                        czyPrzod4 = true;
                    }
                    // warunek dla kuli 1
                    else if (Math.Round(kula4Z, 1) == Math.Round(kula1Z, 1) + r1 && (Math.Round(kula4X, 1) > Math.Round(kula1X, 1) - r1) && (Math.Round(kula4X, 1) < Math.Round(kula1X, 1) + r1) && (Math.Round(kula4Y, 1) > Math.Round(kula1Y, 1) - r1 && Math.Round(kula4Y, 1) < Math.Round(kula1Y, 1) + r1))
                    {
                        czyTyl4 = false;
                        czyPrzod4 = true; ;
                    }
                    //warunek kuli 2 
                    else if (Math.Round(kula4Z, 1) == Math.Round(kula2Z, 1) + r2 && (Math.Round(kula4X, 1) > Math.Round(kula2X, 1) - r2) && (Math.Round(kula4X, 1) < Math.Round(kula2X, 1) + r2) && (Math.Round(kula4Y, 1) > Math.Round(kula2Y, 1) - r2 && Math.Round(kula4Y, 1) < Math.Round(kula2Y, 1) + r2))
                    {
                        czyTyl4 = false;
                        czyPrzod4 = true;
                    }
                    // warunek kuli 3
                    else if (Math.Round(kula4Z, 1) == Math.Round(kula3Z, 1) + r3 && (Math.Round(kula4X, 1) > Math.Round(kula3X, 1) - r3) && (Math.Round(kula4X, 1) < Math.Round(kula3X, 1) + r3) && (Math.Round(kula4Y, 1) > Math.Round(kula3Y, 1) - r3 && Math.Round(kula4Y, 1) < Math.Round(kula3Y, 1) + r3))
                    {
                        czyTyl4 = false;
                        czyPrzod4 = true;
                    }

                }
            Vector3 K4 = new Vector3(0 + kula4X, 0 + kula4Y, 0 + kula4Z); 
            Gl.BindTexture(kolorK4);
            program["model_matrix"].SetValue(Matrix4.CreateTranslation(K4));
            Glut.glutSolidSphere(r4, 20, 20);


        }
        
        private static void OnKeyboardDown(byte key,int x,int y)
        {
            if (key == 'w') angleY += 0.5f;
            else if (key == 's') angleY -= 0.5f;
            else if (key == 'a') angleX -= 0.5f;
            else if (key == 'd') angleX += 0.5f;
            else if (key == 'p') angleZ -= 0.5f;
            else if (key == 'l') angleZ += 0.5f;

            else if(key == '1')
            {
                if (timeOut1 == false) 
                {
                    time_skal1 += 0.1f;
                    if(time_skal1>=3f)
                    {
                        timeOut1 = true;
                    }
                }
                else if(timeOut1 == true)
                {
                    time_skal1 -= 0.1f;
                    if(time_skal1 <= 0.1f)
                    {
                        timeOut1 = false;
                    }

                }
 
            }
            else if(key == '2')
            {
                if (timeOut2 == false)
                {
                    time_skal2 += 0.1f;
                    if (time_skal2 >= 3f)
                    {
                        timeOut2 = true;
                    }
                }
                else if (timeOut2 == true)
                {
                    time_skal2 -= 0.1f;
                    if (time_skal2 <= 0.1f)
                    {
                        timeOut2 = false;
                    }

                }
            }
            else if(key == '3')
            {
                if (timeOut3 == false)
                {
                    time_skal3 += 0.1f;
                    if (time_skal3 >= 3f)
                    {
                        timeOut3 = true;
                    }
                }
                else if (timeOut3 == true)
                {
                    time_skal3 -= 0.1f;
                    if (time_skal3 <= 0.1f)
                    {
                        timeOut3 = false;
                    }

                }
            }
            else if (key=='4')
            {
                if (timeOut4 == false)
                {
                    time_skal4 += 0.1f;
                    if (time_skal4 >= 3f)
                    {
                        timeOut4 = true;
                    }
                }
                else if (timeOut4 == true)
                {
                    time_skal4 -= 0.1f;
                    if (time_skal4 <= 0.1f)
                    {
                        timeOut4 = false;
                    }

                }
            }
        }
        private static void OnKeyboardUp(byte key, int x, int y)
        {

            
        }
        public static string VertexShader = @"
#version 330

in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexUV;



out vec3 normal;
out vec2 uv;


uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
        {
            normal = normalize((model_matrix * vec4(floor(vertexNormal), 0)).xyz);
            uv = vertexUV;
            

            gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
        }
";
        public static string FragmentShader = @"
#version 330

uniform vec3 light_direction;
uniform sampler2D texture;


in vec3 normal;
in vec2 uv;

out vec4 fragment;
 
void main(void)
        {   
                float diffuse = max(dot(normal, light_direction), 0);
                float ambient = 0.5;
                float lighting = max(diffuse,ambient);
               fragment = vec4(lighting * texture2D(texture, uv).xyz, 0.5);
               // fragment =  vec4(texture2D(texture, uv).xyz,0.3);
   
        }


";

    }

}
