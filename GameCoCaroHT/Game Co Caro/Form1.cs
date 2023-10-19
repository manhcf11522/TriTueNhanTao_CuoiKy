using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_Co_Caro
{
    public partial class Form1 : Form
    {
        private Label[,] Map;
        private static int columns, rows;
        
        private int player;
        private bool gameover;
        private bool vsComputer;
        private int[,] vtMap;
        private Stack<Chess> chesses;
        private Chess chess;

        public Form1()
        {
            //Rong va dai cua ban co
            columns = 20;
            rows = 17;

            vsComputer = false;
            gameover = false;
            player = 1;          
            Map = new Label[rows + 2, columns + 2];
            vtMap = new int[rows + 2, columns + 2];
            chesses = new Stack<Chess>();
            InitializeComponent();

            BuildTable();
        }

        private void BuildTable()
        {
            for (int i = 2; i <= rows; i++)
                for (int j = 1; j <= columns; j++)
                {
                    Map[i, j] = new Label();
                    Map[i, j].Parent = pnTableChess;
                    Map[i, j].Top = i * Contain.edgeChess;
                    Map[i, j].Left = j * Contain.edgeChess;
                    Map[i, j].Size = new Size(Contain.edgeChess-1, Contain.edgeChess-1);
                    Map[i, j].BackColor = Color.Snow;
                    //them event click
                    Map[i, j].MouseLeave += Form1_MouseLeave;
                    Map[i, j].MouseEnter += Form1_MouseEnter;
                    Map[i, j].Click += Form1_Click;
                }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
            {

                if (gameover)
                    return;
                Label lb = (Label)sender;
                int x = lb.Top / Contain.edgeChess - 1, y = lb.Left / Contain.edgeChess;
                if (vtMap[x, y] != 0)
                    return;
                if (vsComputer)
                {
                    player = 1;
                    psbCooldownTime.Value = 0;
                    tmCooldown.Start();
                    lb.Image = Properties.Resources.o;
                    vtMap[x, y] = 1;
                    Check(x, y);
                    CptFindChess();
                }
                else
                {
                    if (player == 1)
                    {
                        psbCooldownTime.Value = 0;
                        tmCooldown.Start();
                        lb.Image = Properties.Resources.o;
                        vtMap[x, y] = 1;
                        Check(x, y);

                        player = 2;
                        ptbPayer.Image = Properties.Resources.x_copy;
                        txtNamePlayer.Text = "Player2";
                    }
                    else
                    {
                        psbCooldownTime.Value = 0;
                        lb.Image = Properties.Resources.x;
                        vtMap[x, y] = 2;
                        Check(x, y);

                        player = 1;
                        ptbPayer.Image = Properties.Resources.onnnn;
                        txtNamePlayer.Text = "Player1";
                    }
                }
                chess = new Chess(lb, x, y);
                chesses.Push(chess);
            }
            else
            {
                MessageBox.Show("Hay Chon Mode");
            }
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            if (gameover)
                return;
            Label lb = (Label)sender;
            lb.BackColor = Color.AliceBlue;
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            if (gameover)
                return;
            Label lb = (Label)sender;
            lb.BackColor = Color.Snow;
        }

        
        private void tmCooldown_Tick(object sender, EventArgs e)
        {
            psbCooldownTime.PerformStep();
            psbCooldownTime.Maximum=Contain.cooldown_step;
            if (psbCooldownTime.Value >= psbCooldownTime.Maximum)
            {
                Gameover();
            }
        }
      
        private void menuUndo_Click(object sender, EventArgs e)
        {
            if (!vsComputer)
            {
                Chess template = new Chess();
                template = chesses.Pop();
                template.lb.Image = null;
                vtMap[template.X, template.Y] = 0;
                psbCooldownTime.Value = 0;
                ChangePlayer();
            }
            else
            {
                Chess template = new Chess();
                template = chesses.Pop();
                template.lb.Image = null;
                vtMap[template.X, template.Y] = 0;

                template = chesses.Pop();
                template.lb.Image = null;
                vtMap[template.X, template.Y] = 0;

                psbCooldownTime.Value = 0;
                player = 1;
            }
        }

        private void menuQuit_Click_1(object sender, EventArgs e)
        {
            DialogResult dialog;
            dialog = MessageBox.Show("Bạn có chắc muốn thoát không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                this.Dispose();
                this.Close();
            }
        }

        private void player1VsPlayer2(object sender, EventArgs e)
        {
            vsComputer = false;
            gameover = false;
            psbCooldownTime.Value = 0;
            tmCooldown.Stop();
            pnTableChess.Controls.Clear();

            txtNamePlayer.Text = "";
            ptbPayer.Image = null;
            menuStrip1.Parent = pnTableChess;
            player = 1;
            Map = new Label[rows + 2, columns + 2];
            vtMap = new int[rows + 2, columns + 2];
            chesses = new Stack<Chess>();

            BuildTable();
        }
        
        private void PlayVsComputer(object sender, EventArgs e)// neu chon danh với máy thì khởi tạo giá trị ban đầu liên quan tới người chơi
        {
            vsComputer = true;// mở chế độ đánh với máy
            gameover = false;// game over trở lại giá trị fales nếu như new game
            psbCooldownTime.Value = 0;// giá tri khởi đầu để thanh thời gian đánh của phayer =0
            tmCooldown.Stop();//  dừng thời gian của thanh thời gian đánh của phayer
            pnTableChess.Controls.Clear();// xóa các nut đánh trước đó
            comboBox1.Enabled = true;// mở lại chon chế độ đánh nếu new game
            ptbPayer.Image = Properties.Resources.onnnn;// hình anh đạu diện cho phayer
            txtNamePlayer.Text = "Player";// tên phayer
            menuStrip1.Parent = pnTableChess;
            player = 1;// giá trị của phayer =1 , máy =0
            Map = new Label[rows + 2, columns + 2];// khởi tạo giá trị mặc định vi trí đánh của phayer trên map 
            vtMap = new int[rows + 2, columns + 2];// khởi tạo vị trí mặc định khi đánh của phayer trên map
            chesses = new Stack<Chess>();
            BuildTable();
        }

        private void Gameover()
        {
            tmCooldown.Stop();
            gameover = true;
            backgroundgameover();
        }
        private void backgroundgameover()
        {
            for (int i = 2; i <= rows; i++)
                for (int j = 1; j <= columns; j++)
                {
                    Map[i, j].BackColor = Color.Gray;
                }
        }

        private void ChangePlayer()// hàm đổi người đánh
        {
            if (player == 1)
            {
                player = 2;
                txtNamePlayer.Text = "Player2";
                ptbPayer.Image = Properties.Resources.x_copy;
            }
            else
            {
                player = 1;
                txtNamePlayer.Text = "Player1";
                ptbPayer.Image = Properties.Resources.onnnn;
            }
        }

        private void Check(int x, int y) //Hàm kiểm tra ngang, dọc, chéo chính, chéo phụ nếu ==5 thì overgame
        {
                int i = x - 1, j = y;//giảm biến x đi 1 để bắt đầu kiểm tra ngược xuống từ chỗ được đánh có giá trị x,y
                int column = 1, row = 1, mdiagonal = 1, ediagonal = 1;// đặt khởi đầu là 1 vì sẽ kiểm tra những chỗ có người đánh nên phải luôn bắt bằng 1
                while (vtMap[x, y] == vtMap[i, j] && i >= 0) // kiểm tra hàng dọc nếu vị trí đó có người đánh sẽ tăng cột dọc lên 1 và giảm chỉ số của x để kiểm tra dọc xuống
                {
                    column++;
                    i--;
                }
                i = x + 1;// tăng biến x lên 1 để bắt đầu kiểm tra ngược lên.
                while (vtMap[x, y] == vtMap[i, j] && i <= rows)// kiểm tra nếu vị trí có người đánh sẽ tăng biến cột dọc lên 1 và tăng chỉ số của x để kiểm tra dọc lên
                {
                    column++;
                    i++;
                }
                i = x; j = y - 1;// giảm y là giảm về hàng ngang để bắt kiếm kiểm tra hàng ngang từ phải qua trái từ cho có người đánh
                while (vtMap[x, y] == vtMap[i, j] && j >= 0)// kiểm tra hàng ngang nếu vị đó có người đánh sẽ tăng biết ngang lên 1 và giảm chỉ số của y để kiếm tra theo hàng ngang từ trái qua phải
                {
                    row++;
                    j--;
                }
                j = y + 1;// tăng chỉ số y để kiếm tra từ trái qua phải bắt đầu từ chỗ được đánh
                while (vtMap[x, y] == vtMap[i, j] && j <= columns) // kiểm tra hàng ngang nếu vị đó có người đánh sẽ tăng biết ngang lên 1 và tăng chỉ số của y để kiếm tra theo hàng ngang từ phải qua trái
                {
                    row++;
                    j++;
                }
                i = x - 1; j = y - 1;// giảm x đi 1 và y đi 1 để kiểm tra chéo chính theo hướng đi xuống
                while (vtMap[x, y] == vtMap[i, j] && i >= 0 && j >= 0)
                {
                    mdiagonal++;
                    i--;
                    j--;
                }
                i = x + 1; j = y + 1;// tăng x lên 1 và y lên 1 để kiểm tra chéo chính theo hướng đi lên
                while (vtMap[x, y] == vtMap[i, j] && i <= rows && j <= columns)
                {
                    mdiagonal++;
                    i++;
                    j++;
                }
                i = x - 1; j = y + 1; // giảm x đi 1 và y đi 1 để kiểm tra chéo phụ theo hướng đi xuống
                while (vtMap[x, y] == vtMap[i, j] && i >= 0 && j <= columns)
                {
                    ediagonal++;
                    i--;
                    j++;
                }
                i = x + 1; j = y - 1; // tăng x lên 1 và y lên 1 để kiểm tra chéo phụ theo hướng đi lên
                while (vtMap[x, y] == vtMap[i, j] && i <= rows && j >= 0)
                {
                    ediagonal++;
                    i++;
                    j--;
                }
                if (row >= 5 || column >= 5 || mdiagonal >= 5 || ediagonal >= 5)// kiểm tra giá trị doc, ngang, chéo chính chéo phụ nếu bằng 5 sẽ kết thúc game
                {
                    Gameover();
                    if (vsComputer)
                    {
                        if (player == 1)//xuất kết quả thắng thua giữa người và máy theo điều kiện lượt đánh hiện tại là của ai
                            MessageBox.Show("You win!!");
                        else
                            MessageBox.Show("You lost!!");
                    }
                    else
                    {
                        if (player == 1)//xuất kết quả thắng thua giữa người chơi với nhau theo điều kiện lượt đánh hiện tại là của ai
                            MessageBox.Show("Player1 Win");
                        else
                            MessageBox.Show("Player2 Win");
                    }
                }

            
        }


        
        #region AI
        //0, 9, 54, 162, 1458, 13112, 118008 // chi so quyet dinh se phong thu hay danh cua AI
        //0, 3, 27, 99, 729, 6561, 59049
        private int[] Attack = new int[7] { 0, 9, 54, 162, 1458, 13112, 118008 };
        private int[] Defense = new int[7] { 0, 3, 27, 99, 729, 6561, 59049 };

        private void PutChess(int x, int y)// Tới lượt đánh của máy
        {
            player = 0;
            psbCooldownTime.Value = 0;          
            Map[x+1, y].Image = Properties.Resources.x;// thêm quân cờ trên danh sách
            vtMap[x, y] = 2;// vị trí của may đánh trên ban cờ
            Check(x, y);// kiểm tra xem win chưa.
            chess = new Chess(Map[x+1, y], x, y);//tạo cờ trên map hiển thị
            chesses.Push(chess);
        }

        private void CptFindChess()// hàm tim vị trí đánh của máy
        {
            if (gameover) return;// nếu ket thuc game thi dừng
            long max = 0;// biến lưu giá trị lớn nhất giá trị các quân cờ
            int imax = 1, jmax = 1;
            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < columns; j++)
                    if (vtMap[i, j] == 0)
                    {
                        long temp = Caculate(i, j);//kiểm tra chỉ số Attack và def tai i j;
                        if (temp > max)// kiểm tra tam với chỉ số cao nhất.
                        {
                            max = temp;// gắn max bằng biến tạm nếu tạm có chỉ lớn hơn max
                            imax = i; jmax = j;// lưu lại vị trí có hỉ số cao nhất
                        }
                    }
            }
            
          
                PutChess(imax, jmax);// đánh cờ vào vị trí có def hoặc attack cao nhất đã lưu

            
        }
        private long Caculate(int x, int y)
        {
            return EnemyChesses(x, y) + ComputerChesses(x, y);
        }
        private long ComputerChesses(int x, int y)//kiểm tra chỉ số tấn công của máy // kiem tra các nước đánh của máy
        {
            int i = x - 1, j = y;
            int column = 0, row = 0, mdiagonal = 0, ediagonal = 0;
            int sc_ = 0, sc = 0, sr_ = 0, sr = 0, sm_ = 0, sm = 0, se_ = 0, se = 0;// các biến thử của máy  với các ô trống xung quanh các cờ đã được dánh
            while (vtMap[i, j] == 2 && i >= 0)// hàm kiểm tra như check thắng thua
            {
                column++;
                i--;
            }
            if (vtMap[i, j] == 0) sc_ = 1; // sử dụng các biến thử để đánh thử vào các ô trống quanh các quân cờ đã được đánh
            i = x + 1;
            while (vtMap[i, j] == 2 && i <= rows)
            {
                column++;
                i++;
            }
            if (vtMap[i, j] == 0) sc = 1;
            i = x; j = y - 1;
            while (vtMap[i, j] == 2 && j >= 0)
            {
                row++;
                j--;
            }
            if (vtMap[i, j] == 0) sr_ = 1;
            j = y + 1;
            while (vtMap[i, j] == 2 && j <= columns)
            {
                row++;
                j++;
            }
            if (vtMap[i, j] == 0) sr = 1;
            i = x - 1; j = y - 1;
            while (vtMap[i, j] == 2 && i >= 0 && j >= 0)
            {
                mdiagonal++;
                i--;
                j--;
            }
            if (vtMap[i, j] == 0) sm_ = 1;
            i = x + 1; j = y + 1;
            while (vtMap[i, j] == 2 && i <= rows && j <= columns)
            {
                mdiagonal++;
                i++;
                j++;
            }
            if (vtMap[i, j] == 0) sm = 1;
            i = x - 1; j = y + 1;
            while (vtMap[i, j] == 2 && i >= 0 && j <= columns)
            {
                ediagonal++;
                i--;
                j++;
            }
            if (vtMap[i, j] == 0) se_ = 1;
            i = x + 1; j = y - 1;
            while (vtMap[i, j] == 2 && i <= rows && j >= 0)
            {
                ediagonal++;
                i++;
                j--;
            }
            if (vtMap[i, j] == 0) se = 1;

            if (column == 4) column = 5;
            if (row == 4) row = 5;
            if (mdiagonal == 4) mdiagonal = 5;
            if (ediagonal == 4) ediagonal = 5;

            if (column == 3 && sc == 1 && sc_ == 1) column = 4;
            if (row == 3 && sr == 1 && sr_ == 1) row = 4;
            if (mdiagonal == 3 && sm == 1 && sm_ == 1) mdiagonal = 4;
            if (ediagonal == 3 && se == 1 && se_ == 1) ediagonal = 4;

            if (column == 2 && row == 2 && sc == 1 && sc_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (column == 2 && mdiagonal == 2 && sc == 1 && sc_ == 1 && sm == 1 && sm_ == 1) column = 3;
            if (column == 2 && ediagonal == 2 && sc == 1 && sc_ == 1 && se == 1 && se_ == 1) column = 3;
            if (row == 2 && mdiagonal == 2 && sm == 1 && sm_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (row == 2 && ediagonal == 2 && se == 1 && se_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (ediagonal == 2 && mdiagonal == 2 && sm == 1 && sm_ == 1 && se == 1 && se_ == 1) column = 3;

            long Sum = Attack[row] + Attack[column] + Attack[mdiagonal] + Attack[ediagonal];

            return Sum;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if(comboBox1.SelectedIndex == 0)
                Contain.cooldown_step = 10000;
            if (comboBox1.SelectedIndex == 1)
                Contain.cooldown_step = 5000;
            if (comboBox1.SelectedIndex == 2)
                Contain.cooldown_step = 3000;
            comboBox1.Enabled= false;
        }

        private long EnemyChesses(int x, int y)// kiểm tra chỉ số phòng thủ của máy // kiểm tra các nước đánh của người chơi
        {
            int i = x - 1, j = y;
            int sc_ = 0, sc = 0, sr_ = 0, sr = 0, sm_ = 0, sm = 0, se_ = 0, se = 0;// các biến thử của máy  với các ô trống xung quanh các cờ đã được dánh
            int column = 0, row = 0, mdiagonal = 0, ediagonal = 0;
            while (vtMap[i, j] == 1 && i >= 0)
            {
                column++;
                i--;
            }
            if (vtMap[i, j] == 0) sc_ = 1;
            i = x + 1;
            while (vtMap[i, j] == 1 && i <= rows)
            {
                column++;
                i++;
            }
            if (vtMap[i, j] == 0) sc = 1;
            i = x; j = y - 1;
            while (vtMap[i, j] == 1 && j >= 0)
            {
                row++;
                j--;
            }
            if (vtMap[i, j] == 0) sr_ = 1;
            j = y + 1;
            while (vtMap[i, j] == 1 && j <= columns)
            {
                row++;
                j++;
            }
            if (vtMap[i, j] == 0) sr = 1;
            i = x - 1; j = y - 1;
            while (vtMap[i, j] == 1 && i >= 0 && j >= 0)
            {
                mdiagonal++;
                i--;
                j--;
            }
            if (vtMap[i, j] == 0) sm_ = 1;
            i = x + 1; j = y + 1;
            while (vtMap[i, j] == 1 && i <= rows && j <= columns)
            {
                mdiagonal++;
                i++;
                j++;
            }
            if (vtMap[i, j] == 0) sm = 1;
            i = x - 1; j = y + 1;
            while (vtMap[i, j] == 1 && i >= 0 && j <= columns)
            {
                ediagonal++;
                i--;
                j++;
            }
            if (vtMap[i, j] == 0) se_ = 1;
            i = x + 1; j = y - 1;
            while (vtMap[i, j] == 1 && i <= rows && j >= 0)
            {
                ediagonal++;
                i++;
                j--;
            }
            if (vtMap[i, j] == 0) se = 1;

            if (column == 4) column = 5;
            if (row == 4) row = 5;
            if (mdiagonal == 4) mdiagonal = 5;
            if (ediagonal == 4) ediagonal = 5;

            if (column == 3 && sc == 1 && sc_ == 1) column = 4;
            if (row == 3 && sr == 1 && sr_ == 1) row = 4;
            if (mdiagonal == 3 && sm == 1 && sm_ == 1) mdiagonal = 4;
            if (ediagonal == 3 && se == 1 && se_ == 1) ediagonal = 4;

            if (column == 2 && row == 2 && sc == 1 && sc_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (column == 2 && mdiagonal == 2 && sc == 1 && sc_ == 1 && sm == 1 && sm_ == 1) column = 3;
            if (column == 2 && ediagonal == 2 && sc == 1 && sc_ == 1 && se == 1 && se_ == 1) column = 3;
            if (row == 2 && mdiagonal == 2 && sm == 1 && sm_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (row == 2 && ediagonal == 2 && se == 1 && se_ == 1 && sr == 1 && sr_ == 1) column = 3;
            if (ediagonal == 2 && mdiagonal == 2 && sm == 1 && sm_ == 1 && se == 1 && se_ == 1) column = 3;
            long Sum = Defense[row] + Defense[column] + Defense[mdiagonal] + Defense[ediagonal];

            return Sum;
        }
        #endregion 

    }

    public class Chess
    {
        public Label lb;
        public int X;
        public int Y;
        public Chess()
        {
            lb = new Label();
        }
        public Chess(Label _lb, int x, int y)
        {
            lb = new Label();
            lb = _lb;
            X = x;
            Y = y;
        }
    }
}
