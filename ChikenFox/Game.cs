using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChikenFox
{
    class Game
    {
        //типы клеток
        public static int UNAVAILABLE = -1; 
        public static int EMPTY = 0;    
        public static int CHIKEN = 1;    
        public static int FOX = 2;     

        public static int SIZE = 7; //размер поля

        public int[,] pole; //матрица игрового поля

        public bool isEnd;  //не закончена ли игра

        public Game()
        {
            isEnd = false;

            //создаем поле
            pole = new int[SIZE, SIZE];
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                {
                    //углы недоступны
                    if (i < 2 && j < 2 || i < 2 && j >= 5 || i >= 5 && j < 2 || i >= 5 && j >= 5)
                        pole[i, j] = UNAVAILABLE;
                    else if (i > 2) //курицы
                        pole[i, j] = CHIKEN;
                    else if (i == 2 && (j == 2 || j == 4))
                        pole[i, j] = FOX;   //лисы
                    else
                        pole[i, j] = EMPTY; //остальное пусты клетки
                }
        }

        public bool playerMove(int selX, int selY, int x, int y)
        {
            //если одна из клеток за границами поля
            if (x < 0 || x >= Game.SIZE || y < 0 || y >= Game.SIZE ||
                selX < 0 || selX >= Game.SIZE || selY < 0 || selY >= Game.SIZE)
                return false;

            //если выделена не курица
            if (pole[selY, selX] != CHIKEN)
                return false;

            //если кликнули не по пустой клетке
            if (pole[y, x] != EMPTY)
                return false;

            //если ходят на 1 клетку и в положенную сторону
            if (Math.Abs(x - selX) == 1 && selY == y ||
                x == selX && selY - y == 1)
            {
                //перемещениие
                pole[y, x] = pole[selY, selX];  
                pole[selY, selX] = EMPTY;       
                return true;
            }
            
            return false;
        }

        public int isGameEnd()
        {
            int cnt = 0;    
            int cntEnd = 0; 

            //считавание всех куриц
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                {
                    if (pole[i, j] == CHIKEN)
                    {
                        cnt++;

                        //если курица дошла до нужной части
                        if (j >= 2 && j <= 4 && i <= 2)
                            cntEnd++;
                    }
                    
                }

            //если дошли
            if (cntEnd == 9)
            {
                //победа
                isEnd = true;
                return 1;
            }
            //если кур меньше 9
            if (cnt < 9)
            {
                //поражение
                isEnd = true;
                return 2;
            }

            return 0;
        }

        public void compMove()
        {
            List<StepVariant> eats = new List<StepVariant>(); 
            for (int i=0; i<SIZE; i++)
                for (int j=0; j<SIZE; j++)
                {
                    if (pole[i, j] == FOX)
                    {
                        StepVariant step = new StepVariant(j, i);
                        eats.Add(findMaxKillLength(j, i, pole, step));
                    }
                }
            //выбор лучшей последовательности ходов
            StepVariant choose = chooseStep(eats);
            if (choose != null)
            {
                doStep(choose);
                return;
            }

            //поииск лучшшей клетки
            eats.Clear();
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                {
                    if (pole[i, j] == EMPTY)
                    {
                        StepVariant step = new StepVariant(j, i);
                        eats.Add(findMaxKillLength(j, i, pole, step));
                    }
                }

            //выбор лучшей клетки
            choose = chooseStep(eats);
            if (choose != null)
            {
                //поиск лис
                for (int i = 0; i < SIZE; i++)
                    for (int j = 0; j < SIZE; j++)
                    {
                        if (pole[i, j] == FOX)
                        { 
                            //проверка, может ли лиса дойти до нужной клетки
                            if (choose.x < j && foxWalk(j, i, -1, 0))
                                return;
                            else if (choose.x > j && foxWalk(j, i, 1, 0))
                                return;
                            else if (choose.y < i && foxWalk(j, i, 0, -1))
                                return;
                            else if (choose.y > i && foxWalk(j, i, 0, 1))
                                return;
                        }
                    }

                
            }

            //если нет такойи, то
            //ищем любой случайны ход
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                {
                    if (pole[i, j] == FOX)
                    {
                        if (foxWalk(j, i, 1, 0) ||
                            foxWalk(j, i, -1, 0) ||
                            foxWalk(j, i, 0, 1) ||
                            foxWalk(j, i, 0, -1))
                            return;
                    }
                }
        }

        private bool foxWalk(int x, int y, int dx, int dy)
        {
            //проверка 
            if (x + dx < 0 || x + dx >= SIZE || y + dy < 0 || y + dy >= SIZE)
                return false;

            //если не пустая
            if (pole[y + dy, x + dx] != EMPTY)
                return false;

            //ход
            pole[y + dy, x + dx] = FOX;
            pole[y, x] = EMPTY;

            return true;
        }

        private StepVariant chooseStep(List<StepVariant> eats)
        {
            StepVariant choose = null;
            foreach (StepVariant stepVariant in eats)
            {
                //если длина  не нулевая
                if (stepVariant.steps.Count > 0)
                {
                    if (choose == null)
                        choose = stepVariant;
                    else
                    {
                        //ищем с максимальной длиной
                        if (choose.steps.Count < stepVariant.steps.Count)
                            choose = stepVariant;
                    }
                }
            }

            return choose;
        }

        private StepVariant findMaxKillLength(int x, int y, int[,] pole, StepVariant steps)
        {
            
            int[,] pole1 = (int[,])pole.Clone();
            int[,] pole2 = (int[,])pole.Clone();
            int[,] pole3 = (int[,])pole.Clone();
            int[,] pole4 = (int[,])pole.Clone();

            //список цепочек вариантов ходов
            List<StepVariant> stepVariants = new List<StepVariant>();

            //проверяем все направления движения
            if (foxCanKillDist(x, y, 2, 0, pole1))
            {
                StepVariant newSteps = steps.Clone();   
                newSteps.steps.Add(new Point(x + 2, y));  
                stepVariants.Add(findMaxKillLength(x + 2, y, pole1, newSteps)); 
            }

            if (foxCanKillDist(x, y, -2, 0, pole2))
            {
                StepVariant newSteps = steps.Clone();
                newSteps.steps.Add(new Point(x - 2, y));
                stepVariants.Add(findMaxKillLength(x - 2, y, pole2, newSteps));
            }

            if (foxCanKillDist(x, y, 0, 2, pole3))
            {
                StepVariant newSteps = steps.Clone();
                newSteps.steps.Add(new Point(x, y+2));
                stepVariants.Add(findMaxKillLength(x, y + 2, pole3, newSteps));
            }

            if (foxCanKillDist(x, y, 0, -2, pole4))
            {
                StepVariant newSteps = steps.Clone();
                newSteps.steps.Add(new Point(x, y-2));
                stepVariants.Add(findMaxKillLength(x, y - 2, pole4, newSteps));
            }

            //поиск лусчшей цепочки  
            StepVariant best = null;
            foreach (StepVariant step in stepVariants)
            {
                if (best == null || step!=null && best.steps.Count < step.steps.Count)
                    best = step;
            }

            //если нет
            if (best == null)
                return steps;

            return best;
        }

        private bool foxCanKillDist(int x, int y, int dx, int dy, int[,] pole)
        {
            //проверка 
            if (x + dx < 0 || x + dx >= SIZE || y + dy < 0 || y + dy >= SIZE)
                return false;

            if (pole[y + dy, x + dx] != EMPTY)
                return false;

            if (pole[y + dy/2, x + dx/2] != CHIKEN)
                return false;

            //выполняем
            pole[y + dy, x + dx] = FOX;
            pole[y + dy / 2, x + dx / 2] = EMPTY;
            pole[y, x] = EMPTY;

            return true;
        }

        private void doStep(StepVariant choose)
        {
            int x = choose.x;
            int y = choose.y;
            
            //идем по всей цепочки
            for (int i=0; i<choose.steps.Count; i++)
            {
                pole[y, x] = EMPTY;
                pole[(y + choose.steps[i].Y) / 2, (x + choose.steps[i].X) / 2] = EMPTY;
                pole[choose.steps[i].Y, choose.steps[i].X] = FOX;
                x = choose.steps[i].X;
                y = choose.steps[i].Y;
            }
        }
    }
}
