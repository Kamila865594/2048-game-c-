using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp6
{
    public partial class Form1 : Form
    {
        private int[,] board = new int[4, 4];
        private Random random = new Random();
        private const int tileSize = 100;
        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(420, 500);
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            StartNewGame();
        }
        private void StartNewGame()
        {
            Array.Clear(board, 0, board.Length);
            AddRandomTile();
            AddRandomTile();
            Invalidate();
        }

        private void AddRandomTile()
        {
            int emptyCount = board.Cast<int>().Count(x => x == 0);
            if (emptyCount == 0) return;

            int index = random.Next(0, emptyCount);
            for (int i = 0; i < 16; i++)
            {
                if (board[i / 4, i % 4] == 0)
                {
                    if (index == 0)
                    {
                        board[i / 4, i % 4] = random.Next(1, 3) * 2; // 2 или 4
                        break;
                    }
                    index--;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawBoard(e.Graphics);
        }

        private void DrawBoard(Graphics g)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Rectangle rect = new Rectangle(j * tileSize + 10, i * tileSize + 10, tileSize - 20, tileSize - 20);
                    g.FillRectangle(GetTileColor(board[i, j]), rect);
                    if (board[i, j] != 0)
                    {
                        g.DrawString(board[i, j].ToString(), new Font("Arial", 24), Brushes.Black,
                            rect.X + rect.Width / 2 - g.MeasureString(board[i, j].ToString(), new Font("Arial", 24)).Width / 2,
                            rect.Y + rect.Height / 2 - g.MeasureString(board[i, j].ToString(), new Font("Arial", 24)).Height / 2);
                    }
                }
            }
        }

        private Brush GetTileColor(int value)
        {
            switch (value)
            {
                case 0: return Brushes.LightGray;
                case 2: return Brushes.LightYellow;
                case 4: return Brushes.LightGoldenrodYellow;
                case 8: return Brushes.LightCoral;
                case 16: return Brushes.Salmon;
                case 32: return Brushes.Tomato;
                case 64: return Brushes.Red;
                case 128: return Brushes.YellowGreen;
                case 256: return Brushes.GreenYellow;
                case 512: return Brushes.LimeGreen;
                case 1024: return Brushes.Cyan;
                case 2048: return Brushes.BlueViolet;
                default: return Brushes.Black;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            bool moved = false;

            switch (e.KeyCode)
            {
                case Keys.W:
                    moved = MoveUp();
                    break;
                case Keys.S:
                    moved = MoveDown();
                    break;
                case Keys.A:
                    moved = MoveLeft();
                    break;
                case Keys.D:
                    moved = MoveRight();
                    break;
            }

            if (moved)
            {
                AddRandomTile();
                Invalidate();

                if (IsGameOver())
                {
                    MessageBox.Show("Game Over!");
                    StartNewGame();
                }
            }
        }

        private bool MoveUp()
        {
            bool moved = false;

            for (int col = 0; col < 4; col++)
            {
                int[] temp = new int[4];

                for (int row = 0; row < 4; row++)
                    temp[row] = board[row, col];

                moved |= Merge(temp);

                for (int row = 0; row < temp.Length; row++)
                    board[row, col] = temp[row];
            }

            return moved;
        }

        private bool MoveDown()
        {
            bool moved = false;

            for (int col = 0; col < 4; col++)
            {
                int[] temp = new int[4];

                for (int row = 0; row < temp.Length; row++)
                    temp[row] = board[3 - row, col];

                moved |= Merge(temp);

                for (int row = temp.Length - 1; row >= 0; row--)
                    board[3 - row, col] = temp[row];
            }

            return moved;
        }

        private bool MoveLeft()
        {
            bool moved = false;

            for (int row = 0; row < 4; row++)
            {
                int[] temp = new int[4];
                for (int col = 0; col < temp.Length; col++)
                    temp[col] = board[row, col];

                moved |= Merge(temp);

                for (int col = 0; col < temp.Length; col++)
                    board[row, col] = temp[col];
            }

            return moved;
        }

        private bool MoveRight()
        {
            bool moved = false;

            for (int row = 0; row < 4; row++)
            {
                int[] temp = new int[4];

                for (int col = 0; col < temp.Length; col++)
                    temp[col] = board[row, temp.Length - 1 - col];

                moved |= Merge(temp);

                for (int col = 0; col < temp.Length; col++)
                    board[row, temp.Length - 1 - col] = temp[col];
            }

            return moved;
        }

        private bool Merge(int[] line)
        {
            // Сжимаем массив
            line.ToList().RemoveAll(x => x == 0);

            // Объединяем одинаковые элементы
            bool merged = false;

            for (int i = line.Length - 1; i > 0; i--)
            {
                if (line[i] == line[i - 1])
                {
                    line[i] *= 2;
                    line[i - 1] = 0;
                    merged = true;
                }
            }

            // Сжимаем массив снова
            line.ToList().RemoveAll(x => x == 0);

            // Заполняем оставшиеся нули
            while (line.Length < 4)
            {
                Array.Resize(ref line, line.Length + 1);
                line[line.Length - 1] = 0;
            }

            return merged;
        }

        private bool IsGameOver()
        {
            // Проверка на наличие пустых клеток или возможность слияния
            if (board.Cast<int>().Any(x => x == 0)) return false;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == board[i, j + 1]) return false;
                    if (board[j, i] == board[j + 1, i]) return false;
                }
            }

            return true;
        }
    }
}
    
    

