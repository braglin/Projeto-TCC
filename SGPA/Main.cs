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
        bool bomCollapsed = false;

        public Main()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
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
                    kanbanAddCardBtn.Visible = true;
                    novoProjBtn.Visible = true;
                    listaProjetos.Location = new Point(8, 84);
                    listaProjetos.Height = 674;
                }
                else if (tipoUsuario == 1)
                {
                    tipoUsuarioTxt.Text = "Comprador";
                    fotoUsuario.Image = imageList1.Images[2];
                }
                else if (tipoUsuario == 2)
                {
                    tipoUsuarioTxt.Text = "Desenvolvedor";
                    fotoUsuario.Image = imageList1.Images[1];
                    cadastrarBtn.Visible = false;
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

            foreach (Control gb in this.Controls)
            {
                if (gb.Name.ToString().Contains("Scr"))
                {
                    gb.Visible = false;
                }
            }

            if (btn.Name == "projetosBtn")
            {
                projetosScr.Visible = true;
                projetosScr.Enabled = true;
                CarregarListaProjetos();
            }
            else if  (btn.Name == "estoqueBtn")
            {
                estoqueScr.Visible = true;
                estoqueScr.Enabled = true;
                CarregarEstoque("");
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

        private void CancelarNovoProjetoBtn_Click(object sender, EventArgs e)
        {
            salvarNovoProjeto.Visible = false;
        }
        private void KanbanBtn_Click(object sender, EventArgs e)
        {
            projetosScr.Visible = false;
            kanbanScr.Visible = true;

            CarregarCartao();
        }

        private void BomBtn_Click(object sender, EventArgs e)
        {
            try
            {
                projetosScr.Visible = false;
                bomScr.Visible = true;
                
                projetoNomeBomTxt.Text = listaProjetos.SelectedItem.ToString();
                
                string query = "SELECT revisao FROM dbo.bom WHERE projid = " + projetoAtivo + " GROUP BY revisao";
                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
                {
                    using (SqlCommand sqlcmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        SqlDataReader reader = sqlcmd.ExecuteReader();
                        while (reader.Read())
                        {
                            revCb.Items.Add(String.Format("{0}", reader[0]));
                        }
                        reader.Close();
                        con.Close();
                        revCb.SelectedIndex = revCb.Items.Count - 1;
                    }
                }
                
                CarregaBOM();
            }
            catch (Exception)
            {}
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
            cartaoMenu.Name = cartao.Name;
            cartaoMenu.ContextMenuStrip = this.contextMenuStrip1;
            cartaoMenu.FlatAppearance.BorderSize = 0;
            cartaoMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cartaoMenu.Image = ((System.Drawing.Image)(Resources.cardMenu));
            cartaoMenu.Location = new System.Drawing.Point(205, 90);
            cartaoMenu.Size = new System.Drawing.Size(22, 23);
            cartaoMenu.UseVisualStyleBackColor = true;

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

        public void CarregarCartao()
        {
            foreach (Control gb in this.Controls)
            {
                foreach (Control fp in gb.Controls)
                {
                    fp.GetType();

                    if (fp.Name.ToString().Contains("flowLayoutPanel"))
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

            try
            {
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
            catch (Exception)
            { }
            
        }

        private void moverPendenteBtn_Click(object sender, EventArgs e)
        {
            cartaoAtivo = (contextMenuStrip1.SourceControl.Name.ToString());

            string query = "UPDATE dbo.kanban SET categoria = '0' WHERE id = " + cartaoAtivo.Replace("cartao", "") + " AND projid = " + projetoAtivo;
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    sqlcmd.ExecuteNonQuery();
                    TrocarColunaKanban(flowLayoutPanel1);
                    con.Close();
                }
            }
        }

        private void moverAndamentoBtn_Click(object sender, EventArgs e)
        {
            cartaoAtivo = (contextMenuStrip1.SourceControl.Name.ToString());

            string query = "UPDATE dbo.kanban SET categoria = '1' WHERE id = " + cartaoAtivo.Replace("cartao", "") + " AND projid = " + projetoAtivo;
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    sqlcmd.ExecuteNonQuery();
                    TrocarColunaKanban(flowLayoutPanel2);
                    con.Close();
                }
            }
        }

        private void moverFinalizadoBtn_Click(object sender, EventArgs e)
        {
            cartaoAtivo = (contextMenuStrip1.SourceControl.Name.ToString());

            string query = "UPDATE dbo.kanban SET categoria = '2' WHERE id = " + cartaoAtivo.Replace("cartao", "") + " AND projid = " + projetoAtivo;
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    sqlcmd.ExecuteNonQuery();
                    TrocarColunaKanban(flowLayoutPanel3);
                    con.Close();
                }
            }
        }

        private void TrocarColunaKanban(FlowLayoutPanel newfp)
        {
            foreach (Control gb in this.Controls)
            {
                foreach (Control fp in gb.Controls)
                {
                    foreach (Control card in fp.Controls)
                    {
                        if (card.Name.ToString() == cartaoAtivo.ToString())
                        {
                            fp.Controls.Remove(card);
                            newfp.Controls.Add(card);
                            break;
                        }
                    }
                }
            }
        }

        private void removerBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Você realmente quer apagar este cartão?", "Cuidado!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                string query = "DELETE FROM dbo.kanban WHERE id = " + Convert.ToInt32(cartaoAtivo.Replace("cartao", "")) + " AND projig = '" + projetoAtivo + "'";
                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
                {
                    using (SqlCommand sqlcmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        sqlcmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                foreach (Control gb in this.Controls)
                {
                    foreach (Control fp in gb.Controls)
                    {
                        foreach (Control card in fp.Controls)
                        {
                            if (card.Name.ToString() == cartaoAtivo.ToString())
                            {
                                fp.Controls.Remove(card);
                                break;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region BOM
        public void CarregaBOM()
        {
            string query = "SELECT * FROM dbo.bom WHERE projid = '" + projetoAtivo + "' AND revisao = '" + revCb.Text + "'";

            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = sqlcmd.ExecuteReader();

                    bomDGV.Rows.Clear();

                    while (reader.Read())
                    {
                        string[] row = new string[] {
                            (String.Format("{0}", reader[2]) != "0" ? String.Format("{0}", reader[2]) : ""),
                            (String.Format("{0}", reader[3]) != "0" ? String.Format("{0}", reader[3]) : ""),
                            (String.Format("{0}", reader[4]) != "0" ? String.Format("{0}", reader[4]) : ""),
                            String.Format("{0}", reader[5]),
                            String.Format("{0}", reader[6]),
                            String.Format("{0}", reader[7]),
                            String.Format("{0}", reader[8]),
                            String.Format("{0}", reader[9]),
                            String.Format("{0}", reader[10]),
                            String.Format("{0}", reader[11])
                        };
                        bomDGV.Rows.Add(row);
                    }
                    reader.Close();
                    con.Close();
                }
            }

            System.Threading.Thread.Sleep(2000);
            foreach (DataGridViewRow row in bomDGV.Rows)
            {
                try
                {
                    if (Convert.ToInt32(row.Cells[0].Value) != 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void salvarBomBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Realizar upload da BOM para o servidor?\n\n*Essa ação resultará em uma nova revisão da BOM!", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;

                foreach (DataGridViewRow row in bomDGV.Rows)
                {
                    try
                    {
                        int number;

                        bool level1 = int.TryParse(row.Cells[0].Value?.ToString(), out number);
                        bool level2 = int.TryParse(row.Cells[1].Value?.ToString(), out number);
                        bool level3 = int.TryParse(row.Cells[2].Value?.ToString(), out number);

                        if (!(ConverterNumero(row.Cells[0].Value?.ToString(), level1) is Int32))
                        {
                            throw new ArgumentException("Inconsistência nos dados (Coluna 0, Linha " + (row.Index + 1).ToString() + ")!");
                        }
                        if (!(ConverterNumero(row.Cells[1].Value?.ToString(), level2) is Int32))
                        {
                            throw new ArgumentException("Inconsistência nos dados (Coluna 1, Linha " + (row.Index + 1).ToString() + ")!");
                        }
                        if (!(ConverterNumero(row.Cells[2].Value?.ToString(), level3) is Int32))
                        {
                            throw new ArgumentException("Inconsistência nos dados (Coluna 2, Linha " + (row.Index + 1).ToString() + ")!");
                        }

                        if (row.Cells[0].Value?.ToString() == "")
                        {
                            if (row.Cells[3].Value?.ToString() == "" || row.Cells[3].Value?.ToString() == " ")
                            {
                                throw new ArgumentException("Inconsistência nos dados na coluna Código, Linha " + (row.Index + 1).ToString() + "!");
                            }
                        }

                        if (row.Cells[4].Value?.ToString() == "" || row.Cells[4].Value?.ToString() == " ")
                        {
                            throw new ArgumentException("Inconsistência nos dados na coluna Descrição, Linha " + (row.Index + 1).ToString() + "!");
                        }

                        if (row.Cells[0].Value?.ToString() == "")
                        {
                            if (row.Cells[5].Value?.ToString() == "" || row.Cells[5].Value?.ToString() == " ")
                            {
                                throw new ArgumentException("Inconsistência nos dados na coluna Quantidade, Linha " + (row.Index + 1).ToString() + "!");
                            }
                        }

                        if (row.Cells[0].Value?.ToString() == "")
                        {
                            if (row.Cells[6].Value?.ToString() == "" || row.Cells[6].Value?.ToString() == " ")
                            {
                                throw new ArgumentException("Inconsistência nos dados na coluna Unidade, Linha " + (row.Index + 1).ToString() + "!");
                            }
                        }

                        if (row.Cells[8].Value?.ToString() == "" || row.Cells[8].Value?.ToString() == " ")
                        {
                            throw new ArgumentException("Inconsistência nos dados na coluna Designer, Linha " + (row.Index + 1).ToString() + "!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Cursor.Current = Cursors.Default;
                        MessageBox.Show(ex.Message, "Erro na operação!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                foreach (DataGridViewRow row in bomDGV.Rows)
                {
                    string query = "INSERT INTO dbo.bom (projid, um, dois, tres, codigo, descricao, qtd, unidade, observacao, designer, revisao) VALUES (";
                    try
                    {
                        int number;

                        bool level1 = int.TryParse(row.Cells[0].Value?.ToString(), out number);
                        bool level2 = int.TryParse(row.Cells[1].Value?.ToString(), out number);
                        bool level3 = int.TryParse(row.Cells[2].Value?.ToString(), out number);

                        query += projetoAtivo + ",";
                        query += ConverterNumero(row.Cells[0].Value?.ToString(), level1) + ",";
                        query += ConverterNumero(row.Cells[1].Value?.ToString(), level2) + ",";
                        query += ConverterNumero(row.Cells[2].Value?.ToString(), level3) + ",'";
                        query += row.Cells[3].Value?.ToString() + "','";
                        query += row.Cells[4].Value?.ToString() + "',";
                        query += Convert.ToDouble(row.Cells[5].Value?.ToString().Replace(",", ".")) + ",'";
                        query += row.Cells[6].Value?.ToString() + "','";
                        query += row.Cells[7].Value?.ToString() + "','";
                        query += row.Cells[8].Value?.ToString() + "',";
                        query += (Convert.ToDouble(revCb.Text) + 0.1).ToString() + ")";

                        using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
                        {
                            using (SqlCommand sqlcmd = new SqlCommand(query, con))
                            {
                                con.Open();
                                sqlcmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Cursor.Current = Cursors.Default;
                        MessageBox.Show(ex.Message + "\n Linha: " + (row.Index + 1).ToString() + "\n\n" + query, "Erro na operação!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                revCb.Text = (Convert.ToDouble(revCb.Text) + 0.1).ToString();
                Cursor.Current = Cursors.Default;
                MessageBox.Show("As alterações foram salvas com sucesso!", "Operação Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public int ConverterNumero(string value, bool flag)
        {
            if (flag)
            {
                return Convert.ToInt32(value);
            }
            else
            {
                return 0;
            }
        }

        private void revCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregaBOM();
        }

        private void adicionarNovaLinhaBom_Click(object sender, EventArgs e)
        {
            try
            {
                if (bomDGV.Rows.Count > 0)
                {
                    bomDGV.Rows.Insert(bomDGV.CurrentRow.Index + 1);
                }
                else
                {
                    bomDGV.Rows.Insert(0);
                }
            }
            catch (Exception)
            { }
        }

        private void removerLinhaSelecionadaBom_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Remover a linha selecionada da BOM?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                bomDGV.Rows.RemoveAt(bomDGV.CurrentRow.Index);
            }
        }

        private void bomDGV_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip2.Show(Cursor.Position);
            }
        }

        private void colapsarBtn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in bomDGV.Rows)
            {
                try
                {
                    if (row.Cells[0].Value == null || row.Cells[0].Value == DBNull.Value || row.Cells[0].Value.ToString() == " " || String.IsNullOrWhiteSpace(row.Cells[0].Value.ToString()))
                    {
                        if (bomCollapsed == false)
                        {
                            row.Visible = false;
                        }
                        else
                        {
                            row.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                }

            }
            if (bomCollapsed == false)
            {
                bomCollapsed = true;
            }
            else
            {
                bomCollapsed = false;
            }
        }

        private void subirBom_Click(object sender, EventArgs e)
        {
            if (bomDGV.RowCount > 0)
            {
                if (bomDGV.SelectedRows.Count > 0)
                {
                    int rowCount = bomDGV.Rows.Count;
                    int index = bomDGV.SelectedCells[0].OwningRow.Index;

                    if (index == 0)
                    {
                        return;
                    }
                    DataGridViewRowCollection rows = bomDGV.Rows;

                    DataGridViewRow prevRow = rows[index - 1];
                    rows.Remove(prevRow);
                    prevRow.Frozen = false;
                    rows.Insert(index, prevRow);
                    bomDGV.ClearSelection();
                    bomDGV.Rows[index - 1].Selected = true;
                    bomDGV.Rows[index - 1].Cells[1].Value = Convert.ToInt32(bomDGV.CurrentRow.Cells[1].Value) - 1;
                    bomDGV.Rows[index].Cells[1].Value = Convert.ToInt32(bomDGV.CurrentRow.Cells[1].Value) + 1;
                }
            }
        }

        private void descerBom_Click(object sender, EventArgs e)
        {
            if (bomDGV.RowCount > 0)
            {
                if (bomDGV.SelectedRows.Count > 0)
                {
                    int rowCount = bomDGV.Rows.Count;
                    int index = bomDGV.SelectedCells[0].OwningRow.Index;

                    if (index == (rowCount - 2))
                    {
                        return;
                    }
                    DataGridViewRowCollection rows = bomDGV.Rows;

                    DataGridViewRow nextRow = rows[index + 1];
                    rows.Remove(nextRow);
                    nextRow.Frozen = false;
                    rows.Insert(index, nextRow);
                    bomDGV.ClearSelection();
                    bomDGV.Rows[index + 1].Selected = true;

                    if (bomDGV.Rows[index + 1].Cells[1].Value.ToString() != "" || bomDGV.Rows[index + 1].Cells[1].Value.ToString() != " ")
                    {
                        bomDGV.Rows[index + 1].Cells[1].Value = Convert.ToInt32(bomDGV.CurrentRow.Cells[1].Value) + 1;
                        bomDGV.Rows[index].Cells[1].Value = Convert.ToInt32(bomDGV.CurrentRow.Cells[1].Value) - 1;
                    }
                }
            }
        }

        private void sairBom_Click(object sender, EventArgs e)
        {
            bomScr.Visible = false;
            projetosScr.Visible = true;
        }


        #endregion

        #region Estoque
        private void CarregarEstoque(string desc)
        {
            string query = "SELECT codigo, descricao, fornecedor, estoque, locacao FROM dbo.estoque WHERE descricao LIKE '%" + desc + "%'";
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = sqlcmd.ExecuteReader();

                    estoqueDGV.Rows.Clear();

                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            string[] row = new string[] {
                            String.Format("{0}", reader[0]),
                            String.Format("{0}", reader[1]),
                            String.Format("{0}", reader[2]),
                            String.Format("{0}", reader[3]),
                            String.Format("{0}", reader[4])
                            };
                            estoqueDGV.Rows.Add(row);
                        }
                    }
                    reader.Close();
                    con.Close();
                }
            }
        }
        private void cadastrarFecharBtn_Click(object sender, EventArgs e)
        {
            cadastrarItemScr.Visible = false;
        }

        private void cadastrarBtn_Click(object sender, EventArgs e)
        {
            cadastrarItemScr.Visible = true;
        }

        private void retirarBtn_Click(object sender, EventArgs e)
        {
            retirarItemScr.Visible = true;
        }

        private void fecharRetirarBtn_Click(object sender, EventArgs e)
        {
            retirarItemScr.Visible = false;
        }

        private void retirarItemBtn_Click(object sender, EventArgs e)
        {
            string query = "UPDATE dbo.estoque SET estoque = estoque - " + int.Parse(retItemQtd.Value.ToString()) + " WHERE codigo = '" + retItemCodigoTxt.Text.ToUpper() + "'";
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    sqlcmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            CarregarEstoque("");
            MessageBox.Show("Item atualizado com sucesso!", "SGPA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            retirarItemScr.Visible = false;
            LimpaTelaRetItem();
        }

        private void LimpaTelaRetItem()
        {
            retItemCodigoTxt.Clear();
            retItemDescricaoTxt.Clear();
            retItemFornecedorTxt.Clear();
            retItemEstoqueTxt.Text = "";
            retItemLocacaoTxt.Text = "";
            retItemQtd.Value = 0;
        }

        private void cadItemCodigoTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string query = "SELECT descricao, fornecedor, estoque, locacao FROM dbo.estoque WHERE codigo = '" + cadItemCodigoTxt.Text.ToUpper() + "'";
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
                                cadItemDescricaoTxt.Text = String.Format("{0}", reader[0]);
                                cadItemFornecedorTxt.Text = String.Format("{0}", reader[1]);
                                cadItemEstoqueTxt.Text = String.Format("{0}", reader[2]);
                                cadItemLocacaoTxt.Text = String.Format("{0}", reader[3]);
                            }
                        }
                        reader.Close();
                        con.Close();
                    }
                }
            }
        }

        private void cadastraItemBtn_Click(object sender, EventArgs e)
        {
            string query = "SELECT id FROM dbo.estoque WHERE codigo = '" + cadItemCodigoTxt.Text.ToUpper() + "'";
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = sqlcmd.ExecuteReader();

                    query = "INSERT INTO dbo.estoque (codigo, descricao, fornecedor, estoque, locacao) VALUES ('" + cadItemCodigoTxt.Text.ToUpper() + "', '" + cadItemDescricaoTxt.Text + "', '" + cadItemFornecedorTxt.Text + "', " + Convert.ToDouble(cadItemQtd.Value) + ", '" + cadItemLocacaoTxt.Text + "')";

                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            int itemId = Convert.ToInt32(String.Format("{0}", reader[0]));
                            query = "UPDATE dbo.estoque SET estoque = " + (Convert.ToDouble(cadItemQtd.Value) + Convert.ToDouble(cadItemEstoqueTxt.Text)) + " WHERE id = " + itemId;
                        }
                    }
                    reader.Close();
                    con.Close();
                }
            }

            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=sgpa;Trusted_Connection=True;"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(query, con))
                {
                    con.Open();
                    sqlcmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            CarregarEstoque("");
            MessageBox.Show("Item cadastrado/atualizado com sucesso!", "SGPA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            cadastrarItemScr.Visible = false;
            LimpaTelaCadItem();
        }

        private void estBuscaBtn_Click(object sender, EventArgs e)
        {
            CarregarEstoque(descTxtBox.Text);
        }

        private void LimpaTelaCadItem()
        {
            cadItemCodigoTxt.Clear();
            cadItemDescricaoTxt.Clear();
            cadItemFornecedorTxt.Clear();
            cadItemEstoqueTxt.Text = "";
            cadItemLocacaoTxt.Text = "";
            cadItemQtd.Value = 0;
        }

        private void retItemCodTxt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string query = "SELECT descricao, fornecedor, estoque, locacao FROM dbo.estoque WHERE codigo = '" + retItemCodigoTxt.Text.ToUpper() + "'";
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
                                retItemDescricaoTxt.Text = String.Format("{0}", reader[0]);
                                retItemFornecedorTxt.Text = String.Format("{0}", reader[1]);
                                retItemEstoqueTxt.Text = String.Format("{0}", reader[2]);
                                retItemLocacaoTxt.Text = String.Format("{0}", reader[3]);
                            }
                        }
                        reader.Close();
                        con.Close();
                    }
                }
            }
        }

        private void descTxtBox_KeyUp(object sender, KeyEventArgs e)
        {
            estBuscaBtn.PerformClick();
        }
        #endregion
         
    }
}
