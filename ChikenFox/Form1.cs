using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChikenFox
{
    public partial class Form1 : Form
    {
        Game game;  //класс игры
        //координаты выделенной клетки
        int selectX = -1;
        int selectY = -1;
        Pen selectPen;  //pen для выделения клетки
        int dx; //смещение поля, для расположения его по центру формы
        int rectSize;   //размер клетки

        public Form1()
        {
            game = new Game();

            InitializeComponent();
            
            //создаем ручку синего цвета с шириной 3 пикселя
            selectPen = new Pen(Brushes.Pink, 5);

        }

        //рисование игрового поля
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //ищем минимальный размер
            int minSize = panel1.Width;
            if (panel1.Height < minSize)
                minSize = panel1.Height;
            //размер клетки
            rectSize = minSize / Game.SIZE;
            //смещение поля
            dx = (panel1.Width - rectSize * Game.SIZE) / 2;
            int r = 2;  //отступ для рисования картинок, чтобы не наезжали на рамки
            
            //очищаем поле
            e.Graphics.Clear(Color.White);

            //рисуем клетки
            for (int i = 0; i < Game.SIZE; i++)
                for (int j = 0; j < Game.SIZE; j++)
                {
                    //рисуем клетки
                    if (game.pole[i, j] != Game.UNAVAILABLE)
                        e.Graphics.DrawRectangle(Pens.Black, dx + j * rectSize, i * rectSize, rectSize, rectSize);

                    //рисуем куриц
                    if (game.pole[i, j] == Game.CHIKEN)
                        e.Graphics.DrawImage(Properties.Resources.chiken, dx + j * rectSize + r, i * rectSize + r, rectSize - r, rectSize - r);
                    else if (game.pole[i, j] == Game.FOX)   //рисуем лис
                        e.Graphics.DrawImage(Properties.Resources.lisa, dx + j * rectSize + r, i * rectSize + r, rectSize - r, rectSize - r);
               
                    //выделенная клетка
                    if (selectX == j && selectY == i)
                        e.Graphics.DrawRectangle(selectPen, dx + j * rectSize, i * rectSize, rectSize, rectSize);
                }
        }

        //обновление формы
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            panel1.Invalidate();
        }

        //при изменении размеров формы, обновляем 
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            panel1.Invalidate();
        }

        private void panel1_Click(object sender, EventArgs e)
        {

        }

        //клик по игровому полю
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            //если игра закончена
            if (game.isEnd)
                return; //выходим

            //получаем клетку, по которой кликнули
            int x = e.X;
            int y = e.Y;

            x -= dx;

            x /= rectSize;
            y /= rectSize;

            //если можно сходить
            if (game.playerMove(selectX, selectY, x, y))
            {
                //снимаем выделение
                selectX = -1;
                selectY = -1;

                //проверяем на конец игры
                int gameRes = game.isGameEnd();
                if (gameRes == 0)   //если не конец
                {
                    game.compMove();    //ходят лисы
                    gameRes = game.isGameEnd(); //проверяем на конец игры
                    if (gameRes == 1)
                        MessageBox.Show("Победа!");
                    else if (gameRes == 2)
                        MessageBox.Show("Поражение!");
                }
                else if (gameRes == 1)
                    MessageBox.Show("Победа!");
                else if (gameRes == 2)
                    MessageBox.Show("Поражение!");
            }
            else
            {
                //если кликнули по клетке с курицей
                if (x >= 0 && x < Game.SIZE && y >= 0 && y < Game.SIZE && game.pole[y, x] == Game.CHIKEN)
                {
                    //выделяем ее
                    selectX = x;
                    selectY = y;
                }
                else
                {
                    //иначе снимаем выделение
                    selectX = -1;
                    selectY = -1;
                }                
            }
                       
            panel1.Invalidate();    //обновляем поле
        }

    

        private void button1_Click(object sender, EventArgs e)
        {
            game = new Game();  //начинаем новую игру
            panel1.Invalidate();    //обновляем форму
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
