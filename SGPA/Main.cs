using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using SGPA.Properties;

namespace SGPA
{
    public partial class Main : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]       
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        int tipoUsuario = 0;
        int projetoAtivo = 0;
        string cartaoAtivo = "";

        public Main()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //loginScr.Dock = DockStyle.Fill;
            //projetosScr.Dock = DockStyle.Fill;
            //kanbanScr.Dock = DockStyle.Fill;
        }

        #region Login
        private void senhaTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                loginBtn.PerformClick();
            }
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            loginErrorLbl.Visible = false;
            if ((usuarioTxt.Text.Length >= 5) && (senhaTxt.Text.Length >= 4))
            {
                string query = "SELECT tipo from users WHERE usuario = '" + usuarioTxt.Text + "' and senha = '" + senhaTxt.Text + "'";

                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
                {
                    using (SqlCommand sqlcmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        object resposta = sqlcmd.ExecuteScalar();
                        if (resposta != null)
                        {
                            tipoUsuario = (int)resposta;
                        }
                        else
                        {
                            loginErrorLbl.Visible = true;
                            return;
                        }
                    }
                }

                if (tipoUsuario == 0)
                {
                    tipoUsuarioTxt.Text = "Gerente";
                    fotoUsuario.Image = imageList1.Images[0];
                }
                else if (tipoUsuario == 1)
                {
                    tipoUsuarioTxt.Text = "Desenvolvedor";
                    fotoUsuario.Image = imageList1.Images[1];
                }
                else if (tipoUsuario == 2)
                {
                    tipoUsuarioTxt.Text = "Comprador";
                    fotoUsuario.Image = imageList1.Images[2];
                }
                else
                {
                    loginErrorLbl.Visible = true;
                    return;
                }

                loginScr.Visible = false;
                estoqueBtn.Visible = true;
                projetosBtn.Visible = true;
            }
            else
            {
                loginErrorLbl.Visible = true;
            }
        }
        #endregion


        #region Menu
        private void MenuBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            navPanel.Visible = true;
            navPanel.Height = btn.Height;
            navPanel.Top = btn.Top;
            navPanel.Left = btn.Left;
            btn.BackColor = Color.Silver;
            btn.ForeColor = Color.FromArgb(64,64,64);

            if (btn.Name == "projetosBtn")
            {
                projetosScr.Visible = true;
                projetosScr.Enabled = true;
                CarregarListaProjetos();
            }
            else if (btn.Name == "sairBtn")
            {
                DialogResult dialogResult = MessageBox.Show("Deseja mesmo sair?", "SGPA", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Close();
                }
            }
        }

        private void MenuBtn_Leave(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackColor = Color.FromArgb(64,64,64); 
            btn.ForeColor = Color.Silver;
        }
        #endregion

        #region Projetos
        private void novoProjBtn_Click(object sender, EventArgs e)
        {
            projetoAtivo = 0;
            LimparTela();
        }

        private void SalvarProjetoBtn_Click(object sender, EventArgs e)
        {
            if (projetoAtivo == 0)
            {
                salvarNovoProjeto.Visible = true;
            }
            else
            {
                AtualizarProjeto();
            }
        }

        private void KanbanBtn_Click(object sender, EventArgs e)
        {
            projetosScr.Visible = false;
            kanbanScr.Visible = true;
            CarregarCartao();
        }

        private void BomBtn_Click(object sender, EventArgs e)
        {
            projetosScr.Visible = false;
            bomScr.Visible = true;
        }
        private void listaProjetos_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listaProjetos.IndexFromPoint(e.Location) +1;
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                LimparTela();
                try
                {
                    string query = "SELECT dbo.projetos.id FROM dbo.projetos WHERE nome = '" + listaProjetos.SelectedItem.ToString() + "'";
                    using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
                    {
                        using (SqlCommand sqlcmd = new SqlCommand(query, con))
                        {
                            con.Open();
                            object resposta = sqlcmd.ExecuteScalar();
                            con.Close();
                            if (resposta != null)
                            {
                                projetoAtivo = (int)resposta;
                            }
                            else
                            {
                                return;
                            }                       
                        }
                    }


                    query = "SELECT * FROM dbo.projetos WHERE id = '" + index + "'";
                    using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
                    {
                        using (SqlCommand sqlcmd = new SqlCommand(query, con))
                        {
                            con.Open();
                            SqlDataReader reader = sqlcmd.ExecuteReader();
                            while (reader.Read())
                            {
                                projDescRtx.Text = String.Format("{0}", reader[2]);
                                projHistRtx.Text = String.Format("{0}", reader[3]);
                                projEquipeRtx.Text = String.Format("{0}", reader[4]);
                                anaViaCb.Checked = String.Format("{0}", reader[5]) == "1" ? true : false;
                                planCb.Checked = String.Format("{0}", reader[6]) == "1" ? true : false;
                                exeCb.Checked = String.Format("{0}", reader[7]) == "1" ? true : false;
                                monCb.Checked = String.Format("{0}", reader[8]) == "1" ? true : false;
                                encCb.Checked = String.Format("{0}", reader[9]) == "1" ? true : false;
                            }
                            reader.Close();
                            con.Close();
                        }
                    }
                }
                catch (Exception)
                {}                
            }
        }

        public void CarregarListaProjetos()
        {
            string query = "SELECT dbo.projetos.nome FROM dbo.projetos";
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    listaProjetos.Items.Clear();

                    SqlDataReader reader = sqlcmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listaProjetos.Items.Add(String.Format("{0}", reader[0]));
                    }
                    reader.Close();
                    con.Close();
                }
            }
        }

        public void LimparTela()
        {
            anaViaCb.Checked = false;
            planCb.Checked = false;
            exeCb.Checked = false;
            monCb.Checked = false;
            encCb.Checked = false;

            projDescRtx.Clear();
            projEquipeRtx.Clear();
            projHistRtx.Clear();

            projetoAtivo = 0;

           // listaProjetos.ClearSelected();
        }

        private void salvarNovoProjetoBtn_Click(object sender, EventArgs e)
        {
            erroNovoProjetoLbl.Visible = false;
            if (tipoUsuario != 1)
            {
                erroNovoProjetoLbl.Text = "Usuário inválido!";
                erroNovoProjetoLbl.Visible = true;
            }
            if ((novoProjetoNomeTxt.Text == "") || (novoProjetoNomeTxt.Text.Length < 5))
            {
                erroNovoProjetoLbl.Text = "Nome inválido... Tente novamente!";
                erroNovoProjetoLbl.Visible = true;
                return;
            }

            string query = "SELECT id FROM dbo.projetos WHERE nome = '" + novoProjetoNomeTxt.Text + "'";
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = sqlcmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            erroNovoProjetoLbl.Text = "Este projeto já existe!";
                            erroNovoProjetoLbl.Visible = true;
                            return;
                        }
                    }
                    reader.Close();
                    con.Close();
                }
            }

            query = "INSERT INTO dbo.projetos(nome, descricao, historico, equipe, analise, planejamento, execucao, monitoramento, encerramento) VALUES('" + novoProjetoNomeTxt.Text + "', '" + projDescRtx.Text + "', '" + projHistRtx.Text + "', '" + projEquipeRtx.Text + "', " + ((anaViaCb.Checked) ? 1 : 0).ToString() + ", " + ((planCb.Checked) ? 1 : 0).ToString() + ", " + ((exeCb.Checked) ? 1 : 0).ToString() + ", " + ((monCb.Checked) ? 1 : 0).ToString() + ", " + ((encCb.Checked) ? 1 : 0).ToString() + ")";
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    sqlcmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            query = "SELECT id FROM dbo.projetos Where nome = '" + novoProjetoNomeTxt.Text + "'";
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    projetoAtivo = Convert.ToInt32(sqlcmd.ExecuteScalar());
                    con.Close();
                }
            }

            salvarNovoProjeto.Visible = false;
            CarregarListaProjetos();
        }

        private void AtualizarProjeto()
        {
            DialogResult dialogResult = MessageBox.Show("Deseja salvar as alterações no projeto?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                string query = "UPDATE dbo.projetos SET descricao ='" + projDescRtx.Text +
                    "', historico ='" + projHistRtx.Text + "', equipe = '" + projEquipeRtx.Text +
                    "',analise = " + ((anaViaCb.Checked) ? 1 : 0).ToString() + ",planejamento = " + ((planCb.Checked) ? 1 : 0).ToString() + ", execucao = " + ((exeCb.Checked) ? 1 : 0).ToString() +
                    ", monitoramento = " + ((exeCb.Checked) ? 1 : 0).ToString() + ", encerramento = " + ((encCb.Checked) ? 1 : 0).ToString() + " WHERE id = " + projetoAtivo;

                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
                {
                    using (SqlCommand sqlcmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        sqlcmd.ExecuteNonQuery();
                        con.Close(); 
                        MessageBox.Show("Projeto atualizado com sucesso!", "SGPA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }
        #endregion

        #region Kanban
        private void KanbanAddCardBtn_Click(object sender, EventArgs e)
        {
            string idx = ("Done");
            novoCartaoScr.Visible = true;
        }
        private void NovoCartaoAdcBtn_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO dbo.kanban (projid, descricao, equipe, categoria) VALUES ('" + projetoAtivo + "', '" + cardDescRtxt.Text + "','" + cardEquipeTxt.Text + "', 0)";
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    sqlcmd.ExecuteNonQuery();
                    con.Close();
                    CarregarCartao();
                }
            }
        }
        private void NovoCartaoCloseBtn_Click(object sender, EventArgs e)
        {
            novoCartaoScr.Visible = false;
        }
        private void FecharKanbanBtn_Click(object sender, EventArgs e)
        {
            kanbanScr.Visible = false;
            projetosScr.Visible = true;
        }


        public void CriarCartao(String id, String descricao, String equipe, String categoria)
        {
            Panel cartao = new Panel();
            cartao.Size = new Size(232, 115);
            cartao.BorderStyle = BorderStyle.FixedSingle;
            cartao.BackColor = Color.Cornsilk;
            cartao.Name = "cartao" + id;

            Label cartaoDesc = new Label();
            cartaoDesc.Text = descricao;
            cartaoDesc.Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold);
            cartaoDesc.Location = new System.Drawing.Point(3, 0);
            cartaoDesc.AutoSize = false;
            cartaoDesc.Size = new Size(230, 80);


            Button cartaoMenu = new Button();
            cartaoMenu.ContextMenuStrip = this.contextMenuStrip1;
            cartaoMenu.FlatAppearance.BorderSize = 0;
            cartaoMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cartaoMenu.Image = ((System.Drawing.Image)(Resources.cardMenu));
            cartaoMenu.Location = new System.Drawing.Point(200, 90);
            cartaoMenu.Size = new System.Drawing.Size(22, 23);
            cartaoMenu.UseVisualStyleBackColor = true;
            cartaoMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cartaoMenu_Click);
            cartaoMenu.ContextMenuStrip = this.contextMenuStrip1;

            Label cartaoEquipe = new Label();
            cartaoEquipe.Name = "cardMembers";
            cartaoEquipe.Text = equipe;
            cartaoEquipe.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
            cartaoEquipe.Location = new System.Drawing.Point(3, 98);
            cartaoEquipe.AutoSize = false;
            cartaoEquipe.Size = new Size(195, 23);

            switch (categoria)
            {
                case "0":
                    flowLayoutPanel1.Controls.Add(cartao);
                    break;
                case "1":
                    flowLayoutPanel2.Controls.Add(cartao);
                    break;
                case "2":
                    flowLayoutPanel3.Controls.Add(cartao);
                    break;
                default:
                    break;
            }

            cartao.Controls.Add(cartaoDesc);
            cartao.Controls.Add(cartaoEquipe);
            cartao.Controls.Add(cartaoMenu);
        }

        private void cartaoMenu_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(this.PointToScreen(new Point(Cursor.Position.X - 5, Cursor.Position.Y - 40)));
            cartaoAtivo = ((Button)sender).Parent.Name.ToString();
        }

        public void CarregarCartao()
        {
            foreach (Control gb in this.Controls)
            {
                foreach (Control fp in gb.Controls)
                {
                    fp.GetType();

                    if (fp.GetType().ToString() == "flowLayoutPanel")
                    {
                        List<Control> cardlist = fp.Controls.Cast<Control>().ToList();
                        fp.Controls.Clear();
                        foreach (Control card in cardlist)
                        {
                            fp.Controls.Remove(card);
                        }
                    }
                }
            }
           
            string query = "SELECT id, descricao, equipe, categoria FROM dbo.kanban WHERE projid = " + projetoAtivo.ToString() + "";
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = sqlcmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            CriarCartao(String.Format("{0}", reader[0]), String.Format("{0}", reader[1]), String.Format("{0}", reader[2]), String.Format("{0}", reader[3]));
                        }
                    }
                    reader.Close();
                    con.Close();
                }
            }
        }














        #endregion

        #region BOM

        #endregion

        #region Estoque

        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            salvarNovoProjeto.Visible = false;
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
