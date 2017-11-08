using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;

namespace KinMatematica
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Skeleton jogador;

        //kinect
        KinectSensor kinect;
        bool KinectDetectado = false, UsandoKinect = false;

        //Funciona o esqueleto
        Skeleton[] skeletons;
        Skeleton trackedSkeleton1;

        //fontes
        SpriteFont sfontPequena, sfontGrande, sFontCustom, sfontCourierPequena, sfontOpcao,sFontMedia;

        //imagens estáticas
        Texture2D mAberturaFundo, mLousa, mBloco, mIncorreto, mCorreto,mCorpo;

        //objetos
        clsObjeto mCursorDireita, mCursorEsquerda;
        clsObjeto mArea1, mArea2, mArea3;

        //jogo
        float Cena = 1;
        int Fase = 0, _Pergunta = 1, _CenaStatus = 1, _OpcaoSelecionada = 0, _OpcaoCorreta = 0;
        string _Calculo = "";
        List<string> _ResultadoFinal = new List<string>();
        int _ResultadoFinalErro = 0, _ResultadoFinalAcerto = 0;
        float ResultadoErrado1, ResultadoErrado2;
        float _CalculoResultado = 0, _OpcaoMostrarTela1, _OpcaoMostrarTela2, _OpcaoMostrarTela3;
        TimeSpan _tempoMensagem;




        //pressionou tecla
        KeyboardState oldState;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            jogoIniciarSistema();

            //Inicializa o sensor
            try
            {
                kinect = KinectSensor.KinectSensors[0];
                //Inicializa o esqueleto
                kinect.SkeletonStream.Enable();
                //Inicia tudo
                kinect.Start();

                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
                KinectDetectado = true;
            }
            catch
            {
                KinectDetectado = true;
            }

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //fonte
            sfontPequena = Content.Load<SpriteFont>("SpriteFont1");
            sFontMedia = Content.Load<SpriteFont>("SpriteFontMedia");
            
            sfontGrande = Content.Load<SpriteFont>("SpriteFontGrande");
            sfontOpcao = Content.Load<SpriteFont>("SpriteFontOpcao");
            sfontCourierPequena = Content.Load<SpriteFont>("SpriteFontCourierPequena");
            sFontCustom = Content.Load<SpriteFont>(@"myCustomFont");


            //imagens
            mAberturaFundo = this.Content.Load<Texture2D>("AberturaFundo");
            mLousa = this.Content.Load<Texture2D>("Lousa");

            mBloco = this.Content.Load<Texture2D>("Bloco");

            mIncorreto = this.Content.Load<Texture2D>("Incorreto");
            mCorreto = this.Content.Load<Texture2D>("Correto");
            mCorpo = this.Content.Load<Texture2D>("Corpo");

            //objetos
            mCursorDireita = new clsObjeto(this, Content.Load<Texture2D>("Cursor"), new Vector2(38f, 36f), new Vector2(20f, 20f), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            mCursorEsquerda = new clsObjeto(this, Content.Load<Texture2D>("Cursor"), new Vector2(38f, 36f), new Vector2(20f, 20f), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            mArea1 = new clsObjeto(this, Content.Load<Texture2D>("Opcao"), new Vector2(1f, 1f), new Vector2(1f, 1f), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            mArea2 = new clsObjeto(this, Content.Load<Texture2D>("Opcao"), new Vector2(1f, 1f), new Vector2(1f, 1f), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            mArea3 = new clsObjeto(this, Content.Load<Texture2D>("Opcao"), new Vector2(1f, 1f), new Vector2(1f, 1f), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);


        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            kinect.Stop();
        }
        
        public float convPontoRealX(float Ponto)
        {
            //converte o PONTO em relação a posição do KINECT
            float max, conv, total;

            Ponto = Ponto * 10;

            conv = graphics.PreferredBackBufferWidth / 6;

            max = 3;
            total = max + Ponto;
            total = total * conv;
            
            return (total);
        }

        public float convPontoRealY(float Ponto)
        {
            //converte o PONTO em relação a posição do KINECT
            float max, conv, total;

            Ponto = Ponto * -1;
            Ponto = Ponto * 10;

            conv = graphics.PreferredBackBufferHeight / 6;

            max = 7;
            total = max + Ponto;
            total = conv * total;

            return (total);
        }

        public float convPontoRealCorpoX(float Centro, float Ponto)
        {
            float conv, total;
            int pxCentro, pxPonto;
            //Tamanho da tela=graphics.PreferredBackBufferWidth
            //6= onde do centro até o máximo que consigo chegar na tela é 6 pontos(unidade do kinect)
            //a cada 1 ponto do kinect conv é a qtde em pixels que deve andar

            Centro = Centro * 10;
            Ponto = Ponto * 10;
            //14 = 10 para a conversão do ponto e 4 é a métrica do ponto para chegar até o canto da tela
            conv = graphics.PreferredBackBufferWidth / 14;

            pxPonto = (int)(conv * Ponto);
            pxCentro = (int)(conv * Centro);

            total = (graphics.PreferredBackBufferWidth / 2) - (pxCentro - pxPonto);

            return (total);
        }

        public float convPontoRealCorpoYMaos(int tamanhoVertical, float Joelho, float Ombro, float Ponto)
        {
            float total, alturaMaxima;

            Joelho = Joelho * 10;
            Ombro = Ombro * 10;
            Ponto = (Ponto * 10) * -1;

            alturaMaxima = (Joelho + (float)2.5) - Ombro;
            if (alturaMaxima < 0)
            {
                alturaMaxima *= -1;
            }
            total = Ponto - ((alturaMaxima + Ombro) * -1);
            //70=qtde de pixels
            return (total * 70);
            
        }

        void kinSetPosicaoCursorJogador(Skeleton jogador)
        {
            if (jogador != null)
            {
                mCursorDireita.position = new Vector2(convPontoRealCorpoX(jogador.Joints[JointType.Spine].Position.X, jogador.Joints[JointType.HandRight].Position.X) - mCursorDireita.size.X, convPontoRealCorpoYMaos(graphics.PreferredBackBufferHeight, jogador.Joints[JointType.KneeRight].Position.Y, jogador.Joints[JointType.ShoulderRight].Position.Y, jogador.Joints[JointType.HandRight].Position.Y));
                mCursorEsquerda.position = new Vector2(convPontoRealCorpoX(jogador.Joints[JointType.Spine].Position.X, jogador.Joints[JointType.HandLeft].Position.X) - mCursorEsquerda.size.X, convPontoRealCorpoYMaos(graphics.PreferredBackBufferHeight, jogador.Joints[JointType.KneeLeft].Position.Y, jogador.Joints[JointType.ShoulderLeft].Position.Y, jogador.Joints[JointType.HandLeft].Position.Y));
                
            }
        }

        Boolean kinPosicaoMaoAcimaCabeca(Skeleton jogador)
        {

            Boolean retorno = false;
            if (jogador != null)
            {
                if (jogador.Joints[JointType.HandRight].Position.Y > jogador.Joints[JointType.Head].Position.Y)
                {
                    UsandoKinect = true;
                    retorno = true;
                }
            }

            return (retorno);
        }
        
        void jogoSelecionarNivel()
        {
            Cena = 2;
            _tempoMensagem = TimeSpan.Zero;//zera para escolher o nivel
            
        }
        
        void jogoIniciar()
        {
            Cena = 4;

            _tempoMensagem = TimeSpan.Zero;//zera para dar um tempo antes de pedir a primeira pergunta
            jogoPerguntaCriar();
        }

        void jogoPerguntaPreencherOpcoes()
        {
            float ResultadoCorreto = _CalculoResultado;
            Random rnd = new Random(DateTime.Now.Millisecond);

            //cria a opção correta
            _OpcaoCorreta = rnd.Next(1, 4);
            if (_OpcaoCorreta == 1)
            {
                _OpcaoMostrarTela1 = ResultadoCorreto;
                _OpcaoMostrarTela2 = ResultadoErrado1;
                _OpcaoMostrarTela3 = ResultadoErrado2;
            }
            else if (_OpcaoCorreta == 2)
            {
                _OpcaoMostrarTela1 = ResultadoErrado1;
                _OpcaoMostrarTela2 = ResultadoCorreto;
                _OpcaoMostrarTela3 = ResultadoErrado2;
            }
            else
            {
                _OpcaoMostrarTela1 = ResultadoErrado1;
                _OpcaoMostrarTela2 = ResultadoErrado2;
                _OpcaoMostrarTela3 = ResultadoCorreto;

            }


        }

        void jogoPerguntaCriar()
        {
            Boolean fim_do_jogo = false;
            float v1, v2;

            Random rnd = new Random(DateTime.Now.Millisecond);
            //fica a fase e pergunta fixa\
            //Fase =2;
            //_Pergunta = 4;
            if (Fase == 1)
            {
                //soma com 1 casa decimal
                if (_Pergunta == 1)
                {
                    //não inclui o último valor
                    v1 = rnd.Next(1, 10);
                    v2 = rnd.Next(1, 10);
                    _Calculo = v1.ToString() + "+" + v2.ToString();
                    _CalculoResultado = v1 + v2;
                    ResultadoErrado1 = _CalculoResultado - 1;
                    ResultadoErrado2 = _CalculoResultado + 1;
                }
                //soma com 1 casa decimal
                else if (_Pergunta == 2)
                {
                    //não inclui o último valor
                    v1 = rnd.Next(1, 10);
                    v2 = rnd.Next(1, 10);
                    _Calculo = v1.ToString() + "+" + v2.ToString();
                    _CalculoResultado = v1 + v2;
                    ResultadoErrado1 = _CalculoResultado - 1;
                    ResultadoErrado2 = _CalculoResultado + 1;
                }
                //subtração com 1 casa decimal
                else if (_Pergunta == 3)
                {
                    v1 = rnd.Next(1, 10);
                    v2 = rnd.Next(1, (int)v1 + 1);
                    _Calculo = v1.ToString() + "-" + v2.ToString();
                    _CalculoResultado = v1 - v2;
                    ResultadoErrado1 = _CalculoResultado - 1;
                    ResultadoErrado2 = _CalculoResultado + 1;
                }
                //subtração com 1 casa decimal
                else if (_Pergunta == 4)
                {
                    v1 = rnd.Next(1, 10);
                    v2 = rnd.Next(1, (int)v1 + 1);
                    _Calculo = v1.ToString() + "-" + v2.ToString();
                    _CalculoResultado = v1 - v2;
                    ResultadoErrado1 = _CalculoResultado - 1;
                    ResultadoErrado2 = _CalculoResultado + 1;
                }
                //multiplicação com 1 casa decimal
                else if (_Pergunta == 5)
                {
                    v1 = rnd.Next(1, 10);
                    v2 = rnd.Next(0, 10);
                    _Calculo = v1.ToString() + "*" + v2.ToString();
                    _CalculoResultado = v1 * v2;
                    ResultadoErrado1 = v1 * (v2 - 1);
                    ResultadoErrado2 = v1 * (v2 + 1);

                }
                //multiplicação com 1 casa decimal
                else if (_Pergunta == 6)
                {
                    v1 = rnd.Next(1, 10);
                    v2 = rnd.Next(0, 10);
                    _Calculo = v1.ToString() + "*" + v2.ToString();
                    _CalculoResultado = v1 * v2;
                    ResultadoErrado1 = v1 * (v2 - 1);
                    ResultadoErrado2 = v1 * (v2 + 1);

                }
                //divisão com 1 casa decimal
                else if (_Pergunta == 7)
                {
                    v1 = rnd.Next(1, 10);
                    v2 = rnd.Next(1, 10);
                    ResultadoErrado1 = v1 - 1;
                    ResultadoErrado2 = v1 + 1;
                    v1 = v1 * v2;
                    _Calculo = v1.ToString() + "/" + v2.ToString();
                    _CalculoResultado = v1 / v2;
                }
                //divisão com 1 casa decimal
                else if (_Pergunta == 8)
                {
                    v1 = rnd.Next(1, 10);
                    v2 = rnd.Next(1, 10);
                    ResultadoErrado1 = v1 - 1;
                    ResultadoErrado2 = v1 + 1;
                    v1 = v1 * v2;
                    _Calculo = v1.ToString() + "/" + v2.ToString();
                    _CalculoResultado = v1 / v2;
                }
                else
                {
                    fim_do_jogo = true;
                }                
            }
            else if (Fase == 2)
            {
                //soma com 2 casas decimais
                if (_Pergunta == 1)
                {
                    //não inclui o último valor
                    v1 = rnd.Next(1, 100);
                    v2 = rnd.Next(1, 100);
                    _Calculo = v1.ToString() + "+" + v2.ToString();
                    _CalculoResultado = v1 + v2;
                    ResultadoErrado1 = _CalculoResultado - 10;
                    ResultadoErrado2 = _CalculoResultado + 10;
                }
                //soma com 2 casas decimais
                else if (_Pergunta == 2)
                {
                    //não inclui o último valor
                    v1 = rnd.Next(1, 100);
                    v2 = rnd.Next(1, 100);
                    _Calculo = v1.ToString() + "+" + v2.ToString();
                    _CalculoResultado = v1 + v2;
                    ResultadoErrado1 = _CalculoResultado - 10;
                    ResultadoErrado2 = _CalculoResultado + 10;
                }
                //subtração com 2 casas decimais
                else if (_Pergunta == 3)
                {
                    v1 = rnd.Next(1, 100);
                    v2 = rnd.Next(1, (int)v1 + 1);
                    _Calculo = v1.ToString() + "-" + v2.ToString();
                    _CalculoResultado = v1 - v2;
                    ResultadoErrado1 = _CalculoResultado - 10;
                    ResultadoErrado2 = _CalculoResultado + 10;
                }
                //subtração com 2 casas decimais
                else if (_Pergunta == 4)
                {
                    v1 = rnd.Next(1, 100);
                    v2 = rnd.Next(1, (int)v1 + 1);
                    _Calculo = v1.ToString() + "-" + v2.ToString();
                    _CalculoResultado = v1 - v2;
                    ResultadoErrado1 = _CalculoResultado - 10;
                    ResultadoErrado2 = _CalculoResultado + 10;
                }
                //multiplicação com 2 casas decimais no primeiro valor
                else if (_Pergunta == 5)
                {
                    v1 = rnd.Next(1, 100);
                    v2 = rnd.Next(01, 10);
                    _Calculo = v1.ToString() + "*" + v2.ToString();
                    _CalculoResultado = v1 * v2;
                    ResultadoErrado1 = v1 * (v2 - 1);
                    ResultadoErrado2 = v1 * (v2 + 1);
                }
                //multiplicação com 2 casas decimais no primeiro valor
                else if (_Pergunta == 6)
                {
                    v1 = rnd.Next(1, 100);
                    v2 = rnd.Next(01, 10);
                    _Calculo = v1.ToString() + "*" + v2.ToString();
                    _CalculoResultado = v1 * v2;
                    ResultadoErrado1 = v1 * (v2 - 1);
                    ResultadoErrado2 = v1 * (v2 + 1);
                }
                //divisão com 2 casas decimais no dividendos
                else if (_Pergunta == 7)
                {
                    v1 = rnd.Next(1, 21);
                    v2 = rnd.Next(1, 21);
                    ResultadoErrado1 = v1 - 1;
                    ResultadoErrado2 = v1 + 1;
                    v1 = v1 * v2;
                    _Calculo = v1.ToString() + "/" + v2.ToString();
                    _CalculoResultado = v1 / v2;
                }
                //divisão com 2 casas decimais no dividendos
                else if (_Pergunta == 8)
                {
                    v1 = rnd.Next(1, 21);
                    v2 = rnd.Next(1, 21);
                    ResultadoErrado1 = v1 - 1;
                    ResultadoErrado2 = v1 + 1;
                    v1 = v1 * v2;
                    _Calculo = v1.ToString() + "/" + v2.ToString();
                    _CalculoResultado = v1 / v2;
                }
                else
                {
                    fim_do_jogo = true;
                }
            }
            else if (Fase == 3)
            {
                //soma com 3 casas decimais
                if (_Pergunta == 1)
                {
                    //não inclui o último valor
                    v1 = rnd.Next(1, 1000);
                    v2 = rnd.Next(1, 1000);
                    _Calculo = v1.ToString() + "+" + v2.ToString();
                    _CalculoResultado = v1 + v2;
                    ResultadoErrado1 = _CalculoResultado - 10;
                    ResultadoErrado2 = _CalculoResultado + 10;
                }
                else if (_Pergunta == 2) //soma com 3 casas decimais
                {
                    //não inclui o último valor
                    v1 = rnd.Next(1, 1000);
                    v2 = rnd.Next(1, 1000);
                    _Calculo = v1.ToString() + "+" + v2.ToString();
                    _CalculoResultado = v1 + v2;
                    ResultadoErrado1 = _CalculoResultado - 10;
                    ResultadoErrado2 = _CalculoResultado + 10;
                }
                //subtração com 2 casas decimais
                else if (_Pergunta == 3)
                {
                    v1 = rnd.Next(1, 1000);
                    v2 = rnd.Next(1, (int)v1 + 1);
                    _Calculo = v1.ToString() + "-" + v2.ToString();
                    _CalculoResultado = v1 - v2;
                    ResultadoErrado1 = _CalculoResultado - 10;
                    ResultadoErrado2 = _CalculoResultado + 10;
                }
                //subtração com 2 casas decimais
                else if (_Pergunta == 4)
                {
                    v1 = rnd.Next(1, 1000);
                    v2 = rnd.Next(1, (int)v1 + 1);
                    _Calculo = v1.ToString() + "-" + v2.ToString();
                    _CalculoResultado = v1 - v2;
                    ResultadoErrado1 = _CalculoResultado - 10;
                    ResultadoErrado2 = _CalculoResultado + 10;
                }
                //multiplicação com 2 casas decimais no primeiro valor
                else if (_Pergunta == 5)
                {
                    v1 = rnd.Next(1, 1000);
                    v2 = rnd.Next(01, 10);
                    _Calculo = v1.ToString() + "*" + v2.ToString();
                    _CalculoResultado = v1 * v2;
                    ResultadoErrado1 = v1 * (v2 - 1);
                    ResultadoErrado2 = v1 * (v2 + 1);
                }
                //multiplicação com 2 casas decimais no primeiro valor
                else if (_Pergunta == 6)
                {
                    v1 = rnd.Next(1, 1000);
                    v2 = rnd.Next(01, 10);
                    _Calculo = v1.ToString() + "*" + v2.ToString();
                    _CalculoResultado = v1 * v2;
                    ResultadoErrado1 = v1 * (v2 - 1);
                    ResultadoErrado2 = v1 * (v2 + 1);
                }
                //divisão com 2 casas decimais no dividendos
                else if (_Pergunta == 7)
                {
                    v1 = rnd.Next(1, 100);
                    v2 = rnd.Next(1, 21);
                    ResultadoErrado1 = v1 - 1;
                    ResultadoErrado2 = v1 + 1;
                    v1 = v1 * v2;
                    _Calculo = v1.ToString() + "/" + v2.ToString();
                    _CalculoResultado = v1 / v2;

                }
                //divisão com 2 casas decimais no dividendos
                else if (_Pergunta == 8)
                {
                    v1 = rnd.Next(1, 100);
                    v2 = rnd.Next(1, 21);
                    ResultadoErrado1 = v1 - 1;
                    ResultadoErrado2 = v1 + 1;
                    v1 = v1 * v2;
                    _Calculo = v1.ToString() + "/" + v2.ToString();
                    _CalculoResultado = v1 / v2;

                }
                else
                {
                    fim_do_jogo = true;
                }
            }

            else
            {
                fim_do_jogo = true;
            }

            //mostra a pergunta na tela
            if (fim_do_jogo == false)
            {
                _Pergunta++;

                jogoPerguntaPreencherOpcoes();
                jogoOpcaoMostrar();
                _OpcaoSelecionada = 0;
                _CenaStatus = 1;
            }
            else
            {
                Cena = 10;
                _CenaStatus = 3;

            }
        }

        void jogoOpcaoMostrar()
        {
            mArea1.size = new Vector2(200, 600);//esquerda
            mArea2.size = new Vector2(630, 200);//cima
            mArea3.size = new Vector2(200, 600);//direita
            mArea1.position = new Vector2(0, 100);
            mArea2.position = new Vector2(200, 0);
            mArea3.position = new Vector2(830, 100);
        }

        void jogoOpcaoMostrarMaosBaixo()
        {
            mArea1.size = new Vector2(400, 400);
            mArea1.position = new Vector2(300, 500);
        }

        bool jogoOpcaoMaosBaixo()
        {
            Boolean Retorno = false;
            if (UsandoKinect == true)
            {
                //se o usuário está com as 2 mãos para baixo
                if (mCursorDireita.ObjetoColisao(mArea1) && mCursorEsquerda.ObjetoColisao(mArea1))
                {
                    _OpcaoSelecionada = 1;
                    Retorno = true;
                }
            }
            else
            {
                Retorno = true;
            }
            return (Retorno);
        }

        bool jogoOpcaoSelecionou()
        {
            Boolean Retorno = false;
            KeyboardState ks = Keyboard.GetState();
            if (UsandoKinect == true)
            {
                if (mCursorDireita.ObjetoColisao(mArea1) || mCursorEsquerda.ObjetoColisao(mArea1))
                {
                    _OpcaoSelecionada = 1;
                    Retorno = true;
                }
                else if (mCursorDireita.ObjetoColisao(mArea2) || mCursorEsquerda.ObjetoColisao(mArea2))
                {
                    _OpcaoSelecionada = 2;
                    Retorno = true;
                }
                else if (mCursorDireita.ObjetoColisao(mArea3) || mCursorEsquerda.ObjetoColisao(mArea3))
                {
                    _OpcaoSelecionada = 3;
                    Retorno = true;
                }
                else
                {
                    Retorno = false;
                }
            }
            else
            {
                //se está pressionado a tecla que seleciona a opção
                if (ks.IsKeyDown(Keys.Left))
                {
                    //então verifica se o usuário soltou a tecla
                    if (!oldState.IsKeyDown(Keys.Left))
                    {
                        _OpcaoSelecionada = 1;
                        Retorno = true;
                    }
                }
                else if (ks.IsKeyDown(Keys.Up))
                {
                    if (!oldState.IsKeyDown(Keys.Up))
                    {
                        _OpcaoSelecionada = 2;
                        Retorno = true;
                    }
                }
                else if (ks.IsKeyDown(Keys.Right))
                {
                    if (!oldState.IsKeyDown(Keys.Right))
                    {
                        _OpcaoSelecionada = 3;
                        Retorno = true;
                    }
                }
                else
                {
                    Retorno = false;
                }
            }



            // Update saved state.
            oldState = ks;

            return (Retorno);
        }

        void jogoIniciarSistema()
        {
            Cena = 1;
            Fase = 0;
            _Pergunta = 1;
            _CenaStatus = 1;
            _OpcaoSelecionada = 0;
            _OpcaoCorreta = 0;
            _Calculo = "";
            _ResultadoFinal.Clear();
            _ResultadoFinalErro = 0;
            _ResultadoFinalAcerto = 0;
            _CalculoResultado = 0;
            _OpcaoMostrarTela1 = 0;
            _OpcaoMostrarTela2 = 0;
            _OpcaoMostrarTela3 = 0;
        }

        void jogoOpcaoVerificaAcertou()
        {
            string temp;

            temp = _Calculo + "=" + _CalculoResultado;
            if (_OpcaoSelecionada == _OpcaoCorreta)
            {
                temp += " Acertou ";
                _ResultadoFinalAcerto++;
            }
            else
            {
                temp += " Errou ";
                _ResultadoFinalErro++;
            }

            _ResultadoFinal.Add(temp);

            //depois que selecionou a opção, exibe uma mensagem entre a pergunta selecionada e a próxima pergunta
            _CenaStatus = 2;

            _tempoMensagem = TimeSpan.Zero;
        }

        Boolean jogoMensagemJogador(int tempo)
        {
            if (_tempoMensagem >= TimeSpan.FromSeconds(tempo))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        void jogoOpcaoMensagem()
        {
            if (_tempoMensagem >= TimeSpan.FromSeconds(3))
            {
                jogoOpcaoMostrarMaosBaixo(); 
                if (jogoOpcaoMaosBaixo() == true)
                {
                    //passa para a próxima pergunta
                    _CenaStatus = 3;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState ks = Keyboard.GetState();

            //Se não foi encontrado o jogador, então reinicia o jogo
            if (trackedSkeleton1==null)
            {
                jogoIniciarSistema();
            }

            //como o usuário consegui pegar X else Y 
            kinSetPosicaoCursorJogador(trackedSkeleton1);

            _tempoMensagem += gameTime.ElapsedGameTime;

            if (Cena == 1)//menu
            {
                if (kinPosicaoMaoAcimaCabeca(trackedSkeleton1) == true)
                {
                    jogoSelecionarNivel();
                }

                if (ks.IsKeyDown(Keys.Enter))
                {
                    //então verifica se o usuário soltou a tecla
                    
                    if (!oldState.IsKeyDown(Keys.Enter))
                    {
                        jogoSelecionarNivel();
                    }
                }
                else if (ks.IsKeyDown(Keys.Up))
                {
                    //então verifica se o usuário soltou a tecla
                    if (!oldState.IsKeyDown(Keys.Up))
                    {
                        try
                        {
                            kinect.ElevationAngle = kinect.ElevationAngle + 3;
                        }
                        catch { }
                    }

                }
                else if (ks.IsKeyDown(Keys.Down))
                {
                    //então verifica se o usuário soltou a tecla
                    if (!oldState.IsKeyDown(Keys.Down))
                    {
                        try
                        {
                            kinect.ElevationAngle = kinect.ElevationAngle - 3;
                        }
                        catch { }
                    }

                }


            }
            else if (Cena == 2)//mensagem para selecionar o nivel
            {
                jogoOpcaoMostrarMaosBaixo();
                if (jogoOpcaoMaosBaixo() == true)
                {
                    Cena = 3;
                }
            }
            else if (Cena == 3)//seleciona o nivel
            {
                jogoOpcaoMostrar();
                if (jogoOpcaoSelecionou() == true)
                {
                    Fase = _OpcaoSelecionada;
                    jogoIniciar();
                }

            }
            else if (Cena == 4)//Prepara o jogador
            {
                jogoOpcaoMostrarMaosBaixo();
                if (jogoOpcaoMaosBaixo() == true)
                {
                    Cena = 5;
                    _CenaStatus = 1;
                }
                /*
                if (jogoMensagemJogador(5) == true)
                {
                    Cena = 5;
                }
                 */
            }
            else if (Cena == 5)//jogando
            {

                if (_CenaStatus == 1)//mostrando a opçao
                {
                    if (jogoOpcaoMaosBaixo() == false)
                    {
                        if (jogoOpcaoSelecionou() == true)
                        {
                            jogoOpcaoVerificaAcertou();
                        }
                    }
                }
                else if (_CenaStatus == 2)//mostra mensagem após selecionar uma opção
                {
                    jogoOpcaoMensagem();
                }
                else//próxima pergunta
                {
                    jogoPerguntaCriar();
                }

            }
            else if (Cena == 10)
            {
                if (UsandoKinect == true)
                {
                    if (kinPosicaoMaoAcimaCabeca(trackedSkeleton1) == true)
                    {
                        jogoIniciarSistema();
                    }
                }
                else
                {
                    if (ks.IsKeyDown(Keys.Enter))
                    {
                        //então verifica se o usuário soltou a tecla
                        if (!oldState.IsKeyDown(Keys.Enter))
                        {
                            jogoIniciarSistema();
                        }
                    }
                }

            }
            //só volta se soltar o botão
            oldState = ks;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Draw(mAberturaFundo, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            
            if (Cena == 1)
            {
                spriteBatch.DrawString(sfontGrande, "Jogo de Matemática", new Vector2(140, 40), Color.Blue);


                if (KinectDetectado == true)
                {
                    if (trackedSkeleton1 == null)
                    {
                        spriteBatch.DrawString(sfontGrande, "Nenhum jogador na tela", new Vector2(60, 300), Color.Red);
                    }
                    else
                    {
                        if (trackedSkeleton1 != null)
                        {
                            spriteBatch.DrawString(sfontGrande, "Olá, jogador", new Vector2(300, 230), Color.Red);
                            spriteBatch.DrawString(sfontGrande, "Coloque a mão DIREITA", new Vector2(50, 320), Color.Black);
                            spriteBatch.DrawString(sfontGrande, "acima da cabeça para jogar", new Vector2(10, 390), Color.Black);
                        }

                    }
                }
                else
                {
                    spriteBatch.DrawString(sfontGrande, "KINECT não detectado", new Vector2(100, 300), Color.Black);


                }
                spriteBatch.DrawString(sfontGrande, "ENTER inicia com o teclado", new Vector2(10, 600), Color.Black);
            }
            else if (Cena == 2)//mensagem seleciona o nivel
            {
                spriteBatch.Draw(mLousa, new Rectangle(140, 140, 746, 490), Color.White);
                spriteBatch.DrawString(sfontGrande, "Coloque as mãos", new Vector2(200, 200), Color.White);
                spriteBatch.DrawString(sfontGrande, "igual o desenho", new Vector2(220, 300), Color.White);
                spriteBatch.DrawString(sfontGrande, "ao lado", new Vector2(220, 400), Color.White);

                spriteBatch.Draw(mCorpo, new Rectangle(820, 140, 173, 480), Color.White);
            }
            else if (Cena == 3)//Seleciona o nivel
            {
                if (UsandoKinect == true)
                {
                    spriteBatch.DrawString(sfontGrande, "Coloque a", new Vector2(240, 300), Color.Black);
                    spriteBatch.DrawString(sfontGrande, "mão no nível", new Vector2(280, 400), Color.Black);
                    spriteBatch.DrawString(sfontGrande, "de dificuldade", new Vector2(230, 500), Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(sfontGrande, "Selecione a dificuldade", new Vector2(120, 600), Color.Black);
                }

                spriteBatch.Draw(mBloco, new Rectangle(0, 300, 200, 220), Color.White);
                spriteBatch.Draw(mBloco, new Rectangle(414, 0, 200, 220), Color.White);
                spriteBatch.Draw(mBloco, new Rectangle(824, 300, 200, 220), Color.White);


                spriteBatch.DrawString(sfontOpcao, "Fácil", new Vector2(30, 340), Color.Black);
                spriteBatch.DrawString(sfontOpcao, "Médio", new Vector2(425, 40), Color.Black);
                spriteBatch.DrawString(sfontOpcao, "Difícil", new Vector2(840, 340), Color.Black);

            }
            else if (Cena == 4)//Prepara o jogador
            {
                spriteBatch.Draw(mLousa, new Rectangle(140, 140, 746, 490), Color.White);
                spriteBatch.DrawString(sfontGrande, "Prepare-se", new Vector2(270, 250), Color.White);
                if (UsandoKinect == true)
                {
                    spriteBatch.DrawString(sFontMedia , "Coloque as mãos igual", new Vector2(220, 400), Color.White);
                    spriteBatch.DrawString(sFontMedia, "o menino do desenho do lado!", new Vector2(200, 450), Color.White);
                }

                spriteBatch.Draw(mCorpo, new Rectangle(820, 140, 173, 480), Color.White);

            }
            else if (Cena == 5)
            {
                spriteBatch.Draw(mLousa, new Rectangle(140, 140, 746, 490), Color.White);
                if (_CenaStatus == 1)
                {
                    spriteBatch.DrawString(sfontPequena, "Calcule:", new Vector2(220, 200), Color.White);

                    spriteBatch.DrawString(sfontGrande, _Calculo.ToString(), new Vector2(220, 280), Color.White);
                    jogoOpcaoMostrar();

                    spriteBatch.Draw(mBloco, new Rectangle(0, 300, 170, 190), Color.White);
                    spriteBatch.Draw(mBloco, new Rectangle(414, 0, 170, 190), Color.White);
                    spriteBatch.Draw(mBloco, new Rectangle(854, 300, 170, 190), Color.White);

                    spriteBatch.DrawString(sfontOpcao, _OpcaoMostrarTela1.ToString(), new Vector2(5, 340), Color.Black);
                    spriteBatch.DrawString(sfontOpcao, _OpcaoMostrarTela2.ToString(), new Vector2(420, 40), Color.Black);
                    spriteBatch.DrawString(sfontOpcao, _OpcaoMostrarTela3.ToString(), new Vector2(860, 340), Color.Black);

                }
                else if (_CenaStatus == 2)
                {
                    //Verifica se a resposta está certa
                    if (_OpcaoSelecionada == _OpcaoCorreta)
                    {
                        spriteBatch.DrawString(sfontPequena, "Resposta correta! " + _Calculo.ToString() + " = " + _CalculoResultado, new Vector2(220, 200), Color.White);

                        spriteBatch.Draw(mCorreto, new Rectangle(400, 270, 250, 250), Color.White);
                    }
                    else
                    {
                        String texto;
                        texto=_Calculo.ToString() + " = " + _CalculoResultado.ToString() + " e não ";
                        if (_OpcaoSelecionada==1){
                            texto+=_OpcaoMostrarTela1;
                        } else if (_OpcaoSelecionada==2){
                            texto+=_OpcaoMostrarTela2;
                        } else {
                            texto+=_OpcaoMostrarTela3;
                        }

                        spriteBatch.DrawString(sfontPequena, "Resposta incorreta: " + texto, new Vector2(220, 200), Color.White);

                        spriteBatch.Draw(mIncorreto, new Rectangle(400, 270, 250, 250), Color.White);
                    }
                    spriteBatch.DrawString(sFontMedia, "Volte as mãos na posição", new Vector2(200, 650), Color.White);
                    spriteBatch.Draw(mCorpo, new Rectangle(820, 140, 173, 480), Color.White);

                }
            }
            else if (Cena == 10)//fim do jogo
            {
                spriteBatch.Draw(mLousa, new Rectangle(150, 140, 746, 490), Color.White);
                spriteBatch.DrawString(sfontPequena, "Fim do jogo", new Vector2(250, 200), Color.White);

                int posicaoTexto = 240;

                for (int i = 0; i < _ResultadoFinal.Count; i++) // Loop through List with for
                {
                    spriteBatch.DrawString(sfontCourierPequena, (string)_ResultadoFinal[i].ToString(), new Vector2(250, posicaoTexto + (i * 20)), Color.White);
                }

                spriteBatch.DrawString(sfontPequena, "Resultado:", new Vector2(580, 200), Color.White);
                spriteBatch.DrawString(sfontPequena, "Acertos=" + _ResultadoFinalAcerto.ToString(), new Vector2(580, 250), Color.White);
                spriteBatch.DrawString(sfontPequena, "Erros  =" + _ResultadoFinalErro.ToString(), new Vector2(580, 300), Color.White);
                if (UsandoKinect == true)
                {
                    spriteBatch.DrawString(sfontPequena, "Coloque a mão DIREITA acima da cabeça para reiniciar", new Vector2(100, 640), Color.Black);
                }
            }
            
            spriteBatch.End();


            base.Draw(gameTime);
        }

        void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if (skeletons == null)
                    {
                        skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(skeletons);

                    trackedSkeleton1 = skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
                }
            }

        }
    }

}
