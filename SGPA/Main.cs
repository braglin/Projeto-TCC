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

        int tipoUsuario = 4;
        int projetoAtivo = 0;

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
        private void novoProjBtn_Click(object sender, EventArgs e)
        {
            LimparTela();
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

            listaProjetos.ClearSelected();
        }

        private void SalvarProjetoBtn_Click(object sender, EventArgs e)
        {
            if (listaProjetos.SelectedItem == null)
            {
                salvarNovoProjeto.Visible = true;
            }
        }

        private void KanbanBtn_Click(object sender, EventArgs e)
        {
            projetosScr.Visible = false;
            kanbanScr.Visible = true;
        }

        private void BomBtn_Click(object sender, EventArgs e)
        {
            projetosScr.Visible = false;
            bomScr.Visible = true;
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
        #endregion

        #region BOM

        #endregion

        #region Estoque

        #endregion





        

        











        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void salvarNovoProjetoBtn_Click(object sender, EventArgs e)
        {
            erroNovoProjetoLbl.Visible = false;
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
                    projetoAtivo= Convert.ToInt32(sqlcmd.ExecuteScalar());
                    con.Close();
                }
            }


            salvarNovoProjeto.Visible = false;
            CarregarListaProjetos();
        }
    }
}
